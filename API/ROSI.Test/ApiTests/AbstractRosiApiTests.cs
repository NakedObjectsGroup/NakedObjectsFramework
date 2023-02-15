// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Rattle.Helpers;
using NakedFramework.Rattle.TestCase;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Extensions;
using NakedFramework.Test.TestCase;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Records;
using ROSI.Test.Data;


namespace ROSI.Test.ApiTests;

public abstract class AbstractRosiApiTests : BaseRattleTestCase {
    protected override void ConfigureServices(IServiceCollection services) {
         
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.AddNakedFramework(frameworkOptions => {
                frameworkOptions.MainMenus = f =>  new[] { f.NewMenu<SimpleService>(true) };
                frameworkOptions.AddEFCorePersistor();
                frameworkOptions.AddRestfulObjects(restOptions => {  });
                frameworkOptions.AddNakedObjects(appOptions => {
                    appOptions.DomainModelTypes = new Type[] {
                        typeof(Class),
                        typeof(ClassWithActions),
                        typeof(TestChoices),
                        typeof(TestEnum),
                        typeof(ClassWithScalars),
                        typeof(ClassToPersist)
                    };
                    appOptions.DomainModelServices = new Type[] {typeof(SimpleService)};
                });
            });
            services.AddDbContext<DbContext, EFCoreObjectDbContext>();
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddScoped(p => TestPrincipal);
    }
    protected void CleanUpDatabase() {
        new EFCoreObjectDbContext().Delete();
    }

    [SetUp]
    public void SetUp() {
        StartTest();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        ObjectReflectorConfiguration.NoValidate = true;
        InitializeNakedObjectsFramework(this);
        new EFCoreObjectDbContext().Create();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CleanUpDatabase();
    }

    protected DomainObject GetObject(string type, string id) {
        return ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;
    }

    protected static string FullName<T>() => typeof(T).FullName;

    protected InvokeOptions TestInvokeOptions(string token = null, EntityTagHeaderValue tag = null) =>
        new() {
            Token = token,
            Tag = tag,
            HttpClient = new HttpClient(new StubHttpMessageHandler(Api()))
        };
}