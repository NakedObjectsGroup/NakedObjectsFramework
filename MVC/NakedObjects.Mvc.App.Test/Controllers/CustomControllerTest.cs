// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Web.Mvc;
using AdventureWorksModel;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Util;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.DatabaseHelpers;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;
using System.Linq;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class CustomControllerTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
        }

        [TearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
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

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {new OrderContributedActions()}); }
        }


        protected override IObjectPersistorInstaller Persistor {
            get { return new EntityPersistorInstaller(); }
        }

        [TestFixtureSetUp]
        public void SetupFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }


        private static FormCollection GetForm(IDictionary<string, string> nameValues) {
            var form = new FormCollection();
            nameValues.ForEach(kvp => form.Add(kvp.Key, kvp.Value));
            return form;
        }


        private static INakedObject EmployeeRepo {
            get { return FrameworkHelper.GetAdaptedService("EmployeeRepository"); }
        }


        private static string EmployeeRepoId {
            get { return FrameworkHelper.GetObjectId(EmployeeRepo); }
        }


        private class CustomControllerWrapper : CustomController {
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


        [Test]
        public void InvokeActionByLambda() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            bool valid;
            Employee result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.RandomEmployee, new FormCollection(), out valid);
            Assert.IsNotNull(result);
            Assert.IsTrue(valid);
        }

        [Test]
        public void InvokeActionByLambdaWithInvalidParms() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            bool valid;
            IQueryable<Employee> result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, new FormCollection(), out valid);
            Assert.IsNull(result);
            Assert.IsFalse(valid);
        }

        [Test]
        public void InvokeActionByLambdaWithValidParms() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
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
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            bool valid;
            var result = controller.InvokeAction<Employee>(EmployeeRepo.Object, "RandomEmployee", new FormCollection(), out valid);
            Assert.IsNotNull(result);
            Assert.IsTrue(valid);
        }


        [Test]
        public void InvokeViewActionByLambda() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.RandomEmployee, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithInvalidParms() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepo.GetDomainObject<EmployeeRepository>(), x => x.FindEmployeeByName, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("FailView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithInvalidParmsWithOid() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction<EmployeeRepository, string, string, IQueryable<Employee>>(EmployeeRepoId, x => x.FindEmployeeByName, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("FailView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithOid() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction<EmployeeRepository, Employee>(EmployeeRepoId, x => x.RandomEmployee, new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByLambdaWithValidParms() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
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
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
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
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction(EmployeeRepo.Object, "RandomEmployee", new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }

        [Test]
        public void InvokeViewActionByNameWithOid() {
            var controller = new CustomControllerWrapper();
            new ContextMocks(controller);
            ViewResult result = controller.InvokeAction(EmployeeRepoId, "RandomEmployee", new FormCollection(), "FailView", "OKView");
            Assert.IsNotNull(result);
            Assert.AreEqual("OKView", result.ViewName);
        }
    }
}