// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;

using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    public class DatabaseInitializer : DropCreateDatabaseAlways<MvcTestContext> {}


    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        #region Setup/Teardown

        private static bool runFixtures;

        private void RunFixturesOnce() {
            if (!runFixtures) {
                RunFixtures();
                runFixtures = true;
            }
        }


        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            RunFixturesOnce();
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("CollectionMementoTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public  void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            //InitializeNakedObjectsFramework(this);
            //RunFixtures();
        }

        [TestFixtureTearDown]
        public  void TearDownTest() {
            //CleanupNakedObjectsFramework(this);
            Database.Delete("CollectionMementoTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository") ).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                var types3 = new Type[] {typeof (IEnumerable<Claim>)};

                return types1.Union(types2).Union(types3).ToArray();
            }
        }
    

        protected override object[] MenuServices {
            get { return (DemoServicesSet.ServicesSet()); }
        }

        protected override object[] ContributedActions {
            get { return (new object[] {new RecordedActionContributedActions()}); }
        }

        protected override object[] Fixtures {
            get { return (DemoFixtureSet.FixtureSet()); }
        }

        private class DummyController : Controller {}

        [Test]
        public void CollectionMemento() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm, cm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithEnum() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaimsByEnumStatus");
            INakedObject[] parms = new[] {(object) ClaimStatusEnum.New}.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);

            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithNull() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithObject() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            var status = NakedObjectsFramework.Persistor.Instances<ClaimStatus>().First();
            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new object[] { status, "" }.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }


        [Test]
        public void CollectionMementoWithFilterAll() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            object[] selected = claims.Cast<object>().ToArray();

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  cm, selected);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(5, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterNone() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  cm, new object[] { });

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(0, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterOne() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetObjectActions().Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var selected = new[] {claims.Cast<object>().First()};

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager,  NakedObjectsFramework.Metamodel,  cm, selected);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(1, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }
    }
}