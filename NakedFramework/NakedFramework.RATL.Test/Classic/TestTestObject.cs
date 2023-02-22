using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Extensions;
using NakedFramework.Persistor.EFCore.Util;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using ROSI.Test.Data;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestTestObject : AcceptanceTestCase {
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

    private static void AssertExpectException(Action f, string msg) {
        try {
            f();
            Assert.Fail("Expect exception");
        }
        catch (Exception ex) {
            Assert.IsInstanceOfType(ex, typeof(AssertFailedException));
            Assert.AreEqual(msg, ex.Message);
        }
    }


    [Test]
    public virtual void TestTitle() {
        var obj = NewTestObject<Object1>();

        Assert.AreEqual("FooBar", obj.Title);
    }

    [Test]
    public virtual void TestProperties() {
        var obj = NewTestObject<Object1>();
        var properties = obj.Properties;

        Assert.AreEqual(4, properties.Length);

        Assert.AreEqual("Id", properties[0].Name);
        Assert.AreEqual("Prop1", properties[1].Name);
        Assert.AreEqual("Foo", properties[2].Name);
        Assert.AreEqual("Prop3", properties[3].Name);
    }

   
    [Test]
    public virtual void TestAssertTitleEquals() {
        var obj = NewTestObject<Object1>();
        obj.AssertTitleEquals("FooBar");
        AssertExpectException(() => obj.AssertTitleEquals("Qux"), "Assert.IsTrue failed. Expected title 'Qux' but got 'FooBar'");
    }

    [Test]
    public virtual void TestSave() {
        var transient = GetTestService(FullName<Service1>()).GetAction("Get Transient").InvokeReturnObject();

        transient.GetPropertyByName("Id").SetValue("0");
        transient.GetPropertyByName("Prop1").SetValue("1");
        transient.GetPropertyByName("Foo").SetValue("test");
        transient.GetPropertyByName("Prop3").SetValue(DateTime.Now.ToString());
        var obj = transient.Save();

        Assert.AreEqual("test", obj.GetPropertyById(nameof(Object1.Prop2)).Title);
    }
}