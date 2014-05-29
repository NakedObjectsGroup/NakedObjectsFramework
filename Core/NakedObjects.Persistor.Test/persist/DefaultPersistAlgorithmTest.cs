// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks;
using NakedObjects.Testing;
using NakedObjects.Testing.Dom;
using NUnit.Framework;

namespace NakedObjects.Persistor.Persist {
    /// <summary>
    /// Summary description for DefaultPersistAlgorithmTest
    /// </summary>
    [TestFixture]
    public class DefaultPersistAlgorithmTest  {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            system = new ProgrammableTestSystem();
        
            Person person = new Person();
            Role role = new Role();
            role.Person = person;

            personAdapter = system.AdapterFor(person);
            roleAdapter = system.AdapterFor(role);

            adder = new PersistedObjectAdderSpy();
            algorithm = new DefaultPersistAlgorithm();

            Assert.IsFalse(roleAdapter.ResolveState.IsResolved());
            Assert.That(adder.PersistedObjects.Count, Is.EqualTo(0));

            roleAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(roleAdapter.Specification));
            roleAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(roleAdapter.Specification));
            roleAdapter.Specification.AddFacet(new LoadingCallbackFacetNull(roleAdapter.Specification));
            roleAdapter.Specification.AddFacet(new LoadedCallbackFacetNull(roleAdapter.Specification));
            personAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(personAdapter.Specification));
            personAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(personAdapter.Specification));
            personAdapter.Specification.AddFacet(new LoadingCallbackFacetNull(personAdapter.Specification));
            personAdapter.Specification.AddFacet(new LoadedCallbackFacetNull(personAdapter.Specification));
        }

        #endregion

        private PersistedObjectAdderSpy adder;
        private DefaultPersistAlgorithm algorithm;
        private INakedObject roleAdapter;
        private INakedObject personAdapter;
        private ProgrammableTestSystem system;
        /*
        private class DefaultPersistAlgorithmSubclassForTesting : DefaultPersistAlgorithm {
            protected new void Persist(INakedObject nakedObject, IPersistedObjectAdder persistor) {
                base.Persist(nakedObject, persistor);
            }

            public void SensingPersist(INakedObject nakedObject, IPersistedObjectAdder persistor) {
                Persist(nakedObject, persistor);
            }
        }
*/
        private class PersistedObjectAdderSpy : IPersistedObjectAdder {
            private readonly IList<INakedObject> persistedObjects = new List<INakedObject>();

            public IList<INakedObject> PersistedObjects {
                get { return persistedObjects; }
            }

            public int PersistedCount {
                get { return persistedObjects.Count; }
            }

            #region IPersistedObjectAdder Members

            public void AddPersistedObject(INakedObject nakedObject) {
                persistedObjects.Add(nakedObject);
            }

            public void MadePersistent(INakedObject nakedObject) {
                if (nakedObject != null) {
                    nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                    nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
                }
            }

            #endregion

            public void Reset() {
                persistedObjects.Clear();
            }
        }

        [Test]
        public void TestMakePersistent() {
            algorithm.MakePersistent(roleAdapter, adder);
            Assert.IsTrue(roleAdapter.ResolveState.IsResolved());
            Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
            Assert.IsTrue(adder.PersistedObjects.Contains(personAdapter));
            Assert.That(adder.PersistedCount, Is.EqualTo(2));
        }

        [Test]
        public void TestMakePersistentFailsIfObjectAlreadyPersistent() {
            roleAdapter.ResolveState.Handle(Events.StartResolvingEvent);
            roleAdapter.ResolveState.Handle(Events.EndResolvingEvent);

            try {
                algorithm.MakePersistent(roleAdapter, adder);
                Assert.Fail();
            }
            catch (NotPersistableException /*expected*/) {}
        }

        [Test]
        public void TestMakePersistentFailsIfObjectMustBeTransient() {
            roleAdapter.Specification.AddFacet(new PersistingCallbackFacetNull(roleAdapter.Specification));
            roleAdapter.Specification.AddFacet(new PersistedCallbackFacetNull(roleAdapter.Specification));


            try {
                ((ProgrammableSpecification) roleAdapter.Specification).SetUpPersistable(Persistable.TRANSIENT);
                algorithm.MakePersistent(roleAdapter, adder);
                Assert.Fail();
            }
            catch (NotPersistableException /*expected*/) {}
        }

        [Test]
        public void TestMakePersistentSkipsAlreadyPersistedObjects() {
            algorithm.MakePersistent(personAdapter, adder);
            adder.Reset();

            algorithm.MakePersistent(roleAdapter, adder);

            Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
            Assert.That(adder.PersistedCount, Is.EqualTo(1));
        }

        [Test]
        public void TestMakePersistentSkipsAggregatedObjects() {
            Person person = new Person();
            system.AdapterFor(person, Events.InitializeAggregateEvent);
            ((Role)roleAdapter.GetDomainObject()).Person = person;

            algorithm.MakePersistent(roleAdapter, adder);

            Assert.IsTrue(adder.PersistedObjects.Contains(roleAdapter));
            Assert.That(adder.PersistedCount, Is.EqualTo(1));
            Assert.IsFalse(adder.PersistedObjects.Contains(personAdapter));
        }
    }
}