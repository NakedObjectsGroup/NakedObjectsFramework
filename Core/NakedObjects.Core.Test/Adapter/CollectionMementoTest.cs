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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.Core.Test.Adapter {
    public class TestDomainObject {
        public IDomainObjectContainer Container { protected get; set; }

        [Key]
        public virtual int Id { get; set; }

        public ICollection<TestDomainObject> Action1() => Container.Instances<TestDomainObject>().ToList();

        public ICollection<TestDomainObject> Action2(int filter) => Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filter).ToList();

        public ICollection<TestDomainObject> Action3(TestDomainObject filter) {
            if (filter != null) {
                return Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filter.Id).ToList();
            }

            return new TestDomainObject[] { };
        }

        // ReSharper disable PossibleMultipleEnumeration

        public IQueryable<TestDomainObject> Action4(IEnumerable<int> filter) =>
            from tdo in Container.Instances<TestDomainObject>()
            from ids in filter
            where tdo.Id != ids
            select tdo;

        // ReSharper restore PossibleMultipleEnumeration

        public IQueryable<TestDomainObject> Action5(IEnumerable<TestDomainObject> filter) {
            var idsToFilter = filter.Select(tdo => tdo.Id);
            return Action4(idsToFilter);
        }

        public ICollection<TestDomainObject> Action6(string filter) {
            var filterInt = int.Parse(filter);
            return Container.Instances<TestDomainObject>().Where(tdo => tdo.Id != filterInt).ToList();
        }

        public IQueryable<TestDomainObject> Action7(IEnumerable<string> filter) {
            var idsToFilter = filter.Select(int.Parse);
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
        protected override Type[] ObjectTypes => new[] {
            typeof(TestDomainObject)
        };

        protected override object[] Fixtures => new object[] {new TestDataFixture()};

        protected override Type[] Services => new[] {typeof(SimpleRepository<TestDomainObject>)};

        protected override EntityObjectStoreConfiguration Persistor {
            get {
                var c = new EntityObjectStoreConfiguration();
                c.UsingContext(() => {
                    var cs = RootServiceProvider.GetService<IConfiguration>().GetConnectionString("TestContext");
                    return new TestContext(cs);
                });
                return c;
            }
        }

        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            RunFixtures();
            StartTest();
        }

        #endregion

        protected override IDictionary<string, string> Configuration() {
            var config = base.Configuration();
            config["ConnectionStrings:TestContext"] = @"Server=(localdb)\MSSQLLocalDB;Initial Catalog=CodeSystemTest;Integrated Security=True;";
            return config;
        }

        [OneTimeSetUp]
        public void SetupFixture() => InitializeNakedObjectsFramework(this);

        [OneTimeTearDown]
        public void TearDownFixture() => CleanupNakedObjectsFramework(this);

        private void RoundTrip(CollectionMemento memento) {
            var strings1 = memento.ToEncodedStrings();
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();
            var newMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, LoggerFactory, logger, strings1);
            var strings2 = newMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings2), "memento failed roundtrip");

            var copyMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, memento, new object[] { });
            var strings3 = copyMemento.ToEncodedStrings();
            Assert.IsTrue(strings1.SequenceEqual(strings3), "memento failed copy");
        }

        private static void RecoverCollection(IEnumerable<TestDomainObject> originalCollection, CollectionMemento memento, INakedObjectManager manager) {
            var recoveredCollection = memento.RecoverCollection().GetAsEnumerable(manager).Select(AdapterUtils.GetDomainObject<TestDomainObject>);
            var oc = originalCollection.ToList();
            var rc = recoveredCollection.ToList();

            Assert.IsTrue(oc.SequenceEqual(rc), "recovered collection not same as original");
        }

        [Test]
        public void TestActionNoParms() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new INakedObjectAdapter[] { });
            RoundTrip(memento);
            RecoverCollection(target.Action1(), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionNoParmsTransient() {
            var targetNo = NakedObjectsFramework.LifecycleManager.CreateInstance((IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(typeof(TestDomainObject)));
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new INakedObjectAdapter[] { });
            RoundTrip(memento);
            RecoverCollection(targetNo.GetDomainObject<TestDomainObject>().Action1(), memento, NakedObjectsFramework.NakedObjectManager);
        }

        // ReSharper disable PossibleMultipleEnumeration

        [Test]
        public void TestActionNoParmsWithSelected() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action1");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new INakedObjectAdapter[] { });

            var selectedMemento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, memento, new object[] {target});

            RoundTrip(selectedMemento);
            var recoveredCollection = selectedMemento.RecoverCollection().GetAsEnumerable(NakedObjectsFramework.NakedObjectManager).Select(AdapterUtils.GetDomainObject<TestDomainObject>);
            Assert.IsFalse(target.Action1().SequenceEqual(recoveredCollection), "recovered selected collection same as original");

            var selectedCollection = target.Action1().Where(tdo => tdo.Id == target.Id);

            Assert.IsTrue(selectedCollection.SequenceEqual(recoveredCollection), "recovered selected collection not same as original selected collection");
        }

        // ReSharper restore PossibleMultipleEnumeration

        [Test]
        public void TestActionObjectCollectionParm() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);

            var obj2 = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 2);

            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject> {target, obj2};
            var parm = NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null);
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectCollectionParmEmpty() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);

            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action5");

            var rawParm = new List<TestDomainObject>();
            var parm = NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null);
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {parm});

            RoundTrip(memento);
            RecoverCollection(target.Action5(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectParm() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action3");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {targetNo});

            RoundTrip(memento);
            RecoverCollection(target.Action3(target), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionObjectParmNull() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action3");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new INakedObjectAdapter[] {null});

            RoundTrip(memento);
            RecoverCollection(target.Action3(null), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParm() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action4");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();
            var rawParm = new List<int> {1, 2};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParmEmpty() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action4");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var rawParm = new List<int>();
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action4(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueCollectionParmString() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action7");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var rawParm = new List<string> {"1", "2"};
            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(rawParm, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action7(rawParm), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueParm() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action2");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter(1, null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action2(1), memento, NakedObjectsFramework.NakedObjectManager);
        }

        [Test]
        public void TestActionValueParmString() {
            var target = NakedObjectsFramework.Persistor.Instances<TestDomainObject>().Single(i => i.Id == 1);
            var targetNo = NakedObjectsFramework.NakedObjectManager.CreateAdapter(target, null, null);
            var actionSpec = targetNo.Spec.GetActions().Single(a => a.Id == "Action6");
            var logger = LoggerFactory.CreateLogger<CollectionMemento>();

            var memento = new CollectionMemento(NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.MetamodelManager, logger, targetNo, actionSpec, new[] {NakedObjectsFramework.NakedObjectManager.CreateAdapter("1", null, null)});

            RoundTrip(memento);
            RecoverCollection(target.Action6("1"), memento, NakedObjectsFramework.NakedObjectManager);
        }
    }
}