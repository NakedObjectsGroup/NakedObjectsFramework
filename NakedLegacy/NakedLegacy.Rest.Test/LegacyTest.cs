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
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test;
using NakedLegacy.Reflector.Component;
using NakedLegacy.Reflector.Extensions;
using NakedLegacy.Rest.Test.Data;
using NakedLegacy.Rest.Test.Data.AppLib;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using IMenu = NakedLegacy.Types.IMenu;

namespace NakedLegacy.Rest.Test;

public class LegacyTest : AcceptanceTestCase {
    protected Type[] LegacyTypes { get; } = {
        typeof(ClassWithTextString),
        typeof(ClassWithInternalCollection),
        typeof(ClassWithActionAbout),
        typeof(ClassWithFieldAbout),
        typeof(ILegacyRoleInterface),
        typeof(ClassWithMenu),
        typeof(ClassWithDate),
        typeof(ClassWithTimeStamp),
        typeof(ClassWithWholeNumber),
        typeof(ClassWithLogical),
        typeof(ClassWithMoney),
        typeof(ClassWithReferenceProperty),
        typeof(ClassWithOrderedProperties),
        typeof(ClassWithOrderedActions),
        typeof(ClassWithBounded)
    };

    protected Type[] LegacyServices { get; } = { typeof(SimpleService) };

    protected Type[] LegacyValueHolders { get; } = {
        typeof(Data.AppLib.TextString),
        typeof(Data.AppLib.Money),
        typeof(Data.AppLib.Logical),
        typeof(Data.AppLib.MultiLineTextString),
        typeof(Data.AppLib.WholeNumber),
        typeof(Data.AppLib.NODate),
        typeof(Data.AppLib.NODateNullable),
        typeof(Data.AppLib.TimeStamp)
    };

    protected override bool EnforceProxies => false;

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => _ => { };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

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

    private static NakedFramework.Menu.IMenu MakeMenu<T>(IMenuFactory factory) {
        var t = typeof(T);
        var m = factory.NewMenu(t, false, t.Name);
        var actions = t.GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var action in actions) {
            m.AddAction(action.Name);
        }

        return m;
    }

    protected override NakedFramework.Menu.IMenu[] MainMenus(IMenuFactory factory) =>
        new[] {
            MakeMenu<ClassWithMenu>(factory)
        };


    [SetUp]
    public void SetUp() {
        StartTest();
        ThreadLocals.Initialize(GetConfiguredContainer(), sp => new Container(sp.GetService<INakedFramework>()));
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

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Name"]);
        Assert.IsNotNull(parsedResult["members"]["ActionUpdateName"]);

        Assert.AreEqual("Fred", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetBounded()
    {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithBounded>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Name"]);
        Assert.IsNotNull(parsedResult["members"]["ChoicesProperty"]);
        Assert.AreEqual("True", parsedResult["members"]["ChoicesProperty"]["hasChoices"].ToString());
        Assert.AreEqual("data1", parsedResult["members"]["ChoicesProperty"]["choices"][0]["title"].ToString());
        Assert.AreEqual("data2", parsedResult["members"]["ChoicesProperty"]["choices"][1]["title"].ToString());

        Assert.AreEqual("data1", parsedResult["title"].ToString());
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
        Assert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("string", parsedResult["extensions"]["format"].ToString());
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

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["TestCollection"]);
        Assert.IsNotNull(parsedResult["members"]["ActionUpdateTestCollection"]);

        Assert.AreEqual("1", parsedResult["members"]["TestCollection"]["size"].ToString());
        Assert.AreEqual("collection", parsedResult["members"]["TestCollection"]["memberType"].ToString());
    }

    [Test]
    public void TestGetObjectWithAction() {
        ClassWithActionAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithActionAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
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

        Assert.AreEqual(0, ((JContainer)parsedResult["members"]).Count);
        //Assert.IsNotNull(parsedResult["members"]["Id"]);
    }

    [Test]
    public void TestGetObjectWithField() {
        ClassWithFieldAbout.TestInvisibleFlag = false;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(1, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Name"]);
    }

    [Test]
    public void TestGetObjectWithInvisibleField() {
        ClassWithFieldAbout.TestInvisibleFlag = true;

        var api = Api();
        var result = api.GetObject(FullName<ClassWithFieldAbout>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(0, ((JContainer)parsedResult["members"]).Count);
        //Assert.IsNotNull(parsedResult["members"]["Id"]);
    }

    //[Test]
    //public void TestGetLegacyObjectWithMenu() {
    //    var api = Api();
    //    var result = api.GetObject(FullName<ClassWithMenu>(), "1");
    //    var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
    //    Assert.AreEqual((int)HttpStatusCode.OK, sc);
    //    var parsedResult = JObject.Parse(json);

    //    Assert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);

    //    Assert.IsNotNull(parsedResult["members"]["ActionMethod1"]);
    //    Assert.IsNotNull(parsedResult["members"]["actionMethod2"]);
    //    Assert.AreEqual("Submenu1", parsedResult["members"]["actionMethod2"]["extensions"]["x-ro-nof-menuPath"].ToString());
    //}

    [Test]
    public void TestGetLegacyMainMenu() {
        var api = Api();
        var result = api.GetMenu("ClassWithMenu");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);

        Assert.IsNotNull(parsedResult["members"]["ActionMenuAction"]);
        Assert.IsNotNull(parsedResult["members"]["ActionMenuAction1"]);
    }

    [Test]
    [Ignore("fix locale")]
    public void TestGetObjectWithDate() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithDate>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Date"]);
        Assert.IsNotNull(parsedResult["members"]["ActionUpdateDate"]);

        Assert.AreEqual("11/01/2021", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetDateProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.Date));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithDate.Date), parsedResult["id"].ToString());
        Assert.AreEqual("2021-11-01", parsedResult["value"].ToString());
        Assert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("date", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestGetNullableDateProperty()
    {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.DateNullable));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithDate.DateNullable), parsedResult["id"].ToString());
        Assert.AreEqual("2021-11-01", parsedResult["value"].ToString());
        Assert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("date", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithDate() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newDate", new ScalarValue(new DateTime(1998, 7, 6)) } } };

        var result = api.PostInvoke(FullName<ClassWithDate>(), "1", nameof(ClassWithDate.ActionUpdateDate), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("1998-07-06", resultObj["members"]["Date"]["value"].ToString());
        Assert.AreEqual("string", resultObj["members"]["Date"]["extensions"]["returnType"].ToString());
        Assert.AreEqual("date", resultObj["members"]["Date"]["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeActionThatUsesContainer()
    {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "name", new ScalarValue("Bill") } } };

        var result = api.PostInvoke(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.actionGetObject), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual(2, ((JContainer)resultObj["members"]).Count);
        Assert.IsNull(resultObj["members"]["Id"]);
        Assert.IsNotNull(resultObj["members"]["Name"]);
        Assert.IsNotNull(resultObj["members"]["ActionUpdateName"]);

        Assert.AreEqual("Bill", resultObj["title"].ToString());
    }


    [Test]
    public void TestGetObjectWithTimeStamp() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithTimeStamp>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["TimeStamp"]);
        Assert.IsNotNull(parsedResult["members"]["ActionUpdateTimeStamp"]);

        Assert.AreEqual("01/11/2021 12:00:00", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetTimeStampProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithTimeStamp>(), "1", nameof(ClassWithTimeStamp.TimeStamp));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithTimeStamp.TimeStamp), parsedResult["id"].ToString());
        Assert.AreEqual(DateTime.Parse("11/01/2021 00:00:00", CultureInfo.InvariantCulture), parsedResult["value"].Value<DateTime>());
        Assert.AreEqual("string", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("date-time", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithTimestamp() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newTimeStamp", new ScalarValue(new DateTime(1998, 7, 6)) } } };

        var result = api.PostInvoke(FullName<ClassWithTimeStamp>(), "1", nameof(ClassWithTimeStamp.ActionUpdateTimeStamp), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual(DateTime.Parse("1998-07-06 00:00:00", CultureInfo.InvariantCulture), resultObj["members"]["TimeStamp"]["value"].Value<DateTime>());

        Assert.AreEqual("string", resultObj["members"]["TimeStamp"]["extensions"]["returnType"].ToString());
        Assert.AreEqual("date-time", resultObj["members"]["TimeStamp"]["extensions"]["format"].ToString());
    }

    [Test]
    public void TestGetObjectWithWholeNumber() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithWholeNumber>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["WholeNumber"]);
        Assert.IsNotNull(parsedResult["members"]["actionUpdateWholeNumber"]);

        Assert.AreEqual("10", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetWholeNumberProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithWholeNumber>(), "1", nameof(ClassWithWholeNumber.WholeNumber));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithWholeNumber.WholeNumber), parsedResult["id"].ToString());
        Assert.AreEqual("10", parsedResult["value"].ToString());
        Assert.AreEqual("number", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("int", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithWholeNumber() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newWholeNumber", new ScalarValue(66) } } };

        var result = api.PostInvoke(FullName<ClassWithWholeNumber>(), "1", nameof(ClassWithWholeNumber.actionUpdateWholeNumber), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("66", resultObj["members"]["WholeNumber"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithLogical() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithLogical>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Logical"]);
        Assert.IsNotNull(parsedResult["members"]["actionUpdateLogical"]);

        Assert.AreEqual("True", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetLogicalProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithLogical>(), "1", nameof(ClassWithLogical.Logical));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithLogical.Logical), parsedResult["id"].ToString());
        Assert.AreEqual("True", parsedResult["value"].ToString());
        Assert.AreEqual("boolean", parsedResult["extensions"]["returnType"].ToString());
        Assert.IsNull(parsedResult["extensions"]["format"]);
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithLogical() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newLogical", new ScalarValue(false) } } };

        var result = api.PostInvoke(FullName<ClassWithLogical>(), "1", nameof(ClassWithLogical.actionUpdateLogical), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("False", resultObj["members"]["Logical"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithMoney() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithMoney>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(2, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["Money"]);
        Assert.IsNotNull(parsedResult["members"]["actionUpdateMoney"]);

        Assert.AreEqual("€ 10.00", parsedResult["title"].ToString());
    }

    [Test]
    public void TestGetMoneyProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithMoney>(), "1", nameof(ClassWithMoney.Money));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithMoney.Money), parsedResult["id"].ToString());
        Assert.AreEqual("10", parsedResult["value"].ToString());
        Assert.AreEqual("number", parsedResult["extensions"]["returnType"].ToString());
        Assert.AreEqual("decimal", parsedResult["extensions"]["format"].ToString());
    }

    [Test]
    public void TestInvokeUpdateAndPersistObjectWithMoney() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { { "newMoney", new ScalarValue(66) } } };

        var result = api.PostInvoke(FullName<ClassWithMoney>(), "1", nameof(ClassWithMoney.actionUpdateMoney), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("66", resultObj["members"]["Money"]["value"].ToString());
    }

    [Test]
    public void TestGetObjectWithReferenceProperty() {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithReferenceProperty>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(3, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.IsNotNull(parsedResult["members"]["ReferenceProperty"]);
        Assert.IsNotNull(parsedResult["members"]["actionUpdateReferenceProperty"]);
        Assert.IsNotNull(parsedResult["members"]["actionGetObject"]);

        Assert.AreEqual("Untitled Class With Reference Property", parsedResult["title"].ToString());
    }


    [Test]
    public void TestGetReferencePropertyProperty() {
        var api = Api();
        var result = api.GetProperty(FullName<ClassWithReferenceProperty>(), "1", nameof(ClassWithReferenceProperty.ReferenceProperty));
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(nameof(ClassWithReferenceProperty.ReferenceProperty), parsedResult["id"].ToString());
        Assert.AreEqual("Fred", parsedResult["value"]["title"].ToString());
        Assert.AreEqual(@"http://localhost/objects/NakedLegacy.Rest.Test.Data.ClassWithTextString/1", parsedResult["value"]["href"].ToString());
        Assert.AreEqual("NakedLegacy.Rest.Test.Data.ClassWithTextString", parsedResult["extensions"]["returnType"].ToString());
    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnObject()
    {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuAction), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        var resultObj = parsedResult["result"];

        Assert.AreEqual("Fred", resultObj["title"].ToString());
        Assert.AreEqual("NakedLegacy.Rest.Test.Data.ClassWithTextString", resultObj["extensions"]["domainType"].ToString());

    }

    [Test]
    public void TestInvokeMenuActionWithContainerReturnArrayList()
    {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue> { } };

        var result = api.PostInvokeOnMenu(nameof(ClassWithMenu), nameof(ClassWithMenu.ActionMenuAction1), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual("list", parsedResult["resultType"].ToString());

        var resultObj = parsedResult["result"];

        Assert.AreEqual(2, ((JContainer) resultObj["value"]).Count);
        Assert.AreEqual("Fred", resultObj["value"][0]["title"].ToString());
        Assert.AreEqual("Bill", resultObj["value"][1]["title"].ToString());
    }


    [Test]
    public void TestGetObjectWithOrderedProperties()
    {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithOrderedProperties>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.AreEqual("Name2", ((JProperty)parsedResult["members"].First).Name);
        Assert.AreEqual("0", parsedResult["members"]["Name2"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("Name3", ((JProperty)parsedResult["members"].First.Next).Name);
        Assert.AreEqual("1", parsedResult["members"]["Name3"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("Name1", ((JProperty)parsedResult["members"].First.Next.Next).Name);
        Assert.AreEqual("2", parsedResult["members"]["Name1"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("Name4", ((JProperty)parsedResult["members"].First.Next.Next.Next).Name);
        Assert.AreEqual("4", parsedResult["members"]["Name4"]["extensions"]["memberOrder"].ToString());
    }

    [Test]
    public void TestGetObjectWithOrderedActions()
    {
        var api = Api();
        var result = api.GetObject(FullName<ClassWithOrderedActions>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        Assert.AreEqual(4, ((JContainer)parsedResult["members"]).Count);
        Assert.IsNull(parsedResult["members"]["Id"]);
        Assert.AreEqual("actionAction2", ((JProperty)parsedResult["members"].First).Name);
        Assert.AreEqual("0", parsedResult["members"]["actionAction2"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("actionAction3", ((JProperty)parsedResult["members"].First.Next).Name);
        Assert.AreEqual("1", parsedResult["members"]["actionAction3"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("actionAction1", ((JProperty)parsedResult["members"].First.Next.Next).Name);
        Assert.AreEqual("2", parsedResult["members"]["actionAction1"]["extensions"]["memberOrder"].ToString());

        Assert.AreEqual("actionAction4", ((JProperty)parsedResult["members"].First.Next.Next.Next).Name);
        Assert.AreEqual("4", parsedResult["members"]["actionAction4"]["extensions"]["memberOrder"].ToString());
    }
}