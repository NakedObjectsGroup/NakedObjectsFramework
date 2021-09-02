// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Facade.Utility;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Rest.API;
using NakedFramework.Test.TestCase;
using NakedFunctions.Reflector.Authorization;
using NakedFunctions.Rest.Test.Data;
using NakedFunctions.Rest.Test.Data.Sub;
using NakedFunctions.Security;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;


namespace NakedFunctions.Rest.Test {
    public class NullStringHasher : IStringHasher {
        public string GetHash(string toHash) => null;
    }

    public class TestDefaultAuthorizer : ITypeAuthorizer<object> {
        public static bool Allow = true;
        public static int EditCount = 0;
        public static int VisibleCount = 0;

        public bool IsEditable(object target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            EditCount++;
            return Allow;
        }

        public bool IsVisible(object target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            VisibleCount++;
            return Allow;
        }
    }

    public class TestNamespaceAuthorizer : INamespaceAuthorizer {
        public static bool Allow = true;
        public static int EditCount = 0;
        public static int VisibleCount = 0;

        public bool IsEditable(object target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            EditCount++;
            return Allow;
        }

        public bool IsVisible(object target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            VisibleCount++;
            return Allow;
        }
    }

    public class TestTypeAuthorizerFoo : ITypeAuthorizer<Foo> {
        public static bool Allow = true;
        public static int EditCount = 0;
        public static int VisibleCount = 0;

        public bool IsEditable(Foo target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            EditCount++;
            return Allow;
        }

        public bool IsVisible(Foo target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            VisibleCount++;
            return Allow;
        }
    }

    public class TestTypeAuthorizerFooSub : ITypeAuthorizer<FooSub> {
        public static bool Allow = true;
        public static int EditCount = 0;
        public static int VisibleCount = 0;

        public bool IsEditable(FooSub target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            EditCount++;
            return Allow;
        }

        public bool IsVisible(FooSub target, string memberName, IContext context) {
            Assert.IsNotNull(target);
            Assert.IsNotNull(memberName);
            Assert.IsNotNull(context);
            VisibleCount++;
            return Allow;
        }
    }


    public class AuthTestEF6 : AcceptanceTestCase {
        protected override Type[] Functions { get; } = {
            typeof(BarFunctions),
            typeof(QuxFunctions),
            typeof(FooFunctions),
            typeof(FooSubFunctions),
        };

        protected override Type[] Records { get; } = {
            typeof(Foo),
            typeof(Bar),
            typeof(Qux),
            typeof(FooSub),
        };

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextCreators =>
            new Func<IConfiguration, DbContext>[] {config => new AuthDbContext()};

        protected virtual void CleanUpDatabase()
        {
            ObjectDbContext.Delete();
        }

        protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        protected override IAuthorizationConfiguration AuthorizationConfiguration {
            get {
                var config = new AuthorizationConfiguration<TestDefaultAuthorizer>();

                config.AddNamespaceAuthorizer<TestNamespaceAuthorizer>("NakedFunctions.Rest.Test.Data.Sub");
                config.AddTypeAuthorizer<Foo, TestTypeAuthorizerFoo>();
                config.AddTypeAuthorizer<FooSub, TestTypeAuthorizerFooSub>();
                return config;
            }
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
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }

        private static string FullName<T>() => typeof(T).FullName;

        private static void ResetDefaultAuth(bool allow) {
            TestDefaultAuthorizer.Allow = allow;
            TestDefaultAuthorizer.EditCount = 0;
            TestDefaultAuthorizer.VisibleCount = 0;
        }

        private static void ResetNamespaceAuth(bool allow) {
            TestNamespaceAuthorizer.Allow = allow;
            TestNamespaceAuthorizer.EditCount = 0;
            TestNamespaceAuthorizer.VisibleCount = 0;
        }

        private static void ResetTypeFooAuth(bool allow) {
            TestTypeAuthorizerFoo.Allow = allow;
            TestTypeAuthorizerFoo.EditCount = 0;
            TestTypeAuthorizerFoo.VisibleCount = 0;
        }

        private static void ResetTypeFooSubAuth(bool allow) {
            TestTypeAuthorizerFooSub.Allow = allow;
            TestTypeAuthorizerFooSub.EditCount = 0;
            TestTypeAuthorizerFooSub.VisibleCount = 0;
        }

        private static void AssertDefaultAuth(int expectedVisible, int expectedEditable) {
            Assert.AreEqual(expectedVisible, TestDefaultAuthorizer.VisibleCount);
            Assert.AreEqual(expectedEditable, TestDefaultAuthorizer.EditCount);
        }

        private static void AssertNamespaceAuth(int expectedVisible, int expectedEditable) {
            Assert.AreEqual(expectedVisible, TestNamespaceAuthorizer.VisibleCount);
            Assert.AreEqual(expectedEditable, TestNamespaceAuthorizer.EditCount);
        }

        private static void AssertTypeFooAuth(int expectedVisible, int expectedEditable) {
            Assert.AreEqual(expectedVisible, TestTypeAuthorizerFoo.VisibleCount);
            Assert.AreEqual(expectedEditable, TestTypeAuthorizerFoo.EditCount);
        }

        private static void AssertTypeFooSubAuth(int expectedVisible, int expectedEditable) {
            Assert.AreEqual(expectedVisible, TestTypeAuthorizerFooSub.VisibleCount);
            Assert.AreEqual(expectedEditable, TestTypeAuthorizerFooSub.EditCount);
        }

        [Test]
        public void DefaultAuthorizerCalledForNonSpecificTypeAllowsProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Bar>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Prop1"]);
            Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

            AssertDefaultAuth(3,0);
            AssertNamespaceAuth(0,0);
            AssertTypeFooAuth(0,0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void DefaultAuthorizerCalledForNonSpecificTypeBlocksProp() {
            ResetDefaultAuth(false);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Bar>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Prop1"]);
            
            AssertDefaultAuth(3, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void DefaultAuthorizerCalledForNonSpecificTypeAllowsMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Bar>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(3, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void DefaultAuthorizerCalledForNonSpecificTypeBlocksMethod() {
            ResetDefaultAuth(false);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Bar>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(3, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void NamespaceAuthorizerCalledForNonSpecificTypeAllowsProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Qux>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Prop1"]);
            Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(3, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void NamespaceAuthorizerCalledForNonSpecificTypeBlocksProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(false);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Qux>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Prop1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(3, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void NamespaceAuthorizerCalledForNonSpecificTypeAllowsMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Qux>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(3, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void NamespaceAuthorizerCalledForNonSpecificTypeBlocksMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(false);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Qux>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(3, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificTypeAllowsProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Foo>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Prop1"]);
            Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(3, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificTypeBlocksProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(false);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Foo>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Prop1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(3, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificTypeAllowsMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Foo>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(3, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificTypeBlocksMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(false);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<Foo>(), "1");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Act1"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(3, 0);
            AssertTypeFooSubAuth(0, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificSubTypeAllowsProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<FooSub>(), "2");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Prop1"]);
            Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);
            Assert.IsNotNull(parsedResult["members"]["Prop2"]);
            Assert.IsNotNull(parsedResult["members"]["Prop2"]["disabledReason"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(5, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificSubTypeBlocksProp() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(false);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<FooSub>(), "2");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Prop1"]);
            Assert.IsNull(parsedResult["members"]["Prop2"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(5, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificSubTypeAllowsMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(true);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<FooSub>(), "2");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNotNull(parsedResult["members"]["Act2"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(5, 0);
        }

        [Test]
        public void TypeAuthorizerCalledForSpecificSubTypeBlocksMethod() {
            ResetDefaultAuth(true);
            ResetNamespaceAuth(true);
            ResetTypeFooAuth(true);
            ResetTypeFooSubAuth(false);

            var api = Api().AsGet();
            var result = api.GetObject(FullName<FooSub>(), "2");
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.IsNull(parsedResult["members"]["Act2"]);

            AssertDefaultAuth(0, 0);
            AssertNamespaceAuth(0, 0);
            AssertTypeFooAuth(0, 0);
            AssertTypeFooSubAuth(5, 0);
        }

    }
}