// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;

namespace NakedObjects.Persistor.Objectstore.Inmemory {
    /// <summary>
    ///     Installs the in-memory object store
    /// </summary>
    public class InMemoryObjectPersistorInstaller : AbstractObjectPersistorInstaller {
        private static readonly ILog Log;

        static InMemoryObjectPersistorInstaller() {
            Log = LogManager.GetLogger(typeof (InMemoryObjectPersistorInstaller));
        }

        public override string Name {
            get { return "in-memory"; }
        }

        public int? SimpleOidGeneratorStart { get; set; }

        public override INakedObjectPersistor CreateObjectPersistor() {
            Log.Info("installing " + GetType().FullName);

            var reflector = NakedObjectsContext.Reflector;
            var inMemoryObjectStore = new MemoryObjectStore(reflector);
            var oidGenerator = SimpleOidGeneratorStart.HasValue ? new SimpleOidGenerator(reflector, SimpleOidGeneratorStart.Value) : new TimeBasedOidGenerator(reflector);

            var identityMapImpl = new IdentityMapImpl(
                oidGenerator,
                identityAdapterMap ?? new IdentityAdapterHashMap(),
                new CreateIfNullPocoAdapterDecorator(inMemoryObjectStore, pocoAdapterMap ?? new PocoAdapterHashMap()));

            var persistor = new ObjectStorePersistor(
                reflector,  
                inMemoryObjectStore,
                new DefaultPersistAlgorithm(),
                oidGenerator,
                identityMapImpl);

            inMemoryObjectStore.IdentityMap = identityMapImpl;
            inMemoryObjectStore.Manager = persistor;

            return persistor;
        }

        #region Nested type: CreateIfNullPocoAdapterDecorator

        private class CreateIfNullPocoAdapterDecorator : IPocoAdapterMap {
            private readonly IPocoAdapterMap decorated;
            private readonly MemoryObjectStore objectStore;

            public CreateIfNullPocoAdapterDecorator(MemoryObjectStore objectStore, IPocoAdapterMap decorated) {
                this.objectStore = objectStore;
                this.decorated = decorated;
            }

            #region IPocoAdapterMap Members

            public IEnumerator<INakedObject> GetEnumerator() {
                return decorated.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public void Add(object obj, INakedObject adapter) {
                decorated.Add(obj, adapter);
            }

            public bool ContainsObject(object obj) {
                return decorated.ContainsObject(obj);
            }

            public INakedObject GetObject(object obj) {
                return decorated.GetObject(obj) ?? objectStore.CreateAdapter(obj, NakedObjectsContext.Session);
            }

            public void Reset() {
                decorated.Reset();
            }

            public void Shutdown() {
                decorated.Shutdown();
            }

            public void Remove(INakedObject nakedObject) {
                decorated.Remove(nakedObject);
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}