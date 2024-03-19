// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Facade.Utility;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test.TestCase;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NOF2.Reflector.Extensions;
using NOF2.Rest.Test.Data;
using NOF2.Rest.Test.Data.AppLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NOF2.Rest.Test;

public class NullStringHasher : IStringHasher {
    public string GetHash(string toHash) => null;
}

public class NOF2Test : AcceptanceTestCase {
    protected Type[] NOF2Types { get; } = {
        typeof(ClassWithTextString),
        typeof(ClassWithInternalCollection),
        typeof(ClassWithActionAbout),
        typeof(ClassWithFieldAbout),
        typeof(INOF2RoleInterface),
        typeof(ClassWithMenu),
        typeof(ClassWithDate),
        typeof(ClassWithTimeStamp),
        typeof(ClassWithWholeNumber),
        typeof(ClassWithLogical),
        typeof(ClassWithMoney),
        typeof(ClassWithReferenceProperty),
        typeof(ClassWithOrderedProperties),
        typeof(ClassWithOrderedActions),
        typeof(ClassWithBounded),
        typeof(ClassToPersistWithAbout),
        typeof(ClassToPersist),
        typeof(ClassWithAnnotations),
        typeof(ClassWithInvalidNames)
    };

    protected Type[] NOF2Services { get; } = { typeof(SimpleService) };

    protected Type[] NOF2ValueHolders { get; } = {
        typeof(TextString),
        typeof(Money),
        typeof(Logical),
        typeof(MultiLineTextString),
        typeof(WholeNumber),
        typeof(NODate),
        typeof(NODateNullable),
        typeof(TimeStamp)
    };

    protected override bool EnforceProxies => false;

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => _ => { };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected Action<NOF2Options> NOF2Options =>
        options => {
            options.DomainModelTypes = NOF2Types;
            options.DomainModelServices = NOF2Services;
            options.ValueHolderTypes = NOF2ValueHolders;
        };

    protected virtual Action<NakedFrameworkOptions> AddNOF2 => builder => builder.AddNOF2(NOF2Options);

    protected override Action<NakedFrameworkOptions> NakedFrameworkOptions =>
        builder => {
            AddCoreOptions(builder);
            AddPersistor(builder);
            AddRestfulObjects(builder);
            AddNOF2(builder);
        };

    protected new Func<IConfiguration, DbContext>[] ContextCreators => new Func<IConfiguration, DbContext>[] {
        config => {
            var context = new EFCoreObjectDbContext();
            context.Create();
            return context;
        }
    };

#pragma warning disable CS0618 // Type or member is obsolete
    protected virtual Action<EFCorePersistorOptions> EFCorePersistorOptions =>
        options => { options.ContextCreators = ContextCreators; };
#pragma warning restore CS0618 // Type or member is obsolete

    protected override Action<NakedFrameworkOptions> AddPersistor => builder => { builder.AddEFCorePersistor(EFCorePersistorOptions); };

    protected void CleanUpDatabase() {
        new EFCoreObjectDbContext().Delete();
    }

    protected override void RegisterTypes(IServiceCollection services) {
        base.RegisterTypes(services);
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddTransient<IStringHasher, NullStringHasher>();
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }

    [SetUp]
    public void SetUp() {
        StartTest();
        ThreadLocals.Initialize(GetConfiguredContainer(), sp => new Reflector.Component.Container(sp.GetService<INakedFramework>()));
    }

    [TearDown]
    public void TearDown() {
        EndTest();
        ThreadLocals.Reset();
    }

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
    public void TestGetObjectWithTextString() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithTextString>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionUpdateName"]);

        ClassicAssert.AreEqual("Fred", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetObjectWithRestExtension() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithTextString>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionUpdateName"]);

        ClassicAssert.AreEqual("class-value", parsedResult["extensions"]["x-ro-class-ext"].ToString());
        ClassicAssert.AreEqual("prop-value", parsedResult["members"]["Name"]["extensions"]["x-ro-prop-ext"].ToString());
        ClassicAssert.AreEqual("act-value", parsedResult["members"]["ActionUpdateName"]["extensions"]["x-ro-act-ext"].ToString());
        ClassicAssert.AreEqual("parm-value", parsedResult["members"]["ActionUpdateName"]["parameters"]["newName"]["extensions"]["x-ro-parm-ext"].ToString());
    }

    [Test]
    public void TestGetBounded() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithBounded>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ChoicesProperty"]);
        ClassicAssert.AreEqual("True", parsedResult["members"]["ChoicesProperty"]["hasChoices"].ToString());
        ClassicAssert.AreEqual("data1", parsedResult["members"]["ChoicesProperty"]["choices"][0]["title"].ToString());
        ClassicAssert.AreEqual("data2", parsedResult["members"]["ChoicesProperty"]["choices"][1]["title"].ToString());

        ClassicAssert.AreEqual("data1", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetImmutable() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithBounded>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Field disabled as object cannot be changed", parsedResult["members"]["Name"]["disabledReason"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["ChoicesProperty"]);
        ClassicAssert.AreEqual("Field disabled as object cannot be changed", parsedResult["members"]["ChoicesProperty"]["disabledReason"].ToString());
    }

    [Test]
    public void TestGetTextStringProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.Name));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithTextString.Name), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("Fred", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("string", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithTextString() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newName", new ScalarValue("Ted") } } };

        var result = api.PostInvoke(FullName<ClassWithTextString>(), "1", nameof(ClassWithTextString.ActionUpdateName), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Ted", resultObj["members"]["Name"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithInternalCollection() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithInternalCollection>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["TestCollection"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionUpdateTestCollection"]);

        ClassicAssert.AreEqual("1", parsedResult["members"]["TestCollection"]["size"].ToString());
        ClassicAssert.AreEqual("collection", parsedResult["members"]["TestCollection"]["memberType"].ToString());
    }

    [Test]
    public void TestGetObjectWithAction() {
        ClassWithActionAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Test Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
    }

    [Test]
    public void TestGetObjectWithInvisibleAction() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestInvisibleFlag = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        //ClassicAssert.IsNotNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetObjectWithVisibleField() {
        ClassWithFieldAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
    }

    [Test]
    public void TestGetObjectWithNameAction() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestName = "Renamed Action";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Renamed Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
    }

    [Test]
    public void TestGetObjectWithNameActionParameter() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestName = "Renamed Action";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Renamed Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.AreEqual("renamed param1", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("renamed param2", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestGetObjectWithInferredNameActionParameter() {
        ClassWithActionAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Test Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.AreEqual("Ts", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("Wn", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestGetObjectWithDefaultedActionParameter() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestName = "Renamed Action";
        ClassWithActionAbout.TestDefaults = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.AreEqual("def", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["default"].ToString());
        ClassicAssert.AreEqual("66", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["default"].ToString());
    }

    [Test]
    public void TestGetObjectWithNonDefaultedActionParameter() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestName = "Renamed Action";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.IsNull(parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["default"]);
        ClassicAssert.IsNull(parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["default"]);
    }

    [Test]
    public void TestGetObjectWithChoicesActionParameter() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestOptions = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.AreEqual("opt1", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["choices"][0].ToString());
        ClassicAssert.AreEqual("opt2", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["choices"][1].ToString());
        ClassicAssert.AreEqual("1", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["choices"][0].ToString());
        ClassicAssert.AreEqual("5", parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["choices"][4].ToString());
    }

    [Test]
    public void TestGetObjectWithNonChoicesActionParameter() {
        ClassWithActionAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
        ClassicAssert.IsNull(parsedResult["members"]["actionTestActionWithParms"]["parameters"]["ts"]["choices"]);
        ClassicAssert.IsNull(parsedResult["members"]["actionTestActionWithParms"]["parameters"]["wn"]["choices"]);
    }

    [Test]
    public void TestGetObjectWithDescribedAction() {
        ClassWithActionAbout.ResetTest();
        ClassWithActionAbout.TestDescription = "Action With Description";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Test Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("Action With Description", parsedResult["members"]["actionTestAction"]["extensions"]["description"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
    }

    [Test]
    public void TestGetObjectWithDisabledAction() {
        ClassWithActionAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestAction"]);
        ClassicAssert.AreEqual("Test Action", parsedResult["members"]["actionTestAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("Unusable by about", parsedResult["members"]["actionTestAction"]["disabledReason"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["actionTestActionWithParms"]);
    }

    [Test]
    public void TestInvokeActionWithInvalidParameter() {
        ClassWithActionAbout.ResetTest();

        ClassWithActionAbout.TestUsableFlag = true;
        ClassWithActionAbout.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("invalid") }, { "wn", new ScalarValue(0) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("invalid", parsedResult["ts"]["value"].ToString());
        ClassicAssert.AreEqual("0", parsedResult["wn"]["value"].ToString());
        ClassicAssert.AreEqual("ts is invalid", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeActionWithInvalidParameter1() {
        ClassWithActionAbout.ResetTest();

        ClassWithActionAbout.TestUsableFlag = true;
        ClassWithActionAbout.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("valid") }, { "wn", new ScalarValue(101) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("valid", parsedResult["ts"]["value"].ToString());
        ClassicAssert.AreEqual("101", parsedResult["wn"]["value"].ToString());
        ClassicAssert.AreEqual("wn is invalid", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeActionWithValidParameter() {
        ClassWithActionAbout.ResetTest();

        ClassWithActionAbout.TestUsableFlag = true;
        ClassWithActionAbout.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("valid") }, { "wn", new ScalarValue(0) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("void", parsedResult["resultType"].ToString());
        ClassicAssert.IsNull(parsedResult["x-ro-invalidReason"]);
    }

    [Test]
    public void TestInvokeAboutActionWithEmptyParameter() {
        ClassWithActionAbout.ResetTest();

        ClassWithActionAbout.TestUsableFlag = true;
        ClassWithActionAbout.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("") }, { "wn", new ScalarValue(0) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
    }

    [Test]
    public void TestInvokeActionWithEmptyParameter() {
        ClassWithActionAbout.ResetTest();

        ClassWithActionAbout.TestUsableFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("") }, { "wn", new ScalarValue(0) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
    }

    [Test]
    public void TestInvokeActionWithEmptyRefParameter() {
        ClassWithActionAbout.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue(null) } } };

        var result = api.PostInvoke(FullName<ClassWithActionAbout>(), "1", nameof(ClassWithActionAbout.actionTestActionWithRefParms), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);
        ClassicAssert.AreEqual("", parsedResult["result"], ToString());
    }

    [Test]
    public void TestGetObjectWithInvisibleField() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestInvisibleFlag = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(0, ((JContainer)parsedResult["members"]).Count);
        //ClassicAssert.IsNotNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetObjectWithUnNamedField() {
        ClassWithFieldAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestGetObjectWithNamedField() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestName = "Name from About";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name from About", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestGetObjectWithUsableField() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestUsableFlag = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("", parsedResult["members"]["Name"]["extensions"]["description"].ToString());
        ClassicAssert.IsNull(parsedResult["members"]["Name"]["disabledReason"]);
    }

    [Test]
    public void TestGetObjectWithUnUsableField() {
        ClassWithFieldAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("", parsedResult["members"]["Name"]["extensions"]["description"].ToString());
        ClassicAssert.AreEqual("Unusable by about", parsedResult["members"]["Name"]["disabledReason"].ToString());
    }

    [Test]
    public void TestGetObjectWithDescribedField() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestDescription = "Description from About";

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("Description from About", parsedResult["members"]["Name"]["extensions"]["description"].ToString());
    }

    [Test]
    public void TestGetObjectWithUnDescribedField() {
        ClassWithFieldAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("Name", parsedResult["members"]["Name"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("", parsedResult["members"]["Name"]["extensions"]["description"].ToString());
    }

    [Test]
    public void TestGetObjectWithOptionsField() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestChoices = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("True", parsedResult["members"]["Name"]["hasChoices"].ToString());
        ClassicAssert.AreEqual("fieldopt1", parsedResult["members"]["Name"]["choices"][0].ToString());
        ClassicAssert.AreEqual("fieldopt2", parsedResult["members"]["Name"]["choices"][1].ToString());
    }

    [Test]
    public void TestGetObjectWithNoOptionsField() {
        ClassWithFieldAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["Name"]);
        ClassicAssert.AreEqual("False", parsedResult["members"]["Name"]["hasChoices"].ToString());
        ClassicAssert.IsNull(parsedResult["members"]["Name"]["choices"]);
    }

    [Test]
    public void TestPutInvalidProperty() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestValidFlag = true;
        ClassWithFieldAbout.TestUsableFlag = true;

        var api = Api().AsPut();
        var sva = new SingleValueArgument { Value = new ScalarValue("invalid") };
        var result = api.PutProperty(FullName<ClassWithFieldAbout>(), "1", nameof(ClassWithFieldAbout.Name), sva);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("invalid", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("invalid by about", parsedResult["invalidReason"].ToString());
    }

    [Test]
    public void TestPutUnParseableProperty() {
        var api = Api().AsPut();
        var sva = new SingleValueArgument { Value = new ScalarValue("unparseable value") };
        var result = api.PutProperty(FullName<ClassWithWholeNumber>(), "1", nameof(ClassWithWholeNumber.WholeNumber), sva);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("unparseable value", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("unparseable value", parsedResult["invalidReason"].ToString());
    }

    [Test]
    public void TestPutValidProperty() {
        ClassWithFieldAbout.ResetTest();
        ClassWithFieldAbout.TestValidFlag = true;
        ClassWithFieldAbout.TestUsableFlag = true;

        var api = Api().AsPut();
        var sva = new SingleValueArgument { Value = new ScalarValue("valid") };
        var result = api.PutProperty(FullName<ClassWithFieldAbout>(), "1", nameof(ClassWithFieldAbout.Name), sva);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("valid", parsedResult["value"].ToString());
    }

    [Test]
    public void TestPutReferenceProperty() {
        var api = Api().AsPut();
        var sva = new SingleValueArgument { Value = new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "referenceProperty") };
        var result = api.PutProperty(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.ReferenceProperty), sva);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Bill", parsedResult["value"]["title"].ToString());
    }

    [Test]
    public void TestPutEmptyProperty() {
        ClassWithFieldAbout.ResetTest();

        ClassWithFieldAbout.TestUsableFlag = true;

        var api = Api().AsPut();
        var sva = new SingleValueArgument { Value = new ScalarValue("") };
        var result = api.PutProperty(FullName<ClassWithFieldAbout>(), "1", nameof(ClassWithFieldAbout.Name), sva);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("", parsedResult["value"].ToString());
    }

    [Test]
    public void TestGetNOF2ObjectWithMenu() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithMenu>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMethod1"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionMethod2"]);
        ClassicAssert.AreEqual("Submenu1", parsedResult["members"]["actionMethod2"]["extensions"]["x-ro-nof-menuPath"].ToString());
    }

    [Test]
    public void TestGetNOF2MainMenu() {
        var api = Api();
        var result = api.GetMenu("ClassWithMenu");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(8, ((JContainer)parsedResult["members"]).Count);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction1"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction2"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuActionWithParm"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMethodInjected"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMethodInjectedWithParm"]);
    }

    [Test]
    public void TestGetObjectWithDate() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithDate>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Date"]);
        ClassicAssert.AreEqual("2021-11-01", parsedResult["members"]["Date"]["value"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionUpdateDate"]);

        ClassicAssert.AreEqual("01/11/2021", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetDateProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.Date));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithDate.Date), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("2021-11-01", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("date", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestGetNullableDateProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.DateNullable));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithDate.DateNullable), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("2021-11-01", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("date", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithDate() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newDate", new ScalarValue(new DateTime(1998, 7, 6)) } } };

        var result = api.PostInvoke(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.ActionUpdateDate), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("1998-07-06", resultObj["members"]["Date"]["value"].ToString());
        ClassicAssert.AreEqual("string", resultObj["members"]["Date"]["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("date", resultObj["members"]["Date"]["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeActionThatUsesContainer() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Bill") } } };

        var result = api.PostInvoke(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.actionGetObject), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual(2, ((JContainer)resultObj["members"]).Count);
        ClassicAssert.IsNull(resultObj["members"]["Id"]);
        ClassicAssert.IsNotNull(resultObj["members"]["Name"]);
        ClassicAssert.IsNotNull(resultObj["members"]["ActionUpdateName"]);

        ClassicAssert.AreEqual("Bill", resultObj["title"].ToString());
    }

    [Test]
    public void TestInvokeActionThatUsesRepository() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Bill") } } };

        var result = api.PostInvoke(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.actionGetObject1), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual(2, ((JContainer)resultObj["members"]).Count);
        ClassicAssert.IsNull(resultObj["members"]["Id"]);
        ClassicAssert.IsNotNull(resultObj["members"]["Name"]);
        ClassicAssert.IsNotNull(resultObj["members"]["ActionUpdateName"]);

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
    }

    [Test]
    public void TestGetObjectWithTimeStamp() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithTimeStamp>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["TimeStamp"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionUpdateTimeStamp"]);

        ClassicAssert.AreEqual("01/11/2021 12:00:00", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetTimeStampProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithTimeStamp>(), "1", nameof(ClassWithTimeStamp.TimeStamp));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithTimeStamp.TimeStamp), parsedResult["id"].ToString());
        ClassicAssert.AreEqual(DateTime.Parse("11/01/2021 00:00:00", CultureInfo.InvariantCulture), parsedResult["value"].Value<DateTime>());
        ClassicAssert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("date-time", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithTimestamp() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newTimeStamp", new ScalarValue(new DateTime(1998, 7, 6)) } } };

        var result = api.PostInvoke(FullName<ClassWithTimeStamp>(), "1", nameof(ClassWithTimeStamp.ActionUpdateTimeStamp), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual(DateTime.Parse("1998-07-06 00:00:00", CultureInfo.InvariantCulture), resultObj["members"]["TimeStamp"]["value"].Value<DateTime>());

        ClassicAssert.AreEqual("string", resultObj["members"]["TimeStamp"]["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("date-time", resultObj["members"]["TimeStamp"]["extensions"]["format"].ToString());
    }

    [Test]
    public void TestGetObjectWithWholeNumber() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithWholeNumber>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["WholeNumber"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionUpdateWholeNumber"]);

        ClassicAssert.AreEqual("10", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetWholeNumberProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithWholeNumber>(), "1", nameof(ClassWithWholeNumber.WholeNumber));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithWholeNumber.WholeNumber), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("10", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("number", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("int", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithWholeNumber() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newWholeNumber", new ScalarValue(66) } } };

        var result = api.PostInvoke(FullName<ClassWithWholeNumber>(), "1", nameof(ClassWithWholeNumber.actionUpdateWholeNumber), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("66", resultObj["members"]["WholeNumber"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithLogical() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithLogical>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Logical"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionUpdateLogical"]);

        ClassicAssert.AreEqual("True", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetLogicalProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithLogical>(), "1", nameof(ClassWithLogical.Logical));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithLogical.Logical), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("True", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("boolean", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.IsNull(parsedResult["extensions"]["format"]);
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithLogical() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newLogical", new ScalarValue(false) } } };

        var result = api.PostInvoke(FullName<ClassWithLogical>(), "1", nameof(ClassWithLogical.actionUpdateLogical), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("False", resultObj["members"]["Logical"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithMoney() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithMoney>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["Money"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionUpdateMoney"]);

        ClassicAssert.AreEqual("€ 10.00", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetMoneyProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithMoney>(), "1", nameof(ClassWithMoney.Money));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithMoney.Money), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("10", parsedResult["value"].ToString());
        ClassicAssert.AreEqual("number", parsedResult["extensions"]["returnType"].ToString());
        ClassicAssert.AreEqual("decimal", parsedResult["extensions"]["format"].ToString());
        ClassicAssert.AreEqual("C", parsedResult["extensions"]["x-ro-nof-mask"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithMoney() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newMoney", new ScalarValue(66) } } };

        var result = api.PostInvoke(FullName<ClassWithMoney>(), "1", nameof(ClassWithMoney.actionUpdateMoney), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("66", resultObj["members"]["Money"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithReferenceProperty() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithReferenceProperty>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ReferenceProperty"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionUpdateReferenceProperty"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionGetObject"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionGetObject1"]);

        ClassicAssert.AreEqual("Untitled Class With Reference Property", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetReferencePropertyProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.ReferenceProperty));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(nameof(ClassWithReferenceProperty.ReferenceProperty), parsedResult["id"].ToString());
        ClassicAssert.AreEqual("Fred", parsedResult["value"]["title"].ToString());
        ClassicAssert.AreEqual(@"http://localhost/objects/NOF2.Rest.Test.Data.ClassWithTextString/1", parsedResult["value"]["href"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", parsedResult["extensions"]["returnType"].ToString());
    }

    [Test]
    public void TestMainMenus() {
        var api = Api();

        var result = api.GetMenus();
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("ClassWithMenu Main Menu", parsedResult["value"][0]["title"].ToString());
    }

    [Test]
    public void TestMainMenu() {
        ClassWithMenu.ResetTest();
        var api = Api();

        var result = api.GetMenu("ClassWithMenu");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction"]);
        ClassicAssert.AreEqual("Renamed menu Action", parsedResult["members"]["ActionMenuAction"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction1"]);
        ClassicAssert.AreEqual("Menu Action1", parsedResult["members"]["ActionMenuAction1"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuAction2"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuActionWithParm"]);
    }

    [Test]
    public void TestMainMenuWithAboutName() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestNameFlag = true;
        ClassWithMenu.TestDescriptionFlag = true;
        var api = Api();

        var result = api.GetMenu(nameof(ClassWithMenu));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuActionWithParm"]);
        ClassicAssert.AreEqual("Renamed Name", parsedResult["members"]["ActionMenuActionWithParm"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("A Description", parsedResult["members"]["ActionMenuActionWithParm"]["extensions"]["description"].ToString());
    }

    [Test]
    public void TestMainMenuWithAboutUnusable() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestUsableFlag = true;
        var api = Api();

        var result = api.GetMenu(nameof(ClassWithMenu));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["ActionMenuActionWithParm"]);
    }

    [Test]
    public void TestMainMenuWithAboutHidden() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestInvisibleFlag = true;
        var api = Api();

        var result = api.GetMenu(nameof(ClassWithMenu));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNull(parsedResult["members"]["ActionMenuActionWithParm"]);
    }

    [Test]
    public void TestMainMenuWithAboutParameters() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestParametersFlag = true;
        var api = Api();

        var result = api.GetMenu(nameof(ClassWithMenu));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuActionWithParm"]);
        ClassicAssert.AreEqual("def", parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["default"].ToString());
        ClassicAssert.AreEqual("opt1", parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["choices"][0].ToString());
        ClassicAssert.AreEqual("opt2", parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["choices"][1].ToString());
        ClassicAssert.AreEqual("renamed ts", parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestMainMenuWithoutAbout() {
        ClassWithMenu.ResetTest();
        var api = Api();

        var result = api.GetMenu(nameof(ClassWithMenu));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMenuActionWithParm"]);
        ClassicAssert.AreEqual("Menu Action With Parm", parsedResult["members"]["ActionMenuActionWithParm"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.AreEqual("", parsedResult["members"]["ActionMenuActionWithParm"]["extensions"]["description"].ToString());
        ClassicAssert.IsNull(parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["default"]);
        ClassicAssert.IsNull(parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["choices"]);
        ClassicAssert.AreEqual("Ts", parsedResult["members"]["ActionMenuActionWithParm"]["parameters"]["ts"]["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public void TestObjectMenu() {
        ClassWithMenu.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithMenu>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);

        ClassicAssert.IsNotNull(parsedResult["members"]["ActionMethod1"]);
        ClassicAssert.IsNotNull(parsedResult["members"]["actionMethod2"]);
        ClassicAssert.AreEqual("Submenu1", parsedResult["members"]["actionMethod2"]["extensions"]["x-ro-nof-menuPath"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuAction), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithInjectedContainerReturnObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMethodInjected), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithParmInjectedContainerReturnObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("Fred") } } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMethodInjectedWithParm), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnTransientObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionCreateTransient), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Untitled Class With Text String", resultObj["title"].ToString());
        ClassicAssert.AreEqual("transient", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnPersistedObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionPersistTransient), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Jenny", resultObj["title"].ToString());
        ClassicAssert.AreEqual("persistent", resultObj["extensions"]["x-ro-nof-interactionMode"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithParmReturnObject() {
        ClassWithMenu.ResetTest();

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("Fred") } } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuActionWithParm), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithAboutValid() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("Fred") } } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuActionWithParm), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("Fred", resultObj["title"].ToString());
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithAboutInvalid() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("invalid") } } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuActionWithParm), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual("invalid", parsedResult["ts"]["value"].ToString());
        ClassicAssert.AreEqual("ts invalid", parsedResult["x-ro-invalidReason"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithAboutEmpty() {
        ClassWithMenu.ResetTest();
        ClassWithMenu.TestValidFlag = true;

        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "ts", new ScalarValue("") } } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuActionWithParm), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("", parsedResult["result"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnArrayList() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuAction1), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("list", parsedResult["resultType"].ToString());

        var resultObj = parsedResult["result"];

        ClassicAssert.AreEqual(3, ((JContainer)resultObj["value"]).Count);
        ClassicAssert.AreEqual("Fred", resultObj["value"][0]["title"].ToString());
        ClassicAssert.AreEqual("Bill", resultObj["value"][1]["title"].ToString());
        ClassicAssert.AreEqual("Tom", resultObj["value"][2]["title"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnQueryable() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuAction2), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("list", parsedResult["resultType"].ToString());

        var resultObj = parsedResult["result"];

        //ClassicAssert.AreEqual(2, ((JContainer)resultObj["value"]).Count);
        ClassicAssert.AreEqual("NOF2.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["elementType"].ToString());
        ClassicAssert.AreEqual("Fred", resultObj["value"][0]["title"].ToString());
        ClassicAssert.AreEqual("Bill", resultObj["value"][1]["title"].ToString());
    }

    [Test]
    public void TestGetObjectWithOrderedProperties() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithOrderedProperties>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.AreEqual("Name2", ((JProperty)parsedResult["members"].First).Name);
        ClassicAssert.AreEqual("0", parsedResult["members"]["Name2"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("Name3", ((JProperty)parsedResult["members"].First.Next).Name);
        ClassicAssert.AreEqual("1", parsedResult["members"]["Name3"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("Name1", ((JProperty)parsedResult["members"].First.Next.Next).Name);
        ClassicAssert.AreEqual("2", parsedResult["members"]["Name1"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("Name4", ((JProperty)parsedResult["members"].First.Next.Next.Next).Name);
        ClassicAssert.AreEqual("4", parsedResult["members"]["Name4"]["extensions"]["memberOrder"].ToString());
        ClassicAssert.AreEqual("10", parsedResult["members"]["Name4"]["extensions"]["maxLength"].ToString());
    }

    [Test]
    public void TestGetObjectWithOrderedActions() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithOrderedActions>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        ClassicAssert.IsNull(parsedResult["members"]["Id"]);
        ClassicAssert.AreEqual("actionAction2", ((JProperty)parsedResult["members"].First).Name);
        //ClassicAssert.AreEqual("0", parsedResult["members"]["actionAction2"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("actionAction3", ((JProperty)parsedResult["members"].First.Next).Name);
        //ClassicAssert.AreEqual("1", parsedResult["members"]["actionAction3"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("actionAction1", ((JProperty)parsedResult["members"].First.Next.Next).Name);
        //ClassicAssert.AreEqual("2", parsedResult["members"]["actionAction1"]["extensions"]["memberOrder"].ToString());

        ClassicAssert.AreEqual("actionAction4", ((JProperty)parsedResult["members"].First.Next.Next.Next).Name);
        //ClassicAssert.AreEqual("4", parsedResult["members"]["actionAction4"]["extensions"]["memberOrder"].ToString());
    }

    [Test]
    public void TestPersistTransient() {
        ClassToPersistWithAbout.ResetTest();

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassToPersistWithAbout>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual("Jean", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("persistent", parsedResult["extensions"]["x-ro-nof-interactionMode"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientPassSave() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassToPersistWithAbout>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual("Jean", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("persistent", parsedResult["extensions"]["x-ro-nof-interactionMode"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientNoAboutPassSave() {
        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> { { "Name", new ScalarValue("Jan") } };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersist>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassToPersist>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual("Jan", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("persistent", parsedResult["extensions"]["x-ro-nof-interactionMode"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientNoSaveFail() {
        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> { { "Name", new ScalarValue("Jim") } };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassWithTextString>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassWithTextString>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual($"Attempt to save an object without an ActionSave: {FullName<ClassWithTextString>()}", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientPassProperty() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestValueProperty = true;
        ClassToPersistWithAbout.TestRefProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassToPersistWithAbout>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual("Jean", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("persistent", parsedResult["extensions"]["x-ro-nof-interactionMode"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientPassBoth() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;
        ClassToPersistWithAbout.TestValueProperty = true;
        ClassToPersistWithAbout.TestRefProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(FullName<ClassToPersistWithAbout>(), parsedResult["domainType"].ToString());
        ClassicAssert.AreEqual("Jean", parsedResult["title"].ToString());
        ClassicAssert.AreEqual("persistent", parsedResult["extensions"]["x-ro-nof-interactionMode"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientFailSaveValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("invalid") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Object Name is invalid", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientFailSaveRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Ref Object Name is invalid", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptySaveValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Object Name is null", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptySaveRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ScalarValue(null) }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Ref Object is null", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientFailPropertyValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestValueProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("invalid") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/3", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Property Name is invalid", parsedResult["members"]["Name"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientFailPropertyRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestRefProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Ref Property Name is invalid", parsedResult["members"]["ReferenceProperty"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptyPropertyValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestValueProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Property Name is null", parsedResult["members"]["Name"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptyPropertyRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestRefProperty = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ScalarValue(null) }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.Created, sc);
    }

    [Test]
    public void TestPersistTransientFailBothValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestValueProperty = true;
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("invalid") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Property Name is invalid", parsedResult["members"]["Name"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientFailBothRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestRefProperty = true;
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Ref Property Name is invalid", parsedResult["members"]["ReferenceProperty"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptyBothValue() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestValueProperty = true;
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("") },
            { "ReferenceProperty", new ReferenceValue($"http://localhost/objects/{FullName<ClassWithTextString>()}/2", "ReferenceProperty") }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Property Name is null", parsedResult["members"]["Name"]["invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestPersistTransientEmptyBothRef() {
        ClassToPersistWithAbout.ResetTest();
        ClassToPersistWithAbout.TestRefProperty = true;
        ClassToPersistWithAbout.TestSave = true;

        var api = Api().AsPost();

        var dict = new Dictionary<string, IValue> {
            { "Name", new ScalarValue("Jean") },
            { "ReferenceProperty", new ScalarValue(null) }
        };

        var map = new PersistArgumentMap { Map = dict, ReservedArguments = new ReservedArguments() };

        var result = api.PostPersist(FullName<ClassToPersistWithAbout>(), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.UnprocessableEntity, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("Ref Object is null", parsedResult["x-ro-invalidReason"].ToString());

        ClassicAssert.IsNull(parsedResult["members"]["ActionSave"]);
    }

    [Test]
    public void TestGetClassWithAnnotations() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithAnnotations>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("True", parsedResult["members"]["Name"]["extensions"]["optional"].ToString());
        ClassicAssert.AreEqual("False", parsedResult["members"]["RequiredName"]["extensions"]["optional"].ToString());
        ClassicAssert.AreEqual("renamed", parsedResult["members"]["NamedName"]["extensions"]["friendlyName"].ToString());
        ClassicAssert.IsNull(parsedResult["members"]["HiddenName"]);

        ClassicAssert.AreEqual("True", parsedResult["members"]["TestTableView"]["extensions"]["x-ro-nof-tableViewTitle"].ToString());
        ClassicAssert.AreEqual("one", parsedResult["members"]["TestTableView"]["extensions"]["x-ro-nof-tableViewColumns"][0].ToString());
        ClassicAssert.AreEqual("two", parsedResult["members"]["TestTableView"]["extensions"]["x-ro-nof-tableViewColumns"][1].ToString());

        ClassicAssert.AreEqual("False", parsedResult["members"]["ActionTestTableView"]["extensions"]["x-ro-nof-tableViewTitle"].ToString());
        ClassicAssert.AreEqual("three", parsedResult["members"]["ActionTestTableView"]["extensions"]["x-ro-nof-tableViewColumns"][0].ToString());
        ClassicAssert.AreEqual("four", parsedResult["members"]["ActionTestTableView"]["extensions"]["x-ro-nof-tableViewColumns"][1].ToString());
    }

    [Test]
    public void TestGetObjectWithInvalidNames() {
        ClassWithActionAbout.ResetTest();

        var api = Api();
        var result = api.GetObject(FullName<ClassWithInvalidNames>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual(0, ((JContainer)parsedResult["members"]).Count);
    }

    [Test]
    public void TestGetVersion() {
        var api = Api();
        var result = api.GetVersion();
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.IsTrue(parsedResult["implVersion"].ToString().StartsWith("NOF2 "));
    }
}