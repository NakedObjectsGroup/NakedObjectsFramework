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

    //[Test]
    //public virtual void TestSave() {
    //    var transient = GetTestService(FullName<Service1>()).GetAction("Get Transient").InvokeReturnObject();

    //    transient.GetPropertyByName("Id").SetValue("0");
    //    transient.GetPropertyByName("Prop1").SetValue("1");
    //    transient.GetPropertyByName("Foo").SetValue("test");
    //    transient.GetPropertyByName("Prop3").SetValue(DateTime.Now.ToString());
    //    var obj = transient.Save();

    //    obj.AssertIsPersistent();
    //    Assert.AreEqual("test", obj.GetPropertyById(nameof(Object1.Prop2)).Title);
    //}

    [Test]
    public virtual void TestSaveFails() {
        var transient = GetTestService<Service1>().GetAction("Get Transient").InvokeReturnObject();
        AssertExpectException(() => transient.Save(), "Assert.Fail failed. Id:Mandatory");
    }

    [Test]
    public virtual void TestRefresh() {
        var obj = NewTestObject<Object1>();
        var newObj = obj.Refresh();

        Assert.AreEqual("FooBar", newObj.Title);
        Assert.AreNotEqual(obj, newObj);
    }

    [Test]
    public virtual void TestAssertIsType() {
        var obj = NewTestObject<Object1>();
        obj.AssertIsType(typeof(Object1));
        AssertExpectException(() => obj.AssertIsType(typeof(Service1)), "Assert.IsTrue failed. Expected type 'ROSI.Test.Data.Service1' but got 'ROSI.Test.Data.Object1'");
    }

    [Test]
    public virtual void TestGetPropertyByName() {
        var obj = NewTestObject<Object1>();
        obj.GetPropertyByName("Foo");
        AssertExpectException(() => obj.GetPropertyByName("Bar"), "Assert.Fail failed. No Property named 'Bar'");
    }

    [Test]
    public virtual void TestGetPropertyById() {
        var obj = NewTestObject<Object1>();
        obj.GetPropertyById("Prop1");
        AssertExpectException(() => obj.GetPropertyById("Foo"), "Assert.Fail failed. No Property with Id 'Foo'");
    }

    //[Test]
    //public virtual void TestAssertCanBeSaved() {
    //    var transient = GetTestService(FullName<Service1>()).GetAction("Get Transient").InvokeReturnObject();

    //    transient.GetPropertyByName("Id").SetValue("0");
    //    transient.GetPropertyByName("Prop1").SetValue("1");
    //    transient.GetPropertyByName("Foo").SetValue("test");
    //    transient.GetPropertyByName("Prop3").SetValue(DateTime.Now.ToString());
    //    var obj = transient.AssertCanBeSaved();

    //    obj.AssertIsTransient();
    //}

    //[Test]
    //public virtual void TestAssertCannotBeSavedFails() {
    //    var transient = GetTestService(FullName<Service1>()).GetAction("Get Transient").InvokeReturnObject();

    //    transient.GetPropertyByName("Id").SetValue("0");
    //    transient.GetPropertyByName("Prop1").SetValue("1");
    //    transient.GetPropertyByName("Foo").SetValue("test");
    //    transient.GetPropertyByName("Prop3").SetValue(DateTime.Now.ToString());
    //    AssertExpectException(() => transient.AssertCannotBeSaved(), "Assert.Fail failed. Object should not be saveable");
    //}

    [Test]
    public virtual void TestAssertCanBeSavedFails() {
        var transient = GetTestService<Service1>().GetAction("Get Transient").InvokeReturnObject();
        AssertExpectException(() => transient.AssertCanBeSaved(), "Assert.Fail failed. Id:Mandatory");
    }

    [Test]
    public virtual void TestAssertCannotBeSaved() {
        var transient = GetTestService<Service1>().GetAction("Get Transient").InvokeReturnObject();
        transient.AssertCannotBeSaved();
    }

    [Test]
    public virtual void TestAssertCanBeSavedOnPersistent() {
        var obj = NewTestObject<Object1>();
        AssertExpectException(() => obj.AssertCanBeSaved(), "Assert.IsTrue failed. Can only persist a transient object: 'ROSI.Test.Data.Object1'");
    }

    [Test]
    public virtual void TestAssertCannotBeSavedOnPersistent() {
        var obj = NewTestObject<Object1>();
        obj.AssertCannotBeSaved();
    }

    [Test]
    public virtual void TestAssertOnTransient() {
        var transient = GetTestService<Service1>().GetAction("Get Transient").InvokeReturnObject();
        transient.AssertIsTransient();
        AssertExpectException(() => transient.AssertIsPersistent(), "Assert.IsTrue failed. Object is not persistent");
    }

    [Test]
    public virtual void TestAssertOnPersistent() {
        var obj = NewTestObject<Object1>();
        obj.AssertIsPersistent();
        AssertExpectException(() => obj.AssertIsTransient(), "Assert.IsTrue failed. Object is not transient");
    }

    [Test]
    public virtual void TestGetPropertyOrder() {
        var obj = NewTestObject<Object1>();
        var order = obj.GetPropertyOrder();
        Assert.AreEqual("Id, Prop1, Foo, Prop3", order);
    }

    [Test]
    public virtual void TestAssertPropertyOrderIs() {
        var obj = NewTestObject<Object1>();
        obj.AssertPropertyOrderIs("Id, Prop1, Foo, Prop3");
        AssertExpectException(() => obj.AssertPropertyOrderIs("Prop1, Foo, Prop3, Id"), "Assert.AreEqual failed. Expected:<Prop1, Foo, Prop3, Id>. Actual:<Id, Prop1, Foo, Prop3>. ");
    }

    [Test]
    public virtual void TestAssertIsDescribedAsNone() {
        var obj = NewTestObject<Object1>();
        obj.AssertIsDescribedAs("");
        AssertExpectException(() => obj.AssertIsDescribedAs("a description"), "Assert.IsTrue failed. Description expected: 'a description' actual: ''");
    }

    [Test]
    public virtual void TestAssertIsDescribedAs() {
        var obj = NewTestObject<Object2>();
        obj.AssertIsDescribedAs("an object");
        AssertExpectException(() => obj.AssertIsDescribedAs("a object"), "Assert.IsTrue failed. Description expected: 'a object' actual: 'an object'");
    }
}