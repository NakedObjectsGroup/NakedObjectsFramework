// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Persistor.Entity.Util;
using NakedObjects.Util;

[assembly: InternalsVisibleTo("NakedObjects.Persistor.Entity.Test")]

namespace NakedObjects.Persistor.Entity.Component {
    public sealed class EntityObjectStore : IObjectStore, IDisposable {
        #region Delegates

        public delegate INakedObjectAdapter CreateAdapterDelegate(IOid oid, object domainObject);

        public delegate INakedObjectAdapter CreateAggregatedAdapterDelegate(INakedObjectAdapter nakedObjectAdapter, PropertyInfo property, object newDomainObject);

        public delegate INakedObjectAdapter GetAdapterForDelegate(object domainObject);

        public delegate void NotifyUiDelegate(INakedObjectAdapter changedObjectAdapter);

        public delegate void RemoveAdapterDelegate(INakedObjectAdapter nakedObjectAdapter);

        public delegate void ReplacePocoDelegate(INakedObjectAdapter nakedObjectAdapter, object newDomainObject);

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(EntityObjectStore));
        private readonly GetAdapterForDelegate getAdapterFor;
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly EntityOidGenerator oidGenerator;
        private readonly ISession session;
        private IDictionary<CodeFirstEntityContextConfiguration, LocalContext> contexts = new Dictionary<CodeFirstEntityContextConfiguration, LocalContext>();
        private CreateAdapterDelegate createAdapter;
        private CreateAggregatedAdapterDelegate createAggregatedAdapter;
        private Action<INakedObjectAdapter> handleLoaded;
        private IDomainObjectInjector injector;
        private Func<Type, ITypeSpec> loadSpecification;
        private RemoveAdapterDelegate removeAdapter;
        private ReplacePocoDelegate replacePoco;
        private EventHandler savingChangesHandlerDelegate;

        internal EntityObjectStore(IMetamodelManager metamodelManager, ISession session, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager) {
            this.metamodelManager = metamodelManager;
            this.session = session;
            this.injector = injector;
            this.nakedObjectManager = nakedObjectManager;

            getAdapterFor = domainObject => this.nakedObjectManager.GetAdapterFor(domainObject);
            createAdapter = (oid, domainObject) => this.nakedObjectManager.CreateAdapter(domainObject, oid, null);
            replacePoco = (nakedObject, newDomainObject) => this.nakedObjectManager.ReplacePoco(nakedObject, newDomainObject);
            removeAdapter = o => this.nakedObjectManager.RemoveAdapter(o);
            createAggregatedAdapter = (parent, property, obj) => this.nakedObjectManager.CreateAggregatedAdapter(parent, ((IObjectSpec) parent.Spec).GetProperty(property.Name).Id, obj);

            handleLoaded = HandleLoadedDefault;
            savingChangesHandlerDelegate = SavingChangesHandler;
            loadSpecification = metamodelManager.GetSpecification;
        }

        public EntityObjectStore(ISession session, IEntityObjectStoreConfiguration config, EntityOidGenerator oidGenerator, IMetamodelManager metamodel, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager)
            : this(metamodel, session, injector, nakedObjectManager) {
            config.AssertSetup();
            this.oidGenerator = oidGenerator;
            contexts = config.ContextConfiguration.ToDictionary<CodeFirstEntityContextConfiguration, CodeFirstEntityContextConfiguration, LocalContext>(c => c, c => null);

            EnforceProxies = config.EnforceProxies;
            RequireExplicitAssociationOfTypes = config.RequireExplicitAssociationOfTypes;
            RollBackOnError = config.RollBackOnError;
            MaximumCommitCycles = config.MaximumCommitCycles;
            IsInitializedCheck = config.IsInitializedCheck;
            SetupContexts();
        }

        private static bool EnforceProxies { get; set; }

        private bool RollBackOnError { get; set; }

        // set is internally visible for testing
        internal static int MaximumCommitCycles { get; set; }
        private Func<bool> IsInitializedCheck { get; }
        internal static bool RequireExplicitAssociationOfTypes { get; private set; }

        #region IDisposable Members

        public void Dispose() {
            contexts.Values.ForEach(c => c.Dispose());
            contexts = null;
        }

        #endregion

        #region IObjectStore Members

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool parentIsGhost) {
            if (EntityFrameworkKnowsType(adapter.Object.GetEntityProxiedType())) {
                foreach (var pi in GetContext(adapter).GetComplexMembers(adapter.Object.GetEntityProxiedType())) {
                    var complexObject = pi.GetValue(adapter.Object, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    InjectParentIntoChild(adapter.Object, complexObject);
                    injector.InjectInto(complexObject);
                    createAggregatedAdapter(adapter, pi, complexObject);
                }
            }
        }

        public void AbortTransaction() => RollBackContext();

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EntityCreateObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter), this));

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EntityDestroyObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EntitySaveObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void EndTransaction() {
            try {
                using (var transaction = CreateTransactionScope()) {
                    RecurseUntilAllChangesApplied(1);
                    transaction.Complete();
                }

                PostSaveWrapUp();
            }
            catch (OptimisticConcurrencyException oce) {
                InvokeErrorFacet(new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = GetSourceNakedObject(oce)});
            }
            catch (UpdateException ue) {
                InvokeErrorFacet(new DataUpdateException(ConcatenateMessages(ue), ue));
            }
            catch (Exception e) {
                Log.Error($"Unexpected exception while applying changes {e.Message}");
                RollBackContext();
                throw;
            }
            finally {
                contexts.Values.ForEach(c => c.SaveOrUpdateComplete());
            }
        }

        public void Execute(IPersistenceCommand[] commands) => ExecuteCommands(commands);

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = TypeUtils.GetType(spec.FullName);
            return GetContext(type).GetObjectSet(type);
        }

        public INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint) {
            switch (oid) {
                case IAggregateOid aggregateOid: {
                    var parentOid = (IEntityOid) aggregateOid.ParentOid;
                    var parentType = parentOid.TypeName;
                    var parentSpec = (IObjectSpec) metamodelManager.GetSpecification(parentType);
                    var parent = createAdapter(parentOid, GetObjectByKey(parentOid, parentSpec));

                    return parentSpec.GetProperty(aggregateOid.FieldName).GetNakedObject(parent);
                }
                case IEntityOid eoid: {
                    var adapter = createAdapter(eoid, GetObjectByKey(eoid, hint));
                    adapter.UpdateVersion(session, nakedObjectManager);
                    return adapter;
                }
                default:
                    throw new NakedObjectSystemException(Log.LogAndReturn($"Unexpected oid type: {oid.GetType()}"));
            }
        }

        public bool IsInitialized => IsInitializedCheck();

        public string Name => "Entity Object Store";

        public void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field) => field.GetNakedObject(nakedObjectAdapter);

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {
            var type = TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
            var countMethod = GetType().GetMethod("Count")?.GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int) (countMethod?.Invoke(this, new object[] {nakedObjectAdapter, associationSpec, nakedObjectManager}) ?? 0);
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {
            var eoid = oidGenerator.CreateOid(type.FullName, keys);
            var hint = (IObjectSpec) loadSpecification(type);
            return GetObject(eoid, hint);
        }

        public void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter) {
            // eagerly load object        
            nakedObjectAdapter.ResolveState.Handle(Events.StartResolvingEvent);
            // only if not proxied
            var entityType = nakedObjectAdapter.Object.GetType();

            if (!TypeUtils.IsEntityProxy(entityType)) {
                var currentContext = GetContext(entityType);

                var propertynames = currentContext.GetNavigationMembers(entityType).Select(x => x.Name);
                var objectSet = currentContext.GetObjectSet(entityType);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in propertynames) {
                    objectSet = objectSet.Invoke<ObjectQuery>("Include", name);
                }

                var idmembers = currentContext.GetIdMembers(entityType);
                var keyValues = ((IEntityOid) nakedObjectAdapter.Oid).Key;
                Assert.AssertEquals("Member and value counts must match", idmembers.Length, keyValues.Length);

                var memberValueMap = MemberValueMap(idmembers, keyValues);

                var query = memberValueMap.Aggregate(string.Empty, (s, kvp) => string.Format("{0}it.{1}=@{1}", s.Length == 0 ? s : $"{s} and ", kvp.Key));
                var parms = memberValueMap.Select(kvp => new ObjectParameter(kvp.Key, kvp.Value)).ToArray();

                First(objectSet.Invoke<IQueryable>("Where", query, parms));
            }

            EndResolving(nakedObjectAdapter);
            ResolveChildCollections(nakedObjectAdapter);
        }

        public void StartTransaction() {
            // do nothing 
        }

        public IQueryable<T> GetInstances<T>() where T : class => (IQueryable<T>) GetContext(typeof(T)).GetQueryableOfDerivedType<T>();

        public IQueryable GetInstances(Type type) => (IQueryable) GetContext(type).GetQueryableOfDerivedType(type);

        public object CreateInstance(Type type) {
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (type.IsAbstract) {
                throw new ModelException(Log.LogAndReturn(string.Format(Resources.NakedObjects.CannotCreateAbstract, type)));
            }

            var domainObject = Activator.CreateInstance(type);
            injector.InjectInto(domainObject);

            if (EntityFrameworkKnowsType(type)) {
                foreach (var pi in GetContext(domainObject).GetComplexMembers(domainObject.GetType())) {
                    var complexObject = pi.GetValue(domainObject, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    injector.InjectInto(complexObject);
                }
            }

            return domainObject;
        }

        public void Reload(INakedObjectAdapter nakedObjectAdapter) => Refresh(nakedObjectAdapter);

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class => (T) CreateInstance(typeof(T));

        public PropertyInfo[] GetKeys(Type type) => GetContext(type.GetProxiedType()).GetIdMembers(type.GetProxiedType());

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() == null) {
                nakedObjectAdapter.Updating();
                GetContext(nakedObjectAdapter.Object.GetType()).WrappedObjectContext.Refresh(RefreshMode.StoreWins, nakedObjectAdapter.Object);
                nakedObjectAdapter.Updated();
            }
        }

        #endregion

        private static void Execute(IPersistenceCommand cmd) {
            try {
                ExecuteCommand(cmd);
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = cmd.OnObject()};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        private LocalContext FindContext(Type type) =>
            contexts.Values.SingleOrDefault(c => c.GetIsOwned(type)) ??
            contexts.Values.Single(c => c.GetIsKnown(type));

        private LocalContext GetContext(Type type) {
            try {
                return contexts.Count == 1 ? contexts.Values.Single() : FindContext(type);
            }
            catch (Exception e) {
                throw new NakedObjectDomainException(Log.LogAndReturn(string.Format(Resources.NakedObjects.EntityContextError, type.FullName)), e);
            }
        }

        private LocalContext GetContext(object domainObject) => GetContext(GetTypeToUse(domainObject));

        private static Type GetTypeToUse(object domainObject) {
            if (domainObject == null) {
                throw new NakedObjectSystemException(Log.LogAndReturn("Could not find Entity Framework context for null object"));
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

        private LocalContext GetContext(INakedObjectAdapter nakedObjectAdapter) => GetContext(nakedObjectAdapter.Object);

        private LocalContext PrepareContextForNewTransaction(KeyValuePair<CodeFirstEntityContextConfiguration, LocalContext> kvp) {
            var codeFirstEntityContextConfiguration = kvp.Key;

            var context = CreateCodeOnlyContext(codeFirstEntityContextConfiguration);
            context.DefaultMergeOption = codeFirstEntityContextConfiguration.DefaultMergeOption;
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = true;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = true;
            context.WrappedObjectContext.SavingChanges += savingChangesHandlerDelegate;
            context.WrappedObjectContext.ObjectStateManager.ObjectStateManagerChanged += (obj, args) => {
                if (args.Action == CollectionChangeAction.Add) {
                    LoadObjectIntoNakedObjectsFramework(args.Element, context);
                }
            };

            codeFirstEntityContextConfiguration.CustomConfig(context.WrappedObjectContext);

            context.Manager = nakedObjectManager;
            return context;
        }

        private void ResolveChildCollections(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec is IObjectSpec spec) {
                // testing check 
                foreach (var assoc in spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted)) {
                    var adapter = assoc.GetNakedObject(nakedObjectAdapter);
                    if (adapter.ResolveState.IsGhost()) {
                        Resolve(adapter, GetContext(nakedObjectAdapter));
                    }
                }
            }
        }

        private void RollBackContext(LocalContext context) {
            if (RollBackOnError) {
                var wContext = context.WrappedObjectContext;
                wContext.DetectChanges();
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).ForEach(ose => context.WrappedObjectContext.Refresh(RefreshMode.StoreWins, ose.Entity));
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).ForEach(ose => ose.ChangeState(EntityState.Detached));
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Where(ose => ose.Entity != null).ForEach(ose => context.WrappedObjectContext.Refresh(RefreshMode.StoreWins, ose.Entity));
                wContext.AcceptAllChanges();
            }
        }

        private void RollBackContext() => contexts.Values.ForEach(RollBackContext);

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter, LocalContext context) {
            var resolveEvent = !nakedObjectAdapter.ResolveState.IsTransient() &&
                               !context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled
                ? Events.StartPartResolvingEvent
                : Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.Handle(nakedObjectAdapter.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);

        private static void Resolve(INakedObjectAdapter nakedObjectAdapter, LocalContext context) {
            StartResolving(nakedObjectAdapter, context);
            EndResolving(nakedObjectAdapter);
        }

        private static void HandleLoadedDefault(INakedObjectAdapter nakedObjectAdapter) => EndResolving(nakedObjectAdapter);

        private void InvokeErrorFacet(Exception exception) {
            var newMessage = exception.Message;

            foreach (var context in contexts.Values) {
                if (context.CurrentSaveRootObjectAdapter?.Spec != null) {
                    var target = context.CurrentSaveRootObjectAdapter;
                    // can be null in tests
                    newMessage = target.Spec.GetFacet<IOnPersistingErrorCallbackFacet>().Invoke(target, exception);
                    break;
                }

                if (context.CurrentUpdateRootObjectAdapter?.Spec != null) {
                    var target = context.CurrentUpdateRootObjectAdapter;
                    // can be null in tests 
                    newMessage = target.Spec.GetFacet<IOnUpdatingErrorCallbackFacet>().Invoke(target, exception);
                    break;
                }
            }

            // Rollback after extracting info from context - rollback clears it all
            RollBackContext();

            switch (exception) {
                case ConcurrencyException concurrencyException:
                    throw new ConcurrencyException(newMessage, exception) {SourceNakedObjectAdapter = concurrencyException.SourceNakedObjectAdapter};
                case DataUpdateException _:
                    throw new DataUpdateException(newMessage, exception);
                default:
                    // should never get here - just rethrow 
                    Log.Error($"Unexpected exception {exception}");
                    throw exception;
            }
        }

        private static void SaveChanges(LocalContext context) => context.WrappedObjectContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);

        private void Save() {
            contexts.Values.ForEach(SaveChanges);
            contexts.Values.ForEach(c => c.WrappedObjectContext.AcceptAllChanges());
        }

        private bool PostSave() {
            // two separate loops as PostSave may have side-affects in previously processed contexts
            contexts.Values.ForEach(c => c.PostSave(this));
            return contexts.Values.Aggregate(false, (current, c) => current || c.HasChanges());
        }

        private void PostSaveWrapUp() => contexts.Values.ForEach(c => c.PostSaveWrapUp(this));

        private void PreSave() => contexts.Values.ForEach(c => c.PreSave());

        private INakedObjectAdapter GetSourceNakedObject(UpdateException oce) {
            var trigger = oce.StateEntries.Where(e => !e.IsRelationship).Select(e => e.Entity).SingleOrDefault();
            return createAdapter(null, trigger);
        }

        private LocalContext CreateCodeOnlyContext(CodeFirstEntityContextConfiguration codeOnlyConfig) {
            try {
                return new LocalContext(codeOnlyConfig, session, this);
            }
            catch (Exception e) {
                var originalMsg = e.Message;
                throw new InitialisationException(Log.LogAndReturn($"{Resources.NakedObjects.StartPersistorErrorCodeFirst}: {originalMsg}"), e);
            }
        }

        private void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
            var oid = (IEntityOid) nakedObjectAdapter.Oid;
            var context = GetContext(nakedObjectAdapter);
            oid.MakePersistentAndUpdateKey(context.GetKey(nakedObjectAdapter));

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {
                Resolve(nakedObjectAdapter, context);
            }

            if (nakedObjectAdapter.Spec is IObjectSpec spec) {
                // testing check 
                var adapters = spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted).Select(a => a.GetNakedObject(nakedObjectAdapter));
                foreach (var adapter in adapters) {
                    if (adapter.ResolveState.IsGhost()) {
                        Resolve(adapter, GetContext(adapter));
                    }
                }
            }

            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);
        }

        private void MarkAsLoaded(INakedObjectAdapter nakedObjectAdapter) => GetContext(nakedObjectAdapter).LoadedNakedObjects.Add(nakedObjectAdapter);

        public object GetObjectByKey(IEntityOid eoid, Type type) {
            var context = GetContext(type);
            var memberValueMap = GetMemberValueMap(type, eoid, out var entitySetName);
            var oq = context.CreateQuery(type, entitySetName);

            foreach (var (key, value) in memberValueMap) {
                var query = string.Format("it.{0}=@{0}", key);
                oq = oq.Invoke<object>("Where", query, new[] {new ObjectParameter(key, value)});
            }

            context.GetNavigationMembers(type).Where(m => !CollectionUtils.IsCollection(m.PropertyType)).ForEach(pi => oq = oq.Invoke<object>("Include", pi.Name));
            return First(oq.Invoke<IEnumerable>("Execute", context.DefaultMergeOption));
        }

        private IDictionary<string, object> GetMemberValueMap(Type type, IEntityOid eoid, out string entitySetName) {
            var context = GetContext(type);
            var set = context.GetObjectSet(type).GetProperty<EntitySet>("EntitySet");
            entitySetName = $"{set.EntityContainer.Name}.{set.Name}";
            var idmembers = context.GetIdMembers(type);
            var keyValues = eoid.Key;
            return MemberValueMap(idmembers, keyValues);
        }

        private static IDictionary<string, object> MemberValueMap(ICollection<PropertyInfo> idmembers, ICollection<object> keyValues) {
            Assert.AssertEquals("Member and value counts must match", idmembers.Count, keyValues.Count);
            return idmembers.Zip(keyValues, (k, v) => new {Key = k, Value = v})
                .ToDictionary(x => x.Key.Name, x => x.Value);
        }

        public object GetObjectByKey(IEntityOid eoid, IObjectSpec hint) => GetObjectByKey(eoid, TypeUtils.GetType(hint.FullName));

        private static object First(IEnumerable enumerable) {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // unfortunately this cast doesn't work with entity linq
            // return queryable.Cast<object>().FirstOrDefault();
            foreach (var o in enumerable) {
                return o;
            }

            return null;
        }

        public bool EntityFrameworkKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    return FindContext(type) != null;
                }
            }
            catch (Exception e) {
                // ignore all 
                Log.WarnFormat("Ignoring exception {0}", e.Message);
            }

            return false;
        }

        private static string ConcatenateMessages(Exception e) {
            var isConcurrency = e is OptimisticConcurrencyException;
            var nestLimit = 3;
            var msg = new StringBuilder(string.Format(isConcurrency ? Resources.NakedObjects.ConcurrencyErrorMessage : Resources.NakedObjects.UpdateErrorMessage, e.Message));
            while (e.InnerException != null && nestLimit-- > 0) {
                msg.AppendLine().AppendLine(isConcurrency ? Resources.NakedObjects.ConcurrencyException : Resources.NakedObjects.DataUpdateException).Append(e.InnerException.Message);
                e = e.InnerException;
            }

            return msg.ToString();
        }

        private static void InjectParentIntoChild(object parent, object child) =>
            child.GetType().GetProperties().SingleOrDefault(p => p.CanWrite &&
                                                                 p.PropertyType.IsInstanceOfType(parent) &&
                                                                 p.GetCustomAttribute<RootAttribute>() != null)?.SetValue(child, parent, null);

        private void CheckProxies(object objectToCheck) {
            var objectType = objectToCheck.GetType();
            if (!EnforceProxies || TypeUtils.IsSystem(objectType) || TypeUtils.IsMicrosoft(objectType)) {
                // may be using types provided by System or Microsoft (eg Authentication User). 
                // No point enforcing proxying on them. 
                return;
            }

            var adapter = getAdapterFor(objectToCheck);
            var isTransientObject = adapter?.Oid != null && adapter.Oid.IsTransient;
            var explanation = isTransientObject ? Resources.NakedObjects.ProxyExplanationTransient : Resources.NakedObjects.ProxyExplanation;

            Assert.AssertTrue(string.Format(Resources.NakedObjects.NoProxyMessage, objectToCheck.GetType(), explanation), TypeUtils.IsEntityProxy(objectToCheck.GetType()));
            Assert.AssertTrue(string.Format(Resources.NakedObjects.NoChangeTrackerMessage, objectToCheck.GetType(), explanation), objectToCheck is IEntityWithChangeTracker);
        }

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, LocalContext context) {
            CheckProxies(domainObject);
            var oid = oidGenerator.CreateOid(EntityUtils.GetEntityProxiedTypeName(domainObject), context.GetKey(domainObject));
            var nakedObjectAdapter = createAdapter(oid, domainObject);
            injector.InjectInto(nakedObjectAdapter.Object);
            LoadComplexTypesIntoNakedObjectFramework(nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsGhost());
            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsGhost()) {
                StartResolving(nakedObjectAdapter, context);
                MarkAsLoaded(nakedObjectAdapter);
            }
        }

        private static IEnumerable<object> GetRelationshipEnds(ObjectContext context, ObjectStateEntry /*RelationshipEntry*/ ose) {
            var key0 = (EntityKey) ose.GetType().GetProperty("Key0", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);
            var key1 = (EntityKey) ose.GetType().GetProperty("Key1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);

            var o0 = context.GetObjectByKey(key0);
            var o1 = context.GetObjectByKey(key1);

            return new[] {o0, o1};
        }

        private static IEnumerable<object> GetChangedObjectsInContext(ObjectContext context) {
            var addedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added).ToArray();
            var addedOseRelationships = addedOses.Where(ose => ose.IsRelationship);

            var deletedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).ToArray();
            var deletedOseRelationships = deletedOses.Where(ose => ose.IsRelationship);

            var changedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
            var changedEntities = changedOses.Select(x => x.Entity).ToList();

            addedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));
            deletedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));

            // this is here just to catch a case (adding sales reason to sales order in adventureworks) 
            // which doesn't work but which should.
            changedEntities.AddRange(GetRelationshipEndsForEntity(addedOses));
            changedEntities.AddRange(GetRelationshipEndsForEntity(deletedOses));

            // filter added and deleted entries 
            return changedEntities.Where(x => x != null).Distinct().Where(e => {
                context.ObjectStateManager.TryGetObjectStateEntry(e, out var ose);
                return ose != null && ose.State != EntityState.Deleted && ose.State != EntityState.Added;
            });
        }

        private static IEnumerable<object> GetRelationshipEndsForEntity(IEnumerable<ObjectStateEntry> addedOses) {
            var relatedends = addedOses.Where(ose => !ose.IsRelationship).SelectMany(x => x.RelationshipManager.GetAllRelatedEnds());
            var references = relatedends.Where(x => x.GetType().GetGenericTypeDefinition() == typeof(EntityReference<>));
            return references.Select(x => x.GetProperty<object>("Value"));
        }

        private static IEnumerable<object> GetChangedComplexObjectsInContext(LocalContext context) =>
            context.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Select(ose => new {Obj = ose.Entity, Prop = context.GetComplexMembers(ose.Entity.GetEntityProxiedType())}).SelectMany(a => a.Prop.Select(p => p.GetValue(a.Obj, null))).Where(x => x != null).Distinct();

        private static void ValidateIfRequired(INakedObjectAdapter adapter) {
            if (adapter.ResolveState.IsPersistent()) {
                if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                    var state = adapter.ValidToPersist();
                    if (state != null) {
                        throw new PersistFailedException(Log.LogAndReturn(string.Format(Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state)));
                    }
                }
            }
        }

        private void SavingChangesHandler(object sender, EventArgs e) {
            var changedObjects = GetChangedObjectsInContext((ObjectContext) sender);
            var adaptedObjects = changedObjects.Where(o => TypeUtils.IsEntityProxy(o.GetType())).Select(domainObject => nakedObjectManager.CreateAdapter(domainObject, null, null)).ToArray();
            adaptedObjects.Where(x => x.ResolveState.IsGhost()).ForEach(ResolveImmediately);
            adaptedObjects.ForEach(ValidateIfRequired);
        }

        private static void ExecuteCommand(IPersistenceCommand command) => command.Execute();

        private static void ExecuteCommands(IPersistenceCommand[] commands) => commands.ForEach(command => command.Execute());

        public void SetupContexts() {
            contexts = contexts.ToDictionary(kvp => kvp.Key, PrepareContextForNewTransaction);
            contexts.Values.ForEach(c => c.Manager = nakedObjectManager);
        }

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                var typeNames = contexts.Values.SelectMany(c => c.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified).Where(o => o.Entity != null).Select(o => o.Entity.GetEntityProxiedType().FullName)).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                throw new NakedObjectDomainException(Log.LogAndReturn(string.Format(Resources.NakedObjects.EntityCommitError, typeNames)));
            }

            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && !field.ContainsFacet<INotPersistedFacet>()) {
                using var dbContext = new DbContext(GetContext(nakedObjectAdapter).WrappedObjectContext, false);
                // check this is an EF collection 
                try {
                    return dbContext.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                }
                catch (ArgumentException) {
                    // not an EF recognised collection 
                    Log.Warn($"Attempting to 'Count' a non-EF collection: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }

        #region Nested type: EntityCreateObjectCommand

        private class EntityCreateObjectCommand : ICreateObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObjectAdapter nakedObjectAdapter;
            private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();
            private readonly EntityObjectStore parent;

            public EntityCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter, LocalContext context, EntityObjectStore parent) {
                this.context = context;
                this.parent = parent;
                this.nakedObjectAdapter = nakedObjectAdapter;
            }

            #region ICreateObjectCommand Members

            public void Execute() {
                try {
                    context.CurrentSaveRootObjectAdapter = nakedObjectAdapter;
                    objectToProxyScratchPad.Clear();
                    ProxyObjectIfAppropriate(nakedObjectAdapter.Object);
                }
                catch (Exception e) {
                    Log.Warn($"Error in EntityCreateObjectCommand.Execute: {e.Message}");
                    throw;
                }
            }

            public INakedObjectAdapter OnObject() => nakedObjectAdapter;

            #endregion

            private void SetKeyAsNecessary(object objectToProxy, object proxy) {
                if (!context.IdMembersAreIdentity(objectToProxy.GetType())) {
                    var idMembers = context.GetIdMembers(objectToProxy.GetType());
                    idMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
                }
            }

            private object ProxyObjectIfAppropriate(object originalObject) {
                if (originalObject == null) {
                    return null;
                }

                if (TypeUtils.IsEntityProxy(originalObject.GetType())) {
                    // object already proxied assume previous save failed - add object to context again 

                    var add = true;
                    if (context.WrappedObjectContext.ObjectStateManager.TryGetObjectStateEntry(originalObject, out var ose)) {
                        // EF knows object so check if detached 
                        add = ose.State == EntityState.Detached;
                    }

                    if (add) {
                        context.GetObjectSet(originalObject.GetEntityProxiedType()).Invoke("AddObject", originalObject);
                    }

                    return originalObject;
                }

                if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                    return objectToProxyScratchPad[originalObject];
                }

                var adapterForOriginalObjectAdapter = parent.createAdapter(null, originalObject);

                return adapterForOriginalObjectAdapter.ResolveState.IsPersistent()
                    ? originalObject
                    : ProxyObject(originalObject, adapterForOriginalObjectAdapter);
            }

            private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObjectAdapter) {
                var objectToAdd = context.CreateObject(originalObject.GetType());

                var proxied = objectToAdd.GetType() != originalObject.GetType();
                if (!proxied) {
                    objectToAdd = originalObject;
                }

                objectToProxyScratchPad[originalObject] = objectToAdd;
                adapterForOriginalObjectAdapter.Persisting();

                // create transient adapter here so that LoadObjectIntoNakedObjectsFramework knows proxy domainObject is transient
                // if not proxied this should just be the same as adapterForOriginalObjectAdapter
                var proxyAdapter = parent.createAdapter(null, objectToAdd);

                SetKeyAsNecessary(originalObject, objectToAdd);
                context.GetObjectSet(originalObject.GetType()).Invoke("AddObject", objectToAdd);

                if (proxied) {
                    ProxyReferencesAndCopyValuesToProxy(originalObject, objectToAdd);
                    context.PersistedNakedObjects.Add(proxyAdapter);
                    // remove temporary adapter for proxy (tidy and also means we will not get problem 
                    // with already known object in identity map when replacing the poco
                    parent.removeAdapter(proxyAdapter);
                    parent.replacePoco(adapterForOriginalObjectAdapter, objectToAdd);
                }
                else {
                    ProxyReferences(originalObject);
                    context.PersistedNakedObjects.Add(proxyAdapter);
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
                refMembers.ForEach(pi => ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)));

                var colmembers = context.GetCollectionMembers(objectToProxy.GetType());
                foreach (var pi in colmembers) {
                    foreach (var item in (IEnumerable) pi.GetValue(objectToProxy, null)) {
                        ProxyObjectIfAppropriate(item);
                    }
                }
            }

            private void ProxyReferencesAndCopyValuesToProxy(object objectToProxy, object proxy) {
                var nonIdMembers = context.GetNonIdMembers(objectToProxy.GetType());
                nonIdMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));

                var refMembers = context.GetReferenceMembers(objectToProxy.GetType());
                refMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)), null));

                var colmembers = context.GetCollectionMembers(objectToProxy.GetType());
                foreach (var pi in colmembers) {
                    var toCol = proxy.GetType().GetProperty(pi.Name).GetValue(proxy, null);
                    var fromCol = (IEnumerable) pi.GetValue(objectToProxy, null);
                    foreach (var item in fromCol) {
                        toCol.Invoke("Add", ProxyObjectIfAppropriate(item));
                    }
                }

                var notPersistedMembers = objectToProxy.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<NotPersistedAttribute>() != null).ToArray();
                notPersistedMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
            }

            public override string ToString() => $"CreateObjectCommand [object={nakedObjectAdapter}]";
        }

        #endregion

        #region Nested type: EntityDestroyObjectCommand

        private class EntityDestroyObjectCommand : IDestroyObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObjectAdapter nakedObjectAdapter;

            public EntityDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter, LocalContext context) {
                this.context = context;
                this.nakedObjectAdapter = nakedObjectAdapter;
            }

            #region IDestroyObjectCommand Members

            public void Execute() {
                context.WrappedObjectContext.DeleteObject(nakedObjectAdapter.Object);
                context.DeletedNakedObjects.Add(nakedObjectAdapter);
            }

            public INakedObjectAdapter OnObject() => nakedObjectAdapter;

            #endregion

            public override string ToString() => $"DestroyObjectCommand [object={nakedObjectAdapter}]";
        }

        #endregion

        #region Nested type: EntitySaveObjectCommand

        private class EntitySaveObjectCommand : ISaveObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObjectAdapter nakedObjectAdapter;

            public EntitySaveObjectCommand(INakedObjectAdapter nakedObjectAdapter, LocalContext context) {
                this.context = context;
                this.nakedObjectAdapter = nakedObjectAdapter;
            }

            #region ISaveObjectCommand Members

            public void Execute() => context.CurrentUpdateRootObjectAdapter = nakedObjectAdapter;

            public INakedObjectAdapter OnObject() => nakedObjectAdapter;

            #endregion

            public override string ToString() => $"SaveObjectCommand [object={nakedObjectAdapter}]";
        }

        #endregion

        #region Nested type: LocalContext

        public class LocalContext : IDisposable {
            private readonly List<object> added = new List<object>();
            private readonly IDictionary<Type, Type> baseTypeMap = new Dictionary<Type, Type>();
            private readonly ISet<Type> notPersistedTypes = new HashSet<Type>();
            private readonly ISet<Type> ownedTypes = new HashSet<Type>();
            private readonly EntityObjectStore parent;
            private readonly ISession session;
            private readonly IDictionary<Type, StructuralType> typeToStructuralType = new Dictionary<Type, StructuralType>();
            private List<INakedObjectAdapter> coUpdating;
            private List<INakedObjectAdapter> updatingNakedObjects;

            private LocalContext(Type[] preCachedTypes, Type[] notPersistedTypes, ISession session, EntityObjectStore parent) {
                this.session = session;
                this.parent = parent;

                preCachedTypes.ForEach(t => ownedTypes.Add(t));
                notPersistedTypes.ForEach(t => this.notPersistedTypes.Add(t));
            }

            public LocalContext(CodeFirstEntityContextConfiguration config, ISession session, EntityObjectStore parent)
                : this(config.PreCachedTypes(), config.NotPersistedTypes(), session, parent) {
                WrappedObjectContext = ((IObjectContextAdapter) config.DbContext()).ObjectContext;
                Name = WrappedObjectContext.DefaultContainerName;
            }

            public INakedObjectManager Manager { protected get; set; }
            public ObjectContext WrappedObjectContext { get; private set; }
            public string Name { get; }

            public ISet<INakedObjectAdapter> LoadedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

            public ISet<INakedObjectAdapter> PersistedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

            public ISet<INakedObjectAdapter> DeletedNakedObjects { get; } = new HashSet<INakedObjectAdapter>();

            public bool IsInitialized { get; set; }
            public MergeOption DefaultMergeOption { get; set; }
            public INakedObjectAdapter CurrentSaveRootObjectAdapter { get; set; }
            public INakedObjectAdapter CurrentUpdateRootObjectAdapter { get; set; }

            #region IDisposable Members

            public void Dispose() {
                try {
                    WrappedObjectContext.Dispose();
                    WrappedObjectContext = null;
                    baseTypeMap.Clear();
                }
                catch (Exception e) {
                    Log.ErrorFormat("Exception disposing context: {0}", e, Name);
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

            public StructuralType GetStructuralType(Type type) {
                if (!typeToStructuralType.ContainsKey(type)) {
                    typeToStructuralType[type] = ObjectContextUtils.GetStructuralType(WrappedObjectContext, type);
                }

                return typeToStructuralType[type];
            }

            private bool IsAlwaysUnrecognised(Type type) =>
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

                if (this.ContextKnowsType(type)) {
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

            public void PreSave() {
                WrappedObjectContext.DetectChanges();
                added.AddRange(WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).Where(ose => !ose.IsRelationship).Select(ose => ose.Entity).ToList());
                updatingNakedObjects = GetChangedObjectsInContext(WrappedObjectContext).Select(obj => parent.createAdapter(null, obj)).ToList();
                updatingNakedObjects.ForEach(no => no.Updating());

                // need to do complex type separately as they'll not be updated in the SavingChangesHandler as they're not proxied. 
                coUpdating = GetChangedComplexObjectsInContext(this).Select(obj => parent.createAdapter(null, obj)).ToList();
                coUpdating.ForEach(no => no.Updating());
            }

            public bool HasChanges() {
                WrappedObjectContext.DetectChanges();
                return WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified).Any();
            }

            public void PostSave(EntityObjectStore store) {
                try {
                    // Take a copy of PersistedNakedObjects and clear original so new ones can be added 
                    // do this before Updated so that any objects added by updated are not immediately
                    // picked up by the 'Persisted' call below.
                    var currentPersistedNakedObjectsAdapter = PersistedNakedObjects.ToArray();
                    PersistedNakedObjects.Clear();
                    updatingNakedObjects.ForEach(no => no.Updated());
                    updatingNakedObjects.ForEach(no => no.UpdateVersion(session, Manager));
                    coUpdating.ForEach(no => no.Updated());
                    currentPersistedNakedObjectsAdapter.ForEach(no => no.Persisted());
                }
                finally {
                    updatingNakedObjects.Clear();
                    coUpdating.Clear();
                }
            }

            public void PostSaveWrapUp(EntityObjectStore store) {
                added.Select(domainObject => parent.createAdapter(null, domainObject)).ForEach(store.HandleAdded);
                LoadedNakedObjects.ToList().ForEach(parent.handleLoaded);
            }
        }

        #endregion

        #region for testing only

        // ReSharper disable once UnusedMember.Global
        // used in F# code
        public void SetupForTesting(IDomainObjectInjector domainObjectInjector,
                                    CreateAdapterDelegate createAdapterDelegate,
                                    ReplacePocoDelegate replacePocoDelegate,
                                    RemoveAdapterDelegate removeAdapterDelegate,
                                    CreateAggregatedAdapterDelegate createAggregatedAdapterDelegate,
                                    Action<INakedObjectAdapter> handleLoadedTest,
                                    EventHandler savingChangeshandler,
                                    Func<Type, IObjectSpec> loadSpecificationHandler) {
            injector = domainObjectInjector;
            createAdapter = createAdapterDelegate;
            replacePoco = replacePocoDelegate;
            removeAdapter = removeAdapterDelegate;
            createAggregatedAdapter = createAggregatedAdapterDelegate;

            savingChangesHandlerDelegate = savingChangeshandler;
            handleLoaded = handleLoadedTest;
            EnforceProxies = false;
            RollBackOnError = true;
            loadSpecification = loadSpecificationHandler;
        }

        public void SetProxyingAndDeferredLoading(LocalContext context, bool newValue) {
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = newValue;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = newValue;
        }

        // ReSharper disable once UnusedMember.Global
        // used in F#code
        public void SetProxyingAndDeferredLoading(bool newValue) => contexts.Values.ForEach(c => SetProxyingAndDeferredLoading(c, newValue));

        #endregion
    }
}