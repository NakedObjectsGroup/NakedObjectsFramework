// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Framework;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using TestObjectMenu;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Menus.Service2;

[TestFixture]
public class TestMainMenusConventional : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
        NakedFramework = ServiceScope.ServiceProvider.GetService<INakedFramework>();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.AddEF6Persistor(options => { options.ContextCreators = ContextCreators; });
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.MainMenus = MainMenus;
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = Array.Empty<Type>();
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected INakedFramework NakedFramework { get; set; }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new MenusDbContext() };

    protected Type[] Services =>
        new[] {
            typeof(FooService),
            typeof(ServiceWithSubMenus),
            typeof(BarService),
            typeof(QuxService)
        };

    protected IMenu[] MainMenus(IMenuFactory factory) => LocalMainMenus.MainMenus(factory);

    [Test]
    public virtual void TestActionVisibility() {
        var q = GetMainMenu("Qs");
        q.AssertItemCountIs(3);

        q.GetAction("Qux Action0").AssertIsVisible();
        q.GetAction("Qux Action3").AssertIsInvisible();
    }

    [Test]
    public virtual void TestAddAllActions() {
        var foo = GetMainMenu("Foo Service");
        foo.AssertItemCountIs(3);
        Assert.AreEqual(3, foo.AllItems().Count(i => i != null));

        foo.AllItems()[0].AssertNameEquals("Foo Action0");
        foo.AllItems()[1].AssertNameEquals("Foo Action1");
        foo.AllItems()[2].AssertNameEquals("Foo Action2");
    }

    [Test]
    public virtual void TestAddAllActionsRecognisesMemberOrder() {
        var bars = GetMainMenu("Bars");
        bars.AssertItemCountIs(4);

        bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
        bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
        bars.AllItems()[2].AssertIsAction().AssertNameEquals("Bar Action2");
        bars.AllItems()[3].AssertIsAction().AssertNameEquals("Bar Action3");
    }

    [Test]
    public virtual void TestAddingActionsToAMenu() {
        var q = GetMainMenu("Qs");
        q.AssertItemCountIs(3);

        q.AllItems()[0].AssertIsAction().AssertNameEquals("Qux Action0");
        q.AllItems()[1].AssertIsAction().AssertNameEquals("Qux Action1");
        q.AllItems()[2].AssertIsAction().AssertNameEquals("Qux Action2");
    }

    [Test]
    public virtual void TestAddingSubMenuToAMenu() {
        var subs = GetMainMenu("Subs");
        subs.AssertItemCountIs(4);
        var sub1 = subs.AllItems()[0].AssertIsSubMenu().AssertSubMenuEquals("Sub1").AsSubMenu();
        sub1.AssertItemCountIs(2);
        sub1.AllItems()[0].AssertIsAction().AssertNameEquals("Action1");
        sub1.AllItems()[1].AssertIsAction().AssertNameEquals("Action3");

        var sub2 = subs.AllItems()[2].AssertIsSubMenu().AssertSubMenuEquals("Sub2").AsSubMenu();
        sub2.AssertItemCountIs(2);
        sub2.AllItems()[0].AssertIsAction().AssertNameEquals("Action2");
        sub2.AllItems()[1].AssertIsAction().AssertNameEquals("Action0");
    }

    [Test]
    public virtual void TestAllMainMenus() {
        var menus = AllMainMenus();
        Assert.AreEqual(menus.Length, menus.Count(i => i != null));
    }

    [Test]
    public void TestGetMainMenu() {
        GetMainMenu("Foo Service").AssertNameEquals("Foo Service");
    }

    [Test]
    public void TestHybridMenu() {
        var hyb = GetMainMenu("Hybrid");
        hyb.AssertItemCountIs(5);

        hyb.AllItems()[0].AssertIsAction().AssertNameEquals("Foo Action0");
        hyb.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
        hyb.AllItems()[2].AssertIsAction().AssertNameEquals("Qux Action0");
        hyb.AllItems()[3].AssertIsAction().AssertNameEquals("Qux Action1");
        hyb.AllItems()[4].AssertIsAction().AssertNameEquals("Qux Action2");
    }

    [Test]
    public virtual void TestMainMenuCount() {
        var menus = AllMainMenus();
        Assert.AreEqual(5, menus.Length);
    }

    [Test]
    public virtual void TestMainMenuNames() {
        var menus = AllMainMenus();

        menus[0].AssertNameEquals("Foo Service");
        menus[1].AssertNameEquals("Bars"); //Picks up Named attribute on service
        menus[2].AssertNameEquals("Qs"); //Named attribute overridden in menu construction
    }

    [Test]
    public virtual void TestMenuWithNoActions() {
        try {
            var e = GetMainMenu("Empty");
            Assert.Fail("Expect exception");
        }

        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No such menu: Empty", e.Message);
        }
    }

    [Test]
    public virtual void TestSubMenuWithNoActions() {
        try {
            var e = GetMainMenu("Empty2");
            Assert.Fail("Expect exception");
        }

        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No such menu: Empty2", e.Message);
        }
    }
}

#region Classes used in test

public class LocalMainMenus {
    public static IMenu[] MainMenus(IMenuFactory factory) {
        var foos = factory.NewMenu<FooService>(true);
        var bars = factory.NewMenu<BarService>(true);

        var q = factory.NewMenu<QuxService>(false, "Qs");
        q.AddAction("QuxAction0");
        q.AddAction("QuxAction3");
        q.AddRemainingNativeActions();

        var subs = factory.NewMenu<ServiceWithSubMenus>();
        var sub1 = subs.CreateSubMenu("Sub1");
        sub1.AddAction("Action1");
        sub1.AddAction("Action3");
        var sub2 = subs.CreateSubMenu("Sub2");
        sub2.AddAction("Action2");
        sub2.AddAction("Action0");

        var hyb = factory.NewMenu("Hybrid", "hybrid");
        hyb.AddAction(typeof(FooService), "FooAction0");
        hyb.AddAction(typeof(BarService), "BarAction0");
        hyb.AddRemainingActions(typeof(QuxService));

        var empty = factory.NewMenu<object>(false, "Empty");

        var empty2 = factory.NewMenu<object>(false, "Empty2");
        empty2.CreateSubMenu("Sub");

        return new[] { foos, bars, q, subs, hyb, empty, empty2 };
    }
}

public class FooService {
    public void FooAction0() { }

    public void FooAction1() { }

    public void FooAction2(string p1, int p2) { }
}

[Named("Quxes")]
public class QuxService {
    public void QuxAction0() { }

    public void QuxAction1() { }

    public void QuxAction2() { }

    [Hidden(WhenTo.Always)]
    public void QuxAction3() { }
}

[Named("Subs")]
public class ServiceWithSubMenus {
    public void Action0() { }

    public void Action1() { }

    public void Action2() { }

    public void Action3() { }
}

[Named("Bars")]
public class BarService {
    [MemberOrder(10)]
    public void BarAction0() { }

    [MemberOrder(1)]
    public void BarAction1() { }

    public void BarAction2() { }

    public void BarAction3() { }
}

#endregion