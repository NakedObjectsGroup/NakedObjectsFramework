// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using Legacy.NakedObjects.Application;
using Legacy.NakedObjects.Application.ValueHolder;
using Legacy.Rest.Test.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Legacy.Rest.Test {
    public class LegacyTest : AcceptanceTestCase {
        protected override Type[] ObjectTypes { get; } = {
            typeof(ClassWithTextString),
            typeof(TextString),
            typeof(BusinessValueHolder),
            typeof(TitledObject)
        };

        protected override Type[] Services { get; } = { typeof(SimpleService)};

        protected override bool EnforceProxies => false;

        protected override Action<NakedFrameworkOptions> AddNakedFunctions => _ => { };

        protected override Action<NakedObjectsOptions> NakedObjectsOptions =>
            options => {
                options.DomainModelTypes = ObjectTypes;
                options.DomainModelServices = Services;
                options.NoValidate = true;
                options.RegisterCustomTypes = services => {
                    services.AddSingleton(typeof(IObjectFacetFactoryProcessor), typeof(TextStringValueTypeFacetFactory));
                };
            };


        protected new Func<IConfiguration, DbContext>[] ContextCreators => new Func<IConfiguration, DbContext>[] {
            config => {
                var context = new EFCoreObjectDbContext();
                context.Create();
                return context;
            }
        };

        protected virtual Action<EFCorePersistorOptions> EFCorePersistorOptions =>
            options => { options.ContextCreators = ContextCreators; };

        protected override Action<NakedFrameworkOptions> AddPersistor => builder => { builder.AddEFCorePersistor(EFCorePersistorOptions); };

        protected void CleanUpDatabase() {
            new EFCoreObjectDbContext().Delete();
        }

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
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }

        private static string FullName<T>() => typeof(T).FullName;

        [Test]
        public void TestGetObject() {
            var api = Api();
            var result = api.GetObject(FullName<ClassWithTextString>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
            Assert.IsNotNull(parsedResult["members"]["Id"]);
            Assert.IsNotNull(parsedResult["members"]["Name"]);
            Assert.IsNotNull(parsedResult["members"]["ActionUpdateName"]);
        }

        [Test]
        public void TestGetObjectProperty() {
            var api = Api();
            var result = api.GetProperty(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.Name));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ClassWithTextString.Name), parsedResult["id"].ToString());
            Assert.AreEqual("Fred", parsedResult["value"].ToString());
        }

        [Test]
        public void TestInvokeUpdateAndPersist() {
            var api = Api().AsPost();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newName", new ScalarValue("Ted") } } };

            var result = api.PostInvoke(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.ActionUpdateName), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("Ted", resultObj["members"]["Name"]["value"].ToString());
        }

    }
}