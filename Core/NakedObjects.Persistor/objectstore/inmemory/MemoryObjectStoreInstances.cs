// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Key;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Persistor.Objectstore.Inmemory {
    /*
    * The objects need to store in a repeatable sequence so the elements and instances method return the same data for any repeated
    * call, and so that one subset of instances follows on the previous. This is done by keeping the objects in the order that they
    * where created.
    */

    public class MemoryObjectStoreInstances {
        private readonly INakedObjectManager manager;
    
        private static int nextKey;
        private readonly IDictionary<IOid, ObjectAndVersion> objectInstances = new Dictionary<IOid, ObjectAndVersion>();

        public static void ResetNextKey() {
            nextKey = 0;
        }

        public MemoryObjectStoreInstances(INakedObjectManager manager) {
            Assert.AssertNotNull(manager);
            this.manager = manager;         
        }


        public virtual INakedObject GetObject(IOid oid) {
            ObjectAndVersion obj;
            INakedObject nakedObject = null;
            lock (objectInstances) {
                obj = objectInstances[oid];
            }
            if (obj != null) {
                nakedObject = manager.CreateAdapter(obj.DomainObject, oid, null);
                nakedObject.OptimisticLock = obj.Version;
                foreach (INakedObjectAssociation field in nakedObject.Specification.Properties.Where(field => field.IsPersisted)) {
                    INakedObject fieldObject = field.GetNakedObject(nakedObject, manager);
                    if (field.IsCollection) {
                        if (fieldObject.ResolveState.IsResolvable()) {
                            fieldObject.ResolveState.Handle(Events.StartResolvingEvent);
                            fieldObject.ResolveState.Handle(Events.EndResolvingEvent);
                        }
                    }
                }
            }
            return nakedObject;
        }

        public virtual IQueryable<T> AllInstances<T>(INakedObjectManager manager) {
            return (from IOid oid in objectInstances.Keys
                    select GetObject(oid).GetDomainObject<T>()).AsQueryable();
        }

        public virtual IQueryable AllInstances(INakedObjectManager manager) {
            return (from IOid oid in objectInstances.Keys
                    select GetObject(oid).GetDomainObject()).AsQueryable();
        }

        public virtual void Remove(IOid oid) {
            lock (objectInstances) {
                objectInstances.Remove(oid);
            }
        }

        private static void SetKey(INakedObject nakedObject) {
            IEnumerable<INakedObjectAssociation> keys = nakedObject.Specification.Properties.Where(p => p.ContainsFacet<IKeyFacet>());
            if (keys.Any()) {
                foreach (IOneToOneAssociation key in keys) {
                    PropertyInfo property = nakedObject.Object.GetType().GetProperty(key.Id);
                    if (property.PropertyType == typeof (int) && ((int) property.GetValue(nakedObject.Object, null)) == 0) {
                        property.SetValue(nakedObject.Object, ++nextKey, null);
                    }
                }
            }
        }


        public virtual void Save(INakedObject nakedObject, ISession session, ILifecycleManager persistor) {
            lock (objectInstances) {
                SerialNumberVersion version;
                if (objectInstances.ContainsKey(nakedObject.Oid)) {
                    version = objectInstances[nakedObject.Oid].Version;
                    version = (SerialNumberVersion) version.Next(session.UserName, DateTime.Now);
                }
                else {
                    version = new SerialNumberVersion(1, session.UserName, DateTime.Now);
                    SetKey(nakedObject);
                    nakedObject.Persisted(session, persistor);
                }
                objectInstances[nakedObject.Oid] = new ObjectAndVersion(nakedObject.Object, version);
                nakedObject.OptimisticLock = version;
            }
        }

        public virtual void Shutdown() {
            lock (objectInstances) {
                objectInstances.Clear();
            }
        }

        public virtual INakedObject GetAdapterFor(object poco, ISession session) {
            lock (objectInstances) {
                foreach (IOid oid in objectInstances.Keys) {
                    ObjectAndVersion holder = objectInstances[oid];
                    object domainObject = holder.DomainObject;
                    if (domainObject == poco) {
                        var adapter = manager.NewAdapterForKnownObject(poco, oid);
                        adapter.OptimisticLock = holder.Version;
                        adapter.ResolveState.Handle(Events.InitializePersistentEvent);
                        adapter.ResolveState.Handle(Events.StartResolvingEvent);
                        adapter.ResolveState.Handle(Events.EndResolvingEvent);
                        return adapter;
                    }
                }
                return null;
            }
        }
    }

    public class ObjectAndVersion {
        public ObjectAndVersion(object obj, SerialNumberVersion version) {
            DomainObject = obj;
            Version = version;
        }

        public object DomainObject { get; set; }
        public SerialNumberVersion Version { get; set; }

        public override string ToString() {
            return DomainObject + " ~ " + Version;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}