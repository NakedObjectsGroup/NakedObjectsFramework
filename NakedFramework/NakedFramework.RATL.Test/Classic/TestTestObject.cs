using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using ROSI.Test.Data;

namespace NakedFramework.RATL.Test.Classic;

[TestClass]
public class TestTestObject : AcceptanceTestCase {
    private static void ConfigureServices(IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.MainMenus = f => new[] { f.NewMenu<Service1>(true) };
            frameworkOptions.AddEFCorePersistor();
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = new[] {
                    typeof(Object1),
                    typeof(TestEnum1)
                };
                appOptions.DomainModelServices = new[] { typeof(Service1) };
            });
        });
        services.AddDbContext<DbContext, EFCoreRATLTestDbContext>();
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
    }

    private static void CleanUpDatabase() {
        new EFCoreRATLTestDbContext().Delete();
    }

    [TestInitialize]
    public void SetUp() {
        StartTest();
    }

    [TestCleanup]
    public void TearDown() => EndTest();

    [ClassInitialize]
    public static void FixtureSetUp(TestContext tc) {
        InitializeNakedObjectsFramework(ConfigureServices);
        new EFCoreRATLTestDbContext().Create();
    }

    [ClassCleanup]
    public static void FixtureTearDown() {
        CleanupNakedObjectsFramework();
        CleanUpDatabase();
    }

    [TestMethod]
    public virtual void AttemptToGetANonExistentService() {
        try {
            ITestObject obj = NewTestObject<Object1>();
           


        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            Assert.AreEqual("Assert.Fail failed. No such service: AwolService", e.Message);
        }
    }

   
}