// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Component;
using NakedObjects.Core.Util;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Translation;
using NakedObjects.Facade.Utility;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Web.Mvc;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class SessionCacheTest : AcceptanceTestCase {
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

                return types1.Union(types2).ToArray();
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
            config.UsingCodeFirstContext(() => new MvcTestContext("SessionCacheTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IOidTranslator, OidTranslatorSemiColonSeparatedList>(new PerResolveLifetimeManager());

        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            Database.Delete("SessionCacheTest");
        }

        private void SetupViewData() {
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoFramework] = NakedObjectsFramework;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoSurface] = Surface;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.IdHelper] = new IdHelper();
        }

        [Test]
        public void AddPersistentToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            session.AddObjectToSession(Surface, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(Surface, "key1"));
        }

        [Test]
        public void AddTransientToSession() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            var claim = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();
            session.AddObjectToSession(Surface, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(Surface, "key1"));
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
            GeneralExpense item1 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();
            session.AddObjectToSession(Surface, "key1", item1);
            session.AddObjectToSession(Surface, "key2", item2);
            Assert.AreEqual(item1, session.GetObjectFromSession<GeneralExpense>(Surface, "key1"));
            Assert.AreEqual(item2, session.GetObjectFromSession<AbstractExpenseItem>(Surface, "key2"));
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            GeneralExpense item1 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();
            session.AddObjectToSession(Surface, "key1", item1);
            session.AddObjectToSession(Surface, "key2", item2);
            Assert.IsNull(session.GetObjectFromSession<Claim>(Surface, "key1"));
            Assert.IsNull(session.GetObjectFromSession<Claim>(Surface, "key1"));
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
            Assert.IsNull(session.GetObjectFromSession<Claim>(Surface, "key1"));
        }

        [Test]
        public void RemoveObjectFromCache() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            session.AddObjectToSession(Surface, "key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>(Surface, "key1"));
            session.ClearFromSession("key1");
            Assert.IsNull(session.GetObjectFromSession<Claim>(Surface, "key1"));
        }

        [Test]
        public void RemoveValueFromCache() {
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
            session.AddValueToSession("key1", 1);
            Assert.AreEqual(1, session.GetValueFromSession<int>("key1"));
            session.ClearFromSession("key1");
            Assert.IsNull(session.GetValueFromSession<int>("key1"));
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

        protected IFrameworkFacade Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        protected override void StartTest() {
            base.StartTest();
            Surface = GetConfiguredContainer().Resolve<IFrameworkFacade>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
            MessageBroker = NakedObjectsFramework.MessageBroker;
        }

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            StartTest();
            RunFixturesOnce();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
            SetupViewData();
        }

        #endregion
    }
}