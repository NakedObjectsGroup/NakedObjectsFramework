// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Rest;
using NakedObjects.Rest.Model;
using NakedObjects.Xat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {
    public class MenuTest : AcceptanceTestCase {
        protected override Type[] Functions { get; } = {
            typeof(SimpleMenuFunctions),
            typeof(DateMenuFunctions),
            typeof(ChoicesMenuFunctions),
            typeof(DefaultedMenuFunctions),
            typeof(ValidatedMenuFunctions),
            typeof(DisabledMenuFunctions),
            typeof(HiddenMenuFunctions),
            typeof(AutoCompleteMenuFunctions)
        };

        // todo should IAlert be here or should we ignore?
        protected override Type[] Records { get; } = {typeof(SimpleRecord), typeof(DateRecord), typeof(TestEnum)};

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] {config => new MenuDbContext()};

        private static string FullName<T>() => typeof(T).FullName;

        protected override IMenu[] MainMenus(IMenuFactory factory) => Functions.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();


        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ObjectReflectorConfiguration.NoValidate = true;
            var context = new MenuDbContext();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            MenuDbContext.Delete();
        }

        protected RestfulObjectsControllerBase Api() {
            var sp = GetConfiguredContainer();
            var api = sp.GetService<RestfulObjectsController>();
            return Helpers.SetMockContext(api, sp);
        }

        [Test]
        public void TestGetMenus() {
            var api = Api();
            var result = api.GetMenus();
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var val = parsedResult.GetValue("value") as JArray;

            Assert.IsNotNull(val);
            Assert.AreEqual(8, val.Count);

            var firstItem = val.First;

            firstItem.AssertMenuLink("SimpleMenuFunctions", "GET", nameof(SimpleMenuFunctions));
        }

        [Test]
        public void TestGetMenu() {
            var api = Api();
            var result = api.GetMenu(nameof(SimpleMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("SimpleMenuFunctions", parsedResult["title"].ToString());
            Assert.AreEqual(nameof(SimpleMenuFunctions), parsedResult["menuId"].ToString());

            var members = parsedResult["members"] as JObject;
            Assert.AreEqual(typeof(SimpleMenuFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public).Length, members?.Count);

            var function = members[nameof(SimpleMenuFunctions.FindByName)];

            function.AssertAction(nameof(SimpleMenuFunctions.FindByName));
            function["extensions"].AssertExtensions(7); // todo add 

            var links = function["links"] as JArray;

            Assert.AreEqual(2, links.Count);

            var invokeLink = links.Last;

            invokeLink.AssertMenuInvokeLink("{\r\n  \"searchString\": {\r\n    \"value\": null\r\n  }\r\n}", "GET", nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByName));
        }

        [Test]
        public void TestGetMenuAction() {
            var api = Api();
            var result = api.GetMenuAction(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByName));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(SimpleMenuFunctions.FindByName), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var parameter = parameters["searchString"];
            Assert.AreEqual(2, parameter.Count());
            var links = parameter["links"];
            var extensions = parameter["extensions"];
            Assert.AreEqual(0, links.Count());
            Assert.AreEqual(7, extensions.Count());

            // todo test rest of json
        }

        [Test]
        public void TestGetMenuEnumAction() {
            var api = Api();
            var result = api.GetMenuAction(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByEnum));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("number", parsedResult["parameters"]["eParm"]["extensions"]["returnType"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsSingleItemList() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"searchString", new ScalarValue("Fred")}}};
            var result = api.GetInvokeOnMenu(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByName), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];
            var value = resultObj["value"] as JArray;

            Assert.AreEqual(1, value.Count);

            value[0].AssertObjectElementLink("Fred", "GET", Helpers.FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsMultipleItemList() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"length", new ScalarValue(4)}}};
            var result = api.GetInvokeOnMenu(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByLength), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];
            var value = resultObj["value"] as JArray;

            Assert.AreEqual(3, value.Count);

            value[0].AssertObjectElementLink("Bill", "GET", Helpers.FullName<SimpleRecord>(), "2");
            value[1].AssertObjectElementLink("Fred", "GET", Helpers.FullName<SimpleRecord>(), "1");
            value[2].AssertObjectElementLink("Jack", "GET", Helpers.FullName<SimpleRecord>(), "3");
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsRandomItem() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue>()};
            var result = api.GetInvokeOnMenu(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.Random), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            switch (resultObj["instanceId"].ToString()) {
                case "1":
                    resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
                    break;
                case "2":
                    resultObj.AssertObject("Bill", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "2");
                    break;
                case "3":
                    resultObj.AssertObject("Jack", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "3");
                    break;
                default:
                    Assert.Fail("unexpected result");
                    break;
            }
        }

        [Test]
        public void TestInvokeMenuActionThatGeneratesWarningNoObject() {
            var api = Api().AsPost();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"number", new ScalarValue("4")}}};
            var result = api.PostInvokeOnMenu(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByNumber), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            Assert.AreEqual("There is no matching object", parsedResult["extensions"]["x-ro-nof-warnings"][0].ToString());

            var resultObj = parsedResult["result"];

            Assert.AreEqual("", resultObj.ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatGeneratesWarningObject() {
            var api = Api().AsPost();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"number", new ScalarValue("1")}}};
            var result = api.PostInvokeOnMenu(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.FindByNumber), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            Assert.AreEqual(null, parsedResult["extensions"]["x-ro-nof-warnings"]);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }

        private static string FormatForTest(DateTime dt) => $"{dt.Year}-{dt.Month:00}-{dt.Day:00}";

        [Test]
        public void TestGetMenuActionWithAnnotatedDefaults() {
            var api = Api();
            var result = api.GetMenuAction(nameof(DateMenuFunctions), nameof(DateMenuFunctions.DateWithDefault));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(DateRecordFunctions.DateWithDefault), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var psd = parameters["dt"];

            Assert.AreEqual(FormatForTest(DateTime.UtcNow.AddDays(22)), psd["default"].ToString());
        }

        [Test]
        public void TestGetMenuActionWithChoices() {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoices));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoices), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var choices = parameters["record"]["choices"];

            Assert.AreEqual(3, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
            Assert.AreEqual("Bill", choices[1]["title"].ToString());
            Assert.AreEqual("Jack", choices[2]["title"].ToString());
        }

        [Test]
        public void TestGetMenuActionWithChoicesNoContext()
        {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoicesNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoicesNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var choices = parameters["record"]["choices"];

            Assert.AreEqual(0, choices.Count());
        }

        [Test]
        public void TestGetMenuActionWithChoicesWithParameters() {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoicesWithParameters));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoicesWithParameters), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(3, parameters.Count());
            var prompt = parameters["record"]["links"][0];

            Assert.AreEqual(2, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/menus/ChoicesMenuFunctions/actions/WithChoicesWithParameters/params/record/prompt", prompt["href"].ToString());
        }


        [Test]
        public void TestGetMenuActionWithChoicesWithParametersNoContext()
        {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoicesWithParametersNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoicesWithParametersNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(3, parameters.Count());
            var prompt = parameters["record"]["links"][0];

            Assert.AreEqual(2, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/menus/ChoicesMenuFunctions/actions/WithChoicesWithParametersNoContext/params/record/prompt", prompt["href"].ToString());
        }

        [Test]
        public void TestGetMenuActionWithMultipleChoices() {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithMultipleChoices));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithMultipleChoices), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            var choices = parameters["simpleRecords"]["choices"];

            Assert.AreEqual(3, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
            Assert.AreEqual("Bill", choices[1]["title"].ToString());
            Assert.AreEqual("Jack", choices[2]["title"].ToString());

            var prompt = parameters["dateRecords"]["links"][0];

            Assert.AreEqual(1, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/menus/ChoicesMenuFunctions/actions/WithMultipleChoices/params/dateRecords/prompt", prompt["href"].ToString());
        }


        [Test]
        public void TestGetMenuActionWithMultipleChoicesNoContext()
        {
            var api = Api();
            var result = api.GetMenuAction(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithMultipleChoicesNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesMenuFunctions.WithMultipleChoicesNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            var choices = parameters["simpleRecords"]["choices"];

            Assert.AreEqual(0, choices.Count());


            var prompt = parameters["dateRecords"]["links"][0];

            Assert.AreEqual(1, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/menus/ChoicesMenuFunctions/actions/WithMultipleChoicesNoContext/params/dateRecords/prompt", prompt["href"].ToString());
        }

        [Test]
        public void TestGetMenuActionCreateNew()
        {
            var api = Api();
            var result = api.GetMenuAction(nameof(SimpleMenuFunctions), nameof(SimpleMenuFunctions.CreateNewFunction));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("True", parsedResult["extensions"]["x-ro-nof-createNew"].ToString());
        }


        [Test]
        public void TestGetMenuPrompt() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"parm1", new ScalarValue("1")}, {"parm2", new ScalarValue("J")}}};
            var result = api.GetParameterPromptOnMenu(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoicesWithParameters), "record", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("record", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(1, choices.Count());
            Assert.AreEqual("Jack", choices[0]["title"].ToString());
            ;
        }

        [Test]
        public void TestGetMenuPromptNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm1", new ScalarValue("1") }, { "parm2", new ScalarValue("J") } } };
            var result = api.GetParameterPromptOnMenu(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithChoicesWithParametersNoContext), "record", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("record", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(0, choices.Count());
        }


        [Test]
        public void TestGetMenuPromptWithMultipleChoices() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"simplerecords", new ListValue(new IValue[] {new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord")})}}};
            var result = api.GetParameterPromptOnMenu(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithMultipleChoices), "dateRecords", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("dateRecords", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(1, choices.Count());
            Assert.IsTrue(choices[0]["title"].ToString().StartsWith("DateRecord"));
        }


        [Test]
        public void TestGetMenuPromptWithMultipleChoicesNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simplerecords", new ListValue(new IValue[] { new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") }) } } };
            var result = api.GetParameterPromptOnMenu(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithMultipleChoicesNoContext), "dateRecords", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("dateRecords", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(0, choices.Count());
        }


        [Test]
        public void TestGetMenuActionWithDefaults() {
            var api = Api();
            var result = api.GetMenuAction(nameof(DefaultedMenuFunctions), nameof(DefaultedMenuFunctions.WithDefaults));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(DefaultedMenuFunctions.WithDefaults), parsedResult["id"].ToString());

            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            Assert.AreEqual("101", parameters["default1"]["default"].ToString());
            Assert.AreEqual("Fred", parameters["default2"]["default"]["title"].ToString());
        }

        [Test]
        public void TestInvokeRecordActionWithValidateFail() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("2")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
            Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithValidateSuccess() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("1")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestInvokeRecordActionWithCrossValidateFail() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("2")}, {"validate2", new ScalarValue("1")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithCrossValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithCrossValidateSuccess() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("1")}, {"validate2", new ScalarValue("1")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithCrossValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestGetMenuActionWithDisable1() {
            var api = Api();
            var result = api.GetMenu(nameof(DisabledMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["WithDisabled1"]);
        }

        [Test]
        public void TestGetMenuActionWithDisable2() {
            var api = Api();
            var result = api.GetMenu(nameof(DisabledMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.IsNotNull(parsedResult["members"]["WithDisabled2"]);
        }

        [Test]
        public void TestGetMenuActionWithHidden1() {
            var api = Api();
            var result = api.GetMenu(nameof(HiddenMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["WithHidden1"]);
        }

        [Test]
        public void TestGetMenuActionWithHidden2() {
            var api = Api();
            var result = api.GetMenu(nameof(HiddenMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.IsNotNull(parsedResult["members"]["WithHidden2"]);
        }

        [Test]
        public void TestGetMenuActionWithAutoComplete() {
            var api = Api();
            var result = api.GetMenuAction(nameof(AutoCompleteMenuFunctions), nameof(AutoCompleteMenuFunctions.WithAutoComplete));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(AutoCompleteMenuFunctions.WithAutoComplete), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());

            var parameter = parameters["simpleRecord"];
            Assert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
            Assert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
            Assert.AreEqual("http://localhost/menus/AutoCompleteMenuFunctions/actions/WithAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionPromptWithAutoComplete() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue>(), ReservedArguments = new ReservedArguments {SearchTerm = "Fr"}};
            var result = api.GetParameterPromptOnMenu(nameof(AutoCompleteMenuFunctions), nameof(AutoCompleteMenuFunctions.WithAutoComplete), "simpleRecord", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("simpleRecord", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(2, choices.Count()); // tests PageSize
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
            Assert.AreEqual("Bill", choices[1]["title"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionWithAutoComplete() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord")}}};

            var result = api.GetInvokeOnMenu(nameof(AutoCompleteMenuFunctions), nameof(AutoCompleteMenuFunctions.WithAutoComplete), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("Fred", resultObj["title"].ToString());
        }

        [Test]
        public void TestGetMenuActionWithValidateNoContext() {
            var api = Api();
            var result = api.GetMenuAction(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithValidationNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ValidatedRecordFunctions.WithValidationNoContext), parsedResult["id"].ToString());

            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
        }

        [Test]
        public void TestInvokeMenuActionWithValidateFailNoContext() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("2")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
            Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeMenuActionWithValidateSuccessNoContext() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"validate1", new ScalarValue("1")}}};
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestInvokeMenuActionWithCrossValidateFailNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithCrossValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeMenuActionWithCrossValidateSuccessNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvokeOnMenu(nameof(ValidatedMenuFunctions), nameof(ValidatedMenuFunctions.WithCrossValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

    }
}