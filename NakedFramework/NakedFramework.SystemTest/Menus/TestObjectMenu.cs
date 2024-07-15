// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
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
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;
using TestObjectMenu;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Service3 {
    [TestFixture]
    public class TestObjectMenu : AcceptanceTestCase {
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
                    appOptions.DomainModelTypes = ObjectTypes;
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


        protected  Type[] ObjectTypes =>
            new[] {
                typeof(Foo),
                typeof(Foo2),
                typeof(Bar),
                typeof(Bar2),
                typeof(Bar3),
                typeof(Bar4),
                typeof(Bar5)
            };

        protected  Type[] Services =>
            new[] {
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo2>),
                typeof(SimpleRepository<Bar>),
                typeof(SimpleRepository<Bar2>),
                typeof(SimpleRepository<Bar3>),
                typeof(SimpleRepository<Bar4>),
                typeof(SimpleRepository<Bar5>),
                typeof(Contrib1),
                typeof(Contrib2),
                typeof(Contrib3)
            };

        [Test]
        public void SubClassAddingNewSubMenuAboveSuperMenu() {
            var bar4 = NewTestObject<Bar5>("5");
            var menu = bar4.GetMenu();

            menu.AssertItemCountIs(10);
            var items = menu.AllItems();

            items[0].AssertIsAction().AssertNameEquals("Action 14");

            var sub = items[1].AssertIsSubMenu().AssertSubMenuEquals("Bar 4").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action 13");

            items[2].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[3].AssertIsAction().AssertNameEquals("Action1");
            sub = items[4].AssertIsSubMenu().AssertSubMenuEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[5].AssertIsSubMenu().AssertSubMenuEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[7].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[9].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [Test]
        public void SubClassDelegatingToSuperMenu() {
            var bar3 = NewTestObject<Bar3>("3");
            var menu = bar3.GetMenu();

            menu.AssertItemCountIs(9);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[1].AssertIsAction().AssertNameEquals("Action1");
            var sub = items[2].AssertIsSubMenu().AssertSubMenuEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[3].AssertIsSubMenu().AssertSubMenuEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            //Note that the sub-type's new action has been added here, by the
            //AddRemainingNativeActions on the superclass menu
            items[5].AssertIsAction().AssertNameEquals("Action 12");

            sub = items[6].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[8].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [Test]
        public void SubClassWithNoNewActionsOrMenuGetsDefaultMenu() {
            var bar2 = NewTestObject<Bar2>("2");
            var menu = bar2.GetMenu();

            menu.AssertItemCountIs(9);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action1");
            items[1].AssertIsAction().AssertNameEquals("Action 11");
            items[2].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[3].AssertIsAction().AssertNameEquals("Action3");
            items[4].AssertIsAction().AssertNameEquals("Action4"); //New in this sub-class

            var sub = items[5].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[7].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");

            sub = items[8].AssertIsSubMenu().AssertSubMenuEquals("Docs").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action8");
        }

        [Test]
        public void TestDefaultMenu() {
            var foo = NewTestObject<Foo>();
            var menu = foo.GetMenu();

            menu.AssertItemCountIs(7);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            items[2].AssertIsAction().AssertNameEquals("Action4");
            items[3].AssertIsAction().AssertNameEquals("Action3");

            var sub = items[4].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[6].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [Test]
        public void TestDefaultMenuOnSubClass() {
            var foo2 = NewTestObject<Foo2>("2");
            var menu = foo2.GetMenu();

            menu.AssertItemCountIs(8);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            items[2].AssertIsAction().AssertNameEquals("Action4");
            items[3].AssertIsAction().AssertNameEquals("Action3");
            items[4].AssertIsAction().AssertNameEquals("Action 15");

            var sub = items[5].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[7].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [Test]
        public void TestSpecifiedMenu() {
            var bar = NewTestObject<Bar>();
            var menu = bar.GetMenu();

            menu.AssertItemCountIs(8);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[1].AssertIsAction().AssertNameEquals("Action1");
            var sub = items[2].AssertIsSubMenu().AssertSubMenuEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[3].AssertIsSubMenu().AssertSubMenuEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[5].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[7].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [Test]
        public void TwoLevelsOfInheritance() {
            var bar5 = NewTestObject<Bar5>("5");
            var menu = bar5.GetMenu();

            menu.AssertItemCountIs(10);
            var items = menu.AllItems();

            items[0].AssertIsAction().AssertNameEquals("Action 14");
            var sub = items[1].AssertIsSubMenu().AssertSubMenuEquals("Bar 4").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action 13");

            items[2].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[3].AssertIsAction().AssertNameEquals("Action1");
            sub = items[4].AssertIsSubMenu().AssertSubMenuEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[5].AssertIsSubMenu().AssertSubMenuEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[7].AssertIsSubMenu().AssertSubMenuEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[9].AssertIsSubMenu().AssertSubMenuEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }
    }
}

namespace TestObjectMenu {
    public class MenusDbContext : DbContext {
        public const string DatabaseName = "TestMenus";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
        public MenusDbContext() : base(Cs) { }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<Bar> Bars { get; set; }

        public DbSet<Foo2> Foo2s { get; set; }
       
        public DbSet<Bar2> Bar2s { get; set; }
        public DbSet<Bar3> Bar3s { get; set; }
        public DbSet<Bar4> Bar4s { get; set; }
        public DbSet<Bar5> Bar5s { get; set; }


        public static void Delete() => Database.Delete(Cs);

        protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new MenuDatabaseInitializer());

        public class MenuDatabaseInitializer : DropCreateDatabaseAlways<MenusDbContext> {
            protected override void Seed(MenusDbContext context) {
                context.Foos.Add(new Foo { Id = 1 });
                context.Foo2s.Add(new Foo2 { Id = 2 });
                context.Bars.Add(new Bar { Id = 1 });
                context.Bar2s.Add(new Bar2 { Id = 1 });
                context.Bar3s.Add(new Bar3 { Id = 1 });
                context.Bar4s.Add(new Bar4 { Id = 1 });
                context.Bar5s.Add(new Bar5 { Id = 1 });
            }
        }

    }

    public class Foo {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(2)]
        [DisplayName("Renamed1")]
        public void Action1() { }

        [MemberOrder(1)]
        public void Action2() { }

        [MemberOrder(4)]
        public void Action3() { }

        [MemberOrder(Sequence = "3")]
        public void Action4() { }
    }

    public class Foo2 : Foo {
        public void Action15() { }
    }

    public class Contrib1 {
        [MemberOrder(2)]
        public void Action5([ContributedAction("Contrib1")] Foo foo, [ContributedAction("Contrib1")] Bar bar) { }

        [MemberOrder(1)]
        [Named("Action6a")]
        public void Action6([ContributedAction("Contrib1")] Foo foo, [ContributedAction("Contrib1")] Bar bar) { }
    }

    public class Contrib2 {
        public void Action7([ContributedAction("Contrib2a")] Foo foo, [ContributedAction("Contrib2a")] Bar bar) { }
    }

    public class Contrib3 {
        public void Action8([ContributedAction("Docs")] Bar bar) { }

        public void Action9(IQueryable<Bar> bars) { }

        public void Action10([ContributedAction] IQueryable<Bar> bars) { }
    }

    public class Bar {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public static void Menu(IMenu menu) {
            menu.AddAction("Action2");
            menu.AddAction("Action1");
            var sub = menu.CreateSubMenu("Sub1");
            sub.AddAction("Action3");

            sub = menu.CreateSubMenu("Docs");
            sub.AddAction("Action4");
            menu.AddRemainingNativeActions();
            menu.AddContributedActions();
        }

        public void Action1() { }

        [DisplayName("Action2 Renamed")]
        public void Action2() { }

        public void Action3() { }

        public void Action4() { }
    }

    public class Bar2 : Bar {
        public void Action11() { }
    }

    public class Bar3 : Bar {
        public new static void Menu(IMenu menu) {
            Bar.Menu(menu);
        }

        public void Action12() { }
    }

    public class Bar4 : Bar {
        public new static void Menu(IMenu menu) {
            menu.CreateSubMenu("Bar 4").AddAction("Action13");
            Bar.Menu(menu);
        }

        public void Action13() { }
    }

    public class Bar5 : Bar4 {
        public new static void Menu(IMenu menu) {
            menu.AddAction("Action14");
            Bar4.Menu(menu);
        }

        public void Action14() { }
    }
}