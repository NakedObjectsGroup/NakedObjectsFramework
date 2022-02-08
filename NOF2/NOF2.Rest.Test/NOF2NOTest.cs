// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
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

namespace NOF2.Rest.Test;

public class NOF2NOTest : AcceptanceTestCase {
    protected Type[] NOF2Types { get; } = {
        typeof(ClassWithTextString),
        typeof(ClassWithInternalCollection),
        typeof(ClassWithActionAbout),
        typeof(ClassWithFieldAbout),
        typeof(ClassWithLinkToNOFClass),
        typeof(ClassWithNOFInternalCollection),
        typeof(NOF2ClassWithInterface),
        typeof(INOF2RoleInterface),
        typeof(ClassWithMenu),
        typeof(ClassWithDate),
        typeof(ClassWithTimeStamp),
        typeof(ClassWithWholeNumber),
        typeof(ClassWithLogical),
        typeof(ClassWithMoney),
        typeof(ClassWithReferenceProperty)
    };

    protected Type[] NOF2Services { get; } = { typeof(SimpleService) };

    protected Type[] NOF2ValueHolders { get; } = {
        typeof(TextString),
        typeof(Money),
        typeof(Logical),
        typeof(MultiLineTextString),
        typeof(WholeNumber),
        typeof(NODate),
        typeof(TimeStamp)
    };

    protected override Type[] ObjectTypes { get; } = {
        typeof(ClassWithString),
        typeof(ClassWithNOF2Interface),
        typeof(IRoleInterface)
    };

    protected override Type[] Services { get; } = { typeof(SimpleNOService) };

    protected override bool EnforceProxies => false;

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => _ => { };

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
            AddNakedObjects(builder);
            //AddNakedFunctions(builder);
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

    //[Test]
    //public void TestInvokeUpdateAndPersistObjectWithInternalCollection() {
    //    var api = Api().AsPost();
    //    var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newName", new ScalarValue("Bill") } } };

    //    var result = api.PostInvoke(FullName<ClassWithInternalCollection>(), "2", nameof(ClassWithInternalCollection.ActionUpdateTestCollection), map);
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    var resultObj = parsedResult["result"];

    //    Assert.AreEqual("1", resultObj["members"]["TestCollection"]["size"].ToString());
    //    Assert.AreEqual("collection", resultObj["members"]["TestCollection"]["memberType"].ToString());
    //}

    //[Test]
    //public void TestAboutCaching() {
    //    ClassWithActionAbout.AboutCount = 0;

    //    var api = Api();
    //    var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
    //    Assert.AreEqual(1, ClassWithActionAbout.AboutCount);
    //    //Assert.IsNotNull(parsedResult["members"]["Id"]);
    //}

    //[Test]
    //public void TestNOFToNOF2() {
    //    var api = Api();
    //    var result = api.GetObject(FullName<ClassWithString>(), "1");
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
    //    Assert.IsNotNull(parsedResult["members"]["LinkToNOF2Class"]);
    //    Assert.IsNotNull(parsedResult["members"]["CollectionOfNOF2Class"]);

    //    Assert.AreEqual("Ted", parsedResult["members"]["LinkToNOF2Class"]["value"]["title"].ToString());
    //}

    [Test]
    public void TestNOFToNOF2Collection() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithString>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["LinkToNOF2Class"]);
        Assert.IsNotNull(parsedResult["members"]["CollectionOfNOF2Class"]);

        Assert.AreEqual("2", parsedResult["members"]["CollectionOfNOF2Class"]["size"].ToString());
    }

    //[Test]
    //public void TestNOF2ToNOF() {
    //    var api = Api();
    //    var result = api.GetObject(FullName<ClassWithLinkToNOFClass>(), "1");
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
    //    Assert.IsNotNull(parsedResult["members"]["LinkToNOFClass"]);

    //    Assert.AreEqual("Untitled Class With String", parsedResult["members"]["LinkToNOFClass"]["value"]["title"].ToString());
    //}

    [Test]
    public void TestNOF2ToNOFCollection() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithNOFInternalCollection>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["CollectionOfNOFClass"]);

        Assert.AreEqual("2", parsedResult["members"]["CollectionOfNOFClass"]["size"].ToString());
    }

    [Test]
    public void TestGetObjectWithNOF2Interface() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithNOF2Interface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetObjectWithNOF2InterfaceConfirmSubtype() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "supertype", new ScalarValue(FullName<INOF2RoleInterface>()) } } };
        var api = Api();
        var result = api.GetInvokeTypeActions(FullName<ClassWithNOF2Interface>(), "isSubtypeOf", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("True", parsedResult["value"].ToString());
    }

    [Test]
    public void TestGetNOF2ObjectWithInterface() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<NOF2ClassWithInterface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetNOF2ObjectWithContributedAction() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<NOF2ClassWithInterface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["ContributedAction"]);
    }

    [Test]
    public void TestGetNOF2ObjectWithInterfaceConfirmSubtype() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "supertype", new ScalarValue(FullName<IRoleInterface>()) } } };
        var api = Api();
        var result = api.GetInvokeTypeActions(FullName<NOF2ClassWithInterface>(), "isSubtypeOf", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("True", parsedResult["value"].ToString());
    }
}