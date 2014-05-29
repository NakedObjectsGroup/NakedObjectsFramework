// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Adapter.Map {
    public class IdentityMapImpl : IIdentityMap {
        private static readonly ILog Log;
        private readonly IDictionary<object, object> unloadedObjects = new Dictionary<object, object>();
        private IIdentityAdapterMap identityAdapterMap;
        private IPocoAdapterMap pocoAdapterMap;

        static IdentityMapImpl() {
            Log = LogManager.GetLogger(typeof (IdentityMapImpl));
        }

        /// <summary>
        ///     For dependency injection.
        /// </summary>
        /// <para>
        ///     If not injected, will be instantiated within <see cref="Init" /> method.
        /// </para>
        public virtual IIdentityAdapterMap IdentityAdapterMap {
            set { identityAdapterMap = value; }
            private get { return identityAdapterMap; }
        }

        /// <summary>
        ///     For dependency injection.
        /// </summary>
        /// <para>
        ///     If not injected, will be instantiated within <see cref="Init" /> method.
        /// </para>
        public virtual IPocoAdapterMap PocoAdapterMap {
            set { pocoAdapterMap = value; }
            private get { return pocoAdapterMap; }
        }

        #region IIdentityMap Members

        public virtual IEnumerator<INakedObject> GetEnumerator() {
            return PocoAdapterMap.GetEnumerator();
        }

        public virtual void Init() {
            if (identityAdapterMap == null) {
                identityAdapterMap = new IdentityAdapterHashMap();
            }
            if (pocoAdapterMap == null) {
                pocoAdapterMap = new PocoAdapterHashMap();
            }
        }

        public virtual void Shutdown() {
            IdentityAdapterMap.Shutdown();
            PocoAdapterMap.Shutdown();
        }

        public virtual void Reset() {
            IdentityAdapterMap.Reset();
            PocoAdapterMap.Reset();
            unloadedObjects.Clear();
        }

        public virtual void AddAdapter(INakedObject nakedObject) {
            Assert.AssertNotNull("Cannot add null adapter to IdentityAdapterMap", nakedObject);
            object obj = nakedObject.Object;
            Assert.AssertFalse("POCO Map already contains object", obj, PocoAdapterMap.ContainsObject(obj));

            if (unloadedObjects.ContainsKey(obj)) {
                string msg = string.Format(Resources.NakedObjects.TransientReferenceMessage, obj);
                throw new TransientReferenceException(msg);
            }

            // TODO we should be ignoring immutable values object as well
            if (nakedObject.Specification.IsObject) {
                PocoAdapterMap.Add(obj, nakedObject);
            }
            // order is important - add to identity map after poco map 
            IdentityAdapterMap.Add(nakedObject.Oid, nakedObject);

            // log at end so that if ToString needs adapters they're in maps. 
            Log.DebugFormat("Adding identity for {0}", nakedObject);
        }

        public virtual void MadePersistent(INakedObject adapter) {
            IOid oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            IdentityAdapterMap.Remove(oid);
            NakedObjectsContext.ObjectPersistor.ConvertTransientToPersistentOid(oid);

            adapter.ResolveState.Handle(Events.StartResolvingEvent);
            adapter.ResolveState.Handle(Events.EndResolvingEvent);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", PocoAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, IdentityAdapterMap.GetAdapter(oid));
            IdentityAdapterMap.Add(oid, adapter);
            Log.DebugFormat("Made persistent {0}; was {1}", adapter, oid.Previous);
        }

        public virtual void UpdateViewModel(INakedObject adapter, string[] keys) {
            IOid oid = adapter.Oid;

            // Changing the OID object that is already a key in the identity map messes up the hashing so it can't
            // be found afterwards. To work properly, we therefore remove the identity first then change the oid,
            // finally re-add to the map.

            IdentityAdapterMap.Remove(oid);

            ((ViewModelOid)adapter.Oid).UpdateKeys(keys, false);

            //adapter.ResolveState.Handle(Events.StartResolvingEvent);
            //adapter.ResolveState.Handle(Events.EndResolvingEvent);

            Assert.AssertTrue("Adapter's poco should exist in poco map and return the adapter", PocoAdapterMap.GetObject(adapter.Object) == adapter);
            Assert.AssertNull("Changed OID should not already map to a known adapter " + oid, IdentityAdapterMap.GetAdapter(oid));
            IdentityAdapterMap.Add(oid, adapter);
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
                IdentityAdapterMap.Remove(oid);
            }
            PocoAdapterMap.Remove(nakedObject);
        }

        public virtual INakedObject GetAdapterFor(object domainObject) {
            Assert.AssertNotNull("can't get an adapter for null", this, domainObject);
            return PocoAdapterMap.GetObject(domainObject);
        }

        public virtual INakedObject GetAdapterFor(IOid oid) {
            Assert.AssertNotNull("OID should not be null", this, oid);
            ProcessChangedOid(oid);
            return IdentityAdapterMap.GetAdapter(oid);
        }

        public virtual bool IsIdentityKnown(IOid oid) {
            Assert.AssertNotNull("OID should not be null", oid);
            ProcessChangedOid(oid);
            return IdentityAdapterMap.IsIdentityKnown(oid);
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