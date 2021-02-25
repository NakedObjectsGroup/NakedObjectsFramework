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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Persist;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.Entity.Configuration;
using NakedFramework.Persistor.Entity.Util;
using NakedObjects;

[assembly: InternalsVisibleTo("NakedFramework.Persistor.Entity.Test")]

namespace NakedFramework.Persistor.Entity.Component {
    public sealed class EntityObjectStore : IObjectStore, IDisposable {
        private readonly GetAdapterForDelegate getAdapterFor;
        internal readonly ILogger<EntityObjectStore> logger;
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly EntityOidGenerator oidGenerator;
        private readonly ISession session;
        private IDictionary<CodeFirstEntityContextConfiguration, LocalContext> contexts = new Dictionary<CodeFirstEntityContextConfiguration, LocalContext>();
        internal CreateAdapterDelegate createAdapter;
        internal CreateAggregatedAdapterDelegate createAggregatedAdapter;

        public Func<IDictionary<object, object>, bool> FunctionalPostSave = _ => false;

        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();
        internal Action<INakedObjectAdapter> handleLoaded;
        private IDomainObjectInjector injector;
        private Func<Type, ITypeSpec> loadSpecification;
        internal RemoveAdapterDelegate removeAdapter;
        internal ReplacePocoDelegate replacePoco;
        private EventHandler savingChangesHandlerDelegate;

        internal EntityObjectStore(IMetamodelManager metamodelManager, ISession session, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager, ILogger<EntityObjectStore> logger) {
            this.metamodelManager = metamodelManager ?? throw new InitialisationException($"{nameof(metamodelManager)} is null");
            this.session = session ?? throw new InitialisationException($"{nameof(session)} is null");
            this.injector = injector ?? throw new InitialisationException($"{nameof(injector)} is null");
            this.nakedObjectManager = nakedObjectManager ?? throw new InitialisationException($"{nameof(nakedObjectManager)} is null");
            this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");

            getAdapterFor = domainObject => this.nakedObjectManager.GetAdapterFor(domainObject);
            createAdapter = (oid, domainObject) => this.nakedObjectManager.CreateAdapter(domainObject, oid, null);
            replacePoco = (nakedObject, newDomainObject) => this.nakedObjectManager.ReplacePoco(nakedObject, newDomainObject);
            removeAdapter = o => this.nakedObjectManager.RemoveAdapter(o);
            createAggregatedAdapter = (parent, property, obj) => this.nakedObjectManager.CreateAggregatedAdapter(parent, ((IObjectSpec) parent.Spec).GetProperty(property.Name).Id, obj);

            handleLoaded = HandleLoadedDefault;
            savingChangesHandlerDelegate = SavingChangesHandler;
            loadSpecification = metamodelManager.GetSpecification;
        }

        public EntityObjectStore(ISession session, IEntityObjectStoreConfiguration config, EntityOidGenerator oidGenerator, IMetamodelManager metamodel, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager, ILogger<EntityObjectStore> logger)
            : this(metamodel, session, injector, nakedObjectManager, logger) {
            config.Validate();
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

        private static IList<(object original, object updated)> Execute(EntityPersistUpdateDetachedObjectCommand cmd) {
            try {
                return cmd.Execute();
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
                throw new NakedObjectDomainException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityContextError, type.FullName)), e);
            }
        }

        internal LocalContext GetContext(object domainObject) => GetContext(GetTypeToUse(domainObject));

        private Type GetTypeToUse(object domainObject) {
            if (domainObject == null) {
                throw new NakedObjectSystemException(logger.LogAndReturn("Could not find Entity Framework context for null object"));
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
                case DataUpdateException _:
                    throw new DataUpdateException(newMessage, exception);
                default:
                    // should never get here - just rethrow 
                    logger.LogError($"Unexpected exception {exception}");
                    throw exception;
            }
        }

        private static void SaveChanges(LocalContext context) => context.WrappedObjectContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);

        private void Save() {
            contexts.Values.ForEach(SaveChanges);
            contexts.Values.ForEach(c => c.WrappedObjectContext.AcceptAllChanges());
        }

        private IList<(object original, object updated)> SetFunctionalProxyMap(IList<(object original, object updated)> updatedTuples) {
            functionalProxyMap = updatedTuples.ToDictionary(t => t.original, t => t.updated);
            return updatedTuples;
        }

        private bool PostSave() {
            var changes = FunctionalPostSave(functionalProxyMap);

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
                throw new InitialisationException(logger.LogAndReturn($"{NakedObjects.Resources.NakedObjects.StartPersistorErrorCodeFirst}: {originalMsg}"), e);
            }
        }

        internal void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
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
            return ObjectContextUtils.First(oq.Invoke<IEnumerable>("Execute", context.DefaultMergeOption));
        }

        private IDictionary<string, object> GetMemberValueMap(Type type, IEntityOid eoid, out string entitySetName) {
            var context = GetContext(type);
            var set = context.GetObjectSet(type).GetProperty<EntitySet>("EntitySet");
            entitySetName = $"{set.EntityContainer.Name}.{set.Name}";
            var idmembers = context.GetIdMembers(type);
            var keyValues = eoid.Key;
            return ObjectContextUtils.MemberValueMap(idmembers, keyValues);
        }

        public object GetObjectByKey(IEntityOid eoid, IObjectSpec hint) => GetObjectByKey(eoid, NakedObjects.TypeUtils.GetType(hint.FullName));

        public bool EntityFrameworkKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    return FindContext(type) != null;
                }
            }
            catch (Exception e) {
                // ignore all 
                logger.LogWarning($"Ignoring exception {e.Message}");
            }

            return false;
        }

        private static string ConcatenateMessages(Exception e) {
            var isConcurrency = e is OptimisticConcurrencyException;
            var nestLimit = 3;
            var msg = new StringBuilder(string.Format(isConcurrency ? NakedObjects.Resources.NakedObjects.ConcurrencyErrorMessage : NakedObjects.Resources.NakedObjects.UpdateErrorMessage, e.Message));
            while (e.InnerException != null && nestLimit-- > 0) {
                msg.AppendLine().AppendLine(isConcurrency ? NakedObjects.Resources.NakedObjects.ConcurrencyException : NakedObjects.Resources.NakedObjects.DataUpdateException).Append(e.InnerException.Message);
                e = e.InnerException;
            }

            return msg.ToString();
        }

        private static void InjectParentIntoChild(object parent, object child) =>
            child.GetType().GetProperties().SingleOrDefault(p => p.CanWrite &&
                                                                 p.PropertyType.IsInstanceOfType(parent) &&
                                                                 p.GetCustomAttribute<RootAttribute>() != null)?.SetValue(child, parent, null);

        internal void CheckProxies(object objectToCheck) {
            var objectType = objectToCheck.GetType();
            if (!EnforceProxies || NakedObjects.TypeUtils.IsSystem(objectType) || NakedObjects.TypeUtils.IsMicrosoft(objectType)) {
                // may be using types provided by System or Microsoft (eg Authentication User). 
                // No point enforcing proxying on them. 
                return;
            }

            var adapter = getAdapterFor(objectToCheck);
            var isTransientObject = adapter?.Oid != null && adapter.Oid.IsTransient;
            var explanation = isTransientObject ? NakedObjects.Resources.NakedObjects.ProxyExplanationTransient : NakedObjects.Resources.NakedObjects.ProxyExplanation;
            var msg = "";

            if (!NakedObjects.TypeUtils.IsEntityProxy(objectToCheck.GetType())) {
                msg = string.Format(NakedObjects.Resources.NakedObjects.NoProxyMessage, objectToCheck.GetType(), explanation);
            }

            if (!(objectToCheck is IEntityWithChangeTracker)) {
                msg = string.Format(NakedObjects.Resources.NakedObjects.NoChangeTrackerMessage, objectToCheck.GetType(), explanation);
            }

            if (!string.IsNullOrEmpty(msg)) {
                throw new NakedObjectSystemException(msg);
            }
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

        private void ValidateIfRequired(INakedObjectAdapter adapter) {
            if (adapter.ResolveState.IsPersistent()) {
                if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                    var state = adapter.ValidToPersist();
                    if (state != null) {
                        throw new PersistFailedException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state)));
                    }
                }
            }
        }

        private void SavingChangesHandler(object sender, EventArgs e) {
            var changedObjects = ObjectContextUtils.GetChangedObjectsInContext((ObjectContext) sender);
            var adaptedObjects = changedObjects.Where(o => NakedObjects.TypeUtils.IsEntityProxy(o.GetType())).Select(domainObject => nakedObjectManager.CreateAdapter(domainObject, null, null)).ToArray();
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

                throw new NakedObjectDomainException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, typeNames)));
            }

            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        private static bool FieldIsPersisted(IAssociationSpec field) => !(field.ContainsFacet<INotPersistedFacet>() || field.ContainsFacet<IDisplayAsPropertyFacet>());

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && FieldIsPersisted(field)) {
                using var dbContext = new DbContext(GetContext(nakedObjectAdapter).WrappedObjectContext, false);
                // check this is an EF collection 
                try {
                    return dbContext.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                }
                catch (ArgumentException) {
                    // not an EF recognized collection 
                    logger.LogWarning($"Attempting to 'Count' a non-EF collection: {field.Id}");
                }
                catch (InvalidOperationException) {
                    // not an EF recognised entity 
                    logger.LogWarning($"Attempting to 'Count' a non attached entity: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }

        #region Delegates

        public delegate INakedObjectAdapter CreateAdapterDelegate(IOid oid, object domainObject);

        public delegate INakedObjectAdapter CreateAggregatedAdapterDelegate(INakedObjectAdapter nakedObjectAdapter, PropertyInfo property, object newDomainObject);

        public delegate INakedObjectAdapter GetAdapterForDelegate(object domainObject);

        public delegate void RemoveAdapterDelegate(INakedObjectAdapter nakedObjectAdapter);

        public delegate void ReplacePocoDelegate(INakedObjectAdapter nakedObjectAdapter, object newDomainObject);

        #endregion

        #region IObjectStore Members

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool parentIsGhost) {
            var proxiedType = adapter.Object.GetEntityProxiedType();

            if (contexts.All(c => c.Value.IsAlwaysUnrecognised(proxiedType))) {
                return;
            }

            if (EntityFrameworkKnowsType(proxiedType)) {
                foreach (var pi in GetContext(adapter).GetComplexMembers(proxiedType)) {
                    var complexObject = pi.GetValue(adapter.Object, null);
                    if (complexObject == null) {
                        throw new NakedObjectSystemException("Complex type members should never be null");
                    }

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

        public IList<(object original, object updated)> ExecuteAttachObjectCommandUpdate(IDetachedObjects objects) =>
            Execute(new EntityPersistUpdateDetachedObjectCommand(objects, this));

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
                logger.LogError($"Unexpected exception while applying changes {e.Message}");
                RollBackContext();
                throw;
            }
            finally {
                contexts.Values.ForEach(c => c.SaveOrUpdateComplete());
            }
        }

        public void Execute(IPersistenceCommand[] commands) => ExecuteCommands(commands);

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = NakedObjects.TypeUtils.GetType(spec.FullName);
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
                    throw new NakedObjectSystemException(logger.LogAndReturn($"Unexpected oid type: {oid.GetType()}"));
            }
        }

        public bool IsInitialized => IsInitializedCheck();

        public string Name => "Entity Object Store";

        public void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field) => field.GetNakedObject(nakedObjectAdapter);

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {
            var type = NakedObjects.TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
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

            if (!NakedObjects.TypeUtils.IsEntityProxy(entityType)) {
                var currentContext = GetContext(entityType);

                var propertynames = currentContext.GetNavigationMembers(entityType).Select(x => x.Name);
                var objectSet = currentContext.GetObjectSet(entityType);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in propertynames) {
                    objectSet = objectSet.Invoke<ObjectQuery>("Include", name);
                }

                var idmembers = currentContext.GetIdMembers(entityType);
                var keyValues = ((IEntityOid) nakedObjectAdapter.Oid).Key;

                if (idmembers.Length != keyValues.Length) {
                    throw new NakedObjectSystemException("Member and value counts must match");
                }

                var memberValueMap = ObjectContextUtils.MemberValueMap(idmembers, keyValues);

                var query = memberValueMap.Aggregate(string.Empty, (s, kvp) => string.Format("{0}it.{1}=@{1}", s.Length == 0 ? s : $"{s} and ", kvp.Key));
                var parms = memberValueMap.Select(kvp => new ObjectParameter(kvp.Key, kvp.Value)).ToArray();

                ObjectContextUtils.First(objectSet.Invoke<IQueryable>("Where", query, parms));
            }

            EndResolving(nakedObjectAdapter);
            ResolveChildCollections(nakedObjectAdapter);
        }

        public void StartTransaction() {
            // do nothing 
        }

        private IQueryable<T> EagerLoad<T>(LocalContext context, Type entityType, IQueryable queryable) => (IQueryable<T>) queryable.AsNoTracking();

        public IQueryable<T> GetInstances<T>(bool tracked = true) where T : class {
            var context = GetContext(typeof(T));
            var queryable = (IQueryable<T>) context.GetQueryableOfDerivedType<T>();
            return tracked ? queryable : EagerLoad<T>(context, typeof(T), queryable);
        }

        public IQueryable GetInstances(Type type) => (IQueryable) GetContext(type).GetQueryableOfDerivedType(type);

        public object CreateInstance(Type type) {
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (type.IsAbstract) {
                throw new ModelException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.CannotCreateAbstract, type)));
            }

            var domainObject = Activator.CreateInstance(type);
            injector.InjectInto(domainObject);

            if (EntityFrameworkKnowsType(type)) {
                foreach (var pi in GetContext(domainObject).GetComplexMembers(domainObject.GetType())) {
                    var complexObject = pi.GetValue(domainObject, null);
                    if (complexObject == null) {
                        throw new NakedObjectSystemException("Complex type members should never be null");
                    }

                    injector.InjectInto(complexObject);
                }
            }

            return domainObject;
        }

        public void Reload(INakedObjectAdapter nakedObjectAdapter) => Refresh(nakedObjectAdapter);

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class => (T) CreateInstance(typeof(T));

        public PropertyInfo[] GetKeys(Type type) => GetContext(NakedObjects.TypeUtils.GetProxiedType(type)).GetIdMembers(NakedObjects.TypeUtils.GetProxiedType(type));

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() == null) {
                nakedObjectAdapter.Updating();
                GetContext(nakedObjectAdapter.Object.GetType()).WrappedObjectContext.Refresh(RefreshMode.StoreWins, nakedObjectAdapter.Object);
                nakedObjectAdapter.Updated();
            }
        }

        internal static bool EmptyKey(object key) =>
            key switch {
                // todo for all null keys
                string s => string.IsNullOrEmpty(s),
                int i => i == 0,
                null => true,
                _ => false
            };

        public IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects) {
            FunctionalPostSave = objects.PostSaveFunction;
            return SetFunctionalProxyMap(ExecuteAttachObjectCommandUpdate(objects));
        }

        public bool HasChanges() => contexts.Values.Any(c => c.HasChanges());

        public T ValidateProxy<T>(T toCheck) where T : class {
            var toCheckType = toCheck.GetType();
            if (!NakedObjects.TypeUtils.IsEntityProxy(toCheckType)) {
                var context = GetContext(toCheck);
                if (context is null || context.GetReferenceMembers(toCheckType).Any() || context.GetCollectionMembers(toCheckType).Any()) {
                    throw new PersistFailedException($"{toCheck}  type {toCheckType} is not proxy but has reference members or is unknown to EF");
                }
            }

            return toCheck;
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

        public static void SetProxyingAndDeferredLoading(LocalContext context, bool newValue) {
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = newValue;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = newValue;
        }

        // ReSharper disable once UnusedMember.Global
        // used in F#code
        public void SetProxyingAndDeferredLoading(bool newValue) => contexts.Values.ForEach(c => SetProxyingAndDeferredLoading(c, newValue));

        #endregion
    }
}