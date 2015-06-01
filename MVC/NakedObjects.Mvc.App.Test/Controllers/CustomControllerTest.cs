// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Component;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Core.Util;
using NakedObjects.Facade;
using NakedObjects.Surface.Interface;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    //[Ignore]
    public class CustomControllerTest : AcceptanceTestCase {
        private CustomControllerWrapper controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get {
                return new[] {
                    "AdventureWorksModel", "MvcTestApp.Tests.Controllers"
                };
            }
        }

        protected override Type[] Types {
            get {
                return new Type[] {
                    typeof (ObjectQuery<string>)
                };
            }
        }

        protected override object[] MenuServices {
            get {
                return (new object[] {
                    new CustomerRepository(),
                    new OrderRepository(),
                    new ProductRepository(),
                    new EmployeeRepository(),
                    new SalesRepository(),
                    new SpecialOfferRepository(),
                    new ContactRepository(),
                    new VendorRepository(),
                    new PurchaseOrderRepository(),
                    new WorkOrderRepository(),
                    new SimpleRepository<NotPersistedObject>()
                });
            }
        }

        protected override object[] ContributedActions {
            get { return (new object[] {new OrderContributedActions()}); }
        }

        private INakedObjectAdapter EmployeeRepo {
            get { return NakedObjectsFramework.GetAdaptedService("EmployeeRepository"); }
        }

        private string EmployeeRepoId {
            get { return NakedObjectsFramework.GetObjectId(EmployeeRepo); }
        }

        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            StartTest();

            controller = new CustomControllerWrapper(Surface, new IdHelper());
            mocks = new ContextMocks(controller);
        }

        protected IFrameworkFacade Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        protected override void StartTest() {
            Surface = this.GetConfiguredContainer().Resolve<IFrameworkFacade>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
            MessageBroker = NakedObjectsFramework.MessageBroker;
        }


        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IOidTranslator, OidTranslatorSemiColonSeparatedList>(new PerResolveLifetimeManager());

        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        private static FormCollection GetForm(IDictionary<string, string> nameValues) {
            var form = new FormCollection();
            nameValues.ForEach(kvp => form.Add(kvp.Key, kvp.Value));
            return form;
        }

        [Test]
        public void InvokeActionByLambda() {
            bool valid;
            Employee result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.RandomEmployee, new FormCollection(), out valid);
            Assert.IsNotNull(result);
            Assert.IsTrue(valid);
        }

        [Test]
        public void InvokeActionByLambdaWithInvalidParms() {
            bool valid;
            IQueryable<Employee> result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, new FormCollection(), out valid);
            Assert.IsNull(result);
            Assert.IsFalse(valid);
        }

        [Test]
        public void InvokeActionByLambdaWithValidParms() {
            bool valid;
            FormCollection fc = GetForm(new Dictionary<string, string> {
                {"EmployeeRepository-FindEmployeeByName-FirstName-Input", ""},
                {"EmployeeRepository-FindEmployeeByName-LastName-Input", "Smith"}
            });
            IQueryable<Employee> result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, fc, out valid);
            Assert.IsNotNull(result);
            Assert.IsTrue(valid);
        }

        [Test]
        public void InvokeActionByName() {
            bool valid;
            var result = controller.InvokeAction<Employee>(EmployeeRepo.Object, "RandomEmployee", new FormCollection(), out valid);
            Assert.IsNotNull(result);
            Assert.IsTrue(valid);
        }

        [Test]
        public void InvokeViewActionByLambda() {
            ViewResult result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.RandomEmployee, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithInvalidParms() {
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("FailView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithInvalidParmsWithOid() {
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepoId, x => x.FindEmployeeByName, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("FailView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithOid() {
            ViewResult result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepoId, x => x.RandomEmployee, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithValidParms() {
            FormCollection fc = GetForm(new Dictionary<string, string> {
                {"EmployeeRepository-FindEmployeeByName-FirstName-Input", ""},
                {"EmployeeRepository-FindEmployeeByName-LastName-Input", "Smith"}
            });
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, fc, "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithValidParmsWithOid() {
            FormCollection fc = GetForm(new Dictionary<string, string> {
                {"EmployeeRepository-FindEmployeeByName-FirstName-Input", ""},
                {"EmployeeRepository-FindEmployeeByName-LastName-Input", "Smith"}
            });
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepoId, x => x.FindEmployeeByName, fc, "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByName() {
            ViewResult result = controller.InvokeAction(EmployeeRepo.Object, "RandomEmployee", new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByNameWithOid() {
            ViewResult result = controller.InvokeAction(EmployeeRepoId, "RandomEmployee", new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        #region Nested type: CustomControllerWrapper

        private class CustomControllerWrapper : CustomController {
            public CustomControllerWrapper(IFrameworkFacade surface,  IIdHelper idHelper) : base(surface, idHelper)  {}

            public new T InvokeAction<T>(object domainObject, string actionName, FormCollection parameters, out bool valid) {
                return base.InvokeAction<T>(domainObject, actionName, parameters, out valid);
            }

            public new TResult InvokeAction<TTarget, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TResult>>> expression, FormCollection parameters, out bool valid) {
                return base.InvokeAction(domainObject, expression, parameters, out valid);
            }

            public new TResult InvokeAction<TTarget, TParm, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm, TResult>>> expression, FormCollection parameters, out bool valid) {
                return base.InvokeAction(domainObject, expression, parameters, out valid);
            }

            public new TResult InvokeAction<TTarget, TParm1, TParm2, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TResult>>> expression, FormCollection parameters, out bool valid) {
                return base.InvokeAction(domainObject, expression, parameters, out valid);
            }

            public new TResult InvokeAction<TTarget, TParm1, TParm2, TParm3, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TParm3, TResult>>> expression, FormCollection parameters, out bool valid) {
                return base.InvokeAction(domainObject, expression, parameters, out valid);
            }

            public new TResult InvokeAction<TTarget, TParm1, TParm2, TParm3, TParm4, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, FormCollection parameters, out bool valid) {
                return base.InvokeAction(domainObject, expression, parameters, out valid);
            }

            public new ViewResult InvokeAction(object domainObject, string actionName, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, actionName, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget>(TTarget domainObject, Expression<Func<TTarget, Action>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm>(TTarget domainObject, Expression<Func<TTarget, Action<TParm>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2>(TTarget domainObject, Expression<Func<TTarget, Action<TParm1, TParm2>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2, TParm3>(TTarget domainObject, Expression<Func<TTarget, Action<TParm1, TParm2, TParm3>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2, TParm3, TParm4>(TTarget domainObject, Expression<Func<TTarget, Action<TParm1, TParm2, TParm3, TParm4>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2, TParm3, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TParm3, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<TTarget, TParm1, TParm2, TParm3, TParm4, TResult>(TTarget domainObject, Expression<Func<TTarget, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(domainObject, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction(string objectId, string actionName, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, actionName, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T>(string objectId, Expression<Func<T, Action>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm>(string objectId, Expression<Func<T, Action<TParm>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2>(string objectId, Expression<Func<T, Action<TParm1, TParm2>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2, TParm3>(string objectId, Expression<Func<T, Action<TParm1, TParm2, TParm3>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2, TParm3, TParm4>(string objectId, Expression<Func<T, Action<TParm1, TParm2, TParm3, TParm4>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TResult>(string objectId, Expression<Func<T, Func<TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm, TResult>(string objectId, Expression<Func<T, Func<TParm, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2, TResult>(string objectId, Expression<Func<T, Func<TParm1, TParm2, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2, TParm3, TResult>(string objectId, Expression<Func<T, Func<TParm1, TParm2, TParm3, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }

            public new ViewResult InvokeAction<T, TParm1, TParm2, TParm3, TParm4, TResult>(string objectId, Expression<Func<T, Func<TParm1, TParm2, TParm3, TParm4, TResult>>> expression, FormCollection parameters, String viewNameForFailure, string viewNameForSuccess = null) {
                return base.InvokeAction(objectId, expression, parameters, viewNameForFailure, viewNameForSuccess);
            }
        }

        #endregion
    }
}