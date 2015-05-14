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
using Moq;
using MvcTestApp.Tests.Util;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Surface.Utility;
using NakedObjects.Web.Mvc;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class HomeControllerTest : AcceptanceTestCase {
        private HomeController controller;
        private ContextMocks mocks;

        protected override string[] Namespaces {
            get {
                return new[] {
                    "AdventureWorksModel", "MvcTestApp.Tests.Controllers"
                };
            }
        }

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

        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            StartTest();
            //var mockMessageBroker = new Mock<IMessageBrokerSurface>().Object;

            controller = new HomeController( Surface, new IdHelper());
            mocks = new ContextMocks(controller);
        }

        protected INakedObjectsSurface Surface { get; set; }

        protected override void StartTest() {
            Surface = this.GetConfiguredContainer().Resolve<INakedObjectsSurface>();
            NakedObjectsFramework = ((dynamic)Surface).Framework;
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration { EnforceProxies = false };
            config.UsingCodeFirstContext(() => new AdventureWorksContext());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
            container.RegisterType<IOidStrategy, MVCOid>(new PerResolveLifetimeManager());
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        [Test]
        public void ClearAllHistory() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult) controller.ClearHistory(true);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [Test]
        public void ClearEmptyHistory() {
            var result = (RedirectToRouteResult) controller.ClearHistory(false);

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [Test]
     

        public void ClearHistory() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistory(false);

            Assert.AreEqual("ObjectView", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<Employee>(data.Model);
        }

        [Test]
        public void ClearHistoryItemNext1() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryItem(EmployeeId, VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<Vendor>(data.Model);
        }

        [Test]
        public void ClearHistoryItemNext2() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryItem(VendorId, EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<Employee>(data.Model);
        }

        [Test]
        public void ClearHistoryItemNoNext() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (RedirectToRouteResult) controller.ClearHistoryItem(EmployeeId, "", new ObjectAndControlData());

            Assert.AreEqual("Index", result.RouteValues.Values.ElementAt(0));
            Assert.AreEqual("Home", result.RouteValues.Values.ElementAt(1));
        }

        [Test]
        public void ClearHistoryOthers1() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryOthers(EmployeeId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<Employee>(data.Model);
        }

        [Test]
        public void ClearHistoryOthers2() {
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Employee, ObjectCache.ObjectFlag.BreadCrumb);
            mocks.HttpContext.Object.Session.AddToCache(NakedObjectsFramework, Vendor, ObjectCache.ObjectFlag.BreadCrumb);

            var result = (ViewResult) controller.ClearHistoryOthers(VendorId, new ObjectAndControlData());

            Assert.AreEqual("ViewNameSetAfterTransaction", result.ViewName);
            ViewDataDictionary data = result.ViewData;
            Assert.IsInstanceOf<Vendor>(data.Model);
        }
    }
}