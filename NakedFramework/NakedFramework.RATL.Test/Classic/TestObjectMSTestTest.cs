using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using ROSI.Test.Data;
using System.Security.Principal;

namespace NakedFramework.RATL.Test.Classic;

[TestClass]
public class TestObjectMSTestTest : AcceptanceMSTestTestCase {

    private new static IDictionary<string, string> Configuration() => new Dictionary<string, string>();


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
        InitializeNakedObjectsFramework(ConfigureServices, Configuration);
        new EFCoreRATLTestDbContext().Create();
    }

    [ClassCleanup]
    public static void FixtureTearDown() {
        CleanupNakedObjectsFramework();
        CleanUpDatabase();
    }

    [TestMethod]
    public virtual void IncorrectTitle() {
        var obj1 = NewTestObject<Object1>();

        try {
            obj1.AssertTitleEquals("Yoda");
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            Assert.AreEqual("Assert.IsTrue failed. Expected title 'Yoda' but got 'FooBar'", e.Message);
        }
    }
}