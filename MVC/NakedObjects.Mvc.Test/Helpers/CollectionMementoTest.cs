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
using NakedObjects.Core.Util;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Helpers {
    public class DatabaseInitializer : DropCreateDatabaseAlways<MvcTestContext> {}

    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        private DummyController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get { return Types.Select(t => t.Namespace).Distinct().ToArray(); }
        }

        protected override Type[] Types {
            get {
                var types1 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("Expenses") && !t.FullName.Contains("Repository")).ToArray();

                var types2 = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "NakedObjects.Mvc.Test.Data").
                    GetTypes().Where(t => t.FullName.StartsWith("MvcTestApp.Tests.Helpers") && t.IsPublic).ToArray();

                var types3 = new[] {typeof (IEnumerable<Claim>)};

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

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("CollectionMementoTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            //InitializeNakedObjectsFramework(this);
            //RunFixtures();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            //CleanupNakedObjectsFramework(this);
            Database.Delete("CollectionMementoTest");
        }

        [Test]
        public void CollectionMemento() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm, cm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithEnum() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaimsByEnumStatus");
            INakedObjectAdapter[] parms = new[] {(object) ClaimStatusEnum.New}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = (IEncodedToStrings) CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);

            string[] strings = cm.ToEncodedStrings();
            var cm2 = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithNull() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = (IEncodedToStrings) CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithObject() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            var status = NakedObjectsFramework.Persistor.Instances<ClaimStatus>().First();
            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new object[] {status, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = (IEncodedToStrings) CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, strings);
            var claims = (IEnumerable) cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterAll() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            object[] selected = claims.Cast<object>().ToArray();

            var newCm = cm.NewSelectionMemento(selected, false);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(5, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterNone() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var newCm = cm.NewSelectionMemento(new object[] {}, false);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(0, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterOne() {
            mocks.ViewDataContainer.Object.ViewData["Services"] = NakedObjectsFramework.GetServices();

            INakedObjectAdapter service = NakedObjectsFramework.ServicesManager.GetService("ClaimRepository");
            IActionSpec action = service.Spec.GetActions().Single(a => a.Id == "FindMyClaims");
            INakedObjectAdapter[] parms = new[] {null, ""}.Select(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null)).ToArray();

            var cm = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, service, action, parms);
            var claims = (IEnumerable) cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var selected = new[] {claims.Cast<object>().First()};

            var newCm = cm.NewSelectionMemento(selected, false);

            var newClaims = (IEnumerable) newCm.RecoverCollection().Object;
            Assert.AreEqual(1, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        #region Nested type: DummyController

        private class DummyController : Controller {}

        #endregion

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
            SetUser("sven");

            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
        }

        #endregion
    }
}