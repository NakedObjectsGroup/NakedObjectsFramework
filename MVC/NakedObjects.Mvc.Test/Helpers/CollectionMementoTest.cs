// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
            SetUser("sven");
        }

        [TearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
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

        private class DummyController : Controller { }

        private readonly Controller controller = new DummyController();

        [Test]
        public void CollectionMemento() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            var claims = (IEnumerable)cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm, cm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithNull() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(strings);
            var claims = (IEnumerable)cm2.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithObject() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            object status = NakedObjectsContext.ObjectPersistor.Instances(typeof(ClaimStatus)).Cast<object>().First();
            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { status, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(strings);
            var claims = (IEnumerable)cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoToStringWithEnum() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaimsByEnumStatus");
            INakedObject[] parms = new[] { (object)ClaimStatusEnum.New }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);

            string[] strings = cm.ToEncodedStrings();
            var cm2 = new CollectionMemento(strings);
            var claims = (IEnumerable)cm2.RecoverCollection().Object;
            Assert.AreEqual(2, claims.Cast<object>().Count());
            Assert.AreEqual(cm2, cm2.RecoverCollection().Oid);
        }



        [Test]
        public void CollectionMementoWithFilterAll() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            var claims = (IEnumerable)cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            object[] selected = claims.Cast<object>().ToArray();

            var newCm = new CollectionMemento(cm, selected);

            var newClaims = (IEnumerable)newCm.RecoverCollection().Object;
            Assert.AreEqual(5, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterNone() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            var claims = (IEnumerable)cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var newCm = new CollectionMemento(cm, new object[] { });

            var newClaims = (IEnumerable)newCm.RecoverCollection().Object;
            Assert.AreEqual(0, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }

        [Test]
        public void CollectionMementoWithFilterOne() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData["Services"] = FrameworkHelper.GetServices();

            INakedObject service = NakedObjectsContext.ObjectPersistor.GetService("ClaimRepository");
            INakedObjectAction action = service.Specification.GetObjectActions().Where(a => a.Id == "Find").SelectMany(a => a.Actions).Single(a => a.Id == "FindMyClaims");
            INakedObject[] parms = new[] { null, "" }.Select(PersistorUtils.CreateAdapter).ToArray();

            var cm = new CollectionMemento(service, action, parms);
            var claims = (IEnumerable)cm.RecoverCollection().Object;
            Assert.AreEqual(5, claims.Cast<object>().Count());

            var selected = new[] { claims.Cast<object>().First() };

            var newCm = new CollectionMemento(cm, selected);

            var newClaims = (IEnumerable)newCm.RecoverCollection().Object;
            Assert.AreEqual(1, newClaims.Cast<object>().Count());
            Assert.AreEqual(newCm, newCm.RecoverCollection().Oid);
        }
    }
}