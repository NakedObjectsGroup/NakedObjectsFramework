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
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestTestHasActions : AcceptanceTestCase {
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
    public virtual void TestGetAction() {
        var obj = NewTestObject<Object1>();
        Assert.AreEqual("Do Return String", obj.GetAction("Do Return String").Name);

        AssertExpectException(() => obj.GetAction("No Such Action"), "Assert.Fail failed. No Action named 'No Such Action'");
    }

    [Test]
    public virtual void TestGetActionWithTypes() {
        var obj = NewTestObject<Object1>();
        Assert.AreEqual("Do Something", obj.GetAction("Do Something", typeof(int), typeof(string)).Name);

        AssertExpectException(() => obj.GetAction("Do Something", typeof(string), typeof(int)), "Assert.Fail failed. No Action named 'Do Something' (with specified parameters)");
        AssertExpectException(() => obj.GetAction("Do Something", Array.Empty<Type>()), "Assert.Fail failed. No Action named 'Do Something' (with specified parameters)");
    }

    [Test]
    public virtual void TestGetActionOnMenu() {
        var obj = NewTestObject<Object1>();
        Assert.AreEqual("Do Something On Menu", obj.GetAction("Do Something On Menu", "Sub1").Name);

        AssertExpectException(() => obj.GetAction("Do Something On Menu", "Wrong Name"), "Assert.Fail failed. No Action named 'Do Something On Menu' within sub-menu 'Wrong Name'");
        AssertExpectException(() => obj.GetAction("Do Something On Menu"), "Assert.Fail failed. No Action named 'Do Something On Menu'");
    }

    [Test]
    public virtual void TestGetActionOnMenuWithTypes() {
        var obj = NewTestObject<Object1>();
        Assert.AreEqual("Do Something On Menu", obj.GetAction("Do Something On Menu", "Sub1", typeof(int), typeof(string)).Name);

        AssertExpectException(() => obj.GetAction("Do Something On Menu", "Wrong Name", typeof(int), typeof(string)), "Assert.Fail failed. No Action named 'Do Something On Menu' (with specified parameters) within sub-menu 'Wrong Name'");
        AssertExpectException(() => obj.GetAction("Do Something On Menu", "Wrong Name", typeof(string), typeof(int)), "Assert.Fail failed. No Action named 'Do Something On Menu' (with specified parameters) within sub-menu 'Wrong Name'");
        AssertExpectException(() => obj.GetAction("Do Something On Menu", "Wrong Name", Array.Empty<Type>()), "Assert.Fail failed. No Action named 'Do Something On Menu' (with specified parameters) within sub-menu 'Wrong Name'");
    }

    [Test]
    public virtual void TestGetActionOrder() {
        var obj = NewTestObject<Object1>();
        var order = obj.GetObjectActionOrder();
        Assert.AreEqual("Do Return String, Do Something, Do Something Else, (Sub1:Do Something On Menu, (SubSub:Do Something On Sub Menu))", order);
    }

    [Test]
    public virtual void TestAssertActionOrderIs() {
        var obj = NewTestObject<Object1>();
        obj.AssertActionOrderIs("Do Return String, Do Something, Do Something Else, (Sub1:Do Something On Menu, (SubSub:Do Something On Sub Menu))");
        AssertExpectException(() => obj.AssertActionOrderIs("Do Return String"), "Assert.AreEqual failed. Expected:<Do Return String>. Actual:<Do Return String, Do Something, Do Something Else, (Sub1:Do Something On Menu, (SubSub:Do Something On Sub Menu))>. ");
    }
}