// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class IdentityMapImpl : IIdentityMap {
        private static readonly ILog Log = LogManager.GetLogger(typeof (IdentityMapImpl));
        private readonly IIdentityAdapterMap identityAdapterMap;
        private readonly IOidGenerator oidGenerator;
        private readonly IPocoAdapterMap pocoAdapterMap;
        private readonly IDictionary<object, object> unloadedObjects = new Dictionary<object, object>();

        public IdentityMapImpl(IOidGenerator oidGenerator, IIdentityAdapterMap identityAdapterMap, IPocoAdapterMap pocoAdapterMap) {
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(identityAdapterMap);
            Assert.AssertNotNull(pocoAdapterMap);

            this.oidGenerator = oidGenerator;
            this.identityAdapterMap = identityAdapterMap;
            this.pocoAdapterMap = pocoAdapterMap;
        }

        #region IIdentityMap Members

        public virtual IEnumerator<INakedObject> GetEnumerator() {
            return pocoAdapterMap.GetEnumerator();
        }

        public virtual void Reset() {
            identityAdapterMap.Reset();
            pocoAdapterMap.Reset();
            unloadedObjects.Clear();
        }

        public virtual void AddAdapter(INakedObject nakedObject) {
            Assert.AssertNotNull("Cannot add null adapter to IdentityAdapterMap", nakedObject);
            object obj = nakedObject.Object;
            Assert.AssertFalse("POCO Map already contains object", obj, pocoAdapterMap.ContainsObject(obj));

            if (unloadedObjects.ContainsKey(obj)) {
                string msg = string.Format(Resources.NakedObjects.TransientReferenceMessage, obj);
                throw new TransientReferenceException(msg);
            }

            // TODO we should be ignoring immutable values object as well
            if (nakedObject.Spec.IsObject) {
                pocoAdapterMap.Add(obj, nakedObject);
            }
            // order is important - add to identity map after poco map 
            identityAdapterMap.Add(nakedObject.Oid, nakedObject);

            // log at end so that if ToString needs adapters they're in maps. 
            Log.DebugFormat("Adding identity for {0}", nakedObject);

            nakedObject.LoadAnyComplexTypes();
        }

        public virtual void MadePersistent(INakedObject adapter) {
            IOid oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            identityAdapterMap.Remove(oid);
            oidGenerator.ConvertTransientToPersistentOid(oid);

            adapter.ResolveState.Handle(Events.StartResolvingEvent);
            adapter.ResolveState.Handle(Events.EndResolvingEvent);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", pocoAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, identityAdapterMap.GetAdapter(oid));
            identityAdapterMap.Add(oid, adapter);
            Log.DebugFormat("Made persistent {0}; was {1}", adapter, oid.Previous);
        }

        public virtual void UpdateViewModel(INakedObject adapter, string[] keys) {
            IOid oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            identityAdapterMap.Remove(oid);

            ((ViewModelOid) adapter.Oid).UpdateKeys(keys, false);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", pocoAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, identityAdapterMap.GetAdapter(oid));
            identityAdapterMap.Add(oid, adapter);
            Log.DebugFormat("UpdateView Model {0}; was {1}", adapter, oid.Previous);
        }

        public virtual void Unloaded(INakedObject nakedObject) {
            Log.DebugFormat("Unload: {0}", nakedObject);

            // TODO need to unload object that are no longer referenced
            // 
            // If an object is unloaded while its poco still exist then accessing that poco via the reflector will
            // create a different PocoAdapter and no OID will exist to identify - hence the adapter will appear as
            // transient and will no longer be usable as a persistent object

            Log.DebugFormat("Removed loaded object {0}", nakedObject);
            IOid oid = nakedObject.Oid;
            if (oid != null) {
                identityAdapterMap.Remove(oid);
            }
            pocoAdapterMap.Remove(nakedObject);
        }

        public virtual INakedObject GetAdapterFor(object domainObject) {
            Assert.AssertNotNull("can't get an adapter for null", this, domainObject);
            return pocoAdapterMap.GetObject(domainObject);
        }

        public virtual INakedObject GetAdapterFor(IOid oid) {
            Assert.AssertNotNull("OID should not be null", this, oid);
            ProcessChangedOid(oid);
            return identityAdapterMap.GetAdapter(oid);
        }

        public virtual bool IsIdentityKnown(IOid oid) {
            Assert.AssertNotNull("OID should not be null", oid);
            ProcessChangedOid(oid);
            return identityAdapterMap.IsIdentityKnown(oid);
        }

        public void Replaced(object domainObject) {
            unloadedObjects[domainObject] = domainObject;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        ///     Given a new Oid (not from the adapter, but usually a reference during distribution) this method
        ///     extracts the original Oid, find the associated adapter and then updates the lookup so that the new Oid
        ///     now keys the adapter. The adapter's oid is then updated to take on the new Oid's identity.
        /// </summary>
        private void ProcessChangedOid(IOid updatedOid) {
            if (updatedOid.HasPrevious) {
                IOid previousOid = updatedOid.Previous;
                INakedObject nakedObject = identityAdapterMap.GetAdapter(previousOid);
                if (nakedObject != null) {
                    Log.DebugFormat("Updating oid {0} to {1}", previousOid, updatedOid);
                    identityAdapterMap.Remove(previousOid);
                    IOid oidFromObject = nakedObject.Oid;
                    oidFromObject.CopyFrom(updatedOid);
                    identityAdapterMap.Add(oidFromObject, nakedObject);
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}