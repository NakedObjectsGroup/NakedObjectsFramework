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
using Expenses.ExpenseClaims.Items;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Component;
using NakedObjects.Core.Util;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Surface;
using NakedObjects.Surface.Interface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class ObjectCacheTest : AcceptanceTestCase {
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
            config.UsingCodeFirstContext(() => new MvcTestContext("ObjectCacheTest"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
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
            Database.Delete("ObjectCacheTest");
        }

        private void SetupViewData() {
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoFramework] = NakedObjectsFramework;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.NoSurface] = Surface;
            mocks.ViewDataContainer.Object.ViewData[IdConstants.IdHelper] = new IdHelper();
        }

        [Test]
        public void AddCollection() {
            var claimRepo = NakedObjectsFramework.GetServices().OfType<ClaimRepository>().FirstOrDefault();
            var claimRepoAdapter = NakedObjectsFramework.GetNakedObject(claimRepo);
            var claims = claimRepo.AllClaims();
            var claimsAdapter = NakedObjectsFramework.GetNakedObject(claims);
            var mockOid = CollectionMementoHelper.TestMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, claimRepoAdapter, claimRepoAdapter.GetActionLeafNode("AllClaims"), new INakedObjectAdapter[] {});

            claimsAdapter.SetATransientOid(mockOid);

            var ca = Surface.GetObject(claimsAdapter.Object);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, ca);
            Assert.IsTrue(((IEnumerable<object>) mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).First()).SequenceEqual(claims));
        }

        [Test]
        public void AddNakedObjectToCache() {
            var claim = Surface.GetObject(NakedObjectsFramework.Persistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));
        }

        [Test]
        public void AddToCache() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));
        }

        [Test]
        public void AddTransient() {
            var claim = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof (Claim))).GetDomainObject<Claim>();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Any());
        }

        [Test]
        public void AllCachedObjects() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Count() == 2);
        }

        [Test]
        public void CacheLimit() {
            var claims = NakedObjectsFramework.Persistor.Instances<Claim>().Where(c => c.Claimant.UserName == "dick");

            claims.ForEach(c => mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, c));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Count() == ObjectCache.CacheSize);
        }

        [Test]
        public void CachedObjectsOfBaseType() {
            GeneralExpense item1 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, item2);

            var spec = Surface.GetDomainType(typeof (AbstractExpenseItem).FullName);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(item1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(item2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Count() == 2);
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            GeneralExpense item1 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.Persistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, item2);

            var spec = Surface.GetDomainType(typeof(Claim).FullName);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(item1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(item2));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Any());
        }

        [Test]
        public void CachedObjectsOfType() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim2);

            var spec = Surface.GetDomainType(typeof(Claim).FullName);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(Surface, spec).Count() == 2);
        }

        [Test]
        public void ClearCache() {
            var claim = Surface.GetObject(NakedObjectsFramework.Persistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));
        }

        [Test]
        public void ClearDisposedFromCache() {
            var claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.Persistor.Instances<Claim>().OrderByDescending(c => c.Id).First());

            var cc = Surface.GetObject(claim.Object);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, cc);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));

            NakedObjectsFramework.TransactionManager.StartTransaction();
            NakedObjectsFramework.Persistor.DestroyObject(claim);
            NakedObjectsFramework.TransactionManager.EndTransaction();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));
        }

        [Test]
        public void DoNotAddDuplicates() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Count() == 1);
        }

        [Test]
        public void PurgesOldest() {
            var claims = NakedObjectsFramework.Persistor.Instances<Claim>().Where(c => c.Claimant.UserName == "dick").Take(ObjectCache.CacheSize + 1);

            Claim claim1 = null;
            Claim claim2 = null;
            foreach (var c in claims.OrderBy(c => c.Id)) {
                claim1 = claim1 ?? c;
                mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, c);
                claim2 = c;
            }
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Count() == ObjectCache.CacheSize);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim2));
        }

        [Test]
        public void RemoveFromCache() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(Surface, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));
        }

        [Test]
        public void RemoveFromCacheNotThere() {
            Claim claim = NakedObjectsFramework.Persistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(Surface, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Any());
        }

        [Test]
        public void RemoveNakedObjectFromCache() {
            var claim = Surface.GetObject(NakedObjectsFramework.Persistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(Surface, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));
        }

        [Test]
        public void RemoveNakedObjectFromCacheNotThere() {
            var claim = Surface.GetObject(NakedObjectsFramework.Persistor.Instances<Claim>().First());

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(Surface, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim.Object));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Any());
        }

        [Test]
        public void RemoveOthersFromCache() {
            Claim claim1 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim2));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveOthersFromCache(Surface, claim1);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim2));
        }

        [Test]
        public void SeperateCaches() {
            Claim c1 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim c2 = NakedObjectsFramework.Persistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            var claim1 = Surface.GetObject(c1);
            var claim2 = Surface.GetObject(c2);

            Assert.AreNotSame(claim1, claim2);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(Surface, claim2, ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim2.Object));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim1.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects(ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(Surface, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));
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

        protected INakedObjectsSurface Surface { get; set; }
        protected IMessageBroker MessageBroker { get; set; }

        protected override void StartTest() {
            base.StartTest();
            Surface = this.GetConfiguredContainer().Resolve<INakedObjectsSurface>();
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