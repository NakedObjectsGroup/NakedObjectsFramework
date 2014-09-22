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

namespace NakedObjects.SystemTest.Persistence
{
    [TestClass]
    public class TestPersistence : 
        AbstractSystemTest2<PersistenceDbContext>
    {
            #region Setup/Teardown
            [ClassInitialize]
            public static void ClassInitialize(TestContext tc)
            {
                InitializeNakedObjectsFramework(new TestPersistence());
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
                CleanupNakedObjectsFramework(new TestPersistence());
                Database.Delete(PersistenceDbContext.DatabaseName);
            }

            [TestInitialize()]
            public void TestInitialize()
            {
                StartTest();
            }

            [TestCleanup()]
            public void TestCleanup()
            {
            }

            #endregion

        #region Run configuration

        protected override IServicesInstaller MenuServices
        {
            get
            {
                return new ServicesInstaller(
                    new SimpleRepository<Foo1>(),
                    new SimpleRepository<Bar1>(),
                    new SimpleRepository<Qux1>());
            }
        }

        #endregion

        [TestMethod]
        public virtual void IdIsSetByTheTimePersistedIsCalled()
        {
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
        public virtual void ExceptionInPersistedCausesWholeTransactionToFail()
        {
            ITestObject bar = GetTestService("Bar1s").GetAction("New Instance").InvokeReturnObject();
            try
            {
                bar.Save();
            }
            catch
            {
                bar.AssertIsTransient();
            }
        }

        private static bool triggerFail = false;
        internal static void FailAsRequired()
        {
            if (triggerFail)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public virtual void ExceptionInUpdatedCausesWholeTransactionToFail()
        {
            ITestAction qs = GetTestService("Qux1s").GetAction("All Instances");
            ITestObject q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
            ITestProperty name = q.GetPropertyByName("Name");
            name.AssertValueIsEqual("Qux 1");
            try
            {
                triggerFail = true;
                name.SetValue("yyy");
            }
            catch
            {
                triggerFail = false;
                q.Refresh();
                q = qs.InvokeReturnCollection().AssertCountIs(1).ElementAt(0);
                name = q.GetPropertyByName("Name");
                name.AssertValueIsEqual("Qux 1");
            }
        }
    }

    #region Classes used by tests

    public class PersistenceDbContext : DbContext
    {
         public const string DatabaseName = "TestPersistence";
         public PersistenceDbContext() : base(DatabaseName) { }

        public DbSet<Foo1> Foos { get; set; }
        public DbSet<Bar1> Bars { get; set; }
        public DbSet<Qux1> Quxes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MyDbInitialiser());
        }
    }

    public class MyDbInitialiser : DropCreateDatabaseAlways<PersistenceDbContext>
    {
        protected override void Seed(PersistenceDbContext context)
        {
            Qux1 q1 = NewQux("Qux 1", context);
        }

        private Qux1 NewQux(string name, PersistenceDbContext context)
        {
            var q = new Qux1 { Name = name };
            context.Quxes.Add(q);
            return q;
        }
    }

    public class Foo1
    {
        private int myIdOnPersisted;
        public virtual int Id { get; set; }

        public virtual int IdOnPersisting { get; set; }

        public virtual int IdOnPersisted
        {
            get { return myIdOnPersisted; }
        }

        public void Persisting()
        {
            IdOnPersisting = Id;
        }

        public void Persisted()
        {
            myIdOnPersisted = Id;
        }
    }

    public class Bar1
    {
        public virtual int Id { get; set; }

        public void Persisted()
        {
            throw new NotImplementedException();
        }
    }

    public class Qux1
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Name { get; set; }

        public void Updated()
        {
            TestPersistence.FailAsRequired();
        }
    }
    #endregion
}