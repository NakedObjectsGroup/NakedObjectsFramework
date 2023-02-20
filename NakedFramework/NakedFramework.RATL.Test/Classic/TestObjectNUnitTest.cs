using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.RATL.Classic.Interface;
using NakedFramework.RATL.Classic.NonDocumenting;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using ROSI.Apis;
using ROSI.Test.Data;
using Assert = NUnit.Framework.Assert;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestObjectNUnitTest : AcceptanceNUnitTestCase {
    protected override void ConfigureServices(IServiceCollection services) {
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

    protected void CleanUpDatabase() {
        new EFCoreRATLTestDbContext().Delete();
    }

    [SetUp]
    public void SetUp() {
        StartTest();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
        new EFCoreRATLTestDbContext().Create();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CleanUpDatabase();
    }


    //[Test]
    //public virtual void IncorrectTitle() {
    //    var obj1 = NewTestObject<Object1>();

    //    try {
    //        obj1.AssertTitleEquals("Yoda");
    //        NUnit.Framework.Assert.Fail("Should not get to here");
    //    }
    //    catch (Exception e) {
    //       // Assert.IsInstanceOfType(e, typeof(AssertFailedException));
    //        Assert.AreEqual("Assert.IsTrue failed. Expected title 'Yoda' but got 'FooBar'", e.Message);
    //    }
    //}
}