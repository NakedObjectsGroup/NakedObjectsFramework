// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class SessionCacheTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
            SetupViewData();
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("SessionCacheTest"));
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
            Database.Delete("SessionCacheTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        private void SetupViewData() {
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NoFramework] = NakedObjectsFramework;
        }

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
        public void AddPersistentToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            Claim claim = NakedObjectsFramework.LifecycleManager.Instances<Claim>().First();
            session.AddObjectToSession(NakedObjectsFramework, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
        }

        [Test, Ignore] // revisit - value object is transient and so has no object id
        public void AddStringToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            const string testvalue = "test string";
            session.AddObjectToSession(NakedObjectsFramework, "key1", testvalue);
            Assert.AreEqual(testvalue, session.GetObjectFromSession<string>(NakedObjectsFramework, "key1"));
        }

        [Test]
        public void AddTransientToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            var claim = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();
            session.AddObjectToSession(NakedObjectsFramework, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
        }

        [Test]
        public void AddValueToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            const int testvalue = 99;
            session.AddValueToSession("key1", testvalue);
            Assert.AreEqual(testvalue, session.GetValueFromSession<int>("key1"));
        }

        [Test]
        public void CachedObjectsOfBaseType() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            GeneralExpense item1 = NakedObjectsFramework.LifecycleManager.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.LifecycleManager.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();
            session.AddObjectToSession(NakedObjectsFramework, "key1", item1);
            session.AddObjectToSession(NakedObjectsFramework, "key2", item2);
            Assert.AreEqual(item1, session.GetObjectFromSession<GeneralExpense>(NakedObjectsFramework, "key1"));
            Assert.AreEqual(item2, session.GetObjectFromSession<AbstractExpenseItem>(NakedObjectsFramework, "key2"));
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            GeneralExpense item1 = NakedObjectsFramework.LifecycleManager.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.LifecycleManager.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();
            session.AddObjectToSession(NakedObjectsFramework, "key1", item1);
            session.AddObjectToSession(NakedObjectsFramework, "key2", item2);
            Assert.IsNull(session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
            Assert.IsNull(session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
        }

        [Test]
        public void CachedValuesOfBaseType() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            session.AddValueToSession("key1", 1);
            Assert.AreEqual(1, session.GetValueFromSession<int>("key1"));
        }

        [Test]
        public void CachedValuesOfDifferentType() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            session.AddValueToSession("key1", 1);
            Assert.IsNull(session.GetValueFromSession<long>("key1"));
        }

        [Test]
        public void RemoveFromCacheNotThere() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            session.ClearFromSession("key1");
            Assert.IsNull(session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
        }

        [Test]
        public void RemoveObjectFromCache() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            Claim claim = NakedObjectsFramework.LifecycleManager.Instances<Claim>().First();
            session.AddObjectToSession(NakedObjectsFramework, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
            session.ClearFromSession("key1");
            Assert.IsNull(session.GetObjectFromSession<Claim>(NakedObjectsFramework, "key1"));
        }

        [Test]
        public void RemoveValueFromCache() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            session.AddValueToSession("key1", 1);
            Assert.AreEqual(1, session.GetValueFromSession<int>("key1"));
            session.ClearFromSession("key1");
            Assert.IsNull(session.GetValueFromSession<int>("key1"));
        }
    }
}