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
using NakedFramework.Core.Error;
using NakedFramework.Core.Persist;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EF6.Configuration;
using NakedFramework.Persistor.EF6.Util;

[assembly: InternalsVisibleTo("NakedFramework.Persistor.Entity.Test")]

namespace NakedFramework.Persistor.EF6.Component {
    public sealed class EF6ObjectStore : IObjectStore, IDisposable {
        private readonly Func<object, INakedObjectAdapter> getAdapterFor;

        internal readonly ILogger<EF6ObjectStore> Logger;
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly DatabaseOidGenerator oidGenerator;
        private readonly ISession session;
        private IDictionary<EF6ContextConfiguration, EF6LocalContext> contexts = new Dictionary<EF6ContextConfiguration, EF6LocalContext>();
        internal Func<IOid, object, INakedObjectAdapter> CreateAdapter;
        internal Func<INakedObjectAdapter, PropertyInfo, object, INakedObjectAdapter> CreateAggregatedAdapter;
        private Func<IDictionary<object, object>, bool> functionalPostSave = _ => false;
        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();
        internal Action<INakedObjectAdapter> HandleLoaded;
        private IDomainObjectInjector injector;
        private Func<Type, ITypeSpec> loadSpecification;
        internal Action<INakedObjectAdapter> RemoveAdapter;
        internal Action<INakedObjectAdapter, object> ReplacePoco;
        private Action<object, EventArgs> savingChangesHandler;

        internal EF6ObjectStore(IMetamodelManager metamodelManager, ISession session, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager, ILogger<EF6ObjectStore> logger) {
            this.metamodelManager = metamodelManager ?? throw new InitialisationException($"{nameof(metamodelManager)} is null");
            this.session = session ?? throw new InitialisationException($"{nameof(session)} is null");
            this.injector = injector ?? throw new InitialisationException($"{nameof(injector)} is null");
            this.nakedObjectManager = nakedObjectManager ?? throw new InitialisationException($"{nameof(nakedObjectManager)} is null");
            Logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");

            getAdapterFor = domainObject => this.nakedObjectManager.GetAdapterFor(domainObject);
            CreateAdapter = (oid, domainObject) => this.nakedObjectManager.CreateAdapter(domainObject, oid, null);
            ReplacePoco = (nakedObject, newDomainObject) => this.nakedObjectManager.ReplacePoco(nakedObject, newDomainObject);
            RemoveAdapter = o => this.nakedObjectManager.RemoveAdapter(o);
            CreateAggregatedAdapter = (parent, property, obj) => this.nakedObjectManager.CreateAggregatedAdapter(parent, ((IObjectSpec) parent.Spec).GetProperty(property.Name).Id, obj);

            HandleLoaded = HandleLoadedDefault;
            savingChangesHandler = SavingChangesHandler;
            loadSpecification = metamodelManager.GetSpecification;
        }

        public EF6ObjectStore(ISession session, IEF6ObjectStoreConfiguration config, DatabaseOidGenerator oidGenerator, IMetamodelManager metamodel, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager, ILogger<EF6ObjectStore> logger)
            : this(metamodel, session, injector, nakedObjectManager, logger) {
            config.Validate();
            this.oidGenerator = oidGenerator;
            contexts = config.ContextConfiguration.ToDictionary<EF6ContextConfiguration, EF6ContextConfiguration, EF6LocalContext>(c => c, c => null);

            EnforceProxies = config.EnforceProxies;
            RequireExplicitAssociationOfTypes = config.RequireExplicitAssociationOfTypes;
            RollBackOnError = config.RollBackOnError;
            MaximumCommitCycles = config.MaximumCommitCycles;
            IsInitializedCheck = config.IsInitializedCheck;
            SetupContexts();
        }

        private static bool EnforceProxies { get; set; }
        private bool RollBackOnError { get; set; }
        private Func<bool> IsInitializedCheck { get; }

        // set is internally visible for testing
        internal static int MaximumCommitCycles { get; set; }
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

        private static IList<(object original, object updated)> Execute(IDetachedObjectCommand cmd) {
            try {
                return cmd.Execute();
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce);
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        private EF6LocalContext FindContext(Type type) =>
            contexts.Values.SingleOrDefault(c => c.GetIsOwned(type)) ??
            contexts.Values.Single(c => c.GetIsKnown(type));

        private EF6LocalContext GetContext(Type type) {
            try {
                return contexts.Count == 1 ? contexts.Values.Single() : FindContext(type);
            }
            catch (Exception e) {
                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityContextError, type.FullName)), e);
            }
        }

        internal EF6LocalContext GetContext(object domainObject) => GetContext(GetTypeToUse(domainObject));

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

        private EF6LocalContext GetContext(INakedObjectAdapter nakedObjectAdapter) => GetContext(nakedObjectAdapter.Object);

        private EF6LocalContext PrepareContextForNewTransaction(KeyValuePair<EF6ContextConfiguration, EF6LocalContext> kvp) {
            var entityContextConfiguration = kvp.Key;

            var context = CreateCodeOnlyContext(entityContextConfiguration);
            context.DefaultMergeOption = entityContextConfiguration.DefaultMergeOption;
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = true;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = true;
            context.WrappedObjectContext.SavingChanges += (o, e) => savingChangesHandler(o, e);
            context.WrappedObjectContext.ObjectStateManager.ObjectStateManagerChanged += (_, args) => {
                if (args.Action == CollectionChangeAction.Add) {
                    LoadObjectIntoNakedObjectsFramework(args.Element, context);
                }
            };

            entityContextConfiguration.CustomConfig(context.WrappedObjectContext);

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

        private void RollBackContext(EF6LocalContext context) {
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

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter, EF6LocalContext context) {
            var resolveEvent = !nakedObjectAdapter.ResolveState.IsTransient() &&
                               !context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled
                ? Events.StartPartResolvingEvent
                : Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.Handle(nakedObjectAdapter.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);

        private static void Resolve(INakedObjectAdapter nakedObjectAdapter, EF6LocalContext context) {
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
                case DataUpdateException:
                    throw new DataUpdateException(newMessage, exception);
                default:
                    // should never get here - just rethrow 
                    Logger.LogError($"Unexpected exception {exception}");
                    throw exception;
            }
        }

        private static void SaveChanges(EF6LocalContext context) => context.WrappedObjectContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);

        private void Save() {
            contexts.Values.ForEach(SaveChanges);
            contexts.Values.ForEach(c => c.WrappedObjectContext.AcceptAllChanges());
        }

        private IList<(object original, object updated)> SetFunctionalProxyMap(IList<(object original, object updated)> updatedTuples) {
            functionalProxyMap = updatedTuples.ToDictionary(t => t.original, t => t.updated);
            return updatedTuples;
        }

        private bool PostSave() {
            var changes = functionalPostSave(functionalProxyMap);

            // two separate loops as PostSave may have side-affects in previously processed contexts
            contexts.Values.ForEach(c => c.PostSave(this));
            return contexts.Values.Aggregate(false, (current, c) => current || c.HasChanges());
        }

        private void PostSaveWrapUp() => contexts.Values.ForEach(c => c.PostSaveWrapUp(this));

        private void PreSave() => contexts.Values.ForEach(c => c.PreSave());

        private INakedObjectAdapter GetSourceNakedObject(UpdateException oce) {
            var trigger = oce.StateEntries.Where(e => !e.IsRelationship).Select(e => e.Entity).SingleOrDefault();
            return CreateAdapter(null, trigger);
        }

        private EF6LocalContext CreateCodeOnlyContext(EF6ContextConfiguration codeOnlyConfig) {
            try {
                return new EF6LocalContext(codeOnlyConfig, session, this);
            }
            catch (Exception e) {
                var originalMsg = e.Message;
                throw new InitialisationException(Logger.LogAndReturn($"{NakedObjects.Resources.NakedObjects.StartPersistorErrorCodeFirst}: {originalMsg}"), e);
            }
        }

        internal void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
            var oid = (IDatabaseOid) nakedObjectAdapter.Oid;
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

        public object GetObjectByKey(IDatabaseOid eoid, Type type) {
            var context = GetContext(type);
            var memberValueMap = GetMemberValueMap(type, eoid, out var entitySetName);
            var oq = context.CreateQuery(type, entitySetName);

            foreach (var (key, value) in memberValueMap) {
                var query = string.Format("it.{0}=@{0}", key);
                oq = oq.Invoke<object>("Where", query, new[] {new ObjectParameter(key, value)});
            }

            context.GetNavigationMembers(type).Where(m => !CollectionUtils.IsCollection(m.PropertyType)).ForEach(pi => oq = oq.Invoke<object>("Include", pi.Name));
            return EF6Helpers.First(oq.Invoke<IEnumerable>("Execute", context.DefaultMergeOption));
        }

        private IDictionary<string, object> GetMemberValueMap(Type type, IDatabaseOid eoid, out string entitySetName) {
            var context = GetContext(type);
            var set = context.GetObjectSet(type).GetProperty<EntitySet>("EntitySet");
            entitySetName = $"{set.EntityContainer.Name}.{set.Name}";
            var idmembers = context.GetIdMembers(type);
            var keyValues = eoid.Key;
            return EF6Helpers.MemberValueMap(idmembers, keyValues);
        }

        public object GetObjectByKey(IDatabaseOid eoid, IObjectSpec hint) => GetObjectByKey(eoid, TypeUtils.GetType(hint.FullName));

        public bool EntityFrameworkKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    return FindContext(type) != null;
                }
            }
            catch (Exception e) {
                // ignore all 
                Logger.LogWarning($"Ignoring exception {e.Message}");
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

        internal void CheckProxies(object objectToCheck) {
            var objectType = objectToCheck.GetType();
            if (!EnforceProxies || FasterTypeUtils.IsSystem(objectType)) {
                // may be using types provided by System or Microsoft (eg Authentication User). 
                // No point enforcing proxying on them. 
                return;
            }

            var adapter = getAdapterFor(objectToCheck);
            var isTransientObject = adapter?.Oid != null && adapter.Oid.IsTransient;
            var explanation = isTransientObject ? NakedObjects.Resources.NakedObjects.ProxyExplanationTransient : NakedObjects.Resources.NakedObjects.ProxyExplanation;
            var msg = "";

            if (!FasterTypeUtils.IsEF6Proxy(objectToCheck.GetType())) {
                msg = string.Format(NakedObjects.Resources.NakedObjects.NoProxyMessage, objectToCheck.GetType(), explanation);
            }

            if (objectToCheck is not IEntityWithChangeTracker) {
                msg = string.Format(NakedObjects.Resources.NakedObjects.NoChangeTrackerMessage, objectToCheck.GetType(), explanation);
            }

            if (!string.IsNullOrEmpty(msg)) {
                throw new NakedObjectSystemException(msg);
            }
        }

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, EF6LocalContext context) {
            CheckProxies(domainObject);
            var oid = oidGenerator.CreateOid(EF6Helpers.GetEF6ProxiedTypeName(domainObject), context.GetKey(domainObject));
            var nakedObjectAdapter = CreateAdapter(oid, domainObject);
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
                        throw new PersistFailedException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state)));
                    }
                }
            }
        }

        private void SavingChangesHandler(object sender, EventArgs e) {
            var changedObjects = EF6Helpers.GetChangedObjectsInContext((ObjectContext) sender);
            var adaptedObjects = changedObjects.Where(o => FasterTypeUtils.IsEF6Proxy(o.GetType())).Select(domainObject => nakedObjectManager.CreateAdapter(domainObject, null, null)).ToArray();
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
                var typeNames = contexts.Values.SelectMany(c => c.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified).Where(o => o.Entity != null).Select(o => o.Entity.GetEF6ProxiedType().FullName)).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, typeNames)));
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
                    Logger.LogWarning($"Attempting to 'Count' a non-EF collection: {field.Id} annotate with 'NotPersisted' to avoid this warning");
                }
                catch (InvalidOperationException) {
                    // not an EF recognised entity 
                    Logger.LogWarning($"Attempting to 'Count' a non attached entity: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }

        public bool IsNotPersisted(object owner, PropertyInfo pi) {
            if (metamodelManager.GetSpecification(owner.GetEF6ProxiedType()) is IObjectSpec objectSpec) {
                return objectSpec.Properties.SingleOrDefault(p => p.Id == pi.Name)?.ContainsFacet<INotPersistedFacet>() == true;
            }

            return false;
        }

        #region IObjectStore Members

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool parentIsGhost) {
            var proxiedType = adapter.Object.GetEF6ProxiedType();

            if (contexts.All(c => c.Value.IsAlwaysUnrecognised(proxiedType))) {
                return;
            }

            if (EntityFrameworkKnowsType(proxiedType)) {
                foreach (var pi in GetContext(adapter).GetComplexMembers(proxiedType)) {
                    var complexObject = pi.GetValue(adapter.Object, null);
                    if (complexObject == null) {
                        throw new NakedObjectSystemException("Complex type members should never be null");
                    }

                    injector.InjectParentIntoChild(adapter.Object, complexObject);
                    injector.InjectInto(complexObject);
                    CreateAggregatedAdapter(adapter, pi, complexObject);
                }
            }
        }

        public void AbortTransaction() => RollBackContext();

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EF6CreateObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter), this));

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EF6DestroyObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EF6SaveObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        private IList<(object original, object updated)> ExecuteDetachedObjectCommand(IDetachedObjects objects) =>
            Execute(new EF6DetachedObjectCommand(objects, this));

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
                Logger.LogError($"Unexpected exception while applying changes {e.Message}");
                RollBackContext();
                throw;
            }
            finally {
                contexts.Values.ForEach(c => c.SaveOrUpdateComplete());
            }
        }

        public object Resolve(object domainObject) {
            var context = GetContext(domainObject);
            var entityType = domainObject.GetEF6ProxiedType();
            var references = context.GetReferenceMembers(entityType);
            var collection = context.GetCollectionMembers(entityType);

            foreach (var pi in references.Union(collection)) {
                pi.GetValue(domainObject, null);
            }

            return domainObject;
        }

        public void Execute(IPersistenceCommand[] commands) => ExecuteCommands(commands);

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = TypeUtils.GetType(spec.FullName);
            return GetContext(type).GetObjectSet(type);
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
                case IDatabaseOid eoid: {
                    var adapter = CreateAdapter(eoid, GetObjectByKey(eoid, hint));
                    adapter.UpdateVersion(session, nakedObjectManager);
                    return adapter;
                }
                default:
                    throw new NakedObjectSystemException(Logger.LogAndReturn($"Unexpected oid type: {oid.GetType()}"));
            }
        }

        public bool IsInitialized => IsInitializedCheck();

        public string Name => "EF6 Object Store";

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

            if (!FasterTypeUtils.IsEF6Proxy(entityType)) {
                var currentContext = GetContext(entityType);

                var propertynames = currentContext.GetNavigationMembers(entityType).Select(x => x.Name);
                var objectSet = currentContext.GetObjectSet(entityType);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var name in propertynames) {
                    objectSet = objectSet.Invoke<ObjectQuery>("Include", name);
                }

                var idmembers = currentContext.GetIdMembers(entityType);
                var keyValues = ((IDatabaseOid) nakedObjectAdapter.Oid).Key;

                if (idmembers.Length != keyValues.Length) {
                    throw new NakedObjectSystemException("Member and value counts must match");
                }

                var memberValueMap = EF6Helpers.MemberValueMap(idmembers, keyValues);

                var query = memberValueMap.Aggregate(string.Empty, (s, kvp) => string.Format("{0}it.{1}=@{1}", s.Length == 0 ? s : $"{s} and ", kvp.Key));
                var parms = memberValueMap.Select(kvp => new ObjectParameter(kvp.Key, kvp.Value)).ToArray();

                EF6Helpers.First(objectSet.Invoke<IQueryable>("Where", query, parms));
            }

            EndResolving(nakedObjectAdapter);
            ResolveChildCollections(nakedObjectAdapter);
        }

        public void StartTransaction() {
            // do nothing 
        }

        private static IQueryable<T> EagerLoad<T>(EF6LocalContext context, Type entityType, IQueryable queryable) => (IQueryable<T>) queryable.AsNoTracking();

        public IQueryable<T> GetInstances<T>() where T : class {
            var context = GetContext(typeof(T));
            return (IQueryable<T>) context.GetQueryableOfDerivedType<T>();
        }

        public IQueryable GetInstances(Type type) => (IQueryable) GetContext(type).GetQueryableOfDerivedType(type);

        public object CreateInstance(Type type) {
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }

            if (type.IsAbstract) {
                throw new ModelException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.CannotCreateAbstract, type)));
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

        public PropertyInfo[] GetKeys(Type type) => GetContext(type.GetProxiedType()).GetIdMembers(type.GetProxiedType());

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() == null) {
                nakedObjectAdapter.Updating();
                GetContext(nakedObjectAdapter.Object.GetType()).WrappedObjectContext.Refresh(RefreshMode.StoreWins, nakedObjectAdapter.Object);
                nakedObjectAdapter.Updated();
            }
        }

        public IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects) {
            functionalPostSave = objects.PostSaveFunction;
            return SetFunctionalProxyMap(ExecuteDetachedObjectCommand(objects));
        }

        public bool HasChanges() => contexts.Values.Any(c => c.HasChanges());

        public T ValidateProxy<T>(T toCheck) where T : class {
            var toCheckType = toCheck.GetType();
            if (!FasterTypeUtils.IsEF6Proxy(toCheckType)) {
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
                                    Func<IOid, object, INakedObjectAdapter> createAdapter,
                                    Action<INakedObjectAdapter, object> replacePoco,
                                    Action<INakedObjectAdapter> removeAdapter,
                                    Func<INakedObjectAdapter, PropertyInfo, object, INakedObjectAdapter> createAggregatedAdapter,
                                    Action<INakedObjectAdapter> handleLoadedTest,
                                    Action<object, EventArgs> savingChangesHandler,
                                    Func<Type, IObjectSpec> loadSpecificationHandler) {
            injector = domainObjectInjector;
            CreateAdapter = createAdapter;
            ReplacePoco = replacePoco;
            RemoveAdapter = removeAdapter;
            CreateAggregatedAdapter = createAggregatedAdapter;

            this.savingChangesHandler = savingChangesHandler;
            HandleLoaded = handleLoadedTest;
            EnforceProxies = false;
            RollBackOnError = true;
            loadSpecification = loadSpecificationHandler;
        }

        public static void SetProxyingAndDeferredLoading(EF6LocalContext context, bool newValue) {
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = newValue;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = newValue;
        }

        // ReSharper disable once UnusedMember.Global
        // used in F#code
        public void SetProxyingAndDeferredLoading(bool newValue) => contexts.Values.ForEach(c => SetProxyingAndDeferredLoading(c, newValue));

        #endregion
    }
}