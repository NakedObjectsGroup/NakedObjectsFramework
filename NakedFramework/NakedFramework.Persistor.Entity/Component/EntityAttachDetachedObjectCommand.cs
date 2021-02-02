// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Util;

namespace NakedObjects.Persistor.Entity.Component {
    public class EntityAttachDetachedObjectCommand : ICreateObjectCommand {
        private readonly LocalContext context;
        private readonly INakedObjectAdapter nakedObjectAdapter;
        private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();
        private readonly EntityObjectStore parent;
        private readonly object rootProxy;

        public EntityAttachDetachedObjectCommand(INakedObjectAdapter nakedObjectAdapter, LocalContext context, EntityObjectStore parent) {
            this.context = context;
            this.parent = parent;
            this.nakedObjectAdapter = nakedObjectAdapter;
        }

        public EntityAttachDetachedObjectCommand(INakedObjectAdapter nakedObjectAdapter, object rootProxy, LocalContext context, EntityObjectStore parent) {
            this.context = context;
            this.parent = parent;
            this.nakedObjectAdapter = nakedObjectAdapter;
            this.rootProxy = rootProxy;
        }

        public INakedObjectAdapter OnObject() => nakedObjectAdapter;

        public void Execute() {
            try {
                context.CurrentSaveRootObjectAdapter = nakedObjectAdapter;
                objectToProxyScratchPad.Clear();
                ProxyObjectIfAppropriate(nakedObjectAdapter.Object);
            }
            catch (Exception e) {
                parent.logger.LogWarning($"Error in EntityCreateObjectCommand.Execute: {e.Message}");
                throw;
            }
        }

        private void SetKeyAsNecessary(object objectToProxy, object proxy) {
            if (!context.IdMembersAreIdentity(objectToProxy.GetType())) {
                var idMembers = context.GetIdMembers(objectToProxy.GetType());
                idMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
            }
        }

        private static IDictionary<string, object> GetMemberValueMap(Type type, object[] key, LocalContext context, out string entitySetName) {
            var set = context.GetObjectSet(type).GetProperty<EntitySet>("EntitySet");
            entitySetName = $"{set.EntityContainer.Name}.{set.Name}";
            var idmembers = context.GetIdMembers(type);
            var keyValues = key;
            return ObjectContextUtils.MemberValueMap(idmembers, keyValues);
        }

        private object GetOrCreateProxiedObject(object originalObject, object[] keys, LocalContext context) {
            var proxy = keys.All(EntityObjectStore.EmptyKey) ? context.CreateObject(originalObject.GetType()) : rootProxy;

            if (proxy is null) {
                throw new PersistFailedException($"unexpected null proxy for {{originalObject}} type {originalObject.GetType()}");
            }

            return proxy;
        }

        private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObject) {
            var keys = context.GetKey(originalObject);
            var persisting = keys.All(EntityObjectStore.EmptyKey);
            var proxy = GetOrCreateProxiedObject(originalObject, keys, context);

            objectToProxyScratchPad[originalObject] = proxy;

            // create transient adapter here so that LoadObjectIntoNakedObjectsFramework knows proxy domainObject is transient
            // if not proxied this should just be the same as adapterForOriginalObject
            var proxyAdapter = parent.createAdapter(null, proxy);

            SetKeyAsNecessary(originalObject, proxy);

            if (persisting) {
                context.GetObjectSet(originalObject.GetType()).Invoke("AddObject", proxy);
                context.PersistedNakedObjects.Add(proxyAdapter);
            }

            // need to update
            ProxyReferencesAndCopyValuesToProxy(originalObject, proxy);
            parent.removeAdapter(proxyAdapter);
            parent.replacePoco(adapterForOriginalObject, proxy);

            CallPersistingPersistedForComplexObjects(proxyAdapter);

            parent.CheckProxies(proxy);

            return proxy;
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

        private void ProxyReferencesAndCopyValuesToProxy(object originalObject, object proxy) {
            var nonIdMembers = context.GetNonIdMembers(originalObject.GetType());
            nonIdMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));

            var refMembers = context.GetReferenceMembers(originalObject.GetType());
            refMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, ProxyReferenceIfAppropriate(pi.GetValue(originalObject, null)), null));

            var colmembers = context.GetCollectionMembers(originalObject.GetType());
            foreach (var pi in colmembers) {
                var toCol = (IList) proxy.GetType().GetProperty(pi.Name).GetValue(proxy, null);
                var fromCol = (IEnumerable) pi.GetValue(originalObject, null);
                toCol.Clear();
                foreach (var item in fromCol) {
                    toCol.Invoke("Add", ProxyReferenceIfAppropriate(item));
                }
            }

            var notPersistedMembers = originalObject.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<NotPersistedAttribute>() != null).ToArray();
            notPersistedMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));
        }

        public override string ToString() => $"CreateObjectCommand [object={nakedObjectAdapter}]";

        private object ProxyObjectIfAppropriate(object originalObject) {
            if (originalObject == null) {
                return null;
            }

            if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                return objectToProxyScratchPad[originalObject];
            }

            var adapterForOriginalObject = parent.createAdapter(null, originalObject);
            return ProxyObject(originalObject, adapterForOriginalObject);
        }

        private object ProxyReferenceIfAppropriate(object originalObject) {
            if (originalObject == null) {
                return null;
            }

            if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                return objectToProxyScratchPad[originalObject];
            }

            var adapterForOriginalObject = parent.createAdapter(null, originalObject);
            //return ProxyObject(originalObject, adapterForOriginalObject);
            // for moment assume all refences proxied
            return originalObject;
        }
    }
}