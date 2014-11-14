using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menus;
using NakedObjects.Services;
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
                    new SimpleRepository<Foo>() };
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

            menu.AssertItemCountIs(4);
            
            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action1");

        }

        [TestMethod]
        public void TestDefaultMenu2() {
            var foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject().Save();
            var menu = foo.GetMenu();

            menu.AssertItemCountIs(3);

            var items = menu.AllItems();
            items[0].AssertIsAction().AssertNameEquals("Action1");

        }
    }
}

namespace TestObjectMenu {
    public class MenusDbContext : DbContext {
        public const string DatabaseName = "TestMenus";
        public MenusDbContext() : base(DatabaseName) {}
        
            public DbSet<Foo> Foo { get; set; }      
        }

    public class Foo {

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public void Action1() { }

        public void Action2() { }

        public void Action3() { }
     
    }
}
