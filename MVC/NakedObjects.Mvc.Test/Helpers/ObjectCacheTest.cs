// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
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
using NakedObjects.Architecture.Spec;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class ObjectCacheTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
            SetupViewData();
        }

       

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("MvcTest"));
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework();
            RunFixtures();
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        private DummyController controller;
        private ContextMocks mocks;

        #endregion

        private void SetupViewData() {
      
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = NakedObjectsFramework.GetServices();
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NoFramework] = NakedObjectsFramework;
        }

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

        [Test]
        public void AddNakedObjectToCache() {
            
            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void AddToCache() {
            

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
        }

        [Test]
        public void AllCachedObjects() {

            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == 2);
        }

        [Test]
        public void CacheLimit() {
          

            for (int i = 0; i < 200; i++) {
                INakedObject claim = GetTestService("Claims").GetAction("Create New Claim", typeof (string)).InvokeReturnObject(i.ToString()).NakedObject;
                
                mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            }

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == ObjectCache.CacheSize);
        }

        [Test]
        public void CachedObjectsOfBaseType() {


            GeneralExpense item1 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();

       

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (AbstractExpenseItem));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Count() == 2);
        }

        [Test]
        public void CachedObjectsOfDifferentType() {


            GeneralExpense item1 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().OrderBy(c => c.Id).First();
            GeneralExpense item2 = NakedObjectsFramework.ObjectPersistor.Instances<GeneralExpense>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, item2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item1));
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(item2));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Any());
        }

        [Test]
        public void CachedObjectsOfType() {

            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim1);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim2);

            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim));

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(claim1));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Contains(claim2));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.CachedObjectsOfType(NakedObjectsFramework, spec).Count() == 2);
        }

        [Test]
        public void DoNotAddDuplicates() {
       

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Count() == 1);
        }

        [Test]
        public void AddTransient() {
          

            var claim = NakedObjectsFramework.ObjectPersistor.CreateInstance(NakedObjectsFramework.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }

        [Test, Ignore] // temp ignore pending proper tests 
        public void AddCollection() {
           

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
        

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
        }

        [Test]
        public void RemoveOthersFromCache() {


            Claim claim1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim claim2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

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
         

            Claim claim = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First();

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }

        [Test]
        public void ClearDisposedFromCache() {


          
            var claim = NakedObjectsFramework.GetNakedObject( NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderByDescending(c => c.Id).First());

            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            NakedObjectsFramework.ObjectPersistor.StartTransaction();
            NakedObjectsFramework.ObjectPersistor.DestroyObject(claim);
            NakedObjectsFramework.ObjectPersistor.EndTransaction();

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void ClearNotExistentFromCache() {
         

            INakedObject claim = NakedObjectsFramework.ObjectPersistor.CreateInstance( NakedObjectsFramework.Reflector.LoadSpecification(typeof(Claim)));

            // mangle oid 
            new SimpleOidGenerator(NakedObjectsFramework.Reflector, 100).ConvertTransientToPersistentOid(claim.Oid);

            mocks.HtmlHelper.ViewContext.HttpContext.Session.TestAddToCache(NakedObjectsFramework, claim);
                    
            // object not found 

            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCache() {
           

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }

        [Test]
        public void ClearCache() {
         

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());
            mocks.HtmlHelper.ViewContext.HttpContext.Session.AddToCache(NakedObjectsFramework, claim);
            Assert.IsTrue(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));

            mocks.HtmlHelper.ViewContext.HttpContext.Session.ClearCachedObjects();
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
        }


        [Test]
        public void RemoveNakedObjectFromCacheNotThere() {
          

            INakedObject claim = NakedObjectsFramework.GetNakedObject(NakedObjectsFramework.ObjectPersistor.Instances<Claim>().First());

            mocks.HtmlHelper.ViewContext.HttpContext.Session.RemoveFromCache(NakedObjectsFramework, claim);
            Assert.IsFalse(mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Contains(claim.Object));
            Assert.IsTrue(!mocks.HtmlHelper.ViewContext.HttpContext.Session.AllCachedObjects(NakedObjectsFramework).Any());
        }


        [Test]
        public void SeperateCaches() {
          

            Claim c1 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderBy(c => c.Id).First();
            Claim c2 = NakedObjectsFramework.ObjectPersistor.Instances<Claim>().OrderByDescending(c => c.Id).First();

            INakedObject claim1 = NakedObjectsFramework.GetNakedObject(c1);
            INakedObject claim2 = NakedObjectsFramework.GetNakedObject(c2);

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