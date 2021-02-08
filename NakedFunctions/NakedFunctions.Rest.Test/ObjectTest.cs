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
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Facade.Interface;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Rest;
using NakedObjects.Rest.Model;
using NakedObjects.Xat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {
    public class NullStringHasher : IStringHasher {
        public string GetHash(string toHash) => null;
    }

    public class ObjectTest : AcceptanceTestCase {
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
            typeof(CollectionContributedFunctions)
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
            typeof(OrderedRecord)
        };

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] {config => new ObjectDbContext()};

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
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            ObjectDbContext.Delete();
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
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }

        private static string FullName<T>() => typeof(T).FullName;

        [Test]
        public void TestGetObjectAction() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(SimpleRecordFunctions.EditSimpleRecord), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var parameter = parameters["name"];
            Assert.AreEqual(2, parameter.Count());
            var links = parameter["links"];
            var extensions = parameter["extensions"];
            Assert.AreEqual(0, links.Count());
            Assert.AreEqual(8, extensions.Count());

            // todo test rest of json
        }

        [Test]
        public void TestGetObjectHints() {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("Hint1", parsedResult["extensions"]["x-ro-nof-presentationHint"].ToString());
            Assert.AreEqual("Hint2", parsedResult["members"]["Name"]["extensions"]["x-ro-nof-presentationHint"].ToString());
        }

        [Test]
        public void TestGetObjectActionHints() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("Hint3", parsedResult["extensions"]["x-ro-nof-presentationHint"].ToString());
            Assert.AreEqual("Hint4", parsedResult["parameters"]["name"]["extensions"]["x-ro-nof-presentationHint"].ToString());
        }

        [Test]
        public void TestGetObjectGrouping()
        {
            var api = Api();
            var result = api.GetObject(FullName<OrderedRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("name_group", parsedResult["members"]["Name"]["extensions"]["x-ro-nof-propertyGrouping"].ToString());
            Assert.AreEqual("name1_group", parsedResult["members"]["Name1"]["extensions"]["x-ro-nof-propertyGrouping"].ToString());
            Assert.AreEqual("function1_group", parsedResult["members"]["Function1"]["extensions"]["x-ro-nof-menuPath"].ToString());
            Assert.AreEqual("function2_group", parsedResult["members"]["Function2"]["extensions"]["x-ro-nof-menuPath"].ToString());
        }



        [Test]
        public void TestGetObjectActionCreateNew()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.CreateNewFunction));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("True", parsedResult["extensions"]["x-ro-nof-createNew"].ToString());
        }

        [Test]
        public void TestGetObjectActionPassword() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.PasswordParmSimpleRecord));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("password", parsedResult["parameters"]["parm"]["extensions"]["x-ro-nof-dataType"].ToString());
        }

        [Test]
        public void TestGetEnumObject() {
            var api = Api();
            var result = api.GetObject(FullName<EnumRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("number", parsedResult["members"]["TestEnum"]["extensions"]["returnType"].ToString());
        }

        [Test]
        public void TestGetGuidObject() {
            var api = Api();
            var result = api.GetObject(FullName<GuidRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);
        }

        [Test]
        public void TestGetEnumAction() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EnumParmSimpleRecord));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("number", parsedResult["parameters"]["eParm"]["extensions"]["returnType"].ToString());
        }

        [Test]
        public void TestInvokeUpdateAndPersistSimpleRecord()
        {
            var api = Api().AsPut();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Fred4") } } };

            var result = api.PutInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.EditSimpleRecord), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred4", FullName<SimpleRecord>(), "1");
            Assert.AreEqual("Fred4", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeCreateSimpleRecord() {
            var api = Api().AsPost();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"name", new ScalarValue("Ellen")}}};

            var result = api.PostInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.CreateSimpleRecord), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());

            //resultObj.AssertObject("Ellen", FullName<UpdatedRecord>()", "4");
            Assert.AreEqual("Ellen", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeCurrentUserAsEmployee()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(SimpleRecordFunctions.SimpleRecordAsCurrentUser), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());

            Assert.AreEqual("Test", resultObj["members"]["Name"]["value"].ToString());
        }


        private static string FormatForTest(DateTime dt) => $"{dt.Year}-{dt.Month:00}-{dt.Day:00}";

        [Test]
        public void TestGetObjectActionWithDateDefaults() {
            var api = Api();
            var result = api.GetAction(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.EditDates));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(DateRecordFunctions.EditDates), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());
            var psd = parameters["startDate"];

            Assert.AreEqual(FormatForTest(DateTime.Today), psd["default"].ToString());

            var ped = parameters["endDate"];

            Assert.AreEqual(FormatForTest(DateTime.Today.AddDays(90)), ped["default"].ToString());
        }

        [Test]
        public void TestGetObjectActionWithAnnotatedDefaults() {
            var api = Api();
            var result = api.GetAction(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.DateWithDefault));
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
        public void TestGetRecordActionWithChoices() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoices));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithChoices), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var choices = parameters["record"]["choices"];

            Assert.AreEqual(4, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
            Assert.AreEqual("Bill", choices[1]["title"].ToString());
            Assert.AreEqual("Jack", choices[2]["title"].ToString());
            Assert.AreEqual("hide it", choices[3]["title"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithChoicesNoContext()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
            var choices = parameters["record"]["choices"];

            Assert.AreEqual(1, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
        }



        [Test]
        public void TestGetRecordActionWithChoicesWithParameters() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParameters));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesWithParameters), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(3, parameters.Count());
            var prompt = parameters["record"]["links"][0];

            Assert.AreEqual(2, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithChoicesWithParameters/params/record/prompt", prompt["href"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithChoicesWithParametersNoContext()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(3, parameters.Count());
            var prompt = parameters["record"]["links"][0];

            Assert.AreEqual(2, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithChoicesWithParametersNoContext/params/record/prompt", prompt["href"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithMultipleChoices() {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoices));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithMultipleChoices), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            var choices = parameters["simpleRecords"]["choices"];

            Assert.AreEqual(4, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
            Assert.AreEqual("Bill", choices[1]["title"].ToString());
            Assert.AreEqual("Jack", choices[2]["title"].ToString());
            Assert.AreEqual("hide it", choices[3]["title"].ToString());

            var prompt = parameters["dateRecords"]["links"][0];

            Assert.AreEqual(1, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithMultipleChoices/params/dateRecords/prompt", prompt["href"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithMultipleChoicesNoContext()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            var choices = parameters["simpleRecords"]["choices"];

            Assert.AreEqual(1, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
          

            var prompt = parameters["dateRecords"]["links"][0];

            Assert.AreEqual(1, prompt["arguments"].Count());
            Assert.AreEqual(@"http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithMultipleChoicesNoContext/params/dateRecords/prompt", prompt["href"].ToString());
        }


        [Test]
        public void TestGetObjectPrompt() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"parm1", new ScalarValue("1")}, {"parm2", new ScalarValue("J")}}};
            var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParameters), "record", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("record", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(1, choices.Count());
            Assert.AreEqual("Jack", choices[0]["title"].ToString());
        }

        [Test]
        public void TestGetObjectPromptNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "parm1", new ScalarValue("1") }, { "parm2", new ScalarValue("J") } } };
            var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithChoicesWithParametersNoContext), "record", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("record", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(1, choices.Count());
            Assert.AreEqual("Fred", choices[0]["title"].ToString());
        }


        [Test]
        public void TestGetObjectPromptWithMultipleChoices() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue> {{"simplerecords", new ListValue(new IValue[] {new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord")})}}};
            var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoices), "dateRecords", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("dateRecords", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(1, choices.Count());
            Assert.IsTrue(choices[0]["title"].ToString().StartsWith("DateRecord"));
        }

        [Test]
        public void TestGetObjectPromptWithMultipleChoicesNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simplerecords", new ListValue(new IValue[] { new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") }) } } };
            var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(ChoicesRecordFunctions.WithMultipleChoicesNoContext), "dateRecords", map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("dateRecords", parsedResult["id"].ToString());
            var choices = parsedResult["choices"];
            Assert.AreEqual(0, choices.Count());
        }


        [Test]
        public void TestInvokeEditDates()
        {
            var api = Api().AsPut();
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2001, 12, 31);
            var map = new ArgumentMap
            {
                Map = new Dictionary<string, IValue> {
                    {"startDate", new ScalarValue(startDate)},
                    {"endDate", new ScalarValue(endDate)}
                }
            };

            var result = api.PutInvoke(FullName<DateRecord>(), "1", nameof(DateRecordFunctions.EditDates), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];
            var sd = resultObj["members"]["StartDate"]["value"].ToString();
            var ed = resultObj["members"]["EndDate"]["value"].ToString();

            Assert.AreEqual(startDate, DateTime.Parse(sd));
            Assert.AreEqual(endDate, DateTime.Parse(ed));

            //resultObj.AssertObject("Fred4", FullName<UpdatedRecord>()", "1");
            //Assert.AreEqual("Fred4", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithDefaults()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(DefaultedRecordFunctions.WithDefaults));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(DefaultedRecordFunctions.WithDefaults), parsedResult["id"].ToString());

            var parameters = parsedResult["parameters"];
            Assert.AreEqual(2, parameters.Count());

            Assert.AreEqual("101", parameters["default1"]["default"].ToString());
            Assert.AreEqual("Fred", parameters["default2"]["default"]["title"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithValidate()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ValidatedRecordFunctions.WithValidation), parsedResult["id"].ToString());

            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
        }

        [Test]
        public void TestInvokeRecordActionWithValidateFail()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
            Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithValidateSuccess()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestGetRecordActionWithValidateNoContext()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ValidatedRecordFunctions.WithValidationNoContext), parsedResult["id"].ToString());

            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());
        }

        [Test]
        public void TestInvokeRecordActionWithValidateFailNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("2", parsedResult["validate1"]["value"].ToString());
            Assert.AreEqual("invalid", parsedResult["validate1"]["invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithValidateSuccessNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }


        [Test]
        public void TestInvokeRecordActionWithCrossValidateFail()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);

          
            Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithCrossValidateSuccess()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidation), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }

        [Test]
        public void TestInvokeRecordActionWithCrossValidateFailNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("2") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("invalid: 2:1", parsedResult["x-ro-invalidReason"].ToString());
        }


        [Test]
        public void TestInvokeRecordActionWithCrossValidateSuccessNoContext()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "validate1", new ScalarValue("1") }, { "validate2", new ScalarValue("1") } } };
            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(ValidatedRecordFunctions.WithCrossValidationNoContext), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", FullName<SimpleRecord>(), "1");
        }



        [Test]
        public void TestGetRecordActionWithDisable1() {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("disabled", parsedResult["members"]["WithDisabled1"]["disabledReason"].ToString());
        }

        [Test]
        public void TestGetRecordActionWithDisable2()
        {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["WithDisabled2"]["disabledReason"]);
        }

        [Test]
        public void TestGetRecordActionWithHidden1()
        {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["WithHidden1"]);
        }

        [Test]
        public void TestGetRecordActionWithHidden2()
        {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["WithHidden2"]);
        }

        [Test]
        public void TestGetRecordActionWithNotHiddenProperty()
        {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Name"]);
        }

        [Test]
        public void TestGetRecordActionWithHiddenProperty()
        {
            var api = Api();
            var result = api.GetObject(FullName<SimpleRecord>(), "4");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Name"]);
        }


        [Test]
        public void TestGetObjectActionWithAutoComplete()
        {
            var api = Api();
            var result = api.GetAction(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(AutoCompleteRecordFunctions.WithAutoComplete), parsedResult["id"].ToString());
            var parameters = parsedResult["parameters"];
            Assert.AreEqual(1, parameters.Count());

            var parameter = parameters["simpleRecord"];
            Assert.AreEqual("", parameter["links"][0]["arguments"]["x-ro-searchTerm"]["value"].ToString());
            Assert.AreEqual("2", parameter["links"][0]["extensions"]["minLength"].ToString());
            Assert.AreEqual("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1/actions/WithAutoComplete/params/simpleRecord/prompt", parameter["links"][0]["href"].ToString());
        }

        [Test]
        public void TestInvokeObjectActionPromptWithAutoComplete() {
            var api = Api();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue>(), ReservedArguments = new ReservedArguments {SearchTerm = "Fr"}};
            var result = api.GetParameterPrompt(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete), "simpleRecord", map);
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
        public void TestInvokeActionWithAutoComplete()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "simpleRecord", new ReferenceValue("http://localhost/objects/NakedFunctions.Rest.Test.Data.SimpleRecord/1", "simpleRecord") } } };

            var result = api.GetInvoke(FullName<SimpleRecord>(), "1", nameof(AutoCompleteRecordFunctions.WithAutoComplete), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("Fred", resultObj["title"].ToString());
        }

        [Test]
        public void TestGetObjectDisplayAsProperty()
        {
            var api = Api();
            var result = api.GetObject(FullName<DisplayAsPropertyRecord>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(3, parsedResult["members"].Count());
            Assert.AreEqual("DisplayAsPropertyRecord { Id = 1 }", parsedResult["members"]["DisplayAsProperty"]["value"]["title"].ToString());

            Assert.AreEqual("DisplayAsPropertyRecord { Id = 1 }", parsedResult["members"]["DisplayAsCollection"]["value"][0]["title"].ToString());

        }

        [Test]
        public void TestGetViewModel()
        {
            var api = Api();
            var result = api.GetObject(FullName<ViewModel>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("1", parsedResult["title"].ToString());

            Assert.AreEqual(2, parsedResult["members"].Count());
            Assert.AreEqual("1", parsedResult["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeActionOnViewModel()
        {
            var api = Api();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("2") } }  };

            var result = api.GetInvoke(FullName<ViewModel>(), "1", nameof(ViewModelFunctions.UpdateName), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("2", resultObj["title"].ToString());
        }

      



    }
}