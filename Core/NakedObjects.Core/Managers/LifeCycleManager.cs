// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Persistor;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Util;

namespace NakedObjects.Managers {
    public class LifeCycleManager : ILifecycleManager {
        private static readonly ILog Log;
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodel;
        private readonly IObjectPersistor objectPersistor;
        private readonly IPersistAlgorithm persistAlgorithm;
        private readonly ISession session;
        private readonly INakedObjectTransactionManager transactionManager;

        static LifeCycleManager() {
            Log = LogManager.GetLogger(typeof (LifeCycleManager));
        }

        public LifeCycleManager(ISession session,
                                IMetamodelManager metamodel,
                                INakedObjectStore objectStore,
                                IPersistAlgorithm persistAlgorithm,
                                IOidGenerator oidGenerator,
                                IIdentityMap identityMap,
                                IContainerInjector injector,
                                INakedObjectTransactionManager transactionManager,
                                IObjectPersistor objectPersistor,
                                INakedObjectManager manager
            ) {
            Assert.AssertNotNull(objectStore);
            Assert.AssertNotNull(persistAlgorithm);
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(identityMap);
            Assert.AssertNotNull(metamodel);

            this.transactionManager = transactionManager;
            this.objectPersistor = objectPersistor;
            this.manager = manager;
            this.session = session;
            this.metamodel = metamodel;
            this.persistAlgorithm = persistAlgorithm;
            this.injector = injector;

            Log.DebugFormat("Creating {0}", this);
        }

        #region ILifecycleManager Members

        public INakedObject LoadObject(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, specification);
            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", specification);
            return manager.GetKnownAdapter(oid) ?? objectPersistor.LoadObject(oid, specification);
        }

        /// <summary>
        ///     Factory (for transient instance)
        /// </summary>
        public virtual INakedObject CreateInstance(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateInstance of: {0}", specification);
            if (specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                throw new TransientReferenceException(Resources.NakedObjects.NoTransientInline);
            }
            object obj = CreateObject(specification);
            var adapter = manager.CreateInstanceAdapter(obj);
            InitializeNewObject(adapter);
            return adapter;
        }

        public INakedObject CreateViewModel(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateViewModel of: {0}", specification);
            object viewModel = CreateObject(specification);
            var adapter = manager.CreateViewModelAdapter(specification, viewModel);
            InitializeNewObject(adapter);
            return adapter;
        }


        public virtual INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("RecreateInstance oid: {0} hint: {1}", oid, specification);
            INakedObject adapter = manager.GetAdapterFor(oid);
            if (adapter != null) {
                if (!adapter.Specification.Equals(specification)) {
                    throw new AdapterException(string.Format("Mapped adapter is for a different type of object: {0}; {1}", specification.FullName, adapter));
                }
                return adapter;
            }
            Log.DebugFormat("Recreating instance for {0}", specification);
            object obj = CreateObject(specification);
            return manager.AdapterForExistingObject(obj, oid);
        }

        public virtual INakedObject GetViewModel(IOid oid) {
            return manager.GetKnownAdapter(oid) ?? RecreateViewModel((ViewModelOid) oid);
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
        public void MakePersistent(INakedObject nakedObject) {
            Log.DebugFormat("MakePersistent nakedObject: {0}", nakedObject);
            if (IsPersistent(nakedObject)) {
                throw new NotPersistableException("Object already persistent: " + nakedObject);
            }
            if (nakedObject.Specification.Persistable == PersistableType.Transient) {
                throw new NotPersistableException("Object must be kept transient: " + nakedObject);
            }
            INakedObjectSpecification specification = nakedObject.Specification;
            if (specification.IsService) {
                throw new NotPersistableException("Cannot persist services: " + nakedObject);
            }

            persistAlgorithm.MakePersistent(nakedObject, session);
        }
        

        private object CreateObject(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateObject: " + specification);
            Type type = TypeUtils.GetType(specification.FullName);

            if (specification.IsViewModel) {
                object viewModel = Activator.CreateInstance(type);
                InitDomainObject(viewModel);
                return viewModel;
            }

            return objectPersistor.CreateObject(specification);
        }


        public void AbortTransaction() {
            Log.Debug("AbortTransaction");
            transactionManager.AbortTransaction();
        }

        public void UserAbortTransaction() {
            Log.Debug("UserAbortTransaction");
            transactionManager.UserAbortTransaction();
        }

        public void EndTransaction() {
            Log.Debug("EndTransaction");
            transactionManager.EndTransaction();
        }

        public bool FlushTransaction() {
            Log.Debug("FlushTransaction");
            return transactionManager.FlushTransaction();
        }

        public void StartTransaction() {
            Log.Debug("StartTransaction");
            transactionManager.StartTransaction();
        }

        public void AddCommand(IPersistenceCommand command) {
            Log.Debug("AddCommand: " + command);
            transactionManager.AddCommand(command);
        }


        public void Abort(ILifecycleManager objectManager, IFacetHolder holder) {
            Log.Info("exception executing " + holder + ", aborting transaction");
            try {
                objectManager.AbortTransaction();
            }
            catch (Exception e2) {
                Log.Error("failure during abort", e2);
            }
        }

        public IOid RestoreGenericOid(string[] encodedData) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(encodedData.First()));
            INakedObjectSpecification spec = metamodel.GetSpecification(typeName);

            if (spec.IsCollection) {
                return new CollectionMemento(this, manager, objectPersistor, metamodel, session, encodedData);
            }

            if (spec.ContainsFacet<IViewModelFacet>()) {
                return new ViewModelOid(metamodel, encodedData);
            }

            return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(metamodel, encodedData) : null;
        }

        public void PopulateViewModelKeys(INakedObject nakedObject) {
            var vmoid = nakedObject.Oid as ViewModelOid;

            if (vmoid == null) {
                throw new UnknownTypeException(string.Format("Expect ViewModelOid got {0}", nakedObject.Oid == null ? "null" : nakedObject.Oid.GetType().ToString()));
            }

            if (!vmoid.IsFinal) {
                vmoid.UpdateKeys(nakedObject.Specification.GetFacet<IViewModelFacet>().Derive(nakedObject), true);
            }
        }

        #endregion

        private void InitDomainObject(object obj) {
            Log.DebugFormat("InitDomainObject: {0}", obj);
            injector.InitDomainObject(obj);
        }

        private void InitInlineObject(object root, object inlineObject) {
            Log.DebugFormat("InitInlineObject root: {0} inlineObject: {1}", root, inlineObject);
            injector.InitInlineObject(root, inlineObject);
        }

        private INakedObject RecreateViewModel(ViewModelOid oid) {
            string[] keys = oid.Keys;
            INakedObjectSpecification spec = oid.Specification;
            INakedObject vm = CreateViewModel(spec);
            vm.Specification.GetFacet<IViewModelFacet>().Populate(keys, vm);
            manager.UpdateViewModel(vm, keys);
            return vm;
        }

        private void CreateInlineObjects(INakedObject parentObject, object rootObject) {
            foreach (IOneToOneAssociation assoc in parentObject.Specification.Properties.Where(p => p.IsInline)) {
                object inlineObject = CreateObject(assoc.Specification);

                InitInlineObject(rootObject, inlineObject);
                INakedObject inlineNakedObject = manager.CreateAggregatedAdapter(parentObject, assoc.Id, inlineObject);
                InitializeNewObject(inlineNakedObject, rootObject);
                assoc.InitAssociation(parentObject, inlineNakedObject);
            }
        }

        private void InitializeNewObject(INakedObject nakedObject, object rootObject) {
            nakedObject.Specification.Properties.ForEach(field => field.ToDefault(nakedObject));
            CreateInlineObjects(nakedObject, rootObject);
            nakedObject.Created(session);
        }

        private void InitializeNewObject(INakedObject nakedObject) {
            InitializeNewObject(nakedObject, nakedObject.GetDomainObject());
        }

        private static bool IsPersistent(INakedObject nakedObject) {
            Log.DebugFormat("IsPersistent nakedObject: {0}", nakedObject);
            return nakedObject.ResolveState.IsPersistent();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}