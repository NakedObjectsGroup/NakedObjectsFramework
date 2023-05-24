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

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Menus {
    //This class is not testing menus, nor TestMenus, but simply backwards compatibility
    //of GetAction, including with specified subMenu.
    [TestFixture]
    public class TestAccessingMenuActionsViaGetAction : AcceptanceTestCase {
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
        }

        protected INakedFramework NakedFramework { get; set; }

        protected Func<IConfiguration, DbContext>[] ContextCreators =>
            new Func<IConfiguration, DbContext>[] { config => new CADbContext() };

        protected Type[] ObjectTypes =>
            new[] {
                typeof(Foo),
                typeof(Foo2),
                typeof(Bar)
            };

        protected Type[] Services =>
            new[] {
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo2>),
                typeof(SimpleRepository<Bar>),
                typeof(ContributingService)
            };

        [Test]
        public void ContributedActionToObjectWithDefaultMenu() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction("Action1"));
        }

        [Test]
        public void ContributedActionToObjectWithExplicitMenu() {
            var bar = NewTestObject<Bar>();
            Assert.IsNotNull(bar.GetAction("Action3"));
            Assert.IsNotNull(bar.GetAction("Action4"));
            Assert.IsNotNull(bar.GetAction("Action5"));
            Assert.IsNotNull(bar.GetAction("Action4", "Sub1"));
            Assert.IsNotNull(bar.GetAction("Action5", "Sub2"));
        }

        [Test]
        public void ContributedActionToSubMenuObjectWithDefaultMenu() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction("Action2", "Sub"));
            Assert.IsNotNull(foo.GetAction("Action2")); //Note that you can also access the action directly
            try {
                foo.GetAction("Action1", "Sub");
            }
            catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Action1' within sub-menu 'Sub'", e.Message);
            }
        }
    }
}

public class CADbContext : DbContext {
    public const string DatabaseName = "Tests";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public CADbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public DbSet<Bar> Bars { get; set; }

    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new CADbDatabaseInitializer());

    public class CADbDatabaseInitializer : DropCreateDatabaseAlways<CADbContext> {
        protected override void Seed(CADbContext context) {
            context.Foos.Add(new Foo { Id = 1 });
            context.Bars.Add(new Bar { Id = 1 });
        }
    }
}

public class Foo {
    [NakedObjectsIgnore]
    public virtual int Id { get; set; }

    public void NativeAction() { }
}

public class Foo2 : Foo { }

public class Bar {
    [NakedObjectsIgnore]
    public virtual int Id { get; set; }

    public static void Menu(IMenu menu) {
        menu.CreateSubMenu("Sub1");
        menu.AddContributedActions();
    }
}

public class ContributingService {
    public void Action1(string str, [ContributedAction] Foo foo) { }

    public void Action2(string str, [ContributedAction("Sub")] Foo foo) { }

    public void Action3([ContributedAction] Bar bar) { }

    public void Action4([ContributedAction("Sub1")] Bar bar) { }

    public void Action5([ContributedAction("Sub2")] Bar bar) { }
}