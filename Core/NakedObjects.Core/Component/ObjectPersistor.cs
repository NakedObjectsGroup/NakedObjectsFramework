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
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Component {
    public class ObjectPersistor : IObjectPersistor {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ObjectPersistor));
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IObjectStore objectStore;

        public ObjectPersistor(IObjectStore objectStore,
            INakedObjectManager nakedObjectManager) {
            Assert.AssertNotNull(objectStore);
            Assert.AssertNotNull(nakedObjectManager);

            this.objectStore = objectStore;
            this.nakedObjectManager = nakedObjectManager;
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

        public virtual IQueryable Instances(IObjectSpec spec) {
            Log.DebugFormat("Instances of: {0}", spec);
            return GetInstances(spec);
        }

        public INakedObject LoadObject(IOid oid, IObjectSpec spec) {
            Log.DebugFormat("LoadObject oid: {0} specification: {1}", oid, spec);

            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", spec);

            return objectStore.GetObject(oid, spec);
        }

        public void AddPersistedObject(INakedObject nakedObject) {
            if (!nakedObject.Spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                objectStore.ExecuteCreateObjectCommand(nakedObject);
            }
        }

        public void Reload(INakedObject nakedObject) {
            Log.DebugFormat("Reload nakedObject: {0}", nakedObject);
            objectStore.Reload(nakedObject);
        }

        public void ResolveField(INakedObject nakedObject, IAssociationSpec field) {
            Log.DebugFormat("ResolveField nakedObject: {0} field: {1}", nakedObject, field);
            if (field.ReturnSpec.HasNoIdentity) {
                return;
            }
            INakedObject reference = field.GetNakedObject(nakedObject);
            if (reference == null || reference.ResolveState.IsResolved()) {
                return;
            }
            if (!reference.ResolveState.IsPersistent()) {
                return;
            }
            if (Log.IsInfoEnabled) {
                // don't log object - its ToString() may use the unresolved field or unresolved collection
                Log.Info("resolve field " + nakedObject.Spec.ShortName + "." + field.Id + ": " + reference.Spec.ShortName + " " + reference.ResolveState.CurrentState.Code + " " + reference.Oid);
            }
            objectStore.ResolveField(nakedObject, field);
        }

        public void LoadField(INakedObject nakedObject, string field) {
            Log.DebugFormat("LoadField nakedObject: {0} field: {1}", nakedObject, field);
            IAssociationSpec associationSpec = nakedObject.Spec.Properties.Single(x => x.Id == field);
            ResolveField(nakedObject, associationSpec);
        }

        public int CountField(INakedObject nakedObject, string field) {
            Log.DebugFormat("CountField nakedObject: {0} field: {1}", nakedObject, field);

            IAssociationSpec associationSpec = nakedObject.Spec.Properties.Single(x => x.Id == field);

            if (nakedObject.Spec.IsViewModel) {
                INakedObject collection = associationSpec.GetNakedObject(nakedObject);
                return collection.GetCollectionFacetFromSpec().AsEnumerable(collection, nakedObjectManager).Count();
            }

            return objectStore.CountField(nakedObject, associationSpec);
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
                    Log.Info("resolve immediately: " + nakedObject.Spec.ShortName + " " + nakedObject.ResolveState.CurrentState.Code + " " + nakedObject.Oid);
                }
                objectStore.ResolveImmediately(nakedObject);
            }
        }

        public void ObjectChanged(INakedObject nakedObject, ILifecycleManager lifecycleManager, IMetamodelManager metamodel) {
            Log.DebugFormat("ObjectChanged nakedObject: {0}", nakedObject);
            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects()) {
                if (nakedObject.Spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                    nakedObject.Updating();
                    nakedObject.Updated();
                }
                else {
                    IObjectSpec spec = nakedObject.Spec;
                    if (spec.IsAlwaysImmutable() || (spec.IsImmutableOncePersisted() && nakedObject.ResolveState.IsPersistent())) {
                        throw new NotPersistableException("cannot change immutable object");
                    }
                    nakedObject.Updating();
                    objectStore.ExecuteSaveObjectCommand(nakedObject);
                    nakedObject.Updated();
                }
            }

            if (nakedObject.ResolveState.RespondToChangesInPersistentObjects() ||
                nakedObject.ResolveState.IsTransient()) { }
        }

        public void DestroyObject(INakedObject nakedObject) {
            Log.DebugFormat("DestroyObject nakedObject: {0}", nakedObject);

            nakedObject.Deleting();
            objectStore.ExecuteDestroyObjectCommand(nakedObject);
            nakedObject.ResolveState.Handle(Events.DestroyEvent);
            nakedObject.Deleted();
        }

        public object CreateObject(IObjectSpec spec) {
            Log.DebugFormat("CreateObject: " + spec);

            Type type = TypeUtils.GetType(spec.FullName);
            return objectStore.CreateInstance(type);
        }

        public IEnumerable GetBoundedSet(IObjectSpec spec) {
            if (spec.IsBoundedSet()) {
                if (spec.IsInterface) {
                    IList<object> instances = new List<object>();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    // LINQ needs cast - need to be careful with EF - safest to leave as loop
                    foreach (IObjectSpec subSpec in GetLeafNodes(spec)) {
                        foreach (object instance in Instances(subSpec)) {
                            instances.Add(instance);
                        }
                    }
                    return instances;
                }
                return Instances(spec);
            }
            return new object[] {};
        }

        public void LoadComplexTypes(INakedObject pocoAdapter, bool isGhost) {
            objectStore.LoadComplexTypes(pocoAdapter, isGhost);
        }

        #endregion

        private IQueryable<T> GetInstances<T>() where T : class {
            Log.Debug("GetInstances<T> of: " + typeof (T));
            return objectStore.GetInstances<T>();
        }

        private IQueryable GetInstances(Type type) {
            Log.Debug("GetInstances of: " + type);
            return objectStore.GetInstances(type);
        }

        private IQueryable GetInstances(IObjectSpec spec) {
            Log.Debug("GetInstances<T> of: " + spec);
            return objectStore.GetInstances(spec);
        }

        private static IEnumerable<IObjectSpec> GetLeafNodes(IObjectSpec spec) {
            if ((spec.IsInterface || spec.IsAbstract)) {
                return spec.Subclasses.SelectMany(GetLeafNodes);
            }
            return new[] {spec};
        }
    }
}