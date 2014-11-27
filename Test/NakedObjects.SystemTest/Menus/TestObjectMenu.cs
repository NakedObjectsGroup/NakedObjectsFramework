using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menus;
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
            CleanupNakedObjectsFramework(new TestMainMenus());
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
                    new SimpleRepository<Foo>(),
                new SimpleRepository<Bar>() };
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

            menu.AssertItemCountIs(3);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            var sub = items[2].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(2);
            sub.AllItems()[0].AssertIsAction().AssertNameEquals("Action4");
            sub.AllItems()[1].AssertIsAction().AssertNameEquals("Action3");         
        }

        [TestMethod]
        public void TestDefaultMenuWithContributedActions() {
            Assert.Fail();
        }

        [TestMethod]
        public void TestSpecifiedMenu() {
            var foo = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = foo.GetMenu();

            menu.AssertItemCountIs(4);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action2");
            items[1].AssertIsAction().AssertNameEquals("Renamed1");
            items[2].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu().AssertItemCountIs(1).AllItems()[0].AssertNameEquals("Action3");
            items[3].AssertIsAction().AssertNameEquals("Action4");
        }

        [TestMethod]
        public void TestSpecifiedMenuWithContributedActions() {
            Assert.Fail();
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

        [MemberOrder(2), DisplayName("Renamed")]
        public void Action1() { }

        [MemberOrder(1)]
        public void Action2() { }

        [MemberOrder(Name = "Sub1", Sequence="2")]
        public void Action3() { }

        [MemberOrder(Name = "Sub1", Sequence = "1")]
        public void Action4() { }
     
    }

    public class Bar {

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public static void Menu(ITypedMenu<Bar> menu) {
            menu.AddAction("Action2");
            menu.AddAction("Action1", "Renamed1");
            var sub1 =menu.CreateSubMenuOfSameType("Sub1");
            sub1.AddAction("Action3");
            menu.AddAllRemainingActions();
        }

        public void Action1() { }

        public void Action2() { }

        public void Action3() { }

        public void Action4() { }

    }
}
