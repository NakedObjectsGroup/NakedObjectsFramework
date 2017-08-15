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
using System.Text;
using System.Transactions;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Persistor.Entity.Util;
using NakedObjects.Util;

namespace NakedObjects.Persistor.Entity.Component {
    public sealed class EntityObjectStore : IObjectStore, IDisposable {
        #region Delegates

        public delegate INakedObjectAdapter CreateAdapterDelegate(IOid oid, object domainObject);

        public delegate INakedObjectAdapter CreateAggregatedAdapterDelegate(INakedObjectAdapter nakedObjectAdapter, PropertyInfo property, object newDomainObject);

        public delegate void NotifyUiDelegate(INakedObjectAdapter changedObjectAdapter);

        public delegate void RemoveAdapterDelegate(INakedObjectAdapter nakedObjectAdapter);

        public delegate void ReplacePocoDelegate(INakedObjectAdapter nakedObjectAdapter, object newDomainObject);

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityObjectStore));
        private CreateAdapterDelegate createAdapter;
        private CreateAggregatedAdapterDelegate createAggregatedAdapter;
        private RemoveAdapterDelegate removeAdapter;
        private ReplacePocoDelegate replacePoco;
        private EventHandler savingChangesHandlerDelegate;
        private Action<INakedObjectAdapter> handleLoaded;
        private Func<Type, ITypeSpec> loadSpecification;
        private readonly IMetamodelManager metamodelManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly EntityOidGenerator oidGenerator;
        private readonly ISession session;
        private IDictionary<EntityContextConfiguration, LocalContext> contexts = new Dictionary<EntityContextConfiguration, LocalContext>();
        private IDomainObjectInjector injector;

        internal EntityObjectStore(IMetamodelManager metamodelManager, ISession session, IDomainObjectInjector injector, INakedObjectManager nakedObjectManager) {
            this.metamodelManager = metamodelManager;
            this.session = session;
            this.injector = injector;
            this.nakedObjectManager = nakedObjectManager;

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
            contexts = config.ContextConfiguration.ToDictionary<EntityContextConfiguration, EntityContextConfiguration, LocalContext>(c => c, c => null);

            EnforceProxies = config.EnforceProxies;
            RequireExplicitAssociationOfTypes = config.RequireExplicitAssociationOfTypes;
            RollBackOnError = config.RollBackOnError;
            MaximumCommitCycles = config.MaximumCommitCycles;
            IsInitializedCheck = config.IsInitializedCheck;
            Reset();
        }

        private static bool EnforceProxies { get; set; }
        private bool RollBackOnError { get; set; }
        // set is internally visible for testing
        internal static int MaximumCommitCycles { get; set; }
        private Func<bool> IsInitializedCheck { get; set; }
        internal static bool RequireExplicitAssociationOfTypes { get; private set; }

        #region for testing only

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

        public void SetProxyingAndDeferredLoading(bool newValue) {
            contexts.Values.ForEach(c => SetProxyingAndDeferredLoading(c, newValue));
        }

        #endregion

        #region IObjectStore Members

        public void LoadComplexTypes(INakedObjectAdapter adapter, bool parentIsGhost) {
            if (EntityFrameworkKnowsType(adapter.Object.GetEntityProxiedType())) {
                foreach (PropertyInfo pi in GetContext(adapter).GetComplexMembers(adapter.Object.GetEntityProxiedType())) {
                    object complexObject = pi.GetValue(adapter.Object, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    InjectParentIntoChild(adapter.Object, complexObject);
                    injector.InjectInto(complexObject);
                    createAggregatedAdapter(adapter, pi, complexObject);
                }
            }
        }

        public void AbortTransaction() {
            Log.Debug("AbortTransaction");
            RollBackContext();
        }

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            Log.DebugFormat("CreateCreateObjectCommand : {0}", nakedObjectAdapter);
            try {
                ExecuteCommand(new EntityCreateObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter), this));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = nakedObjectAdapter};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            Log.DebugFormat("CreateDestroyObjectCommand : {0}", nakedObjectAdapter);
            try {
                ExecuteCommand(new EntityDestroyObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = nakedObjectAdapter};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            Log.DebugFormat("CreateSaveObjectCommand : {0}", nakedObjectAdapter);
            try {
                ExecuteCommand(new EntitySaveObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObjectAdapter = nakedObjectAdapter};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public void EndTransaction() {
            Log.Debug("EndTransaction");

            try {
                using (TransactionScope transaction = CreateTransactionScope()) {
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
                Log.ErrorFormat("Unexpected exception while applying changes {0}", e.Message);
                RollBackContext();
                throw;
            }
            finally {
                contexts.Values.ForEach(c => c.SaveOrUpdateComplete());
            }
        }

        public void Execute(IPersistenceCommand[] commands) {
            Log.DebugFormat("Execute {0} commands", commands.Length);
            ExecuteCommands(commands);
        }

        public IQueryable GetInstances(IObjectSpec spec) {
            Log.DebugFormat("GetInstances for: {0}", spec);
            Type type = TypeUtils.GetType(spec.FullName);
            return GetContext(type).GetObjectSet(type);
        }

        public INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint) {
            Log.DebugFormat("GetObject oid: {0} hint: {1}", oid, hint);

            var aggregateOid = oid as IAggregateOid;
            if (aggregateOid != null) {
                var parentOid = (IEntityOid) aggregateOid.ParentOid;
                string parentType = parentOid.TypeName;
                var parentSpec = (IObjectSpec) metamodelManager.GetSpecification(parentType);
                INakedObjectAdapter parent = createAdapter(parentOid, GetObjectByKey(parentOid, parentSpec));

                return parentSpec.GetProperty(aggregateOid.FieldName).GetNakedObject(parent);
            }

            var eoid = oid as IEntityOid;
            if (eoid != null) {
                INakedObjectAdapter adapter = createAdapter(eoid, GetObjectByKey(eoid, hint));
                adapter.UpdateVersion(session, nakedObjectManager);
                return adapter;
            }
            throw new NakedObjectSystemException("Unexpected oid type: " + oid.GetType());
        }

        public bool IsInitialized {
            get { return IsInitializedCheck(); }
            set { }
        }

        public string Name {
            get { return "Entity Object Store"; }
        }

        public void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field) {
            Log.DebugFormat("ResolveField nakedobject: {0} field: {1}", nakedObjectAdapter, field);
            field.GetNakedObject(nakedObjectAdapter);
        }

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {
            Type type = TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
            MethodInfo countMethod = GetType().GetMethod("Count").GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int) countMethod.Invoke(this, new object[] {nakedObjectAdapter, associationSpec, nakedObjectManager});
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {
            IOid eoid = oidGenerator.CreateOid(type.FullName, keys);
            var hint = (IObjectSpec) loadSpecification(type);
            return GetObject(eoid, hint);
        }

        public void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter) {
            Log.DebugFormat("ResolveImmediately nakedobject: {0}", nakedObjectAdapter);
            // eagerly load object        
            nakedObjectAdapter.ResolveState.Handle(Events.StartResolvingEvent);
            // only if not proxied
            Type entityType = nakedObjectAdapter.Object.GetType();

            if (!TypeUtils.IsEntityProxy(entityType)) {
                LocalContext currentContext = GetContext(entityType);

                IEnumerable<string> propertynames = currentContext.GetNavigationMembers(entityType).Select(x => x.Name);
                dynamic objectSet = currentContext.GetObjectSet(entityType);

                // can't use LINQ with dynamic
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (string name in propertynames) {
                    objectSet = objectSet.Include(name);
                }

                IList<PropertyInfo> idmembers = currentContext.GetIdMembers(entityType);
                IList<object> keyValues = ((IEntityOid) nakedObjectAdapter.Oid).Key;
                Assert.AssertEquals("Member and value counts must match", idmembers.Count, keyValues.Count);
                IEnumerator<PropertyInfo> idIter = idmembers.GetEnumerator();
                List<KeyValuePair<string, object>> memberValueMap = keyValues.ToDictionary(x => {
                    idIter.MoveNext();
                    return idIter.Current.Name;
                }).ToList();

                string query = memberValueMap.Aggregate(string.Empty, (s, kvp) => string.Format("{0}it.{1}=@{1}", s.Length == 0 ? s : s + " and ", kvp.Key));
                ObjectParameter[] parms = memberValueMap.Select(kvp => new ObjectParameter(kvp.Key, kvp.Value)).ToArray();

                First(objectSet.Where(query, parms));
            }
            EndResolving(nakedObjectAdapter);
            ResolveChildCollections(nakedObjectAdapter);
        }

        public void StartTransaction() {
            Log.Debug("StartTransaction");
            // do nothing 
        }

        public IQueryable<T> GetInstances<T>() where T : class {
            Log.Debug("GetInstances<T> of: " + typeof (T));
            return GetContext(typeof (T)).GetQueryableOfDerivedType<T>();
        }

        public IQueryable GetInstances(Type type) {
            Log.Debug("GetInstances of: " + type);
            return GetContext(type).GetQueryableOfDerivedType(type);
        }

        public object CreateInstance(Type type) {
            Log.Debug("CreateInstance of: " + type);
            if (type.IsArray) {
                return Array.CreateInstance(type.GetElementType(), 0);
            }
            if (type.IsAbstract) {
                throw new ModelException(string.Format(Resources.NakedObjects.CannotCreateAbstract, type));
            }
            object domainObject = Activator.CreateInstance(type);
            injector.InjectInto(domainObject);

            if (EntityFrameworkKnowsType(type)) {
                foreach (PropertyInfo pi in GetContext(domainObject).GetComplexMembers(domainObject.GetType())) {
                    object complexObject = pi.GetValue(domainObject, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    injector.InjectInto(complexObject);
                }
            }
            return domainObject;
        }

        public void Reload(INakedObjectAdapter nakedObjectAdapter) {
            Log.Debug("Reload nakedobject: " + nakedObjectAdapter);
            Refresh(nakedObjectAdapter);
        }

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class {
            Log.Debug("CreateInstance<T> of: " + typeof (T));
            return (T) CreateInstance(typeof (T));
        }

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            return GetContext(type.GetProxiedType()).GetIdMembers(type.GetProxiedType());
        }

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            Log.Debug("Refresh nakedobject: " + nakedObjectAdapter);
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() == null) {
                nakedObjectAdapter.Updating();
                GetContext(nakedObjectAdapter.Object.GetType()).WrappedObjectContext.Refresh(RefreshMode.StoreWins, nakedObjectAdapter.Object);
                nakedObjectAdapter.Updated();
            }
        }

        #endregion

        private LocalContext FindContext(Type type) {
            return contexts.Values.SingleOrDefault(c => c.GetIsOwned(type)) ?? contexts.Values.Single(c => c.GetIsKnown(type));
        }

        private LocalContext GetContext(Type type) {
            try {
                return contexts.Count == 1 ? contexts.Values.Single() : FindContext(type);
            }
            catch (Exception e) {
                throw new NakedObjectDomainException(string.Format(Resources.NakedObjects.EntityContextError, type.FullName), e);
            }
        }

        private LocalContext GetContext(object domainObject) {
            return GetContext(GetTypeToUse(domainObject));
        }

        private static Type GetTypeToUse(object domainObject) {
            if (domainObject == null) {
                throw new NakedObjectSystemException("Could not find Entity Framework context for null object");
            }
            Type objectType = domainObject.GetType();
            if (CollectionUtils.IsGenericEnumerableOfRefType(objectType)) {
                return objectType.GetGenericArguments().First();
            }
            if (objectType.HasElementType) {
                return objectType.GetElementType();
            }
            if (CollectionUtils.IsCollection(objectType)) {
                return GetTypeToUse(((IEnumerable) domainObject).Cast<object>().FirstOrDefault());
            }

            return objectType;
        }

        private LocalContext GetContext(INakedObjectAdapter nakedObjectAdapter) {
            return GetContext(nakedObjectAdapter.Object);
        }

        private LocalContext ResetContext(KeyValuePair<EntityContextConfiguration, LocalContext> kvp) {
            LocalContext context = kvp.Value;
            EntityContextConfiguration config = kvp.Key;

            CodeFirstEntityContextConfiguration codeFirstEntityContextConfiguration = config as CodeFirstEntityContextConfiguration;
            context = codeFirstEntityContextConfiguration != null ? ResetCodeOnlyContext(codeFirstEntityContextConfiguration) : ResetPocoContext(config as PocoEntityContextConfiguration);
            context.DefaultMergeOption = config.DefaultMergeOption;
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = true;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = true;
            context.WrappedObjectContext.SavingChanges += savingChangesHandlerDelegate;
            context.WrappedObjectContext.ObjectStateManager.ObjectStateManagerChanged += (obj, args) => {
                if (args.Action == CollectionChangeAction.Add) {
                    LoadObject(args.Element, context);
                }
            };

            config.CustomConfig(context.WrappedObjectContext);

            context.Manager = nakedObjectManager;
            return context;
        }

        private void ResolveChildCollections(INakedObjectAdapter nakedObjectAdapter) {
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            if (spec != null) {
                // testing check 
                foreach (IOneToManyAssociationSpec assoc in spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted)) {
                    INakedObjectAdapter adapter = assoc.GetNakedObject(nakedObjectAdapter);
                    if (adapter.ResolveState.IsGhost()) {
                        StartResolving(adapter, GetContext(nakedObjectAdapter));
                        EndResolving(adapter);
                    }
                }
            }
        }

        private  void RollBackContext(LocalContext context) {
            if (RollBackOnError) {
                ObjectContext wContext = context.WrappedObjectContext;
                wContext.DetectChanges();
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified).ForEach(ose => context.WrappedObjectContext.Refresh(RefreshMode.StoreWins, ose.Entity));
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added).ForEach(ose => ose.ChangeState(EntityState.Detached));
                wContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Where(ose => ose.Entity != null).ForEach(ose => context.WrappedObjectContext.Refresh(RefreshMode.StoreWins, ose.Entity));
                wContext.AcceptAllChanges();
            }
        }

        private void RollBackContext() {
            contexts.Values.ForEach(RollBackContext);
        }

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter, LocalContext context) {
            IResolveEvent resolveEvent = (!nakedObjectAdapter.ResolveState.IsTransient() &&
                                          !context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled) ? Events.StartPartResolvingEvent :
                Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObjectAdapter nakedObjectAdapter) {
            nakedObjectAdapter.ResolveState.Handle(nakedObjectAdapter.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);
        }

        private static void HandleLoadedDefault(INakedObjectAdapter nakedObjectAdapter) {
            EndResolving(nakedObjectAdapter);
        }

        private void InvokeErrorFacet(Exception exception) {
            string newMessage = exception.Message;

            foreach (LocalContext context in contexts.Values) {
                if (context.CurrentSaveRootObjectAdapter != null) {
                    INakedObjectAdapter target = context.CurrentSaveRootObjectAdapter;
                    if (target.Spec != null) {
                        // can be null in tests
                        newMessage = target.Spec.GetFacet<IOnPersistingErrorCallbackFacet>().Invoke(target, exception);
                    }
                    break;
                }
                if (context.CurrentUpdateRootObjectAdapter != null) {
                    INakedObjectAdapter target = context.CurrentUpdateRootObjectAdapter;
                    if (target.Spec != null) {
                        // can be null in tests 
                        newMessage = target.Spec.GetFacet<IOnUpdatingErrorCallbackFacet>().Invoke(target, exception);
                    }
                    break;
                }
            }

            // Rollback after extracting info from context - rollback clears it all
            RollBackContext();

            var concurrencyException = exception as ConcurrencyException;
            if (concurrencyException != null) {
                throw new ConcurrencyException(newMessage, exception) {SourceNakedObjectAdapter = concurrencyException.SourceNakedObjectAdapter};
            }
            if (exception is DataUpdateException) {
                throw new DataUpdateException(newMessage, exception);
            }
            // should never get here - just rethrow 
            throw exception;
        }

        private static void SaveChanges(LocalContext context) {
            int saved = context.WrappedObjectContext.SaveChanges(SaveOptions.DetectChangesBeforeSave);
            Log.InfoFormat("Saved {0} changes", saved);
        }

        private void Save() {
            contexts.Values.ForEach(SaveChanges);
            contexts.Values.ForEach(c => c.WrappedObjectContext.AcceptAllChanges());
        }

        private bool PostSave() {
            // two separate loops as PostSave may have side-affects in previously processed contexts
            contexts.Values.ForEach(c => c.PostSave(this));
            return contexts.Values.Aggregate(false, (current, c) => current || c.HasChanges());
        }

        private void PostSaveWrapUp() {
            contexts.Values.ForEach(c => c.PostSaveWrapUp(this));
        }

        private void PreSave() {
            contexts.Values.ForEach(c => c.PreSave());
        }

        private  INakedObjectAdapter GetSourceNakedObject(OptimisticConcurrencyException oce) {
            object trigger = oce.StateEntries.Where(e => !e.IsRelationship).Select(e => e.Entity).SingleOrDefault();
            return createAdapter(null, trigger);
        }

        private LocalContext ResetPocoContext(PocoEntityContextConfiguration pocoConfig) {
            try {
                return new LocalContext(pocoConfig, session, this) {IsInitialized = true};
            }
            catch (Exception e) {
                string explain = string.Format(Resources.NakedObjects.StartPersistorErrorMessage, pocoConfig.ContextName);
                throw new InitialisationException(explain, e);
            }
        }

        private LocalContext ResetCodeOnlyContext(CodeFirstEntityContextConfiguration codeOnlyConfig) {
            try {
                return new LocalContext(codeOnlyConfig, session, this);
            }
            catch (Exception e) {
                throw new InitialisationException(Resources.NakedObjects.StartPersistorErrorCodeFirst, e);
            }
        }

        private void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
            var oid = (IEntityOid) nakedObjectAdapter.Oid;
            LocalContext context = GetContext(nakedObjectAdapter);
            oid.MakePersistentAndUpdateKey(context.GetKey(nakedObjectAdapter));

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {
                StartResolving(nakedObjectAdapter, context);
                EndResolving(nakedObjectAdapter);
            }
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            if (spec != null) {
                // testing check 
                foreach (IOneToManyAssociationSpec assoc in spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted)) {
                    INakedObjectAdapter adapter = assoc.GetNakedObject(nakedObjectAdapter);
                    if (adapter.ResolveState.IsGhost()) {
                        StartResolving(adapter, GetContext(adapter));
                        EndResolving(adapter);
                    }
                }
            }
            Log.DebugFormat("Added {0}", nakedObjectAdapter);
        }

        private void MarkAsLoaded(INakedObjectAdapter nakedObjectAdapter) {
            GetContext(nakedObjectAdapter).LoadedNakedObjects.Add(nakedObjectAdapter);
        }

        public object GetObjectByKey(IEntityOid eoid, Type type) {
            string entitySetName;
            LocalContext context = GetContext(type);
            List<KeyValuePair<string, object>> memberValueMap = GetMemberValueMap(type, eoid, out entitySetName);

            dynamic oq = context.CreateQuery(type, entitySetName);

            foreach (KeyValuePair<string, object> kvp in memberValueMap) {
                string query = string.Format("it.{0}=@{0}", kvp.Key);
                oq = oq.Where(query, new ObjectParameter(kvp.Key, kvp.Value));
            }
            context.GetNavigationMembers(type).Where(m => !CollectionUtils.IsCollection(m.PropertyType)).ForEach(pi => oq = oq.Include(pi.Name));
            return First((IEnumerable) oq.Execute(context.DefaultMergeOption));
        }

        private List<KeyValuePair<string, object>> GetMemberValueMap(Type type, IEntityOid eoid, out string entitySetName) {
            LocalContext context = GetContext(type);
            dynamic set = context.GetObjectSet(type).EntitySet;
            entitySetName = set.EntityContainer.Name + "." + set.Name;
            IList<PropertyInfo> idmembers = context.GetIdMembers(type);
            IList<object> keyValues = eoid.Key;
            Assert.AssertEquals("Member and value counts must match", idmembers.Count, keyValues.Count);
            IEnumerator<PropertyInfo> idIter = idmembers.GetEnumerator();
            return keyValues.ToDictionary(x => {
                idIter.MoveNext();
                return idIter.Current.Name;
            }).ToList();
        }

        public object GetObjectByKey(IEntityOid eoid, IObjectSpec hint) {
            Type type = TypeUtils.GetType(hint.FullName);
            return GetObjectByKey(eoid, type);
        }

        private static object First(IEnumerable enumerable) {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // unfortunately this cast doesn't work with entity linq
            // return queryable.Cast<object>().FirstOrDefault();
            foreach (object o in enumerable) {
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
                Log.InfoFormat("Ignoring exception {0}", e.Message);
            }

            return false;
        }

        private static string ConcatenateMessages(Exception e) {
            bool isConcurrency = e is OptimisticConcurrencyException;
            int nestLimit = 3;
            var msg = new StringBuilder(string.Format((isConcurrency ? Resources.NakedObjects.ConcurrencyErrorMessage : Resources.NakedObjects.UpdateErrorMessage), e.Message));
            while (e.InnerException != null && nestLimit-- > 0) {
                msg.AppendLine().AppendLine(isConcurrency ? Resources.NakedObjects.ConcurrencyException : Resources.NakedObjects.DataUpdateException).Append(e.InnerException.Message);
                e = e.InnerException;
            }
            return msg.ToString();
        }

        private static void InjectParentIntoChild(object parent, object child) {
            PropertyInfo property = child.GetType().GetProperties().SingleOrDefault(p => p.CanWrite &&
                                                                                         p.PropertyType.IsInstanceOfType(parent) &&
                                                                                         p.GetCustomAttribute<RootAttribute>() != null);
            if (property != null) {
                property.SetValue(child, parent, null);
            }
        }

        private static void CheckProxies(object objectToCheck) {
            Type objectType = objectToCheck.GetType();
            if (!EnforceProxies || TypeUtils.IsSystem(objectType) || TypeUtils.IsMicrosoft(objectType)) {
                // may be using types provided by System or Microsoft (eg Authentication User). 
                // No point enforcing proxying on them. 
                return;
            }
            Assert.AssertTrue(string.Format(Resources.NakedObjects.NoProxyMessage, objectToCheck.GetType(), Resources.NakedObjects.ProxyExplanation), TypeUtils.IsEntityProxy(objectToCheck.GetType()));
            Assert.AssertTrue(string.Format(Resources.NakedObjects.NoChangeTrackerMessage, objectToCheck.GetType(), Resources.NakedObjects.ProxyExplanation), objectToCheck is IEntityWithChangeTracker);
        }

        private void LoadObject(object domainObject, LocalContext context) {
            CheckProxies(domainObject);
            IOid oid = oidGenerator.CreateOid(EntityUtils.GetEntityProxiedTypeName(domainObject), context.GetKey(domainObject));
            INakedObjectAdapter nakedObjectAdapter = createAdapter(oid, domainObject);
            injector.InjectInto(nakedObjectAdapter.Object);
            LoadComplexTypes(nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsGhost());
            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsGhost()) {
                StartResolving(nakedObjectAdapter, context);
                MarkAsLoaded(nakedObjectAdapter);
            }
        }

        private static IEnumerable<object> GetRelationshipEnds(ObjectContext context, ObjectStateEntry /*RelationshipEntry*/ ose) {
            var key0 = (EntityKey) ose.GetType().GetProperty("Key0", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);
            var key1 = (EntityKey) ose.GetType().GetProperty("Key1", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(ose, null);

            object o0 = context.GetObjectByKey(key0);
            object o1 = context.GetObjectByKey(key1);

            return new[] {o0, o1};
        }

        private static IEnumerable<object> GetChangedObjectsInContext(ObjectContext context) {
            IEnumerable<ObjectStateEntry> addedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added).ToArray();
            IEnumerable<ObjectStateEntry> addedOseRelationships = addedOses.Where(ose => ose.IsRelationship);

            IEnumerable<ObjectStateEntry> deletedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).ToArray();
            IEnumerable<ObjectStateEntry> deletedOseRelationships = deletedOses.Where(ose => ose.IsRelationship);

            IEnumerable<ObjectStateEntry> changedOses = context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
            List<object> changedEntities = changedOses.Select(x => x.Entity).ToList();

            addedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));
            deletedOseRelationships.ForEach(x => changedEntities.AddRange(GetRelationshipEnds(context, x)));

            // this is here just to catch a case (adding sales reason to sales order in adventureworks) 
            // which doesn't work but which should.
            changedEntities.AddRange(GetRelationshipEndsForEntity(addedOses));
            changedEntities.AddRange(GetRelationshipEndsForEntity(deletedOses));

            // filter added and deleted entries 
            return changedEntities.Where(x => x != null).Distinct().Where(e => {
                ObjectStateEntry ose;
                context.ObjectStateManager.TryGetObjectStateEntry(e, out ose);
                return ose != null && ose.State != EntityState.Deleted && ose.State != EntityState.Added;
            });
        }

        private static IEnumerable<object> GetRelationshipEndsForEntity(IEnumerable<ObjectStateEntry> addedOses) {
            IEnumerable<IRelatedEnd> relatedends = addedOses.Where(ose => !ose.IsRelationship).SelectMany(x => x.RelationshipManager.GetAllRelatedEnds());
            IEnumerable<IRelatedEnd> references = relatedends.Where(x => x.GetType().GetGenericTypeDefinition() == typeof (EntityReference<>));
            return references.Select<IRelatedEnd, object>(x => ((dynamic) x).Value);
        }

        private static IEnumerable<object> GetChangedComplexObjectsInContext(LocalContext context) {
            IEnumerable<ObjectStateEntry> changedOses = context.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
            List<object> changedEntities = changedOses.Select(x => x.Entity).ToList();

            List<object> complexObjects = changedEntities.Select(o => new {Obj = o, Prop = context.GetComplexMembers(o.GetEntityProxiedType())}).
                SelectMany(a => a.Prop.Select(p => p.GetValue(a.Obj, null))).ToList();

            return complexObjects.Where(x => x != null).Distinct();
        }

        private static void ValidateIfRequired(INakedObjectAdapter adapter) {
            if (adapter.ResolveState.IsPersistent()) {
                if (adapter.Spec.ContainsFacet<IValidateProgrammaticUpdatesFacet>()) {
                    string state = adapter.ValidToPersist();
                    if (state != null) {
                        throw new PersistFailedException(string.Format(Resources.NakedObjects.PersistStateError, adapter.Spec.ShortName, adapter.TitleString(), state));
                    }
                }
            }
        }

        private void SavingChangesHandler(object sender, EventArgs e) {
            IEnumerable<object> changedObjects = GetChangedObjectsInContext((ObjectContext) sender);
            IEnumerable<INakedObjectAdapter> adaptedObjects = changedObjects.Where(o => TypeUtils.IsEntityProxy(o.GetType())).Select(domainObject => nakedObjectManager.CreateAdapter(domainObject, null, null)).ToArray();
            adaptedObjects.Where(x => x.ResolveState.IsGhost()).ForEach(ResolveImmediately);
            adaptedObjects.ForEach(ValidateIfRequired);
        }

        private static void ExecuteCommand(IPersistenceCommand command) {
            command.Execute();
        }

        private static void ExecuteCommands(IPersistenceCommand[] commands) {
            commands.ForEach(command => command.Execute());
        }

        public void Reset() {
            Log.Debug("Reset");
            contexts = contexts.ToDictionary(kvp => kvp.Key, ResetContext);
            contexts.Values.ForEach(c => c.Manager = nakedObjectManager);
        }

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                string typeNames = contexts.Values.SelectMany(c => c.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified).Where(o => o.Entity != null).Select(o => o.Entity.GetEntityProxiedType().FullName)).
                    Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                throw new NakedObjectDomainException(string.Format(Resources.NakedObjects.EntityCommitError, typeNames));
            }
            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && !field.ContainsFacet<INotPersistedFacet>()) {
                using (var dbContext = new DbContext(GetContext(nakedObjectAdapter).WrappedObjectContext, false)) {
                    // check this is an EF collection 
                    try {
                        return dbContext.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                    }
                    catch (ArgumentException) {
                        // not an EF recognised collection 
                    }
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }

        #region Nested type: EntityCreateObjectCommand

        private class EntityCreateObjectCommand : ICreateObjectCommand {
            private readonly LocalContext context;
            private readonly EntityObjectStore parent;
            private readonly INakedObjectAdapter nakedObjectAdapter;
            private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();

            public EntityCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter, LocalContext context, EntityObjectStore parent) {
                this.context = context;
                this.parent = parent;

                this.nakedObjectAdapter = nakedObjectAdapter;
            }

            #region ICreateObjectCommand Members

            public void Execute() {
                try {
                    Log.DebugFormat("Creating: {0}", nakedObjectAdapter);
                    context.CurrentSaveRootObjectAdapter = nakedObjectAdapter;
                    objectToProxyScratchPad.Clear();
                    ProxyObjectIfAppropriate(nakedObjectAdapter.Object);
                }
                catch (Exception e) {
                    Log.WarnFormat("Error in EntityCreateObjectCommand.Execute: {0}", e.Message);
                    throw;
                }
            }

            public INakedObjectAdapter OnObject() {
                return nakedObjectAdapter;
            }

            #endregion

            private void SetKeyAsNecessary(object objectToProxy, object proxy) {
                if (!context.IdMembersAreIdentity(objectToProxy.GetType())) {
                    PropertyInfo[] idMembers = context.GetIdMembers(objectToProxy.GetType());
                    idMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
                }
            }

            private object ProxyObjectIfAppropriate(object originalObject) {
                if (originalObject == null) {
                    return null;
                }

                if (TypeUtils.IsEntityProxy(originalObject.GetType())) {
                    // object already proxied assume previous save failed - add object to context again 

                    bool add = true;
                    ObjectStateEntry ose;
                    if (context.WrappedObjectContext.ObjectStateManager.TryGetObjectStateEntry(originalObject, out ose)) {
                        // EF knows object so check if detached 
                        add = ose.State == EntityState.Detached;
                    }
                    if (add) {
                        context.GetObjectSet(originalObject.GetEntityProxiedType()).AddObject(originalObject);
                    }
                    return originalObject;
                }

                if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                    return objectToProxyScratchPad[originalObject];
                }

                INakedObjectAdapter adapterForOriginalObjectAdapter = parent.createAdapter(null, originalObject);

                if (adapterForOriginalObjectAdapter.ResolveState.IsPersistent()) {
                    return originalObject;
                }

                return ProxyObject(originalObject, adapterForOriginalObjectAdapter);
            }

            private object ProxyObject(object originalObject, INakedObjectAdapter adapterForOriginalObjectAdapter) {
                dynamic objectToAdd = context.CreateObject(originalObject.GetType());

                bool proxied = objectToAdd.GetType() != originalObject.GetType();
                if (!proxied) {
                    objectToAdd = originalObject;
                }

                objectToProxyScratchPad[originalObject] = objectToAdd;
                adapterForOriginalObjectAdapter.Persisting();

                // create transient adapter here so that LoadObject knows proxy domainObject is transient
                // if not proxied this should just be the same as adapterForOriginalObjectAdapter
                INakedObjectAdapter proxyAdapter = parent.createAdapter(null, objectToAdd);

                SetKeyAsNecessary(originalObject, objectToAdd);
                context.GetObjectSet(originalObject.GetType()).AddObject(objectToAdd);

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

                CheckProxies((object) objectToAdd);

                return objectToAdd;
            }

            private void CallPersistingPersistedForComplexObjects(INakedObjectAdapter parentAdapter) {
                PropertyInfo[] complexMembers = context.GetComplexMembers(parentAdapter.Object.GetEntityProxiedType());
                foreach (PropertyInfo pi in complexMembers) {
                    object complexObject = pi.GetValue(parentAdapter.Object, null);
                    INakedObjectAdapter childAdapter = parent.createAggregatedAdapter(nakedObjectAdapter, pi, complexObject);
                    childAdapter.Persisting();
                    context.PersistedNakedObjects.Add(childAdapter);
                }
            }

            private void ProxyReferences(object objectToProxy) {
                // this is to ensure persisting/persisted gets call for all referenced transient objects - what it wont handle 
                // is if a referenced object is proxied - as it doesn't update the reference - not sure if that will be a requirement. 
                PropertyInfo[] refMembers = context.GetReferenceMembers(objectToProxy.GetType());
                refMembers.ForEach(pi => ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)));

                PropertyInfo[] colmembers = context.GetCollectionMembers(objectToProxy.GetType());
                foreach (PropertyInfo pi in colmembers) {
                    foreach (object item in (IEnumerable) pi.GetValue(objectToProxy, null)) {
                        ProxyObjectIfAppropriate(item);
                    }
                }
            }

            private void ProxyReferencesAndCopyValuesToProxy(object objectToProxy, dynamic proxy) {
                PropertyInfo[] nonIdMembers = context.GetNonIdMembers(objectToProxy.GetType());
                nonIdMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));

                PropertyInfo[] refMembers = context.GetReferenceMembers(objectToProxy.GetType());
                refMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, ProxyObjectIfAppropriate(pi.GetValue(objectToProxy, null)), null));

                PropertyInfo[] colmembers = context.GetCollectionMembers(objectToProxy.GetType());
                foreach (PropertyInfo pi in colmembers) {
                    dynamic toCol = proxy.GetType().GetProperty(pi.Name).GetValue(proxy, null);
                    var fromCol = (IEnumerable) pi.GetValue(objectToProxy, null);
                    foreach (object item in fromCol) {
                        toCol.Add((dynamic) ProxyObjectIfAppropriate(item));
                    }
                }

                PropertyInfo[] notPersistedMembers = objectToProxy.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<NotPersistedAttribute>() != null).ToArray();
                notPersistedMembers.ForEach(pi => proxy.GetType().GetProperty(pi.Name).SetValue(proxy, pi.GetValue(objectToProxy, null), null));
            }

            public override string ToString() {
                return "CreateObjectCommand [object=" + nakedObjectAdapter + "]";
            }
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
                Log.DebugFormat("Destroying: {0}", nakedObjectAdapter);
                context.WrappedObjectContext.DeleteObject(nakedObjectAdapter.Object);
                context.DeletedNakedObjects.Add(nakedObjectAdapter);
            }

            public INakedObjectAdapter OnObject() {
                return nakedObjectAdapter;
            }

            #endregion

            public override string ToString() {
                return "DestroyObjectCommand [object=" + nakedObjectAdapter + "]";
            }
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

            public void Execute() {
                // Log.DebugFormat("EntitySaveObjectCommand: pre refresh version in object {0}", nakedObjectAdapter.GetVersion());
                Log.DebugFormat("Saving: {0}", nakedObjectAdapter);
                context.CurrentUpdateRootObjectAdapter = nakedObjectAdapter;
            }

            public INakedObjectAdapter OnObject() {
                return nakedObjectAdapter;
            }

            #endregion

            public override string ToString() {
                return "SaveObjectCommand [object=" + nakedObjectAdapter + "]";
            }
        }

        #endregion

        #region Nested type: LocalContext

        public class LocalContext : IDisposable {
            private readonly List<object> added = new List<object>();
            private readonly IDictionary<Type, Type> baseTypeMap = new Dictionary<Type, Type>();
            private readonly ISet<INakedObjectAdapter> deletedNakedObjects = new HashSet<INakedObjectAdapter>();
            private readonly ISet<INakedObjectAdapter> loadedNakedObjects = new HashSet<INakedObjectAdapter>();
            private readonly ISet<Type> notPersistedTypes = new HashSet<Type>();
            private readonly ISet<Type> ownedTypes = new HashSet<Type>();
            private readonly ISet<INakedObjectAdapter> persistedNakedObjects = new HashSet<INakedObjectAdapter>();
            private readonly ISession session;
            private readonly EntityObjectStore parent;
            private readonly IDictionary<Type, StructuralType> typeToStructuralType = new Dictionary<Type, StructuralType>();
            private List<INakedObjectAdapter> coUpdating;
            private List<INakedObjectAdapter> updatingNakedObjects;

            private LocalContext(Type[] preCachedTypes, Type[] notPersistedTypes, ISession session, EntityObjectStore parent) {
                this.session = session;
                this.parent = parent;

                preCachedTypes.ForEach(t => ownedTypes.Add(t));
                notPersistedTypes.ForEach(t => this.notPersistedTypes.Add(t));
            }

            public LocalContext(PocoEntityContextConfiguration config, ISession session, EntityObjectStore parent)
                : this(config.PreCachedTypes(), config.NotPersistedTypes(), session, parent) {
                WrappedObjectContext = new ObjectContext("name=" + config.ContextName);
                Name = config.ContextName;
                Log.DebugFormat("Context {0} Created", Name);
            }

            public LocalContext(CodeFirstEntityContextConfiguration config, ISession session, EntityObjectStore parent)
                : this(config.PreCachedTypes(), config.NotPersistedTypes(), session, parent) {
                WrappedObjectContext = ((IObjectContextAdapter) config.DbContext()).ObjectContext;
                Name = WrappedObjectContext.DefaultContainerName;
                Log.DebugFormat("Context {0} Wrapped", Name);
            }

            public INakedObjectManager Manager { protected get; set; }
            public ObjectContext WrappedObjectContext { get; private set; }
            public string Name { get; private set; }

            public ISet<INakedObjectAdapter> LoadedNakedObjects {
                get { return loadedNakedObjects; }
            }

            public ISet<INakedObjectAdapter> PersistedNakedObjects {
                get { return persistedNakedObjects; }
            }

            public ISet<INakedObjectAdapter> DeletedNakedObjects {
                get { return deletedNakedObjects; }
            }

            public bool IsInitialized { get; set; }
            public MergeOption DefaultMergeOption { get; set; }
            public INakedObjectAdapter CurrentSaveRootObjectAdapter { get; set; }
            public INakedObjectAdapter CurrentUpdateRootObjectAdapter { get; set; }

            public Type GetMostBaseType(Type type) {
                if (!baseTypeMap.ContainsKey(type)) {
                    baseTypeMap[type] = MostBaseType(type);
                }
                return baseTypeMap[type];
            }

            private Type MostBaseType(Type type) {
                if (type.BaseType == typeof (object) || !(GetIsOwned(type.BaseType) || GetIsKnown(type.BaseType))) {
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

            private bool IsAlwaysUnrecognised(Type type) {
                if (type == null || type == typeof (object) || type.IsGenericType || notPersistedTypes.Contains(type)) {
                    return true;
                }
                return false;
            }

            private bool IsOwnedOrBaseTypeIsOwned(Type type) {
                if (IsAlwaysUnrecognised(type)) {
                    return false;
                }
                if (ownedTypes.Contains(type)) {
                    Log.DebugFormat("Context {0} found in owned cache type {1}", Name, type.FullName);
                    return true;
                }
                return IsOwnedOrBaseTypeIsOwned(type.BaseType);
            }

            private bool IsKnownOrBaseTypeIsKnown(Type type) {
                if (IsAlwaysUnrecognised(type)) {
                    return false;
                }
                if (this.ContextKnowsType(type)) {
                    Log.DebugFormat("Context {0} adding to owned cache type {1}", Name, type.FullName);
                    ownedTypes.Add(type);
                    return true;
                }
                return IsKnownOrBaseTypeIsKnown(type.BaseType);
            }

            public bool GetIsKnown(Type type) {
                return IsKnownOrBaseTypeIsKnown(type);
            }

            public bool GetIsOwned(Type type) {
                return IsOwnedOrBaseTypeIsOwned(type);
            }

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

                // need to do complextype separately as they'll not be updated in the SavingChangeshandler as they're not proxied. 
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
                    INakedObjectAdapter[] currentPersistedNakedObjectsAdapter = PersistedNakedObjects.ToArray();
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

            public void Dispose() {
                try {
                    WrappedObjectContext.Dispose();
                    baseTypeMap.Clear();
                }
                catch (Exception e) {
                    Log.ErrorFormat("Exception disposing context: {0}", e, Name);
                }
            }
        }

        #endregion

        public void Dispose() {
            contexts.Values.ForEach(c => c.Dispose());
        }
    }
}