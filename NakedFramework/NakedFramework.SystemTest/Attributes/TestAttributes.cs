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
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedFramework.Rest.Model;
using NakedFramework.Test;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Exceptions;
using SystemTest.Attributes;
using RestfulObjectsController = NakedFramework.RATL.Helpers.RestfulObjectsController;

namespace NakedFramework.SystemTest.Attributes;

[TestFixture]
public class TestAttributes : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.AddEF6Persistor(options => { options.ContextCreators = ContextCreators; });
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = ObjectTypes;
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected Type[] ObjectTypes {
        get {
            return new[] {
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
                typeof(FinderAction1),
                typeof(Multiline1)
            };
        }
    }

    protected Type[] Services {
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

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AttributesDbContext() };

    private const string format = "yyyy-MM-dd";

    private static readonly string todayMinus31 = DateTime.Today.AddDays(-31).ToString(format);
    private static readonly string todayMinus30 = DateTime.Today.AddDays(-30).ToString(format);
    private static readonly string todayMinus1 = DateTime.Today.AddDays(-1).ToString(format);
    private static readonly string today = DateTime.Today.ToString(format);
    private static readonly string todayPlus1 = DateTime.Today.AddDays(1).ToString(format);
    private static readonly string todayPlus30 = DateTime.Today.AddDays(30).ToString(format);
    private static readonly string todayPlus31 = DateTime.Today.AddDays(31).ToString(format);

    private JObject GetObject<T>(string id = "1") {
        var api = Api();
        var result = api.GetObject(FullName<T>(), id);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private JObject GetService<T>() {
        var api = Api();
        var result = api.GetService(FullName<T>());
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
        var api = TestHelpers.AsPost(Api());
        var map = new ArgumentMap { Map = new Dictionary<string, IValue>() };
        var result = api.GetInvokeOnService(FullName<SimpleRepository<T>>(), nameof(SimpleRepository<T>.NewInstance), map);
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        return JObject.Parse(json);
    }

    private static void AssertOrderIs(JProperty[] properties, params string[] names) {
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

    private static JProperty[] GetActions(JObject obj) => obj["members"].Cast<JProperty>().Where(p => p.Value["memberType"].ToString() is "action").ToArray();

    private static JProperty[] GetProperties(JObject obj) => obj["members"].Cast<JProperty>().Where(p => p.Value["memberType"].ToString() is "property").ToArray();

    private static JProperty[] GetCollections(JObject obj) => obj["members"].Cast<JProperty>().Where(p => p.Value["memberType"].ToString() is "collection").ToArray();

    private static JToken GetMember(JObject obj, string name) => obj["members"][name];

    private void NumericPropertyRangeTest(JObject obj, string name) {
        var prop = GetMember(obj, name);
        var range = prop["extensions"]["x-ro-nof-range"];
        var min = range["min"];
        var max = range["max"];
        Assert.AreEqual("-1", min.ToString());
        Assert.AreEqual("10", max.ToString());
    }

    private void NumericParmRangeTest(JObject obj, string name) {
        var act = GetMember(obj, name);
        var range = act["parameters"]["parm"]["extensions"]["x-ro-nof-range"];
        var min = range["min"];
        var max = range["max"];
        Assert.AreEqual("5", min.ToString());
        Assert.AreEqual("6", max.ToString());
    }

    //[Test]
    //public virtual void ActionsIncludedInFinderMenu() {
    //    var service = (TestServiceFinderAction)GetTestService(typeof(TestServiceFinderAction)).NakedObject.Object;
    //    var obj = service.NewObject1();
    //    var adapter = NakedFramework.NakedObjectManager.CreateAdapter(obj, null, null);
    //    var finderActions = ((IObjectSpec)adapter.Spec).GetFinderActions();

    //    Assert.AreEqual(3, finderActions.Length);
    //    Assert.AreEqual("Finder Action1", finderActions[0].Name(null));
    //    Assert.AreEqual("Finder Action2", finderActions[1].Name(null));
    //    Assert.AreEqual("Finder Action3", finderActions[2].Name(null));
    //}

    [Test]
    public void ActionOrder() {
        var obj = GetObject<Memberorder1>();
        var actions = GetActions(obj);
        AssertOrderIs(actions, nameof(Memberorder1.Action2), nameof(Memberorder1.Action1));
        AssertMemberOrderExtensionIs(actions, 1, 3);
    }

    [Test]
    public void ActionOrderOnSubClass() {
        var obj = GetObject<Memberorder2>("2");
        var actions = GetActions(obj);
        AssertOrderIs(actions, nameof(Memberorder2.Action2), nameof(Memberorder2.Action4), nameof(Memberorder2.Action1), nameof(Memberorder2.Action3));
        AssertMemberOrderExtensionIs(actions, 1, 2, 3, 4);
    }

    [Test]
    public virtual void CMaskOnDecimalProperty() {
        var mask1 = GetObject(FullName<Mask2>(), "1");
        var prop1 = mask1.GetProperty(nameof(Mask2.Prop1));

        Assert.AreEqual(32.7, prop1.GetValue<decimal>());
        Assert.AreEqual("decimal", prop1.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.format));
        Assert.AreEqual("c", prop1.GetExtensions().GetExtension<string>(ExtensionsApi.ExtensionKeys.x_ro_nof_mask));
    }

    [Test]
    public virtual void CollectionContributed() {
        var obj = GetObjects<Contributee2>();
        var actions = GetActions(obj["result"] as JObject);
        AssertOrderIs(actions, "CollectionContributedAction", "CollectionContributedAction1", "CollectionContributedAction2");
    }

    [Test]
    public virtual void CollectionContributedNotToAnotherClass() {
        var obj = GetObjects<Contributee>();
        var actions = GetActions(obj["result"] as JObject);
        AssertOrderIs(actions);
    }

    [Test]
    public virtual void CollectionContributedToSubClass() {
        var obj = GetObjects<Contributee3>();
        var actions = GetActions(obj["result"] as JObject);
        AssertOrderIs(actions, "CollectionContributedAction", "CollectionContributedAction1", "CollectionContributedAction2");
    }

    [Test]
    public virtual void ComponentModelMaxLengthOnParm() {
        var obj = GetObject<Maxlength2>();
        var act = GetMember(obj, nameof(Maxlength2.Action));

        Assert.AreEqual(8, act["parameters"]["parm"]["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void ComponentModelMaxLengthOnProperty() {
        var obj = GetObject<Maxlength2>();
        var prop2 = GetMember(obj, nameof(Maxlength2.Prop2));
        Assert.AreEqual(7, prop2["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void Contributed() {
        var obj = GetObject<Contributee>();
        var actions = GetActions(obj);
        AssertOrderIs(actions, "ContributedAction");
    }

    [Test]
    public virtual void DefaultNumericProperty() {
        var obj = GetTransientObject<Default1>();
        var prop = GetMember((JObject)obj["result"], nameof(Default1.Prop1));
        var def = prop["value"];
        Assert.AreEqual(8, def.Value<int>());
    }

    [Test]
    public virtual void DefaultStringProperty() {
        var obj = GetTransientObject<Default1>();
        var prop = GetMember((JObject)obj["result"], nameof(Default1.Prop2));
        var def = prop["value"];
        Assert.AreEqual("Foo", def.Value<string>());
    }

    [Test]
    public void DefaultParameters() {
        var obj = GetObject<Default1>();
        var action = GetMember(obj, nameof(Default1.DoSomething));
        var def0 = action["parameters"]["param0"]["default"];

        Assert.AreEqual(8, def0.Value<int>());

        var def1 = action["parameters"]["param1"]["default"];

        Assert.AreEqual("Foo", def1.Value<string>());
    }

    [Test]
    public virtual void DescribedAsAppliedToAction() {
        var obj = GetObject<Describedas1>();
        var action = GetMember(obj, nameof(Describedas1.DoSomething));
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
        var action = GetMember(obj, nameof(Describedas1.DoSomething));
        var param = action["parameters"]["param1"];
        Assert.AreEqual("Yop", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescribedAsAppliedToProperty() {
        var obj = GetObject<Describedas1>();
        var prop = GetMember(obj, nameof(Describedas1.Prop1));
        Assert.AreEqual("Bar", prop["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToAction() {
        var obj = GetObject<Description1>();
        var action = GetMember(obj, nameof(Description1.DoSomething));
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

        var action = GetMember(obj, nameof(Description1.DoSomething));
        var param = action["parameters"]["param1"];
        Assert.AreEqual("Yop", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DescriptionAppliedToProperty() {
        var obj = GetObject<Description1>();
        var prop = GetMember(obj, nameof(Description1.Prop1));
        Assert.AreEqual("Bar", prop["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void DisabledTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop1 = GetMember((JObject)obj["result"], nameof(Disabled1.Prop1));

        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
    }

    [Test]
    public virtual void Disabled() {
        var obj = GetObject<Disabled1>();
        var prop1 = GetMember(obj, nameof(Disabled1.Prop1));

        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledAlwaysTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop5 = GetMember((JObject)obj["result"], nameof(Disabled1.Prop5));
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledAlways() {
        var obj = GetObject<Disabled1>();
        var prop5 = GetMember(obj, nameof(Disabled1.Prop5));
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledNeverTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop4 = GetMember((JObject)obj["result"], nameof(Disabled1.Prop4));
        Assert.AreEqual(null, prop4["disabledReason"]);
    }

    [Test]
    public virtual void DisabledNever() {
        var obj = GetObject<Disabled1>();
        var prop4 = GetMember(obj, nameof(Disabled1.Prop4));
        Assert.AreEqual(null, prop4["disabledReason"]);
    }

    [Test]
    public virtual void DisabledOncePersistedTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop2 = GetMember((JObject)obj["result"], nameof(Disabled1.Prop2));
        Assert.AreEqual(null, prop2["disabledReason"]);
    }

    [Test]
    public virtual void DisabledOncePersisted() {
        var obj = GetObject<Disabled1>();
        var prop2 = GetMember(obj, nameof(Disabled1.Prop2));
        Assert.AreEqual("Field not editable now that object is persistent", prop2["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledUntilPersistedTransient() {
        var obj = GetTransientObject<Disabled1>();
        var prop3 = GetMember((JObject)obj["result"], nameof(Disabled1.Prop3));
        Assert.AreEqual("Field not editable until the object is persistent", prop3["disabledReason"].ToString());
    }

    [Test]
    public virtual void DisabledUntilPersisted() {
        var obj = GetObject<Disabled1>();
        var prop3 = GetMember(obj, nameof(Disabled1.Prop3));
        Assert.AreEqual(null, prop3["disabledReason"]);
    }

    [Test]
    public virtual void DisplayNameAppliedToAction() {
        var obj = GetObject<Displayname1>();
        var action = GetMember(obj, nameof(Displayname1.DoSomething));
        Assert.AreEqual("Hex", action["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public virtual void DisplayNameAppliedToObject() {
        var obj = GetObject<Displayname1>();
        Assert.AreEqual("Untitled Foo", obj["title"].ToString());
    }

    [Test]
    public virtual void DMaskOnDateProperty() {
        var obj = GetObject<Mask1>();
        var prop1 = GetMember(obj, nameof(Mask1.Prop1));
        var prop2 = GetMember(obj, nameof(Mask1.Prop2));

        Assert.AreEqual("2009-09-23", prop1["value"].ToString());
        Assert.AreEqual("date", prop1["extensions"]["format"].ToString());

        Assert.AreEqual("2009-09-24", prop2["value"].ToString());
        Assert.AreEqual("date", prop2["extensions"]["format"].ToString());
        Assert.AreEqual("d", prop2["extensions"]["x-ro-nof-mask"].ToString());
    }

    [Test]
    public virtual void HiddenTransient() {
        var obj = GetTransientObject<Hidden1>();
        var prop1 = GetMember((JObject)obj["result"], nameof(Hidden1.Prop1));
        Assert.IsNull(prop1);
    }

    [Test]
    public virtual void Hidden() {
        var obj = GetObject<Hidden1>();
        var prop1 = GetMember(obj, nameof(Hidden1.Prop1));
        Assert.IsNull(prop1);
    }

    [Test]
    public virtual void HiddenAlwaysTransient() {
        var obj = GetTransientObject<Hidden1>();
        var prop5 = GetMember((JObject)obj["result"], nameof(Hidden1.Prop5));
        Assert.IsNull(prop5);
    }

    [Test]
    public virtual void HiddenAlways() {
        var obj = GetObject<Hidden1>();
        var prop5 = GetMember(obj, nameof(Hidden1.Prop5));
        Assert.IsNull(prop5);
    }

    [Test]
    public virtual void HiddenNeverTransient() {
        var obj = GetTransientObject<Hidden1>();
        var prop4 = GetMember((JObject)obj["result"], nameof(Hidden1.Prop4));
        Assert.IsNotNull(prop4);
    }

    [Test]
    public virtual void HiddenNever() {
        var obj = GetObject<Hidden1>();
        var prop4 = GetMember(obj, nameof(Hidden1.Prop4));
        Assert.IsNotNull(prop4);
    }

    [Test]
    public virtual void HiddenOncePersistedTransient() {
        var obj = GetTransientObject<Hidden1>();
        var prop2 = GetMember((JObject)obj["result"], nameof(Hidden1.Prop2));
        Assert.IsNotNull(prop2);
    }

    [Test]
    public virtual void HiddenOncePersisted() {
        var obj = GetObject<Hidden1>();
        var prop2 = GetMember(obj, nameof(Hidden1.Prop2));
        Assert.IsNull(prop2);
    }

    [Test]
    public virtual void HiddenUntilPersistedTransient() {
        var obj = GetTransientObject<Hidden1>();
        var prop3 = GetMember((JObject)obj["result"], nameof(Hidden1.Prop3));
        Assert.IsNull(prop3);
    }

    [Test]
    public virtual void HiddenUntilPersisted() {
        var obj = GetObject<Hidden1>();
        var prop3 = GetMember(obj, nameof(Hidden1.Prop3));
        Assert.IsNotNull(prop3);
    }

    //[Test]
    //public virtual void NakedObjectsIgnore_OnIndividualMembers() {
    //    var obj = GetObject<NakedObjectsIgnore1>();
    //    //Note: numbers will change to 3 & 1 when NakedObjectsType
    //    //is re-introduced and commented back in
    //    Assert.AreEqual(3, GetProperties(obj).Length);
    //    Assert.AreEqual(2, GetCollections(obj).Length);
    //    Assert.AreEqual(3, GetActions(obj).Length);
    //}

    [Test]
    public virtual void NakedObjectsMaxLengthOnParm() {
        var obj = GetObject<Maxlength1>();
        var act = GetMember(obj, nameof(Maxlength1.Action));

        Assert.AreEqual(8, act["parameters"]["parm"]["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void NakedObjectsMaxLengthOnProperty() {
        var obj = GetObject<Maxlength1>();
        var prop2 = GetMember(obj, nameof(Maxlength1.Prop2));
        Assert.AreEqual(7, prop2["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void NamedAppliedToAction() {
        var obj = GetObject<Named1>();
        var action = GetMember(obj, nameof(Named1.DoSomething));
        Assert.AreEqual("Hex", action["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public virtual void NamedAppliedToObject() {
        var obj = GetObject<Named1>();
        Assert.AreEqual("Untitled Foo", obj["title"].ToString());
    }

    [Test]
    public virtual void NamedAppliedToParameter() {
        var obj = GetObject<Named1>();
        var action = GetMember(obj, nameof(Named1.DoSomething));
        var param = action["parameters"]["param1"];
        Assert.AreEqual("Yop", param["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public virtual void NamedAppliedToProperty() {
        var obj = GetObject<Named1>();
        var prop = GetMember(obj, nameof(Named1.Prop1));
        Assert.AreEqual("Bar", prop["extensions"]["friendlyName"].ToString());
    }

    [Test]
    public virtual void NullDescribedAsAppliedToAction() {
        var obj = GetObject<Describedas2>();
        var action = GetMember(obj, nameof(Describedas2.DoSomething));
        Assert.AreEqual("", action["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescribedAsAppliedToObject() {
        var obj = GetObject<Describedas2>();
        Assert.AreEqual("", obj["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescribedAsAppliedToParameter() {
        var obj = GetObject<Describedas2>();
        var action = GetMember(obj, nameof(Describedas2.DoSomething));
        var param = action["parameters"]["param1"];
        Assert.AreEqual("", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescribedAsAppliedToProperty() {
        var obj = GetObject<Describedas2>();
        var prop = GetMember(obj, nameof(Describedas2.Prop1));
        Assert.AreEqual("", obj["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescriptionAppliedToAction() {
        var obj = GetObject<Description2>();
        var action = GetMember(obj, nameof(Description2.DoSomething));
        Assert.AreEqual("", action["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescriptionAppliedToObject() {
        var obj = GetObject<Description2>();
        Assert.AreEqual("", obj["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescriptionAppliedToParameter() {
        var obj = GetObject<Description2>();
        var action = GetMember(obj, nameof(Description2.DoSomething));
        var param = action["parameters"]["param1"];
        Assert.AreEqual("", param["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void NullDescriptionAppliedToProperty() {
        var obj = GetObject<Description2>();
        var prop = GetMember(obj, nameof(Description2.Prop1));
        Assert.AreEqual("", prop["extensions"]["description"].ToString());
    }

    [Test]
    public virtual void ObjectImmutableTransient() {
        var obj = GetTransientObject<Immutable2>();
        var prop0 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop0));
        Assert.AreEqual("Field disabled as object cannot be changed", prop0["disabledReason"].ToString());
        var prop1 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop1));
        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
        var prop2 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop2));
        Assert.AreEqual("Field disabled as object cannot be changed", prop2["disabledReason"].ToString());
        var prop3 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop3));
        Assert.AreEqual("Field not editable until the object is persistent", prop3["disabledReason"].ToString());
        var prop4 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop4));
        Assert.AreEqual("Field disabled as object cannot be changed", prop4["disabledReason"].ToString());
        var prop5 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop5));
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
        var prop6 = GetMember((JObject)obj["result"], nameof(Immutable2.Prop6));
        Assert.AreEqual("Field disabled as object cannot be changed", prop6["disabledReason"].ToString());
    }

    [Test]
    public virtual void ObjectImmutable() {
        var obj = GetObject<Immutable2>("2");
        var prop0 = GetMember(obj, nameof(Immutable2.Prop0));
        Assert.AreEqual("Field disabled as object cannot be changed", prop0["disabledReason"].ToString());
        var prop1 = GetMember(obj, nameof(Immutable2.Prop1));
        Assert.AreEqual("Field not editable", prop1["disabledReason"].ToString());
        var prop2 = GetMember(obj, nameof(Immutable2.Prop2));
        Assert.AreEqual("Field not editable now that object is persistent", prop2["disabledReason"].ToString());
        var prop3 = GetMember(obj, nameof(Immutable2.Prop3));
        Assert.AreEqual("Field disabled as object cannot be changed", prop3["disabledReason"].ToString());
        var prop4 = GetMember(obj, nameof(Immutable2.Prop4));
        Assert.AreEqual("Field disabled as object cannot be changed", prop4["disabledReason"].ToString());
        var prop5 = GetMember(obj, nameof(Immutable2.Prop5));
        Assert.AreEqual("Field not editable", prop5["disabledReason"].ToString());
        var prop6 = GetMember(obj, nameof(Immutable2.Prop6));
        Assert.AreEqual("Field disabled as object cannot be changed", prop6["disabledReason"].ToString());
    }

    [Test]
    public virtual void ObjectWithTitleAttributeOnString() {
        var obj = GetObject<Title1>();
        Assert.AreEqual("Foo", obj["title"].ToString());
    }

    [Test]
    public void PropertyOrder() {
        var obj = GetObject<Memberorder1>();
        var properties = GetProperties(obj);
        AssertOrderIs(properties, nameof(Memberorder1.Prop2), nameof(Memberorder1.Prop1));
        AssertMemberOrderExtensionIs(properties, 1, 3);
    }

    [Test]
    public void PropertyOrderOnSubClass() {
        var obj = GetObject<Memberorder2>("2");
        var properties = GetProperties(obj);
        AssertOrderIs(properties, nameof(Memberorder2.Prop2), nameof(Memberorder2.Prop4), nameof(Memberorder2.Prop1), nameof(Memberorder2.Prop3));
        AssertMemberOrderExtensionIs(properties, 1, 2, 3, 4);
    }

    [Test]
    public virtual void RangeOnDateParms1() {
        var obj = GetObject<Range1>();
        var act = GetMember(obj, nameof(Range1.Action24));
        var range = act["parameters"]["parm"]["extensions"]["x-ro-nof-range"];
        var min = range["min"];
        var max = range["max"];

        Assert.AreEqual(todayMinus30, min.ToString());
        Assert.AreEqual(today, max.ToString());
    }

    [Test]
    public virtual void RangeOnDateParms2() {
        var obj = GetObject<Range1>();
        var act = GetMember(obj, nameof(Range1.Action25));
        var range = act["parameters"]["parm"]["extensions"]["x-ro-nof-range"];
        var min = range["min"];
        var max = range["max"];

        Assert.AreEqual(todayPlus1, min.ToString());
        Assert.AreEqual(todayPlus30, max.ToString());
    }

    //[Test]
    //public virtual void RangeOnDateProperty1() {
    //    var obj = GetObject<Range1>();
    //    var prop = GetMember(obj, nameof(Range1.Prop25));
    //    var range = prop["extensions"]["x-ro-nof-range"];
    //    var min = range["min"];
    //    var max = range["max"];

    //    Assert.AreEqual(todayMinus30, min.ToString());
    //    Assert.AreEqual(today, max.ToString());
    //}

    //[Test]
    //public virtual void RangeOnDateProperty2() {
    //    var obj = GetObject<Range1>();

    //    var prop = GetMember(obj, nameof(Range1.Prop26));
    //    var range = prop["extensions"]["x-ro-nof-range"];
    //    var min = range["min"];
    //    var max = range["max"];
    //    Assert.AreEqual(todayPlus1, min.ToString());
    //    Assert.AreEqual(todayPlus30, max.ToString());
    //}

    [Test]
    public virtual void RangeOnNumericParms() {
        var obj = GetObject<Range1>();
        NumericParmRangeTest(obj, nameof(Range1.Action1));
        NumericParmRangeTest(obj, nameof(Range1.Action2));
        NumericParmRangeTest(obj, nameof(Range1.Action3));
        NumericParmRangeTest(obj, nameof(Range1.Action4));
        NumericParmRangeTest(obj, nameof(Range1.Action5));
        NumericParmRangeTest(obj, nameof(Range1.Action6));
        NumericParmRangeTest(obj, nameof(Range1.Action7));
        NumericParmRangeTest(obj, nameof(Range1.Action8));
        NumericParmRangeTest(obj, nameof(Range1.Action9));
        NumericParmRangeTest(obj, nameof(Range1.Action10));
        NumericParmRangeTest(obj, nameof(Range1.Action11));
        NumericParmRangeTest(obj, nameof(Range1.Action12));
        NumericParmRangeTest(obj, nameof(Range1.Action13));
        NumericParmRangeTest(obj, nameof(Range1.Action14));
        NumericParmRangeTest(obj, nameof(Range1.Action15));
        NumericParmRangeTest(obj, nameof(Range1.Action16));
        NumericParmRangeTest(obj, nameof(Range1.Action17));
        NumericParmRangeTest(obj, nameof(Range1.Action18));
        NumericParmRangeTest(obj, nameof(Range1.Action19));
        NumericParmRangeTest(obj, nameof(Range1.Action20));
        NumericParmRangeTest(obj, nameof(Range1.Action21));
        NumericParmRangeTest(obj, nameof(Range1.Action22));
    }

    [Test]
    public virtual void RangeOnNumericProperties() {
        var obj = GetObject<Range1>();
        NumericPropertyRangeTest(obj, nameof(Range1.Prop3));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop4));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop5));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop6));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop7));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop8));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop9));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop10));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop11));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop12));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop14));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop15));
        NumericPropertyRangeTest(obj, nameof(Range1.Prop16));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop17));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop18));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop19));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop20));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop21));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop22));
        //NumericPropertyRangeTest(obj, nameof(Range1.Prop23));
    }

    [Test]
    public virtual void SimpleRegExAttributeOnProperty() {
        var obj = GetObject<Regex1>();
        var email = GetMember(obj, nameof(Regex1.Email));

        Assert.AreEqual(@"^[\-\w\.]+@[\-\w\.]+\.[A-Za-z]+$", email["extensions"]["pattern"].Value<string>());
    }

    [Test]
    public virtual void StringLengthOnParm() {
        var obj = GetObject<Stringlength1>();
        var act = GetMember(obj, nameof(Stringlength1.Action));

        Assert.AreEqual(8, act["parameters"]["parm"]["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void StringLengthOnProperty() {
        var obj = GetObject<Stringlength1>();
        var prop2 = GetMember(obj, nameof(Stringlength1.Prop2));

        Assert.AreEqual(7, prop2["extensions"]["maxLength"].Value<int>());
    }

    [Test]
    public virtual void TestObjectImmutableOncePersistedBefore() {
        var obj = GetTransientObject<Immutable3>();
        var prop0 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop0));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop1 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop1));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop2 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop2));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop3 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop3));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop4 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop4));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop5 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop5));
        Assert.AreEqual(null, prop0["disabledReason"]);
        var prop6 = GetMember((JObject)obj["result"], nameof(Immutable3.Prop6));
        Assert.AreEqual(null, prop0["disabledReason"]);
    }

    [Test]
    public virtual void TestObjectImmutableOncePersistedAfter() {
        var obj = GetObject<Immutable3>("3");
        var prop0 = GetMember(obj, nameof(Immutable3.Prop0));
        Assert.AreEqual("Field disabled as object cannot be changed", prop0["disabledReason"].Value<string>());
        var prop1 = GetMember(obj, nameof(Immutable3.Prop1));
        Assert.AreEqual("Field not editable", prop1["disabledReason"].Value<string>());
        var prop2 = GetMember(obj, nameof(Immutable3.Prop2));
        Assert.AreEqual("Field not editable now that object is persistent", prop2["disabledReason"].Value<string>());
        var prop3 = GetMember(obj, nameof(Immutable3.Prop3));
        Assert.AreEqual("Field disabled as object cannot be changed", prop3["disabledReason"].Value<string>());
        var prop4 = GetMember(obj, nameof(Immutable3.Prop4));
        Assert.AreEqual("Field disabled as object cannot be changed", prop4["disabledReason"].Value<string>());
        var prop5 = GetMember(obj, nameof(Immutable3.Prop5));
        Assert.AreEqual("Field not editable", prop5["disabledReason"].Value<string>());
        var prop6 = GetMember(obj, nameof(Immutable3.Prop6));
        Assert.AreEqual("Field disabled as object cannot be changed", prop6["disabledReason"].Value<string>());
    }

    [Test] //Error caused by change to TitleFacetViaProperty in f86f40ac on 08/10/2014
    public virtual void TitleAttributeOnReferencePropertyThatHasATitleAttribute1() {
        var obj1 = GetObject<Title1>();
        Assert.AreEqual("Foo", obj1["title"].Value<string>());
    }

    [Test] //Error caused by change to TitleFacetViaProperty in f86f40ac on 08/10/2014
    public virtual void TitleAttributeOnReferencePropertyThatHasATitleAttribute2() {
        var obj8 = GetObject<Title8>();
        Assert.AreEqual("Foo", obj8["title"].Value<string>());
    }

    [Test] //Error caused by change to TitleFacetViaProperty in f86f40ac on 08/10/2014
    public virtual void TitleAttributeOnReferencePropertyThatHasATitleMethod1() {
        var obj4 = GetObject<Title4>();
        Assert.AreEqual("Bar", obj4["title"].Value<string>());
    }

    [Test] //Error caused by change to TitleFacetViaProperty in f86f40ac on 08/10/2014
    public virtual void TitleAttributeOnReferencePropertyThatHasATitleMethod2() {
        var obj7 = GetObject<Title7>();
        Assert.AreEqual("Bar", obj7["title"].Value<string>());
    }

    [Test]
    public virtual void TitleAttributeOnReferencePropertyThatHasAToString1() {
        var obj2 = GetObject<Title2>();
        Assert.AreEqual("Baz", obj2["title"].Value<string>());
    }

    [Test]
    public virtual void TitleAttributeOnReferencePropertyThatHasAToString2() {
        var obj9 = GetObject<Title9>();
        Assert.AreEqual("Baz", obj9["title"].Value<string>());
    }

    [Test]
    public virtual void TitleAttributeTakesPrecedenceOverTitleMethod() {
        var obj = GetObject<Title6>();
        Assert.AreEqual("Foo", obj["title"].Value<string>());
    }

    [Test]
    public virtual void TitleAttributeTakesPrecedenceOverToString() {
        var obj = GetObject<Title3>();
        Assert.AreEqual("Qux", obj["title"].Value<string>());
    }

    [Test]
    public virtual async Task ValidateObjectChange() {
        var vpu1 = GetObject(FullName<Validateprogrammaticupdates1>(), "1");

        try {
            var prop = await vpu1.GetProperty(nameof(Validateprogrammaticupdates1.Prop1)).GetDetails();
            var val = await prop.SetValue("fail");
            Assert.Fail();
        }
        catch (HttpInvalidArgumentsRosiException e) {
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, e.StatusCode);
            Assert.AreEqual("fail", e.Content.GetArgument().GetInvalidReason());
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [Test]
    public virtual async Task ValidateObjectSave() {
        var service = GetService(FullName<TestServiceValidateProgrammaticUpdates>());
        var vpu1 = GetObject(FullName<Validateprogrammaticupdates1>(), "1");

        try {
            var result = await service.GetAction(nameof(TestServiceValidateProgrammaticUpdates.SaveObject1)).Invoke(vpu1, "fail");
            Assert.Fail();
        }
        catch (HttpErrorRosiException e) {
            Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
            Assert.AreEqual(@"199 RestfulObjects ""Validateprogrammaticupdates1/Untitled Validateprogrammaticupdates1 not in a valid state to be persisted - Validateprogrammaticupdates1.Prop1 is invalid: fail""", e.Message);
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [Test]
    public virtual async Task ValidateObjectCrossValidationPersist() {
        var service = GetService(FullName<TestServiceValidateProgrammaticUpdates>());
        var result = await service.GetAction(nameof(TestServiceValidateProgrammaticUpdates.GetObject2)).Invoke();
        var vpu2 = result.GetObject();

        try {
            await vpu2.PersistWithNamedParams(new Dictionary<string, object> { { "Id", 0 }, { "Prop1", "fail" }, { "Prop2", "" } });

            Assert.Fail();
        }
        catch (HttpInvalidArgumentsRosiException e) {
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, e.StatusCode);
            Assert.AreEqual("fail", e.Content.GetInvalidReason());
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [Test]
    public virtual async Task ValidateObjectCrossValidationSave() {
        var service = GetService(FullName<TestServiceValidateProgrammaticUpdates>());
        var vpu2 = GetObject(FullName<Validateprogrammaticupdates2>(), "1");

        try {
            var result = await service.GetAction(nameof(TestServiceValidateProgrammaticUpdates.SaveObject2)).Invoke(vpu2, "fail");

            Assert.Fail();
        }
        catch (HttpErrorRosiException e) {
            Assert.AreEqual(HttpStatusCode.InternalServerError, e.StatusCode);
            Assert.AreEqual(@"199 RestfulObjects ""Validateprogrammaticupdates2/Untitled Validateprogrammaticupdates2 not in a valid state to be persisted - fail""", e.Message);
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [Test]
    public virtual async Task MultilineMandatoryParameter() {
        var obj = GetObject(FullName<Multiline1>(), "1");
        var result = await obj.GetAction(nameof(Multiline1.Action)).Invoke("fred");

        Assert.AreEqual(ActionResultApi.ResultType.Void, result.GetResultType());
    }

    [Test]
    public virtual async Task MultilineMandatoryParameterEmpty() {
        try {
            var obj = GetObject(FullName<Multiline1>(), "1");
            var result = await obj.GetAction(nameof(Multiline1.Action)).Invoke("");
            Assert.Fail("expect exception");
        }
        catch (HttpInvalidArgumentsRosiException e) {
            Assert.AreEqual("Mandatory", e.Content.GetArguments()["parm"].GetInvalidReason());
        }
    }

    [Test]
    public virtual async Task MultilineOptionalParameter() {
        var obj = GetObject(FullName<Multiline1>(), "1");
        var result = await obj.GetAction(nameof(Multiline1.Action1)).Invoke("fred");

        Assert.AreEqual(ActionResultApi.ResultType.Void, result.GetResultType());
    }

    [Test]
    public virtual async Task MultilineOptionalParameterEmpty() {
        var obj = GetObject(FullName<Multiline1>(), "1");
        var result = await obj.GetAction(nameof(Multiline1.Action1)).Invoke("");

        Assert.AreEqual(ActionResultApi.ResultType.Void, result.GetResultType());
    }
}