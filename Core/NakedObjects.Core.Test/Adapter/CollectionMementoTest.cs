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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

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

            return new TestDomainObject[] { };
        }

        // ReSharper disable PossibleMultipleEnumeration

        public IQueryable<TestDomainObject> Action4(IEnumerable<int> filter) {
            return from tdo in Container.Instances<TestDomainObject>()
                from ids in filter
                where tdo.Id != ids
                select tdo;
        }

        // ReSharper restore PossibleMultipleEnumeration

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
        public TestContext(string name) : base(name) { }
        public DbSet<TestDomainObject> TestDomainObjects { get; set; }
    }

    public class TestDataFixture {
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
            NewTdo(1);
            NewTdo(2);
            NewTdo(3);
        }

        private void NewTdo(int id) {
            var tdo = Container.NewTransientInstance<TestDomainObject>();
            tdo.Id = id;

            Container.Persist(ref tdo);
        }
    }

    [TestFixture]
    public class CollectionMementoTest : AcceptanceTestCase {
        protected override string[] Namespaces {
            get { return new[] {typeof(TestDomainObject).Namespace}; }
        }

        protected override Type[] Types {
            get { return new[] {typeof(TestDomainObject), typeof(TestDomainObject[]), typeof(List<TestDomainObject>), typeof(ObjectQuery<TestDomainObject>), typeof(List<Int32>)}; }
        }

        protected override object[] Fixtures {
            get { return new object[] {new TestDataFixture()}; }
        }

        protected override object[] MenuServices {
            get { return new object[] {new SimpleRepository<TestDomainObject>()}; }
        }

        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            RunFixtures();
            StartTest();
        }

        #endregion

        protected override void RegisterTypes(IServiceCollection container) {
            base.RegisterTypes(container);
            // replace INakedObjectStore types
            var c = new EntityObjectStoreConfiguration();
            c.UsingCodeFirstContext(() => {
                var cs = GetConfiguredContainer().GetService<IConfiguration>().GetConnectionString("TestContext");
                return new TestContext(cs);
            });
            container.AddSingleton<IEntityObjectStoreConfiguration>(c);

            var types = new[] {typeof(TestDomainObject[]), typeof(List<TestDomainObject>), typeof(ObjectQuery<TestDomainObject>), typeof(List<Int32>)};
            var ms = new[] {typeof(SimpleRepository<TestDomainObject>)};
            var ns = new[] {typeof(TestDomainObject).Namespace};
            var rc = new ReflectorConfiguration(types, ms, ns);
            container.AddSingleton<IReflectorConfiguration>(rc);
        }

        protected override IDictionary<string, string> Configuration() {
            var config = base.Configuration();
            config["ConnectionStrings:TestContext"] = @"Server=(localdb);Initial Catalog=CodeSystemTest;Integrated Security=True;";
            return config;
        }

        [OneTimeSetUp]
        public void SetupFixture() {
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void TearDownFixture() {
            CleanupNakedObjectsFramework(this);
        }

        private void RoundTrip(CollectionMemento memento) {
            string[] strings1 = memento.ToEncodedStrings();
            var newMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, strings1);
            string[] strings2 = newMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings2), "memento failed roundtrip");

            var copyMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, memento, new object[] { });
            string[] strings3 = copyMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings3), "memento failed copy");
        }

        private void RecoverCollection(IEnumerable<TestDomainObject> originalCollection, CollectionMemento memento, INakedObjectManager manager) {
            IEnumerable<TestDomainObject> recoveredCollection = memento.RecoverCollection().GetAsEnumerable(manager).Select(AdapterUtils.GetDomainObject<TestDomainObject>);
            List<TestDomainObject> oc = originalCollection.ToList();
            List<TestDomainObject> rc = recoveredCollection.ToList();

            Assert.IsTrue(oc.SequenceEqual(rc), "recovered collection not same as original");
        }

        [Test]
        public void TestActionNoParms() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new INakedObjectAdapter[] { });
            RoundTrip(memento);
            RecoverCollection(target.Action1(), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionNoParmsTransient() {
            INakedObjectAdapter targetNo = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof(TestDomainObject)));

            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new INakedObjectAdapter[] { });
            RoundTrip(memento);
            RecoverCollection(targetNo.GetDomainObject<TestDomainObject>().Action1(), memento, NakedObjectsFramework.NakedObjectManager);
        }

        // ReSharper disable PossibleMultipleEnumeration

        [Test]
        public void TestActionNoParmsWithSelected() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new INakedObjectAdapter[] { });

            var selectedMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, memento, new object[] {target});

            RoundTrip(selectedMemento);
            IEnumerable<TestDomainObject> recoveredCollection = selectedMemento.RecoverCollection().GetAsEnumerable(NakedObjectsFramework.NakedObjectManager).Select(AdapterUtils.GetDomainObject<TestDomainObject>);
            Assert.IsFalse(target.Action1().SequenceEqual(recoveredCollection), "recovered selected collection same as original");

            IEnumerable<TestDomainObject> selectedCollection = target.Action1().Where(tdo => tdo.Id == target.Id);

            Assert.IsTrue(selectedCollection.SequenceEqual(recoveredCollection), "recovered selected collection not same as original selected collection");
        }

        // ReSharper restore PossibleMultipleEnumeration

        [Test]
        public void TestActionObjectCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);

            TestDomainObject obj2 = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 2);

            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject> {target, obj2};
            INakedObjectAdapter parm = NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);

            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject>();
            INakedObjectAdapter parm = NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null);

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {targetNo});

            RoundTrip(memento);
            RecoverCollection(target.Action3(target), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectParmNull() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action3");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new INakedObjectAdapter[] {null});

            RoundTrip(memento);
            RecoverCollection(target.Action3(null), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int> {1, 2};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParmEmpty() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action4");

            var rawParm = new List<int>();
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParmString() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action7");

            var rawParm = new List<string> {"1", "2"};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action7(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueParm() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action2");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(1, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action2(1), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueParmString() {
            TestDomainObject target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            INakedObjectAdapter targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            IActionSpec actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action6");

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter("1", null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action6("1"), memento, NakedObjectsFramework.NakedObjectManager);
        }
    }
}