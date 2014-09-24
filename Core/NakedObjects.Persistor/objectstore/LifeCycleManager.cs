// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Service;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;
using NakedObjects.Persistor.Transaction;
using NakedObjects.Util;

namespace NakedObjects.Persistor.Objectstore {
    internal class NakedObjectFactory {
        private ILifecycleManager persistor;
        private INakedObjectReflector reflector;
        private ISession session;

        public void Initialize(INakedObjectReflector reflector, ISession session, ILifecycleManager persistor) {
            this.reflector = reflector;
            this.session = session;
            this.persistor = persistor;
        }

        public INakedObject CreateAdapter(object obj, IOid oid) {
            return new PocoAdapter(reflector, session, persistor, obj, oid);
        }
    }

    internal class ObjectPersistor : IObjectPersistor {
        private static readonly ILog Log;
        private readonly INakedObjectManager manager;
        private readonly INakedObjectStore objectStore;
        private readonly ISession session;
        private readonly INakedObjectTransactionManager transactionManager;
        private readonly IUpdateNotifier updateNotifier;

        static ObjectPersistor() {
            Log = LogManager.GetLogger(typeof (ObjectPersistor));
        }

        public ObjectPersistor(INakedObjectStore objectStore,
            INakedObjectTransactionManager transactionManager,
            ISession session,
            INakedObjectManager manager,
            IUpdateNotifier updateNotifier) {
            this.objectStore = objectStore;
            this.transactionManager = transactionManager;
            this.session = session;
            this.manager = manager;
            this.updateNotifier = updateNotifier;
        }

        #region IObjectPersistor Members

        public virtual IQueryable<T> Instances<T>() where T : class {
            Log.DebugFormat("Instances<T> of: {0}", typeof (T));
            return GetInstances<T>();
        }

        public virtual IQueryable Instances(Type type) {
            Log.DebugFormat("Instances of: {0}", type);
            return GetInstances(type);
        }

        public virtual IQueryable Instances(INakedObjectSpecification specification) {
            Log.DebugFormat("Instances of: {0}", specification);
            return GetInstances(specification);
        }

        public INakedObject LoadObject(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, specification);

            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", specification);

            return objectStore.GetObject(oid, specification);
        }

        public void AddPersistedObject(INakedObject nakedObject) {
            if (nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                return;
            }
            ICreateObjectCommand createObjectCommand = objectStore.CreateCreateObjectCommand(nakedObject, session);
            transactionManager.AddCommand(createObjectCommand);
        }

        public void Reload(INakedObject nakedObject) {
            Log.DebugFormat("Reload nakedObject: {0}", nakedObject);
            objectStore.Reload(nakedObject);
        }

        public void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            Log.DebugFormat("ResolveField nakedObject: {0} field: {1}", nakedObject, field);
            if (field.Specification.HasNoIdentity) {
                return;
            }
            INakedObject reference = field.GetNakedObject(nakedObject, manager);
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

        public void LoadField(INakedObject nakedObject, string field) {
            Log.DebugFormat("LoadField nakedObject: {0} field: {1}", nakedObject, field);
            INakedObjectAssociation association = nakedObject.Specification.Properties.Single(x => x.Id == field);
            ResolveField(nakedObject, association);
        }

        public int CountField(INakedObject nakedObject, string field) {
            Log.DebugFormat("CountField nakedObject: {0} field: {1}", nakedObject, field);

            INakedObjectAssociation association = nakedObject.Specification.Properties.Single(x => x.Id == field);

            if (nakedObject.Specification.IsViewModel) {
                INakedObject collection = association.GetNakedObject(nakedObject, manager);
                return collection.GetCollectionFacetFromSpec().AsEnumerable(collection, manager).Count();
            }

            return objectStore.CountField(nakedObject, association);
        }

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            return objectStore.GetKeys(type);
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            Log.Debug("FindByKeys");
            return objectStore.FindByKeys(type, keys);
        }

        public void Refresh(INakedObject nakedObject) {
            Log.DebugFormat("Refresh nakedObject: {0}", nakedObject);
            objectStore.Refresh(nakedObject);
        }

        public void ResolveImmediately(INakedObject nakedObject) {
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

        public void ObjectChanged(INakedObject nakedObject) {
            Log.DebugFormat("ObjectChanged nakedObject: {0}", nakedObject);
            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects()) {
                if (nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                    nakedObject.Updating(session);
                    nakedObject.Updated(session);
                    updateNotifier.AddChangedObject(nakedObject);
                }
                else {
                    INakedObjectSpecification specification = nakedObject.Specification;
                    if (specification.IsAlwaysImmutable() || (specification.IsImmutableOncePersisted() && nakedObject.ResolveState.IsPersistent())) {
                        throw new NotPersistableException("cannot change immutable object");
                    }
                    nakedObject.Updating(session);
                    ISaveObjectCommand saveObjectCommand = objectStore.CreateSaveObjectCommand(nakedObject, session);
                    transactionManager.AddCommand(saveObjectCommand);
                    nakedObject.Updated(session);
                    updateNotifier.AddChangedObject(nakedObject);
                }
            }

            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects() ||
                nakedObject.ResolveState.IsTransient()) {
                updateNotifier.AddChangedObject(nakedObject);
            }
        }

        public void DestroyObject(INakedObject nakedObject) {
            Log.DebugFormat("DestroyObject nakedObject: {0}", nakedObject);

            nakedObject.Deleting(session);
            IDestroyObjectCommand command = objectStore.CreateDestroyObjectCommand(nakedObject);
            transactionManager.AddCommand(command);
            nakedObject.ResolveState.Handle(Events.DestroyEvent);
            nakedObject.Deleted(session);
        }

        public object CreateObject(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateObject: " + specification);

            Type type = TypeUtils.GetType(specification.FullName);
            return objectStore.CreateInstance(type);
        }

        #endregion

        protected IQueryable<T> GetInstances<T>() where T : class {
            Log.Debug("GetInstances<T> of: " + typeof (T));
            return objectStore.GetInstances<T>();
        }

        protected IQueryable GetInstances(Type type) {
            Log.Debug("GetInstances of: " + type);
            return objectStore.GetInstances(type);
        }

        protected IQueryable GetInstances(INakedObjectSpecification specification) {
            Log.Debug("GetInstances<T> of: " + specification);
            return objectStore.GetInstances(specification);
        }
    }

    internal class NakedObjectManager : INakedObjectManager {
        private static readonly ILog Log;
        private readonly INoIdentityAdapterCache adapterCache = new NoIdentityAdapterCache();
        private readonly IIdentityMap identityMap;
        private readonly NakedObjectFactory nakedObjectFactory;
        private readonly IOidGenerator oidGenerator;
        private readonly INakedObjectReflector reflector;
        private readonly ISession session;

        static NakedObjectManager() {
            Log = LogManager.GetLogger(typeof (NakedObjectManager));
        }

        public NakedObjectManager(INakedObjectReflector reflector, ISession session, IIdentityMap identityMap, IOidGenerator oidGenerator, NakedObjectFactory nakedObjectFactory) {
            this.reflector = reflector;
            this.session = session;
            this.identityMap = identityMap;
            this.oidGenerator = oidGenerator;
            this.nakedObjectFactory = nakedObjectFactory;
            this.OidGenerator = oidGenerator;
        }

        #region INakedObjectManager Members

        public IOidGenerator OidGenerator { get; private set; }

        public void RemoveAdapter(INakedObject objectToDispose) {
            Log.DebugFormat("RemoveAdapter nakedObject: {0}", objectToDispose);
            identityMap.Unloaded(objectToDispose);
        }

        public INakedObject GetAdapterFor(object obj) {
            Log.DebugFormat("GetAdapterFor: {0}", obj);
            Assert.AssertNotNull("must have a domain object", obj);
            INakedObject nakedObject = identityMap.GetAdapterFor(obj);
            if (nakedObject != null && nakedObject.Object != obj) {
                throw new AdapterException("Mapped adapter is for different domain object: " + obj + "; " + nakedObject);
            }
            return nakedObject;
        }

        public INakedObject GetAdapterFor(IOid oid) {
            Log.DebugFormat("GetAdapterFor oid: {0}", oid);
            Assert.AssertNotNull("must have an OID", oid);
            return identityMap.GetAdapterFor(oid);
        }

        public INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
            Log.DebugFormat("AdapterFor domainObject: {0} oid: {1} version: {2}", domainObject, oid, version);
            if (domainObject == null) {
                return null;
            }
            if (oid == null) {
                INakedObjectSpecification nakedObjectSpecification = reflector.LoadSpecification(domainObject.GetType());
                if (nakedObjectSpecification.ContainsFacet(typeof (IComplexTypeFacet))) {
                    return GetAdapterFor(domainObject);
                }
                if (nakedObjectSpecification.HasNoIdentity) {
                    return AdapterForNoIdentityObject(domainObject);
                }
                return AdapterForExistingObject(domainObject, nakedObjectSpecification);
            }
            return AdapterForExistingObject(domainObject, oid);
        }

        public void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
            Log.DebugFormat("ReplacePoco nakedObject: {0} newDomainOject: {1}", nakedObject, newDomainObject);
            RemoveAdapter(nakedObject);
            identityMap.Replaced(nakedObject.Object);
            nakedObject.ReplacePoco(newDomainObject);
            identityMap.AddAdapter(nakedObject);
        }

        public virtual void MadePersistent(INakedObject nakedObject) {
            identityMap.MadePersistent(nakedObject);
        }

        public virtual void UpdateViewModel(INakedObject adapter, string[] keys) {
            identityMap.UpdateViewModel(adapter, keys);
        }

        public INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
            GetAdapterFor(obj);

            IOid oid = new AggregateOid(reflector, parent.Oid, fieldId, obj.GetType().FullName);
            INakedObject adapterFor = GetAdapterFor(oid);
            if (adapterFor == null || adapterFor.Object != obj) {
                if (adapterFor != null) {
                    RemoveAdapter(adapterFor);
                }
                adapterFor = CreateAdapter(obj, oid, null);
                adapterFor.OptimisticLock = new NullVersion();
            }
            Assert.AssertNotNull(adapterFor);
            return adapterFor;
        }

        public INakedObject NewAdapterForKnownObject(object domainObject, IOid transientOid) {
            return nakedObjectFactory.CreateAdapter(domainObject, transientOid);
        }

        public List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
            return (from object domainObject in domainObjects
                select CreateAdapter(domainObject, null, null)).ToList();
        }

        public INakedObject GetServiceAdapter(object service) {
            IOid oid = GetOidForService(ServiceUtils.GetId(service), service.GetType().FullName);
            return AdapterForService(oid, service);
        }

        #endregion

        public IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            return OidGenerator.CreateOid(typeName, new object[] {0});
        }

        public INakedObject GetKnownAdapter(IOid oid) {
            if (identityMap.IsIdentityKnown(oid)) {
                return GetAdapterFor(oid);
            }
            return null;
        }

        private INakedObject AdapterForNoIdentityObject(object domainObject) {
            INakedObject pocoAdapter = adapterCache.GetAdapter(domainObject);

            if (pocoAdapter == null) {
                pocoAdapter = NewAdapterForKnownObject(domainObject, null);
                NewTransientsResolvedState(pocoAdapter);
                adapterCache.AddAdapter(pocoAdapter);
            }

            return pocoAdapter;
        }

        private INakedObject AdapterForExistingObject(object domainObject, INakedObjectSpecification spec) {
            return identityMap.GetAdapterFor(domainObject) ?? NewAdapterForViewModel(domainObject, spec) ?? NewAdapterForTransient(domainObject);
        }

        public INakedObject AdapterForExistingObject(object domainObject, IOid oid) {
            return GetAdapterFor(domainObject) ?? NewAdapterBasedOnOid(domainObject, oid);
        }

        private static void NewTransientsResolvedState(INakedObject pocoAdapter) {
            pocoAdapter.ResolveState.Handle(pocoAdapter.Specification.IsAggregated ? Events.InitializeAggregateEvent : Events.InitializeTransientEvent);
        }

        private INakedObject NewAdapterBasedOnOid(object domainObject, IOid oid) {
            INakedObject nakedObject = NewAdapterForKnownObject(domainObject, oid);
            identityMap.AddAdapter(nakedObject);

            if (oid is AggregateOid && nakedObject.Specification.IsObject) {
                nakedObject.ResolveState.Handle(Events.InitializeAggregateEvent);
            }
            else {
                nakedObject.ResolveState.Handle(oid.IsTransient ? Events.InitializeTransientEvent : Events.InitializePersistentEvent);
            }

            return nakedObject;
        }

        private INakedObject NewAdapterForViewModel(object domainObject, INakedObjectSpecification spec) {
            if (spec.IsViewModel) {
                INakedObject adapter = CreateAdapterForViewModel(domainObject, spec);
                adapter.ResolveState.Handle(Events.InitializePersistentEvent);
                return adapter;
            }
            return null;
        }

        private INakedObject NewAdapterForTransient(object domainObject) {
            INakedObject adapter = CreateAdapterForNewObject(domainObject);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        private INakedObject CreateAdapterForViewModel(object viewModel, INakedObjectSpecification spec) {
            var oid = new ViewModelOid(reflector, spec);
            INakedObject adapter = NewAdapterForKnownObject(viewModel, oid);

            object versionObject = adapter.GetVersion(this);
            if (versionObject != null) {
                adapter.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
                Log.DebugFormat("CreateAdapterForViewModel: Updating Version {0} on {1}", adapter.Version, adapter);
            }

            Log.DebugFormat("Creating adapter (ViewModel) {0}", adapter);
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        private INakedObject CreateAdapterForNewObject(object domainObject) {
            IOid transientOid = oidGenerator.CreateTransientOid(domainObject);
            INakedObject adapter = NewAdapterForKnownObject(domainObject, transientOid);
            Log.DebugFormat("Creating adapter (transient) {0}", adapter);
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        public INakedObject CreateInstanceAdapter(object obj) {
            INakedObject adapter = CreateAdapterForNewObject(obj);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        public INakedObject CreateViewModelAdapter(INakedObjectSpecification specification, object viewModel) {
            INakedObject adapter = CreateAdapterForViewModel(viewModel, specification);
            adapter.ResolveState.Handle(Events.InitializePersistentEvent);
            return adapter;
        }

        private INakedObject AdapterForService(IOid oid, object serv) {
            // do not use PersistorUtils here we want to avoid calling into NakedObjectsContext to avoid a stack overflow ! 
            INakedObject adapter = CreateAdapter(serv, oid, null);
            if (adapter.ResolveState.IsResolvable()) {
                adapter.ResolveState.Handle(Events.StartResolvingEvent);
                adapter.ResolveState.Handle(Events.EndResolvingEvent);
            }
            return adapter;
        }
    }

    internal class ServicesManager : IServicesManager {
        private static readonly ILog Log;
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly List<ServiceWrapper> services = new List<ServiceWrapper>();
        private readonly ISession session;
        private bool servicesInit;

        static ServicesManager() {
            Log = LogManager.GetLogger(typeof (ServicesManager));
        }

        public ServicesManager(IContainerInjector injector, INakedObjectManager manager, ServicesConfiguration servicesConfig, ISession session) {
            this.injector = injector;
            this.manager = manager;
            this.session = session;

            AddServices(servicesConfig.Services);
        }

        protected virtual List<ServiceWrapper> Services {
            get {
                if (!servicesInit) {
                    services.ForEach(sw => injector.InitDomainObject(sw.Service));
                    servicesInit = true;
                }

                return services;
            }
        }

        #region IServicesManager Members

        public virtual ServiceTypes GetServiceType(INakedObjectSpecification spec) {
            return Services.Where(sw => manager.GetServiceAdapter(sw.Service).Specification == spec).Select(sw => sw.ServiceType).FirstOrDefault();
        }

        public virtual INakedObject GetService(string id) {
            Log.DebugFormat("GetService: {0}", id);
            return (Services.Where(sw => id.Equals(ServiceUtils.GetId(sw.Service))).Select(sw => manager.GetServiceAdapter(sw.Service))).FirstOrDefault();
        }

        public virtual INakedObject[] GetServices() {
            Log.Debug("GetServices");
            return Services.Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType, ILifecycleManager persistor) {
            Log.DebugFormat("GetServicesWithVisibleActions of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).
                Where(no => no.Specification.GetObjectActions().Any(a => a.IsVisible(session, no, persistor))).ToArray();
        }

        public virtual INakedObject[] GetServices(ServiceTypes serviceType) {
            Log.DebugFormat("GetServices of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] ServiceAdapters {
            get { return Services.Select(x => manager.CreateAdapter(x.Service, null, null)).ToArray(); }
        }

        #endregion

        public void AddServices(IEnumerable<ServiceWrapper> ss) {
            Log.DebugFormat("AddServices count: {0}", ss.Count());
            services.AddRange(ss);
        }
    }

    public class LifeCycleManager : ILifecycleManager {
        private static readonly ILog Log;
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly IObjectPersistor objectPersistor;
        private readonly IPersistAlgorithm persistAlgorithm;
        private readonly INakedObjectReflector reflector;
        private readonly IServicesManager servicesManager;
        private readonly ISession session;
        private readonly INakedObjectTransactionManager transactionManager;

        static LifeCycleManager() {
            Log = LogManager.GetLogger(typeof (LifeCycleManager));
        }

        public LifeCycleManager(ISession session, IUpdateNotifier updateNotifier, INakedObjectReflector reflector, INakedObjectStore objectStore, IPersistAlgorithm persistAlgorithm, IOidGenerator oidGenerator, IIdentityMap identityMap, IContainerInjector injector, ServicesConfiguration servicesConfig) {
            Assert.AssertNotNull(objectStore);
            Assert.AssertNotNull(persistAlgorithm);
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(identityMap);
            Assert.AssertNotNull(reflector);

            transactionManager = new ObjectStoreTransactionManager(objectStore);
            objectPersistor = new ObjectPersistor(objectStore, transactionManager, session, this, updateNotifier);

            var nakedObjectFactory = new NakedObjectFactory();

            manager = new NakedObjectManager(reflector, session, identityMap, oidGenerator, nakedObjectFactory);

            servicesManager = new ServicesManager(injector, manager, servicesConfig, session);

            this.session = session;
            this.reflector = reflector;

            this.persistAlgorithm = persistAlgorithm;

            this.injector = injector;

            nakedObjectFactory.Initialize(reflector, session, this);

            // TODO - fix !
            objectStore.Manager = this;

            Log.DebugFormat("Creating {0}", this);


            this.injector.ServiceTypes = servicesConfig.Services.Select(sw => sw.Service.GetType()).ToArray();
        }

        public string DebugTitle {
            get { return "Object Store Persistor"; }
        }

        #region ILifecycleManager Members

        public IOidGenerator OidGenerator {
            get { return manager.OidGenerator; }
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
                if (adapter.Specification != specification) {
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

        public virtual INakedObject GetAdapterFor(object obj) {
            return manager.GetAdapterFor(obj);
        }

        public virtual INakedObject GetAdapterFor(IOid oid) {
            return manager.GetAdapterFor(oid);
        }

        public virtual INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
            return manager.CreateAdapter(domainObject, oid, version);
        }

        public void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
            manager.ReplacePoco(nakedObject, newDomainObject);
        }

        public virtual void RemoveAdapter(INakedObject nakedObject) {
            manager.RemoveAdapter(nakedObject);
        }

        public virtual IQueryable<T> Instances<T>() where T : class {
            return objectPersistor.Instances<T>();
        }

        public virtual IQueryable Instances(Type type) {
            return objectPersistor.Instances(type);
        }

        public virtual IQueryable Instances(INakedObjectSpecification specification) {
            return objectPersistor.Instances(specification);
        }

        public virtual ServiceTypes GetServiceType(INakedObjectSpecification spec) {
            return servicesManager.GetServiceType(spec);
        }

        public virtual INakedObject GetService(string id) {
            return servicesManager.GetService(id);
        }

        // TODO REVIEW why does this get called multiple times when starting up
        public virtual INakedObject[] GetServices() {
            return servicesManager.GetServices();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType, ILifecycleManager persistor) {
            return servicesManager.GetServicesWithVisibleActions(serviceType, this);
        }

        public virtual INakedObject[] GetServices(ServiceTypes serviceType) {
            return servicesManager.GetServices(serviceType);
        }

        public virtual INakedObject[] ServiceAdapters {
            get { return servicesManager.ServiceAdapters; }
        }

        public INakedObject LoadObject(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, specification);
            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", specification);
            return manager.GetKnownAdapter(oid) ?? objectPersistor.LoadObject(oid, specification);
        }

        public void Reload(INakedObject nakedObject) {
            objectPersistor.Reload(nakedObject);
        }

        public void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            objectPersistor.ResolveField(nakedObject, field);
        }

        public void LoadField(INakedObject nakedObject, string field) {
            objectPersistor.LoadField(nakedObject, field);
        }

        public int CountField(INakedObject nakedObject, string field) {
            return objectPersistor.CountField(nakedObject, field);
        }

        public void ResolveImmediately(INakedObject nakedObject) {
            objectPersistor.ResolveImmediately(nakedObject);
        }

        public void ObjectChanged(INakedObject nakedObject) {
            objectPersistor.ObjectChanged(nakedObject);
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
            if (nakedObject.Specification.Persistable == Persistable.TRANSIENT) {
                throw new NotPersistableException("Object must be kept transient: " + nakedObject);
            }
            INakedObjectSpecification specification = nakedObject.Specification;
            if (specification.IsService) {
                throw new NotPersistableException("Cannot persist services: " + nakedObject);
            }

            persistAlgorithm.MakePersistent(nakedObject, this, session);
        }

        /// <summary>
        ///     Removes the specified object from the system. The specified object's data should be removed from the
        ///     persistence mechanism.
        /// </summary>
        public void DestroyObject(INakedObject nakedObject) {
            objectPersistor.DestroyObject(nakedObject);
        }

        public object CreateObject(INakedObjectSpecification specification) {
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

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            return objectPersistor.GetKeys(type);
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            Log.Debug("FindByKeys");
            return objectPersistor.FindByKeys(type, keys);
        }

        public void Refresh(INakedObject nakedObject) {
            Log.DebugFormat("Refresh nakedObject: {0}", nakedObject);
            objectPersistor.Refresh(nakedObject);
        }

        public INakedObject NewAdapterForKnownObject(object domainObject, IOid transientOid) {
            return manager.NewAdapterForKnownObject(domainObject, transientOid);
        }

        public INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
            return manager.CreateAggregatedAdapter(parent, fieldId, obj);
        }

        public List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
            return manager.GetCollectionOfAdaptedObjects(domainObjects);
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
            INakedObjectSpecification spec = reflector.LoadSpecification(typeName);

            if (spec.IsCollection) {
                return new CollectionMemento(this, reflector, session, encodedData);
            }

            if (spec.ContainsFacet<IViewModelFacet>()) {
                return new ViewModelOid(reflector, encodedData);
            }

            return spec.ContainsFacet<IComplexTypeFacet>() ? new AggregateOid(reflector, encodedData) : null;
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

        public virtual void AddPersistedObject(INakedObject nakedObject) {
            objectPersistor.AddPersistedObject(nakedObject);
        }

        public virtual void MadePersistent(INakedObject nakedObject) {
            manager.MadePersistent(nakedObject);
        }

        public virtual void UpdateViewModel(INakedObject adapter, string[] keys) {
            manager.UpdateViewModel(adapter, keys);
        }

        public INakedObject GetServiceAdapter(object service) {
            return manager.GetServiceAdapter(service);
        }

        public INakedObject GetKnownAdapter(IOid oid) {
            return manager.GetKnownAdapter(oid);
        }

        public INakedObject CreateViewModelAdapter(INakedObjectSpecification specification, object viewModel) {
            return manager.CreateViewModelAdapter(specification, viewModel);
        }

        public INakedObject CreateInstanceAdapter(object obj) {
            return manager.CreateInstanceAdapter(obj);
        }

        public INakedObject AdapterForExistingObject(object domainObject, IOid oid) {
            return manager.AdapterForExistingObject(domainObject, oid);
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
            UpdateViewModel(vm, keys);
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
            nakedObject.Specification.Properties.ForEach(field => field.ToDefault(nakedObject, this));
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