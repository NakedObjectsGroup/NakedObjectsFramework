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
    public sealed class LifeCycleManager : ILifecycleManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LifeCycleManager));
        private readonly IDomainObjectInjector injector;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IDictionary<Type, object> nonPersistedObjectCache = new Dictionary<Type, object>();
        private readonly IObjectPersistor objectPersistor;
        private readonly IOidGenerator oidGenerator;
        private readonly IPersistAlgorithm persistAlgorithm;

        public LifeCycleManager(
            IMetamodelManager metamodel,
            IPersistAlgorithm persistAlgorithm,
            IOidGenerator oidGenerator,
            IDomainObjectInjector injector,
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
        }

        #region ILifecycleManager Members

        public INakedObjectAdapter LoadObject(IOid oid, ITypeSpec spec) {
            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", spec);
            return nakedObjectManager.GetKnownAdapter(oid) ?? objectPersistor.LoadObject(oid, (IObjectSpec) spec);
        }

        /// <summary>
        ///     Factory (for transient instance)
        /// </summary>
        public INakedObjectAdapter CreateInstance(IObjectSpec spec) {
            if (spec.ContainsFacet(typeof(IComplexTypeFacet))) {
                throw new TransientReferenceException(Log.LogAndReturn(Resources.NakedObjects.NoTransientInline));
            }

            var obj = CreateObject(spec);
            var adapter = nakedObjectManager.CreateInstanceAdapter(obj);
            InitializeNewObject(adapter);
            return adapter;
        }

        public INakedObjectAdapter CreateViewModel(IObjectSpec spec) {
            var viewModel = CreateObject(spec);
            var adapter = nakedObjectManager.CreateViewModelAdapter(spec, viewModel);
            InitializeNewObject(adapter);
            return adapter;
        }

        public INakedObjectAdapter RecreateInstance(IOid oid, ITypeSpec spec) {
            var adapter = nakedObjectManager.GetAdapterFor(oid);
            if (adapter != null) {
                if (!adapter.Spec.Equals(spec)) {
                    throw new AdapterException(Log.LogAndReturn($"Mapped adapter is for a different type of object: {spec.FullName}; {adapter}"));
                }

                return adapter;
            }

            var obj = CreateObject(spec);
            return nakedObjectManager.AdapterForExistingObject(obj, oid);
        }

        public object CreateNonAdaptedInjectedObject(Type type) => CreateNotPersistedObject(type, true);

        public INakedObjectAdapter GetViewModel(IOid oid) => nakedObjectManager.GetKnownAdapter(oid) ?? RecreateViewModel((ViewModelOid) oid);

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
        public void MakePersistent(INakedObjectAdapter nakedObjectAdapter) {
            if (IsPersistent(nakedObjectAdapter)) {
                throw new NotPersistableException(Log.LogAndReturn($"Object already persistent: {nakedObjectAdapter}"));
            }

            if (nakedObjectAdapter.Spec.Persistable == PersistableType.Transient) {
                throw new NotPersistableException(Log.LogAndReturn($"Object must be kept transient: {nakedObjectAdapter}"));
            }

            var spec = nakedObjectAdapter.Spec;
            if (spec is IServiceSpec) {
                throw new NotPersistableException(Log.LogAndReturn($"Cannot persist services: {nakedObjectAdapter}"));
            }

            persistAlgorithm.MakePersistent(nakedObjectAdapter);
        }

        public void PopulateViewModelKeys(INakedObjectAdapter nakedObjectAdapter) {
            var vmoid = nakedObjectAdapter.Oid as ViewModelOid;

            if (vmoid == null) {
                throw new UnknownTypeException(Log.LogAndReturn($"Expect ViewModelOid got {(nakedObjectAdapter.Oid == null ? "null" : nakedObjectAdapter.Oid.GetType().ToString())}"));
            }

            // todo fix - temp hack
            //if (!vmoid.IsFinal) {
            vmoid.UpdateKeys(nakedObjectAdapter.Spec.GetFacet<IViewModelFacet>().Derive(nakedObjectAdapter, nakedObjectManager, injector), true);
            //}
        }

        public IOid RestoreOid(string[] encodedData) => RestoreGenericOid(encodedData) ?? oidGenerator.RestoreOid(encodedData);

        #endregion

        private object CreateObject(ITypeSpec spec) {
            var type = TypeUtils.GetType(spec.FullName);
            return spec.IsViewModel || spec is IServiceSpec || spec.ContainsFacet<INotPersistedFacet>() ? CreateNotPersistedObject(type, spec is IServiceSpec) : objectPersistor.CreateObject(spec);
        }

        private object CreateNotPersistedObject(Type type) {
            var instance = Activator.CreateInstance(type);
            return InitDomainObject(instance);
        }

        private object CreateNotPersistedObject(Type type, bool cache) {
            if (cache) {
                return nonPersistedObjectCache.ContainsKey(type) ? nonPersistedObjectCache[type] : nonPersistedObjectCache[type] = CreateNotPersistedObject(type);
            }

            return CreateNotPersistedObject(type);
        }

        private IOid RestoreGenericOid(string[] encodedData) {
            var typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(encodedData.First()));
            var spec = metamodel.GetSpecification(typeName);

            if (spec.IsCollection) {
                return new CollectionMemento(this, nakedObjectManager, metamodel, encodedData);
            }

            if (spec.ContainsFacet<IViewModelFacet>()) {
                return new ViewModelOid(metamodel, encodedData);
            }

            return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(metamodel, encodedData) : null;
        }

        private object InitDomainObject(object obj) {
            injector.InjectInto(obj);
            return obj;
        }

        private void InitInlineObject(object root, object inlineObject) => injector.InjectIntoInline(root, inlineObject);

        private INakedObjectAdapter RecreateViewModel(ViewModelOid oid) {
            var keys = oid.Keys;
            var spec = (IObjectSpec) oid.Spec;
            var vm = CreateViewModel(spec);
            vm.Spec.GetFacet<IViewModelFacet>().Populate(keys, vm, nakedObjectManager, injector);
            nakedObjectManager.UpdateViewModel(vm, keys);
            return vm;
        }

        private void CreateInlineObjects(INakedObjectAdapter parentObjectAdapter, object rootObject) {
            var spec = parentObjectAdapter.Spec as IObjectSpec;
            Trace.Assert(spec != null);
            foreach (var assoc in spec.Properties.OfType<IOneToOneAssociationSpec>().Where(p => p.IsInline)) {
                var inlineObject = CreateObject(assoc.ReturnSpec);

                InitInlineObject(rootObject, inlineObject);
                var inlineNakedObjectAdapter = nakedObjectManager.CreateAggregatedAdapter(parentObjectAdapter, assoc.Id, inlineObject);
                InitializeNewObject(inlineNakedObjectAdapter, rootObject);
                assoc.InitAssociation(parentObjectAdapter, inlineNakedObjectAdapter);
            }
        }

        private void InitializeNewObject(INakedObjectAdapter nakedObjectAdapter, object rootObject) {
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            Trace.Assert(spec != null);
            spec.Properties.ForEach(field => field.ToDefault(nakedObjectAdapter));
            CreateInlineObjects(nakedObjectAdapter, rootObject);
            nakedObjectAdapter.Created();
        }

        private void InitializeNewObject(INakedObjectAdapter nakedObjectAdapter) => InitializeNewObject(nakedObjectAdapter, nakedObjectAdapter.GetDomainObject());

        private static bool IsPersistent(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.IsPersistent();
    }

    // Copyright (c) Naked Objects Group Ltd.
}