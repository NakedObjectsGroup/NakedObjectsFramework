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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Persist;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Util;

namespace NakedObjects.Persistor.Entity.Component {
    public class EntityPersistUpdateDetachedObjectCommand  {
        private readonly IDetachedObjects detachedObjects;
        private LocalContext context;
        private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();
        private readonly EntityObjectStore parent;

        public EntityPersistUpdateDetachedObjectCommand(IDetachedObjects detachedObjects, EntityObjectStore parent)
        {
            this.detachedObjects = detachedObjects;
            this.parent = parent;
        }

        public INakedObjectAdapter OnObject() => context.CurrentSaveRootObjectAdapter;


        private bool IsSavedOrUpdated(object obj) => detachedObjects.SavedAndUpdated.Any(t => t.original == obj);

        public IList<(object original, object updated)> Execute() {
            try {

                objectToProxyScratchPad.Clear();

                foreach (var toSave in detachedObjects.ToSave) {
                    if (!IsSavedOrUpdated(toSave)) {
                        context = parent.GetContext(toSave);
                        context.CurrentSaveRootObjectAdapter = parent.createAdapter(null, toSave);
                        ProxyObjectIfAppropriate(toSave, null);
                    }
                }

                foreach (var updateTuple in detachedObjects.ToUpdate) {
                    var (proxy, updated) = updateTuple;

                    if (!IsSavedOrUpdated(updated)) {
                        context = parent.GetContext(updated);
                        context.CurrentSaveRootObjectAdapter = parent.AdaptDetachedObject(updated);
                        ProxyObjectIfAppropriate(updated, proxy);
                    }
                }

                return detachedObjects.SavedAndUpdated;
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

        private static object GetOrCreateProxiedObject(object originalObject, object[] keys, LocalContext context, object potentialProxy) {
            var proxy = keys.All(EntityObjectStore.EmptyKey) ? context.CreateObject(originalObject.GetType()) : potentialProxy;

            if (proxy is null) {
                throw new PersistFailedException($"unexpected null proxy for {{originalObject}} type {originalObject.GetType()}");
            }

            return proxy;
        }

        private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObject, object potentialProxy) {
            var keys = context.GetKey(originalObject);
            var persisting = keys.All(EntityObjectStore.EmptyKey);
            var proxy = GetOrCreateProxiedObject(originalObject, keys, context, potentialProxy);

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

            parent.CheckProxies(proxy);

            detachedObjects.SavedAndUpdated.Add((originalObject, proxy));

            return proxy;
        }

        private void ProxyReferencesAndCopyValuesToProxy(object originalObject, object proxy) {
            var nonIdMembers = context.GetNonIdMembers(originalObject.GetType());
            nonIdMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));

            var refMembers = context.GetReferenceMembers(originalObject.GetType());
            refMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, ProxyReferenceIfAppropriate(pi.GetValue(originalObject, null)), null));

            var colmembers = context.GetCollectionMembers(originalObject.GetType());
            foreach (var pi in colmembers) {
                var toCol = proxy.GetType().GetProperty(pi.Name).GetValue(proxy, null);
                var fromCol = pi.GetValue(originalObject, null);

                if (!ReferenceEquals(toCol, fromCol)) {
                    toCol.Invoke("Clear");
                    foreach (var item in (IEnumerable) fromCol) {
                        toCol.Invoke("Add", ProxyReferenceIfAppropriate(item));
                    }
                }
            }

            var notPersistedMembers = originalObject.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<NotPersistedAttribute>() != null).ToArray();
            notPersistedMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));
        }

        public override string ToString() => $"CreateObjectCommand";

        private void ProxyObjectIfAppropriate(object originalObject, object existingProxy) {
            if (originalObject == null) {
                return;
            }

            if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                return;
            }

            var adapterForOriginalObject = parent.createAdapter(null, originalObject);
            ProxyObject(originalObject, adapterForOriginalObject, existingProxy);
        }

        private object ProxyReferenceIfAppropriate(object originalObject) {
            if (originalObject == null) {
                return null;
            }

            if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                return objectToProxyScratchPad[originalObject];
            }

            var adapterForOriginalObject = parent.createAdapter(null, originalObject);
            
            var keys = context.GetKey(originalObject);
            var persisting = keys.All(EntityObjectStore.EmptyKey);

            if (persisting) {
                return ProxyObject(originalObject, adapterForOriginalObject, null);
            }

            if (detachedObjects.ToUpdate.Select(t => t.updated).Contains(originalObject)) {
                // improve 
                var (proxy, _) = detachedObjects.ToUpdate.SingleOrDefault(t => t.updated == originalObject);
                return ProxyObject(originalObject, adapterForOriginalObject, proxy);
            }

            return originalObject;
        }
    }
}