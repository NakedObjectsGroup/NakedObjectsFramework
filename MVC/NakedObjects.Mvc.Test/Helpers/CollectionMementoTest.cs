// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
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
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    public class DatabaseInitializer : DropCreateDatabaseAlways<MvcTestContext> {}


    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
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
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(this);
            RunFixtures();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            Database.Delete("CollectionMementoTest");
        }

        private DummyController controller;
        private ContextMocks mocks;


        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {new RecordedActionContributedActions()}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        private class DummyController : Controller {}

        [Test]
        public void CollectionMemento() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm, cm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithEnum() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaimsByEnumStatus");
            INakedObject[] parms = new[] {(object) ClaimStatusEnum.New}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);

            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithNull() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithObject() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            var status = NakedObjectsFramework.Persistor.Instances<ClaimStatus>().First();
            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new object[] {status, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }


        [Test]
        public void CollectionMementoWithFilterAll() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            object[] selected = claims.Cast<object>().ToArray();

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, cm, selected);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(5, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterNone() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, cm, new object[] { });

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(0, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterOne() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObject service = NakedObjectsFramework.Services.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetAllActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.LifecycleManager.CreateAdapter(o, null, null)).ToArray();

            var cm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var selected = new[] {claims.Cast<object>().First()};

            var newCm = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, cm, selected);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(1, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }
    }
}