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
public class TestFailureScenarios : AcceptanceTestCase {
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
            GetTestService("AwolService");
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            Assert.AreEqual("Assert.Fail failed. No such service: AwolService", e.Message);
        }
    }

    [TestMethod]
    public virtual void InvokeActionWithIncorrectParams() {
        ITestObject obj1 = NewTestObject<Object1>();
        try {
            obj1.GetAction("Do Something").InvokeReturnObject(1, 2);
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.IsInstanceOfType(e, typeof(AssertFailedException));
            Assert.AreEqual("Assert.Fail failed. Invalid Argument(s)", e.Message);
        }
    }

    [TestMethod]
    public virtual void IncorrectTitle() {
        ITestObject obj1 = NewTestObject<Object1>();

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

    [TestMethod]
    public virtual void TestEnumDefault() {
        ITestObject obj = NewTestObject<Object1>();

        ITestAction a1 = obj.GetAction("Do Something Else");

        ITestParameter p1 = a1.Parameters.First();
        ITestParameter p2 = a1.Parameters.Last();

        ITestNaked def1 = p1.GetDefault();
        ITestNaked def2 = p2.GetDefault();

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

    [TestMethod]
    public virtual void TestTooFewParms() {
        ITestObject obj = NewTestObject<Object1>();

        ITestAction a1 = obj.GetAction("Do Something");

        try {
            ITestObject res = a1.InvokeReturnObject();
            Assert.Fail("expect exception");
        }
        catch (Exception expected) {
            Assert.AreEqual("Assert.IsTrue failed. Action 'Do Something' is unusable: wrong number of parameters, got 0, expect 2", expected.Message);
        }
    }

    [TestMethod]
    public virtual void TestPropertyValue() {
        ITestObject obj = NewTestObject<Object1>();

        ITestProperty p1 = obj.GetPropertyById(nameof(Object1.Prop3));

        p1.AssertValueIsEqual("16/08/2013 00:00:00");
        p1.AssertTitleIsEqual("16/08/2013");
    }
}