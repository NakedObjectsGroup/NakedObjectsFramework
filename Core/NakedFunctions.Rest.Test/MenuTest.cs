using System;
using System.Net;
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
using NakedObjects.Xat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade ff, ILogger<RestfulObjectsControllerBase> l, ILoggerFactory lf) : base(ff, l, lf) { }
    }

    public class MenuTest : AcceptanceTestCase {
        private readonly Type[] FunctionTypes = {typeof(SimpleMenuFunction)};

        private readonly Type[] RecordTypes = {typeof(SimpleRecord)};
        protected override Type[] Types { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override string[] Namespaces { get; } = {"NakedFunctions.Rest.Test.Data"};

        protected override EntityObjectStoreConfiguration Persistor {
            get {
                var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
                config.UsingContext(Activator.CreateInstance<TestDbContext>);
                return config;
            }
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) {
            return new[] {
                factory.NewMenu(typeof(SimpleMenuFunction), true, "Test menu")
            };
        }

        private IFunctionalReflectorConfiguration FunctionalReflectorConfiguration() => new FunctionalReflectorConfiguration(RecordTypes, FunctionTypes);

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddScoped<IOidStrategy, EntityOidStrategy>();
            services.AddScoped<IStringHasher, NullStringHasher>();
            services.AddScoped<IFrameworkFacade, FrameworkFacade>();
            services.AddScoped<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddSingleton(FunctionalReflectorConfiguration());
            services.AddMvc((options) => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson((options) => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);

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
            return Helpers.SetMockContext(api, sp);
        }

        [Test]
        public void TestGetMenu() {
            var api = Api();
            var result = api.GetMenus();
            var (json, sc, headers) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            var parsedResult = JObject.Parse(json);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);

            var val = parsedResult.GetValue("value") as JArray;

            Assert.IsNotNull(val);
            Assert.AreEqual(1, val.Count);

            var firstItem = val.First;

            Assert.AreEqual("Test menu", firstItem["title"].ToString());
            Assert.AreEqual("urn:org.restfulobjects:rels/menu;menuId=\"SimpleMenuFunction\"", firstItem["rel"].ToString());
            Assert.AreEqual("GET", firstItem["method"].ToString());
            Assert.AreEqual("application/json; profile=\"urn:org.restfulobjects:repr-types/menu\"; charset=utf-8", firstItem["type"].ToString());
            Assert.AreEqual("http://localhost/menus/SimpleMenuFunction", firstItem["href"].ToString());
        }
    }
}