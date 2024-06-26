﻿using Microsoft.EntityFrameworkCore;
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
public class TestTestParameter : AcceptanceTestCase {
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
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();

        Assert.AreEqual("Param0", param.Name);
    }

    [Test]
    public virtual void TestTypeValue() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();

        Assert.AreEqual(typeof(int), param.Type);
    }

    [Test]
    public virtual void TestTypeRef() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("With Ref Param").Parameters.First();

        Assert.AreEqual(typeof(Object1), param.Type);
    }

    [Test]
    public virtual void TestGetRefChoices() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("With Ref Param").Parameters.First();
        var choices = param.GetChoices();

        Assert.AreEqual(1, choices.Length);
        Assert.AreEqual("FooBar", choices.First().Title);
    }

    [Test]
    public virtual void TestGetValueChoices() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();
        var choices = param.GetChoices();

        Assert.AreEqual(3, choices.Length);
        Assert.AreEqual("0", choices.First().Title);
    }

    [Test]
    public virtual void TestGetCompletions() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("With Auto Complete").Parameters.First();
        var completions = param.GetCompletions("foo");

        Assert.AreEqual(1, completions.Length);
        Assert.AreEqual("FooBar", completions.First().Title);
    }

    [Test]
    public virtual void TestGetDefault() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();
        var def = param.GetDefault();

        Assert.AreEqual("8", def.Title);
    }

    [Test]
    public virtual void TestAssertIsMandatory() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();

        param.AssertIsMandatory();
        AssertExpectException(() => param.AssertIsOptional(), "Assert.IsTrue failed. Parameter: Param0 is mandatory");
    }

    [Test]
    public virtual void TestAssertIsOptional() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.Last();

        param.AssertIsOptional();
        AssertExpectException(() => param.AssertIsMandatory(), "Assert.IsTrue failed. Parameter: Param1 is optional");
    }

    [Test]
    public virtual void TestAssertIsDescribedAsNone() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.First();
        param.AssertIsDescribedAs("");
        AssertExpectException(() => param.AssertIsDescribedAs("a description"), "Assert.IsTrue failed. Parameter: Param0 description: '' expected: 'a description'");
    }

    [Test]
    public virtual void TestAssertIsDescribedAs() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.Last();
        param.AssertIsDescribedAs("a param");
        AssertExpectException(() => param.AssertIsDescribedAs("an param"), "Assert.IsTrue failed. Parameter: Param1 description: 'a param' expected: 'an param'");
    }

    [Test]
    public virtual void TestAssertIsNamed() {
        var obj = NewTestObject<Object2>();
        var param = obj.GetAction("Return Collection").Parameters.Last();
        param.AssertIsNamed("Param1");
        AssertExpectException(() => param.AssertIsNamed("Param2"), "Assert.IsTrue failed. Parameter name :'Param1' expected 'Param2'");
    }
}