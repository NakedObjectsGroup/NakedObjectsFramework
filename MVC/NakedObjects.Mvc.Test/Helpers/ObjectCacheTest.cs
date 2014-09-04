// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class ObjectCacheTest : AcceptanceTestCase {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();

        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public new void StartTest() {
            SetUser("sven");
            Fixtures.InstallFixtures(NakedObjectsFramework.ObjectPersistor, null);
        }

        [TearDown]
        public void EndTest() {
            MemoryObjectStore.DiscardObjects();
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions() }); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }

        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();

        [Test]
        public void AddNakedObjectToCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void AddToCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
        }

        [Test]
        public void AllCachedObjects() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == 2);
        }

        [Test]
        public void CacheLimit() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            for (int i = 0; i < 200; i++) {
                INakedObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof (string)).InvokeReturnObject(i.ToString()).NakedObject;
                mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            }

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == ObjectCache.CacheSize);
        }

        [Test]
        public void CachedObjectsOfBaseType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            GeneralExpense item1 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (AbstractExpenseItem));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Count() == 2);
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            GeneralExpense item1 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item2));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Any());
        }

        [Test]
        public void CachedObjectsOfType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Count() == 2);
        }

        [Test]
        public void DoNotAddDuplicates() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == 1);
        }

        [Test]
        public void AddTransient() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            var claim = NakedObjectsFramework.ObjectPersistor.CreateInstance(NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }

        [Test, Ignore] // temp ignore pending proper tests 
        public void AddCollection() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            var claim = NakedObjectsFramework.ObjectPersistor.CreateInstance(NakedObjectsFramework.Reflector.LoadSpecification(typeof(Claim))).GetDomainObject<Claim>();
            var claims = new List<Claim> {claim};
            var claimAdapter = NakedObjectsFramework.GetNakedObject(claim);
            var claimsAdapter = NakedObjectsFramework.GetNakedObject(claims);

            var mockOid = new CollectionMemento(NakedObjectsFramework.ObjectPersistor, NakedObjectsFramework.Reflector, NakedObjectsFramework.Session, claimAdapter, claimAdapter.GetActionLeafNode("ApproveItems"), new INakedObject[] { });

            claimsAdapter.SetATransientOid(mockOid);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claimsAdapter);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claims));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == 1);
        }


        [Test]
        public void PurgesOldest() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            var testObjects = new List<INakedObject>();

            for (int i = 0; i <= ObjectCache.CacheSize; i++) {
                INakedObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof (string)).InvokeReturnObject(i.ToString()).NakedObject;
                testObjects.Add(claim);
            }

            testObjects.ForEach(o => mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, o));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == ObjectCache.CacheSize);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(testObjects[0].Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(testObjects[ObjectCache.CacheSize].Object));
        }

        [Test]
        public void RemoveFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
        }

        [Test]
        public void RemoveOthersFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().Last();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim2));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveOthersFromCache(NakedObjectsFramework, claim1);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim2));
        }


        [Test]
        public void RemoveFromCacheNotThere() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }

        [Test]
        public void ClearDisposedFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().Last());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            NakedObjectsFramework.ObjectPersistor.StartTransaction();
            NakedObjectsFramework.ObjectPersistor.DestroyObject(claim);
            NakedObjectsFramework.ObjectPersistor.EndTransaction();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void ClearNotExistentFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.ObjectPersistor.CreateInstance( NakedObjectsFramework.Reflector.LoadSpecification(typeof(Claim)));

            // mangle oid 
            new SimpleOidGenerator(NakedObjectsFramework.Reflector, 100).ConvertTransientToPersistentOid(claim.Oid);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.TestAddToCache(NakedObjectsFramework, claim);
                    
            // object not found 

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void ClearCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCacheNotThere() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }


        [Test]
        public void SeperateCaches() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();

            INakedObject claim1 = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            INakedObject claim2 = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().Last());
            Assert.AreNotSame(claim1, claim2);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2, ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim2.Object));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim1.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1.Object));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects(ObjectCache.ObjectFlag.BreadCrumb);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework, ObjectCache.ObjectFlag.BreadCrumb).Contains(claim2.Object));
        }

    }
}