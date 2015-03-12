// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Component {
    public class LifeCycleManager : ILifecycleManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LifeCycleManager));
        private readonly IContainerInjector injector;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IObjectPersistor objectPersistor;
        private readonly IOidGenerator oidGenerator;
        private readonly IPersistAlgorithm persistAlgorithm;
        private readonly IDictionary<Type, object> nonPersistedObjectCache = new Dictionary<Type, object>();


        public LifeCycleManager(
            IMetamodelManager metamodel,
            IPersistAlgorithm persistAlgorithm,
            IOidGenerator oidGenerator,
            IContainerInjector injector,
            IObjectPersistor objectPersistor,
            INakedObjectManager nakedObjectManager
            ) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(persistAlgorithm);
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(injector);
            Assert.AssertNotNull(objectPersistor);
            Assert.AssertNotNull(nakedObjectManager);

            this.metamodel = metamodel;
            this.persistAlgorithm = persistAlgorithm;
            this.oidGenerator = oidGenerator;
            this.injector = injector;
            this.objectPersistor = objectPersistor;
            this.nakedObjectManager = nakedObjectManager;

            Log.DebugFormat("Creating {0}", this);
        }

        #region ILifecycleManager Members

        public INakedObject LoadObject(IOid oid, ITypeSpec spec) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, spec);
            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", spec);
            return nakedObjectManager.GetKnownAdapter(oid) ?? objectPersistor.LoadObject(oid, (IObjectSpec) spec);
        }

        /// <summary>
        ///     Factory (for transient instance)
        /// </summary>
        public virtual INakedObject CreateInstance(IObjectSpec spec) {
            Log.DebugFormat("CreateInstance of: {0}", spec);
            if (spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                throw new TransientReferenceException(Resources.NakedObjects.NoTransientInline);
            }
            object obj = CreateObject(spec);
            INakedObject adapter = nakedObjectManager.CreateInstanceAdapter(obj);
            InitializeNewObject(adapter);
            return adapter;
        }

        public INakedObject CreateViewModel(IObjectSpec spec) {
            Log.DebugFormat("CreateViewModel of: {0}", spec);
            object viewModel = CreateObject(spec);
            INakedObject adapter = nakedObjectManager.CreateViewModelAdapter(spec, viewModel);
            InitializeNewObject(adapter);
            return adapter;
        }

        public virtual INakedObject RecreateInstance(IOid oid, ITypeSpec spec) {
            Log.DebugFormat("RecreateInstance oid: {0} hint: {1}", oid, spec);
            INakedObject adapter = nakedObjectManager.GetAdapterFor(oid);
            if (adapter != null) {
                if (!adapter.Spec.Equals(spec)) {
                    throw new AdapterException(string.Format("Mapped adapter is for a different type of object: {0}; {1}", spec.FullName, adapter));
                }
                return adapter;
            }
            Log.DebugFormat("Recreating instance for {0}", spec);
            object obj = CreateObject(spec);
            return nakedObjectManager.AdapterForExistingObject(obj, oid);
        }

        public object CreateNonAdaptedInjectedObject(Type type) {
            return CreateNotPersistedObject(type, true);
        }

        public virtual INakedObject GetViewModel(IOid oid) {
            return nakedObjectManager.GetKnownAdapter(oid) ?? RecreateViewModel((ViewModelOid) oid);
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
            if (nakedObject.Spec.Persistable == PersistableType.Transient) {
                throw new NotPersistableException("Object must be kept transient: " + nakedObject);
            }
            ITypeSpec spec = nakedObject.Spec;
            if (spec is IServiceSpec) {
                throw new NotPersistableException("Cannot persist services: " + nakedObject);
            }

            persistAlgorithm.MakePersistent(nakedObject);
        }

        public void PopulateViewModelKeys(INakedObject nakedObject) {
            var vmoid = nakedObject.Oid as ViewModelOid;

            if (vmoid == null) {
                throw new UnknownTypeException(string.Format("Expect ViewModelOid got {0}", nakedObject.Oid == null ? "null" : nakedObject.Oid.GetType().ToString()));
            }

            if (!vmoid.IsFinal) {
                vmoid.UpdateKeys(nakedObject.Spec.GetFacet<IViewModelFacet>().Derive(nakedObject, nakedObjectManager, injector), true);
            }
        }

        public IOid RestoreOid(string[] encodedData) {
            return RestoreGenericOid(encodedData) ?? oidGenerator.RestoreOid(encodedData);
        }

        #endregion

        private object CreateObject(ITypeSpec spec) {
            Log.DebugFormat("CreateObject: " + spec);
            Type type = TypeUtils.GetType(spec.FullName);

            return spec.IsViewModel || spec is IServiceSpec || spec.ContainsFacet<INotPersistedFacet>() ? CreateNotPersistedObject(type, spec is IServiceSpec) : objectPersistor.CreateObject(spec);
        }

        private object CreateNotPersistedObject(Type type) {
            object instance = Activator.CreateInstance(type);
            return InitDomainObject(instance);
        }

        private object CreateNotPersistedObject(Type type, bool cache) {
            if (cache) {
                return nonPersistedObjectCache.ContainsKey(type) ? nonPersistedObjectCache[type] : (nonPersistedObjectCache[type] = CreateNotPersistedObject(type));
            }
            return CreateNotPersistedObject(type);
        }

        private IOid RestoreGenericOid(string[] encodedData) {
            string typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(encodedData.First()));
            ITypeSpec spec = metamodel.GetSpecification(typeName);

            if (spec.IsCollection) {
                return new CollectionMemento(this, nakedObjectManager, metamodel, encodedData);
            }

            if (spec.ContainsFacet<IViewModelFacet>()) {
                return new ViewModelOid(metamodel, encodedData);
            }

            return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(metamodel, encodedData) : null;
        }

        private object InitDomainObject(object obj) {
            Log.DebugFormat("InitDomainObject: {0}", obj);
            injector.InitDomainObject(obj);
            return obj;
        }

        private void InitInlineObject(object root, object inlineObject) {
            Log.DebugFormat("InitInlineObject root: {0} inlineObject: {1}", root, inlineObject);
            injector.InitInlineObject(root, inlineObject);
        }

        private INakedObject RecreateViewModel(ViewModelOid oid) {
            string[] keys = oid.Keys;
            var spec = (IObjectSpec) oid.Spec;
            INakedObject vm = CreateViewModel(spec);
            vm.Spec.GetFacet<IViewModelFacet>().Populate(keys, vm, nakedObjectManager, injector);
            nakedObjectManager.UpdateViewModel(vm, keys);
            return vm;
        }

        private void CreateInlineObjects(INakedObject parentObject, object rootObject) {
            var spec = parentObject.Spec as IObjectSpec;
            Trace.Assert(spec != null);
            foreach (IOneToOneAssociationSpec assoc in spec.Properties.OfType<IOneToOneAssociationSpec>().Where(p => p.IsInline)) {
                object inlineObject = CreateObject(assoc.ReturnSpec);

                InitInlineObject(rootObject, inlineObject);
                INakedObject inlineNakedObject = nakedObjectManager.CreateAggregatedAdapter(parentObject, assoc.Id, inlineObject);
                InitializeNewObject(inlineNakedObject, rootObject);
                assoc.InitAssociation(parentObject, inlineNakedObject);
            }
        }

        private void InitializeNewObject(INakedObject nakedObject, object rootObject) {
            var spec = nakedObject.Spec as IObjectSpec;
            Trace.Assert(spec != null);
            spec.Properties.ForEach(field => field.ToDefault(nakedObject));
            CreateInlineObjects(nakedObject, rootObject);
            nakedObject.Created();
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