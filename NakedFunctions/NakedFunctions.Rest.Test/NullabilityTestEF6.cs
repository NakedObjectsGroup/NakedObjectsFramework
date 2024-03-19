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
using NakedFramework.Rest.API;
using NakedFramework.Test;
using NakedFramework.Test.TestCase;
using NakedFunctions.Reflector.Extensions;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Reflector.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NakedFunctions.Rest.Test;

public class NullabilityTestEF6 : AcceptanceTestCase {
    protected override Type[] Functions { get; } = {
        typeof(NullableParameterFunctions)
    };

    protected override Type[] Records { get; } = {
        typeof(UrlLinkRecord)
    };

    protected override Type[] ObjectTypes { get; } = { };

    protected override Type[] Services { get; } = { };

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new NullabilityDbContext() };

    protected override Action<NakedFrameworkOptions> AddNakedObjects => _ => { };

    protected override Action<NakedFunctionsOptions> NakedFunctionsOptions =>
        options => {
            options.DomainTypes = Records;
            options.DomainFunctions = Functions;
            options.DomainServices = Services;
            options.UseNullableReferenceTypesForOptionality = true;
        };

    protected virtual void CleanUpDatabase() {
        NullabilityDbContext.Delete();
    }

    protected virtual void CreateDatabase() { }

    protected override void RegisterTypes(IServiceCollection services) {
        base.RegisterTypes(services);
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }

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
    public void TestGetFunctionWithNullability() {
        var api = Api();
        var result = api.GetObject(FullName<UrlLinkRecord>(), "1");
        var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, sc);
        var parsedResult = JObject.Parse(json);

        ClassicAssert.AreEqual("True", parsedResult["members"]["NullableFunction"]["parameters"]["p1"]["extensions"]["optional"].ToString());
        ClassicAssert.AreEqual("False", parsedResult["members"]["NullableFunction"]["parameters"]["p2"]["extensions"]["optional"].ToString());
    }
}