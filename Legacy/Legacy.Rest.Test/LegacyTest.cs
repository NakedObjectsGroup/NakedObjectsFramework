// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using Legacy.Metamodel;
using Legacy.NakedObjects.Application;
using Legacy.NakedObjects.Application.Collection;
using Legacy.NakedObjects.Application.Control;
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
            typeof(ClassWithInternalCollection),
            typeof(ClassWithActionAbout),
            typeof(TextString),
            typeof(MultilineTextString),
            typeof(InternalCollection),
            typeof(BusinessValueHolder),
            typeof(TitledObject),
            typeof(ActionAbout),
            typeof(FieldAbout),
            typeof(User),
            typeof(Role),
            typeof(State),
            typeof(Title)
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
                    services.AddSingleton(typeof(IObjectFacetFactoryProcessor), typeof(InternalCollectionFacetFactory));
                    services.AddSingleton(typeof(IObjectFacetFactoryProcessor), typeof(AboutMethodsFacetFactory));
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
        public void TestGetObjectWithTextString() {
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
        public void TestGetTextStringProperty() {
            var api = Api();
            var result = api.GetProperty(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.Name));
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(nameof(ClassWithTextString.Name), parsedResult["id"].ToString());
            Assert.AreEqual("Fred", parsedResult["value"].ToString());
        }

        [Test]
        public void TestInvokeUpdateAndPersistObjectWithTextString() {
            var api = Api().AsPost();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newName", new ScalarValue("Ted") } } };

            var result = api.PostInvoke(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.ActionUpdateName), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("Ted", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestGetObjectWithInternalCollection() {
            var api = Api();
            var result = api.GetObject(FullName<ClassWithInternalCollection>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
            Assert.IsNotNull(parsedResult["members"]["Id"]);
            Assert.IsNotNull(parsedResult["members"]["TestCollection"]);
            Assert.IsNotNull(parsedResult["members"]["ActionUpdateTestCollection"]);

            Assert.AreEqual("1", parsedResult["members"]["TestCollection"]["size"].ToString());
            Assert.AreEqual("collection", parsedResult["members"]["TestCollection"]["memberType"].ToString());
        }

        [Test]
        public void TestInvokeUpdateAndPersistObjectWithInternalCollection() {
            var api = Api().AsPost();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newName", new ScalarValue("Bill") } } };

            var result = api.PostInvoke(FullName<ClassWithInternalCollection>(), "2", nameof(ClassWithInternalCollection.ActionUpdateTestCollection), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            Assert.AreEqual("1", resultObj["members"]["TestCollection"]["size"].ToString());
            Assert.AreEqual("collection", resultObj["members"]["TestCollection"]["memberType"].ToString());
        }

        [Test]
        public void TestGetObjectWithAction() {
            ClassWithActionAbout.TestInvisibleFlag = false;

            var api = Api();
            var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
            Assert.IsNotNull(parsedResult["members"]["Id"]);
            Assert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        }

        [Test]
        public void TestGetObjectWithInvisibleAction() {
            ClassWithActionAbout.TestInvisibleFlag = true;

            var api = Api();
            var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
            Assert.IsNotNull(parsedResult["members"]["Id"]);
        }
    }
}