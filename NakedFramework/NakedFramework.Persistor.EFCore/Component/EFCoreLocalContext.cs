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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedFramework.Persistor.EFCore.Util;

namespace NakedFramework.Persistor.EFCore.Component;

public class EFCoreLocalContext : IDisposable {
    private readonly List<object> added = new();
    private readonly IDictionary<Type, Type> baseTypeMap = new Dictionary<Type, Type>();
    private readonly ISet<Type> notPersistedTypes = new HashSet<Type>();
    private readonly ISet<Type> ownedTypes = new HashSet<Type>();
    private readonly EFCoreObjectStore parent;
    private readonly ISession session;
    private List<INakedObjectAdapter> updatingNakedObjects;

    private EFCoreLocalContext(Type[] preCachedTypes, Type[] notPersistedTypes, ISession session, EFCoreObjectStore parent) {
        this.session = session;
        this.parent = parent;

        preCachedTypes.ForEach(t => ownedTypes.Add(t));
        notPersistedTypes.ForEach(t => this.notPersistedTypes.Add(t));
    }

    public EFCoreLocalContext(DbContext context, EFCorePersistorConfiguration config, ISession session, EFCoreObjectStore parent)
        : this(config.PreCachedTypes(), config.NotPersistedTypes(), session, parent)
    {
        WrappedDbContext = context;
        Name = WrappedDbContext.ToString();
    }

    private string Name { get; }

    public DbContext WrappedDbContext { get; private set; }
    public ISet<INakedObjectAdapter> LoadedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();
    public ISet<INakedObjectAdapter> PersistedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();
    public ISet<INakedObjectAdapter> DeletedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();
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

    private Type GetMostBaseType(Type type) {
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
        LoadedNakedObjects.Clear();
        PersistedNakedObjects.Clear();
        DeletedNakedObjects.Clear();
    }

    private IEnumerable<object> CheckForForeignKeys(EntityEntry entry, IMetamodelManager metamodelManager) {
        var updatedObjects = new List<object>();
        var updatedForeignKeys = 0;
        var foreignKeys = WrappedDbContext.Model.FindEntityType(entry.Entity.GetType().GetProxiedType())?.GetForeignKeys() ?? Array.Empty<IForeignKey>();

        foreach (var foreignKey in foreignKeys) {
            var isAlternateKey = !foreignKey.PrincipalKey.IsPrimaryKey();
            var names = foreignKey.Properties.Select(p => p.Name);
            var members = entry.Members.OfType<PropertyEntry>();

            var matchingMembers = names.Select(name => members.Single(m => m.Metadata.Name == name)).ToList();

            if (matchingMembers.Any(m => m.IsModified)) {
                var type = foreignKey.PrincipalEntityType.ClrType;
                var spec = metamodelManager.GetSpecification(type);

                if (spec?.GetFacet<IUpdatingCallbackFacet>()?.IsActive is true) {
                    if (isAlternateKey) {
                        throw new NakedObjectDomainException($"Type:{type} is referenced via a foreign alternate key from {entry.Entity.GetType()} and has an 'Updating' method this is not currently supported: issue #412 on github");
                    }

                    var keys = matchingMembers.Select(m => m.OriginalValue).ToArray();
                    updatedForeignKeys++;

                    if (keys.All(k => k is not null)) {
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

    public void PreSave(IMetamodelManager metamodelManager) {
        WrappedDbContext.ChangeTracker.DetectChanges();
        var entries = WrappedDbContext.ChangeTracker.Entries().ToArray();

        entries.ForEach(e => e.DetectChanges());

        added.AddRange(entries.Where(e => e.State is EntityState.Added && WrappedDbContext.IsNotMappingObject(e.Entity.GetType())).Select(ose => ose.Entity).ToList());

        updatingNakedObjects = entries.Where(e => e.State != EntityState.Added && e.Members.Any(m => m.IsModified)).SelectMany(entry => CheckForForeignKeys(entry, metamodelManager)).Distinct().Select(o => parent.CreateAdapter(null, o)).ToList();

        updatingNakedObjects.ForEach(no => no.Updating());
    }

    public bool HasChanges() {
        WrappedDbContext.ChangeTracker.DetectChanges();
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
            updatingNakedObjects.ForEach(no => no.UpdateVersion(session));
            currentPersistedNakedObjectsAdapter.ForEach(no => no.Persisted());
        }
        finally {
            updatingNakedObjects.Clear();
        }
    }

    public void PostSaveWrapUp() {
        // complex types give null adapter
        added.Select(domainObject => parent.CreateAdapter(null, domainObject)).Where(a => a?.Oid is IDatabaseOid).ForEach(parent.HandleAdded);
        LoadedNakedObjects.ToList().ForEach(parent.HandleLoaded);
    }
}