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
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test;

public class ServiceTestEF6 : AcceptanceTestCase {
    protected override Type[] Services { get; } = {
        typeof(MenuTestFunctions),
        typeof(DateMenuFunctions),
        typeof(ChoicesMenuFunctions),
        typeof(DefaultedMenuFunctions),
        typeof(ValidatedMenuFunctions),
        typeof(DisabledMenuFunctions),
        typeof(HiddenMenuFunctions),
        typeof(AutoCompleteMenuFunctions),
        typeof(ViewModelMenuFunctions),
        typeof(ReferenceMenuFunctions),
        typeof(ViewModelFunctions),
        typeof(CollectionMenuFunctions),
        typeof(CollectionContributedFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(SimpleRecord),
        typeof(DateRecord),
        typeof(TestEnum),
        typeof(ViewModel),
        typeof(ReferenceRecord),
        typeof(UpdatedRecord),
        typeof(CollectionRecord),
        typeof(OrderedRecord),
        typeof(AlternateKeyRecord),
        typeof(NToNCollectionRecord1),
        typeof(NToNCollectionRecord2)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Functions { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new ServiceDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected virtual void CleanUpDatabase() {
        ServiceDbContext.Delete();
    }

    protected virtual void CreateDatabase() { }

    protected static string FullName<T>() => typeof(T).FullName;

    protected override IMenu[] MainMenus(IMenuFactory factory) => Services.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();

    protected override void RegisterTypes(IServiceCollection services) {
        base.RegisterTypes(services);
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }

    [SetUp]
    public void SetUp() {
        CreateDatabase();
        StartTest();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        ObjectReflectorConfiguration.NoValidate = true;
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CleanUpDatabase();
    }

    protected RestfulObjectsControllerBase Api() {
        var sp = GetConfiguredContainer();
        var api = sp.GetService<RestfulObjectsController>();
        return Helpers.SetMockContext(api, sp);
    }

    [Test]
    public void TestGetServices() {
        var api = Api();
        var result = api.GetServices();
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var val = parsedResult.GetValue("value") as JArray;

        Assert.IsNotNull(val);
        Assert.AreEqual(13, val.Count);

        var firstItem = val.First;

        firstItem.AssertServiceLink(nameof(MenuTestFunctions), "GET", typeof(MenuTestFunctions).FullName);
    }

    [Test]
    public void TestGetService() {
        var api = Api();
        var result = api.GetService(typeof(MenuTestFunctions).FullName);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(MenuTestFunctions), parsedResult["title"].ToString());
        Assert.AreEqual(typeof(MenuTestFunctions).FullName, parsedResult["serviceId"].ToString());

        var members = parsedResult["members"] as JObject;
        Assert.AreEqual(typeof(MenuTestFunctions).GetMethods(BindingFlags.Static | BindingFlags.Public).Length, members?.Count);

        var function = members[nameof(MenuTestFunctions.FindByName)];

        Assert.AreEqual("findbyname_group", function["extensions"]["x-ro-nof-menuPath"].ToString());

        function.AssertAction(nameof(MenuTestFunctions.FindByName));
        function["extensions"].AssertExtensions(8);

        var links = function["links"] as JArray;

        Assert.AreEqual(2, links.Count);

        var invokeLink = links.Last;

        invokeLink.AssertMenuInvokeLink("{\r\n  \"searchString\": {\r\n    \"value\": null\r\n  }\r\n}", "GET", typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByName));
    }

    [Test]
    public void TestGetServiceAction() {
        var api = Api();
        var result = api.GetServiceAction(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByName));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(MenuTestFunctions.FindByName), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());
        var parameter = parameters["searchString"];
        Assert.AreEqual(2, parameter.Count());
        var links = parameter["links"];
        var extensions = parameter["extensions"];
        Assert.AreEqual(0, links.Count());
        Assert.AreEqual(7, extensions.Count());
    }

    [Test]
    public void TestGetServiceEnumAction() {
        var api = Api();
        var result = api.GetServiceAction(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByEnum));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("number", parsedResult["parameters"]["eParm"]["extensions"]["returnType"].ToString());
    }

    [Test]
    public void TestInvokeServiceActionThatReturnsSingleItemList() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "searchString", new ScalarValue("Fred") } } };
        var result = api.GetInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByName), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("list", parsedResult["resultType"].ToString());

        var resultObj = parsedResult["result"];
        var value = resultObj["value"] as JArray;

        Assert.AreEqual(1, value.Count);

        value[0].AssertObjectElementLink("Fred", "GET", Helpers.FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeServiceActionThatReturnsMultipleItemList() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "length", new ScalarValue(4) } } };
        var result = api.GetInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByLength), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
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
    public void TestInvokeServiceActionThatReturnsRandomItem() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.Random), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
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
            case "4":
                resultObj.AssertObject("hide it", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "4");
                break;
            default:
                Assert.Fail("unexpected result");
                break;
        }
    }

    [Test]
    public void TestInvokeServiceActionThatGeneratesWarningNoObject() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "number", new ScalarValue("5") } } };
        var result = api.PostInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByNumber), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("object", parsedResult["resultType"].ToString());

        Assert.AreEqual("There is no matching object", parsedResult["extensions"]["x-ro-nof-warnings"][0].ToString());

        var resultObj = parsedResult["result"];

        Assert.AreEqual("", resultObj.ToString());
    }

    [Test]
    public void TestInvokeServiceActionThatGeneratesWarningObject() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "number", new ScalarValue("1") } } };
        var result = api.PostInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.FindByNumber), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("object", parsedResult["resultType"].ToString());

        Assert.AreEqual(null, parsedResult["extensions"]["x-ro-nof-warnings"]);

        Assert.AreEqual("object", parsedResult["resultType"].ToString());

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
    }

    private static string FormatForTest(DateTime dt) => $"{dt.Year}-{dt.Month:00}-{dt.Day:00}";

    [Test]
    public void TestGetServiceActionWithAnnotatedDefaults() {
        var api = Api();
        var result = api.GetServiceAction(typeof(DateMenuFunctions).FullName, nameof(DateMenuFunctions.DateWithDefault));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(DateRecordFunctions.DateWithDefault), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());
        var psd = parameters["dt"];

        Assert.AreEqual(FormatForTest(DateTime.UtcNow.AddDays(22)), psd["default"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithChoices() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithChoices));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoices), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());
        var choices = parameters["record"]["choices"];

        Assert.AreEqual(4, choices.Count());
        Assert.AreEqual("Fred", choices[0]["title"].ToString());
        Assert.AreEqual("Bill", choices[1]["title"].ToString());
        Assert.AreEqual("Jack", choices[2]["title"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithChoicesNoContext() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithChoicesNoContext));
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
    public void TestGetServiceActionWithChoicesWithParameters() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithChoicesWithParameters));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoicesWithParameters), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(3, parameters.Count());
        var prompt = parameters["record"]["links"][0];

        Assert.AreEqual(2, prompt["arguments"].Count());
        Assert.AreEqual(@"http://localhost/menus/NakedFunctions.Rest.Test.Data.ChoicesMenuFunctions/actions/WithChoicesWithParameters/params/record/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithChoicesWithParametersNoContext() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithChoicesWithParametersNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ChoicesMenuFunctions.WithChoicesWithParametersNoContext), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(3, parameters.Count());
        var prompt = parameters["record"]["links"][0];

        Assert.AreEqual(2, prompt["arguments"].Count());
        Assert.AreEqual(@"http://localhost/menus/NakedFunctions.Rest.Test.Data.ChoicesMenuFunctions/actions/WithChoicesWithParametersNoContext/params/record/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithMultipleChoices() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithMultipleChoices));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ChoicesMenuFunctions.WithMultipleChoices), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(2, parameters.Count());

        var choices = parameters["simpleRecords"]["choices"];

        Assert.AreEqual(4, choices.Count());
        Assert.AreEqual("Fred", choices[0]["title"].ToString());
        Assert.AreEqual("Bill", choices[1]["title"].ToString());
        Assert.AreEqual("Jack", choices[2]["title"].ToString());

        var prompt = parameters["dateRecords"]["links"][0];

        Assert.AreEqual(1, prompt["arguments"].Count());
        Assert.AreEqual(@"http://localhost/menus/NakedFunctions.Rest.Test.Data.ChoicesMenuFunctions/actions/WithMultipleChoices/params/dateRecords/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithMultipleChoicesNoContext() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ChoicesMenuFunctions).FullName, nameof(ChoicesMenuFunctions.WithMultipleChoicesNoContext));
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
        Assert.AreEqual(@"http://localhost/menus/NakedFunctions.Rest.Test.Data.ChoicesMenuFunctions/actions/WithMultipleChoicesNoContext/params/dateRecords/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetServiceActionCreateNew() {
        var api = Api();
        var result = api.GetServiceAction(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.CreateNewFunction));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("Name,Name1,Id", parsedResult["extensions"]["x-ro-nof-createNew"].ToString());
    }

   

    [Test]
    public void TestGetMenuPromptWithMultipleChoices() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simplerecords", new ListValue(new IValue[] { new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") }) } } };
        var result = api.GetParameterPromptOnMenu(nameof(ChoicesMenuFunctions), nameof(ChoicesMenuFunctions.WithMultipleChoices), "dateRecords", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("dateRecords", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        Assert.AreEqual(1, choices.Count());
        Assert.IsTrue(choices[0]["title"].ToString().StartsWith("DateRecord"));
    }

    [Test]
    public void TestGetMenuPromptWithMultipleChoicesNoContext() {
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
    public void TestGetServiceActionWithDefaults() {
        var api = Api();
        var result = api.GetServiceAction(typeof(DefaultedMenuFunctions).FullName, nameof(DefaultedMenuFunctions.WithDefaults));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
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
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
        Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithValidateSuccess() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateFail() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithCrossValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateSuccess() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithCrossValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestGetMenuActionWithDisable1() {
        var api = Api();
        var result = api.GetService(typeof(DisabledMenuFunctions).FullName);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["WithDisabled1"]);
    }

    [Test]
    public void TestGetMenuActionWithDisable2() {
        var api = Api();
        var result = api.GetService(typeof(DisabledMenuFunctions).FullName);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["WithDisabled2"]);
    }

    [Test]
    public void TestGetMenuActionWithHidden1() {
        var api = Api();
        var result = api.GetService(typeof(HiddenMenuFunctions).FullName);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["WithHidden1"]);
    }

    [Test]
    public void TestGetMenuActionWithHidden2() {
        var api = Api();
        var result = api.GetService(typeof(HiddenMenuFunctions).FullName);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["WithHidden2"]);
    }

    [Test]
    public void TestGetMenuActionWithAutoComplete() {
        var api = Api();
        var result = api.GetMenuAction(typeof(AutoCompleteMenuFunctions).FullName, nameof(AutoCompleteMenuFunctions.WithAutoComplete));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(AutoCompleteMenuFunctions.WithAutoComplete), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());

        var parameter = parameters["simpleRecord"];
        Assert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
        Assert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
        Assert.AreEqual("http://localhost/menus/NakedFunctions.Rest.Test.Data.AutoCompleteMenuFunctions/actions/WithAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") } } };

        var result = api.GetInvokeOnMenu(typeof(AutoCompleteMenuFunctions).FullName, nameof(AutoCompleteMenuFunctions.WithAutoComplete), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("Fred", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetMenuActionWithSingleAutoComplete() {
        var api = Api();
        var result = api.GetMenuAction(typeof(AutoCompleteMenuFunctions).FullName, nameof(AutoCompleteMenuFunctions.WithSingleAutoComplete));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(AutoCompleteMenuFunctions.WithSingleAutoComplete), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());

        var parameter = parameters["simpleRecord"];
        Assert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
        Assert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
        Assert.AreEqual("http://localhost/menus/NakedFunctions.Rest.Test.Data.AutoCompleteMenuFunctions/actions/WithSingleAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
    }

    [Test]
    public void TestInvokeActionWithSingleAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") } } };

        var result = api.GetInvokeOnMenu(typeof(AutoCompleteMenuFunctions).FullName, nameof(AutoCompleteMenuFunctions.WithSingleAutoComplete), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("Fred", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetServiceActionWithValidateNoContext() {
        var api = Api();
        var result = api.GetServiceAction(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithValidationNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ValidatedRecordFunctions.WithValidationNoContext), parsedResult["id"].ToString());

        var parameters = parsedResult["parameters"];
        Assert.AreEqual(1, parameters.Count());
    }

    [Test]
    public void TestInvokeServiceActionWithValidateFailNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
        Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeServiceActionWithValidateSuccessNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeServiceActionWithCrossValidateFailNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithCrossValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeServiceActionWithCrossValidateSuccessNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ValidatedMenuFunctions).FullName, nameof(ValidatedMenuFunctions.WithCrossValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeGetViewModel() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("1") } } };
        var result = api.GetInvokeOnMenu(typeof(ViewModelMenuFunctions).FullName, nameof(ViewModelMenuFunctions.GetViewModel), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("1", FullName<ViewModel>(), "1");
    }

    [Test]
    public void TestCreateNewRecordWithReferences() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.CreateNewWithExistingReferences), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test1-3-1-1", FullName<ReferenceRecord>(), "3");
    }

    [Test]
    public void TestCreateNewRecordNewReferences() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.CreateNewWithNewReferences), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test1-2-2-1", FullName<ReferenceRecord>(), "2");
        Assert.AreEqual("Test2", resultObj["members"]["UpdatedRecord"]["value"]["title"].ToString());
    }

    [Test]
    public void TestUpdateRecordWithReferences() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateExisting), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test2-1-1-1", FullName<ReferenceRecord>(), "1");
    }

    [Test]
    public void TestUpdateExistingAndReference() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateExistingAndReference), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test3-1-1-1", FullName<ReferenceRecord>(), "1");
        Assert.AreEqual("Jill", resultObj["members"]["UpdatedRecord"]["value"]["title"].ToString());
    }

    [Test]
    public void TestCreateNewRecordWithUpdatedReferences() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.CreateNewUpdateReference), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test4-4-1-1", FullName<ReferenceRecord>(), "4");
        Assert.AreEqual("Janet", resultObj["members"]["UpdatedRecord"]["value"]["title"].ToString());
    }

    [Test]
    public void TestCreateNewRecordWithCollection() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.CreateNewWithExistingCollection), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test1-2-12", FullName<CollectionRecord>(), "2");
    }

    [Test]
    public void TestUpdateRecordWithCollection() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateExistingCollectionRecord), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test2-1-1", FullName<CollectionRecord>(), "1");
    }

    [Test]
    public void TestUpdateExistingAndCollection() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateExistingAndCollection), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test3-2-2", FullName<CollectionRecord>(), "2");
    }

    [Test]
    public void TestUpdateExistingAndAddToCollection() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateExistingAndAddToCollection), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Test3-1-1", FullName<CollectionRecord>(), "1");
    }

    [Test]
    public void TestInvokeActionWithCollectionContribActions() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnMenu(typeof(CollectionMenuFunctions).FullName, nameof(CollectionMenuFunctions.GetQueryable), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual(5, resultObj["members"].Count());

        Assert.IsNull(resultObj["members"]["ContributedFunction1"]["extensions"]["returnType"]);
        Assert.AreEqual("POST", resultObj["members"]["ContributedFunction1"]["links"][1]["method"].ToString());
        Assert.AreEqual(1, resultObj["members"]["ContributedFunction1"]["parameters"].Count());

        Assert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", resultObj["members"]["ContributedFunction2"]["extensions"]["returnType"].ToString());
        Assert.AreEqual("POST", resultObj["members"]["ContributedFunction2"]["links"][1]["method"].ToString());
        Assert.AreEqual(1, resultObj["members"]["ContributedFunction2"]["parameters"].Count());

        Assert.AreEqual("list", resultObj["members"]["ContributedFunction3"]["extensions"]["returnType"].ToString());
        Assert.AreEqual("POST", resultObj["members"]["ContributedFunction3"]["links"][1]["method"].ToString());
        Assert.AreEqual(2, resultObj["members"]["ContributedFunction3"]["parameters"].Count());

        Assert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", resultObj["members"]["ContributedFunction4"]["extensions"]["returnType"].ToString());
        Assert.AreEqual("GET", resultObj["members"]["ContributedFunction4"]["links"][1]["method"].ToString());
        Assert.AreEqual(1, resultObj["members"]["ContributedFunction4"]["parameters"].Count());

        Assert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", resultObj["members"]["ContributedFunction5"]["extensions"]["returnType"].ToString());
        Assert.AreEqual(4, resultObj["members"]["ContributedFunction5"]["parameters"]["psr"]["choices"].Count());
    }

    [Test]
    public void TestInvokeServiceActionThatReturnsAlternateKeyObject() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnMenu(typeof(MenuTestFunctions).FullName, nameof(MenuTestFunctions.AlternateKey), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("1", resultObj["instanceId"].ToString());
        Assert.AreEqual("http://localhost/objects/NakedFunctions.Rest.Test.Data.AlternateKeyRecord/1", resultObj["links"][0]["href"].ToString());
    }

    [Test]
    public void TestNtoN1() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.CreateNtoN), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("name1-1-1", FullName<NToNCollectionRecord1>(), "1");
    }

    [Test]
    public void TestNtoN2() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.PostInvokeOnMenu(typeof(ReferenceMenuFunctions).FullName, nameof(ReferenceMenuFunctions.UpdateNtoN), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("name1-1-12", FullName<NToNCollectionRecord1>(), "1");
    }
}