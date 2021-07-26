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
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Persist;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedFramework.Persistor.EFCore.Util;

[assembly: InternalsVisibleTo("NakedFramework.Persistor.Entity.Test")]
[assembly: InternalsVisibleTo("NakedFramework.Persistor.EFCore.Test")]

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreObjectStore : IObjectStore, IDisposable {
        private readonly Dictionary<ComplexTypeMatcher, (INakedObjectAdapter, PropertyInfo)> adaptersWithComplexChildren = new();
        private readonly EFCorePersistorConfiguration config;
        internal readonly ILogger<EFCoreObjectStore> Logger;
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IOidGenerator oidGenerator;
        private readonly ISession session;
        private EFCoreLocalContext[] contexts;
        internal Func<IOid, object, INakedObjectAdapter> CreateAdapter;
        internal Func<INakedObjectAdapter, PropertyInfo, object, INakedObjectAdapter> CreateAggregatedAdapter;
        private Func<IDictionary<object, object>, bool> functionalPostSave = _ => false;
        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();
        internal Action<INakedObjectAdapter> HandleLoaded;
        private IDomainObjectInjector injector;
        internal Action<INakedObjectAdapter> RemoveAdapter;
        internal Action<INakedObjectAdapter, object> ReplacePoco;
        private Action<object> savingChanges;

        public EFCoreObjectStore(EFCorePersistorConfiguration config,
                                 IOidGenerator oidGenerator,
                                 INakedObjectManager nakedObjectManager,
                                 ISession session,
                                 IMetamodelManager metamodelManager,
                                 IDomainObjectInjector injector,
                                 ILogger<EFCoreObjectStore> logger) {
            this.config = config;
            this.oidGenerator = oidGenerator;
            this.nakedObjectManager = nakedObjectManager;
            this.session = session;
            this.metamodelManager = metamodelManager;
            this.injector = injector;
            Logger = logger;
            MaximumCommitCycles = config.MaximumCommitCycles;

            CreateAdapter = (oid, domainObject) => this.nakedObjectManager.CreateAdapter(domainObject, oid, null);
            ReplacePoco = (nakedObject, newDomainObject) => this.nakedObjectManager.ReplacePoco(nakedObject, newDomainObject);
            RemoveAdapter = o => this.nakedObjectManager.RemoveAdapter(o);
            CreateAggregatedAdapter = (parent, property, obj) => this.nakedObjectManager.CreateAggregatedAdapter(parent, ((IObjectSpec) parent.Spec).GetProperty(property.Name).Id, obj);
            HandleLoaded = HandleLoadedDefault;
            savingChanges = SavingChangesHandler;

            SetupContexts();
        }

        // internal and settable for testing
        internal int MaximumCommitCycles { get; set; }

        public void Dispose() => contexts.ForEach(c => c.Dispose());

        public bool IsInitialized => true;
        public string Name => "EF Core Object Store";

        public void AbortTransaction() => RollBackContext();

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreCreateObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter), this));

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreDestroyObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreSaveObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void EndTransaction() {
            try {
                using (var transaction = CreateTransactionScope()) {
                    RecurseUntilAllChangesApplied(1);
                    transaction.Complete();
                }

                PostSaveWrapUp();
            }
            catch (DbUpdateConcurrencyException oce) {
                InvokeErrorFacet(new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = GetSourceNakedObject(oce)});
            }
            catch (DbUpdateException ue) {
                InvokeErrorFacet(new DataUpdateException(ConcatenateMessages(ue), ue));
            }
            catch (Exception e) {
                Logger.LogError($"Unexpected exception while applying changes {e.Message}");
                RollBackContext();
                throw;
            }
            finally {
                contexts.ForEach(c => c.SaveOrUpdateComplete());
            }
        }

        public IQueryable<T> GetInstances<T>() where T : class => GetContext(typeof(T)).WrappedDbContext.Set<T>();

        public IQueryable GetInstances(Type type) {
            var dbContext = GetContext(type).WrappedDbContext;
            var mi = dbContext.GetType().GetMethod("Set", Array.Empty<Type>())?.MakeGenericMethod(type);
            return (IQueryable) mi?.Invoke(dbContext, Array.Empty<object>());
        }

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = TypeUtils.GetType(spec.FullName);
            return GetInstances(type);
        }

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class => (T) CreateInstance(typeof(T));

        public object CreateInstance(Type type) {
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (type.IsAbstract) {
                throw new ModelException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.CannotCreateAbstract, type)));
            }

            var domainObject = Activator.CreateInstance(type);
            injector.InjectInto(domainObject);

            if (EFCoreKnowsType(type)) {
                foreach (var pi in GetContext(domainObject).WrappedDbContext.GetComplexMembers(domainObject.GetType())) {
                    var complexObject = pi.GetValue(domainObject, null);
                    if (complexObject == null) {
                        throw new NakedObjectSystemException("Complex type members should never be null");
                    }

                    injector.InjectInto(complexObject);
                }
            }

            return domainObject;
        }

        public INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint) {
            switch (oid) {
                case IAggregateOid aggregateOid: {
                    var parentOid = (IDatabaseOid) aggregateOid.ParentOid;
                    var parentType = parentOid.TypeName;
                    var parentSpec = (IObjectSpec) metamodelManager.GetSpecification(parentType);
                    var parent = CreateAdapter(parentOid, GetObjectByKey(parentOid, parentSpec));

                    return parentSpec.GetProperty(aggregateOid.FieldName).GetNakedObject(parent);
                }
                case IDatabaseOid databaseOid: {
                    var adapter = CreateAdapter(databaseOid, GetObjectByKey(databaseOid, hint));
                    adapter.UpdateVersion(session);
                    return adapter;
                }
                default:
                    throw new NakedObjectSystemException(Logger.LogAndReturn($"Unexpected oid type: {oid.GetType()}"));
            }
        }

        public void Reload(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field) {
            throw new NotImplementedException();
        }

        public void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public object Resolve(object domainObject) {
            var context = GetContext(domainObject).WrappedDbContext;
            var entityType = domainObject.GetEFCoreProxiedType();
            var references = context.GetReferenceMembers(entityType);
            var collection = context.GetCollectionMembers(entityType);

            foreach (var pi in references.Union(collection)) {
                pi.GetValue(domainObject, null);
            }

            return domainObject;
        }

        public void Execute(IPersistenceCommand[] commands) {
            throw new NotImplementedException();
        }

        public void StartTransaction() { }

        public PropertyInfo[] GetKeys(Type type) => GetContext(type).WrappedDbContext.SafeGetKeys(type);

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() is null) {
                nakedObjectAdapter.Updating();
                var toRefresh = nakedObjectAdapter.Object;
                var dbContext = GetContext(toRefresh).WrappedDbContext;
                var entity = dbContext.Entry(toRefresh);
                var keys = dbContext.GetKeyValues(toRefresh);
                entity.State = EntityState.Detached;
                var refreshed = dbContext.Find(toRefresh.GetEFCoreProxiedType(), keys);
                var refreshedAdapter = nakedObjectManager.GetAdapterFor(refreshed);
                RemoveAdapter(refreshedAdapter);
                CopyTo(toRefresh, refreshed);
                nakedObjectAdapter.Updated();
            }
        }

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {
            var type = TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
            var countMethod = GetType().GetMethod("Count")?.GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int) (countMethod?.Invoke(this, new object[] {nakedObjectAdapter, associationSpec, nakedObjectManager}) ?? 0);
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {
            var obj = GetContext(type).WrappedDbContext.Find(type, keys);
            var oid = oidGenerator.CreateOid(type.FullName, keys);
            var adapter = CreateAdapter(oid, obj);
            adapter.UpdateVersion(session);
            return adapter;
        }

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool isGhost) {
            var proxiedType = adapter.Object.GetEFCoreProxiedType();

            if (contexts.All(c => c.IsAlwaysUnrecognised(proxiedType))) {
                return;
            }

            if (EFCoreKnowsType(proxiedType)) {
                var dbContext = GetContext(proxiedType).WrappedDbContext;
                foreach (var pi in dbContext.GetComplexMembers(proxiedType)) {
                    var complexObject = pi.GetValue(adapter.Object, null);
                    if (complexObject == null) {
                        throw new NakedObjectSystemException("Complex type members should never be null");
                    }

                    adaptersWithComplexChildren[new ComplexTypeMatcher(adapter.Object.GetEFCoreProxiedType(), dbContext.GetKeyValues(adapter.Object).Single())] = (adapter, pi);
                }
            }
        }

        public IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects) {
            functionalPostSave = objects.PostSaveFunction;
            return SetFunctionalProxyMap(ExecuteDetachedObjectCommand(objects));
        }

        public bool HasChanges() => contexts.Any(c => c.WrappedDbContext.ChangeTracker.HasChanges());

        public T ValidateProxy<T>(T toCheck) where T : class => toCheck;

        internal void SetupContexts() {
            contexts = config.Contexts.Select(c => new EFCoreLocalContext(c, config, session, this)).ToArray();
            foreach (var context in contexts) {
                context.WrappedDbContext.ChangeTracker.StateChanged += (_, args) => {
                    if (args.OldState == EntityState.Added) {
                        LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context.WrappedDbContext);
                    }

                    if (args.NewState == EntityState.Modified) {
                        savingChanges(args.Entry.Entity);
                    }
                };

                context.WrappedDbContext.ChangeTracker.Tracked += (_, args) => { LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context.WrappedDbContext); };
            }
        }

        private void SavingChangesHandler(object changedObject) {
            var adaptedObject = CreateAdapter(null, changedObject);
            if (adaptedObject.ResolveState.IsGhost()) {
                ResolveImmediately(adaptedObject);
            }

            ValidateIfRequired(adaptedObject);
        }

        private void ValidateIfRequired(INakedObjectAdapter adapter) {
            if (adapter.ResolveState.IsPersistent()) {
                if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                    if (adapter.ValidToPersist() is { } state) {
                        throw new PersistFailedException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state)));
                    }
                }
            }
        }

        private void PostSaveWrapUp() => contexts.ForEach(c => c.PostSaveWrapUp());

        internal void InvokeErrorFacet(Exception exception) {
            var newMessage = exception.Message;

            foreach (var context in contexts) {
                if (context.CurrentSaveRootObjectAdapter?.Spec != null) {
                    var target = context.CurrentSaveRootObjectAdapter;
                    // can be null in tests
                    newMessage = target.Spec.GetFacet<IOnPersistingErrorCallbackFacet>()?.Invoke(target, exception);
                    break;
                }

                if (context.CurrentUpdateRootObjectAdapter?.Spec != null) {
                    var target = context.CurrentUpdateRootObjectAdapter;
                    // can be null in tests 
                    newMessage = target.Spec.GetFacet<IOnUpdatingErrorCallbackFacet>()?.Invoke(target, exception);
                    break;
                }
            }

            // Rollback after extracting info from context - rollback clears it all
            RollBackContext();

            newMessage ??= exception.Message;

            switch (exception) {
                case ConcurrencyException concurrencyException:
                    throw new ConcurrencyException(newMessage, exception) {SourceNakedObjectAdapter = concurrencyException.SourceNakedObjectAdapter};
                case DataUpdateException:
                    throw new DataUpdateException(newMessage, exception);
                default:
                    // should never get here - just rethrow 
                    Logger.LogError($"Unexpected exception {exception}");
                    throw exception;
            }
        }

        private INakedObjectAdapter GetSourceNakedObject(DbUpdateException oce) {
            var trigger = oce.Entries.Select(e => e.Entity).FirstOrDefault();
            return CreateAdapter(null, trigger);
        }

        private bool EFCoreKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    return contexts.Any(c => c.WrappedDbContext.HasEntityType(type));
                }
            }
            catch (Exception e) {
                // ignore all 
                Logger.LogWarning($"Ignoring exception {e.Message}");
            }

            return false;
        }

        // internal for testing
        internal object GetObjectByKey(IDatabaseOid eoid, Type type) => GetContext(type).WrappedDbContext.Find(type, eoid.Key);

        private object GetObjectByKey(IDatabaseOid eoid, IObjectSpec hint) => GetObjectByKey(eoid, TypeUtils.GetType(hint.FullName));

        private void CopyTo(object toObj, object fromObj) {
            var toObjType = toObj.GetEFCoreProxiedType();
            var fromObjType = fromObj.GetEFCoreProxiedType();

            if (toObjType != fromObjType) {
                throw new PersistFailedException($"cannot copy different types {toObjType} and {fromObjType}");
            }

            var properties = GetContext(toObjType).WrappedDbContext.GetCloneableMembers(toObjType);

            properties.ForEach(pi => pi.SetValue(toObj, pi.GetValue(fromObj, null)));
        }

        private void LoadComplexTypesIntoNakedObjectFramework(object complexObject) {
            var dbContext = GetContext(complexObject).WrappedDbContext;
            var entry = dbContext.Entry(complexObject);

            var foreignKey = dbContext.Model.FindEntityType(entry.Entity.GetType().GetProxiedType()).GetForeignKeys().SingleOrDefault();

            if (foreignKey is not null) {
                var ownerEntityType = foreignKey.PrincipalEntityType;
                var key = dbContext.GetForeignKeyValues(complexObject, ownerEntityType).Single();
                var matcher = new ComplexTypeMatcher(ownerEntityType.ClrType, key);

                if (adaptersWithComplexChildren.ContainsKey(matcher)) {
                    var (adapter, pi) = adaptersWithComplexChildren[matcher];

                    injector.InjectParentIntoChild(adapter.Object, complexObject);
                    injector.InjectInto(complexObject);
                    CreateAggregatedAdapter(adapter, pi, complexObject);
                }
            }
        }

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void Save() {
            contexts.ForEach(c => c.WrappedDbContext.SaveChanges());
        }

        private bool PostSave() {
            functionalPostSave(functionalProxyMap);
            contexts.ForEach(c => c.PostSave());
            return contexts.Any(c => c.HasChanges());
        }

        private void PreSave() => contexts.ForEach(c => c.PreSave());

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                var typeNames = contexts.SelectMany(c => c.WrappedDbContext.ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified).Select(o => o.Entity.GetEFCoreProxiedType().FullName)).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, typeNames)));
            }

            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        private static string ConcatenateMessages(Exception e) {
            var isConcurrency = e is DbUpdateConcurrencyException;
            var nestLimit = 3;
            var msg = new StringBuilder(string.Format(isConcurrency ? NakedObjects.Resources.NakedObjects.ConcurrencyErrorMessage : NakedObjects.Resources.NakedObjects.UpdateErrorMessage, e.Message));
            while (e.InnerException is not null && nestLimit-- > 0) {
                msg.AppendLine().AppendLine(isConcurrency ? NakedObjects.Resources.NakedObjects.ConcurrencyException : NakedObjects.Resources.NakedObjects.DataUpdateException).Append(e.InnerException.Message);
                e = e.InnerException;
            }

            return msg.ToString();
        }

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter) {
            var resolveEvent = Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.Handle(nakedObjectAdapter.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);

        private static void Resolve(INakedObjectAdapter nakedObjectAdapter) {
            StartResolving(nakedObjectAdapter);
            EndResolving(nakedObjectAdapter);
        }

        private void MarkAsLoaded(INakedObjectAdapter nakedObjectAdapter) =>
            GetContext(nakedObjectAdapter.Object).LoadedNakedObjects.Add(nakedObjectAdapter);

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, DbContext context) {
            var keys = context.GetKeyValues(domainObject);
            if (keys.Any()) {
                var oid = oidGenerator.CreateOid(EFCoreHelpers.GetEFCoreProxiedTypeName(domainObject), keys);
                var nakedObjectAdapter = CreateAdapter(oid, domainObject);
                injector.InjectInto(nakedObjectAdapter.Object);
                LoadComplexTypesIntoNakedObjectFramework(nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsGhost());
                nakedObjectAdapter.UpdateVersion(session);

                if (nakedObjectAdapter.ResolveState.IsGhost()) {
                    StartResolving(nakedObjectAdapter);
                    MarkAsLoaded(nakedObjectAdapter);
                }
            }
            else {
                LoadComplexTypesIntoNakedObjectFramework(domainObject);
            }
        }

        internal void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
            var oid = (IDatabaseOid) nakedObjectAdapter.Oid;
            var key = GetContext(nakedObjectAdapter.Object).WrappedDbContext.GetKeyValues(nakedObjectAdapter.Object);

            if (key is null || !key.Any()) {
                // complex type ?
                return;
            }

            oid.UpdateKey(key);

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {
                Resolve(nakedObjectAdapter);
            }

            if (nakedObjectAdapter.Spec is IObjectSpec spec) {
                static INakedObjectAdapter GetCollection(IOneToManyAssociationSpec assoc, INakedObjectAdapter owner) =>
                    assoc.GetNakedObject(owner) ?? throw new PersistFailedException($"Unexpected null collection {assoc.Name} on {owner.Spec.FullName}");

                // testing check 
                var ghostAdapters = spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted).Select(a => GetCollection(a, nakedObjectAdapter)).Where(a => a.ResolveState.IsGhost());
                foreach (var adapter in ghostAdapters) {
                    Resolve(adapter);
                }
            }

            nakedObjectAdapter.UpdateVersion(session);
        }

        private static bool FieldIsPersisted(IAssociationSpec field) => !(field.ContainsFacet<INotPersistedFacet>() || field.ContainsFacet<IDisplayAsPropertyFacet>());

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && FieldIsPersisted(field)) {
                // check this is an EF collection 
                try {
                    return GetContext(nakedObjectAdapter.Object).WrappedDbContext.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                }
                catch (ArgumentException) {
                    // not an EF recognized collection 
                    Logger.LogWarning($"Attempting to 'Count' a non-EF collection: {field.Id}  annotate with 'NotPersisted' to avoid this warning");
                }
                catch (InvalidOperationException) {
                    // not an EF recognised entity 
                    Logger.LogWarning($"Attempting to 'Count' a non attached entity: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }

        private IList<(object original, object updated)> SetFunctionalProxyMap(IList<(object original, object updated)> updatedTuples) {
            functionalProxyMap = updatedTuples.ToDictionary(t => t.original, t => t.updated);
            return updatedTuples;
        }

        private static IList<(object original, object updated)> Execute(EFCoreDetachedObjectCommand cmd) {
            try {
                return cmd.Execute();
            }
            catch (DbUpdateConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce);
            }
            catch (DbUpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        private static void Execute(IPersistenceCommand cmd) {
            try {
                cmd.Execute();
            }
            catch (DbUpdateConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = cmd.OnObject()};
            }
            catch (DbUpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        private IList<(object original, object updated)> ExecuteDetachedObjectCommand(IDetachedObjects objects) =>
            Execute(new EFCoreDetachedObjectCommand(objects, this));

        private EFCoreLocalContext FindContext(Type type) =>
            contexts.SingleOrDefault(c => c.GetIsOwned(type)) ??
            contexts.Single(c => c.GetIsKnown(type));

        private EFCoreLocalContext GetContext(Type type) {
            try {
                return contexts.Length == 1 ? contexts.Single() : FindContext(type);
            }
            catch (Exception e) {
                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityContextError, type.FullName)), e);
            }
        }

        internal EFCoreLocalContext GetContext(object domainObject) => GetContext(GetTypeToUse(domainObject));

        private Type GetTypeToUse(object domainObject) {
            if (domainObject == null) {
                throw new NakedObjectSystemException(Logger.LogAndReturn("Could not find Entity Framework context for null object"));
            }

            var objectType = domainObject.GetType();
            if (CollectionUtils.IsGenericEnumerableOfRefType(objectType)) {
                return objectType.GetGenericArguments().First();
            }

            if (objectType.HasElementType) {
                return objectType.GetElementType();
            }

            return CollectionUtils.IsCollection(objectType)
                ? GetTypeToUse(((IEnumerable) domainObject).Cast<object>().FirstOrDefault())
                : objectType;
        }

        private EFCoreLocalContext GetContext(INakedObjectAdapter nakedObjectAdapter) => GetContext(nakedObjectAdapter.Object);

        internal bool EmptyKeys(object obj) => GetKeys(obj.GetType()).Select(k => k.GetValue(obj, null)).All(TypeKeyUtils.EmptyKey);

        public bool IsNotPersisted(object owner, PropertyInfo pi) {
            if (metamodelManager.GetSpecification(owner.GetEFCoreProxiedType()) is IObjectSpec objectSpec) {
                return objectSpec.Properties.SingleOrDefault(p => p.Id == pi.Name)?.ContainsFacet<INotPersistedFacet>() == true;
            }

            return false;
        }

        private static void HandleLoadedDefault(INakedObjectAdapter nakedObjectAdapter) => EndResolving(nakedObjectAdapter);

        private void RollBackContext() => SetupContexts();

        public void SetupForTesting(IDomainObjectInjector domainObjectInjector,
                                    Func<IOid, object, INakedObjectAdapter> createAdapter,
                                    Action<INakedObjectAdapter, object> replacePoco,
                                    Action<INakedObjectAdapter> removeAdapter,
                                    Func<INakedObjectAdapter, PropertyInfo, object, INakedObjectAdapter> createAggregatedAdapter,
                                    Action<INakedObjectAdapter> handleLoadedTest,
                                    Action<object> savingChangesHandler,
                                    Func<Type, IObjectSpec> loadSpecificationHandler
        ) {
            injector = domainObjectInjector;
            CreateAdapter = createAdapter;
            ReplacePoco = replacePoco;
            RemoveAdapter = removeAdapter;
            CreateAggregatedAdapter = createAggregatedAdapter;

            savingChanges = savingChangesHandler;
            HandleLoaded = handleLoadedTest;
        }

        private record ComplexTypeMatcher(Type Type, object Key);
    }
}