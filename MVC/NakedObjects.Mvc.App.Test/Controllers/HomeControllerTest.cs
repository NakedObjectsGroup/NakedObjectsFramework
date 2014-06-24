// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using MvcTestApp.Tests.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.DatabaseHelpers;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Controllers {

    [TestFixture]
    public class HomeControllerTest : AcceptanceTestCase {
        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                                                                    new CustomerRepository(),
                                                                    new OrderRepository(),
                                                                    new ProductRepository(),
                                                                    new EmployeeRepository(),
                                                                    new SalesRepository(),
                                                                    new SpecialOfferRepository(),
                                                                    new ContactRepository(),
                                                                    new VendorRepository(),
                                                                    new PurchaseOrderRepository(),
                                                                    new WorkOrderRepository()
                                                                });
            }
        }

        public class TestFixture {
            public void Install() { }
        }

        protected override IFixturesInstaller Fixtures {
            get {
                return new FixturesInstaller(new object[] {new TestFixture()});
            }
        }

        protected override IObjectPersistorInstaller Persistor {
            get { return new EntityPersistorInstaller(); }
        }

      


        [TestFixtureSetUp]
        public void SetupFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
        }

        [TearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }


        [Test]
        public void ExecuteFixture() {
            var controller = new HomeController();
            new ContextMocks(controller);

            var result = (ViewResult)controller.ExecuteFixture("TestFixture");

            Assert.AreEqual("Index", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.AreEqual("Fixture: TestFixture installed", (data[IdHelper.SystemMessages] as string[])[0]);
        }

        private static Employee Employee {
            get {
                return NakedObjectsContext.ObjectPersistor.Instances<Employee>().First();
            }
        }

        private static Vendor Vendor {
            get {
                return NakedObjectsContext.ObjectPersistor.Instances<Vendor>().First();
            }
        }


        private static string EmployeeId {
            get {
                return FrameworkHelper.GetObjectId(Employee);
            }
        }

        private static string VendorId {
            get {
                return FrameworkHelper.GetObjectId(Vendor);
            }
        }


        [Test]
        public void ClearHistory() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult)controller.ClearHistory(false);

            Assert.AreEqual("ObjectView", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof(Employee), data.Model);           
        }

        [Test]
        public void ClearAllHistory() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult)controller.ClearHistory(true);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [Test]
        public void ClearHistoryItemNoNext() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult)controller.ClearHistoryItem(EmployeeId, "", new ObjectAndControlData());

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [Test]
        public void ClearHistoryItemNext1() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult)controller.ClearHistoryItem(EmployeeId, VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof(Vendor), data.Model);
        }

        [Test]
        public void ClearHistoryItemNext2() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult)controller.ClearHistoryItem(VendorId, EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof(Employee), data.Model);
        }


        [Test]
        public void ClearHistoryOthers1() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult)controller.ClearHistoryOthers(EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof(Employee), data.Model);
        }

        [Test]
        public void ClearHistoryOthers2() {
            var controller = new HomeController();
            var mocks = new ContextMocks(controller);

            mocks.HttpContext.Object.Session.AddToCache(Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult)controller.ClearHistoryOthers(VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf(typeof(Vendor), data.Model);
        }

        [Test]
        public void ClearEmptyHistory() {
            var controller = new HomeController();
            new ContextMocks(controller);

            var result = (RedirectToRouteResult) controller.ClearHistory(false);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }
    }
}