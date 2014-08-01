// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Adapter.Map;
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

            var inMemoryObjectStore = new MemoryObjectStore();
            var persistor = new ObjectStorePersistor {
                ObjectStore = inMemoryObjectStore,
                PersistAlgorithm = new DefaultPersistAlgorithm(),
                OidGenerator = SimpleOidGeneratorStart.HasValue ? new SimpleOidGenerator(SimpleOidGeneratorStart.Value) : new TimeBasedOidGenerator()
            };


            var identityMapImpl = new IdentityMapImpl(persistor, identityAdapterMap ?? new IdentityAdapterHashMap(), new CreateIfNullPocoAdapterDecorator(inMemoryObjectStore, pocoAdapterMap ?? new PocoAdapterHashMap()));
            inMemoryObjectStore.IdentityMap = identityMapImpl;
            persistor.IdentityMap = identityMapImpl;
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
                return decorated.GetObject(obj) ?? objectStore.CreateAdapter(obj);
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