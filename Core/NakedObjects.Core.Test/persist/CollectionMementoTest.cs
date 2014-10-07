using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Persist;
using NakedObjects.EntityObjectStore;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.Core.Persist {
    public class TestDomainObject {
        public IDomainObjectContainer Container { protected get; set; }

        [Key]
        public virtual int Id { get; set; }

        public ICollection<TestDomainObject> Action1() {
            return Container.Instances<TestDomainObject>().ToList();
        }

        public ICollection<TestDomainObject> Action2(int filter) {
            return Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filter).ToList();
        }

        public ICollection<TestDomainObject> Action3(TestDomainObject filter) {
            if (filter != null) {
                return Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filter.Id).ToList();
            }
            return new TestDomainObject[] {};
        }

        public IQueryable<TestDomainObject> Action4(IEnumerable<int> filter) {
            return from tdo in Container.Instances<TestDomainObject>()
                   from ids in filter
                   where tdo.Id != ids
                   select tdo;
        }

        public IQueryable<TestDomainObject> Action5(IEnumerable<TestDomainObject> filter) {
            IEnumerable<int> idsToFilter = filter.Select(tdo => tdo.Id);
            return Action4(idsToFilter);
        }

        public ICollection<TestDomainObject> Action6(string filter) {
            int filterInt = int.Parse(filter);
            return Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filterInt).ToList();
        }

        public IQueryable<TestDomainObject> Action7(IEnumerable<string> filter) {
            IEnumerable<int> idsToFilter = filter.Select(int.Parse);
            return Action4(idsToFilter);
        }
    }

    public class TestContext : DbContext {

        public TestContext(string name) : base(name) {
            
        }
        public DbSet<TestDomainObject> TestDomainObjects { get; set; }
    }


    public class TestDataFixture  {

        public IDomainObjectContainer Container { protected get; set; }

        public  void Install() {
            TestDomainObject tdo1 = NewTdo(1);
            TestDomainObject tdo2 = NewTdo(2);
            TestDomainObject tdo3 = NewTdo(3);
        }

        private TestDomainObject NewTdo(int id) {
            var tdo = Container.NewTransientInstance<TestDomainObject>();
            tdo.Id = id;

            Container.Persist(ref tdo);
            return tdo;
        }
    }


    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            // replace INakedObjectStore types
            var c = new EntityObjectStoreConfiguration();
            c.UsingCodeFirstContext(() => new TestContext("TestContext"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(c, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupFixture() {
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownFixture() {
            CleanupNakedObjectsFramework(this);
        }

        [SetUp]
        public void Setup() {
            RunFixtures();
            StartTest();
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {new TestDataFixture()}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<TestDomainObject>()}); }
        }

        private void RoundTrip(CollectionMemento memento) {
            string[] strings1 = memento.ToEncodedStrings();
            var newMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, strings1);
            string[] strings2 = newMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings2), "memento failed roundtrip");

            var copyMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,   NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, memento, new object[] { });
            string[] strings3 = copyMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings3), "memento failed copy");
        }

        private void RecoverCollection(IEnumerable<TestDomainObject> originalCollection, CollectionMemento memento, INakedObjectManager manager) {
            IEnumerable<TestDomainObject> recoveredCollection = memento.RecoverCollection().GetAsEnumerable(manager).Select(no => no.GetDomainObject<TestDomainObject>());
            Assert.IsTrue(originalCollection.SequenceEqual(recoveredCollection), "recovered collection not same as original");
        }

        [Test]
        public void TestActionNoParms() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new INakedObject[] { });
            RoundTrip(memento);
            RecoverCollection(target.Action1(), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionNoParmsTransient() {
            var targetNo = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metadata.GetSpecification(typeof(TestDomainObject)));

            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new INakedObject[] { });
            RoundTrip(memento);
            RecoverCollection(targetNo.GetDomainObject<TestDomainObject>().Action1(), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionNoParmsWithSelected() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new INakedObject[] { });

            var selectedMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session,  memento, new[] { target });

            RoundTrip(selectedMemento);
            IEnumerable<TestDomainObject> recoveredCollection = selectedMemento.RecoverCollection().GetAsEnumerable(NakedObjectsFramework.LifecycleManager).Select(no => no.GetDomainObject<TestDomainObject>());
            Assert.IsFalse(target.Action1().SequenceEqual(recoveredCollection), "recovered selected collection same as original");

            IEnumerable<TestDomainObject> selectedCollection = target.Action1().Where(tdo => tdo.Id == target.Id);

            Assert.IsTrue(selectedCollection.SequenceEqual(recoveredCollection), "recovered selected collection not same as original selected collection");
        }


        [Test, Ignore] // Fix !
        public void TestActionObjectCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);

            TestDomainObject obj2 = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 2);

            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject> {target, obj2};
            INakedObject parm = NakedObjectsFramework.LifecycleManager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session,  targetNo, action, new[] { parm });

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionObjectCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);

            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject>();
            INakedObject parm = NakedObjectsFramework.LifecycleManager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { parm });

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.LifecycleManager);
        }


        [Test]
        public void TestActionObjectParm() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { targetNo });

            RoundTrip(memento);
            RecoverCollection(target.Action3(target), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionObjectParmNull() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session,  targetNo, action, new INakedObject[] { null });

            RoundTrip(memento);
            RecoverCollection(target.Action3(null), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test, Ignore] // Fix !
        public void TestActionValueCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int> {1, 2};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { NakedObjectsFramework.LifecycleManager.CreateAdapter(rawParm, null, null) });

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionValueCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int>();
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { NakedObjectsFramework.LifecycleManager.CreateAdapter(rawParm, null, null) });

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test, Ignore] // Fix !
        public void TestActionValueCollectionParmString() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action7");

            var rawParm = new List<string> {"1", "2"};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { NakedObjectsFramework.LifecycleManager.CreateAdapter(rawParm, null, null) });

            RoundTrip(memento);
            RecoverCollection(target.Action7(rawParm), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionValueParm() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action2");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { NakedObjectsFramework.LifecycleManager.CreateAdapter(1, null, null) });

            RoundTrip(memento);
            RecoverCollection(target.Action2(1), memento, NakedObjectsFramework.LifecycleManager);
        }

        [Test]
        public void TestActionValueParmString() {
            TestDomainObject target = NakedObjectsFramework.LifecycleManager.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.LifecycleManager.CreateAdapter(target, null, null);
            INakedObjectAction action = targetNo.Specification.GetAllActions().Single(a => a.Id == "Action6");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor,  NakedObjectsFramework.Metadata, NakedObjectsFramework.Session, targetNo, action, new[] { NakedObjectsFramework.LifecycleManager.CreateAdapter("1", null, null) });

            RoundTrip(memento);
            RecoverCollection(target.Action6("1"), memento, NakedObjectsFramework.LifecycleManager);
        }
    }
}