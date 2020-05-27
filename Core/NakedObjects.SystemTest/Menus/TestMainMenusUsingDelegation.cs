// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NUnit.Framework;
using TestObjectMenu;

namespace NakedObjects.SystemTest.Menus.Service {
    [TestFixture]
    public class TestMainMenusUsingDelegation : AbstractSystemTest<MenusDbContext> {
        protected override Type[] Services =>
            new[] {
                typeof(FooService),
                typeof(ServiceWithSubMenus),
                typeof(BarService),
                typeof(QuxService)
            };

        protected override string[] Namespaces => Types.Select(t => t.Namespace).Distinct().ToArray();

        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void CleanUp() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            MenusDbContext.Delete();
            var context = Activator.CreateInstance<MenusDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            MenusDbContext.Delete();
        }

        protected override IMenu[] MainMenus(IMenuFactory factory) => LocalMainMenus.MainMenus(factory);

        [Test]
        public virtual void TestMainMenus() {
            var menus = AllMainMenus();

            menus[0].AssertNameEquals("Foo Service");
            menus[1].AssertNameEquals("Bars"); //Picks up Named attribute on service
            menus[2].AssertNameEquals("Subs"); //Named attribute overridden in menu construction

            var foo = menus[0];
            foo.AssertItemCountIs(3);
            Assert.AreEqual(3, foo.AllItems().Count(i => i != null));

            foo.AllItems()[0].AssertNameEquals("Foo Action0");
            foo.AllItems()[1].AssertNameEquals("Foo Action1");
            foo.AllItems()[2].AssertNameEquals("Foo Action2");
        }
    }

    #region Classes used in test

    public class LocalMainMenus {
        public static IMenu[] MainMenus(IMenuFactory factory) {
            var menuDefs = new Dictionary<Type, Action<IMenu>>();
            menuDefs.Add(typeof(FooService), FooService.Menu);
            menuDefs.Add(typeof(BarService), BarService.Menu);
            menuDefs.Add(typeof(ServiceWithSubMenus), ServiceWithSubMenus.Menu);

            var menus = new List<IMenu>();
            foreach (var menuDef in menuDefs) {
                var menu = factory.NewMenu(menuDef.Key);
                menuDef.Value(menu);
                menus.Add(menu);
            }

            return menus.ToArray();
        }
    }

    public class FooService {
        public static void Menu(IMenu menu) {
            menu.Type = typeof(FooService);
            menu.AddRemainingNativeActions();
        }

        public void FooAction0() { }

        public void FooAction1() { }

        public void FooAction2(string p1, int p2) { }
    }

    [Named("Subs")]
    public class ServiceWithSubMenus {
        public static void Menu(IMenu menu) {
            menu.Type = typeof(ServiceWithSubMenus);
            var sub1 = menu.CreateSubMenu("Sub1");
            sub1.AddAction("Action1");
            sub1.AddAction("Action3");
            var sub2 = menu.CreateSubMenu("Sub2");
            sub2.AddAction("Action2");
            sub2.AddAction("Action0");
        }

        public void Action0() { }

        public void Action1() { }

        public void Action2() { }

        public void Action3() { }
    }

    [Named("Bars")]
    public class BarService {
        public static void Menu(IMenu menu) {
            menu.Type = typeof(BarService);
            menu.AddRemainingNativeActions();
        }

        [MemberOrder(10)]
        public void BarAction0() { }

        [MemberOrder(1)]
        public void BarAction1() { }

        public void BarAction2() { }

        public void BarAction3() { }
    }

    #endregion
}