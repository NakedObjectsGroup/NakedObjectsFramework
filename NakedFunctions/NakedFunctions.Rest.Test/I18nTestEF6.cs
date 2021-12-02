// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.I18N;
using NakedFramework.Rest.API;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Rest.Test.Data;

namespace NakedFunctions.Rest.Test;

public class I18nTestEF6 : AcceptanceTestCase {
    protected override Type[] Functions { get; } = {
        typeof(FooFunctions),
        typeof(FooMenuFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(Foo)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Services { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AuthDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected override Action<NakedFrameworkOptions> AddCoreOptions => builder => {
        builder.UseI18N = true;
        base.AddCoreOptions(builder);
    };

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
        I18NManager.TestResourceManager = Model.ResourceManager;
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
        var result = api.GetObject(FullName<Foo>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("Changed Foo Name", parsedResult["extensions"]["friendlyName"].ToString());
        Assert.AreEqual("ChangedFooDescription", parsedResult["extensions"]["description"].ToString());
        Assert.AreEqual("Changed Prop 1 Name", parsedResult["members"]["Prop1"]["extensions"]["friendlyName"].ToString());
        Assert.AreEqual("ChangedProp1Description", parsedResult["members"]["Prop1"]["extensions"]["description"].ToString());
        Assert.AreEqual("Changed Act 1 Name", parsedResult["members"]["Act1"]["extensions"]["friendlyName"].ToString());
        Assert.AreEqual("ChangedAct1Description", parsedResult["members"]["Act1"]["extensions"]["description"].ToString());
    }

    [Test]
    public void TestGetMenu() {
        var api = Api();
        var result = api.GetMenu(nameof(FooMenuFunctions));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        //Assert.AreEqual("Changed Foo Menu Functions Name", parsedResult["extensions"]["friendlyName"].ToString());
        //Assert.AreEqual("ChangedFooMenuFunctionsDescription", parsedResult["extensions"]["description"].ToString());
        Assert.AreEqual("Changed Act 1 Name", parsedResult["members"]["Act1"]["extensions"]["friendlyName"].ToString());
        Assert.AreEqual("ChangedAct1Description", parsedResult["members"]["Act1"]["extensions"]["description"].ToString());
    }
}