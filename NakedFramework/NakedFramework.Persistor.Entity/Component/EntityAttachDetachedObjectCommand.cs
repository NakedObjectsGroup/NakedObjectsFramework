// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Util;

namespace NakedObjects.Persistor.Entity.Component {
    public class EntityAttachDetachedObjectCommand : ICreateObjectCommand {
        private readonly EntityObjectStore.LocalContext context;
        private readonly INakedObjectAdapter nakedObjectAdapter;
        private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();
        private readonly EntityObjectStore parent;

        public EntityAttachDetachedObjectCommand(INakedObjectAdapter nakedObjectAdapter, object[] allChanged, EntityObjectStore.LocalContext context, EntityObjectStore parent) {
            AllChanged = allChanged;
            this.context = context;
            this.parent = parent;
            this.nakedObjectAdapter = nakedObjectAdapter;
        }

        private object[] AllChanged { get; set; }

        public void Execute() {
            try {
                context.CurrentSaveRootObjectAdapter = nakedObjectAdapter;
                objectToProxyScratchPad.Clear();
                AllChanged = AllChanged.Except(new[] {nakedObjectAdapter.Object}).ToArray();
                ProxyObjectIfAppropriate(nakedObjectAdapter.Object, true);
            }
            catch (Exception e) {
                parent.logger.LogWarning($"Error in EntityCreateObjectCommand.Execute: {e.Message}");
                throw;
            }
        }

        public INakedObjectAdapter OnObject() => nakedObjectAdapter;

        private void SetKeyAsNecessary(object objectToProxy, object proxy) {
            if (!context.IdMembersAreIdentity(objectToProxy.GetType())) {
                var idMembers = context.GetIdMembers(objectToProxy.GetType());
                idMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
            }
        }

        private static IDictionary<string, object> GetMemberValueMap(Type type, object[] key, EntityObjectStore.LocalContext context, out string entitySetName) {
            var set = context.GetObjectSet(type).GetProperty<EntitySet>("EntitySet");
            entitySetName = $"{set.EntityContainer.Name}.{set.Name}";
            var idmembers = context.GetIdMembers(type);
            var keyValues = key;
            return EntityObjectStore.MemberValueMap(idmembers, keyValues);
        }

        public object GetObjectByKey(object[] keys, Type type, EntityObjectStore.LocalContext context) {
            var memberValueMap = GetMemberValueMap(type, keys, context, out var entitySetName);
            var oq = context.CreateQuery(type, entitySetName);

            foreach (var (key, value) in memberValueMap) {
                var query = string.Format("it.{0}=@{0}", key);
                oq = oq.Invoke<object>("Where", query, new[] {new ObjectParameter(key, value)});
            }

            context.GetNavigationMembers(type).Where(m => !CollectionUtils.IsCollection(m.PropertyType)).ForEach(pi => oq = oq.Invoke<object>("Include", pi.Name));
            return EntityObjectStore.First(oq.Invoke<IEnumerable>("Execute", MergeOption.OverwriteChanges));
        }

        private (object, bool) GetOrCreateProxiedObject(object originalObject, object[] keys, EntityObjectStore.LocalContext context) {
            var dbObject = keys.All(EntityObjectStore.EmptyKey) ? null : GetObjectByKey(keys, originalObject.GetType().GetProxiedType(), context);

            return dbObject != null ? (dbObject, true) : (context.CreateObject(originalObject.GetType()), false);
        }

        private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObjectAdapter, bool root) {
            var keys = context.GetKey(originalObject);

            var persisting = keys.All(EntityObjectStore.EmptyKey);

            var (objectToAdd, existing) = GetOrCreateProxiedObject(originalObject, keys, context);

            objectToProxyScratchPad[originalObject] = objectToAdd;
            adapterForOriginalObjectAdapter.Persisting();

            // create transient adapter here so that LoadObjectIntoNakedObjectsFramework knows proxy domainObject is transient
            // if not proxied this should just be the same as adapterForOriginalObjectAdapter
            var proxyAdapter = parent.createAdapter(null, objectToAdd);

            SetKeyAsNecessary(originalObject, objectToAdd);

            if (persisting) {
                context.GetObjectSet(originalObject.GetType()).Invoke("AddObject", objectToAdd);
            }

            if (!existing) {
                ProxyReferencesAndCopyValuesToProxy(originalObject, objectToAdd);
                context.PersistedNakedObjects.Add(proxyAdapter);
                // remove temporary adapter for proxy (tidy and also means we will not get problem 
                // with already known object in identity map when replacing the poco
                parent.removeAdapter(proxyAdapter);
                parent.replacePoco(adapterForOriginalObjectAdapter, objectToAdd);
            }
            else if (root || AllChanged.Contains(originalObject)) {
                // need to update
                ProxyReferencesAndCopyValuesToProxy(originalObject, objectToAdd);
                parent.removeAdapter(proxyAdapter);
                parent.replacePoco(adapterForOriginalObjectAdapter, objectToAdd);
            }

            CallPersistingPersistedForComplexObjects(proxyAdapter);

            parent.CheckProxies(objectToAdd);

            return objectToAdd;
        }

        private void CallPersistingPersistedForComplexObjects(INakedObjectAdapter parentAdapter) {
            var complexMembers = context.GetComplexMembers(parentAdapter.Object.GetEntityProxiedType());
            foreach (var pi in complexMembers) {
                var complexObject = pi.GetValue(parentAdapter.Object, null);
                var childAdapter = parent.createAggregatedAdapter(nakedObjectAdapter, pi, complexObject);
                childAdapter.Persisting();
                context.PersistedNakedObjects.Add(childAdapter);
            }
        }

        private void ProxyReferences(object objectToProxy) {
            // this is to ensure persisting/persisted gets call for all referenced transient objects - what it wont handle 
            // is if a referenced object is proxied - as it doesn't update the reference - not sure if that will be a requirement. 
            var refMembers = context.GetReferenceMembers(objectToProxy.GetType());
            refMembers.ForEach(pi => ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null), false));

            var colmembers = context.GetCollectionMembers(objectToProxy.GetType());
            foreach (var pi in colmembers) {
                foreach (var item in (IEnumerable) pi.GetValue(objectToProxy, null)) {
                    ProxyObjectIfAppropriate(item, false);
                }
            }
        }

        private void ProxyReferencesAndCopyValuesToProxy(object originalObject, object proxy) {
            var nonIdMembers = context.GetNonIdMembers(originalObject.GetType());
            nonIdMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));

            var refMembers = context.GetReferenceMembers(originalObject.GetType());
            refMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, ProxyObjectIfAppropriate(pi.GetValue(originalObject, null), false), null));

            var colmembers = context.GetCollectionMembers(originalObject.GetType());
            foreach (var pi in colmembers) {
                var toCol = (IList) proxy.GetType().GetProperty(pi.Name).GetValue(proxy, null);
                var fromCol = (IEnumerable) pi.GetValue(originalObject, null);
                toCol.Clear();
                foreach (var item in fromCol) {
                    toCol.Invoke("Add", ProxyObjectIfAppropriate(item, false));
                }
            }

            var notPersistedMembers = originalObject.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<NotPersistedAttribute>() != null).ToArray();
            notPersistedMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));
        }

        public override string ToString() => $"CreateObjectCommand [object={nakedObjectAdapter}]";

        private object ProxyObjectIfAppropriate(object originalObject, bool root) {
            if (originalObject == null) {
                return null;
            }

            if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                return objectToProxyScratchPad[originalObject];
            }

            var adapterForOriginalObjectAdapter = parent.createAdapter(null, originalObject);
            return ProxyObject(originalObject, adapterForOriginalObjectAdapter, root);
        }
    }
}