using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
public class TestFailureScenarios : AcceptanceTestCase {
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

    [Test]
    public virtual void InvokeActionWithIncorrectParams() {
        var obj1 = NewTestObject<Object1>();
        try {
            obj1.GetAction("Do Something").InvokeReturnObject(1, 2);
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            Assert.AreEqual("Assert.Fail failed. Invalid Argument(s)", e.Message);
        }
    }

    [Test]
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

    //[TestMethod]
    //public virtual void VisiblePropertyHasSameNameAsHiddenProperty() {
    //    ITestObject obj1 = NewTestObject<Object1>();
    //    try {
    //        ITestProperty foo = obj1.GetPropertyByName("Foo");
    //        Assert.Fail("Should not get to here");
    //    }
    //    catch (Exception e) {
    //        Assert.IsInstanceOfType(e, typeof(AssertFailedException));
    //        Assert.AreEqual("Assert.Fail failed. More than one Property named 'Foo'", e.Message);
    //    }
    //}

    [Test]
    public virtual void TestEnumDefault() {
        var obj = NewTestObject<Object1>();

        var a1 = obj.GetAction("Do Something Else");

        var p1 = a1.Parameters.First();
        var p2 = a1.Parameters.Last();

        var def1 = p1.GetDefault();
        var def2 = p2.GetDefault();

        Assert.AreEqual("1", def1.Title);
        Assert.AreEqual(null, def2);
    }

    //[TestMethod]
    //public virtual void TestReturnString() {
    //    ITestObject obj = NewTestObject<Object1>();

    //    ITestAction a1 = obj.GetAction("Do Return String");

    //    ITestObject res = a1.InvokeReturnObject();

    //   //Assert.AreEqual("a string", res.NakedObject.Object);

    //    a1.Invoke();
    //}

    [Test]
    public virtual void TestTooFewParms() {
        var obj = NewTestObject<Object1>();

        var a1 = obj.GetAction("Do Something");

        try {
            var res = a1.InvokeReturnObject();
            Assert.Fail("expect exception");
        }
        catch (Exception expected) {
            Assert.AreEqual("Assert.IsTrue failed. Action 'Do Something' is unusable: wrong number of parameters, got 0, expect 2", expected.Message);
        }
    }

    [Test]
    public virtual void TestPropertyValue() {
        var obj = NewTestObject<Object1>();

        var p1 = obj.GetPropertyById(nameof(Object1.Prop3));

        var dateTime = new DateTime(2013, 8, 16);
        p1.AssertValueIsEqual(dateTime.ToString());
        p1.AssertTitleIsEqual(dateTime.ToString("d"));
    }
}