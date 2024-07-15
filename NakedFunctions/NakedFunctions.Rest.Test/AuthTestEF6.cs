// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Facade.Utility;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test;
using NakedFramework.Test.TestCase;
using NakedFunctions.Reflector.Authorization;
using NakedFunctions.Rest.Test.Data;
using NakedFunctions.Security;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rest.Test.Data;
using Rest.Test.Data.Sub;
using static NakedFunctions.Rest.Test.AuthHelpers;

namespace NakedFunctions.Rest.Test;

public class NullStringHasher : IStringHasher {
    public string GetHash(string toHash) => null;
}

public static class AuthHelpers {
    public static string FullName<T>() => typeof(T).FullName;

    public static void ResetDefaultAuth(bool allow) {
        TestDefaultAuthorizer.Allow = allow;
        TestDefaultAuthorizer.VisibleCount = 0;
    }

    public static void ResetNamespaceAuth(bool allow) {
        TestNamespaceAuthorizer.Allow = allow;
        TestNamespaceAuthorizer.VisibleCount = 0;
    }

    public static void ResetTypeFooAuth(bool allow) {
        TestTypeAuthorizerFoo.Allow = allow;
        TestTypeAuthorizerFoo.VisibleCount = 0;
    }

    public static void ResetTypeFooSubAuth(bool allow) {
        TestTypeAuthorizerFooSub.Allow = allow;
        TestTypeAuthorizerFooSub.VisibleCount = 0;
    }

    public static void ResetMenuAuth(bool allow) {
        TestMenuAuthorizer.Allow = allow;
        TestMenuAuthorizer.VisibleCount = 0;
    }

    public static void ResetQueryableActionFooAuth(bool allow) {
        TestQueryableActionAuthorizerFoo.Allow = allow;
        TestQueryableActionAuthorizerFoo.VisibleCount = 0;
    }

    public static void AssertDefaultAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestDefaultAuthorizer.VisibleCount);
    }

    public static void AssertNamespaceAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestNamespaceAuthorizer.VisibleCount);
    }

    public static void AssertTypeFooAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestTypeAuthorizerFoo.VisibleCount);
    }

    public static void AssertTypeFooSubAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestTypeAuthorizerFooSub.VisibleCount);
    }

    public static void AssertMenuAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestMenuAuthorizer.VisibleCount);
    }

    public static void AssertQueryableActionFooAuth(int expectedVisible) {
        Assert.AreEqual(expectedVisible, TestQueryableActionAuthorizerFoo.VisibleCount);
    }
}

public class TestDefaultAuthorizer : ITypeAuthorizer<object> {
    public static bool Allow = true;
    public static int VisibleCount;

    public bool IsVisible(object target, string memberName, IContext context) {
        Assert.IsNotNull(target);
        Assert.IsNotNull(memberName);
        Assert.IsNotNull(context);
        VisibleCount++;
        return memberName is "Act1" or "Prop1" ? Allow : true;
    }
}

public class TestNamespaceAuthorizer : INamespaceAuthorizer {
    public static bool Allow = true;
    public static int VisibleCount;

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
    public static int VisibleCount;

    public bool IsVisible(Foo target, string memberName, IContext context) {
        Assert.IsNotNull(target);
        Assert.IsNotNull(memberName);
        Assert.IsNotNull(context);
        VisibleCount++;
        return Allow;
    }
}

public class TestQueryableActionAuthorizerFoo : IQueryableActionAuthorizer<Foo> {
    public static bool Allow = true;
    public static int VisibleCount;

    public bool IsVisible(string memberName, IContext context) {
        Assert.IsNotNull(memberName);
        Assert.IsNotNull(context);
        VisibleCount++;
        return Allow;
    }
}


public class TestTypeAuthorizerFooSub : ITypeAuthorizer<FooSub> {
    public static bool Allow = true;
    public static int VisibleCount;

    public bool IsVisible(FooSub target, string memberName, IContext context) {
        Assert.IsNotNull(target);
        Assert.IsNotNull(memberName);
        Assert.IsNotNull(context);
        VisibleCount++;
        return Allow;
    }
}

public class TestMenuAuthorizer : IMainMenuAuthorizer {
    public static bool Allow = true;
    public static int VisibleCount;

    public bool IsVisible(string target, string memberName, IContext context) {
        Assert.IsTrue(target is "Rest.Test.Data.Sub.QuxMenuFunctions" or "Rest.Test.Data.FooMenuFunctions");
        Assert.IsTrue(memberName is "Act1" or "Act2" or "Act3");
        Assert.IsNotNull(context);
        VisibleCount++;
        return memberName is "Act1" ? Allow : true;
    }
}

public class AuthTestEF6 : AcceptanceTestCase {
    protected override Type[] Functions { get; } = {
        typeof(BarFunctions),
        typeof(QuxFunctions),
        typeof(FooFunctions),
        typeof(FooSubFunctions),
        typeof(FooMenuFunctions),
        typeof(QuxMenuFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(Foo),
        typeof(Bar),
        typeof(Qux),
        typeof(FooSub)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Services { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AuthDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected override IAuthorizationConfiguration AuthorizationConfiguration {
        get {
            var config = new AuthorizationConfiguration<TestDefaultAuthorizer, TestMenuAuthorizer>();

            config.AddNamespaceAuthorizer<TestNamespaceAuthorizer>("Rest.Test.Data.Sub");
            config.AddTypeAuthorizer<Foo, TestTypeAuthorizerFoo>();
            config.AddTypeAuthorizer<FooSub, TestTypeAuthorizerFooSub>();
            config.AddQueryableActionAuthorizer<Foo, TestQueryableActionAuthorizerFoo>();
            return config;
        }
    }

    protected virtual void CleanUpDatabase() {
        ObjectDbContext.Delete();
    }

    protected override IMenu[] MainMenus(IMenuFactory factory) => new[] { typeof(FooMenuFunctions), typeof(QuxMenuFunctions) }.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();

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

    [Test]
    public void DefaultAuthorizerCalledForNonSpecificTypeAllowsProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Bar>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Prop1"]);
        Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

        AssertDefaultAuth(3);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void DefaultAuthorizerCalledForNonSpecificTypeBlocksProp() {
        ResetDefaultAuth(false);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Bar>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Prop1"]);

        AssertDefaultAuth(3);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void DefaultAuthorizerCalledForNonSpecificTypeAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Bar>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(3);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void DefaultAuthorizerCalledForNonSpecificTypeBlocksMethod() {
        ResetDefaultAuth(false);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Bar>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(3);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void NamespaceAuthorizerCalledForNonSpecificTypeAllowsProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Qux>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Prop1"]);
        Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(3);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void NamespaceAuthorizerCalledForNonSpecificTypeBlocksProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(false);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Qux>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Prop1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(3);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void NamespaceAuthorizerCalledForNonSpecificTypeAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Qux>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(3);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void NamespaceAuthorizerCalledForNonSpecificTypeBlocksMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(false);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Qux>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(3);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificTypeAllowsProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Foo>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Prop1"]);
        Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(3);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificTypeBlocksProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(false);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Foo>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Prop1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(3);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificTypeAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Foo>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(3);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificTypeBlocksMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(false);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<Foo>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(3);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificSubTypeAllowsProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<FooSub>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Prop1"]);
        Assert.IsNotNull(parsedResult["members"]["Prop1"]["disabledReason"]);
        Assert.IsNotNull(parsedResult["members"]["Prop2"]);
        Assert.IsNotNull(parsedResult["members"]["Prop2"]["disabledReason"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(5);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificSubTypeBlocksProp() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(false);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<FooSub>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Prop1"]);
        Assert.IsNull(parsedResult["members"]["Prop2"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(5);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificSubTypeAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<FooSub>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act2"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(5);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void TypeAuthorizerCalledForSpecificSubTypeBlocksMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(false);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetObject(FullName<FooSub>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Act2"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(5);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(0);
    }

    // menus

    [Test]
    public void DefaultAuthorizerNotCalledForMenuAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetMenu(nameof(FooMenuFunctions));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(4);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void NamespaceAuthorizerNotCalledForMenu() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetMenu(nameof(QuxMenuFunctions));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(5);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void QueryableActionAuthorizerCalledForAction() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>()};
        var result = api.GetInvokeOnMenu(nameof(QuxMenuFunctions), "Act3", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["result"]["members"]["QueryableAct"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(1);
    }

    [Test]
    public void QueryableActionAuthorizerHidesAction() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(false);

        var api = Api().AsGet();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>()};
        var result = api.GetInvokeOnMenu(nameof(QuxMenuFunctions), "Act3", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["result"]["members"]["QueryableAct"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(0);
        AssertQueryableActionFooAuth(1);
    }
}

public class MenuAuthTestEF6 : AcceptanceTestCase {
    protected override Type[] Functions { get; } = {
        typeof(BarFunctions),
        typeof(QuxFunctions),
        typeof(FooFunctions),
        typeof(FooSubFunctions),
        typeof(FooMenuFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(Foo),
        typeof(Bar),
        typeof(Qux),
        typeof(FooSub)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Services { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AuthDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected override IAuthorizationConfiguration AuthorizationConfiguration {
        get {
            var config = new AuthorizationConfiguration<TestDefaultAuthorizer, TestMenuAuthorizer>();

            config.AddNamespaceAuthorizer<TestNamespaceAuthorizer>("Rest.Test.Data.Sub");
            config.AddTypeAuthorizer<Foo, TestTypeAuthorizerFoo>();
            config.AddTypeAuthorizer<FooSub, TestTypeAuthorizerFooSub>();
            config.AddQueryableActionAuthorizer<Foo, TestQueryableActionAuthorizerFoo>();
            return config;
        }
    }

    protected virtual void CleanUpDatabase() {
        ObjectDbContext.Delete();
    }

    protected override IMenu[] MainMenus(IMenuFactory factory) => new[] { typeof(FooMenuFunctions) }.Select(t => factory.NewMenu(t, true, t.Name)).ToArray();

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

    // menus

    [Test]
    public void MenuAuthorizerCalledForNonSpecificMenuAllowsMethod() {
        ResetDefaultAuth(true);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(true);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetMenu(nameof(FooMenuFunctions));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNotNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(3);
        AssertQueryableActionFooAuth(0);
    }

    [Test]
    public void MenuAuthorizerCalledForNonSpecificMenuBlocksMethod() {
        ResetDefaultAuth(false);
        ResetNamespaceAuth(true);
        ResetTypeFooAuth(true);
        ResetTypeFooSubAuth(true);
        ResetMenuAuth(false);
        ResetQueryableActionFooAuth(true);

        var api = Api().AsGet();
        var result = api.GetMenu(nameof(FooMenuFunctions));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.IsNull(parsedResult["members"]["Act1"]);

        AssertDefaultAuth(0);
        AssertNamespaceAuth(0);
        AssertTypeFooAuth(0);
        AssertTypeFooSubAuth(0);
        AssertMenuAuth(4);
        AssertQueryableActionFooAuth(0);
    }
}