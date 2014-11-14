using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menus;
using NakedObjects.Xat;
using System.Data.Entity;
using System.Linq;

namespace NakedObjects.SystemTest.Menus {

    [TestClass]
    public class TestMainMenus : AbstractSystemTest<MenusDbContext> {
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
                return new object[] { new FooService(), new BarService(), new QuxService() };
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
            container.RegisterType<IMainMenuDefinition, LocalMainMenus>();
        }
        #endregion

        [TestMethod]
        public virtual void TestXatHelpersForAccessingMenus() {
            AssertMainMenuCountIs(3);

            var menus = AllMainMenus();
            Assert.AreEqual(3, menus.Count());
            Assert.AreEqual(3, menus.OfType<ITestMenu>().Count());

            GetMainMenu("Foo Service").AssertNameEquals("Foo Service");
        }
    }

    #region Classes used in test

    public class MenusDbContext : DbContext {
        public const string DatabaseName = "TestMenus";
        public MenusDbContext() : base(DatabaseName) { }

        //public DbSet<Default1> Default1s { get; set; }
    }

    public class LocalMainMenus : IMainMenuDefinition {

        public IMenu[] MainMenus(IMenuFactory factory) {
            var foos = factory.NewMenu<FooService>(true);
            var bars = factory.NewMenu<BarService>(false);
            bars.AddAction("BarAction1");
            var qs = factory.NewMenu<QuxService>(true, "Qs");
            return new IMenu[] { foos, bars, qs };
        }
    }

    public class FooService {

        public void FooAction1() { }

        public void FooAction2() { }

        public void FooAction3(string p1, int p2) { }
    }

    [Named("Bars")]
    public class BarService {

        public void BarAction1() { }

        public void BarAction2() { }
    }

    [Named("Quxes")]
    public class QuxService {

        public void QuxAction1() { }
    }

#endregion
}
