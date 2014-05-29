// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Transaction;
using NakedObjects.Util;

namespace NakedObjects.Persistor.Objectstore {
    public class ObjectStorePersistor : NakedObjectPersistorAbstract, IPersistedObjectAdder {
        private static readonly ILog Log;
        private INakedObjectStore objectStore;
        private IPersistAlgorithm persistAlgorithm;
        private INakedObjectTransactionManager transactionManager;

        static ObjectStorePersistor() {
            Log = LogManager.GetLogger(typeof (ObjectStorePersistor));
        }

        public ObjectStorePersistor() {
            Log.DebugFormat("Creating {0}", this);
        }

        public override bool IsInitialized {
            get { return objectStore.IsInitialized; }
            set { objectStore.IsInitialized = value; }
        }

        public virtual IPersistAlgorithm PersistAlgorithm {
            set { persistAlgorithm = value; }
        }

        public virtual INakedObjectTransactionManager TransactionManager {
            set { transactionManager = value; }
        }

        public virtual INakedObjectStore ObjectStore {
            set { objectStore = value; }
        }


        public override string DebugTitle {
            get { return "Object Store Persistor"; }
        }

        #region IPersistedObjectAdder Members

        public virtual void AddPersistedObject(INakedObject nakedObject) {
            if (nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                return;
            }
            ICreateObjectCommand createObjectCommand = objectStore.CreateCreateObjectCommand(nakedObject);
            transactionManager.AddCommand(createObjectCommand);
        }

        public virtual void MadePersistent(INakedObject nakedObject) {
            IdentityMap.MadePersistent(nakedObject);
        }

        #endregion

        /// <summary>
        ///     Initialize the object store so that calls to this object store access persisted objects and persist
        ///     changes to the object that are saved.
        /// </summary>
        public override void Init() {
            Log.Debug("Init");
            Assert.AssertNotNull("persist algorithm required", persistAlgorithm);
            Assert.AssertNotNull("object store required", objectStore);
            objectStore.Init();

            // can inject the TxMgr, but  will otherwise default
            if (transactionManager == null) {
                transactionManager = new ObjectStoreTransactionManager(objectStore);
                transactionManager.Init();
            }

            persistAlgorithm.Init();
            base.Init();
        }

        public override void Shutdown() {
            Log.Debug("Shutdown");
            if (transactionManager != null) {
                transactionManager.Shutdown();
            }
            base.Shutdown();
            persistAlgorithm.Shutdown();
            objectStore.Shutdown();
            objectStore = null;
        }

        public override void Reset() {
            Log.Debug("Reset");
            objectStore.Reset();
            IdentityMap.Reset();
            adapterCache.Reset();
            GetServices();
        }

        public override INakedObject LoadObject(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, specification);

            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", specification);

            INakedObject nakedObject = IdentityMap.IsIdentityKnown(oid) ? GetAdapterFor(oid) : objectStore.GetObject(oid, specification);
            return nakedObject;
        }


        public override void Reload(INakedObject nakedObject) {
            Log.DebugFormat("Reload nakedObject: {0}", nakedObject);
            objectStore.Reload(nakedObject);
        }

        public override void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            Log.DebugFormat("ResolveField nakedObject: {0} field: {1}", nakedObject, field);
            if (field.Specification.HasNoIdentity) {
                return;
            }
            INakedObject reference = field.GetNakedObject(nakedObject);
            if (reference == null || reference.ResolveState.IsResolved()) {
                return;
            }
            if (!reference.ResolveState.IsPersistent()) {
                return;
            }
            if (Log.IsInfoEnabled) {
                // don't log object - its ToString() may use the unresolved field or unresolved collection
                Log.Info("resolve field " + nakedObject.Specification.ShortName + "." + field.Id + ": " + reference.Specification.ShortName + " " + reference.ResolveState.CurrentState.Code + " " + reference.Oid);
            }
            objectStore.ResolveField(nakedObject, field);
        }

        public override void LoadField(INakedObject nakedObject, string field) {
            Log.DebugFormat("LoadField nakedObject: {0} field: {1}", nakedObject, field);
            INakedObjectAssociation association = nakedObject.Specification.Properties.Single(x => x.Id == field);
            ResolveField(nakedObject, association);
        }

        public override int CountField(INakedObject nakedObject, string field) {
            Log.DebugFormat("CountField nakedObject: {0} field: {1}", nakedObject, field);

            INakedObjectAssociation association = nakedObject.Specification.Properties.Single(x => x.Id == field);

            if (nakedObject.Specification.IsViewModel) {
                INakedObject collection = association.GetNakedObject(nakedObject);
                return collection.GetCollectionFacetFromSpec().AsEnumerable(collection).Count();
            }

            return objectStore.CountField(nakedObject, association);
        }


        public override void ResolveImmediately(INakedObject nakedObject) {
            Log.DebugFormat("ResolveImmediately nakedObject: {0}", nakedObject);
            if (nakedObject.ResolveState.IsResolvable()) {
                Assert.AssertFalse("only resolve object that is not yet resolved", nakedObject, nakedObject.ResolveState.IsResolved());
                Assert.AssertTrue("only resolve object that is persistent", nakedObject, nakedObject.ResolveState.IsPersistent());
                if (nakedObject.Oid is AggregateOid) {
                    return;
                }
                if (Log.IsInfoEnabled) {
                    // don't log object - it's ToString() may use the unresolved field, or unresolved collection
                    Log.Info("resolve immediately: " + nakedObject.Specification.ShortName + " " + nakedObject.ResolveState.CurrentState.Code + " " + nakedObject.Oid);
                }
                objectStore.ResolveImmediately(nakedObject);
            }
        }

        public override void ObjectChanged(INakedObject nakedObject) {
            Log.DebugFormat("ObjectChanged nakedObject: {0}", nakedObject);
            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects()) {
                if (nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                    nakedObject.Updating();
                    nakedObject.Updated();
                    NakedObjectsContext.UpdateNotifier.AddChangedObject(nakedObject);
                }
                else {
                    INakedObjectSpecification specification = nakedObject.Specification;
                    if (specification.IsAlwaysImmutable() || (specification.IsImmutableOncePersisted() && nakedObject.ResolveState.IsPersistent())) {
                        throw new NotPersistableException("cannot change immutable object");
                    }
                    nakedObject.Updating();
                    ISaveObjectCommand saveObjectCommand = objectStore.CreateSaveObjectCommand(nakedObject);
                    transactionManager.AddCommand(saveObjectCommand);
                    nakedObject.Updated();
                    NakedObjectsContext.UpdateNotifier.AddChangedObject(nakedObject);
                }
            }

            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects() ||
                nakedObject.ResolveState.IsTransient()) {
                NakedObjectsContext.UpdateNotifier.AddChangedObject(nakedObject);
            }
        }

        /// <summary>
        ///     Makes a naked object persistent. The specified object should be stored away via this object store's
        ///     persistence mechanism, and have an new and unique OID assigned to it. The object, should also be added
        ///     to the cache as the object is implicitly 'in use'.
        /// </summary>
        /// <para>
        ///     If the object has any associations then each of these, where they aren't already persistent, should
        ///     also be made persistent by recursively calling this method.
        /// </para>
        /// <para>
        ///     If the object to be persisted is a collection, then each element of that collection, that is not
        ///     already persistent, should be made persistent by recursively calling this method.
        /// </para>
        public override void MakePersistent(INakedObject nakedObject) {
            Log.DebugFormat("MakePersistent nakedObject: {0}", nakedObject);
            if (IsPersistent(nakedObject)) {
                throw new NotPersistableException("Object already persistent: " + nakedObject);
            }
            if (nakedObject.Specification.Persistable == Persistable.TRANSIENT) {
                throw new NotPersistableException("Object must be kept transient: " + nakedObject);
            }
            INakedObjectSpecification specification = nakedObject.Specification;
            if (specification.IsService) {
                throw new NotPersistableException("Cannot persist services: " + nakedObject);
            }

            persistAlgorithm.MakePersistent(nakedObject, this);
        }

        private static bool IsPersistent(INakedObject nakedObject) {
            Log.DebugFormat("IsPersistent nakedObject: {0}", nakedObject);
            return nakedObject.ResolveState.IsPersistent();
        }

        /// <summary>
        ///     Removes the specified object from the system. The specified object's data should be removed from the
        ///     persistence mechanism.
        /// </summary>
        public override void DestroyObject(INakedObject nakedObject) {
            Log.DebugFormat("DestroyObject nakedObject: {0}", nakedObject);

            nakedObject.Deleting();
            IDestroyObjectCommand command = objectStore.CreateDestroyObjectCommand(nakedObject);
            transactionManager.AddCommand(command);
            nakedObject.ResolveState.Handle(Events.DestroyEvent);
            nakedObject.Deleted();
        }

        protected override IQueryable<T> GetInstances<T>() {
            Log.Debug("GetInstances<T> of: " + typeof (T));
            return objectStore.GetInstances<T>();
        }

        protected override IQueryable GetInstances(Type type) {
            Log.Debug("GetInstances of: " + type);
            return objectStore.GetInstances(type);
        }

        protected override IQueryable GetInstances(INakedObjectSpecification specification) {
            Log.Debug("GetInstances<T> of: " + specification);
            return objectStore.GetInstances(specification);
        }

        protected override IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            return objectStore.GetOidForService(name, typeName);
        }

        protected override void RegisterService(string name, IOid oid) {
            Log.DebugFormat("RegisterService name: {0} oid : {1}", name, oid);
            objectStore.RegisterService(name, oid);
        }

        public override object CreateObject(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateObject: " + specification);
            Type type = TypeUtils.GetType(specification.FullName);

            if (specification.IsViewModel) {
                object viewModel = Activator.CreateInstance(type);
                InitDomainObject(viewModel);
                return viewModel;
            }

            return objectStore.CreateInstance(type);
        }

        public override void AbortTransaction() {
            Log.Debug("AbortTransaction");
            transactionManager.AbortTransaction();
        }

        public override void UserAbortTransaction() {
            Log.Debug("UserAbortTransaction");
            transactionManager.UserAbortTransaction();
        }

        public override void EndTransaction() {
            Log.Debug("EndTransaction");
            transactionManager.EndTransaction();
        }

        public override bool FlushTransaction() {
            Log.Debug("FlushTransaction");
            return transactionManager.FlushTransaction();
        }

        public override void StartTransaction() {
            Log.Debug("StartTransaction");
            transactionManager.StartTransaction();
        }

        public override void AddCommand(IPersistenceCommand command) {
            Log.Debug("AddCommand: " + command);
            transactionManager.AddCommand(command);
        }

        public override PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            return objectStore.GetKeys(type);
        }

        public override INakedObject FindByKeys(Type type, object[] keys) {
            Log.Debug("FindByKeys");
            return objectStore.FindByKeys(type, keys);
        }

        public override void Refresh(INakedObject nakedObject) {
            Log.DebugFormat("Refresh nakedObject: {0}", nakedObject);
            objectStore.Refresh(nakedObject);
        }


        public override string ToString() {
            var asString = new AsString(this);
            if (objectStore != null) {
                asString.Append("objectStore", objectStore.Name);
            }
            if (persistAlgorithm != null) {
                asString.Append("persistAlgorithm", persistAlgorithm.Name);
            }
            return asString.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}