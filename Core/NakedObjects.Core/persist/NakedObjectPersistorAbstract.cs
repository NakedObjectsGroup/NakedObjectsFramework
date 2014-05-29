// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.Service;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;

namespace NakedObjects.Core.Persist {
    public abstract class NakedObjectPersistorAbstract : INakedObjectPersistor {
        private static readonly ILog Log;
        protected INoIdentityAdapterCache adapterCache = new NoIdentityAdapterCache();
        private IContainerInjector containerInjector;
        private IIdentityMap identityMap;
        private IOidGenerator oidGenerator;
        protected List<ServiceWrapper> services = new List<ServiceWrapper>();

        static NakedObjectPersistorAbstract() {
            Log = LogManager.GetLogger(typeof (NakedObjectPersistorAbstract));
        }

        public virtual IOidGenerator OidGenerator {
            set { oidGenerator = value; }
        }

        public virtual IIdentityMap IdentityMap {
            get { return identityMap; }
            set { identityMap = value; }
        }

        protected virtual List<ServiceWrapper> Services {
            get { return services; }
        }

        #region INakedObjectPersistor Members

        public abstract string DebugTitle { get; }

        public virtual void Init() {
            Log.Debug("Init");
            Assert.AssertNotNull("Identity map missing", identityMap);
            Assert.AssertNotNull("OID generator required", oidGenerator);
            identityMap.Init();
            oidGenerator.Init();
            InitServices();
        }

        public virtual void Shutdown() {
            Log.Debug("Shutdown");
            if (identityMap != null) {
                identityMap.Shutdown();
                identityMap = null;
            }

            if (oidGenerator != null) {
                oidGenerator.Shutdown();
                oidGenerator = null;
            }
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

            PocoAdapter adapter = CreateAdapterForNewObject(obj);
            InitializeNewObject(adapter);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        public INakedObject CreateViewModel(INakedObjectSpecification specification) {
            Log.DebugFormat("CreateViewModel of: {0}", specification);

            object viewModel = CreateObject(specification);
            PocoAdapter adapter = CreateAdapterForViewModel(viewModel, specification);
            InitializeNewObject(adapter);
            adapter.ResolveState.Handle(Events.InitializePersistentEvent);
            return adapter;
        }

        public virtual INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification) {
            Log.DebugFormat("RecreateInstance oid: {0} hint: {1}", oid, specification);
            INakedObject adapter = identityMap.GetAdapterFor(oid);
            if (adapter != null) {
                if (adapter.Specification != specification) {
                    throw new AdapterException(string.Format("Mapped adapter is for a different type of object: {0}; {1}", specification.FullName, adapter));
                }
                return adapter;
            }

            Log.DebugFormat("Recreating instance for {0}", specification);
            object obj = CreateObject(specification);

            return AdapterForExistingObject(obj, oid);
        }

        public virtual INakedObject GetViewModel(IOid oid) {
            if (identityMap.IsIdentityKnown(oid)) {
                return GetAdapterFor(oid);
            }

            return RecreateViewModel((ViewModelOid)oid);
        }

        private INakedObject RecreateViewModel(ViewModelOid oid) {
            string[] keys = oid.Keys;
            INakedObjectSpecification spec = oid.Specification;

            INakedObject vm = CreateViewModel(spec);
            vm.Specification.GetFacet<IViewModelFacet>().Populate(keys, vm);
            UpdateViewModel(vm, keys);
            return vm;
        }


        public virtual INakedObject GetAdapterFor(object obj) {
            Log.DebugFormat("GetAdapterFor: {0}", obj);
            Assert.AssertNotNull("must have a domain object", obj);
            INakedObject nakedObject = IdentityMap.GetAdapterFor(obj);
            if (nakedObject != null && nakedObject.Object != obj) {
                throw new AdapterException("Mapped adapter is for different domain object: " + obj + "; " + nakedObject);
            }
            return nakedObject;
        }

        public virtual INakedObject GetAdapterFor(IOid oid) {
            Log.DebugFormat("GetAdapterFor oid: {0}", oid);
            Assert.AssertNotNull("must have an OID", oid);
            return IdentityMap.GetAdapterFor(oid);
        }

        public virtual INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
            Log.DebugFormat("AdapterFor domainObject: {0} oid: {1} version: {2}", domainObject, oid, version);
            if (domainObject == null) {
                return null;
            }
            if (oid == null) {
                INakedObjectSpecification nakedObjectSpecification = NakedObjectsContext.Reflector.LoadSpecification(domainObject.GetType());
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

        public virtual void RemoveAdapter(INakedObject nakedObject) {
            Log.DebugFormat("RemoveAdapter nakedObject: {0}", nakedObject);
            identityMap.Unloaded(nakedObject);
        }

        public virtual IOid CreateTransientOid(object obj) {
            return oidGenerator.CreateTransientOid(obj);
            // don't log until after oid created or we may fail with a duplicate object in map (if title triggers Resolve)
        }

        public virtual void ConvertPersistentToTransientOid(IOid oid) {
            Log.DebugFormat("ConvertPersistentToTransientOid oid: {0}", oid);
            oidGenerator.ConvertPersistentToTransientOid(oid);
        }

        public virtual void ConvertTransientToPersistentOid(IOid oid) {
            Log.DebugFormat("ConvertTransientToPersistentOid oid: {0}", oid);
            oidGenerator.ConvertTransientToPersistentOid(oid);
        }

        public virtual void UpdateViewModel(INakedObject adapter, string[] keys) {
            identityMap.UpdateViewModel(adapter, keys);
        }

        public void AddServices(IEnumerable<ServiceWrapper> services) {
            Log.DebugFormat("AddServices count: {0}", services.Count());
            Services.AddRange(services);
        }

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

        public virtual void InitDomainObject(object obj) {
            Log.DebugFormat("InitDomainObject: {0}", obj);
            containerInjector.InitDomainObject(obj);
        }

        public void InitInlineObject(object root, object inlineObject) {
            Log.DebugFormat("InitInlineObject root: {0} inlineObject: {1}", root, inlineObject);
            containerInjector.InitInlineObject(root, inlineObject);
        }

        public virtual ServiceTypes GetServiceType(INakedObjectSpecification spec) {
            return Services.Where(sw => GetServiceAdapter(sw.Service).Specification == spec).Select(sw => sw.ServiceType).FirstOrDefault();
        }

        public virtual INakedObject GetService(string id) {
            Log.DebugFormat("GetService: {0}", id);
            return (Services.Where(sw => id.Equals(ServiceUtils.GetId(sw.Service))).Select(sw => GetServiceAdapter(sw.Service))).FirstOrDefault();
        }

        // TODO REVIEW why does this get called multiple times when starting up
        public virtual INakedObject[] GetServices() {
            Log.Debug("GetServices");
            return Services.Select(sw => GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType) {
            Log.DebugFormat("GetServicesWithVisibleActions of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                            Select(sw => GetServiceAdapter(sw.Service)).
                            Where(no => no.Specification.GetObjectActions().Any(a => a.IsVisible(NakedObjectsContext.Session, no))).ToArray();
        }

        public virtual INakedObject[] GetServices(ServiceTypes serviceType) {
            Log.DebugFormat("GetServices of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                            Select(sw => GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] ServiceAdapters {
            get { return Services.Select(x => PersistorUtils.CreateAdapter(x.Service)).ToArray(); }
        }

        public virtual IOid RestoreOid(string[] encodedData) {
            Log.DebugFormat("RestoreOid: {0}", encodedData.Aggregate("", (s, t) => s + "," + t));
            return oidGenerator.RestoreOid(encodedData);
        }

        public abstract PropertyInfo[] GetKeys(Type type);
        public abstract void Refresh(INakedObject nakedObject);
        public abstract bool IsInitialized { get; set; }
        public abstract void Reset();
        public abstract void LoadField(INakedObject nakedObject, string field);
        public abstract int CountField(INakedObject nakedObject, string field);
        public abstract void ResolveField(INakedObject nakedObject, INakedObjectAssociation field);
        public abstract void ResolveImmediately(INakedObject nakedObject);
        public abstract void ObjectChanged(INakedObject nakedObject);
        public abstract void MakePersistent(INakedObject nakedObject);
        public abstract void DestroyObject(INakedObject nakedObject);
        public abstract void StartTransaction();
        public abstract bool FlushTransaction();
        public abstract void AbortTransaction();
        public abstract void UserAbortTransaction();
        public abstract void EndTransaction();
        public abstract void AddCommand(IPersistenceCommand command);
        public abstract INakedObject LoadObject(IOid oid, INakedObjectSpecification spec);
        public abstract void Reload(INakedObject nakedObject);
        public abstract object CreateObject(INakedObjectSpecification specification);
        public abstract INakedObject FindByKeys(Type type, object[] keys);

        private PocoAdapter CreateAdapterForViewModel(object viewModel, INakedObjectSpecification spec) {
            var oid = new ViewModelOid(spec);
            var adapter = new PocoAdapter(viewModel, oid);

            object versionObject = adapter.GetVersion();
            if (versionObject != null) {
                adapter.OptimisticLock = new ConcurrencyCheckVersion(NakedObjectsContext.Session.UserName, DateTime.Now, versionObject);
                Log.DebugFormat("CreateAdapterForViewModel: Updating Version {0} on {1}", adapter.Version, adapter);
            }

            Log.DebugFormat("Creating adapter (ViewModel) {0}", adapter);
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        private INakedObject AdapterForExistingObject(object domainObject, INakedObjectSpecification spec) {
            return identityMap.GetAdapterFor(domainObject) ?? NewAdapterForViewModel(domainObject, spec) ?? NewAdapterForTransient(domainObject);
        }

        private INakedObject AdapterForExistingObject(object domainObject, IOid oid) {
            return GetAdapterFor(domainObject) ?? NewAdapterBasedOnOid(domainObject, oid);
        }

        private INakedObject NewAdapterBasedOnOid(object domainObject, IOid oid) {
            INakedObject nakedObject = new PocoAdapter(domainObject, oid);
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
                PocoAdapter adapter = CreateAdapterForViewModel(domainObject, spec);
                adapter.ResolveState.Handle(Events.InitializePersistentEvent);
                return adapter;
            }
            return null; 
        }


        private INakedObject NewAdapterForTransient(object domainObject) {
            PocoAdapter adapter = CreateAdapterForNewObject(domainObject);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        private INakedObject AdapterForNoIdentityObject(object domainObject) {
            INakedObject pocoAdapter = adapterCache.GetAdapter(domainObject);

            if (pocoAdapter == null) {
                pocoAdapter = new PocoAdapter(domainObject, null);
                NewTransientsResolvedState(pocoAdapter);
                adapterCache.AddAdapter(pocoAdapter);
            }

            return pocoAdapter;
        }

        #endregion

        private INakedObject GetServiceAdapter(object service) {
            IOid oid = GetOidForService(ServiceUtils.GetId(service), service.GetType().FullName);
            return AdapterForService(oid, service);
        }

        private void InitServices() {
            INakedObjectReflector reflector = NakedObjectsContext.Reflector;
           
            reflector.InstallServiceSpecifications(Services.Select(s => s.Service.GetType()).ToArray());


            reflector.PopulateContributedActions(GetServices(ServiceTypes.Menu | ServiceTypes.Contributor));

            containerInjector = NakedObjectsContext.Reflector.CreateContainerInjector(Services.Select(x => x.Service).ToArray());
            foreach (ServiceWrapper service in Services) {
                containerInjector.InitDomainObject(service.Service);
            }
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

        private static void NewTransientsResolvedState(INakedObject pocoAdapter) {
            pocoAdapter.ResolveState.Handle(pocoAdapter.Specification.IsAggregated ? Events.InitializeAggregateEvent : Events.InitializeTransientEvent);
        }

        private PocoAdapter CreateAdapterForNewObject(object domainObject) {
            IOid transientOid = CreateTransientOid(domainObject);
            var adapter = new PocoAdapter(domainObject, transientOid);
            Log.DebugFormat("Creating adapter (transient) {0}", adapter);
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        private void CreateInlineObjects(INakedObject parentObject, object rootObject) {
            foreach (IOneToOneAssociation assoc in parentObject.Specification.Properties.Where(p => p.IsInline)) {
                object inlineObject = CreateObject(assoc.Specification);

                InitInlineObject(rootObject, inlineObject);
                INakedObject inlineNakedObject = PersistorUtils.CreateAggregatedAdapter(parentObject, assoc, inlineObject);
                InitializeNewObject(inlineNakedObject, rootObject);
                assoc.InitAssociation(parentObject, inlineNakedObject);
            }
        }

        private void InitializeNewObject(INakedObject nakedObject, object rootObject) {
            nakedObject.Specification.Properties.ForEach(field => field.ToDefault(nakedObject));
            CreateInlineObjects(nakedObject, rootObject);
            nakedObject.Created();
        }

        private void InitializeNewObject(INakedObject nakedObject) {
            InitializeNewObject(nakedObject, nakedObject.GetDomainObject());
        }


        protected abstract IQueryable<T> GetInstances<T>() where T : class;

        protected abstract IQueryable GetInstances(Type type);

        protected abstract IQueryable GetInstances(INakedObjectSpecification specification);

        /// <summary>
        ///     Returns the OID for the adapted service. This allows a service object to be given the same OID that it
        ///     had when it was created in a different session.
        /// </summary>
        protected abstract IOid GetOidForService(string name, string typeName);

        /// <summary>
        ///     Registers the specified service as having the specified OID
        /// </summary>
        protected abstract void RegisterService(string name, IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}