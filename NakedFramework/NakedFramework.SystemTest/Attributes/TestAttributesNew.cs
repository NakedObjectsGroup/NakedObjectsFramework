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
        AttributesDbContext.Delete();
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

    protected RestfulObjectsControllerBase Api() {
        var sp = GetConfiguredContainer();
        var api = sp.GetService<RestfulObjectsController>();
        return Helpers.SetMockContext(api, sp);
    }

    private static string FullName<T>() => typeof(T).FullName;

    [Test]
    public void ActionOrder() {
        var api = Api();

        var result = api.GetObject(FullName<Memberorder1>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        Assert.AreEqual((int)HttpStatusCode.OK, sc);
        var obj2 = JObject.Parse(json);

        var members = obj2["members"];
        var inOrder = members.ToArray();
        Assert.AreEqual(((JProperty)inOrder[2]).Name, "Action2");
        Assert.AreEqual(members["Action2"]["extensions"]["memberOrder"].ToString(), "1");
        Assert.AreEqual(((JProperty)inOrder[3]).Name, "Action1");
        Assert.AreEqual(members["Action1"]["extensions"]["memberOrder"].ToString(), "3");
    }
}