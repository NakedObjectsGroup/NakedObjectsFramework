// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Expenses.ExpenseClaims;
using Expenses.ExpenseClaims.Items;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
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

        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();

        [Test]
        public void AddPersistentToSession() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();

            session.AddObjectToSession("key1", claim);

            Assert.AreSame(claim, session.GetObjectFromSession<Claim>("key1"));
        }

        [Test]
        public void AddStringToSession() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            const string testvalue = "test string";

            session.AddObjectToSession("key1", testvalue);

            Assert.AreEqual(testvalue, session.GetObjectFromSession<string>("key1"));
        }

        [Test]
        public void AddTransientToSession() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            var claim = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (Claim))).GetDomainObject<Claim>();

            session.AddObjectToSession("key1", claim);

            Assert.AreSame(claim, session.GetObjectFromSession<Claim>("key1"));
        }

        [Test]
        public void AddValueToSession() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            const int testvalue = 99;

            session.AddValueToSession("key1", testvalue);

            Assert.AreEqual(testvalue, session.GetValueFromSession<int>("key1"));
        }

        [Test]
        public void CachedObjectsOfBaseType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            GeneralExpense item1 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().Last();

            session.AddObjectToSession("key1", item1);
            session.AddObjectToSession("key2", item2);

            Assert.AreEqual(item1, session.GetObjectFromSession<GeneralExpense>("key1"));
            Assert.AreEqual(item2, session.GetObjectFromSession<AbstractExpenseItem>("key2"));
        }

        [Test]
        public void CachedObjectsOfDifferentType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            GeneralExpense item1 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().First();
            GeneralExpense item2 = NakedObjectsContext.ObjectPersistor.Instances<GeneralExpense>().Last();

            session.AddObjectToSession("key1", item1);
            session.AddObjectToSession("key2", item2);

            Assert.IsNull(session.GetObjectFromSession<Claim>("key1"));
            Assert.IsNull(session.GetObjectFromSession<Claim>("key1"));
        }

        [Test]
        public void CachedValuesOfBaseType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            session.AddValueToSession("key1", 1);

            Assert.AreEqual(1, session.GetValueFromSession<int>("key1"));
        }

        [Test]
        public void CachedValuesOfDifferentType() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            session.AddValueToSession("key1", 1);

            Assert.IsNull(session.GetValueFromSession<long>("key1"));
        }

        [Test]
        public void RemoveObjectFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            Claim claim = NakedObjectsContext.ObjectPersistor.Instances<Claim>().First();
            session.AddObjectToSession("key1", claim);
            Assert.AreSame(claim, session.GetObjectFromSession<Claim>("key1"));

            session.ClearFromSession("key1");

            Assert.IsNull(session.GetObjectFromSession<Claim>("key1"));
        }

        [Test]
        public void RemoveValueFromCache() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;
 
            session.AddValueToSession("key1", 1);
            Assert.AreEqual(1, session.GetValueFromSession<int>("key1"));

            session.ClearFromSession("key1");

            Assert.IsNull(session.GetValueFromSession<int>("key1"));
        }


        [Test]
        public void RemoveFromCacheNotThere() {
            var mocks = new ContextMocks(controller);
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofServices] = FrameworkHelper.GetServices();
            HttpSessionStateBase session = mocks.HtmlHelper.ViewContext.HttpContext.Session;

            session.ClearFromSession("key1");

            Assert.IsNull(session.GetObjectFromSession<Claim>("key1"));
        }
    }
}