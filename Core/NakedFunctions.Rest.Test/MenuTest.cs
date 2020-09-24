// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Rest;
using NakedObjects.Rest.Model;
using NakedObjects.Xat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {
  

    public class MenuTest : AcceptanceTestCase {
        private readonly Type[] FunctionTypes = {typeof(SimpleMenuFunctions)};

        private readonly Type[] RecordTypes = {typeof(SimpleRecord)};
        protected override Type[] Types { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override string[] Namespaces { get; } = {"NakedFunctions.Rest.Test.Data"};

        protected override EntityObjectStoreConfiguration Persistor {
            get {
                var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
                config.UsingContext(Activator.CreateInstance<MenuDbContext>);
                return config;
            }
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) {
            return new[] {
                factory.NewMenu(typeof(SimpleMenuFunctions), true, "Test menu")
            };
        }

        private IFunctionalReflectorConfiguration FunctionalReflectorConfiguration() => new FunctionalReflectorConfiguration(RecordTypes, FunctionTypes);

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddScoped<IOidStrategy, EntityOidStrategy>();
            services.AddScoped<IStringHasher, NullStringHasher>();
            services.AddScoped<IFrameworkFacade, FrameworkFacade>();
            services.AddScoped<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddSingleton(FunctionalReflectorConfiguration());
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }


        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ReflectorConfiguration.NoValidate = true;
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
            Assert.AreEqual(1, val.Count);

            var firstItem = val.First;

            firstItem.AssertMenuLink("Test menu", "GET", nameof(SimpleMenuFunctions));
        }

        [Test]
        public void TestGetMenu() {
            var api = Api();
            var result = api.GetMenu(nameof(SimpleMenuFunctions));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("Test menu", parsedResult["title"].ToString());
            Assert.AreEqual(nameof(SimpleMenuFunctions), parsedResult["menuId"].ToString());

            var members = parsedResult["members"] as JObject;
            Assert.AreEqual(11, members?.Count);

            var function = members[nameof(SimpleMenuFunctions.GetSimpleRecord)];

            function.AssertAction(nameof(SimpleMenuFunctions.GetSimpleRecord), "{}");
            function["extensions"].AssertExtensions(5); // todo add 

            var links = function["links"] as JArray;

            Assert.AreEqual(2, links.Count);

            var invokeLink = links.Last;

            invokeLink.AssertServiceInvokeLink("{}", "GET", nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecord));
        }


        [Test]
        public void TestInvokeMenuActionThatReturnsObject() {
            var api = Api();
            var result = api.GetInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecord), new ArgumentMap {Map = new Dictionary<string, IValue>()});
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsSingleItemList()
        {
            var api = Api();
            var result = api.GetInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordsSingle), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
           
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];
            var value = resultObj["value"] as JArray;

            Assert.AreEqual(1, value.Count);


            value[0].AssertObjectElementLink("Fred", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsMultiItemList()
        {
            var api = Api();
            var result = api.GetInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordsMultiple), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
           
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];
            var value = resultObj["value"] as JArray;

            Assert.AreEqual(2, value.Count);

            var firstItem = value[0];

            value[0].AssertObjectElementLink("Fred", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            value[1].AssertObjectElementLink("Bill", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "2");
        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesObject()
        {
            var api = Api().AsPost();
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetAndUpdateSimpleRecord), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Jack0", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "3");
            Assert.AreEqual("Jack0", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesList()
        {
            var api = Api().AsPost();
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordAndUpdateSimpleRecords), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred2", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            Assert.AreEqual("Fred2", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesListAndReturnsIt()
        {
            var api = Api().AsPost();
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetAndUpdateSimpleRecords), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var item1 = parsedResult["result"]["value"][0];
            var item2 = parsedResult["result"]["value"][1];
            var item3 = parsedResult["result"]["value"][2];

            item1.AssertObjectElementLink("Fred1", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            item2.AssertObjectElementLink("Bill1", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "2");
            item3.AssertObjectElementLink("Jack1", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "3");

        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesTuple()
        {
            var api = Api();
            api.HttpContext.Request.Method = "POST";
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordAndUpdateSimpleRecordsByTuple), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred3", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            Assert.AreEqual("Fred3", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesTupleAndReturnsIt()
        {
            var api = Api().AsPost();
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetAndUpdateSimpleRecordsByTuple), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var item1 = parsedResult["result"]["value"][0];
            var item2 = parsedResult["result"]["value"][1];
            var item3 = parsedResult["result"]["value"][2];

            item1.AssertObjectElementLink("Fred4", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            item2.AssertObjectElementLink("Bill4", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "2");
            item3.AssertObjectElementLink("Jack4", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "3");

        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesSubTuple()
        {
            var api = Api();
            api.HttpContext.Request.Method = "POST";
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordAndUpdateSimpleRecordsBySubTuple), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred5", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            Assert.AreEqual("Fred5", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatUpdatesSubTupleAndReturnsIt()
        {
            var api = Api().AsPost();
            var result = api.PostInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetAndUpdateSimpleRecordsBySubTuple), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);


            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var item1 = parsedResult["result"]["value"][0];
            var item2 = parsedResult["result"]["value"][1];
            var item3 = parsedResult["result"]["value"][2];

            item1.AssertObjectElementLink("Fred6", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            item2.AssertObjectElementLink("Bill6", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "2");
            item3.AssertObjectElementLink("Jack6", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "3");

        }

        [Test]
        public void TestInvokeMenuActionThatReturnsObjectAndAction()
        {
            var api = Api();
            var result = api.GetInvokeOnService(nameof(MenuFunctions), nameof(SimpleMenuFunctions.GetSimpleRecordWithWarning), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, headers) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            Assert.AreEqual("a warning", parsedResult["extensions"]["x-ro-nof-warnings"][0].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }
    }
}