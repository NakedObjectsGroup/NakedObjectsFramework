// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menu;
using TestObjectMenu;
using Microsoft.Practices.Unity;

namespace NakedObjects.SystemTest.Menus {
    [TestClass]
    public class TestServiceMenus : AbstractSystemTest<MenusDbContext> {
        [TestMethod]
        public void TestDefaultServiceMenu() {
            var menu = GetTestService("Foo Service").GetMenu();
            var items = menu.AssertItemCountIs(3).AllItems();
            items[0].AssertIsAction().AssertNameEquals("Foo Action0");
            items[1].AssertIsAction().AssertNameEquals("Foo Action2");
            items[2].AssertIsAction().AssertNameEquals("Foo Action1");
        }

        [TestMethod]
        public void TestDefaultServiceMenuWithSubMenus() {
            var bars = GetTestService("Bars").GetMenu();
            bars.AssertItemCountIs(4);

            bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
            bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            bars.AllItems()[2].AssertIsAction().AssertNameEquals("Bar Action2");
            bars.AllItems()[3].AssertIsAction().AssertNameEquals("Bar Action3");
        }

        [TestMethod]
        public void TestWhenMainMenusNotSpecifiedServiceMenusAreUsed() {
            var bars = GetMainMenu("Bars"); //i.e. same as asking for GetService("Bars").GetMenu();
            bars.AssertItemCountIs(4);

            bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
            bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            bars.AllItems()[2].AssertIsAction().AssertNameEquals("Bar Action2");
            bars.AllItems()[3].AssertIsAction().AssertNameEquals("Bar Action3");
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
            CleanupNakedObjectsFramework(new TestServiceMenus());
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

        protected override object[] SystemServices {
            get {
                return new object[] {
                    new FooService(),
                    new ServiceWithSubMenus(),
                    new BarService(),
                    new QuxService()
                };
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
        }

        #endregion
    }

    #region Classes used in test

    public class FooService {
        [MemberOrder(1)]
        public void FooAction0() {}

        [MemberOrder(3)]
        public void FooAction1() {}

        [MemberOrder(2)]
        public void FooAction2(string p1, int p2) {}
    }

    [Named("Quxes")]
    public class QuxService {
        public void QuxAction0() {}

        public void QuxAction1() {}

        public void QuxAction2() {}

        [Hidden(WhenTo.Always)]
        public void QuxAction3() {}
    }

    [Named("Subs")]
    public class ServiceWithSubMenus {
        public void Action0() {}

        public void Action1() {}

        public void Action2() {}

        public void Action3() {}
    }

    [Named("Bars")]
    public class BarService {
        [MemberOrder(10)]
        public void BarAction0() {}

        [MemberOrder(1)]
        public void BarAction1() {}

        public void BarAction2() {}

        public void BarAction3() {}
    }

    #endregion
}