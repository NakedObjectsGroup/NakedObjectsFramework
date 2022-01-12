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
using NakedFunctions.Rest.Test;
using NakedLegacy.Reflector.Extensions;
using NakedLegacy.Rest.Test.Data;
using NakedLegacy.Rest.Test.Data.AppLib;
using NakedLegacy;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedLegacy.Rest.Test;

public class LegacyNOFTest : AcceptanceTestCase {
    protected Type[] LegacyTypes { get; } = {
        typeof(ClassWithTextString),
        typeof(ClassWithInternalCollection),
        typeof(ClassWithActionAbout),
        typeof(ClassWithFieldAbout),
        typeof(ClassWithLinkToNOFClass),
        typeof(ClassWithNOFInternalCollection),
        typeof(LegacyClassWithInterface),
        typeof(ILegacyRoleInterface),
        typeof(ClassWithMenu),
        typeof(ClassWithDate),
        typeof(ClassWithTimeStamp),
        typeof(ClassWithWholeNumber),
        typeof(ClassWithLogical),
        typeof(ClassWithMoney),
        typeof(ClassWithReferenceProperty)
    };

    protected Type[] LegacyServices { get; } = { typeof(SimpleService) };


    protected Type[] LegacyValueHolders { get; } = {
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
        typeof(ClassWithLegacyInterface),
        typeof(IRoleInterface)
    };

    protected override Type[] Services { get; } = { typeof(SimpleNOService) };

    protected override bool EnforceProxies => false;

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => _ => { };

    protected Action<NakedLegacyOptions> LegacyOptions =>
        options => {
            options.DomainModelTypes = LegacyTypes;
            options.DomainModelServices = LegacyServices;
            options.ValueHolderTypes = LegacyValueHolders;
            options.NoValidate = true;
        };

    protected virtual Action<NakedFrameworkOptions> AddLegacy => builder => builder.AddNakedLegacy(LegacyOptions);

    protected override Action<NakedFrameworkOptions> NakedFrameworkOptions =>
        builder => {
            AddCoreOptions(builder);
            AddPersistor(builder);
            AddNakedObjects(builder);
            //AddNakedFunctions(builder);
            AddRestfulObjects(builder);
            AddLegacy(builder);
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
    //public void TestNOFToLegacy() {
    //    var api = Api();
    //    var result = api.GetObject(FullName<ClassWithString>(), "1");
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
    //    Assert.IsNotNull(parsedResult["members"]["LinkToLegacyClass"]);
    //    Assert.IsNotNull(parsedResult["members"]["CollectionOfLegacyClass"]);

    //    Assert.AreEqual("Ted", parsedResult["members"]["LinkToLegacyClass"]["value"]["title"].ToString());
    //}

    [Test]
    public void TestNOFToLegacyCollection() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithString>(), "2");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["LinkToLegacyClass"]);
        Assert.IsNotNull(parsedResult["members"]["CollectionOfLegacyClass"]);

        Assert.AreEqual("2", parsedResult["members"]["CollectionOfLegacyClass"]["size"].ToString());
    }

    //[Test]
    //public void TestLegacyToNOF() {
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
    public void TestLegacyToNOFCollection() {
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
    public void TestGetObjectWithLegacyInterface() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithLegacyInterface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetObjectWithLegacyInterfaceConfirmSubtype() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "supertype", new ScalarValue(FullName<ILegacyRoleInterface>()) } } };
        var api = Api();
        var result = api.GetInvokeTypeActions(FullName<ClassWithLegacyInterface>(), "isSubtypeOf", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("True", parsedResult["value"].ToString());
    }

    [Test]
    public void TestGetLegacyObjectWithInterface() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<LegacyClassWithInterface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetLegacyObjectWithContributedAction() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<LegacyClassWithInterface>(), "10");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNotNull(parsedResult["members"]["ContributedAction"]);
    }

    [Test]
    public void TestGetLegacyObjectWithInterfaceConfirmSubtype() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "supertype", new ScalarValue(FullName<IRoleInterface>()) } } };
        var api = Api();
        var result = api.GetInvokeTypeActions(FullName<LegacyClassWithInterface>(), "isSubtypeOf", map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("True", parsedResult["value"].ToString());
    }
}