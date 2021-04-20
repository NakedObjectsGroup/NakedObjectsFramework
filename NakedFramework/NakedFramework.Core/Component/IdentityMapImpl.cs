// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Component {
    public sealed class IdentityMapImpl : IIdentityMap {
        private readonly IIdentityAdapterMap identityAdapterMap;
        private readonly ILogger<IdentityMapImpl> logger;
        private readonly INakedObjectAdapterMap nakedObjectAdapterMap;
        private readonly IOidGenerator oidGenerator;
        private readonly IDictionary<object, object> unloadedObjects = new Dictionary<object, object>();

        public IdentityMapImpl(IOidGenerator oidGenerator,
                               IIdentityAdapterMap identityAdapterMap,
                               INakedObjectAdapterMap nakedObjectAdapterMap,
                               ILogger<IdentityMapImpl> logger) {
            this.oidGenerator = oidGenerator ?? throw new InitialisationException($"{nameof(oidGenerator)} is null");
            this.identityAdapterMap = identityAdapterMap ?? throw new InitialisationException($"{nameof(identityAdapterMap)} is null");
            this.nakedObjectAdapterMap = nakedObjectAdapterMap ?? throw new InitialisationException($"{nameof(nakedObjectAdapterMap)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
        }

        private void ValidateAndAdd(INakedObjectAdapter adapter, IOid oid) {
            if (nakedObjectAdapterMap.GetObject(adapter.Object) != adapter) {
                throw new NakedObjectSystemException("Adapter's poco should exist in poco map and return the adapter");
            }

            if (identityAdapterMap.GetAdapter(oid) is not null) {
                throw new NakedObjectSystemException($"Changed OID should not already map to a known adapter {oid}");
            }

            identityAdapterMap.Add(oid, adapter);
        }

        /// <summary>
        ///     Given a new Oid (not from the adapter, but usually a reference during distribution) this method
        ///     extracts the original Oid, find the associated adapter and then updates the lookup so that the new Oid
        ///     now keys the adapter. The adapter's oid is then updated to take on the new Oid's identity.
        /// </summary>
        private void ProcessChangedOid(IOid updatedOid) {
            if (updatedOid.HasPrevious) {
                var previousOid = updatedOid.Previous;
                var nakedObjectAdapter = identityAdapterMap.GetAdapter(previousOid);
                if (nakedObjectAdapter is not null) {
                    identityAdapterMap.Remove(previousOid);
                    var oidFromObject = nakedObjectAdapter.Oid;
                    oidFromObject.CopyFrom(updatedOid);
                    identityAdapterMap.Add(oidFromObject, nakedObjectAdapter);
                }
            }
        }

        #region IIdentityMap Members

        public IEnumerator<INakedObjectAdapter> GetEnumerator() => nakedObjectAdapterMap.GetEnumerator();

        public void Reset() {
            identityAdapterMap.Reset();
            nakedObjectAdapterMap.Reset();
            unloadedObjects.Clear();
        }

        public void AddAdapter(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter is null) {
                throw new NakedObjectSystemException("Cannot add null adapter to IdentityAdapterMap");
            }

            var obj = nakedObjectAdapter.Object;
            if (nakedObjectAdapterMap.ContainsObject(obj)) {
                throw new NakedObjectSystemException("POCO Map already contains object");
            }

            if (unloadedObjects.ContainsKey(obj)) {
                var msg = string.Format(NakedObjects.Resources.NakedObjects.TransientReferenceMessage, obj);
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

            ValidateAndAdd(adapter, oid);
        }

        public void UpdateViewModel(INakedObjectAdapter adapter, string[] keys) {
            var oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            identityAdapterMap.Remove(oid);
            ((ViewModelOid) adapter.Oid).UpdateKeys(keys, false);
            ValidateAndAdd(adapter, oid);
        }

        public void Unloaded(INakedObjectAdapter nakedObjectAdapter) {
            // If an object is unloaded while its poco still exist then accessing that poco via the reflector will
            // create a different NakedObjectAdapter and no OID will exist to identify - hence the adapter will appear as
            // transient and will no longer be usable as a persistent object

            var oid = nakedObjectAdapter.Oid;
            if (oid is not null) {
                identityAdapterMap.Remove(oid);
            }

            nakedObjectAdapterMap.Remove(nakedObjectAdapter);
        }

        public INakedObjectAdapter GetAdapterFor(object domainObject) {
            if (domainObject is null) {
                throw new NakedObjectSystemException("can't get an adapter for null");
            }

            return nakedObjectAdapterMap.GetObject(domainObject);
        }

        public INakedObjectAdapter GetAdapterFor(IOid oid) {
            if (oid is null) {
                throw new NakedObjectSystemException("OID should not be null");
            }

            ProcessChangedOid(oid);
            return identityAdapterMap.GetAdapter(oid);
        }

        public bool IsIdentityKnown(IOid oid) {
            if (oid is null) {
                throw new NakedObjectSystemException("OID should not be null");
            }

            ProcessChangedOid(oid);
            return identityAdapterMap.IsIdentityKnown(oid);
        }

        public void Replaced(object domainObject) {
            unloadedObjects[domainObject] = domainObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}