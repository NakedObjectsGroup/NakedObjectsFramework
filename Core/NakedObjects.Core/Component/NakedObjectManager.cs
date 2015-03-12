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

        public void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
            Log.DebugFormat("ReplacePoco nakedObject: {0} newDomainOject: {1}", nakedObject, newDomainObject);
            RemoveAdapter(nakedObject);
            identityMap.Replaced(nakedObject.Object);
            nakedObject.ReplacePoco(newDomainObject);
            identityMap.AddAdapter(nakedObject);
        }

        public void MadePersistent(INakedObject nakedObject) {
            identityMap.MadePersistent(nakedObject);
        }

        public void UpdateViewModel(INakedObject adapter, string[] keys) {
            identityMap.UpdateViewModel(adapter, keys);
        }

        public INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
            GetAdapterFor(obj);

            IOid oid = new AggregateOid(metamodel, parent.Oid, fieldId, obj.GetType().FullName);
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

        public INakedObject GetKnownAdapter(IOid oid) {
            if (identityMap.IsIdentityKnown(oid)) {
                return GetAdapterFor(oid);
            }
            return null;
        }

        public INakedObject AdapterForExistingObject(object domainObject, IOid oid) {
            return GetAdapterFor(domainObject) ?? NewAdapterBasedOnOid(domainObject, oid);
        }

        public INakedObject CreateInstanceAdapter(object obj) {
            INakedObject adapter = CreateAdapterForNewObject(obj);
            NewTransientsResolvedState(adapter);
            return adapter;
        }

        public INakedObject CreateViewModelAdapter(IObjectSpec spec, object viewModel) {
            INakedObject adapter = CreateAdapterForViewModel(viewModel, spec);
            adapter.ResolveState.Handle(Events.InitializePersistentEvent);
            return adapter;
        }

        #endregion

        private IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            return oidGenerator.CreateOid(typeName, new object[] {0});
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

        private INakedObject AdapterForExistingObject(object domainObject, ITypeSpec spec) {
            return identityMap.GetAdapterFor(domainObject) ?? NewAdapterForViewModel(domainObject, spec) ?? NewAdapterForTransient(domainObject);
        }

        private static void NewTransientsResolvedState(INakedObject pocoAdapter) {
            pocoAdapter.ResolveState.Handle(pocoAdapter.Spec.IsAggregated ? Events.InitializeAggregateEvent : Events.InitializeTransientEvent);
        }

        private INakedObject NewAdapterBasedOnOid(object domainObject, IOid oid) {
            INakedObject nakedObject = NewAdapterForKnownObject(domainObject, oid);
            identityMap.AddAdapter(nakedObject);

            if (oid is AggregateOid && nakedObject.Spec.IsObject) {
                nakedObject.ResolveState.Handle(Events.InitializeAggregateEvent);
            }
            else {
                nakedObject.ResolveState.Handle(oid.IsTransient ? Events.InitializeTransientEvent : Events.InitializePersistentEvent);
            }

            return nakedObject;
        }

        private INakedObject NewAdapterForViewModel(object domainObject, ITypeSpec spec) {
            if (spec.IsViewModel) {
                INakedObject adapter = CreateAdapterForViewModel(domainObject, (IObjectSpec) spec);
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

        private INakedObject CreateAdapterForViewModel(object viewModel, IObjectSpec spec) {
            var oid = new ViewModelOid(metamodel, spec);
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
}