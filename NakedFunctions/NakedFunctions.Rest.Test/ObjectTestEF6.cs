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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NakedFunctions.Rest.Test;

public class ObjectTestEF6 : AcceptanceTestCase {
    protected override Type[] Functions { get; } = {
        typeof(SimpleRecordFunctions),
        typeof(DateRecordFunctions),
        typeof(ChoicesRecordFunctions),
        typeof(DefaultedRecordFunctions),
        typeof(ValidatedRecordFunctions),
        typeof(DisabledRecordFunctions),
        typeof(HiddenRecordFunctions),
        typeof(AutoCompleteRecordFunctions),
        typeof(DisplayAsPropertyRecordFunctions),
        typeof(ViewModelFunctions),
        typeof(OrderedRecordFunctions),
        typeof(CollectionContributedFunctions),
        typeof(EditRecordFunctions),
        typeof(DeleteRecordFunctions),
        typeof(ImmutableCollectionRecordFunctions),
        typeof(UrlLinkFunctions),
        typeof(NullableParameterFunctions),
        typeof(MultilineParameterFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(SimpleRecord),
        typeof(DateRecord),
        typeof(EnumRecord),
        typeof(GuidRecord),
        typeof(TestEnum),
        typeof(ReferenceRecord),
        typeof(DisplayAsPropertyRecord),
        typeof(ViewModel),
        typeof(UpdatedRecord),
        typeof(OrderedRecord),
        typeof(CollectionRecord),
        typeof(EditRecord),
        typeof(DeleteRecord),
        typeof(BoundedRecord),
        typeof(ByteArrayRecord),
        typeof(MaskRecord),
        typeof(HiddenRecord),
        typeof(AlternateKeyRecord),
        typeof(UrlLinkRecord),
        typeof(NToNCollectionRecord1),
        typeof(NToNCollectionRecord2),
        typeof(MultilineRecord)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Services { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new ObjectDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected virtual void CleanUpDatabase() {
        ObjectDbContext.Delete();
    }

    protected virtual void CreateDatabase() { }

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

    private JObject GetObject(string type, string id) {
        var api = Api().AsGet();
        var result = api.GetObject(type, id);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private static string FullName<T>() => typeof(T).FullName;

    [Test]
    public void TestGetObjectAction() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(SimpleRecordFunctions.EditSimpleRecord), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
        var parameter = parameters["name"];
        ClassicAssert.AreEqual(3, parameter.Count());
        var links = parameter["links"];
        var extensions = parameter["extensions"];
        ClassicAssert.AreEqual(0, links.Count());
        ClassicAssert.AreEqual(9, extensions.Count());
    }

    [Test]
    public void TestGetObjectHints() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Hint1", parsedResult["extensions"]["x-ro-nof-presentationHint"].ToString());
        ClassicAssert.AreEqual("Hint2", parsedResult["members"]["Name"]["extensions"]["x-ro-nof-presentationHint"].ToString());
    }

    [Test]
    public void TestGetObjectRestExtensions() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Value1", parsedResult["extensions"]["Name1"].ToString());
        ClassicAssert.AreEqual("Value2", parsedResult["members"]["Name"]["extensions"]["Name2"].ToString());
    }

    [Test]
    public void TestGetObjectMask() {
        var api = Api();
        var result = api.GetObject(FullName<MaskRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Title", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("Title a mask", parsedResult["members"]["MaskRecordProperty"]["value"]["title"].ToString());
    }

    [Test]
    public void TestGetObjectDuplicates() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["Duplicate0"]);
    }

    [Test]
    public void TestGetCollectionDuplicate() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.GetQueryable), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["result"]["members"]["Duplicate1"]);
    }

    [Test]
    public void TestGetObjectActionHints() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Hint3", parsedResult["extensions"]["x-ro-nof-presentationHint"].ToString());
        ClassicAssert.AreEqual("Hint4", parsedResult["parameters"]["name"]["extensions"]["x-ro-nof-presentationHint"].ToString());
    }

    [Test]
    public void TestGetObjectActionRestExtension() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Value3", parsedResult["extensions"]["Name3"].ToString());
        ClassicAssert.AreEqual("Value4", parsedResult["parameters"]["name"]["extensions"]["Name4"].ToString());
    }

    [Test]
    public void TestGetObjectGrouping() {
        var api = Api();
        var result = api.GetObject(FullName<OrderedRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("name_group", parsedResult["members"]["Name"]["extensions"]["x-ro-nof-propertyGrouping"].ToString());
        ClassicAssert.AreEqual("name1_group", parsedResult["members"]["Name1"]["extensions"]["x-ro-nof-propertyGrouping"].ToString());
        ClassicAssert.AreEqual("function1_group", parsedResult["members"]["Function1"]["extensions"]["x-ro-nof-menuPath"].ToString());
        ClassicAssert.AreEqual("function2_group", parsedResult["members"]["Function2"]["extensions"]["x-ro-nof-menuPath"].ToString());
    }

    [Test]
    public void TestGetObjectActionCreateNew() {
        var api = Api();
        var result = api.GetAction(FullName<OrderedRecord>(), "1", nameof(OrderedRecordFunctions.CreateNewFunction));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Name,Name1,Id", parsedResult["extensions"]["x-ro-nof-createNew"].ToString());
    }

    [Test]
    public void TestGetObjectActionPassword() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.PasswordParmSimpleRecord));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("password", parsedResult["parameters"]["parm"]["extensions"]["x-ro-nof-dataType"].ToString());
    }

    [Test]
    public void TestGetEnumObject() {
        var api = Api();
        var result = api.GetObject(FullName<EnumRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("number", parsedResult["members"]["TestEnum"]["extensions"]["returnType"].ToString());
    }

    [Test]
    public void TestGetGuidObject() {
        var api = Api();
        var result = api.GetObject(FullName<GuidRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);
    }

    [Test]
    public void TestGetByteArrayObject() {
        var api = Api();
        var result = api.GetObject(FullName<ByteArrayRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);
    }

    [Test]
    public void TestGetEnumAction() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EnumParmSimpleRecord));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("number", parsedResult["parameters"]["eParm"]["extensions"]["returnType"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistSimpleRecord() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Fred4") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred4", FullName<SimpleRecord>(), "1");
        ClassicAssert.AreEqual("Fred4", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeDeleteRecord() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvoke(FullName<DeleteRecord>(), "1", nameof(DeleteRecordFunctions.DeleteFunction), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
    }

    [Test]
    public void TestInvokeDeleteRecordAndReturn() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvoke(FullName<DeleteRecord>(), "2", nameof(DeleteRecordFunctions.DeleteFunctionAndReturn), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
    }

    [Test]
    public void TestInvokeDeleteRecordAndReturnDeleted() {
        var api = Api();
        var result = api.GetObject(FullName<DeleteRecord>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.NotFound, sc);
    }

    [Test]
    public void TestInvokeUpdateAndPersistSimpleRecordWithPostPersist() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Fred4") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecordWithPostPersist), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred4Updated", FullName<SimpleRecord>(), "1");
        ClassicAssert.AreEqual("Fred4Updated", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistSimpleRecordWithRepeatedPostPersist() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Fred5") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecordWithRepeatedPostPersist), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred5UpdatedUpdatedUpdated", FullName<SimpleRecord>(), "1");
        ClassicAssert.AreEqual("Fred5UpdatedUpdatedUpdated", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeCreateSimpleRecord() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Ellen") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.CreateSimpleRecord), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
        ClassicAssert.AreEqual("Ellen", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeCreateSimpleRecordWithPostPersist() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Ellen") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.CreateSimpleRecordWithPostPersist), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
        ClassicAssert.AreEqual("EllenUpdated", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeCreateSimpleRecordWithRepeatedPostPersist() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Ellen") } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.CreateSimpleRecordWithRepeatedPostPersist), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
        ClassicAssert.AreEqual("EllenUpdatedUpdatedUpdated", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeCurrentUserAsEmployee() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.SimpleRecordAsCurrentUser), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
        ClassicAssert.AreEqual("Test", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeCurrentUserAsEmployeeReset() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.SimpleRecordAsReset), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
        ClassicAssert.AreEqual("Fred", resultObj["members"]["Name"]["value"].ToString());
    }

    private static string FormatForTest(DateTime dt) => $"{dt.Year}-{dt.Month:00}-{dt.Day:00}";

    [Test]
    public void TestGetObjectActionWithDateDefaults() {
        var api = Api();
        var result = api.GetAction(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.EditDates));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(DateRecordFunctions.EditDates), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(2, parameters.Count());
        var psd = parameters["startDate"];

        ClassicAssert.AreEqual(FormatForTest(DateTime.Today), psd["default"].ToString());

        var ped = parameters["endDate"];

        ClassicAssert.AreEqual(FormatForTest(DateTime.Today.AddDays(90)), ped["default"].ToString());
    }

    [Test]
    public void TestGetObjectActionWithAnnotatedDefaults() {
        var api = Api();
        var result = api.GetAction(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.DateWithDefault));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(DateRecordFunctions.DateWithDefault), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
        var psd = parameters["dt"];

        ClassicAssert.AreEqual(FormatForTest(DateTime.UtcNow.AddDays(22)), psd["default"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithChoices() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoices));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithChoices), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
        var choices = parameters["record"]["choices"];

        ClassicAssert.AreEqual(4, choices.Count());
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
        ClassicAssert.AreEqual("Bill", choices[1]["title"].ToString());
        ClassicAssert.AreEqual("Jack", choices[2]["title"].ToString());
        ClassicAssert.AreEqual("hide it", choices[3]["title"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithBoundedChoices() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithBoundedChoices));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithBoundedChoices), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
        var choices = parameters["record"]["choices"];

        ClassicAssert.AreEqual(2, choices.Count());
        ClassicAssert.AreEqual("One", choices[0]["title"].ToString());
        ClassicAssert.AreEqual("Two", choices[1]["title"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithChoicesNoContext() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesNoContext), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
        var choices = parameters["record"]["choices"];

        ClassicAssert.AreEqual(1, choices.Count());
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithChoicesWithParameters() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParameters));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesWithParameters), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(3, parameters.Count());
        var prompt = parameters["record"]["links"][0];

        ClassicAssert.AreEqual(2, prompt["arguments"].Count());
        ClassicAssert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithChoicesWithParameters/params/record/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithChoicesWithParametersNoContext() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(3, parameters.Count());
        var prompt = parameters["record"]["links"][0];

        ClassicAssert.AreEqual(2, prompt["arguments"].Count());
        ClassicAssert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithChoicesWithParametersNoContext/params/record/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithMultipleChoices() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoices));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithMultipleChoices), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(2, parameters.Count());

        var choices = parameters["simpleRecords"]["choices"];

        ClassicAssert.AreEqual(4, choices.Count());
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
        ClassicAssert.AreEqual("Bill", choices[1]["title"].ToString());
        ClassicAssert.AreEqual("Jack", choices[2]["title"].ToString());
        ClassicAssert.AreEqual("hide it", choices[3]["title"].ToString());

        var prompt = parameters["dateRecords"]["links"][0];

        ClassicAssert.AreEqual(1, prompt["arguments"].Count());
        ClassicAssert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithMultipleChoices/params/dateRecords/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithMultipleChoicesNoContext() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(2, parameters.Count());

        var choices = parameters["simpleRecords"]["choices"];

        ClassicAssert.AreEqual(1, choices.Count());
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());

        var prompt = parameters["dateRecords"]["links"][0];

        ClassicAssert.AreEqual(1, prompt["arguments"].Count());
        ClassicAssert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithMultipleChoicesNoContext/params/dateRecords/prompt", prompt["href"].ToString());
    }

    [Test]
    public void TestGetObjectPrompt() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm1", new ScalarValue("1") }, { "parm2", new ScalarValue("J") } } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParameters), "record", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("record", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(1, choices.Count());
        ClassicAssert.AreEqual("Jack", choices[0]["title"].ToString());
    }

    [Test]
    public void TestGetObjectPromptNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm1", new ScalarValue("1") }, { "parm2", new ScalarValue("J") } } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext), "record", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("record", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(1, choices.Count());
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
    }

    [Test]
    public void TestGetObjectPromptWithMultipleChoices() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simplerecords", new ListValue(new IValue[] { new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") }) } } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoices), "dateRecords", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("dateRecords", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(1, choices.Count());
        ClassicAssert.IsTrue(choices[0]["title"].ToString().StartsWith("DateRecord"));
    }

    [Test]
    public void TestGetObjectPromptWithMultipleChoicesNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simplerecords", new ListValue(new IValue[] { new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") }) } } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext), "dateRecords", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("dateRecords", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(0, choices.Count());
    }

    [Test]
    public void TestInvokeEditDates() {
        var api = Api().AsPost();
        var startDate = new DateTime(2000, 1, 1);
        var endDate = new DateTime(2001, 12, 31);
        var map = new ArgumentMap {
            Map = new Dictionary<string, IValue> {
                { "startDate", new ScalarValue(startDate) },
                { "endDate", new ScalarValue(endDate) }
            }
        };

        var result = api.PostInvoke(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.EditDates), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];
        var sd = resultObj["members"]["StartDate"]["value"].ToString();
        var ed = resultObj["members"]["EndDate"]["value"].ToString();

        ClassicAssert.AreEqual(startDate, DateTime.Parse(sd));
        ClassicAssert.AreEqual(endDate, DateTime.Parse(ed));
    }

    [Test]
    public void TestGetRecordActionWithDefaults() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(DefaultedRecordFunctions.WithDefaults));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(DefaultedRecordFunctions.WithDefaults), parsedResult["id"].ToString());

        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(2, parameters.Count());

        ClassicAssert.AreEqual("101", parameters["default1"]["default"].ToString());
        ClassicAssert.AreEqual("Fred", parameters["default2"]["default"]["title"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithValidate() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ValidatedRecordFunctions.WithValidation), parsedResult["id"].ToString());

        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
    }

    [Test]
    public void TestInvokeRecordActionWithValidateFail() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
        ClassicAssert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithValidateSuccess() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestGetRecordActionWithValidateNoContext() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ValidatedRecordFunctions.WithValidationNoContext), parsedResult["id"].ToString());

        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());
    }

    [Test]
    public void TestInvokeRecordActionWithValidateFailNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
        ClassicAssert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithValidateSuccessNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateFail() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateSuccess() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeRecordActionWithFileAttachmentValidateSuccess() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "fp", new FileValue(Convert.ToBase64String(new byte[] { 127, 127 }), "text/plain", "aFile") } } };
        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.FileAttachmentWithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        //resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestInvokeRecordActionWithFileAttachmentValidateFail() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "fp", new FileValue(Convert.ToBase64String(new byte[] { 127 }), "text/plain", "aFile") } } };
        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.FileAttachmentWithValidation), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("invalid", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateFailNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeRecordActionWithCrossValidateSuccessNoContext() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidationNoContext), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
    }

    [Test]
    public void TestGetRecordActionWithDisable1() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("disabled", parsedResult["members"]["WithDisabled1"]["disabledReason"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithDisable2() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["WithDisabled2"]["disabledReason"]);
    }

    [Test]
    public void TestGetRecordActionWithDisabledParameter() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["WithDefaults"]["parameters"]["default1"]["disabledReason"]);
        ClassicAssert.AreEqual("Always disabled", parsedResult["members"]["WithDisabledParameter"]["parameters"]["disabledParameter"]["disabledReason"].ToString());
    }

    [Test]
    public void TestGetRecordActionWithHidden1() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["WithHidden1"]);
    }

    [Test]
    public void TestGetRecordActionWithHidden2() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["WithHidden2"]);
    }

    [Test]
    public void TestGetRecordActionWithNotHiddenProperty() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
    }

    [Test]
    public void TestGetRecordActionWithHiddenProperty() {
        var api = Api();
        var result = api.GetObject(FullName<SimpleRecord>(), "4");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["Name"]);
    }

    [Test]
    public void TestGetObjectActionWithAutoComplete() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(AutoCompleteRecordFunctions.WithAutoComplete), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());

        var parameter = parameters["simpleRecord"];
        ClassicAssert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
        ClassicAssert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
        ClassicAssert.AreEqual("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
    }

    [Test]
    public void TestInvokeObjectActionPromptWithAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>(), ReservedArguments = new ReservedArguments { SearchTerm = "Fr" } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete), "simpleRecord", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("simpleRecord", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(2, choices.Count()); // tests PageSize
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
        ClassicAssert.AreEqual("Bill", choices[1]["title"].ToString());
    }

    [Test]
    public void TestInvokeActionWithAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") } } };

        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetObjectActionWithSingleAutoComplete() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithSingleAutoComplete));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(AutoCompleteRecordFunctions.WithSingleAutoComplete), parsedResult["id"].ToString());
        var parameters = parsedResult["parameters"];
        ClassicAssert.AreEqual(1, parameters.Count());

        var parameter = parameters["simpleRecord"];
        ClassicAssert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
        ClassicAssert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
        ClassicAssert.AreEqual("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithSingleAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
    }

    [Test]
    public void TestInvokeObjectActionPromptWithSingleAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>(), ReservedArguments = new ReservedArguments { SearchTerm = "Fr" } };
        var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithSingleAutoComplete), "simpleRecord", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("simpleRecord", parsedResult["id"].ToString());
        var choices = parsedResult["choices"];
        ClassicAssert.AreEqual(1, choices.Count()); // tests PageSize
        ClassicAssert.AreEqual("Fred", choices[0]["title"].ToString());
    }

    [Test]
    public void TestInvokeActionWithSingleAutoComplete() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") } } };

        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithSingleAutoComplete), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetObjectDisplayAsProperty() {
        var api = Api();
        var result = api.GetObject(FullName<DisplayAsPropertyRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(4, parsedResult["members"].Count());
        ClassicAssert.AreEqual("1", parsedResult["members"]["DisplayAsProperty"]["value"]["title"].ToString());

        ClassicAssert.AreEqual("1", parsedResult["members"]["DisplayAsCollection"]["value"][0]["title"].ToString());

        // check ordering

        ClassicAssert.IsTrue(parsedResult["members"].ToArray()[0].ToString().StartsWith("\"Name"));
        ClassicAssert.IsTrue(parsedResult["members"].ToArray()[1].ToString().StartsWith("\"DisplayAsProperty"));
        ClassicAssert.IsTrue(parsedResult["members"].ToArray()[2].ToString().StartsWith("\"Id"));
        ClassicAssert.IsTrue(parsedResult["members"].ToArray()[3].ToString().StartsWith("\"DisplayAsCollection"));
    }

    [Test]
    public void TestGetViewModel() {
        var api = Api();
        var result = api.GetObject(FullName<ViewModel>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("1", parsedResult["title"].ToString());

        ClassicAssert.AreEqual(2, parsedResult["members"].Count());
        ClassicAssert.AreEqual("1", parsedResult["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestInvokeActionOnViewModel() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("2") } } };

        var result = api.GetInvoke(FullName<ViewModel>(), "1", nameof(ViewModelFunctions.UpdateName), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("2", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetObjectWithLCA() {
        var api = Api();
        var result = api.GetObject(FullName<CollectionRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, parsedResult["members"].Count());

        ClassicAssert.AreEqual(3, parsedResult["members"]["UpdatedRecords"]["members"].Count());

        ClassicAssert.IsNotNull(parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunction"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunctionByMemberOrder"]);

        ClassicAssert.AreEqual(1, parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunction"]["parameters"].Count());
        ClassicAssert.AreEqual(2, parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunction1"]["parameters"].Count());
        ClassicAssert.AreEqual(0, parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunctionByMemberOrder"]["parameters"].Count());
        ClassicAssert.AreEqual(4, parsedResult["members"]["UpdatedRecords"]["members"]["LocalContributedFunction1"]["parameters"]["psr"]["choices"].Count());
    }

    [Test]
    public void TestGetObjectActionWithEdit() {
        var api = Api();
        var result = api.GetAction(FullName<EditRecord>(), "1", nameof(EditRecordFunctions.EditFunction));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(EditRecordFunctions.EditFunction), parsedResult["id"].ToString());

        ClassicAssert.AreEqual("SimpleRecord,Name", parsedResult["extensions"]["x-ro-nof-editProperties"].ToString());

        ClassicAssert.AreEqual("Fred", parsedResult["parameters"]["simpleRecord"]["default"]["title"].ToString());
        ClassicAssert.AreEqual("Jane", parsedResult["parameters"]["name"]["default"].ToString());
    }

    [Test]
    public void TestGetObjectActionWithImmutableList() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ImmutableCollectionRecordFunctions.CollectionReturn));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ImmutableCollectionRecordFunctions.CollectionReturn), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("list", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", parsedResult["extensions"]["elementType"].ToString());
    }

    [Test]
    public void TestGetObjectActionWithImmutableList1() {
        var api = Api();
        var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ImmutableCollectionRecordFunctions.CollectionReturn1));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ImmutableCollectionRecordFunctions.CollectionReturn1), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("list", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", parsedResult["extensions"]["elementType"].ToString());
    }

    [Test]
    public void TestInvokeActionWithImmutableList() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ImmutableCollectionRecordFunctions.CollectionReturn), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual(4, resultObj["value"].Count());
    }

    [Test]
    public void TestGetObjectHidden() {
        var api = Api();
        var result = api.GetObject(FullName<HiddenRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["HiddenProperty"]);
        ClassicAssert.IsNull(parsedResult["members"]["HiddenFunction"]);
    }

    [Test]
    public void TestGetObjectAlternateKey() {
        var api = Api();
        var result = api.GetObject(FullName<AlternateKeyRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);
    }

    [Test]
    public void TestGetPropertyURLLinks() {
        var api = Api();
        var result = api.GetObject(FullName<UrlLinkRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("False,", parsedResult["members"]["Link1"]["extensions"]["x-ro-nof-urlLink"].ToString());
        ClassicAssert.AreEqual("True,", parsedResult["members"]["Link2"]["extensions"]["x-ro-nof-urlLink"].ToString());
        ClassicAssert.AreEqual("False,Name", parsedResult["members"]["Link3"]["extensions"]["x-ro-nof-urlLink"].ToString());
        ClassicAssert.AreEqual("True,Name1", parsedResult["members"]["Link4"]["extensions"]["x-ro-nof-urlLink"].ToString());
    }

    [Test]
    public void TestGetFunctionURLLinks() {
        var api = Api();
        var result = api.GetObject(FullName<UrlLinkRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("True,Name1", parsedResult["members"]["LinkFunction"]["extensions"]["x-ro-nof-urlLink"].ToString());
    }

    [Test]
    public void TestErrorNotFound() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "eType", new ScalarValue(0) } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.Error), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.NotFound, sc);
    }

    [Test]
    public void TestErrorNotAuth() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "eType", new ScalarValue(1) } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.Error), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Unauthorized, sc);
    }

    [Test]
    public void TestErrorGeneral() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "eType", new ScalarValue(2) } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.Error), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.InternalServerError, sc);
    }

    [Test]
    public void TestErrorMultiple() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "eType", new ScalarValue(3) } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.Error), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.NotFound, sc);
    }

    [Test]
    public void TestErrorString() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "eType", new ScalarValue(4) } } };

        var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.Error), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.InternalServerError, sc);
    }

    [Test]
    public void TestGetFunctionWithNullability() {
        var api = Api();
        var result = api.GetObject(FullName<UrlLinkRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("False", parsedResult["members"]["NullableFunction"]["parameters"]["p1"]["extensions"]["optional"].ToString());
        ClassicAssert.AreEqual("False", parsedResult["members"]["NullableFunction"]["parameters"]["p2"]["extensions"]["optional"].ToString());
    }

    [Test]
    public virtual void TestMultilineMandatoryParameter() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm", new ScalarValue("fred") } } };
        var result = api.GetInvoke(FullName<MultilineRecord>(), "1", nameof(MultilineParameterFunctions.Function), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("ok", parsedResult["result"]["value"].ToString());
    }

    [Test]
    public virtual void TestMultilineMandatoryParameterEmpty() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm", new ScalarValue("") } } };
        var result = api.GetInvoke(FullName<MultilineRecord>(), "1", nameof(MultilineParameterFunctions.Function), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual(422, sc);
    }

    [Test]
    public virtual void TestMultilineOptionalParameter() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm", new ScalarValue("fred") } } };
        var result = api.GetInvoke(FullName<MultilineRecord>(), "1", nameof(MultilineParameterFunctions.Function1), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("ok1", parsedResult["result"]["value"].ToString());
    }

    [Test]
    public virtual void TestMultilineOptionalParameterEmpty() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm", new ScalarValue("") } } };
        var result = api.GetInvoke(FullName<MultilineRecord>(), "1", nameof(MultilineParameterFunctions.Function1), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("ok1", parsedResult["result"]["value"].ToString());
    }
}