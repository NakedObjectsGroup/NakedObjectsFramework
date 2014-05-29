// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof;
using NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks;

namespace NakedObjects.TestSystem {
    public class TestProxySystem {
        private readonly TestProxyReflector reflector;
        private NakedObjectsContext nakedObjects;
        private int nextId = 1;

        public TestProxySystem() {
            reflector = new TestProxyReflector();
            Persistor = new TestProxyPersistor();
        }

        private INakedObjectPersistor Persistor { get; set; }

        public TestProxyNakedObject CreateAdapterForTransient(object associate) {
            var testProxyNakedObject = new TestProxyNakedObject();
            testProxyNakedObject.SetupObject(associate);
            testProxyNakedObject.SetupSpecification(GetSpecification(associate.GetType()));
            testProxyNakedObject.ResolveState.Handle(Events.InitializeTransientEvent);
            testProxyNakedObject.SetupOid(new TestProxyOid(nextId++));
            return testProxyNakedObject;
        }

        public void Init() {
            nakedObjects = StaticContext.CreateInstance();

            nakedObjects.SetReflector(reflector);
            nakedObjects.SetObjectPersistor(Persistor);
            nakedObjects.SetSession(new TestProxySession());

            NakedObjectsContext.Reflector.Init();
            NakedObjectsContext.ObjectPersistor.Init();
        }

        public void Shutdown() {
            NakedObjectsContext.Reflector.Shutdown();
            NakedObjectsContext.ObjectPersistor.Shutdown();
        }

        public void ResetLoader() {
            Persistor.Reset();
        }

        public INakedObject CreatePersistentTestObject() {
            var pojo = new TestPojo();
            return CreatePersistentTestObject(pojo);
        }

        public INakedObject CreatePersistentTestObject(object domainObject) {
            // TODO should be done generically via:
            INakedObject adapter = CreateTransientTestObject(domainObject);
            adapter.Specification.AddFacet(new LoadingCallbackFacetNull(adapter.Specification));
            adapter.Specification.AddFacet(new LoadedCallbackFacetNull(adapter.Specification));
            Persistor.MakePersistent(adapter);

            return adapter;
        }

        public void MakePersistent(TestProxyNakedObject adapter) {
            IOid oid = adapter.Oid;
            Persistor.ConvertTransientToPersistentOid(oid);
            adapter.SetupOid(oid);
            Persistor.MakePersistent(adapter);
        }

        public INakedObject CreateTransientTestObject() {
            var pojo = new TestPojo();
            return CreateTransientTestObject(pojo);
        }

        public INakedObject CreateTransientTestObject(object domainObject) {
            var oid = new TestProxyOid(nextId++, false);
            INakedObject adapterFor = Persistor.CreateAdapter(domainObject, oid, null);

            adapterFor.ResolveState.Handle(Events.InitializeTransientEvent);


            return adapterFor;
        }

        public TestProxySpecification GetSpecification(Type type) {
            return (TestProxySpecification) reflector.LoadSpecification(type);
        }

        public void SetObjectPersistor(INakedObjectPersistor persistor) {
            Persistor = persistor;
            if (nakedObjects != null) {
                nakedObjects.SetObjectPersistor(Persistor);
            }
        }

        public TestProxyNakedCollection CreatePersistentTestCollection() {
            var collection = new TestProxyNakedCollection(typeof (ArrayList));
            TestProxySpecification specification = GetSpecification(typeof (ArrayList));
            TestProxySpecification elementSpecification = GetSpecification(typeof (object));
            specification.AddFacet(new TestProxyCollectionFacet(null));
            specification.AddFacet(new TypeOfFacetInferredFromArray(typeof (object), elementSpecification, reflector));
            collection.SetupSpecification(specification);
            return collection;
        }

        public TestProxySpecification GetSpecification(INakedObject nakedObject) {
            return (TestProxySpecification) nakedObject.Specification;
        }

        public void AddSpecification(INakedObjectSpecification specification) {
            reflector.AddSpecification(specification);
        }

        public void AddConfiguration(string name, String value) {
            // configuration.Add(name, value);
        }

        public INakedObject CreateValueAdapter(object domainObject) {
            return Persistor.CreateAdapter(domainObject, null, null);
        }
    }
}