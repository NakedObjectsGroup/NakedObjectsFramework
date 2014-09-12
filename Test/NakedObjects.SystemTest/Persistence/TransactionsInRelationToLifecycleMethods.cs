using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Persistence {
    [TestClass]
    public class TransactionsInRelationToLifecycleMethods : AcceptanceTestCase {
        #region Constructors

        public TransactionsInRelationToLifecycleMethods(string name) : base(name) {}

        public TransactionsInRelationToLifecycleMethods() : this(typeof (TransactionsInRelationToLifecycleMethods).Name) {}

        #endregion

        #region Run configuration

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(
                    new SimpleRepository<Foo1>(),
                    new SimpleRepository<Bar1>(),
                    new SimpleRepository<Qux1>());
            }
        }

        protected override IObjectPersistorInstaller Persistor {
            get {
                //Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
                Database.SetInitializer(new MyDbInitialiser());
                var installer = new EntityPersistorInstaller();
                installer.UsingCodeFirstContext(() => new TestContext());
                return installer;
            }
        }

        #endregion

        #region Initialize and Cleanup

        // to get EF SqlServer Dll in memory
        public SqlProviderServices instance = SqlProviderServices.Instance;

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework(this);
        }

        #endregion

        [TestMethod]
        public virtual void IdIsSetByTheTimePersistedIsCalled() {
            ITestObject foo = GetTestService("Foo1s").GetAction("New Instance").InvokeReturnObject();
            foo.AssertIsTransient();
            ITestProperty id = foo.GetPropertyByName("Id").AssertValueIsEqual("0");
            ITestProperty idPersisting = foo.GetPropertyByName("Id On Persisting").AssertValueIsEqual("0");
            ITestProperty idPersisted = foo.GetPropertyByName("Id On Persisted").AssertValueIsEqual("0");
            foo.Save().AssertIsPersistent();
            id.AssertValueIsEqual("1");
            idPersisting.AssertValueIsEqual("0");
            idPersisted.AssertValueIsEqual("1");
        }

        [TestMethod]
        public virtual void ExceptionInPersistedCausesWholeTransactionToFail() {
            ITestObject bar = GetTestService("Bar1s").GetAction("New Instance").InvokeReturnObject();
            try {
                bar.Save();
            }
            catch {
                bar.AssertIsTransient();
            }
        }

        private static bool triggerFail = false;
        internal static void FailAsRequired() {
            if (triggerFail) {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public virtual void ExceptionInUpdatedCausesWholeTransactionToFail() {
            ITestAction qs = GetTestService("Qux1s").GetAction("All Instances");
            ITestObject q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
            ITestProperty name = q.GetPropertyByName("Name");
            name.AssertValueIsEqual("Qux 1");
            try {
                triggerFail = true;
                name.SetValue("yyy");
            }
            catch {
                triggerFail = false;
                q.Refresh();
                q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
                name = q.GetPropertyByName("Name");
                name.AssertValueIsEqual("Qux 1");
            }
        }
    }

    public class Foo1 {
        private int myIdOnPersisted;
        public virtual int Id { get; set; }

        public virtual int IdOnPersisting { get; set; }

        public virtual int IdOnPersisted {
            get { return myIdOnPersisted; }
        }

        public void Persisting() {
            IdOnPersisting = Id;
        }

        public void Persisted() {
            myIdOnPersisted = Id;
        }
    }

    public class Bar1 {
        public virtual int Id { get; set; }

        public void Persisted() {
            throw new NotImplementedException();
        }
    }

    public class Qux1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Name { get; set; }

        public void Updated() {
            TransactionsInRelationToLifecycleMethods.FailAsRequired();
        }
    }

    public class TestContext : DbContext {
        public TestContext(string name) : base(name) {}
        public TestContext() {}

        public DbSet<Foo1> Foos { get; set; }
        public DbSet<Bar1> Bars { get; set; }
        public DbSet<Qux1> Quxes { get; set; }
    }

    public class MyDbInitialiser : DropCreateDatabaseAlways<TestContext> {
        protected override void Seed(TestContext context) {
            Qux1 q1 = NewQux("Qux 1", context);
        }

        private Qux1 NewQux(string name, TestContext context) {
            var q = new Qux1 {Name = name};
            context.Quxes.Add(q);
            return q;
        }
    }
}