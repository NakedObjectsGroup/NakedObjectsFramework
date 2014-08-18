// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Key;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Transaction;

namespace NakedObjects.Persistor.Objectstore.Inmemory {
    public class MemoryObjectStore : INakedObjectStore {
        private readonly INakedObjectReflector reflector;
        private static readonly ILog Log;
        private static IDictionary<INakedObjectSpecification, MemoryObjectStoreInstances> instances;
        private static IDictionary<string, IOid> services;
        private IIdentityMap identityMap;

        static MemoryObjectStore() {
            Log = LogManager.GetLogger(typeof (MemoryObjectStore));
            instances = new Dictionary<INakedObjectSpecification, MemoryObjectStoreInstances>();
            services = new Dictionary<string, IOid>();
        }

        public MemoryObjectStore(INakedObjectReflector reflector) {
            Assert.AssertNotNull(reflector);
            this.reflector = reflector;
            Log.Info("creating object store");
        }

        public INakedObjectManager Manager { get; set; }

        public virtual IIdentityMap IdentityMap {
            set { identityMap = value; }
        }

        #region INakedObjectStore Members

        public virtual bool IsInitialized {
            get { return instances.Any(kvp => !kvp.Key.IsService); }
            set { }
        }

        public ISession Session { get; set; }

        public virtual void AbortTransaction() {
            Log.Debug("AbortTransaction");
        }

        public virtual ICreateObjectCommand CreateCreateObjectCommand(INakedObject nakedObject, ISession session) {
            Log.DebugFormat("CreateCreateObjectCommand: {0}", nakedObject);
            return new Create(nakedObject, this, session);
        }

        public virtual IDestroyObjectCommand CreateDestroyObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateDestroyObjectCommand: {0}", nakedObject);
            return new Destroy(nakedObject, this);
        }

        public virtual ISaveObjectCommand CreateSaveObjectCommand(INakedObject nakedObject, ISession session) {
            Log.DebugFormat("CreateSaveObjectCommand: {0}", nakedObject);
            return new Save(nakedObject, this, session);
        }

        public virtual void EndTransaction() {
            Log.Debug("EndTransaction");
        }

        public virtual void Execute(IPersistenceCommand[] commands) {
            Log.DebugFormat("Execute {0} commands", commands.Length);
            commands.ForEach(c => c.Execute(null));
            Log.Info("end execution");
        }


        public virtual IQueryable<T> GetInstances<T>() where T : class {
            Log.Debug("GetInstances<T> of: " + typeof (T));
            return (from KeyValuePair<INakedObjectSpecification, MemoryObjectStoreInstances> pair in instances
                    where pair.Key.IsOfType(reflector.LoadSpecification(typeof (T)))
                    from T obj in pair.Value.AllInstances<T>(Manager)
                    select obj).AsQueryable();
        }

        public virtual IQueryable GetInstances(Type type) {
            Log.Debug("GetInstances of: " + type);
            return GetInstances(reflector.LoadSpecification(type));
        }

        public virtual IQueryable GetInstances(INakedObjectSpecification specification) {
            Log.DebugFormat("GetInstances for: {0}", specification);
            return (from KeyValuePair<INakedObjectSpecification, MemoryObjectStoreInstances> pair in instances
                    where pair.Key.IsOfType(specification)
                    from object obj in pair.Value.AllInstances(Manager)
                    select obj).AsQueryable();
        }

        public T CreateInstance<T>() where T : class {
            Log.Debug("CreateInstance<T> of: " + typeof (T));
            return (T) CreateInstance(typeof (T));
        }

        public object CreateInstance(Type type) {
            Log.Debug("CreateInstance of: " + type);
            return reflector.LoadSpecification(type).CreateObject();
        }

        public virtual INakedObject GetObject(IOid oid, INakedObjectSpecification hint) {
            Log.DebugFormat("GetObject oid: {0} hint: {1}", oid, hint);
            INakedObject nakedObject = InstancesFor(hint).GetObject(oid, Manager);
            if (nakedObject == null) {
                throw new FindObjectException(oid);
            }
            return nakedObject;
        }

        public void Reload(INakedObject nakedObject) {
            Log.Debug("Reload nakedobject: " + nakedObject);
            throw new NotImplementedException();
        }

        public virtual IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            lock (services) {
                if (services.ContainsKey(name)) {
                    return services[name];
                }
                SerialOid oid = SerialOid.CreatePersistent(reflector, services.Count(), typeName);
                services[name] = oid;
                return oid;
            }
        }


        public virtual void Init() {
            Log.Debug("Init");
        }

        public virtual string Name {
            get { return "In-Memory Object Store"; }
        }

        public IUpdateNotifier UpdateNotifier { get; set; }

        public virtual void RegisterService(string name, IOid oid) {
            Log.DebugFormat("RegisterService name: {0} oid : {1}", name, oid);
            lock (services) {
                if (services.ContainsKey(name)) {
                    throw new InitialisationException(string.Format(Resources.NakedObjects.ServiceRegisterError, name));
                }
                services[name] = oid;
            }
        }

        public virtual void Reset() {
            Log.Debug("Reset");
        }

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            INakedObjectAssociation[] assocs = reflector.LoadSpecification(type).Properties.Where(p => p.ContainsFacet<IKeyFacet>()).ToArray();
            return type.GetProperties().Where(p => assocs.Any(a => a.Id == p.Name)).ToArray();
        }

        public void Refresh(INakedObject nakedObject) {
            Log.Debug("Refresh nakedobject: " + nakedObject);
            throw new NotImplementedException();
        }

        public int CountField(INakedObject nakedObject, INakedObjectAssociation association) {
            return association.GetNakedObject(nakedObject).GetAsEnumerable().Count();
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            PropertyInfo[] keyProperties = GetKeys(type);

            if (keyProperties.Count() == keys.Count()) {
                IEnumerable<Tuple<PropertyInfo, object>> zip = keyProperties.Zip(keys, (pi, o) => new Tuple<PropertyInfo, object>(pi, o));
                IQueryable<object> objs = GetInstances(type).Cast<object>();
                object match = objs.SingleOrDefault(o => zip.All(z => z.Item1.GetValue(o, null).Equals(z.Item2)));
                return match == null ? null : Manager.GetAdapterFor(match);
            }
            return null;
        }

        public virtual void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            Log.DebugFormat("ResolveField nakedobject: {0} field : {1}", nakedObject, field);
            INakedObject reference = field.GetNakedObject(nakedObject);
            reference.ResolveState.Handle(Events.StartResolvingEvent);
            reference.ResolveState.Handle(Events.EndResolvingEvent);
        }

        public virtual void ResolveImmediately(INakedObject nakedObject) {
            Log.DebugFormat("ResolveImmediately nakedobject: {0}", nakedObject);
            nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
            nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
        }


        public virtual void Shutdown() {
            Log.Debug("Shutdown");
            lock (instances) {
                instances.Values.ForEach(x => x.Shutdown());
                instances.Clear();
            }
        }

        public virtual void StartTransaction() {
            Log.Debug("StartTransaction");
        }

        public virtual bool Flush(IPersistenceCommand[] commands) {
            Log.DebugFormat("Flush {0} commands", commands.Length);
            return false;
        }

        #endregion

        public static void DiscardObjects() {
            instances = new Dictionary<INakedObjectSpecification, MemoryObjectStoreInstances>();
            services = new Dictionary<string, IOid>();
            MemoryObjectStoreInstances.ResetNextKey();
        }

        private void DoDestroy(INakedObject nakedObject) {
            INakedObjectSpecification specification = nakedObject.Specification;
            Log.DebugFormat("Destroy object {0} as instance of {1}", nakedObject, specification.ShortName);
            MemoryObjectStoreInstances ins = InstancesFor(specification);
            ins.Remove(nakedObject.Oid);
        }

        private MemoryObjectStoreInstances InstancesFor(INakedObjectSpecification spec) {
            lock (instances) {
                if (!instances.ContainsKey(spec)) {
                    instances[spec] = new MemoryObjectStoreInstances(reflector);
                }
                return instances[spec];
            }
        }

        private void DoSave(INakedObject nakedObject, ISession session) {
            INakedObjectSpecification specification = nakedObject.Specification;
            Log.DebugFormat("Saving object {0} as instance of {1}", nakedObject, specification.ShortName);
            MemoryObjectStoreInstances ins = InstancesFor(specification);
            ins.Save(nakedObject, session);
        }

        public virtual INakedObject CreateAdapter(object obj, ISession session) {
            INakedObjectSpecification spec = reflector.LoadSpecification(obj.GetType());
            MemoryObjectStoreInstances ins = InstancesFor(spec);
            INakedObject adapterFor = ins.GetAdapterFor(obj, session);
            if (adapterFor != null) {
                identityMap.AddAdapter(adapterFor);
            }
            return adapterFor;
        }

        #region Nested type: Create

        private class Create : ICreateObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly MemoryObjectStore objectStore;
            private readonly ISession session;

            public Create(INakedObject nakedObject, MemoryObjectStore objectStore, ISession session) {
                this.nakedObject = nakedObject;
                this.objectStore = objectStore;
                this.session = session;
            }

            #region ICreateObjectCommand Members

            public void Execute(IExecutionContext context) {
                Log.DebugFormat("Create object {0}", nakedObject);
                objectStore.DoSave(nakedObject, session);
            }

            public INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return string.Format("CreateObjectCommand [object={0}]", nakedObject);
            }
        }

        #endregion

        #region Nested type: Destroy

        private class Destroy : IDestroyObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly MemoryObjectStore objectStore;

            public Destroy(INakedObject nakedObject, MemoryObjectStore objectStore) {
                this.nakedObject = nakedObject;
                this.objectStore = objectStore;
            }

            #region IDestroyObjectCommand Members

            public void Execute(IExecutionContext context) {
                Log.Info("  delete object '" + nakedObject + "'");
                objectStore.DoDestroy(nakedObject);
            }

            public INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "DestroyObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested type: Save

        private class Save : ISaveObjectCommand {
            private readonly INakedObject nakedObject;
            private readonly MemoryObjectStore objectStore;
            private readonly ISession session;

            public Save(INakedObject nakedObject, MemoryObjectStore objectStore, ISession session) {
                this.nakedObject = nakedObject;
                this.objectStore = objectStore;
                this.session = session;
            }

            #region ISaveObjectCommand Members

            public void Execute(IExecutionContext context) {
                objectStore.DoSave(nakedObject, session);
            }

            public INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "SaveObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}