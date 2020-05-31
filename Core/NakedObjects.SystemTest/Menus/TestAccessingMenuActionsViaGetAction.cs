// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using SystemTest.ContributedActions;
using NakedObjects;
using NakedObjects.Menu;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using NUnit.Framework;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.Menus {
    //This class is not testing menus, nor TestMenus, but simply backwards compatibility
    //of GetAction, including with specified subMenu.
    [TestFixture]
    public class TestAccessingMenuActionsViaGetAction : AbstractSystemTest<CADbContext> {
        protected override string[] Namespaces => new[] {typeof(Foo).Namespace};

        protected override Type[] Services =>
            new[] {
                typeof(SimpleRepository<Foo>),
                typeof(SimpleRepository<Foo2>),
                typeof(SimpleRepository<Bar>),
                typeof(ContributingService)
            };

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void CleanUp() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            CADbContext.Delete();
            var context = Activator.CreateInstance<CADbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            CADbContext.Delete();
        }

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
                Assert.AreEqual("Assert.IsNotNull failed. No menu item with name: Action1", e.Message);
            }
        }
    }
}

namespace SystemTest.ContributedActions {
    public class CADbContext : DbContext {
        public const string DatabaseName = "Tests";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public CADbContext() : base(Cs) { }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<Bar> Bars { get; set; }

        public static void Delete() => Database.Delete(Cs);
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
}