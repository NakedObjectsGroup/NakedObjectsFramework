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
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class NakedObjectManager : INakedObjectManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NakedObjectManager));
        private readonly INoIdentityAdapterCache adapterCache = new NoIdentityAdapterCache();
        private readonly IIdentityMap identityMap;
        private readonly IMetamodelManager metamodel;
        private readonly NakedObjectFactory nakedObjectFactory;
        private readonly IOidGenerator oidGenerator;
        private readonly ISession session;

        public NakedObjectManager(IMetamodelManager metamodel, ISession session, IIdentityMap identityMap, IOidGenerator oidGenerator, NakedObjectFactory nakedObjectFactory) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(identityMap);
            Assert.AssertNotNull(oidGenerator);
            Assert.AssertNotNull(nakedObjectFactory);

            this.metamodel = metamodel;
            this.session = session;
            this.identityMap = identityMap;
            this.oidGenerator = oidGenerator;
            this.nakedObjectFactory = nakedObjectFactory;
        }

        #region INakedObjectManager Members

        public void RemoveAdapter(INakedObjectAdapter objectAdapterToDispose) {
            identityMap.Unloaded(objectAdapterToDispose);
        }

        public INakedObjectAdapter GetAdapterFor(object obj) {
            Assert.AssertNotNull("must have a domain object", obj);
            INakedObjectAdapter nakedObjectAdapter = identityMap.GetAdapterFor(obj);
            if (nakedObjectAdapter != null && nakedObjectAdapter.Object != obj) {
                throw new AdapterException(Log.LogAndReturn($"Mapped adapter is for different domain object: {obj}; {nakedObjectAdapter}"));
            }
            return nakedObjectAdapter;
        }

        public INakedObjectAdapter GetAdapterFor(IOid oid) {
            Assert.AssertNotNull("must have an OID", oid);
            return identityMap.GetAdapterFor(oid);
        }

        public INakedObjectAdapter CreateAdapter(object domainObject, IOid oid, IVersion version) {
            if (domainObject == null) {
                return null;
            }
            if (oid == null) {
                ITypeSpec objectSpec = metamodel.GetSpecification(domainObject.GetType());
                if (objectSpec.ContainsFacet(typeof (IComplexTypeFacet))) {
                    return GetAdapterFor(domainObject);
                }
                if (objectSpec.HasNoIdentity) {
                    return AdapterForNoIdentityObject(domainObject);
                }
                return AdapterForExistingObject(domainObject, objectSpec);
            }
            return AdapterForExistingObject(domainObject, oid);
        }

        public void ReplacePoco(INakedObjectAdapter nakedObjectAdapter, object newDomainObject) {
            RemoveAdapter(nakedObjectAdapter);
            identityMap.Replaced(nakedObjectAdapter.Object);
            nakedObjectAdapter.ReplacePoco(newDomainObject);
            identityMap.AddAdapter(nakedObjectAdapter);
        }

        public void MadePersistent(INakedObjectAdapter nakedObjectAdapter) {
            identityMap.MadePersistent(nakedObjectAdapter);
        }

        public void UpdateViewModel(INakedObjectAdapter adapter, string[] keys) {
            identityMap.UpdateViewModel(adapter, keys);
        }

        public INakedObjectAdapter CreateAggregatedAdapter(INakedObjectAdapter parent, string fieldId, object obj) {
            GetAdapterFor(obj);

            IOid oid = new AggregateOid(metamodel, parent.Oid, fieldId, obj.GetType().FullName);
            INakedObjectAdapter adapterFor = GetAdapterFor(oid);
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

        public INakedObjectAdapter NewAdapterForKnownObject(object domainObject, IOid transientOid) {
            return nakedObjectFactory.CreateAdapter(domainObject, transientOid);
        }

        public List<INakedObjectAdapter> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
            return (from object domainObject in domainObjects
                select CreateAdapter(domainObject, null, null)).ToList();
        }

        public INakedObjectAdapter GetServiceAdapter(object service) {
            IOid oid = GetOidForService(ServiceUtils.GetId(service), service.GetType().FullName);
            return AdapterForService(oid, service);
        }

        public INakedObjectAdapter GetKnownAdapter(IOid oid) {
            if (identityMap.IsIdentityKnown(oid)) {
                return GetAdapterFor(oid);
            }
            return null;
        }

        public INakedObjectAdapter AdapterForExistingObject(object domainObject, IOid oid) {
            return GetAdapterFor(domainObject) ?? NewAdapterBasedOnOid(domainObject, oid);
        }

        public INakedObjectAdapter CreateInstanceAdapter(object obj) {
            INakedObjectAdapter adapter = CreateAdapterForNewObject(obj);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        public INakedObjectAdapter CreateViewModelAdapter(IObjectSpec spec, object viewModel) {
            INakedObjectAdapter adapter = CreateAdapterForViewModel(viewModel, spec);
            adapter.ResolveState.Handle(Events.InitializePersistentEvent);
            return adapter;
        }

        #endregion

        private IOid GetOidForService(string name, string typeName) {
            return oidGenerator.CreateOid(typeName, new object[] {0});
        }

        private INakedObjectAdapter AdapterForNoIdentityObject(object domainObject) {
            INakedObjectAdapter adapter = adapterCache.GetAdapter(domainObject);

            if (adapter == null) {
                adapter = NewAdapterForKnownObject(domainObject, null);
                NewTransientsResolvedState(adapter);
                adapterCache.AddAdapter(adapter);
            }

            return adapter;
        }

        private INakedObjectAdapter AdapterForExistingObject(object domainObject, ITypeSpec spec) {
            return identityMap.GetAdapterFor(domainObject) ?? NewAdapterForViewModel(domainObject, spec) ?? NewAdapterForTransient(domainObject);
        }

        private static void NewTransientsResolvedState(INakedObjectAdapter adapter) {
            adapter.ResolveState.Handle(adapter.Spec.IsAggregated ? Events.InitializeAggregateEvent : Events.InitializeTransientEvent);
        }

        private INakedObjectAdapter NewAdapterBasedOnOid(object domainObject, IOid oid) {
            INakedObjectAdapter nakedObjectAdapter = NewAdapterForKnownObject(domainObject, oid);
            identityMap.AddAdapter(nakedObjectAdapter);

            if (oid is AggregateOid && nakedObjectAdapter.Spec.IsObject) {
                nakedObjectAdapter.ResolveState.Handle(Events.InitializeAggregateEvent);
            }
            else {
                nakedObjectAdapter.ResolveState.Handle(oid.IsTransient ? Events.InitializeTransientEvent : Events.InitializePersistentEvent);
            }

            return nakedObjectAdapter;
        }

        private INakedObjectAdapter NewAdapterForViewModel(object domainObject, ITypeSpec spec) {
            if (spec.IsViewModel) {
                INakedObjectAdapter adapter = CreateAdapterForViewModel(domainObject, (IObjectSpec) spec);
                adapter.ResolveState.Handle(Events.InitializePersistentEvent);
                return adapter;
            }
            return null;
        }

        private INakedObjectAdapter NewAdapterForTransient(object domainObject) {
            INakedObjectAdapter adapter = CreateAdapterForNewObject(domainObject);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        private INakedObjectAdapter CreateAdapterForViewModel(object viewModel, IObjectSpec spec) {
            var oid = new ViewModelOid(metamodel, spec);
            INakedObjectAdapter adapter = NewAdapterForKnownObject(viewModel, oid);

            object versionObject = adapter.GetVersion(this);
            if (versionObject != null) {
                adapter.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
            }
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        private INakedObjectAdapter CreateAdapterForNewObject(object domainObject) {
            IOid transientOid = oidGenerator.CreateTransientOid(domainObject);
            INakedObjectAdapter adapter = NewAdapterForKnownObject(domainObject, transientOid);
            identityMap.AddAdapter(adapter);
            return adapter;
        }

        private INakedObjectAdapter AdapterForService(IOid oid, object serv) {
            // do not use PersistorUtils here we want to avoid calling into NakedObjectsContext to avoid a stack overflow ! 
            INakedObjectAdapter adapter = CreateAdapter(serv, oid, null);
            if (adapter.ResolveState.IsResolvable()) {
                adapter.ResolveState.Handle(Events.StartResolvingEvent);
                adapter.ResolveState.Handle(Events.EndResolvingEvent);
            }
            return adapter;
        }
    }
}