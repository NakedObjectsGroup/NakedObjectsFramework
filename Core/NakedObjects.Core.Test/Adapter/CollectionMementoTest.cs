// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Xat;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Core.Test.Adapter {
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
        public TestContext(string name) : base(name) {}
        public DbSet<TestDomainObject> TestDomainObjects { get; set; }
    }


    public class TestDataFixture {
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
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


    [TestClass, Ignore]
    public class CollectionMementoTest : AcceptanceTestCase {
        #region Setup/Teardown

        [TestInitialize]
        public void Setup() {
            InitializeNakedObjectsFrameworkOnceOnly();
            RunFixturesOnce();
            StartTest();
        }

        #endregion

        private static bool runFixtures;

        private void RunFixturesOnce() {
            if (!runFixtures) {
                runFixtures = true;             
                RunFixtures();
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            // replace INakedObjectStore types
            var c = new EntityObjectStoreConfiguration();
            c.UsingCodeFirstContext(() => new TestContext("TestContext"));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(c, (new ContainerControlledLifetimeManager()));

            //var types = new Type[] { typeof(TestDomainObject), typeof(TestDomainObject[]), typeof(List<TestDomainObject>), typeof(ObjectQuery<TestDomainObject>), typeof(List<Int32>) };
            //var ms = new[] {typeof (SimpleRepository<TestDomainObject>)};
            //var rc = new ReflectorConfiguration(types, ms, new Type[] {}, new Type[] {});

            //container.RegisterInstance<IReflectorConfiguration>(rc, (new ContainerControlledLifetimeManager()));
        }

        protected override Type[] Types {
            get { return new[] {typeof (TestDomainObject), typeof (TestDomainObject[]), typeof (List<TestDomainObject>), typeof (ObjectQuery<TestDomainObject>), typeof (List<Int32>)}; }
        }

        protected override object[] Fixtures {
            get { return new object[] {new TestDataFixture()}; }
        }

        protected override object[] MenuServices {
            get { return new object[] {new SimpleRepository<TestDomainObject>()}; }
        }

        private void RoundTrip(CollectionMemento memento) {
            string[] strings1 = memento.ToEncodedStrings();
            var newMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, strings1);
            string[] strings2 = newMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings2), "memento failed roundtrip");

            var copyMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, memento, new object[] {});
            string[] strings3 = copyMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings3), "memento failed copy");
        }

        private void RecoverCollection(IEnumerable<TestDomainObject> originalCollection, CollectionMemento memento, INakedObjectManager manager) {
            IEnumerable<TestDomainObject> recoveredCollection = AdapterUtils.GetAsEnumerable(memento.RecoverCollection(), manager).Select(no => AdapterUtils.GetDomainObject<TestDomainObject>(no));
            var oc = originalCollection.ToList();
            var rc = recoveredCollection.ToList();
            
            Assert.IsTrue(oc.SequenceEqual(rc), "recovered collection not same as original");
        }

        [TestMethod]
        public void TestActionNoParms() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new INakedObject[] {});
            RoundTrip(memento);
            RecoverCollection(target.Action1(), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionNoParmsTransient() {
            var targetNo = NakedObjectsFramework.LifecycleManager.CreateInstance(NakedObjectsFramework.Metamodel.GetSpecification(typeof (TestDomainObject)));

            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new INakedObject[] {});
            RoundTrip(memento);
            RecoverCollection(AdapterUtils.GetDomainObject<TestDomainObject>(targetNo).Action1(), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionNoParmsWithSelected() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new INakedObject[] {});

            var selectedMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, memento, new[] {target});

            RoundTrip(selectedMemento);
            IEnumerable<TestDomainObject> recoveredCollection = AdapterUtils.GetAsEnumerable(selectedMemento.RecoverCollection(), NakedObjectsFramework.Manager).Select(no => AdapterUtils.GetDomainObject<TestDomainObject>(no));
            Assert.IsFalse(target.Action1().SequenceEqual(recoveredCollection), "recovered selected collection same as original");

            IEnumerable<TestDomainObject> selectedCollection = target.Action1().Where(tdo => tdo.Id == target.Id);

            Assert.IsTrue(selectedCollection.SequenceEqual(recoveredCollection), "recovered selected collection not same as original selected collection");
        }


        [TestMethod] 
        public void TestActionObjectCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);

            TestDomainObject obj2 = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 2);

            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject> {target, obj2};
            INakedObject parm = NakedObjectsFramework.Manager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionObjectCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);

            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject>();
            INakedObject parm = NakedObjectsFramework.Manager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.Manager);
        }


        [TestMethod]
        public void TestActionObjectParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {targetNo});

            RoundTrip(memento);
            RecoverCollection(target.Action3(target), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionObjectParmNull() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new INakedObject[] {null});

            RoundTrip(memento);
            RecoverCollection(target.Action3(null), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod] 
        public void TestActionValueCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int> {1, 2};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {NakedObjectsFramework.Manager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod] 
        public void TestActionValueCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int>();
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {NakedObjectsFramework.Manager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod] 
        public void TestActionValueCollectionParmString() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action7");

            var rawParm = new List<string> {"1", "2"};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {NakedObjectsFramework.Manager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action7(rawParm), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionValueParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action2");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {NakedObjectsFramework.Manager.CreateAdapter(1, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action2(1), memento, NakedObjectsFramework.Manager);
        }

        [TestMethod]
        public void TestActionValueParmString() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObject targetNo = NakedObjectsFramework.Manager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetAllActions().Single(a => a.Id == "Action6");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Manager, NakedObjectsFramework.Metamodel, targetNo, actionSpec, new[] {NakedObjectsFramework.Manager.CreateAdapter("1", null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action6("1"), memento, NakedObjectsFramework.Manager);
        }
    }
}