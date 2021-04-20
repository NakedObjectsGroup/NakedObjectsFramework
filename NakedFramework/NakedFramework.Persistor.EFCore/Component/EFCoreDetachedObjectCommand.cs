// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Persist;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Util;

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreDetachedObjectCommand  : IDetachedObjectCommand {
        private readonly IDetachedObjects detachedObjects;
        private readonly EFCoreObjectStore parent;
        private DbContext context;

        public EFCoreDetachedObjectCommand(IDetachedObjects detachedObjects, EFCoreObjectStore parent) {
            this.detachedObjects = detachedObjects;
            this.parent = parent;
        }

        private bool IsSavedOrUpdated(object obj) => detachedObjects.SavedAndUpdated.Any(t => t.original == obj);

        private object AlreadyUpdatedProxy(object obj) => detachedObjects.SavedAndUpdated.Where(t => t.original == obj).Select(t => t.updated).SingleOrDefault();

        private void ProxyIfNotAlreadySeen((object proxy, object toProxy) updateTuple) {
            var (proxy, updated) = updateTuple;

            if (!IsSavedOrUpdated(updated)) {
                context = parent.GetContext(updated).WrappedDbContext;
                ProxyObjectIfAppropriate(updated, proxy);
            }
        }

        private void ValidateDetachedObjects() {
            var errors = new List<string>();

            foreach (var toSave in detachedObjects.ToSave) {
                context = parent.GetContext(toSave).WrappedDbContext;
                if (!parent.EmptyKeys(toSave)) {
                    errors.Add($"Save object {toSave} already has a key");
                }
            }

            foreach (var updateTuple in detachedObjects.ToUpdate) {
                var (_, toUpdate) = updateTuple;
                context = parent.GetContext(toUpdate).WrappedDbContext;
                if (parent.EmptyKeys(toUpdate)) {
                    errors.Add($"Update object {toUpdate} has no key");
                }
            }

            if (errors.Any()) {
                var error = errors.Aggregate("", (s, a) => $"{a}{(string.IsNullOrEmpty(a) ? "" : ", ")}{s}");
                throw new PersistFailedException(error);
            }
        }

        public IList<(object original, object updated)> Execute() {
            ValidateDetachedObjects();
            try {
                foreach (var toSave in detachedObjects.ToSave) {
                    ProxyIfNotAlreadySeen((null, toSave));
                }

                foreach (var updateTuple in detachedObjects.ToUpdate) {
                    ProxyIfNotAlreadySeen(updateTuple);
                }

                foreach (var toDelete in detachedObjects.ToDelete) {
                    DeleteObject(toDelete);
                }

                return detachedObjects.SavedAndUpdated;
            }
            catch (Exception e) {
                parent.Logger.LogWarning($"Error in {nameof(EFCoreDetachedObjectCommand)}.{nameof(Execute)}: {e.Message}");
                throw;
            }
        }

        private void DeleteObject(object toDelete) {
            context = parent.GetContext(toDelete).WrappedDbContext;
            context.Remove(toDelete);
        }

        private static object GetOrCreateProxiedObject(object originalObject, DbContext context, object potentialProxy) {
            var proxy = potentialProxy ?? context.Add(Activator.CreateInstance(originalObject.GetType())).Entity;
            return proxy ?? throw new PersistFailedException($"unexpected null proxy for {originalObject} type {originalObject.GetType()}");
        }

        private object ProxyObject(object originalObject, object potentialProxy = null) {
            var alreadyUpdatedProxy = AlreadyUpdatedProxy(originalObject);
            if (alreadyUpdatedProxy is not null) {
                return alreadyUpdatedProxy;
            }

            var persisting = potentialProxy is null;
            var proxy = GetOrCreateProxiedObject(originalObject, context, potentialProxy);

            // create transient adapter here so that LoadObjectIntoNakedObjectsFramework knows proxy domainObject is transient
            if (persisting) {
                parent.CreateAdapter(null, proxy);
                context.Add(proxy);
            }

            // need to update
            ProxyReferencesAndCopyValuesToProxy(originalObject, proxy);

            detachedObjects.SavedAndUpdated.Add((originalObject, proxy));

            return proxy;
        }

        private void ProxyReferencesAndCopyValuesToProxy(object originalObject, object proxy) {
            var nonIdMembers = context.GetNonIdMembers(originalObject.GetType());
            nonIdMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));

            var refMembers = context.GetReferenceMembers(originalObject.GetType());
            refMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, ProxyReferenceIfAppropriate(pi.GetValue(originalObject, null)), null));

            var collectionMembers = context.GetCollectionMembers(originalObject.GetType());
            foreach (var pi in collectionMembers) {
                var toCol = proxy.GetProperty(pi.Name).GetValue(proxy, null);
                var fromCol = pi.GetValue(originalObject, null);

                if (!ReferenceEquals(toCol, fromCol) && fromCol is not null) {
                    toCol.Invoke("Clear");
                    foreach (var item in fromCol.AsEnumerable()) {
                        toCol.Invoke("Add", ProxyReferenceIfAppropriate(item));
                    }
                }
            }

            var notPersistedMembers = originalObject.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && parent.IsNotPersisted(originalObject, p)).ToArray();
            notPersistedMembers.ForEach(pi => proxy.GetProperty(pi.Name).SetValue(proxy, pi.GetValue(originalObject, null), null));
        }

        public override string ToString() => "EFCoreDetachedObjectCommand";

        private void ProxyObjectIfAppropriate(object originalObject, object existingProxy) {
            if (originalObject is not null) {
                ProxyObject(originalObject, existingProxy);
            }
        }

        private object ProxyReferenceIfAppropriate(object originalObject) {
            if (originalObject == null) {
                return null;
            }

            var alreadyUpdatedProxy = AlreadyUpdatedProxy(originalObject);
            if (alreadyUpdatedProxy is not null) {
                return alreadyUpdatedProxy;
            }

            if (detachedObjects.ToUpdate.Select(t => t.updated).Contains(originalObject)) {
                var (proxy, _) = detachedObjects.ToUpdate.SingleOrDefault(t => t.updated == originalObject);
                return ProxyObject(originalObject, proxy);
            }

            var persisting = parent.EmptyKeys(originalObject);

            return persisting ? ProxyObject(originalObject) : originalObject;
        }
    }
}