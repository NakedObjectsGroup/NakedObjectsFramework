using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects;
using NakedObjects.Menu;
using NakedObjects.Services;
using System;
using System.Data.Entity;
using SystemTest.ContributedActions;

namespace NakedObjects.SystemTest.ContributedActions {

    //Note that the [ContributedAction] attribute is tested under TestAttributes
    //This class just tests that the CAs are visible within XATs
    [TestClass]
    public class ContributedActions : AbstractSystemTest<CADbContext> {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new ContributedActions());
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
        public void ActionIsContributedToFoo() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction("Action1"));
        }

        [TestMethod]
        public void ActionIsContributedToSubclass() {
            var foo = NewTestObject<Foo2>();
            Assert.IsNotNull(foo.GetAction("Action1"));
        }

        [TestMethod]
        public void ActionNotContributedToBar() {
            var bar = NewTestObject<Bar>();
            try {
            bar.GetAction("Action1");
            } catch (Exception e)
            {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Action1'", e.Message);
            }
        }

        [TestMethod, Ignore] //Not working because the sub-menu prop of TestAction is not now being set up
        public void ActionIsContributedIntoSubMenu() {
            var foo = NewTestObject<Foo>();
            Assert.IsNotNull(foo.GetAction("Native Action", "Sub1"));
            Assert.IsNotNull(foo.GetAction( "Action2", "Sub1"));
            //Test that it same action does not show up at root level
            try {
                foo.GetAction("Action2");
            } catch (Exception e) {
                Assert.AreEqual("Assert.Fail failed. No Action named 'Action2'", e.Message);
            }
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

        public static void Menu(ITypedMenu<Foo> menu) {
            var sub = menu.CreateSubMenu("Sub1");
            sub.AddActionFrom<Foo>("NativeAction");
        }
        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        public void NativeAction() {}
    }

    public class Foo2 : Foo {

    }

    public class Bar {

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

    }

    public class ContributingService {

        public void Action1( string str, [ContributedAction] Foo foo, Bar bar) { }

        public void Action2(string str, [ContributedAction("Sub")] Foo foo, Bar bar) { }
    }

    //[Named("Foos")]
    //public class FooRepository : SimpleRepository<Foo> {


    //}
}
