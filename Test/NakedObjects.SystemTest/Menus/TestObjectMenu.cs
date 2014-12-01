using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;
using NakedObjects.Meta.Menu;
using NakedObjects.Services;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using TestObjectMenu;

namespace NakedObjects.SystemTest.Menus {
    [TestClass]
    public class TestObjectMenu : AbstractSystemTest<MenusDbContext> {
                #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestObjectMenu());
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
        protected override object[] MenuServices {
            get {
                return new object[] { 
                    new SimpleRepository<Foo>(),
                new SimpleRepository<Bar>(),
                new Contrib1(),
                new Contrib2(),
                new Contrib3()};
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
        }
        #endregion


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
        public void TestSpecifiedMenu() {
            var bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = bar.GetMenu();

            menu.AssertItemCountIs(6);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
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
        public void Action1() { }

        [MemberOrder(1)]
        public void Action2() { }

        [MemberOrder(4)]
        public void Action3() { }

        [MemberOrder(Sequence = "3")]
        public void Action4() { }
     
    }

    
    public class Contrib1 {

        [MemberOrder(2)]
        public void Action5(Foo foo, Bar bar) { }

        [MemberOrder(1), Named("Action6a")]
        public void Action6(Foo foo, Bar bar) { }

    }

    [Named("Contrib2a")]
    public class Contrib2 {

        public void Action7(Foo foo, Bar bar) { }
    }

    [Named("Docs")]
    public class Contrib3 {

        public void Action8(Bar bar) { }
    }

    public class Bar {

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public static void Menu(ITypedMenu<Bar> menu) {
            menu.AddAction("Action2");
            menu.AddAction("Action1", "Renamed1");
            var sub =menu.CreateSubMenuOfSameType("Sub1");
            sub.AddAction("Action3");

            sub = menu.CreateSubMenuOfSameType("Docs");
            sub.AddAction("Action4");
            menu.AddRemainingNativeActions();
            menu.AddContributedActions();
        }

        public void Action1() { }

        public void Action2() { }

        public void Action3() { }

        public void Action4() { }

    }
}
