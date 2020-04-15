//using NakedObjects.Services;
//using NakedObjects.Xat;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using NUnit.Framework;

//namespace NakedObjects.SystemTest.Persistence2
//{

//    [TestFixture()]
//    public class MultipleDbContexts : AcceptanceTestCase
//    {

//        #region Constructors
//        public MultipleDbContexts(string name) : base(name) { }

//        public MultipleDbContexts() : this(typeof(MultipleDbContexts).Name) { }
//        #endregion

//        #region Run configuration

//        protected override IServicesInstaller MenuServices
//        {
//            get
//            {
//                return new ServicesInstaller(
//                    new SimpleRepository<Foo>(),
//                    new SimpleRepository<Bar>(),
//                    new SimpleRepository<Qux>());
//            }
//        }

//        protected override IObjectPersistorInstaller Persistor
//        {
//            get
//            {
//                Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");
//                var installer = new EntityPersistorInstaller();
//                installer.AddCodeFirstDbContextConstructor(() => new QuxContext());
//                installer.AddCodeFirstDbContextConstructor(() => new FooContext());
//                installer.AddCodeFirstDbContextConstructor(() => new BarContext());
//                return installer;
//            }
//        }
//        #endregion

//        #region Initialize and Cleanup

//        [SetUp()]
//        public void Initialize()
//        {
//            InitializeNakedObjectsFramework();
//            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
//        }

//        [TearDown()]
//        public void Cleanup()
//        {
//            CleanupNakedObjectsFramework();
//        }

//        #endregion

//        [Test()]
//        public virtual void ExceptionInSecond()
//        {
//            var getFoos = GetTestService("Foos").GetAction("All Instances");

//            var foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

//        }

//    }

//    public class Foo
//    {
//        public IDomainObjectContainer Container { set; protected get; }

//        public virtual int Id { get; set; }


//        public void Persisted()
//        {
//            var bar = Container.NewTransientInstance<Bar>();
//            bar.CreatorId = this.Id;
//            Container.Persist(ref bar);
//        }
//    }

//    public class Bar
//    {
//        public IDomainObjectContainer Container { set; protected get; }

//        public virtual int Id { get; set; }


//        public virtual int CreatorId { get; set; }


//        public void Persisted()
//        {
//            var qux = Container.NewTransientInstance<Qux>();
//            qux.CreatorId = this.Id;
//            Container.Persist(ref qux);
//        }
//    }

//    public class Qux
//    {
//        public virtual int Id { get; set; }

//        public virtual int CreatorId { get; set; }

//    }

//    public class FooContext : DbContext
//    {
//        public DbSet<Foo> Foos { get; set; }
//    }

//    public class BarContext : DbContext
//    {
//        public DbSet<Bar> Bars { get; set; }
//    }

//    public class QuxContext : DbContext
//    {
//        public DbSet<Qux> Quxes { get; set; }
//    }
//}