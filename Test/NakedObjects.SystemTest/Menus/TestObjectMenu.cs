// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Menu;
using NakedObjects.Services;
using TestObjectMenu;
using Unity;

namespace NakedObjects.SystemTest.Menus {
    [TestClass]
    public class TestObjectMenu : AbstractSystemTest<MenusDbContext> {
        protected override string[] Namespaces {
            get { return new[] {typeof (Foo).Namespace}; }
        }

        [TestMethod]
        public void TestDefaultMenu() {
            var foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = foo.GetMenu();

            menu.AssertItemCountIs(6);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            items[2].AssertIsAction().AssertNameEquals("Action4");
            items[3].AssertIsAction().AssertNameEquals("Action3");

            var sub = items[4].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [TestMethod]
        public void TestDefaultMenuOnSubClass() {
            var foo2 = GetTestService("Foo2s").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = foo2.GetMenu();

            menu.AssertItemCountIs(7);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            items[2].AssertIsAction().AssertNameEquals("Action4");
            items[3].AssertIsAction().AssertNameEquals("Action3");
            items[4].AssertIsAction().AssertNameEquals("Action5");

            var sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[6].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [TestMethod]
        public void TestSpecifiedMenu() {
            var bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar.GetMenu();

            menu.AssertItemCountIs(6);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[1].AssertIsAction().AssertNameEquals("Action1");
            var sub = items[2].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[3].AssertIsSubMenu().AssertNameEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[4].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [TestMethod]
        public void SubClassWithNoNewActionsOrMenuGetsDefaultMenu() {
            var bar2 = GetTestService("Bar2s").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar2.GetMenu();

            menu.AssertItemCountIs(8);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action1");
            items[1].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[2].AssertIsAction().AssertNameEquals("Action3");
            items[3].AssertIsAction().AssertNameEquals("Action4");
            items[4].AssertIsAction().AssertNameEquals("Action5"); //New in this sub-class

            var sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[6].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");

            sub = items[7].AssertIsSubMenu().AssertNameEquals("Docs").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action8");
        }

        [TestMethod]
        public void SubClassDelegatingToSuperMenu() {
            var bar3 = GetTestService("Bar3s").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar3.GetMenu();

            menu.AssertItemCountIs(7);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[1].AssertIsAction().AssertNameEquals("Action1");
            var sub = items[2].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[3].AssertIsSubMenu().AssertNameEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            //Note that the sub-type's new action has been added here, by the
            //AddRemainingNativeActions on the superclass menu
            items[4].AssertIsAction().AssertNameEquals("Action6");

            sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[6].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [TestMethod]
        public void SubClassAddingNewSubMenuAboveSuperMenu() {
            var bar4 = GetTestService("Bar4s").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar4.GetMenu();

            menu.AssertItemCountIs(7);
            var items = menu.AllItems();

            var sub = items[0].AssertIsSubMenu().AssertNameEquals("Bar 4").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");

            items[1].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[2].AssertIsAction().AssertNameEquals("Action1");
            sub = items[3].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[4].AssertIsSubMenu().AssertNameEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[5].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[6].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        [TestMethod]
        public void TwoLevelsOfInheritance() {
            var bar5 = GetTestService("Bar5s").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar5.GetMenu();

            menu.AssertItemCountIs(8);
            var items = menu.AllItems();

            items[0].AssertIsAction().AssertNameEquals("Action8");
            var sub = items[1].AssertIsSubMenu().AssertNameEquals("Bar 4").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");

            items[2].AssertIsAction().AssertNameEquals("Action2 Renamed");
            items[3].AssertIsAction().AssertNameEquals("Action1");
            sub = items[4].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action3");

            sub = items[5].AssertIsSubMenu().AssertNameEquals("Docs").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action8");

            sub = items[6].AssertIsSubMenu().AssertNameEquals("Contrib1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action6a");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action5");

            sub = items[7].AssertIsSubMenu().AssertNameEquals("Contrib2a").AsSubMenu().AssertItemCountIs(1);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action7");
        }

        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc) {
            Database.Delete(MenusDbContext.DatabaseName);
            var context = Activator.CreateInstance<MenusDbContext>();

            context.Database.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestObjectMenu());
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        #region System Config

        protected override object[] MenuServices {
            get {
                return new object[] {
                    new SimpleRepository<Foo>(),
                    new SimpleRepository<Foo2>(),
                    new SimpleRepository<Bar>(),
                    new SimpleRepository<Bar2>(),
                    new SimpleRepository<Bar3>(),
                    new SimpleRepository<Bar4>(),
                    new SimpleRepository<Bar5>(),
                    new Contrib1(),
                    new Contrib2(),
                    new Contrib3()
                };
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
        }

        #endregion
    }
}

namespace TestObjectMenu {
    public class MenusDbContext : DbContext {
        public const string DatabaseName = "TestMenus";
        public MenusDbContext() : base(DatabaseName) {}

        public DbSet<Foo> Foo { get; set; }
        public DbSet<Bar> Bar { get; set; }
    }

    public class Foo {
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        [MemberOrder(2), DisplayName("Renamed1")]
        public void Action1() {}

        [MemberOrder(1)]
        public void Action2() {}

        [MemberOrder(4)]
        public void Action3() {}

        [MemberOrder(Sequence = "3")]
        public void Action4() {}
    }

    public class Foo2 : Foo {
        public void Action5() {}
    }

    public class Contrib1 {
        [MemberOrder(2)]
        public void Action5([ContributedAction("Contrib1")] Foo foo, [ContributedAction("Contrib1")] Bar bar) {}

        [MemberOrder(1), Named("Action6a")]
        public void Action6([ContributedAction("Contrib1")] Foo foo, [ContributedAction("Contrib1")] Bar bar) {}
    }

    public class Contrib2 {
        public void Action7([ContributedAction("Contrib2a")] Foo foo, [ContributedAction("Contrib2a")] Bar bar) {}
    }

    public class Contrib3 {
        public void Action8([ContributedAction("Docs")] Bar bar) {}

        public void Action9(IQueryable<Bar> bars) {}

        public void Action10([ContributedAction] IQueryable<Bar> bars) {}
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

        public void Action1() {}

        [DisplayName("Action2 Renamed")]
        public void Action2() {}

        public void Action3() {}

        public void Action4() {}
    }

    public class Bar2 : Bar {
        public void Action5() {}
    }

    public class Bar3 : Bar {
        public new static void Menu(IMenu menu) {
            Bar.Menu(menu);
        }

        public void Action6() {}
    }

    public class Bar4 : Bar {
        public new static void Menu(IMenu menu) {
            menu.CreateSubMenu("Bar 4").AddAction("Action7");
            Bar.Menu(menu);
        }

        public void Action7() {}
    }

    public class Bar5 : Bar4 {
        public new static void Menu(IMenu menu) {
            menu.AddAction("Action8");
            Bar4.Menu(menu);
        }

        public void Action8() {}
    }
}