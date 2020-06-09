// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class IdentityMapImpl : IIdentityMap {
        private readonly IIdentityAdapterMap identityAdapterMap;
        private readonly INakedObjectAdapterMap nakedObjectAdapterMap;
        private readonly IOidGenerator oidGenerator;
        private readonly IDictionary<object, object> unloadedObjects = new Dictionary<object, object>();
        private readonly ILogger<IdentityMapImpl> logger;

        public IdentityMapImpl(IOidGenerator oidGenerator,
                               IIdentityAdapterMap identityAdapterMap,
                               INakedObjectAdapterMap nakedObjectAdapterMap,
                               ILogger<IdentityMapImpl> logger) {
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(identityAdapterMap);
            Assert.AssertNotNull(nakedObjectAdapterMap);

            this.oidGenerator = oidGenerator;
            this.identityAdapterMap = identityAdapterMap;
            this.nakedObjectAdapterMap = nakedObjectAdapterMap;
            this.logger = logger;
        }

        #region IIdentityMap Members

        public IEnumerator<INakedObjectAdapter> GetEnumerator() => nakedObjectAdapterMap.GetEnumerator();

        public void Reset() {
            identityAdapterMap.Reset();
            nakedObjectAdapterMap.Reset();
            unloadedObjects.Clear();
        }

        public void AddAdapter(INakedObjectAdapter nakedObjectAdapter) {
            Assert.AssertNotNull("Cannot add null adapter to IdentityAdapterMap", nakedObjectAdapter);
            var obj = nakedObjectAdapter.Object;
            Assert.AssertFalse("POCO Map already contains object", obj, nakedObjectAdapterMap.ContainsObject(obj));

            if (unloadedObjects.ContainsKey(obj)) {
                var msg = string.Format(Resources.NakedObjects.TransientReferenceMessage, obj);
                throw new TransientReferenceException(logger.LogAndReturn(msg));
            }

            if (nakedObjectAdapter.Spec.IsObject) {
                nakedObjectAdapterMap.Add(obj, nakedObjectAdapter);
            }

            // order is important - add to identity map after poco map 
            identityAdapterMap.Add(nakedObjectAdapter.Oid, nakedObjectAdapter);

            nakedObjectAdapter.LoadAnyComplexTypes();
        }

        public void MadePersistent(INakedObjectAdapter adapter) {
            var oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            identityAdapterMap.Remove(oid);
            oidGenerator.ConvertTransientToPersistentOid(oid);

            adapter.ResolveState.Handle(Events.StartResolvingEvent);
            adapter.ResolveState.Handle(Events.EndResolvingEvent);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", nakedObjectAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, identityAdapterMap.GetAdapter(oid));
            identityAdapterMap.Add(oid, adapter);
        }

        public void UpdateViewModel(INakedObjectAdapter adapter, string[] keys) {
            var oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            identityAdapterMap.Remove(oid);

            ((ViewModelOid) adapter.Oid).UpdateKeys(keys, false);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", nakedObjectAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, identityAdapterMap.GetAdapter(oid));
            identityAdapterMap.Add(oid, adapter);
        }

        public void Unloaded(INakedObjectAdapter nakedObjectAdapter) {
            // If an object is unloaded while its poco still exist then accessing that poco via the reflector will
            // create a different NakedObjectAdapter and no OID will exist to identify - hence the adapter will appear as
            // transient and will no longer be usable as a persistent object

            var oid = nakedObjectAdapter.Oid;
            if (oid != null) {
                identityAdapterMap.Remove(oid);
            }

            nakedObjectAdapterMap.Remove(nakedObjectAdapter);
        }

        public INakedObjectAdapter GetAdapterFor(object domainObject) {
            Assert.AssertNotNull("can't get an adapter for null", this, domainObject);
            return nakedObjectAdapterMap.GetObject(domainObject);
        }

        public INakedObjectAdapter GetAdapterFor(IOid oid) {
            Assert.AssertNotNull("OID should not be null", this, oid);
            ProcessChangedOid(oid);
            return identityAdapterMap.GetAdapter(oid);
        }

        public bool IsIdentityKnown(IOid oid) {
            Assert.AssertNotNull("OID should not be null", oid);
            ProcessChangedOid(oid);
            return identityAdapterMap.IsIdentityKnown(oid);
        }

        public void Replaced(object domainObject) {
            unloadedObjects[domainObject] = domainObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        /// <summary>
        ///     Given a new Oid (not from the adapter, but usually a reference during distribution) this method
        ///     extracts the original Oid, find the associated adapter and then updates the lookup so that the new Oid
        ///     now keys the adapter. The adapter's oid is then updated to take on the new Oid's identity.
        /// </summary>
        private void ProcessChangedOid(IOid updatedOid) {
            if (updatedOid.HasPrevious) {
                var previousOid = updatedOid.Previous;
                var nakedObjectAdapter = identityAdapterMap.GetAdapter(previousOid);
                if (nakedObjectAdapter != null) {
                    identityAdapterMap.Remove(previousOid);
                    var oidFromObject = nakedObjectAdapter.Oid;
                    oidFromObject.CopyFrom(updatedOid);
                    identityAdapterMap.Add(oidFromObject, nakedObjectAdapter);
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}