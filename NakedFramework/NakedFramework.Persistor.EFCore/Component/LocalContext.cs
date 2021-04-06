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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedFramework.Persistor.EFCore.Util;

namespace NakedFramework.Persistor.EFCore.Component {
    public class LocalContext : IDisposable {
        private readonly List<object> added = new();
        private readonly IDictionary<Type, Type> baseTypeMap = new Dictionary<Type, Type>();
        private readonly ISet<Type> notPersistedTypes = new HashSet<Type>();
        private readonly ISet<Type> ownedTypes = new HashSet<Type>();
        private readonly EFCoreObjectStore parent;
        private readonly ISession session;
        //private readonly IDictionary<Type, StructuralType> typeToStructuralType = new Dictionary<Type, StructuralType>();
        private List<INakedObjectAdapter> coUpdating;
        private List<INakedObjectAdapter> updatingNakedObjects;

        private LocalContext(Type[] preCachedTypes, Type[] notPersistedTypes, ISession session, EFCoreObjectStore parent) {
            this.session = session;
            this.parent = parent;

            preCachedTypes.ForEach(t => ownedTypes.Add(t));
            notPersistedTypes.ForEach(t => this.notPersistedTypes.Add(t));
        }

        public LocalContext(Func<DbContext> context, EFCorePersistorConfiguration config, ISession session, EFCoreObjectStore parent)
            : this(config.PreCachedTypes(), config.NotPersistedTypes(), session, parent) {
            WrappedDbContext = context();
            Name = WrappedDbContext.ToString();
        }

        public INakedObjectManager Manager { protected get; set; }
        public DbContext WrappedDbContext { get; private set; }
        public string Name { get; }

        public ISet<INakedObjectAdapter> LoadedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

        public ISet<INakedObjectAdapter> PersistedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

        public ISet<INakedObjectAdapter> DeletedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

        //public MergeOption DefaultMergeOption { get; set; }
        public INakedObjectAdapter CurrentSaveRootObjectAdapter { get; set; }
        public INakedObjectAdapter CurrentUpdateRootObjectAdapter { get; set; }

        #region IDisposable Members

        public void Dispose() {
            try {
                WrappedDbContext.Dispose();
                WrappedDbContext = null;
                baseTypeMap.Clear();
            }
            catch (Exception e) {
                parent.Logger.LogError(e, $"Exception disposing context: {Name}");
            }
        }

        #endregion

        public Type GetMostBaseType(Type type) {
            if (!baseTypeMap.ContainsKey(type)) {
                baseTypeMap[type] = MostBaseType(type);
            }

            return baseTypeMap[type];
        }

        private Type MostBaseType(Type type) {
            if (type.BaseType == typeof(object) || !(GetIsOwned(type.BaseType) || GetIsKnown(type.BaseType))) {
                return type;
            }

            return GetMostBaseType(type.BaseType);
        }

        public bool IsAlwaysUnrecognised(Type type) =>
            type == null ||
            type == typeof(object) ||
            type.IsGenericType ||
            notPersistedTypes.Contains(type);

        private bool IsOwnedOrBaseTypeIsOwned(Type type) =>
            !IsAlwaysUnrecognised(type) &&
            (ownedTypes.Contains(type) || IsOwnedOrBaseTypeIsOwned(type.BaseType));

        private bool IsKnownOrBaseTypeIsKnown(Type type) {
            if (IsAlwaysUnrecognised(type)) {
                return false;
            }

            if (WrappedDbContext.HasEntityType(type)) {
                ownedTypes.Add(type);
                return true;
            }

            return IsKnownOrBaseTypeIsKnown(type.BaseType);
        }

        public bool GetIsKnown(Type type) => IsKnownOrBaseTypeIsKnown(type);

        public bool GetIsOwned(Type type) => IsOwnedOrBaseTypeIsOwned(type);

        public void SaveOrUpdateComplete() {
            CurrentSaveRootObjectAdapter = null;
            CurrentUpdateRootObjectAdapter = null;
            added.Clear();
            updatingNakedObjects = null;
            coUpdating = null;
            LoadedNakedObjects.Clear();
            PersistedNakedObjects.Clear();
            DeletedNakedObjects.Clear();
        }

        private IEnumerable<object> CheckForForeignKeys(EntityEntry entry) {
            IList<object> updatedObjects = new List<object>();
            int updatedForeignKeys = 0;

            var foreignKeys = WrappedDbContext.Model.FindEntityType(entry.Entity.GetType().GetProxiedType()).GetForeignKeys();

            foreach (var foreignKey in foreignKeys) {
                var names = foreignKey.Properties.Select(p => p.Name);

                foreach (var name in names) {
                    var matchingMember = entry.Members.SingleOrDefault(m => m.Metadata.Name == name) as PropertyEntry;

                    if (matchingMember?.IsModified == true) {
                        var type = foreignKey.PrincipalEntityType.ClrType;
                        var keys = matchingMember.OriginalValue;
                        updatedForeignKeys++; 

                        if (keys is not null) {
                            var otherEnd = WrappedDbContext.Find(type, keys);
                            updatedObjects.Add(otherEnd);
                        }
                    }
                }
            }

            // check if anything modified as well as foreign keys
            if (entry.Members.Count(m => m.IsModified) > updatedForeignKeys) {
                updatedObjects.Add(entry.Entity);
            }

            return updatedObjects;
        }

        public void PreSave() {
            WrappedDbContext.ChangeTracker.DetectChanges();
            var entries = WrappedDbContext.ChangeTracker.Entries().ToArray();

            entries.ForEach(e => e.DetectChanges());

            added.AddRange(entries.Where(e => e.State == EntityState.Added).Select(ose => ose.Entity).ToList());
            
            updatingNakedObjects = entries.Where(e => e.State != EntityState.Added && e.Members.Any(m => m.IsModified)).
                                           SelectMany(CheckForForeignKeys).
                                           Distinct().
                                           Select(o => parent.CreateAdapter(null, o)).ToList();

            updatingNakedObjects.ForEach(no => no.Updating());

            // need to do complex type separately as they'll not be updated in the SavingChangesHandler as they're not proxied. 
            coUpdating = new List<INakedObjectAdapter>();
            //coUpdating = ObjectContextUtils.GetChangedComplexObjectsInContext(this).Select(obj => parent.CreateAdapter(obj)).ToList();
            //coUpdating.ForEach(no => no.Updating());
        }

        public bool HasChanges() {
            WrappedDbContext.ChangeTracker.DetectChanges();
            //return WrappedDbContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified).Any();
            return WrappedDbContext.ChangeTracker.HasChanges();
        }

        public void PostSave() {
            try {
                // Take a copy of PersistedNakedObjects and clear original so new ones can be added 
                // do this before Updated so that any objects added by updated are not immediately
                // picked up by the 'Persisted' call below.
                var currentPersistedNakedObjectsAdapter = PersistedNakedObjects.ToArray();
                PersistedNakedObjects.Clear();
                updatingNakedObjects.ForEach(no => no.Updated());
                updatingNakedObjects.ForEach(no => EFCoreHelpers.UpdateVersion(no, session, Manager));
                coUpdating.ForEach(no => no.Updated());
                currentPersistedNakedObjectsAdapter.ForEach(no => no.Persisted());
            }
            finally {
                updatingNakedObjects.Clear();
                coUpdating.Clear();
            }
        }

        public void PostSaveWrapUp() {
            // complex types give null adapter
            added.Select(domainObject => parent.CreateAdapter(null, domainObject)).Where(a => a?.Oid is IEntityOid).ForEach(parent.HandleAdded);
            LoadedNakedObjects.ToList().ForEach(parent.HandleLoaded);
        }
    }
}