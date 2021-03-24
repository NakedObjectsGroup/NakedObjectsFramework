// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
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


namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreObjectStore : IObjectStore, IDisposable {
        private readonly LocalContext context;
        internal readonly ILogger<EFCoreObjectStore> Logger;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IOidGenerator oidGenerator;
        private readonly ISession session;
        private readonly IMetamodelManager metamodelManager;
        private readonly IDomainObjectInjector injector;

        public Func<IDictionary<object, object>, bool> FunctionalPostSave = _ => false;

        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();

        public EFCoreObjectStore(EFCorePersistorConfiguration config,
                                 IOidGenerator oidGenerator,
                                 INakedObjectManager nakedObjectManager,
                                 ISession session,
                                 IMetamodelManager metamodelManager,
                                 IDomainObjectInjector injector,
                                 ILogger<EFCoreObjectStore> logger) {
            this.oidGenerator = oidGenerator;
            this.nakedObjectManager = nakedObjectManager;
            this.session = session;
            this.metamodelManager = metamodelManager;
            this.injector = injector;
            Logger = logger;
            context = new LocalContext(config, session, this);
            MaximumCommitCycles = config.MaximumCommitCycles;

            context.WrappedDbContext.ChangeTracker.StateChanged += (_, args) => {
                if (args.OldState == EntityState.Added) {
                    LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context.WrappedDbContext);
                }
            };

            context.WrappedDbContext.ChangeTracker.Tracked += (_, args) => { LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context.WrappedDbContext); };
        }

        private int MaximumCommitCycles { get; }

        public void Dispose() {
            context.Dispose();
        }

        public bool IsInitialized { get; }
        public string Name { get; }

        public void AbortTransaction() {
            throw new NotImplementedException();
        }

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreCreateObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter), this));

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreDestroyObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) =>
            Execute(new EFCoreSaveObjectCommand(nakedObjectAdapter, GetContext(nakedObjectAdapter)));



        private void PostSaveWrapUp() => context.PostSaveWrapUp();

        private void InvokeErrorFacet(Exception exception)
        {
            var newMessage = exception.Message;

            //foreach (var context in contexts.Values)
            //{
                if (context.CurrentSaveRootObjectAdapter?.Spec != null)
                {
                    var target = context.CurrentSaveRootObjectAdapter;
                    // can be null in tests
                    newMessage = target.Spec.GetFacet<IOnPersistingErrorCallbackFacet>()?.Invoke(target, exception);
                    //break;
                }

                if (context.CurrentUpdateRootObjectAdapter?.Spec != null)
                {
                    var target = context.CurrentUpdateRootObjectAdapter;
                    // can be null in tests 
                    newMessage = target.Spec.GetFacet<IOnUpdatingErrorCallbackFacet>()?.Invoke(target, exception);
                    //break;
                }
            //}

            // Rollback after extracting info from context - rollback clears it all
            //RollBackContext();

            newMessage ??= exception.Message;

            switch (exception)
            {
                case ConcurrencyException concurrencyException:
                    throw new ConcurrencyException(newMessage, exception) { SourceNakedObjectAdapter = concurrencyException.SourceNakedObjectAdapter };
                case DataUpdateException _:
                    throw new DataUpdateException(newMessage, exception);
                default:
                    // should never get here - just rethrow 
                    Logger.LogError($"Unexpected exception {exception}");
                    throw exception;
            }
        }

        private INakedObjectAdapter GetSourceNakedObject(UpdateException oce)
        {
            //var trigger = oce.StateEntries.Where(e => !e.IsRelationship).Select(e => e.Entity).SingleOrDefault();
            //return createAdapter(null, trigger);
            throw new NotImplementedException();
        }

        public void EndTransaction()
        {
            try
            {
                using (var transaction = CreateTransactionScope())
                {
                    RecurseUntilAllChangesApplied(1);
                    transaction.Complete();
                }

                PostSaveWrapUp();
            }
            catch (OptimisticConcurrencyException oce)
            {
                InvokeErrorFacet(new ConcurrencyException(ConcatenateMessages(oce), oce) { SourceNakedObjectAdapter = GetSourceNakedObject(oce) });
            }
            catch (UpdateException ue)
            {
                InvokeErrorFacet(new DataUpdateException(ConcatenateMessages(ue), ue));
            }
            catch (Exception e)
            {
                Logger.LogError($"Unexpected exception while applying changes {e.Message}");
                //RollBackContext();
                throw;
            }
            finally
            {
                context.SaveOrUpdateComplete();
            }
        }


        public IQueryable<T> GetInstances<T>(bool tracked = true) where T : class => context.WrappedDbContext.Set<T>();

        public IQueryable GetInstances(Type type) {
            var dbContext = context.WrappedDbContext;
            var mi = dbContext.GetType().GetMethod("Set", Array.Empty<Type>())?.MakeGenericMethod(type);
            return (IQueryable) mi?.Invoke(dbContext, Array.Empty<object>());
        }

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = TypeUtils.GetType(spec.FullName);
            return GetInstances(type);
        }

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class => (T) CreateInstance(typeof(T));

        private bool EFCoreKnowsType(Type type) {
            try {
                if (!CollectionUtils.IsCollection(type)) {
                    //return FindContext(type) != null;
                    return context.WrappedDbContext.HasEntityType(type);
                }
            }
            catch (Exception e) {
                // ignore all 
                Logger.LogWarning($"Ignoring exception {e.Message}");
            }

            return false;
        }

        
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

        private object GetObjectByKey(IEntityOid eoid, Type type) => context.WrappedDbContext.Find(type, eoid.Key);

        private object GetObjectByKey(IEntityOid eoid, IObjectSpec hint) => GetObjectByKey(eoid, TypeUtils.GetType(hint.FullName));

        public INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint)
        {
            switch (oid)
            {
                case IAggregateOid aggregateOid:
                {
                    var parentOid = (IEntityOid)aggregateOid.ParentOid;
                    var parentType = parentOid.TypeName;
                    var parentSpec = (IObjectSpec)metamodelManager.GetSpecification(parentType);
                    var parent = nakedObjectManager.CreateAdapter(GetObjectByKey(parentOid, parentSpec), parentOid, null);

                    return parentSpec.GetProperty(aggregateOid.FieldName).GetNakedObject(parent);
                }
                case IEntityOid eoid:
                {
                    var adapter = nakedObjectManager.CreateAdapter(GetObjectByKey(eoid, hint), eoid, null);
                    adapter.UpdateVersion(session, nakedObjectManager);
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

        public void Execute(IPersistenceCommand[] commands) {
            throw new NotImplementedException();
        }

        public void StartTransaction() { }

        public PropertyInfo[] GetKeys(Type type) => context.WrappedDbContext.SafeGetKeys(type);

        private void CopyTo(object toObj, object fromObj) {
            var toObjType = toObj.GetEFCoreProxiedType();
            var fromObjType = fromObj.GetEFCoreProxiedType();

            if (toObjType != fromObjType) {
                throw new PersistFailedException($"cannot copy different types {toObjType} and {fromObjType}");
            }

          

            var properties = context.WrappedDbContext.GetCloneableMembers(toObjType);

            properties.ForEach(pi =>  pi.SetValue(toObj, pi.GetValue(fromObj, null)));

        }


        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.GetFacet<IComplexTypeFacet>() is null)
            {
                nakedObjectAdapter.Updating();
                var toRefresh = nakedObjectAdapter.Object;
                var entity = context.WrappedDbContext.Entry(toRefresh);
                var keys = context.WrappedDbContext.GetKeyValues(toRefresh);
                entity.State = EntityState.Detached;
                var refreshed = context.WrappedDbContext.Find(toRefresh.GetEFCoreProxiedType(), keys);
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
            var obj = context.WrappedDbContext.Find(type, keys);

            var eoid = oidGenerator.CreateOid(type.FullName, keys);
            var adapter = nakedObjectManager.CreateAdapter(obj, eoid, null);
            adapter.UpdateVersion(session, nakedObjectManager);
            return adapter;
        }

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool isGhost) {
            // do nothing
        }

        public IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects) {
            FunctionalPostSave = objects.PostSaveFunction;
            return SetFunctionalProxyMap(ExecuteAttachObjectCommandUpdate(objects));
        }

        public bool HasChanges() => context.WrappedDbContext.ChangeTracker.HasChanges();

        public T ValidateProxy<T>(T toCheck) where T : class => toCheck;

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void Save() {
            context.WrappedDbContext.SaveChanges();
        }

        private bool PostSave() {
            FunctionalPostSave(functionalProxyMap);
            context.PostSave();
            return context.HasChanges();
        }

        private void PreSave() => context.PreSave();

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, "")));
            }

            PreSave();
            Save();
            if (PostSave()) {
                RecurseUntilAllChangesApplied(depth + 1);
            }
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

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter) {
            //var resolveEvent = !nakedObjectAdapter.ResolveState.IsTransient()
            //    ? Events.StartPartResolvingEvent
            //    : Events.StartResolvingEvent;
            var resolveEvent = Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter, DbContext context) {
            var resolveEvent = !nakedObjectAdapter.ResolveState.IsTransient()
                ? Events.StartPartResolvingEvent
                : Events.StartResolvingEvent;
            nakedObjectAdapter.ResolveState.Handle(resolveEvent);
        }

        private static void EndResolving(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ResolveState.Handle(nakedObjectAdapter.ResolveState.IsPartResolving() ? Events.EndPartResolvingEvent : Events.EndResolvingEvent);

        private static void Resolve(INakedObjectAdapter nakedObjectAdapter) {
            StartResolving(nakedObjectAdapter);
            EndResolving(nakedObjectAdapter);
        }

        private void MarkAsLoaded(INakedObjectAdapter nakedObjectAdapter) => context.LoadedNakedObjects.Add(nakedObjectAdapter);

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, DbContext context) {
            var keys = context.GetKeyValues(domainObject);
            if (keys.Any())
            {
                var oid = oidGenerator.CreateOid(EFCoreHelpers.GetEFCoreProxiedTypeName(domainObject), keys);
                var nakedObjectAdapter = nakedObjectManager.CreateAdapter(domainObject, oid, null);
                injector.InjectInto(nakedObjectAdapter.Object);
                nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

                if (nakedObjectAdapter.ResolveState.IsGhost())
                {
                    StartResolving(nakedObjectAdapter);
                    MarkAsLoaded(nakedObjectAdapter);
                }
            }
            else {
                // complex type ? 
            }
        }

        internal void HandleAdded(INakedObjectAdapter nakedObjectAdapter) {
            var oid = (IEntityOid) nakedObjectAdapter.Oid;
            oid.UpdateKey(context.WrappedDbContext.GetKeyValues(nakedObjectAdapter.Object));

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {
                Resolve(nakedObjectAdapter);
            }

            if (nakedObjectAdapter.Spec is IObjectSpec spec) {
                // testing check 
                var adapters = spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted).Select(a => a.GetNakedObject(nakedObjectAdapter));
                foreach (var adapter in adapters) {
                    if (adapter.ResolveState.IsGhost()) {
                        Resolve(adapter);
                    }
                }
            }

            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);
        }

        private static bool FieldIsPersisted(IAssociationSpec field) => !(field.ContainsFacet<INotPersistedFacet>() || field.ContainsFacet<IDisplayAsPropertyFacet>());

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && FieldIsPersisted(field)) {
                // check this is an EF collection 
                try {
                    return context.WrappedDbContext.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                }
                catch (ArgumentException) {
                    // not an EF recognized collection 
                    Logger.LogWarning($"Attempting to 'Count' a non-EF collection: {field.Id}");
                }
                catch (InvalidOperationException) {
                    // not an EF recognised entity 
                    Logger.LogWarning($"Attempting to 'Count' a non attached entity: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }
        
        public INakedObjectAdapter CreateAdapter(object obj) => nakedObjectManager.CreateAdapter(obj, null, null);

        public void ReplacePoco(INakedObjectAdapter nakedObject, object newDomainObject) => nakedObjectManager.ReplacePoco(nakedObject, newDomainObject);
        public void RemoveAdapter(INakedObjectAdapter nakedObject) => nakedObjectManager.RemoveAdapter(nakedObject);
        public INakedObjectAdapter CreateAggregatedAdapter(INakedObjectAdapter parent, PropertyInfo property, object obj) => nakedObjectManager.CreateAggregatedAdapter(parent, ((IObjectSpec) parent.Spec).GetProperty(property.Name).Id, obj);


        private IList<(object original, object updated)> SetFunctionalProxyMap(IList<(object original, object updated)> updatedTuples) {
            functionalProxyMap = updatedTuples.ToDictionary(t => t.original, t => t.updated);
            return updatedTuples;
        }

        private static IList<(object original, object updated)> Execute(EFCorePersistUpdateDetachedObjectCommand cmd) {
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

        private static void Execute(IPersistenceCommand cmd)
        {
            try
            {
                cmd.Execute();
            }
            catch (OptimisticConcurrencyException oce)
            {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce) { SourceNakedObjectAdapter = cmd.OnObject() };
            }
            catch (UpdateException ue)
            {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public IList<(object original, object updated)> ExecuteAttachObjectCommandUpdate(IDetachedObjects objects) =>
            Execute(new EFCorePersistUpdateDetachedObjectCommand(objects, this));

        public LocalContext GetContext(object o) => context;

        internal static bool EmptyKey(object key) =>
            key switch {
                // todo for all null keys
                string s => string.IsNullOrEmpty(s),
                int i => i == 0,
                null => true,
                _ => false
            };

        public bool EmptyKeys(object obj) => GetKeys(obj.GetType()).Select(k => k.GetValue(obj, null)).All(EmptyKey);

        public bool IsNotPersisted(object owner, PropertyInfo pi) {
            if (metamodelManager.GetSpecification(owner.GetEFCoreProxiedType()) is IObjectSpec objectSpec) {
                return objectSpec.Properties.SingleOrDefault(p => p.Id == pi.Name)?.ContainsFacet<INotPersistedFacet>() == true;
            }

            return false;
        }

        internal void HandleLoaded(INakedObjectAdapter nakedObjectAdapter) => EndResolving(nakedObjectAdapter);

    }
}