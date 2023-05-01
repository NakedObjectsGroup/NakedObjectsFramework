// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using NakedFramework.Test;
using NakedFramework.Test.TestCase;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Services;
using NakedObjects.SystemTest.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SystemTest.Attributes;

namespace NakedFramework.SystemTest.Attributes;

[TestFixture]
public class TestAttributesNew : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        CreateDatabase();
        StartTest();
    }

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

    private static readonly string todayMinus31 = DateTime.Today.AddDays(-31).ToShortDateString();
    private static readonly string todayMinus30 = DateTime.Today.AddDays(-30).ToShortDateString();
    private static readonly string todayMinus1 = DateTime.Today.AddDays(-1).ToShortDateString();
    private static readonly string today = DateTime.Today.ToShortDateString();
    private static readonly string todayPlus1 = DateTime.Today.AddDays(1).ToShortDateString();
    private static readonly string todayPlus30 = DateTime.Today.AddDays(30).ToShortDateString();
    private static readonly string todayPlus31 = DateTime.Today.AddDays(31).ToShortDateString();

    protected override Type[] ObjectTypes {
        get {
            return base.ObjectTypes.Union(new[] {
                typeof(Default1),
                typeof(Describedas1),
                typeof(Describedas2),
                typeof(Description1),
                typeof(Description2),
                typeof(Disabled1),
                typeof(Displayname1),
                typeof(Hidden1),
                typeof(Iconname1),
                typeof(Iconname2),
                typeof(Iconname3),
                typeof(Iconname4),
                typeof(Immutable1),
                typeof(Immutable2),
                typeof(Immutable3),
                typeof(Mask1),
                typeof(Mask2),
                typeof(Maxlength1),
                typeof(Maxlength2),
                typeof(NakedObjectsIgnore1),
                typeof(NakedObjectsIgnore2), //But this one won't be visible
                typeof(NakedObjectsIgnore3),
                typeof(NakedObjectsIgnore4),
                typeof(NakedObjectsIgnore5),
                typeof(NakedObjectsIgnore6),
                typeof(NakedObjectsIgnore7),
                typeof(Named1),
                typeof(Range1),
                typeof(Regex1),
                typeof(Regex2),
                typeof(Memberorder1),
                typeof(Memberorder2),
                typeof(Stringlength1),
                typeof(Title1),
                typeof(Title2),
                typeof(Title3),
                typeof(Title4),
                typeof(Title5),
                typeof(Title6),
                typeof(Title7),
                typeof(Title8),
                typeof(Title9),
                typeof(Validateprogrammaticupdates1),
                typeof(Validateprogrammaticupdates2),
                typeof(Contributee),
                typeof(Contributee2),
                typeof(Contributee3),
                typeof(FinderAction1)
            }).ToArray();
        }
    }

    protected override Type[] Services {
        get {
            return new[] {
                typeof(SimpleRepository<Default1>),
                typeof(SimpleRepository<Describedas1>),
                typeof(SimpleRepository<Describedas2>),
                typeof(SimpleRepository<Description1>),
                typeof(SimpleRepository<Description2>),
                typeof(SimpleRepository<Disabled1>),
                typeof(SimpleRepository<Displayname1>),
                typeof(SimpleRepository<Hidden1>),
                typeof(SimpleRepository<Iconname1>),
                typeof(SimpleRepository<Iconname2>),
                typeof(SimpleRepository<Iconname3>),
                typeof(SimpleRepository<Iconname4>),
                typeof(SimpleRepository<Immutable1>),
                typeof(SimpleRepository<Immutable2>),
                typeof(SimpleRepository<Immutable3>),
                typeof(SimpleRepository<Mask1>),
                typeof(SimpleRepository<Mask2>),
                typeof(SimpleRepository<Maxlength1>),
                typeof(SimpleRepository<Maxlength2>),
                typeof(SimpleRepository<NakedObjectsIgnore1>),
                typeof(SimpleRepository<NakedObjectsIgnore2>), //But this one won't be visible
                typeof(SimpleRepository<NakedObjectsIgnore3>),
                typeof(SimpleRepository<NakedObjectsIgnore4>),
                typeof(SimpleRepository<NakedObjectsIgnore5>),
                typeof(SimpleRepository<NakedObjectsIgnore6>),
                typeof(SimpleRepository<NakedObjectsIgnore7>),
                typeof(SimpleRepository<Named1>),
                typeof(SimpleRepository<Range1>),
                typeof(SimpleRepository<Regex1>),
                typeof(SimpleRepository<Regex2>),
                typeof(SimpleRepository<Memberorder1>),
                typeof(SimpleRepository<Memberorder2>),
                typeof(SimpleRepository<Stringlength1>),
                typeof(SimpleRepository<Title1>),
                typeof(SimpleRepository<Title2>),
                typeof(SimpleRepository<Title3>),
                typeof(SimpleRepository<Title4>),
                typeof(SimpleRepository<Title5>),
                typeof(SimpleRepository<Title6>),
                typeof(SimpleRepository<Title7>),
                typeof(SimpleRepository<Title8>),
                typeof(SimpleRepository<Title9>),
                typeof(SimpleRepository<Validateprogrammaticupdates1>),
                typeof(SimpleRepository<Validateprogrammaticupdates2>),
                typeof(TestServiceValidateProgrammaticUpdates),
                typeof(SimpleRepository<Contributee>),
                typeof(SimpleRepository<Contributee2>),
                typeof(SimpleRepository<Contributee3>),
                typeof(TestServiceContributedAction),
                typeof(SimpleRepository<FinderAction1>),
                typeof(TestServiceFinderAction)
            };
        }
    }

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AttributesDbContext() };

    protected virtual void CleanUpDatabase() {
        //AttributesDbContext.Delete();
    }

    protected virtual void CreateDatabase() {
        //new AttributesDbContext().Database.Create();
    }

    protected override void RegisterTypes(IServiceCollection services) {
        base.RegisterTypes(services);
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }

    protected override Action<NakedFrameworkOptions> AddNakedFunctions => builder => { };

    protected RestfulObjectsControllerBase Api() {
        var sp = GetConfiguredContainer();
        var api = sp.GetService<RestfulObjectsController>();
        return Helpers.SetMockContext(api, sp);
    }

    private static string FullName<T>() => typeof(T).FullName;

    private JObject GetObject<T>(string id = "1") {
        var api = Api();
        var result = api.GetObject(FullName<T>(), id);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private JObject GetObjects<T>() where T : class, new() {
        var api = Api();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnService(FullName<SimpleRepository<T>>(), nameof(SimpleRepository<T>.AllInstances), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private JObject GetTransientObject<T>() where T : class, new() {
        var api = Api().AsPost();
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnService(FullName<SimpleRepository<T>>(), nameof(SimpleRepository<T>.NewInstance), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private static void AssertActionOrderIs(JProperty[] properties, params string[] names) {
        Assert.AreEqual(names.Length, properties.Length);

        for (var i = 0; i < names.Length; i++) {
            Assert.AreEqual(names[i], properties[i].Name);
        }
    }

    private static void AssertMemberOrderExtensionIs(JProperty[] properties, params int[] order) {
        Assert.AreEqual(order.Length, properties.Length);

        for (var i = 0; i < order.Length; i++) {
            Assert.AreEqual(order[i], properties[i].Value["extensions"]["memberOrder"].Value<int>());
        }
    }

    private static JProperty[] GetActions(JObject obj) {
        var actions = obj["members"].Cast<JProperty>().Where(p => p.Value["memberType"].ToString() is "action").ToArray();
        return actions;
    }

    [Test]
    public void ActionOrder() {
        var obj = GetObject<Memberorder1>();
        var actions = GetActions(obj);
        AssertActionOrderIs(actions, "Action2", "Action1");
        AssertMemberOrderExtensionIs(actions, 1, 3);
    }

    [Test]
    public void ActionOrderOnSubClass() {
        var obj = GetObject<Memberorder2>("2");
        var actions = GetActions(obj);
        AssertActionOrderIs(actions, "Action2", "Action4", "Action1", "Action3");
        AssertMemberOrderExtensionIs(actions, 1, 2, 3, 4);
    }

    [Test]
    public virtual void CMaskOnDecimalProperty() {
        var mask1 = GetObject<Mask2>();
        var prop1 = mask1["members"]["Prop1"];

        Assert.AreEqual(32.7, prop1["value"].Value<decimal>());
        Assert.AreEqual("decimal", prop1["extensions"]["format"].ToString());
        Assert.AreEqual("c", prop1["extensions"]["x-ro-nof-mask"].ToString());
    }

    [Test]
    public virtual void CollectionContributed() {
        var obj = GetObjects<Contributee2>();
        var actions = GetActions(obj["result"] as JObject);
        AssertActionOrderIs(actions, "CollectionContributedAction", "CollectionContributedAction1", "CollectionContributedAction2");
    }

    [Test]
    public virtual void CollectionContributedNotToAnotherClass() {
        var obj = GetObjects<Contributee>();
        var actions = GetActions(obj["result"] as JObject);
        AssertActionOrderIs(actions);
    }

    [Test]
    public virtual void CollectionContributedToSubClass() {
        var obj = GetObjects<Contributee3>();
        var actions = GetActions(obj["result"] as JObject);
        AssertActionOrderIs(actions, "CollectionContributedAction", "CollectionContributedAction1", "CollectionContributedAction2");
    }

    [Test]
    public virtual void ComponentModelMaxLengthOnParm() {
        var obj = GetObject<Maxlength2>();
        var act = obj["members"]["Action"];

        Assert.AreEqual(8, act["parameters"]["parm"]["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void ComponentModelMaxLengthOnProperty() {
        var obj = GetObject<Maxlength2>();
        var prop2 = obj["members"]["Prop2"];
        Assert.AreEqual(7, prop2["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void Contributed() {
        var obj = GetObject<Contributee>();
        var actions = GetActions(obj);
        AssertActionOrderIs(actions, "ContributedAction");
    }

    [Test]
    public virtual void DefaultNumericProperty() {
        var obj = GetTransientObject<Default1>();
        var prop = obj["result"]["members"]["Prop1"];
        var def = prop["value"];
        Assert.AreEqual(8, def.Value<int>());
    }

    [Test]
    public virtual void DefaultStringProperty() {
        var obj = GetTransientObject<Default1>();
        var prop = obj["result"]["members"]["Prop2"];
        var def = prop["value"];
        Assert.AreEqual("Foo", def.Value<string>());
    }

    [Test]
    public void DefaultParameters() {
        var obj = GetObject<Default1>();
        var action = obj["members"]["DoSomething"];
        var def0 = action["parameters"]["param0"]["default"];

        Assert.AreEqual(8, def0.Value<int>());

        var def1 = action["parameters"]["param1"]["default"];

        Assert.AreEqual("Foo", def1.Value<string>());
    }

    [Test]
    public virtual void DescribedAsAppliedToAction() {
        var obj = GetObject<Describedas1>();
        var action = obj["members"]["DoSomething"];
        Assert.AreEqual("Hex", action["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescribedAsAppliedToObject() {
        var obj = GetObject<Describedas1>();
        Assert.AreEqual("Foo", obj["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescribedAsAppliedToParameter() {
        var obj = GetObject<Describedas1>();
        var action = obj["members"]["DoSomething"];
        var param = action["parameters"]["param1"];
        Assert.AreEqual("Yop", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescribedAsAppliedToProperty() {
        var obj = GetObject<Describedas1>();
        var prop = obj["members"]["Prop1"];
        Assert.AreEqual("Bar", prop["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToAction() {
        var obj = GetObject<Description1>();
        var action = obj["members"]["DoSomething"];
        Assert.AreEqual("Hex", action["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToObject() {
        var obj = GetObject<Description1>();
        Assert.AreEqual("Foo", obj["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToParameter() {
        var obj = GetObject<Description1>();

        var action = obj["members"]["DoSomething"];
        var param = action["parameters"]["param1"];
        Assert.AreEqual("Yop", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToProperty() {
        var obj = GetObject<Description1>();
        var prop = obj["members"]["Prop1"];
        Assert.AreEqual("Bar", prop["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DisabledTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop1 = obj["result"]["members"]["Prop1"];

        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
    }

    [Test]
    public virtual void Disabled() {
        var obj = GetObject<Disabled1>();
        var prop1 = obj["members"]["Prop1"];

        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledAlwaysTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop5 = obj["result"]["members"]["Prop5"];
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
    }
    [Test]
    public virtual void DisabledAlways() {
        var obj = GetObject<Disabled1>();
        var prop5 = obj["members"]["Prop5"];
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledNeverTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop4 = obj["result"]["members"]["Prop4"];
        Assert.AreEqual(null, prop4["disabledReason"]);
    }

    [Test]
    public virtual void DisabledNever() {
        var obj = GetObject<Disabled1>();
        var prop4 = obj["members"]["Prop4"];
        Assert.AreEqual(null, prop4["disabledReason"]);
    }

    [Test]
    public virtual void DisabledOncePersistedTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop2 = obj["result"]["members"]["Prop2"];
        Assert.AreEqual(null, prop2["disabledReason"]);
    }

    [Test]
    public virtual void DisabledOncePersisted() {
        var obj = GetObject<Disabled1>();
        var prop2 = obj["members"]["Prop2"];
        Assert.AreEqual("Field not editable now that object is persistent", prop2["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledUntilPersistedTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop3 = obj["result"]["members"]["Prop3"];
        Assert.AreEqual("Field not editable until the object is persistent", prop3["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledUntilPersisted() {
        var obj = GetObject<Disabled1>();
        var prop3 = obj["members"]["Prop3"];
        Assert.AreEqual(null, prop3["disabledReason"]);
    }
}