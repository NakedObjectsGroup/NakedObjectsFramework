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
using NUnit.Framework;
using ROSI.Test.Data;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedFramework.RATL.Test.Classic;

[TestFixture]
public class TestTestProperty : AcceptanceTestCase {
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
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop1));

        Assert.AreEqual("something else", prop.Name);
    }

    [Test]
    public virtual void TestId() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop1));

        Assert.AreEqual("Prop1", prop.Id);
    }

    [Test]
    public virtual void TestTitle() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop1));

        Assert.AreEqual("a name", prop.Title);
    }

    [Test]
    public virtual void TestTitleWithMask() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop2));

        var dt = new DateTime(2023, 2, 28);
        Assert.AreEqual(dt.ToString("d"), prop.Title);
    }

    [Test]
    public virtual void TestContent() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop1));
        var content = prop.Content;

        Assert.AreEqual("a name", content.Title);
    }

    [Test]
    public virtual void TestContentAsObject() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop3));
        var content = prop.ContentAsObject;

        Assert.IsInstanceOfType(content, typeof(ITestObject));
        Assert.AreEqual("Untitled Object2", content.Title);
    }

    [Test]
    public virtual void TestContentAsCollection() {
        var obj = NewTestObject<Object2>();
        var prop = obj.GetPropertyById(nameof(Object2.Prop4));
        var content = prop.ContentAsCollection;

        Assert.IsInstanceOfType(content, typeof(ITestCollection));
        Assert.AreEqual(0, content.Count());
    }
}