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

            firstItem.AssertMenuLink("Test menu", "GET", "SimpleMenuFunctions");
        }

        [Test]
        public void TestGetMenu() {
            var api = Api();
            var result = api.GetMenu("SimpleMenuFunctions");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("Test menu", parsedResult["title"].ToString());
            Assert.AreEqual("SimpleMenuFunctions", parsedResult["menuId"].ToString());

            var members = parsedResult["members"] as JObject;
            Assert.AreEqual(2, members?.Count);

            var function = members["GetSimpleRecord"];

            function.AssertAction("GetSimpleRecord", "{}");
            function["extensions"].AssertExtensions(5); // todo add 

            var links = function["links"] as JArray;

            Assert.AreEqual(2, links.Count);

            var invokeLink = links.Last;

            invokeLink.AssertServiceInvokeLink("{}", "GET", "MenuFunctions", "GetSimpleRecord");
        }


        [Test]
        public void TestInvokeMenuActionThatReturnsObject() {
            var api = Api();
            var result = api.GetInvokeOnService("MenuFunctions", "GetSimpleRecord", new ArgumentMap {Map = new Dictionary<string, IValue>()});
            var (json, sc, headers) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            var parsedResult = JObject.Parse(json);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            Assert.AreEqual("1", resultObj["instanceId"].ToString());
            Assert.AreEqual("NakedFunctions.Rest.Test.Data.SimpleRecord", resultObj["domainType"].ToString());
            Assert.AreEqual("Untitled Simple Record", resultObj["title"].ToString());
        }

        [Test]
        public void TestInvokeMenuActionThatReturnsList()
        {
            var api = Api();
            var result = api.GetInvokeOnService("MenuFunctions", "GetSimpleRecords", new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, headers) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            var parsedResult = JObject.Parse(json);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);

            Assert.AreEqual("list", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];
            var value = resultObj["value"] as JArray;

            Assert.AreEqual(1, value.Count);

            var firstItem = value[0];

            firstItem.AssertObjectElementLink("Untitled Simple Record", "GET", "NakedFunctions.Rest.Test.Data.SimpleRecord", "1");
        }
    }
}