// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Persist;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Component;

public sealed class LifeCycleManager : ILifecycleManager {
    private readonly IDomainObjectInjector injector;
    private readonly ILogger<LifeCycleManager> logger;
    private readonly ILoggerFactory loggerFactory;
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
        INakedObjectManager nakedObjectManager,
        ISession session,
        ILoggerFactory loggerFactory,
        ILogger<LifeCycleManager> logger
    ) {
        this.metamodel = metamodel ?? throw new InitialisationException($"{nameof(metamodel)} is null");
        this.persistAlgorithm = persistAlgorithm ?? throw new InitialisationException($"{nameof(persistAlgorithm)} is null");
        this.oidGenerator = oidGenerator ?? throw new InitialisationException($"{nameof(oidGenerator)} is null");
        this.injector = injector ?? throw new InitialisationException($"{nameof(injector)} is null");
        this.objectPersistor = objectPersistor ?? throw new InitialisationException($"{nameof(objectPersistor)} is null");
        this.nakedObjectManager = nakedObjectManager ?? throw new InitialisationException($"{nameof(nakedObjectManager)} is null");
        this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
        this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
    }

    private object CreateObject(ITypeSpec spec) {
        var type = TypeUtils.GetType(spec.FullName);
        return spec.IsViewModel || spec is IServiceSpec || spec.ContainsFacet<INotPersistedFacet>() ? CreateNotPersistedObject(type, spec is IServiceSpec, true) : objectPersistor.CreateObject(spec);
    }

    private object CreateNotPersistedObject(Type type, bool inject) {
        var instance = Activator.CreateInstance(type);
        return inject ? InitDomainObject(instance) : instance;
    }

    private object CreateNotPersistedObject(Type type, bool cache, bool inject) {
        if (cache) {
            return nonPersistedObjectCache.ContainsKey(type) ? nonPersistedObjectCache[type] : nonPersistedObjectCache[type] = CreateNotPersistedObject(type, inject);
        }

        return CreateNotPersistedObject(type, inject);
    }

    private IOid RestoreGenericOid(string[] encodedData, INakedFramework framework) {
        var typeName = TypeNameUtils.DecodeTypeName(HttpUtility.UrlDecode(encodedData.First()));
        var spec = metamodel.GetSpecification(typeName);

        if (spec.IsCollection) {
            return new CollectionMemento(framework, loggerFactory, loggerFactory.CreateLogger<CollectionMemento>(), encodedData);
        }

        if (spec.ContainsFacet<IViewModelFacet>()) {
            return new ViewModelOid(metamodel, loggerFactory, encodedData);
        }

        return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(metamodel, loggerFactory, encodedData) : null;
    }

    private object InitDomainObject(object obj) {
        injector.InjectInto(obj);
        return obj;
    }

    private void InitInlineObject(object root, object inlineObject) => injector.InjectIntoInline(root, inlineObject);

    private INakedObjectAdapter RecreateViewModel(ViewModelOid oid, INakedFramework framework) {
        var keys = oid.Keys;
        var spec = (IObjectSpec)oid.Spec;
        var vm = CreateViewModel(spec);
        vm.Spec.GetFacet<IViewModelFacet>().Populate(keys, vm, framework);
        nakedObjectManager.UpdateViewModel(vm, keys);
        return vm;
    }

    private void CreateInlineObjects(INakedObjectAdapter parentObjectAdapter, object rootObject) {
        var spec = parentObjectAdapter.Spec as IObjectSpec ?? throw new NakedObjectSystemException("parentObjectAdapter.Spec must be IObjectSpec");
        foreach (var assoc in spec.Properties.OfType<IOneToOneAssociationSpec>().Where(p => p.IsInline)) {
            var inlineObject = CreateObject(assoc.ReturnSpec);

            InitInlineObject(rootObject, inlineObject);
            var inlineNakedObjectAdapter = nakedObjectManager.CreateAggregatedAdapter(parentObjectAdapter, assoc.Id, inlineObject);
            InitializeNewObject(inlineNakedObjectAdapter, rootObject);
            assoc.InitAssociation(parentObjectAdapter, inlineNakedObjectAdapter);
        }
    }

    private void InitializeNewObject(INakedObjectAdapter nakedObjectAdapter, object rootObject) {
        var spec = nakedObjectAdapter.Spec as IObjectSpec ?? throw new NakedObjectSystemException("nakedObjectAdapter.Spec must be IObjectSpec");
        spec.Properties.ForEach(field => field.ToDefault(nakedObjectAdapter));
        CreateInlineObjects(nakedObjectAdapter, rootObject);
        nakedObjectAdapter.Created();
    }

    private void InitializeNewObject(INakedObjectAdapter nakedObjectAdapter) => InitializeNewObject(nakedObjectAdapter, nakedObjectAdapter.GetDomainObject());

    private static bool IsPersistent(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.IsPersistent();

    #region ILifecycleManager Members

    public INakedObjectAdapter LoadObject(IOid oid, ITypeSpec spec) {
        if (oid is null) {
            throw new NakedObjectSystemException("needs an OID");
        }

        if (spec is null) {
            throw new NakedObjectSystemException("needs a specification");
        }

        return nakedObjectManager.GetKnownAdapter(oid) ?? objectPersistor.LoadObject(oid, (IObjectSpec)spec);
    }

    public IList<(object original, object updated)> Persist(IDetachedObjects objects) => objectPersistor.UpdateDetachedObjects(objects);

    /// <summary>
    ///     Factory (for transient instance)
    /// </summary>
    public INakedObjectAdapter CreateInstance(IObjectSpec spec) {
        if (spec.ContainsFacet(typeof(IComplexTypeFacet))) {
            throw new TransientReferenceException(logger.LogAndReturn(NakedObjects.Resources.NakedObjects.NoTransientInline));
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
        if (adapter is not null) {
            if (!adapter.Spec.Equals(spec)) {
                throw new AdapterException(logger.LogAndReturn($"Mapped adapter is for a different type of object: {spec.FullName}; {adapter}"));
            }

            return adapter;
        }

        var obj = CreateObject(spec);
        return nakedObjectManager.AdapterForExistingObject(obj, oid);
    }

    public object CreateNonAdaptedInjectedObject(Type type) => CreateNotPersistedObject(type, true);

    public object CreateNonAdaptedObject(Type type) => CreateNotPersistedObject(type, true);

    public INakedObjectAdapter GetViewModel(IOid oid, INakedFramework framework) => nakedObjectManager.GetKnownAdapter(oid) ?? RecreateViewModel((ViewModelOid)oid, framework);

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
            throw new NotPersistableException(logger.LogAndReturn($"Object already persistent: {nakedObjectAdapter}"));
        }

        if (nakedObjectAdapter.Spec.Persistable == PersistableType.Transient) {
            throw new NotPersistableException(logger.LogAndReturn($"Object must be kept transient: {nakedObjectAdapter}"));
        }

        var spec = nakedObjectAdapter.Spec;
        if (spec is IServiceSpec) {
            throw new NotPersistableException(logger.LogAndReturn($"Cannot persist services: {nakedObjectAdapter}"));
        }

        persistAlgorithm.MakePersistent(nakedObjectAdapter);
    }

    public void PopulateViewModelKeys(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        if (nakedObjectAdapter.Oid is not ViewModelOid vmOid) {
            throw new UnknownTypeException(logger.LogAndReturn($"Expect ViewModelOid got {nakedObjectAdapter.Oid?.GetType().ToString() ?? "null"}"));
        }

        vmOid.UpdateKeysIfNecessary(nakedObjectAdapter, framework);
    }

    public IOid RestoreOid(string[] encodedData, INakedFramework framework) => RestoreGenericOid(encodedData, framework) ?? oidGenerator.RestoreOid(encodedData);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.