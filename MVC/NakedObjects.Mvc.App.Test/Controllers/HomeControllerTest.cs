// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcTestApp.Tests.Util;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;

namespace MvcTestApp.Tests.Controllers {
    [TestClass]
    public class HomeControllerTest : AcceptanceTestCase {
        #region Setup/Teardown


        [TestInitialize]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            StartTest();
            controller = new HomeController(NakedObjectsFramework);
            mocks = new ContextMocks(controller);
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingEdmxContext("Model");
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

      

        private HomeController controller;
        private ContextMocks mocks;


        protected override object[] MenuServices {
            get {
                return new object[] {
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
                };
            }
        }


        private Employee Employee {
            get { return NakedObjectsFramework.Persistor.Instances<Employee>().First(); }
        }

        private Vendor Vendor {
            get { return NakedObjectsFramework.Persistor.Instances<Vendor>().First(); }
        }


        private string EmployeeId {
            get { return NakedObjectsFramework.GetObjectId(Employee); }
        }

        private string VendorId {
            get { return NakedObjectsFramework.GetObjectId(Vendor); }
        }


        [TestMethod]
        public void ClearAllHistory() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult) controller.ClearHistory(true);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [TestMethod]
        public void ClearEmptyHistory() {
            var result = (RedirectToRouteResult) controller.ClearHistory(false);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [TestMethod]
        public void ClearHistory() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistory(false);

            Assert.AreEqual("ObjectView", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOfType(data.Model, typeof(Employee));
        }

        [TestMethod]
        public void ClearHistoryItemNext1() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryItem(EmployeeId, VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOfType(data.Model, typeof(Vendor));
        }

        [TestMethod]
        public void ClearHistoryItemNext2() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryItem(VendorId, EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOfType(data.Model, typeof(Employee));
        }

        [TestMethod]
        public void ClearHistoryItemNoNext() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult) controller.ClearHistoryItem(EmployeeId, "", new ObjectAndControlData());

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }


        [TestMethod]
        public void ClearHistoryOthers1() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryOthers(EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOfType(data.Model, typeof(Employee));
        }

        [TestMethod]
        public void ClearHistoryOthers2() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryOthers(VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOfType(data.Model, typeof(Vendor));
        }
    }
}