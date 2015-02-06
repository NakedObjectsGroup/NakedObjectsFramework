using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Menu;
using NakedObjects.Services;
using System;
using System.Data.Entity;
using SystemTest.ContributedActions;

namespace NakedObjects.SystemTest.Menus {

    //This class is not testing menus, nor TestMenus, but simply backwards compatibility
    //of GetAction, including with specified subMenu.
    //access actions via GetAction.
    [TestClass]
    public class TestAccessingMenuActionsViaGetAction : AbstractSystemTest<CADbContext> {
        #region Setup/Teardown 

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestAccessingMenuActionsViaGetAction());
            Database.Delete(CADbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        #endregion

        #region Configuration
        protected override string[] Namespaces {
            get { return new[] { typeof(Foo).Namespace }; }
        }

        protected override object[] Services {
            get {
                return new object[] {
                    new SimpleRepository<Foo>(),
                    new SimpleRepository<Foo2>(),
                    new SimpleRepository<Bar>(),
                    new ContributingService()
                };
            }
        }
        #endregion

        [TestMethod]
        public void ContributedActionToObjectWithDefaultMenu() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction("Action1"));
        }

        [TestMethod]
        public void ContributedActionToSubMenuObjectWithDefaultMenu() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction( "Action2", "Sub"));
            Assert.IsNotNull(foo.GetAction("Action2")); //Note that you can also access the action directly
            try {
                foo.GetAction("Action1", "Sub");
            } catch (Exception e) {
                Assert.AreEqual("Assert.IsNotNull failed. No menu item with name: Action1", e.Message);
            }
        }

        [TestMethod]
        public void ContributedActionToObjectWithExplicitMenu() {
            var bar = NewTestObject<Bar>();
            Assert.IsNotNull(bar.GetAction("Action3"));
            Assert.IsNotNull(bar.GetAction("Action4"));
            Assert.IsNotNull(bar.GetAction("Action5"));
            Assert.IsNotNull(bar.GetAction("Action4", "Sub1"));
            Assert.IsNotNull(bar.GetAction("Action5", "Sub2"));
        }
    }
}

namespace SystemTest.ContributedActions {
    public class CADbContext : DbContext {
        public const string DatabaseName = "TestMethods";
        public CADbContext() : base(DatabaseName) { }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<Bar> Bars { get; set; }
    }

    public class Foo {

        //public static void Menu(ITypedMenu<Foo> menu) {
        //    var sub = menu.CreateSubMenu("Sub1");
        //    sub.AddActionFrom<Foo>("NativeAction");
        //    menu.AddContributedActions();
        //}
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public void NativeAction() {}
    }

    public class Foo2 : Foo {

    }

    public class Bar {

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public static void Menu(ITypedMenu<Bar> menu) {
            menu.CreateSubMenuOfSameType("Sub1");
            menu.AddContributedActions();
        }

    }

    public class ContributingService {

        public void Action1( string str, [ContributedAction] Foo foo) { }

        public void Action2(string str, [ContributedAction("Sub")] Foo foo) { }

        public void Action3([ContributedAction] Bar bar) {}

        public void Action4([ContributedAction("Sub1")] Bar bar) {}

        public void Action5([ContributedAction("Sub2")] Bar bar) {}
    }
}
