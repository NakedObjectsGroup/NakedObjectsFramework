// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Transactions;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Persistor.Transaction;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Reflector.DotNet.Facets.Objects.Aggregated;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using BindingFlags = System.Reflection.BindingFlags;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace NakedObjects.EntityObjectStore {
    public class EntityObjectStore : INakedObjectStore {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityObjectStore));
        private static CreateAdapterDelegate createAdapter;
        private static CreateAggregatedAdapterDelegate createAggregatedAdapter;
        private static NotifyUiDelegate notifyUi;

        private static Action<INakedObject> persisted;
        private static Action<INakedObject> persisting;
        private static RemoveAdapterDelegate removeAdapter;
        private static ReplacePocoDelegate replacePoco;

        private static EventHandler savingChangesHandlerDelegate;
        private static Action<INakedObject> updated;
        private static Action<INakedObject> updating;
        private static Action<INakedObject> handleLoaded;
        private static Func<Type, INakedObjectSpecification> loadSpecification;

        private readonly EntityOidGenerator oidGenerator;

        private IDictionary<EntityContextConfiguration, LocalContext> contexts = new Dictionary<EntityContextConfiguration, LocalContext>();
        private IContainerInjector injector;

        #region Delegates

        public delegate INakedObject CreateAdapterDelegate(IOid oid, object domainObject);

        public delegate INakedObject CreateAggregatedAdapterDelegate(INakedObject nakedObject, PropertyInfo property, object newDomainObject);

        public delegate void NotifyUiDelegate(INakedObject changedObject);

        public delegate void RemoveAdapterDelegate(INakedObject nakedObject);

        public delegate void ReplacePocoDelegate(INakedObject nakedObject, object newDomainObject);

        #endregion

        static EntityObjectStore() {
            MaximumCommitCycles = 10;
            RollBackOnError = true;
            EnforceProxies = true;
            FirstInitialization = true;
            IsInitializedCheck = () => true;
        }

        internal EntityObjectStore() {
            createAdapter = PersistorUtils.CreateAdapter;
            replacePoco = PersistorUtils.ReplacePoco;
            removeAdapter = PersistorUtils.RemoveAdapter;
            createAggregatedAdapter = PersistorUtils.CreateAggregatedAdapter;
            notifyUi = x => NakedObjectsContext.UpdateNotifier.AddChangedObject(x);
            updating = x => x.Updating();
            updated = x => x.Updated();
            persisting = x => x.Persisting();
            persisted = x => x.Persisted();
            handleLoaded = HandleLoadedDefault;
            savingChangesHandlerDelegate = SavingChangesHandler;
            loadSpecification = x => NakedObjectsContext.Reflector.LoadSpecification(x);
        }


        public EntityObjectStore(IEnumerable<EntityContextConfiguration> config, EntityOidGenerator oidGenerator)
            : this() {
            this.oidGenerator = oidGenerator;
            contexts = config.ToDictionary<EntityContextConfiguration, EntityContextConfiguration, LocalContext>(c => c, c => null);
        }

        #region for testing only

        public void SetupForTesting(IContainerInjector containerInjector,
                                    CreateAdapterDelegate createAdapterDelegate,
                                    ReplacePocoDelegate replacePocoDelegate,
                                    RemoveAdapterDelegate removeAdapterDelegate,
                                    CreateAggregatedAdapterDelegate createAggregatedAdapterDelegate,
                                    NotifyUiDelegate notifyUiDelegate,
                                    Action<INakedObject> updatedFunc,
                                    Action<INakedObject> updatingFunc,
                                    Action<INakedObject> persistedFunc,
                                    Action<INakedObject> persistingFunc,
                                    Action<INakedObject> handleLoadedTest,
                                    EventHandler savingChangeshandler,
                                    Func<Type, INakedObjectSpecification> loadSpecificationHandler) {
            injector = containerInjector;
            createAdapter = createAdapterDelegate;
            replacePoco = replacePocoDelegate;
            removeAdapter = removeAdapterDelegate;
            createAggregatedAdapter = createAggregatedAdapterDelegate;
            notifyUi = notifyUiDelegate;
            updated = updatedFunc;
            updating = updatingFunc;
            persisted = persistedFunc;
            persisting = persistingFunc;
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

        public IContainerInjector Injector {
            get {
                if (injector == null) {
                    injector = NakedObjectsContext.ObjectPersistor;
                }
                return injector;
            }
            set { injector = value; }
        }

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

        private LocalContext GetContext(INakedObject nakedObject) {
            return GetContext(nakedObject.Object);
        }

        private LocalContext ResetContext(KeyValuePair<EntityContextConfiguration, LocalContext> kvp) {
            LocalContext context = kvp.Value;
            EntityContextConfiguration config = kvp.Key;

            if (context != null) {
                context.Dispose();
            }

            if (config is CodeFirstEntityContextConfiguration) {
                context = ResetCodeOnlyContext(config as CodeFirstEntityContextConfiguration);
            }
            else {
                context = ResetPocoContext(config as PocoEntityContextConfiguration);
            }
            context.DefaultMergeOption = config.DefaultMergeOption;
            context.WrappedObjectContext.ContextOptions.LazyLoadingEnabled = true;
            context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled = true;
            context.WrappedObjectContext.SavingChanges += savingChangesHandlerDelegate;
            context.WrappedObjectContext.ObjectStateManager.ObjectStateManagerChanged += (obj, args) => {
                if (args.Action == CollectionChangeAction.Add) {
                    LoadObject(args.Element, context);
                }
            };

            return context;
        }

        private void ResolveChildCollections(INakedObject nakedObject) {
            if (nakedObject.Specification != null) {
                // testing check 
                foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(a => a.IsCollection && a.IsPersisted)) {
                    INakedObject adapter = assoc.GetNakedObject(nakedObject);
                    if (adapter.ResolveState.IsGhost()) {
                        StartResolving(adapter, GetContext(nakedObject));
                        EndResolving(adapter);
                    }
                }
            }
        }

        private static void RollBackContext(LocalContext context) {
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

        private static void StartResolving(INakedObject nakedObject, LocalContext context) {
            IResolveEvent resolveEvent = (!nakedObject.ResolveState.IsTransient() &&
                                          !context.WrappedObjectContext.ContextOptions.ProxyCreationEnabled) ? Events.StartPartResolvingEvent :
                                             Events.StartResolvingEvent;
            nakedObject.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObject nakedObject) {
            nakedObject.ResolveState.Handle(nakedObject.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);
        }


        private static void HandleLoadedDefault(INakedObject nakedObject) {
            EndResolving(nakedObject);
        }


        private void InvokeErrorFacet(Exception exception) {
            string newMessage = exception.Message;

            foreach (LocalContext context in contexts.Values) {
                if (context.CurrentSaveRootObject != null) {
                    INakedObject target = context.CurrentSaveRootObject;
                    if (target.Specification != null) {
                        // can be null in tests
                        newMessage = target.Specification.GetFacet<IOnPersistingErrorCallbackFacet>().Invoke(target, exception);
                    }
                    break;
                }
                if (context.CurrentUpdateRootObject != null) {
                    INakedObject target = context.CurrentUpdateRootObject;
                    if (target.Specification != null) {
                        // can be null in tests 
                        newMessage = target.Specification.GetFacet<IOnUpdatingErrorCallbackFacet>().Invoke(target, exception);
                    }
                    break;
                }
            }

            // Rollback after extracting info from context - rollback clears it all
            RollBackContext();

            if (exception is ConcurrencyException) {
                throw new ConcurrencyException(newMessage, exception) {SourceNakedObject = ((ConcurrencyException) exception).SourceNakedObject};
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

        private static INakedObject GetSourceNakedObject(OptimisticConcurrencyException oce) {
            object trigger = oce.StateEntries.Where(e => !e.IsRelationship).Select(e => e.Entity).SingleOrDefault();
            return createAdapter(null, trigger);
        }

        private static LocalContext ResetPocoContext(PocoEntityContextConfiguration pocoConfig) {
            try {
                return new LocalContext(pocoConfig) {IsInitialized = true};
            }
            catch (Exception e) {
                string explain = string.Format(Resources.NakedObjects.StartPersistorErrorMessage, pocoConfig.ContextName);
                throw new InitialisationException(explain, e);
            }
        }

        private static LocalContext ResetCodeOnlyContext(CodeFirstEntityContextConfiguration codeOnlyConfig) {
            try {
                return new LocalContext(codeOnlyConfig);
            }
            catch (Exception e) {
                throw new InitialisationException(Resources.NakedObjects.StartPersistorErrorCodeFirst, e);
            }
        }

        private void HandleAdded(INakedObject nakedObject) {
            var oid = (EntityOid) nakedObject.Oid;
            LocalContext context = GetContext(nakedObject);
            oid.MakePersistentAndUpdateKey(context.GetKey(nakedObject));

            if (nakedObject.ResolveState.IsNotPersistent()) {
                StartResolving(nakedObject, context);
                EndResolving(nakedObject);
            }
            if (nakedObject.Specification != null) {
                // testing check 
                foreach (INakedObjectAssociation assoc in nakedObject.Specification.Properties.Where(a => a.IsCollection && a.IsPersisted)) {
                    INakedObject adapter = assoc.GetNakedObject(nakedObject);
                    if (adapter.ResolveState.IsGhost()) {
                        StartResolving(adapter, GetContext(adapter));
                        EndResolving(adapter);
                    }
                }
            }
            Log.DebugFormat("Added {0}", nakedObject);
        }

        private void MarkAsLoaded(INakedObject nakedObject) {
            GetContext(nakedObject).LoadedNakedObjects.Add(nakedObject);
        }

        public object GetObjectByKey(EntityOid eoid, Type type) {
            string entitySetName;
            LocalContext context = GetContext(type);
            List<KeyValuePair<string, object>> memberValueMap = GetMemberValueMap(type, eoid, out entitySetName);

            dynamic oq = context.CreateQuery(type, entitySetName);

            foreach (var kvp in memberValueMap) {
                string query = string.Format("it.{0}=@{0}", kvp.Key);
                oq = oq.Where(query, new ObjectParameter(kvp.Key, kvp.Value));
            }
            context.GetNavigationMembers(type).Where(m => !CollectionUtils.IsCollection(m.PropertyType)).ForEach(pi => oq = oq.Include(pi.Name));
            return First((IEnumerable) oq.Execute(context.DefaultMergeOption));
        }

        private List<KeyValuePair<string, object>> GetMemberValueMap(Type type, EntityOid eoid, out string entitySetName) {
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

        public object GetObjectByKey(EntityOid eoid, INakedObjectSpecification hint) {
            Type type = TypeUtils.GetType(hint.FullName);
            return GetObjectByKey(eoid, type);
        }

        private static object First(IEnumerable enumerable) {
            foreach (object o in enumerable) {
                return o;
            }
            return null;

            // unfortunately this cast doesn't work with entity linq
            // return queryable.Cast<object>().FirstOrDefault();
        }

        public bool EntityFrameworkKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    return FindContext(type) != null;
                }
            }
            catch {
                // ignore all 
            }

            return false;
        }

        public INakedObject CreateAdapterForKnownObject(object domainObject) {
            EntityOid oid = oidGenerator.CreateOid(EntityUtils.GetProxiedTypeName(domainObject), GetContext(domainObject).GetKey(domainObject));
            return new PocoAdapter(domainObject, oid);
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
                                                                                         p.PropertyType.IsAssignableFrom(parent.GetType()) &&
                                                                                         p.GetCustomAttribute<RootAttribute>() != null);
            if (property != null) {
                property.SetValue(child, parent, null);
            }
        }

        public void LoadComplexTypes(INakedObject nakedObject, bool parentIsGhost) {
            if (EntityFrameworkKnowsType(nakedObject.Object.GetProxiedType())) {
                foreach (PropertyInfo pi in GetContext(nakedObject).GetComplexMembers(nakedObject.Object.GetProxiedType())) {
                    INakedObjectSpecification spec = loadSpecification(pi.PropertyType);
                    if (!spec.ContainsFacet<IComplexTypeFacet>()) {
                        Log.InfoFormat("Adding InlineFacet to {0} by convention", spec.FullName);
                        spec.AddFacet(new ComplexTypeFacetConvention(spec));
                    }
                    object complexObject = pi.GetValue(nakedObject.Object, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    InjectParentIntoChild(nakedObject.Object, complexObject);
                    Injector.InitDomainObject(complexObject);
                    createAggregatedAdapter(nakedObject, pi, complexObject);
                }
            }
        }

        private static void CheckProxies(object objectToCheck) {
            if (EnforceProxies) {
                Assert.AssertTrue(string.Format(Resources.NakedObjects.NoProxyMessage, objectToCheck.GetType(), Resources.NakedObjects.ProxyExplanation), TypeUtils.IsEntityProxy(objectToCheck.GetType()));
                Assert.AssertTrue(string.Format(Resources.NakedObjects.NoChangeTrackerMessage, objectToCheck.GetType(), Resources.NakedObjects.ProxyExplanation), objectToCheck is IEntityWithChangeTracker);
            }
        }

        private void LoadObject(object domainObject, LocalContext context) {
            CheckProxies(domainObject);
            EntityOid oid = oidGenerator.CreateOid(EntityUtils.GetProxiedTypeName(domainObject), context.GetKey(domainObject));
            INakedObject nakedObject = createAdapter(oid, domainObject);
            Injector.InitDomainObject(nakedObject.Object);
            LoadComplexTypes(nakedObject, nakedObject.ResolveState.IsGhost());
            nakedObject.UpdateVersion();

            if (nakedObject.ResolveState.IsGhost()) {
                StartResolving(nakedObject, context);
                MarkAsLoaded(nakedObject);
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

            List<object> complexObjects = changedEntities.Select(o => new {Obj = o, Prop = context.GetComplexMembers(o.GetProxiedType())}).
                                                          SelectMany(a => a.Prop.Select(p => p.GetValue(a.Obj, null))).ToList();

            return complexObjects.Where(x => x != null).Distinct();
        }

        private static void ValidateIfRequired(INakedObject adapter) {
            if (adapter.ResolveState.IsPersistent()) {
                DotNetDomainObjectContainer.Validate(adapter);
            }
        }

        private void SavingChangesHandler(object sender, EventArgs e) {
            IEnumerable<object> changedObjects = GetChangedObjectsInContext((ObjectContext) sender);
            IEnumerable<INakedObject> adaptedObjects = changedObjects.Where(o => TypeUtils.IsEntityProxy(o.GetType())).Select(PersistorUtils.CreateAdapter).ToArray();
            adaptedObjects.Where(x => x.ResolveState.IsGhost()).ForEach(ResolveImmediately);
            adaptedObjects.ForEach(ValidateIfRequired);
            adaptedObjects.ForEach(x => notifyUi(x));
        }

        private static T ExecuteCommand<T>(T command) where T : IPersistenceCommand {
            command.Execute(null);
            return default(T);
        }

        protected static void ExecuteCommands(IPersistenceCommand[] commands) {
            commands.ForEach(command => command.Execute(null));
        }

        #region Nested type: EntityCreateObjectCommand

        private class EntityCreateObjectCommand : ICreateObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObject nakedObject;
            private readonly IDictionary<object, object> objectToProxyScratchPad = new Dictionary<object, object>();

            public EntityCreateObjectCommand(INakedObject nakedObject, LocalContext context) {
                this.context = context;
                this.nakedObject = nakedObject;
            }

            #region ICreateObjectCommand Members

            public void Execute(IExecutionContext executionContext) {
                try {
                    Log.DebugFormat("Creating: {0}", nakedObject);
                    context.CurrentSaveRootObject = nakedObject;
                    objectToProxyScratchPad.Clear();
                    ProxyObjectIfAppropriate(nakedObject.Object);
                }
                catch (Exception e) {
                    Log.WarnFormat("Error in EntityCreateObjectCommand.Execute: {0}", e.Message);
                    throw;
                }
            }

            public INakedObject OnObject() {
                return nakedObject;
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
                        context.GetObjectSet(originalObject.GetProxiedType()).AddObject(originalObject);
                    }
                    return originalObject;
                }

                if (objectToProxyScratchPad.ContainsKey(originalObject)) {
                    return objectToProxyScratchPad[originalObject];
                }

                INakedObject adapterForOriginalObject = createAdapter(null, originalObject);

                if (adapterForOriginalObject.ResolveState.IsPersistent()) {
                    return originalObject;
                }

                return ProxyObject(originalObject, adapterForOriginalObject);
            }

            private object ProxyObject(object originalObject, INakedObject adapterForOriginalObject) {
                dynamic objectToAdd = context.CreateObject(originalObject.GetType());

                bool proxied = objectToAdd.GetType() != originalObject.GetType();
                if (!proxied) {
                    objectToAdd = originalObject;
                }

                objectToProxyScratchPad[originalObject] = objectToAdd;
                persisting(adapterForOriginalObject);

                // create transient adapter here so that LoadObject knows proxy domainObject is transient
                // if not proxied this should just be the same as adapterForOriginalObject
                INakedObject proxyAdapter = createAdapter(null, objectToAdd);

                SetKeyAsNecessary(originalObject, objectToAdd);
                context.GetObjectSet(originalObject.GetType()).AddObject(objectToAdd);

                if (proxied) {
                    ProxyReferencesAndCopyValuesToProxy(originalObject, objectToAdd);
                    context.PersistedNakedObjects.Add(proxyAdapter);
                    // remove temporary adapter for proxy (tidy and also means we will not get problem 
                    // with already known object in identity map when replacing the poco
                    removeAdapter(proxyAdapter);
                    replacePoco(adapterForOriginalObject, objectToAdd);
                }
                else {
                    ProxyReferences(originalObject);
                    context.PersistedNakedObjects.Add(proxyAdapter);
                }
                CallPersistingPersistedForComplexObjects(proxyAdapter);

                CheckProxies((object) objectToAdd);

                return objectToAdd;
            }

            private void CallPersistingPersistedForComplexObjects(INakedObject parent) {
                PropertyInfo[] complexMembers = context.GetComplexMembers(parent.Object.GetProxiedType());
                foreach (PropertyInfo pi in complexMembers) {
                    object complexObject = pi.GetValue(parent.Object, null);
                    INakedObject childAdapter = createAggregatedAdapter(nakedObject, pi, complexObject);
                    persisting(childAdapter);
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
                return "CreateObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested type: EntityDestroyObjectCommand

        private class EntityDestroyObjectCommand : IDestroyObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObject nakedObject;

            public EntityDestroyObjectCommand(INakedObject nakedObject, LocalContext context) {
                this.context = context;
                this.nakedObject = nakedObject;
            }

            #region IDestroyObjectCommand Members

            public void Execute(IExecutionContext executionContext) {
                Log.DebugFormat("Destroying: {0}", nakedObject);
                context.WrappedObjectContext.DeleteObject(nakedObject.Object);
                context.DeletedNakedObjects.Add(nakedObject);
            }

            public INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "DestroyObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested type: EntitySaveObjectCommand

        private class EntitySaveObjectCommand : ISaveObjectCommand {
            private readonly LocalContext context;
            private readonly INakedObject nakedObject;

            public EntitySaveObjectCommand(INakedObject nakedObject, LocalContext context) {
                this.context = context;
                this.nakedObject = nakedObject;
            }

            #region ISaveObjectCommand Members

            public void Execute(IExecutionContext executionContext) {
                Log.DebugFormat("EntitySaveObjectCommand: pre refresh version in object {0}", nakedObject.GetVersion());
                Log.DebugFormat("Saving: {0}", nakedObject);
                context.CurrentUpdateRootObject = nakedObject;
            }

            public INakedObject OnObject() {
                return nakedObject;
            }

            #endregion

            public override string ToString() {
                return "SaveObjectCommand [object=" + nakedObject + "]";
            }
        }

        #endregion

        #region Nested type: LocalContext

        public class LocalContext {
            private readonly List<object> added = new List<object>();
            private readonly IDictionary<Type, Type> baseTypeMap = new Dictionary<Type, Type>();
            private readonly ISet<INakedObject> deletedNakedObjects = new HashSet<INakedObject>();
            private readonly ISet<INakedObject> loadedNakedObjects = new HashSet<INakedObject>();
            private readonly ISet<Type> notPersistedTypes = new HashSet<Type>();
            private readonly ISet<Type> ownedTypes = new HashSet<Type>();
            private readonly ISet<INakedObject> persistedNakedObjects = new HashSet<INakedObject>();
            private readonly IDictionary<Type, StructuralType> typeToStructuralType = new Dictionary<Type, StructuralType>();
            private List<INakedObject> coUpdating;
            private EntityConnection connection;
            private List<INakedObject> updatingNakedObjects;

            private LocalContext(Type[] preCachedTypes, Type[] notPersistedTypes) {
                preCachedTypes.ForEach(t => ownedTypes.Add(t));
                notPersistedTypes.ForEach(t => this.notPersistedTypes.Add(t));
            }

            public LocalContext(PocoEntityContextConfiguration config)
                : this(config.PreCachedTypes(), config.NotPersistedTypes()) {
                WrappedObjectContext = new ObjectContext("name=" + config.ContextName);
                Name = config.ContextName;
                Log.DebugFormat("Context {0} Created", Name);
                ValidatePreCachedTypes(config);
            }

            public LocalContext(CodeFirstEntityContextConfiguration config)
                : this(config.PreCachedTypes(), config.NotPersistedTypes()) {
                WrappedObjectContext = ((IObjectContextAdapter) config.DbContext()).ObjectContext;
                Name = WrappedObjectContext.DefaultContainerName;
                Log.DebugFormat("Context {0} Wrapped", Name);
                ValidatePreCachedTypes(config);
            }

            public ObjectContext WrappedObjectContext { get; private set; }

            public string Name { get; private set; }

            public ISet<INakedObject> LoadedNakedObjects {
                get { return loadedNakedObjects; }
            }

            public ISet<INakedObject> PersistedNakedObjects {
                get { return persistedNakedObjects; }
            }

            public ISet<INakedObject> DeletedNakedObjects {
                get { return deletedNakedObjects; }
            }

            public bool IsInitialized { get; set; }

            public MergeOption DefaultMergeOption { get; set; }
            public INakedObject CurrentSaveRootObject { get; set; }
            public INakedObject CurrentUpdateRootObject { get; set; }

            private void ValidatePreCachedTypes(EntityContextConfiguration config) {
                // do only once 

                if (FirstInitialization) {
                    bool temp = RequireExplicitAssociationOfTypes;

                    try {
                        RequireExplicitAssociationOfTypes = false;

                        IEnumerable<string> incorrectTypes = config.PreCachedTypes().Where(t => !this.ContextKnowsType(t)).Select(t => t.ToString()).ToArray();
                        if (incorrectTypes.Any()) {
                            throw new InitialisationException(string.Format("Attempting to associate types: {0} to context: {1} that are unrecognised", incorrectTypes.ListOut(), Name));
                        }

                        incorrectTypes = config.NotPersistedTypes().Where(this.ContextKnowsType).Select(t => t.ToString());
                        if (incorrectTypes.Any()) {
                            throw new InitialisationException(string.Format("Attempting to mark as 'Not Persisted' types: {0} that are recognised by context: {1}", incorrectTypes.ListOut(), Name));
                        }
                    }
                    finally {
                        RequireExplicitAssociationOfTypes = temp;
                    }
                }
            }

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
                CurrentSaveRootObject = null;
                CurrentUpdateRootObject = null;
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
                updatingNakedObjects = GetChangedObjectsInContext(WrappedObjectContext).Select(obj => createAdapter(null, obj)).ToList();
                updatingNakedObjects.ForEach(updating);

                // need to do complextype separately as they'll not be updated in the SavingChangeshandler as they're not proxied. 
                coUpdating = GetChangedComplexObjectsInContext(this).Select(obj => createAdapter(null, obj)).ToList();
                coUpdating.ForEach(updating);
            }

            public bool HasChanges() {
                WrappedObjectContext.DetectChanges();
                return WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified).Any();
            }


            public void PostSave(EntityObjectStore store) {
                try {
                    updatingNakedObjects.ForEach(updated);
                    updatingNakedObjects.ForEach(x => x.UpdateVersion());
                    coUpdating.ForEach(updated);
                    // Take a copy of PersistedNakedObjects and clear original so new ones can be added 
                    INakedObject[] currentPersistedNakedObjects = PersistedNakedObjects.ToArray();
                    PersistedNakedObjects.Clear();
                    currentPersistedNakedObjects.ForEach(persisted);
                }
                finally {
                    coUpdating.ForEach(x => notifyUi(x));
                    updatingNakedObjects.Clear();
                    coUpdating.Clear();
                }
            }

            public void PostSaveWrapUp(EntityObjectStore store) {
                added.Select(domainObject => createAdapter(null, domainObject)).ForEach(store.HandleAdded);
                LoadedNakedObjects.ToList().ForEach(handleLoaded);
            }

            public void Dispose() {
                if (connection != null) {
                    connection.Dispose();
                    connection = null;
                }
                WrappedObjectContext.Dispose();
                baseTypeMap.Clear();
            }
        }

        #endregion

        #region INakedObjectStore Members

        private static bool FirstInitialization { get; set; }

        public static bool EnforceProxies { get; set; }

        public static bool RollBackOnError { get; set; }

        public static int MaximumCommitCycles { get; set; }

        public static Func<bool> IsInitializedCheck { get; set; }
        public static bool RequireExplicitAssociationOfTypes { get; set; }

        public void Reset() {
            Log.Debug("Reset");
            contexts = contexts.ToDictionary(kvp => kvp.Key, ResetContext);
            FirstInitialization = false; // so validation of types only happens once 
        }

        public void AbortTransaction() {
            Log.Debug("AbortTransaction");
            RollBackContext();
        }


        public ICreateObjectCommand CreateCreateObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateCreateObjectCommand : {0}", nakedObject);
            try {
                return ExecuteCommand(new EntityCreateObjectCommand(nakedObject, GetContext(nakedObject)));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObject = nakedObject};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public IDestroyObjectCommand CreateDestroyObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateDestroyObjectCommand : {0}", nakedObject);
            try {
                return ExecuteCommand(new EntityDestroyObjectCommand(nakedObject, GetContext(nakedObject)));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObject = nakedObject};
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public ISaveObjectCommand CreateSaveObjectCommand(INakedObject nakedObject) {
            Log.DebugFormat("CreateSaveObjectCommand : {0}", nakedObject);
            try {
                return ExecuteCommand(new EntitySaveObjectCommand(nakedObject, GetContext(nakedObject)));
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObject = nakedObject};
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
                InvokeErrorFacet(new ConcurrencyException(ConcatenateMessages(oce), oce) {SourceNakedObject = GetSourceNakedObject(oce)});
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

        public bool Flush(IPersistenceCommand[] commands) {
            Log.DebugFormat("Flush {0} commands", commands.Length);
            if (commands.Length > 0) {
                ExecuteCommands(commands);
                return true;
            }
            return false;
        }

        public IQueryable GetInstances(INakedObjectSpecification spec) {
            Log.DebugFormat("GetInstances for: {0}", spec);
            Type type = TypeUtils.GetType(spec.FullName);
            return GetContext(type).GetObjectSet(type);
        }

        public INakedObject GetObject(IOid oid, INakedObjectSpecification hint) {
            Log.DebugFormat("GetObject oid: {0} hint: {1}", oid, hint);
            if (oid is EntityOid) {
                INakedObject adapter = createAdapter(oid, GetObjectByKey((EntityOid) oid, hint));
                adapter.UpdateVersion();
                return adapter;
            }
            var aggregateOid = oid as AggregateOid;
            if (aggregateOid != null) {
                var parentOid = (EntityOid) aggregateOid.ParentOid;
                string parentType = parentOid.TypeName;
                INakedObjectSpecification parentSpec = NakedObjectsContext.Reflector.LoadSpecification(parentType);
                INakedObject parent = createAdapter(parentOid, GetObjectByKey(parentOid, parentSpec));

                return parent.Specification.GetProperty(aggregateOid.FieldName).GetNakedObject(parent);
            }
            throw new NakedObjectSystemException("Unexpected oid type: " + oid.GetType());
        }

        public IOid GetOidForService(string name, string typeName) {
            Log.DebugFormat("GetOidForService name: {0}", name);
            return oidGenerator.CreateOid(typeName, new object[] {0});
        }

        public bool IsInitialized {
            get { return IsInitializedCheck(); }
            set { }
        }

        public string Name {
            get { return "Entity Object Store"; }
        }


        public void RegisterService(string name, IOid oid) {
            Log.DebugFormat("RegisterService name: {0} oid : {1}", name, oid);
            // do nothing 
        }

        public void ResolveField(INakedObject nakedObject, INakedObjectAssociation field) {
            Log.DebugFormat("ResolveField nakedobject: {0} field: {1}", nakedObject, field);
            field.GetNakedObject(nakedObject);
        }

        public int CountField(INakedObject nakedObject, INakedObjectAssociation field) {
            Type type = TypeUtils.GetType(field.GetFacet<ITypeOfFacet>().ValueSpec.FullName);
            MethodInfo countMethod = GetType().GetMethod("Count").GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int) countMethod.Invoke(this, new object[] {nakedObject, field});
        }

        public INakedObject FindByKeys(Type type, object[] keys) {
            EntityOid eoid = oidGenerator.CreateOid(type.FullName, keys);
            INakedObjectSpecification hint = loadSpecification(type);
            return GetObject(eoid, hint);
        }

        public void ResolveImmediately(INakedObject nakedObject) {
            Log.DebugFormat("ResolveImmediately nakedobject: {0}", nakedObject);
            // eagerly load object        
            nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
            // only if not proxied
            Type entityType = nakedObject.Object.GetType();

            if (!TypeUtils.IsEntityProxy(entityType)) {
                LocalContext currentContext = GetContext(entityType);

                IEnumerable<string> propertynames = currentContext.GetNavigationMembers(entityType).Select(x => x.Name);
                dynamic objectSet = currentContext.GetObjectSet(entityType);

                foreach (string name in propertynames) {
                    objectSet = objectSet.Include(name);
                }

                IList<PropertyInfo> idmembers = currentContext.GetIdMembers(entityType);
                IList<object> keyValues = ((EntityOid) nakedObject.Oid).Key;
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
            EndResolving(nakedObject);
            ResolveChildCollections(nakedObject);
        }


        public void StartTransaction() {
            Log.Debug("StartTransaction");
            // do nothing 
        }

        public void Init() {
            Log.Debug("Init");
        }

        public void Shutdown() {
            Log.Debug("Shutdown");
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
            Injector.InitDomainObject(domainObject);

            if (EntityFrameworkKnowsType(type)) {
                foreach (PropertyInfo pi in GetContext(domainObject).GetComplexMembers(domainObject.GetType())) {
                    object complexObject = pi.GetValue(domainObject, null);
                    Assert.AssertNotNull("Complex type members should never be null", complexObject);
                    Injector.InitDomainObject(complexObject);
                }
            }
            return domainObject;
        }

        public void Reload(INakedObject nakedObject) {
            Log.Debug("Reload nakedobject: " + nakedObject);
            Refresh(nakedObject);
        }

        public T CreateInstance<T>() where T : class {
            Log.Debug("CreateInstance<T> of: " + typeof (T));
            return (T) CreateInstance(typeof (T));
        }

        public PropertyInfo[] GetKeys(Type type) {
            Log.Debug("GetKeys of: " + type);
            return GetContext(type.GetProxiedType()).GetIdMembers(type.GetProxiedType());
        }

        public void Refresh(INakedObject nakedObject) {
            Log.Debug("Refresh nakedobject: " + nakedObject);
            if (nakedObject.Specification.GetFacet<IComplexTypeFacet>() == null) {
                updating(nakedObject);
                GetContext(nakedObject.Object.GetType()).WrappedObjectContext.Refresh(RefreshMode.StoreWins, nakedObject.Object);
                updated(nakedObject);
            }
        }

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                string typeNames = contexts.Values.SelectMany(c => c.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified).Where(o => o.Entity != null).Select(o => o.Entity.GetProxiedType().FullName)).
                                            Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                throw new NakedObjectDomainException(string.Format(Resources.NakedObjects.EntityCommitError, typeNames));
            }
            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        public int Count<T>(INakedObject nakedObject, INakedObjectAssociation field) where T : class {
            if (!nakedObject.ResolveState.IsTransient()) {
                using (var dbContext = new DbContext(GetContext(nakedObject).WrappedObjectContext, false)) {
                    // check this is an EF collection 
                    try {
                        return dbContext.Entry(nakedObject.Object).Collection(field.Id).Query().Cast<T>().Count();
                    }
                    catch (ArgumentException) {
                        // not an EF recognised collection 
                    }
                }
            }

            return field.GetNakedObject(nakedObject).GetAsEnumerable().Count();
        }

        #endregion
    }
}