using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Rest;
using NakedObjects.Rest.Snapshot.Utility;
using NakedObjects.Xat;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test
{

    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade ff, ILogger<RestfulObjectsControllerBase> l, ILoggerFactory lf) : base(ff, l, lf) { }
    }

    public static class Helpers {
        public static DefaultHttpContext CreateTestHttpContext(IServiceProvider sp) {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = sp;
            httpContext.Response.Body = new MemoryStream();
            return httpContext;
        }

        public static RestfulObjectsControllerBase SetMockContext(RestfulObjectsControllerBase api, IServiceProvider sp) {
            var mockContext = new ControllerContext();
            var httpContext = CreateTestHttpContext(sp);
            mockContext.HttpContext = httpContext;
            api.ControllerContext = mockContext;
            return api;
        }

        public class MenuTest : AcceptanceTestCase {
            protected override Type[] Types { get; } = new Type[] { };

            protected override Type[] Services { get; } = new Type[] { };

            protected override string[] Namespaces { get; } = new string[] { "NakedFunctions.Rest.Test.Data" };

            protected override IMenu[] MainMenus(IMenuFactory factory) {
                return new IMenu[] {
                    factory.NewMenu(typeof(SimpleMenuFunction), true, "Test menu")
                };
            }

            private Type[] RecordTypes = new Type[]{typeof(SimpleRecord)};

            private Type[] FunctionTypes = new Type[] { typeof(SimpleMenuFunction) };

            private IFunctionalReflectorConfiguration FunctionalReflectorConfiguration() {
                return new FunctionalReflectorConfiguration(RecordTypes, FunctionTypes);
            }

            protected override void RegisterTypes(IServiceCollection services) {
                
                base.RegisterTypes(services);
                services.AddScoped<IOidStrategy, EntityOidStrategy>();
                services.AddScoped<IStringHasher, NullStringHasher>();
                services.AddScoped<IFrameworkFacade, FrameworkFacade>();
                services.AddScoped<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
                services.AddTransient<RestfulObjectsController, RestfulObjectsController>();

                services.AddSingleton<IFunctionalReflectorConfiguration>(FunctionalReflectorConfiguration());

                //services.AddMvc(fun(options)->options.EnableEndpointRouting < -false)
                //        .AddNewtonsoftJson(fun(options)->options.SerializerSettings.DateTimeZoneHandling < -DateTimeZoneHandling.Utc)
                //        |> ignore
                //()
            }

            protected override EntityObjectStoreConfiguration Persistor {
                get {
                    var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
                    config.UsingContext(Activator.CreateInstance<TestDbContext>);
                    return config;
                }
            }



            [SetUp]
            public void SetUp() => StartTest();

            [TearDown]
            public void TearDown() => EndTest();

            [OneTimeSetUp]
            public void FixtureSetUp() {
                ReflectorConfiguration.NoValidate = true;
                TestDbContext.Delete();
                var context = Activator.CreateInstance<TestDbContext>();

                context.Database.Create();
                InitializeNakedObjectsFramework(this);
            }

            [OneTimeTearDown]
            public void FixtureTearDown() {
                CleanupNakedObjectsFramework(this);
                TestDbContext.Delete();
            }

            protected RestfulObjectsControllerBase Api() {
                var sp = GetConfiguredContainer();
                var api = sp.GetService<RestfulObjectsController>();
                return SetMockContext(api, sp);
            }

            [Test]
            public void TestGetMenu() {
                var result = Api().GetMenus();
            }
        }
    }
}