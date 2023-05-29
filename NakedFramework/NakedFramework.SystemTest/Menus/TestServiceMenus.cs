// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework;
using NakedFramework.Architecture.Framework;
using NakedFramework.DependencyInjection.Extensions;
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

namespace NakedFramework.SystemTest.Menus.Service3;

[TestFixture]
public class TestServiceMenus : AcceptanceTestCase {
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
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = Array.Empty<Type>();
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
    }

    protected INakedFramework NakedFramework { get; set; }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new MenusDbContext() };

    protected  Type[] Services =>
        new[] {
            typeof(FooService),
            typeof(ServiceWithSubMenus),
            typeof(BarService),
            typeof(QuxService)
        };

    [Test]
    public void TestDefaultServiceMenu() {
        var menu = GetTestService("Foo Service").GetMenu();
        var items = menu.AssertItemCountIs(3).AllItems();
        items[0].AssertIsAction().AssertNameEquals("Foo Action0");
        items[1].AssertIsAction().AssertNameEquals("Foo Action2");
        items[2].AssertIsAction().AssertNameEquals("Foo Action1");
    }

    [Test]
    public void TestDefaultServiceMenuWithSubMenus() {
        var bars = GetTestService("Bars").GetMenu();
        bars.AssertItemCountIs(4);

        bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
        bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
        bars.AllItems()[2].AssertIsAction().AssertNameEquals("Bar Action2");
        bars.AllItems()[3].AssertIsAction().AssertNameEquals("Bar Action3");
    }

}

#region Classes used in test

public class FooService {
    [MemberOrder(1)]
    public void FooAction0() { }

    [MemberOrder(3)]
    public void FooAction1() { }

    [MemberOrder(2)]
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