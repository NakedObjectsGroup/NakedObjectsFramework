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
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestTestAction : AcceptanceTestCase {
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
    public virtual void TestName() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        Assert.AreEqual("Do Something", act.Name);
    }

    [Test]
    public virtual void TestSubMenu() {
        var obj = NewTestObject<Object1>();
        var act1 = obj.GetAction("Do Something");
        var act2 = obj.GetAction("Do Something On Menu", "Sub1");

        Assert.AreEqual("", act1.SubMenu);
        Assert.AreEqual("Sub1", act2.SubMenu);
    }

    [Test]
    public virtual void TestLastMessage() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");

        act.AssertIsDisabled();
        Assert.AreEqual("Always disabled", act.LastMessage);
    }

    [Test]
    public virtual void TestParameters() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");
        var parameters = act.Parameters;

        Assert.AreEqual(2, parameters.Length);
        Assert.AreEqual("Param0", parameters[0].Name);
        Assert.AreEqual("Param1", parameters[1].Name);
    }

    [Test]
    public virtual void TestMatchValueParameters() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        Assert.IsTrue(act.MatchParameters(new[] { typeof(int), typeof(string) }));
        Assert.IsFalse(act.MatchParameters(new[] { typeof(string), typeof(int) }));
    }

    [Test]
    public virtual void TestMatchRefParameters() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("With Ref Param");

        Assert.IsTrue(act.MatchParameters(new[] { typeof(Object1) }));
        Assert.IsFalse(act.MatchParameters(new[] { typeof(Object2) }));
    }

    [Test]
    public virtual void TestInvokeReturnNullObject() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        var result = act.InvokeReturnObject(0, "value");

        Assert.IsNull(result);
    }

    [Test]
    public virtual void TestInvoke() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Return Void");

        act.Invoke();
    }

    [Test]
    public virtual void TestInvokeReturnObject() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        var result = act.InvokeReturnObject(1, "value");

        Assert.IsNotNull(result);

        Assert.AreEqual("FooBar", result.Title);
    }

    [Test]
    public virtual void TestInvokeReturnCollection() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Return Collection");

        var result = act.InvokeReturnCollection(1, "value");

        Assert.IsNotNull(result);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Untitled Object2", result.First().Title);
    }

    [Test]
    public virtual void TestInvokeReturnPagedCollection() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Return Collection");

        var result = act.InvokeReturnPagedCollection(1, 1, "value");

        Assert.IsNotNull(result);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Untitled Object2", result.First().Title);
    }

    [Test]
    public virtual void TestAssertDisabledAction() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");
        act.AssertIsDisabled();
        act.AssertLastMessageIs("Always disabled");
        AssertExpectException(() => act.AssertIsEnabled(), "Assert.IsTrue failed. Action 'Always Disabled' is disabled: Always disabled");
        act.AssertLastMessageIs("Always disabled");
    }

    [Test]
    public virtual void TestAssertEnabledAction() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");
        act.AssertIsEnabled();
        AssertExpectException(() => act.AssertIsDisabled(), "Assert.IsFalse failed. Action 'Do Something' is usable: ");
    }

    [Test]
    public virtual void TestAssertIsValidWithParms() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        act.AssertIsValidWithParms(1, "something");

        AssertExpectException(() => act.AssertIsValidWithParms("something", 1), "Assert.Fail failed. Invalid Argument(s) expected: System.Int32 actual: System.String");
    }

    [Test]
    public virtual void TestAssertIsInvalidWithParms() {
        var obj = NewTestObject<Object1>();
        var act = obj.GetAction("Do Something");

        act.AssertIsInvalidWithParms("something", 1);

        AssertExpectException(() => act.AssertIsInvalidWithParms(1, "something"), "Assert.IsFalse failed. Action 'Do Something' is usable and executable");
    }

    [Test]
    public virtual void TestAssertIsVisible() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");
        act.AssertIsVisible();
        act.AssertLastMessageIs("");
        AssertExpectException(() => act.AssertIsInvisible(), "Assert.IsTrue failed. Action 'Always Disabled' is visible");
        act.AssertLastMessageIs("");
    }

    [Test]
    public virtual void TestAssertIsDescribedAsNone() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");
        act.AssertIsDescribedAs("");
        AssertExpectException(() => act.AssertIsDescribedAs("a description"), "Assert.IsTrue failed. Description expected: 'a description' actual: ''");
    }

    [Test]
    public virtual void TestAssertIsDescribedAs() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Return Void");
        act.AssertIsDescribedAs("Does nothing");
        AssertExpectException(() => act.AssertIsDescribedAs("Does something"), "Assert.IsTrue failed. Description expected: 'Does something' actual: 'Does nothing'");
    }

    [Test]
    public virtual void TestAssertLastMessageIs() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");
        act.AssertIsDisabled();
        act.AssertLastMessageIs("Always disabled");
        AssertExpectException(() => act.AssertLastMessageIs("Always visible"), "Assert.IsTrue failed. Last message expected: 'Always visible' actual: 'Always disabled'");
    }

    [Test]
    public virtual void TestAssertLastMessageContains() {
        var obj = NewTestObject<Object2>();
        var act = obj.GetAction("Always Disabled");
        act.AssertIsDisabled();
        act.AssertLastMessageContains("disabled");
        AssertExpectException(() => act.AssertLastMessageContains("visible"), "Assert.IsTrue failed. Last message expected to contain: 'visible' actual: 'Always disabled'");
    }
}