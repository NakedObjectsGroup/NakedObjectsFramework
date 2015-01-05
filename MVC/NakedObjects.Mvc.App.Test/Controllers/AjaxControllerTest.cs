// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AdventureWorksModel;
using Microsoft.Practices.Unity;

using Moq;
using MvcTestApp.Tests.Util;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.DatabaseHelpers;
using NakedObjects.Mvc.App.Controllers;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Web.Mvc.Controllers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NakedObjects.Core.Util;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MvcTestApp.Tests.Controllers {
    [TestFixture]
    public class AjaxControllerTest : AcceptanceTestCase {
        #region Setup/Teardown


        [SetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);

            StartTest();
            controller = new AjaxController(NakedObjectsFramework);
            mocks = new ContextMocks(controller);
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingEdmxContext("Model");
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public  void SetupTestFixture() {
            DatabaseUtils.RestoreDatabase("AdventureWorks", "AdventureWorks", Constants.Server);
            SqlConnection.ClearAllPools();
        }

        private AjaxController controller;
        private ContextMocks mocks;


        protected override object[] MenuServices {
            get {
                return (new object[] {
                    new CustomerRepository(),
                    new OrderRepository(),
                    new ProductRepository(),
                    new EmployeeRepository(),
                    new SalesRepository(),
                    new SpecialOfferRepository(),
                    new ContactRepository(),
                    new VendorRepository(),
                    new PurchaseOrderRepository(),
                    new WorkOrderRepository(),
                    new SimpleRepository<NotPersistedObject>()
                });
            }
        }

        protected override object[] ContributedActions {
            get { return (new object[] {new OrderContributedActions()}); }
        }

        protected override object[] SystemServices {
            get { return (new object[] {new ChoicesRepository(), new AutoCompleteRepository()}); }
        }

        protected override Type[] Types {
            get {
                return new Type[] {
                    typeof (EnumerableQuery<string>)
                };
            }
        }


        public void TestGetActionChoicesOtherParms(string value) {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");

            const string actionName = "AnAction";

            string id = NakedObjectsFramework.GetObjectId(choicesRepo);

            const string parm1Id = "ChoicesRepository-AnAction-Parm1-Input0";
            const string parm2Id = "ChoicesRepository-AnAction-Parm2-Input0";
            const string parm3Id = "ChoicesRepository-AnAction-Parm3-Input0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, value}, {parm2Id, ""}, {parm3Id, ""}});

            JsonResult result = controller.GetActionChoices(id, actionName);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnAction-Parm1-Input"));
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnAction-Parm2-Input"));

            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm1-Input"][0].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm2-Input"][0].SequenceEqual(new[] {value + "postfix1", value + "postfix2"}));

            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm1-Input"][1].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm2-Input"][1].SequenceEqual(new[] {value + "postfix1", value + "postfix2"}));
        }

        public void TestGetActionChoicesOtherParmsMultiple(string value) {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");

            const string actionName = "AnActionMultiple";

            string id = NakedObjectsFramework.GetObjectId(choicesRepo);

            const string parm1Id = "ChoicesRepository-AnActionMultiple-Parm1-Select0";
            const string parm2Id = "ChoicesRepository-AnActionMultiple-Parm2-Select0";
            const string parm3Id = "ChoicesRepository-AnActionMultiple-Parm3-Select0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, value}, {parm2Id, ""}, {parm3Id, ""}});

            JsonResult result = controller.GetActionChoices(id, actionName);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm1-Select"));
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm2-Select"));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][0].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][0].SequenceEqual(new[] {value + "postfix1", value + "postfix2"}));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][1].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][1].SequenceEqual(new[] {value + "postfix1", value + "postfix2"}));
        }

        public void TestGetActionChoicesOtherParmsMultipleMultiSelect(string value) {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");

            const string actionName = "AnActionMultiple";

            string id = NakedObjectsFramework.GetObjectId(choicesRepo);

            const string parm1Id0 = "ChoicesRepository-AnActionMultiple-Parm1-Select0";
            const string parm1Id1 = "ChoicesRepository-AnActionMultiple-Parm1-Select1";
            const string parm2Id = "ChoicesRepository-AnActionMultiple-Parm2-Select0";
            const string parm3Id = "ChoicesRepository-AnActionMultiple-Parm3-Select0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id0, value + value}, {parm1Id1, value}, {parm2Id, ""}, {parm3Id, ""}});

            JsonResult result = controller.GetActionChoices(id, actionName);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm1-Select"));
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm2-Select"));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][0].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][0].SequenceEqual(new[] {value + value + "postfix1", value + "postfix2"}));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][1].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][1].SequenceEqual(new[] {value + value + "postfix1", value + "postfix2"}));
        }

        [Test]
        public void TestGetActionAutoComplete() {
            INakedObject autoCompleteRepo = NakedObjectsFramework.GetAdaptedService("AutoCompleteRepository");

            const string actionName = "AnAction";

            string id = NakedObjectsFramework.GetObjectId(autoCompleteRepo);

            const string parm1Id = "AutoCompleteRepository-AnAction-name-Input";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, ""}});

            JsonResult result = controller.GetActionCompletions(id, actionName, 0, "avalue");
            Assert.IsInstanceOf<List<object>>(result.Data);

            var list = result.Data as IList<object>;

            Assert.AreEqual(2, list.Count);

            var nv1 = new RouteValueDictionary(list[0]);
            var nv2 = new RouteValueDictionary(list[1]);

            Assert.AreEqual("value1", nv1["label"]);
            Assert.AreEqual("value1", nv1["value"]);
            Assert.AreEqual("value1", nv1["link"]);
            Assert.AreEqual("/Images/Default.png", nv1["src"]);
            Assert.AreEqual("String", nv1["alt"]);

            Assert.AreEqual("value2", nv2["label"]);
            Assert.AreEqual("value2", nv2["value"]);
            Assert.AreEqual("value2", nv2["link"]);
            Assert.AreEqual("/Images/Default.png", nv2["src"]);
            Assert.AreEqual("String", nv1["alt"]);
        }

        [Test]
        public void TestGetActionChoicesDefault() {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");

            const string actionName = "AnAction";

            string id = NakedObjectsFramework.GetObjectId(choicesRepo);

            const string parm1Id = "ChoicesRepository-AnAction-Parm1-Input0";
            const string parm2Id = "ChoicesRepository-AnAction-Parm2-Input0";
            const string parm3Id = "ChoicesRepository-AnAction-Parm3-Input0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, ""}, {parm2Id, ""}, {parm3Id, ""},});

            JsonResult result = controller.GetActionChoices(id, actionName);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnAction-Parm1-Input"));
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnAction-Parm2-Input"));

            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm1-Input"][0].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm2-Input"][0].SequenceEqual(new string[] {}));

            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm1-Input"][1].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnAction-Parm2-Input"][1].SequenceEqual(new string[] {}));
        }


        [Test]
        public void TestGetActionChoicesOtherParms1() {
            TestGetActionChoicesOtherParms("value1");
        }

        [Test]
        public void TestGetActionChoicesOtherParms2() {
            TestGetActionChoicesOtherParms("value2");
        }

        [Test]
        public void TestGetActionChoicesOtherParmsMultiple1() {
            TestGetActionChoicesOtherParmsMultiple("value1");
        }

        [Test]
        public void TestGetActionChoicesOtherParmsMultiple2() {
            TestGetActionChoicesOtherParmsMultiple("value2");
        }

        [Test]
        public void TestGetActionChoicesOtherParmsMultipleMultiSelect1() {
            TestGetActionChoicesOtherParmsMultipleMultiSelect("value1");
        }

        [Test]
        public void TestGetActionChoicesOtherParmsMultipleMultiSelect2() {
            TestGetActionChoicesOtherParmsMultipleMultiSelect("value2");
        }

        [Test]
        public void TestGetActionMultipleChoicesDefault() {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");

            const string actionName = "AnActionMultiple";

            string id = NakedObjectsFramework.GetObjectId(choicesRepo);

            const string parm1Id = "ChoicesRepository-AnActionMultiple-Parm1-Select0";
            const string parm2Id = "ChoicesRepository-AnActionMultiple-Parm2-Select0";
            const string parm3Id = "ChoicesRepository-AnActionMultiple-Parm3-Select0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, ""}, {parm2Id, ""}, {parm3Id, ""},});

            JsonResult result = controller.GetActionChoices(id, actionName);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(2, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm1-Select"));
            Assert.IsTrue(dict.ContainsKey("ChoicesRepository-AnActionMultiple-Parm2-Select"));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][0].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][0].SequenceEqual(new string[] {}));

            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm1-Select"][1].SequenceEqual(new[] {"value1", "value2"}));
            Assert.IsTrue(dict["ChoicesRepository-AnActionMultiple-Parm2-Select"][1].SequenceEqual(new string[] {}));
        }


        [Test]
        public void TestGetPropertyAutoComplete() {
            INakedObject autoCompleteRepo = NakedObjectsFramework.GetAdaptedService("AutoCompleteRepository");
            object autoCompleteObject = autoCompleteRepo.GetDomainObject<AutoCompleteRepository>().GetAutoCompleteObject();

            string id = NakedObjectsFramework.GetObjectId(autoCompleteObject);

            const string parm1Id = "AutoCompleteObject-Name-Input";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, ""}});

            JsonResult result = controller.GetPropertyCompletions(id, "AProperty", "");

            var list = result.Data as IList<object>;

            Assert.AreEqual(2, list.Count);

            var nv1 = new RouteValueDictionary(list[0]);
            var nv2 = new RouteValueDictionary(list[1]);

            Assert.AreEqual("value5", nv1["label"]);
            Assert.AreEqual("value5", nv1["value"]);
            Assert.AreEqual("value5", nv1["link"]);
            Assert.AreEqual("/Images/Default.png", nv1["src"]);
            Assert.AreEqual("String", nv1["alt"]);

            Assert.AreEqual("value6", nv2["label"]);
            Assert.AreEqual("value6", nv2["value"]);
            Assert.AreEqual("value6", nv2["link"]);
            Assert.AreEqual("/Images/Default.png", nv2["src"]);
            Assert.AreEqual("String", nv1["alt"]);
        }

        [Test]
        public void TestGetPropertyChoicesDefault() {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");
            object choicesObject = choicesRepo.GetDomainObject<ChoicesRepository>().GetChoicesObject();


            string id = NakedObjectsFramework.GetObjectId(choicesObject);

            const string parm1Id = "ChoicesObject-Name-Input";
            const string parm2Id = "ChoicesObject-AProperty-Input";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, ""}, {parm2Id, ""}});

            JsonResult result = controller.GetPropertyChoices(id);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesObject-AProperty-Input"));


            Assert.IsTrue(dict["ChoicesObject-AProperty-Input"][0].SequenceEqual(new string[] {}));
            Assert.IsTrue(dict["ChoicesObject-AProperty-Input"][1].SequenceEqual(new string[] {}));
        }


        [Test]
        public void TestGetPropertyChoicesOtherValue() {
            INakedObject choicesRepo = NakedObjectsFramework.GetAdaptedService("ChoicesRepository");
            object choicesObject = choicesRepo.GetDomainObject<ChoicesRepository>().GetChoicesObject();


            string id = NakedObjectsFramework.GetObjectId(choicesObject);

            const string parm1Id = "ChoicesObject-Name-Input0";
            const string parm2Id = "ChoicesObject-AProperty-Input0";

            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parm1Id, "AName"}, {parm2Id, ""}});

            JsonResult result = controller.GetPropertyChoices(id);
            Assert.IsInstanceOf<IDictionary<string, string[][]>>(result.Data);

            var dict = result.Data as IDictionary<string, string[][]>;

            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.ContainsKey("ChoicesObject-AProperty-Input"));

            Assert.IsTrue(dict["ChoicesObject-AProperty-Input"][0].SequenceEqual(new[] {"AName-A", "AName-B"}));
            Assert.IsTrue(dict["ChoicesObject-AProperty-Input"][1].SequenceEqual(new[] {"AName-A", "AName-B"}));
        }

        [Test]
        public void TestJsonp() {
            const string data = "testData";

            AjaxControllerImpl.JsonpResult jsonpResult = new AjaxController(null).Jsonp(data, "application/json", Encoding.UTF8);

            var mockControllerContext = new Mock<ControllerContext>();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var mockRequest = new Mock<HttpRequestBase>();

            mockControllerContext.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockRequest.Setup(x => x.Params).Returns(new NameValueCollection() {{"callback", "testCallbackValue"}});

            jsonpResult.ExecuteResult(mockControllerContext.Object);

            mockResponse.Verify(x => x.Write("testCallbackValue(\"testData\")"));
            mockResponse.VerifySet(x => x.ContentType = "application/json");
            mockResponse.VerifySet(x => x.ContentEncoding = Encoding.UTF8);
        }

        [Test]
        public void TestJsonpDefaults() {
            const string data = "testData";

            AjaxControllerImpl.JsonpResult jsonpResult = new AjaxController(null).Jsonp(data);

            var mockControllerContext = new Mock<ControllerContext>();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockResponse = new Mock<HttpResponseBase>();
            var mockRequest = new Mock<HttpRequestBase>();

            mockControllerContext.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            mockHttpContext.Setup(x => x.Response).Returns(mockResponse.Object);
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
            mockRequest.Setup(x => x.Params).Returns(new NameValueCollection() {{"callback", "testCallbackValue"}});

            jsonpResult.ExecuteResult(mockControllerContext.Object);

            mockResponse.Verify(x => x.Write("testCallbackValue(\"testData\")"));
            mockResponse.VerifySet(x => x.ContentType = "application/json");
        }

        [Test]
        public void TestValidateFailRefParameter() {
            Store store = NakedObjectsFramework.Persistor.Instances<Store>().First();
            Vendor vendor = NakedObjectsFramework.Persistor.Instances<Vendor>().First();
            const string actionName = "CreateNewOrder";
            const string parameterName = "customer";

            string id = NakedObjectsFramework.GetObjectId(store);
            string value = NakedObjectsFramework.GetObjectId(vendor);

            JsonResult result = controller.ValidateParameter(id, value, actionName, parameterName);
            Assert.AreEqual("Not a suitable type; must be a Customer", result.Data);
        }

        [Test]
        public void TestValidateFailRefProperty() {
            Store store = NakedObjectsFramework.Persistor.Instances<Store>().First();
            Store store1 = NakedObjectsFramework.Persistor.Instances<Store>().OrderBy(x => "").Skip(1).First();

            string id = NakedObjectsFramework.GetObjectId(store);
            string value = NakedObjectsFramework.GetObjectId(store1);
            const string propertyName = "SalesPerson";

            JsonResult result = controller.ValidateProperty(id, value, propertyName);
            Assert.AreEqual("Not a suitable type; must be a Sales Person", result.Data);
        }

        [Test]
        public void TestValidateFailValueParameter() {
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");


            const string actionName = "FindContactByName";
            const string parameterName = "lastName";

            string id = NakedObjectsFramework.GetObjectId(contactRepo);
            const string value = "";
            const string parmId = "ContactRepository-FindContactByName-LastName-Input";
            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parmId, value}});

            JsonResult result = controller.ValidateParameter(id, null, actionName, parameterName);
            Assert.AreEqual("Mandatory", result.Data);
        }

        [Test]
        public void TestValidateFailValueProperty() {
            Vendor vendor = NakedObjectsFramework.Persistor.Instances<Vendor>().First();
            string id = NakedObjectsFramework.GetObjectId(vendor);
            const string value = "";
            const string propertyName = "AccountNumber";
            const string fieldId = "Vendor-AccountNumber-Input";
            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{fieldId, value}});

            JsonResult result = controller.ValidateProperty(id, null, propertyName);
            Assert.AreEqual("Mandatory", result.Data);
        }

        [Test]
        public void TestValidateOkInlineValueProperty() {
            TimePeriod timePeriod = NakedObjectsFramework.Persistor.Instances<Shift>().First().Times;
            string id = NakedObjectsFramework.GetObjectId(timePeriod);
            string value = DateTime.Now.ToString();
            const string propertyName = "StartTime";
            const string fieldId = "Times-TimePeriod-StartTime-Input";
            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{fieldId, value}});

            JsonResult result = controller.ValidateProperty(id, null, propertyName);
            Assert.IsTrue((bool) result.Data);
        }

        [Test]
        public void TestValidateOkRefParameter() {
            Store store = NakedObjectsFramework.Persistor.Instances<Store>().First();
            const string actionName = "CreateNewOrder";
            const string parameterName = "customer";


            string id = NakedObjectsFramework.GetObjectId(store);
            string value = NakedObjectsFramework.GetObjectId(store);

            JsonResult result = controller.ValidateParameter(id, value, actionName, parameterName);
            Assert.IsTrue((bool) result.Data);
        }

        [Test]
        public void TestValidateOkRefProperty() {
            Store store = NakedObjectsFramework.Persistor.Instances<Store>().First();
            SalesPerson salesPerson = NakedObjectsFramework.Persistor.Instances<SalesPerson>().First();

            string id = NakedObjectsFramework.GetObjectId(store);
            string value = NakedObjectsFramework.GetObjectId(salesPerson);
            const string propertyName = "SalesPerson";

            JsonResult result = controller.ValidateProperty(id, value, propertyName);
            Assert.IsTrue((bool) result.Data);
        }

        [Test]
        public void TestValidateOkValueParameter() {
            INakedObject contactRepo = NakedObjectsFramework.GetAdaptedService("ContactRepository");


            const string actionName = "FindContactByName";
            const string parameterName = "lastName";

            string id = NakedObjectsFramework.GetObjectId(contactRepo);
            const string value = "Bloggs";
            const string parmId = "ContactRepository-FindContactByName-LastName-Input";
            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{parmId, value}});

            JsonResult result = controller.ValidateParameter(id, null, actionName, parameterName);
            Assert.IsTrue((bool) result.Data);
        }

        [Test]
        public void TestValidateOkValueProperty() {
            Vendor vendor = NakedObjectsFramework.Persistor.Instances<Vendor>().First();
            string uniqueActNum = Guid.NewGuid().ToString().Remove(14);
            string id = NakedObjectsFramework.GetObjectId(vendor);
            string value = uniqueActNum;
            const string propertyName = "AccountNumber";
            const string fieldId = "Vendor-AccountNumber-Input";
            mocks.Request.Setup(x => x.Params).Returns(new NameValueCollection {{fieldId, value}});

            JsonResult result = controller.ValidateProperty(id, null, propertyName);
            Assert.IsTrue((bool) result.Data);
        }
    }
}