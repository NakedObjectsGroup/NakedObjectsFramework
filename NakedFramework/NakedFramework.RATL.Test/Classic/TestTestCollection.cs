using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using ROSI.Test.Data;
using static NakedFramework.RATL.Test.Classic.TestHelpers;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestTestCollection : AcceptanceTestCase {
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

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
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
                    typeof(Object2),
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

    [Test]
    public virtual void TestAssertIsEmpty() {
        var obj = NewTestObject<Object2>();
        var coll = obj.GetAction("Return Collection").InvokeReturnCollection(0, "something");

        coll.AssertIsEmpty();
        AssertExpectException(() => coll.AssertIsNotEmpty(), "Assert.IsTrue failed. Collection is empty");
    }

    [Test]
    public virtual void TestAssertIsNotEmpty() {
        var obj = NewTestObject<Object2>();
        var coll = obj.GetAction("Return Collection").InvokeReturnCollection(1, "something");

        coll.AssertIsNotEmpty();
        AssertExpectException(() => coll.AssertIsEmpty(), "Assert.AreEqual failed. Expected:<0>. Actual:<1>. Collection is not empty");
    }

    [Test]
    public virtual void TestAssertCountIs() {
        var obj = NewTestObject<Object2>();
        var coll = obj.GetAction("Return Collection").InvokeReturnCollection(1, "something");

        coll.AssertCountIs(1);
        AssertExpectException(() => coll.AssertCountIs(2), "Assert.IsTrue failed. Collection Size is: 1 expected: 2");
    }
}