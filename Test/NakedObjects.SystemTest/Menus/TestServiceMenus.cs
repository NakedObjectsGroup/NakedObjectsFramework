using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Menu;
using NakedObjects.Xat;
using System.Data.Entity;
using System.Linq;
using TestObjectMenu;

namespace NakedObjects.SystemTest.Menus {

    [TestClass]
    public class TestServiceMenus : AbstractSystemTest<MenusDbContext> {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestServiceMenus());
            Database.Delete(MenusDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() { }

        #endregion

        #region System Config
        protected override object[] SystemServices {
            get {
                return new object[] { 
                    new FooService(), 
                    new ServiceWithSubMenus(),
                    new BarService(), 
                    new QuxService() };
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
        }
        #endregion

        [TestMethod]
        public void TestDefaultServiceMenu() {
           var menu = GetTestService("Foo Service").GetMenu();
          var items = menu.AssertItemCountIs(3).AllItems();
          items[0].AssertIsAction().AssertNameEquals("Foo Action0");
          items[1].AssertIsAction().AssertNameEquals("Foo Action1");
          items[2].AssertIsAction().AssertNameEquals("Foo Action2");
        }

        [TestMethod]
        public void TestDefaultServiceMenuWithSubMenus() {

            var bars = GetTestService("Bars").GetMenu();
            bars.AssertItemCountIs(3);

            bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
            bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            var sub1 = bars.AllItems()[2].AssertIsSubMenu().AssertNameEquals("sub1").AsSubMenu();
            sub1.AssertItemCountIs(2);
            sub1.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action2");
            sub1.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action3");
        }

        [TestMethod]
        public void TestWhenMainMenusNotSpecifiedServiceMenusAreUsed() {
            var bars = GetMainMenu("Bars"); //i.e. same as asking for GetService("Bars").GetMenu();
            bars.AssertItemCountIs(3);

            bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
            bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            var sub1 = bars.AllItems()[2].AssertIsSubMenu().AssertNameEquals("sub1").AsSubMenu();
            sub1.AssertItemCountIs(2);
            sub1.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action2");
            sub1.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action3");
        }

    }

    #region Classes used in test

    //public class LocalMainMenus : IMainMenuDefinition {

    //    public IMenuBuilder[] MainMenus(IMenuFactory factory) {
    //        var foos = factory.NewMenu<FooService>(true);
    //        var bars = factory.NewMenu<BarService>(true);

    //        var q = factory.NewMenu<QuxService>(false, "Qs");
    //        q.AddAction("QuxAction0");
    //        q.AddAction("QuxAction3", "Action X");
    //        q.AddRemainingNativeActions();

    //        var subs = factory.NewMenu<ServiceWithSubMenus>(false);
    //        var sub1 = subs.CreateSubMenuOfSameType("Sub1");
    //        sub1.AddAction("Action1");
    //        sub1.AddAction("Action3");
    //        var sub2 = subs.CreateSubMenuOfSameType("Sub2");
    //        sub2.AddAction("Action2");
    //        sub2.AddAction("Action0");

    //        var hyb = factory.NewMenu("Hybrid");
    //        hyb.AddActionFrom<FooService>("FooAction0");
    //        hyb.AddActionFrom<BarService>("BarAction0");
    //        hyb.AddAllRemainingActionsFrom<QuxService>();

    //        var empty = factory.NewMenu("Empty");

    //        var empty2 = factory.NewMenu("Empty2");
    //        empty2.CreateSubMenu("Sub");

    //        return new IMenuBuilder[] { foos, bars, q, subs, hyb, empty, empty2 };
    //    }
    //}

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

       [Hidden]
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

        [MemberOrder(Name = "sub1", Sequence = "1")]
        public void BarAction2() { }

        [MemberOrder(Name = "sub1", Sequence = "2")]
        public void BarAction3() { }

    }

#endregion
}
