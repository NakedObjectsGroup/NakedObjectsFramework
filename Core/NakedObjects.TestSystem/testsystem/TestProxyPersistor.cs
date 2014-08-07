// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Security;
using NUnit.Framework;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;

namespace NakedObjects.TestSystem {
    public class TestProxyPersistor : INakedObjectPersistor {
        private readonly IList<string> actions = new List<string>();
        private readonly IDictionary<object, INakedObject> domainObjectMap = new Dictionary<object, INakedObject>();
        private readonly IDictionary<IOid, INakedObject> identityMap = new Dictionary<IOid, INakedObject>();
        private readonly IDictionary<IOid, INakedObject> objects = new Dictionary<IOid, INakedObject>();

        public TestProxyPersistor() {
            OidGenerator = new TestProxyOidGenerator();
        }

        #region INakedObjectPersistor Members

        public void AbortTransaction() {}

        public void UserAbortTransaction() {
            actions.Add("User Abort transaction");
        }

        public void DestroyObject(INakedObject nakedObject) {
            actions.Add("object deleted " + nakedObject.Oid);
        }

        public INakedObject GetService(string id) {
            throw new NotImplementedException();
        }

        public ServiceTypes GetServiceType(INakedObjectSpecification spec) {
            throw new NotImplementedException();
        }

        public INakedObject[] GetServices() {
            throw new NotImplementedException();
        }

        public INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType) {
            return new INakedObject[0];
        }

        public INakedObject[] ServiceAdapters {
            get { return new INakedObject[0]; }
        }

        public IOidGenerator OidGenerator { get; private set; }

        public PropertyInfo[] GetKeys(Type type) {
            throw new NotImplementedException();
        }

        public void Refresh(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public int CountField(INakedObject nakedObject, string fieldName) {
            return 0;
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            throw new NotImplementedException();
        }

        public INakedObject[] GetServices(ServiceTypes serviceType) {
           return new INakedObject[]{};
        }

        public INakedObject CreateAggregatedAdapter(INakedObject parent, string fieldId, object obj) {
            throw new NotImplementedException();
        }

        public List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects) {
            throw new NotImplementedException();
        }

        public IOid RestoreGenericOid(string[] encodedData) {
            throw new NotImplementedException();
        }

        public void PopulateViewModelKeys(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public IOid RestoreOid(string[] encodedData) {
            throw new NotImplementedException();
        }

        public void EndTransaction() {
            actions.Add("end transaction");
        }

        public INakedObject CreateAdapter(object domainObject, IOid oid, IVersion version) {
            if (oid != null && identityMap.ContainsKey(oid)) {
                INakedObject adapter = identityMap[oid];
                if (adapter.Specification.HasNoIdentity) {
                    throw new Exception();
                }
                return adapter;
            }

            if (domainObjectMap.ContainsKey(domainObject)) {
                return domainObjectMap[domainObject];
            }

            if (oid == null) {
                oid = CreateTransientOid(domainObject);
            }

            var testProxyNakedObject = new TestProxyNakedObject();
            testProxyNakedObject.SetupObject(domainObject);
            testProxyNakedObject.SetupOid(oid);
            testProxyNakedObject.SetupVersion(version);
            /*        testProxyNakedObject.SetupResolveState(oid == null ? ResolveState.AGGREGATED : oid.isTransient() ? ResolveState.TRANSIENT
                            : ResolveState.GHOST);
               */

            testProxyNakedObject.SetupSpecification(NakedObjectsContext.Reflector.LoadSpecification(domainObject.GetType()));
            AddAdapter(domainObject, testProxyNakedObject);
            return testProxyNakedObject;
        }

        public void ReplacePoco(INakedObject nakedObject, object newDomainObject) {
            throw new NotImplementedException();
        }

        public object CreateObject(INakedObjectSpecification specification) {
            throw new NotImplementedException();
        }

        public void UpdateViewModel(INakedObject nakedObject, string[] keys) {
            throw new NotImplementedException();
        }

        public INakedObject GetViewModel(IOid oid) {
            throw new NotImplementedException();
        }

        public INakedObject LoadObject(IOid oid, INakedObjectSpecification spec) {
            throw new NotImplementedException();
        }

        public ISession Session { get; set; }
        public object UpdateNotifier { get; set; }

        public bool IsInitialized {
            get { return true; }
            set { }
        }

        public void Init() {}

        public void MakePersistent(INakedObject nakedObject) {
            if (nakedObject.Specification.HasNoIdentity) {
                throw new Exception();
            }
            objects.Remove(nakedObject.Oid);
            identityMap.Remove(nakedObject.Oid);
            ConvertTransientToPersistentOid(nakedObject.Oid);
            nakedObject.OptimisticLock = (new TestProxyVersion(1));

            nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
            nakedObject.ResolveState.Handle(Events.EndResolvingEvent);

            objects[nakedObject.Oid] = nakedObject;
            identityMap[nakedObject.Oid] = nakedObject;
        }

        public void ObjectChanged(INakedObject nakedObject) {
            actions.Add("object changed " + nakedObject.Oid);
            nakedObject.OptimisticLock = (((TestProxyVersion) nakedObject.Version).Next());
        }

        public void Reload(INakedObject nakedObject) {}

        public void Reset() {
            actions.Clear();
            domainObjectMap.Clear();
            identityMap.Clear();
        }

        public void AddServices(IEnumerable<ServiceWrapper> services) {
            throw new NotImplementedException();
        }

        public void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {}

        public void LoadField(INakedObject nakedObject, string fieldName) {
            throw new NotImplementedException();
        }

        public void ResolveImmediately(INakedObject nakedObject) {}

        public void Shutdown() {}

        public void StartTransaction() {
            actions.Add("Start transaction");
        }

        public bool FlushTransaction() {
            actions.Add("flush transaction");
            return false;
        }

        public void AddCommand(IPersistenceCommand command) {}
        public void Abort(INakedObjectPersistor objectManager, IFacetHolder holder) {
            throw new NotImplementedException();
        }

        public IOid CreateTransientOid(object obj) {
            return OidGenerator.CreateTransientOid(obj);
        }

        public void ConvertTransientToPersistentOid(IOid oid) {
            OidGenerator.ConvertTransientToPersistentOid(oid);
        }

        public void ConvertPersistentToTransientOid(IOid oid) {
            OidGenerator.ConvertPersistentToTransientOid(oid);
        }

        public IQueryable<T> Instances<T>() where T : class {
            throw new NotImplementedException();
        }

        public IQueryable Instances(Type type) {
            throw new NotImplementedException();
        }

        public IQueryable Instances(INakedObjectSpecification specification) {
            throw new NotImplementedException();
        }

        public INakedObject GetAdapterFor(object domainObject) {
            return domainObjectMap.ContainsKey(domainObject) ? domainObjectMap[domainObject] : null;
        }

        public INakedObject GetAdapterFor(IOid oid) {
            return identityMap[oid];
        }

        public INakedObject CreateInstance(INakedObjectSpecification specification) {
            object domainObject = specification.CreateObject();
            return CreateAdapter(domainObject, null, null);
        }

        public INakedObject CreateViewModel(INakedObjectSpecification specification) {
            throw new NotImplementedException();
        }

        public INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification) {
            if (identityMap.ContainsKey(oid)) {
                return identityMap[oid];
            }
            object domainObject = specification.CreateObject();
            return CreateAdapter(domainObject, oid, null);
        }

        public void RemoveAdapter(INakedObject objectToDispose) {
            throw new NotImplementedException();
        }


        public void InitDomainObject(object obj) {
            throw new NotImplementedException();
        }

        public void InitInlineObject(object root, object inlineObject) {
            throw new NotImplementedException();
        }

        #endregion

        public static TestProxyPersistor Setup() {
            var persistor = new TestProxyPersistor();
            persistor.Reset();
            return persistor;
        }

        public void AssertAction(int i, string action) {
            if (i >= actions.Count) {
                Assert.Fail("No such action " + action);
            }
            Assert.Equals(action, actions[i]);
        }

        protected static void CreateClass(INakedObjectSpecification nc) {}

        public INakedObject GetObject(IOid oid, INakedObjectSpecification hint) {
            if (identityMap.ContainsKey(oid)) {
                INakedObject adapter = identityMap[oid];
                if (adapter.Specification.HasNoIdentity) {
                    throw new Exception();
                }
                return adapter;
            }
            if (objects.ContainsKey(oid)) {
                return objects[oid];
            }
            throw new Exception("No persisted object to get for " + oid);
        }

        public void MadePersistent(INakedObject nakedObject) {
            // TODO modify OID?
        }

        public void SetupAddObject(INakedObject nakedObject) {
            objects[nakedObject.Oid] = nakedObject;
        }

        public void AddAdapter(object domainObject, INakedObject adapter) {
            if (!adapter.Specification.HasNoIdentity) {
                domainObjectMap.Add(domainObject, adapter);
                IOid oid = adapter.Oid;
                if (oid == null) {
                    throw new Exception();
                }
                identityMap.Add(oid, adapter);
            }
        }
    }
}