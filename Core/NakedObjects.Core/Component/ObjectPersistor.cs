// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    /// <summary>
    /// This is generic portion of persistence logic, implemented as a composite wrapping the ObjectStore which is 
    /// the store specific portion of the logic. 
    /// </summary>
    public sealed class ObjectPersistor : IObjectPersistor {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ObjectPersistor));
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

        public IQueryable<T> Instances<T>() where T : class {
            return GetInstances<T>();
        }

        public IQueryable Instances(Type type) {
            return GetInstances(type);
        }

        public IQueryable Instances(IObjectSpec spec) {
            return GetInstances(spec);
        }

        public INakedObjectAdapter LoadObject(IOid oid, IObjectSpec spec) {
            Assert.AssertNotNull("needs an OID", oid);
            Assert.AssertNotNull("needs a specification", spec);

            return objectStore.GetObject(oid, spec);
        }

        public void AddPersistedObject(INakedObjectAdapter nakedObjectAdapter) {
            if (!nakedObjectAdapter.Spec.ContainsFacet(typeof(IComplexTypeFacet))) {
                objectStore.ExecuteCreateObjectCommand(nakedObjectAdapter);
            }
        }

        public void Reload(INakedObjectAdapter nakedObjectAdapter) {
            objectStore.Reload(nakedObjectAdapter);
        }

        public void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field) {
            if (field.ReturnSpec.HasNoIdentity) {
                return;
            }

            INakedObjectAdapter reference = field.GetNakedObject(nakedObjectAdapter);
            if (reference == null || reference.ResolveState.IsResolved()) {
                return;
            }

            if (!reference.ResolveState.IsPersistent()) {
                return;
            }

            // don't log object - its ToString() may use the unresolved field or unresolved collection              
            objectStore.ResolveField(nakedObjectAdapter, field);
        }

        public void LoadField(INakedObjectAdapter nakedObjectAdapter, string field) {
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            Trace.Assert(spec != null);
            IAssociationSpec associationSpec = spec.Properties.Single(x => x.Id == field);
            ResolveField(nakedObjectAdapter, associationSpec);
        }

        public int CountField(INakedObjectAdapter nakedObjectAdapter, string field) {
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            Trace.Assert(spec != null);

            IAssociationSpec associationSpec = spec.Properties.Single(x => x.Id == field);

            if (nakedObjectAdapter.Spec.IsViewModel) {
                INakedObjectAdapter collection = associationSpec.GetNakedObject(nakedObjectAdapter);
                return collection.GetCollectionFacetFromSpec().AsEnumerable(collection, nakedObjectManager).Count();
            }

            return objectStore.CountField(nakedObjectAdapter, associationSpec);
        }

        public PropertyInfo[] GetKeys(Type type) {
            return objectStore.GetKeys(type);
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {
            return objectStore.FindByKeys(type, keys);
        }

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            objectStore.Refresh(nakedObjectAdapter);
        }

        public void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.ResolveState.IsResolvable()) {
                Assert.AssertFalse("only resolve object that is not yet resolved", nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsResolved());
                Assert.AssertTrue("only resolve object that is persistent", nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsPersistent());
                if (nakedObjectAdapter.Oid is AggregateOid) {
                    return;
                }

                // don't log object - it's ToString() may use the unresolved field, or unresolved collection

                objectStore.ResolveImmediately(nakedObjectAdapter);
            }
        }

        public void ObjectChanged(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager, IMetamodelManager metamodel) {
            if (nakedObjectAdapter.ResolveState.RespondToChangesInPersistentObjects()) {
                if (nakedObjectAdapter.Spec.ContainsFacet(typeof(IComplexTypeFacet))) {
                    nakedObjectAdapter.Updating();
                    nakedObjectAdapter.Updated();
                }
                else {
                    ITypeSpec spec = nakedObjectAdapter.Spec;
                    if (spec.IsAlwaysImmutable() || (spec.IsImmutableOncePersisted() && nakedObjectAdapter.ResolveState.IsPersistent())) {
                        throw new NotPersistableException(Log.LogAndReturn("cannot change immutable object"));
                    }

                    nakedObjectAdapter.Updating();

                    if (!spec.IsNeverPersisted()) {
                        objectStore.ExecuteSaveObjectCommand(nakedObjectAdapter);
                    }

                    nakedObjectAdapter.Updated();
                }
            }

            if (nakedObjectAdapter.ResolveState.RespondToChangesInPersistentObjects() ||
                nakedObjectAdapter.ResolveState.IsTransient()) { }
        }

        public void DestroyObject(INakedObjectAdapter nakedObjectAdapter) {
            nakedObjectAdapter.Deleting();
            objectStore.ExecuteDestroyObjectCommand(nakedObjectAdapter);
            nakedObjectAdapter.ResolveState.Handle(Events.DestroyEvent);
            nakedObjectAdapter.Deleted();
        }

        public object CreateObject(ITypeSpec spec) {
            Type type = TypeUtils.GetType(spec.FullName);
            return objectStore.CreateInstance(type);
        }

        public IEnumerable GetBoundedSet(IObjectSpec spec) {
            if (spec.IsBoundedSet()) {
                if (spec.IsInterface) {
                    IList<object> instances = new List<object>();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    // LINQ needs cast - need to be careful with EF - safest to leave as loop
                    foreach (ITypeSpec subSpec in GetLeafNodes(spec)) {
                        foreach (object instance in Instances((IObjectSpec) subSpec)) {
                            instances.Add(instance);
                        }
                    }

                    return instances;
                }

                return Instances(spec);
            }

            return new object[] { };
        }

        public void LoadComplexTypes(INakedObjectAdapter adapter, bool isGhost) {
            objectStore.LoadComplexTypesIntoNakedObjectFramework(adapter, isGhost);
        }

        #endregion

        private IQueryable<T> GetInstances<T>() where T : class {
            return objectStore.GetInstances<T>();
        }

        private IQueryable GetInstances(Type type) {
            return objectStore.GetInstances(type);
        }

        private IQueryable GetInstances(IObjectSpec spec) {
            return objectStore.GetInstances(spec);
        }

        private static IEnumerable<ITypeSpec> GetLeafNodes(ITypeSpec spec) {
            if (spec.IsInterface || spec.IsAbstract) {
                return spec.Subclasses.SelectMany(GetLeafNodes);
            }

            return new[] {spec};
        }
    }
}