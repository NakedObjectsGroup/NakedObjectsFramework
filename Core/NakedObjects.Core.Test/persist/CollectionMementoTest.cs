using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.Core.Persist {
    public class TestDomainObject {
        public IDomainObjectContainer Container { protected get; set; }


        [Key]
        public int Id { get; set; }

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
        [SetUp]
        public void Setup() {
            InitializeNakedObjectsFramework();
        }

        [TearDown]
        public void TearDown() {
            CleanupNakedObjectsFramework();
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {new TestDataFixture()}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<TestDomainObject>()}); }
        }

        private void RoundTrip(CollectionMemento memento) {
            string[] strings1 = memento.ToEncodedStrings();
            var newMemento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, strings1);
            string[] strings2 = newMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings2), "memento failed roundtrip");

            var copyMemento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, memento, new object[] { });
            string[] strings3 = copyMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings3), "memento failed copy");
        }

        private void RecoverCollection(IEnumerable<TestDomainObject> originalCollection, CollectionMemento memento) {
            IEnumerable<TestDomainObject> recoveredCollection = memento.RecoverCollection().GetAsEnumerable().Select(no => no.GetDomainObject<TestDomainObject>());
            Assert.IsTrue(originalCollection.SequenceEqual(recoveredCollection), "recovered collection not same as original");
        }

        [Test]
        public void TestActionNoParms() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new INakedObject[] { });
            RoundTrip(memento);
            RecoverCollection(target.Action1(), memento);
        }

        [Test]
        public void TestActionNoParmsTransient() {
            var targetNo = NakedObjectsContext.ObjectPersistor.CreateInstance(NakedObjectsContext.Reflector.LoadSpecification(typeof (TestDomainObject)));

            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new INakedObject[] { });
            RoundTrip(memento);
            RecoverCollection(targetNo.GetDomainObject<TestDomainObject>().Action1(), memento);
        }

        [Test]
        public void TestActionNoParmsWithSelected() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new INakedObject[] { });

            var selectedMemento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, memento, new[] { target });

            RoundTrip(selectedMemento);
            IEnumerable<TestDomainObject> recoveredCollection = selectedMemento.RecoverCollection().GetAsEnumerable().Select(no => no.GetDomainObject<TestDomainObject>());
            Assert.IsFalse(target.Action1().SequenceEqual(recoveredCollection), "recovered selected collection same as original");

            IEnumerable<TestDomainObject> selectedCollection = target.Action1().Where(tdo => tdo.Id == target.Id);

            Assert.IsTrue(selectedCollection.SequenceEqual(recoveredCollection), "recovered selected collection not same as original selected collection");
        }


        [Test]
        public void TestActionObjectCollectionParm() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);

            TestDomainObject obj2 = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 2);

            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject> {target, obj2};
            INakedObject parm = PersistorUtils.CreateAdapter(rawParm);

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { parm });

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento);
        }

        [Test]
        public void TestActionObjectCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);

            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject>();
            INakedObject parm = PersistorUtils.CreateAdapter(rawParm);

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { parm });

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento);
        }


        [Test]
        public void TestActionObjectParm() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { targetNo });

            RoundTrip(memento);
            RecoverCollection(target.Action3(target), memento);
        }

        [Test]
        public void TestActionObjectParmNull() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new INakedObject[] { null });

            RoundTrip(memento);
            RecoverCollection(target.Action3(null), memento);
        }

        [Test]
        public void TestActionValueCollectionParm() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int> {1, 2};
            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { PersistorUtils.CreateAdapter(rawParm) });

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento);
        }

        [Test]
        public void TestActionValueCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int>();
            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { PersistorUtils.CreateAdapter(rawParm) });

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento);
        }

        [Test]
        public void TestActionValueCollectionParmString() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action7");

            var rawParm = new List<string> {"1", "2"};
            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { PersistorUtils.CreateAdapter(rawParm) });

            RoundTrip(memento);
            RecoverCollection(target.Action7(rawParm), memento);
        }

        [Test]
        public void TestActionValueParm() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action2");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { PersistorUtils.CreateAdapter(1) });

            RoundTrip(memento);
            RecoverCollection(target.Action2(1), memento);
        }

        [Test]
        public void TestActionValueParmString() {
            TestDomainObject target = NakedObjectsContext.ObjectPersistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = PersistorUtils.CreateAdapter(target);
            INakedObjectAction action = targetNo.Specification.GetObjectActions().Single(a => a.Id == "Action6");

            var memento = new CollectionMemento(NakedObjectsContext.ObjectPersistor, targetNo, action, new[] { PersistorUtils.CreateAdapter("1") });

            RoundTrip(memento);
            RecoverCollection(target.Action6("1"), memento);
        }
    }
}