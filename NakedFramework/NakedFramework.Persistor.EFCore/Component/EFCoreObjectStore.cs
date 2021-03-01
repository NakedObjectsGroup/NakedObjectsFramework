using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Persist;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedFramework.Persistor.EFCore.Util;
using NakedFramework.Persistor.Entity.Util;

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreObjectStore : IObjectStore, IDisposable {
        private readonly IOidGenerator oidGenerator;
        private readonly INakedObjectManager nakedObjectManager;
        private DbContext context;


        public EFCoreObjectStore(EFCorePersistorConfiguration config, IOidGenerator oidGenerator, INakedObjectManager nakedObjectManager) {
            this.oidGenerator = oidGenerator;
            this.nakedObjectManager = nakedObjectManager;
            context = config.Context();


            context.ChangeTracker.StateChanged += (_, args) => {
                if (args.OldState == EntityState.Added) {
                    HandleAdded(args.Entry.Entity, context);
                }
            };

            context.ChangeTracker.Tracked += (_, args) => { 
                LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context);
            };

        }

        public void Dispose() {
            context.Dispose();
        }

        public bool IsInitialized { get; }
        public string Name { get; }

        public void AbortTransaction() {
            throw new NotImplementedException();
        }

        public void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        private static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }


        private void Save() {
            context.SaveChanges();
            
        }

        private bool PostSave()
        {
            var changes = FunctionalPostSave(functionalProxyMap);

            // two separate loops as PostSave may have side-affects in previously processed contexts
            //contexts.Values.ForEach(c => c.PostSave(this));
            //return contexts.Values.Aggregate(false, (current, c) => current || c.HasChanges());

            return context.ChangeTracker.HasChanges();
        }


        private void RecurseUntilAllChangesApplied(int depth)
        {
            if (depth > 10)
            {
                //var typeNames = contexts.Values.SelectMany(c => c.WrappedObjectContext.ObjectStateManager.GetObjectStateEntries(System.Data.Entity.EntityState.Added | System.Data.Entity.EntityState.Modified).Where(o => o.Entity != null).Select(o => o.Entity.GetEntityProxiedType().FullName)).Aggregate("", (s, t) => s + (string.IsNullOrEmpty(s) ? "" : ", ") + t);

                //throw new NakedObjectDomainException(logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, typeNames)));
                throw new Exception("looping");
            }

            //PreSave();
            Save();
            if (PostSave())
            {
                RecurseUntilAllChangesApplied(depth + 1);
            }
        }

        public void EndTransaction() {
            //try
            //{
                using (var transaction = CreateTransactionScope())
                {
                    RecurseUntilAllChangesApplied(1);
                    transaction.Complete();
                }

                // PostSaveWrapUp();
            //}
            //catch (OptimisticConcurrencyException oce)
            //{
            //  //  InvokeErrorFacet(new ConcurrencyException(ConcatenateMessages(oce), oce) { SourceNakedObjectAdapter = GetSourceNakedObject(oce) });
            //}
            //catch (UpdateException ue)
            //{
            //    //InvokeErrorFacet(new DataUpdateException(ConcatenateMessages(ue), ue));
            //}
            //catch (Exception )
            //{
            //    //logger.LogError($"Unexpected exception while applying changes {e.Message}");
            //    //RollBackContext();
            //    throw;
            //}
            //finally
            //{
            //   // contexts.Values.ForEach(c => c.SaveOrUpdateComplete());
            //}
        }

        private static void StartResolving(INakedObjectAdapter nakedObjectAdapter) {
            var resolveEvent = !nakedObjectAdapter.ResolveState.IsTransient()
                ? Events.StartPartResolvingEvent
                : Events.StartResolvingEvent;
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

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, DbContext context)
        {
            //CheckProxies(domainObject);
            var oid = oidGenerator.CreateOid(EntityUtils.GetEntityProxiedTypeName(domainObject), context.GetKeyValues(domainObject));
            var nakedObjectAdapter = nakedObjectManager.CreateAdapter(domainObject, oid, null);
           // injector.InjectInto(nakedObjectAdapter.Object);
            //loadComplexTypesIntoNakedObjectFramework(nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsGhost());
            //nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsGhost())
            {
                StartResolving(nakedObjectAdapter);
                //MarkAsLoaded(nakedObjectAdapter);
            }
        }


        private void HandleAdded(object domainObject, DbContext context) {
            //CheckProxies(domainObject);
            //var oid = oidGenerator.CreateOid(EntityUtils.GetEntityProxiedTypeName(domainObject), context.GetKeyValues(domainObject));
            var nakedObjectAdapter = nakedObjectManager.CreateAdapter(domainObject, null, null);
            //injector.InjectInto(nakedObjectAdapter.Object);
            //LoadComplexTypesIntoNakedObjectFramework(nakedObjectAdapter, nakedObjectAdapter.ResolveState.IsGhost());
            //nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {

                var eoid = (IEntityOid)nakedObjectAdapter.Oid;

                eoid.MakePersistentAndUpdateKey(context.GetKeyValues(domainObject));

                Resolve(nakedObjectAdapter);
            }


            //if (nakedObjectAdapter.ResolveState.IsGhost())
            //{
            //    StartResolving(nakedObjectAdapter);
            //   // MarkAsLoaded(nakedObjectAdapter);
            //}
        }

        //private void HandleAdded(INakedObjectAdapter nakedObjectAdapter)
        //{
        //    var oid = (IEntityOid)nakedObjectAdapter.Oid;
        //    var context = GetContext(nakedObjectAdapter);
        //    oid.MakePersistentAndUpdateKey(context.GetKey(nakedObjectAdapter));

        //    if (nakedObjectAdapter.ResolveState.IsNotPersistent())
        //    {
        //        Resolve(nakedObjectAdapter, context);
        //    }

        //    if (nakedObjectAdapter.Spec is IObjectSpec spec)
        //    {
        //        // testing check 
        //        var adapters = spec.Properties.OfType<IOneToManyAssociationSpec>().Where(a => a.IsPersisted).Select(a => a.GetNakedObject(nakedObjectAdapter));
        //        foreach (var adapter in adapters)
        //        {
        //            if (adapter.ResolveState.IsGhost())
        //            {
        //                Resolve(adapter, GetContext(adapter));
        //            }
        //        }
        //    }

        //    //nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);
        //}



        public IQueryable<T> GetInstances<T>(bool tracked = true) where T : class => context.Set<T>();

        public IQueryable GetInstances(Type type) => throw new NotImplementedException();

        public IQueryable GetInstances(IObjectSpec spec) => throw new NotImplementedException();

        public T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class => throw new NotImplementedException();

        public object CreateInstance(Type type) => throw new NotImplementedException();

        public INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint) => throw new NotImplementedException();

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

        public void StartTransaction() {
            // throw new NotImplementedException();
        }

        public PropertyInfo[] GetKeys(Type type) {
            return context.GetKeys(type);
        }

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        private static bool FieldIsPersisted(IAssociationSpec field) => !(field.ContainsFacet<INotPersistedFacet>() || field.ContainsFacet<IDisplayAsPropertyFacet>());

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class
        {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && FieldIsPersisted(field))
            {
               
                // check this is an EF collection 
                try
                {
                    return context.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
                }
                catch (ArgumentException)
                {
                    // not an EF recognized collection 
                   // logger.LogWarning($"Attempting to 'Count' a non-EF collection: {field.Id}");
                }
                catch (InvalidOperationException)
                {
                    // not an EF recognised entity 
                  //  logger.LogWarning($"Attempting to 'Count' a non attached entity: {field.Id}");
                }
            }

            return field.GetNakedObject(nakedObjectAdapter).GetAsEnumerable(manager).Count();
        }



        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {

            var type = NakedObjects.TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
            var countMethod = GetType().GetMethod("Count")?.GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int)(countMethod?.Invoke(this, new object[] { nakedObjectAdapter, associationSpec, nakedObjectManager }) ?? 0);
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {

            var obj = context.Find(type, keys);

            var eoid = oidGenerator.CreateOid(type.FullName, keys);
            var adapter = this.nakedObjectManager.CreateAdapter(obj, eoid, null);
            //adapter.UpdateVersion(session, nakedObjectManager);
            return adapter;

        }

        public INakedObjectAdapter CreateAdapter(object obj) {
            return nakedObjectManager.CreateAdapter(obj, null, null);
        }

        public void RemoveAdapter(INakedObjectAdapter  adapter)
        {
            //return this.nakedObjectManager.CreateAdapter(obj, eoid, null);
        }

        public void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool isGhost) {
           // throw new NotImplementedException();
        }

        public Func<IDictionary<object, object>, bool> FunctionalPostSave = _ => false;

        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();

        private IList<(object original, object updated)> SetFunctionalProxyMap(IList<(object original, object updated)> updatedTuples)
        {
            functionalProxyMap = updatedTuples.ToDictionary(t => t.original, t => t.updated);
            return updatedTuples;
        }

        private static IList<(object original, object updated)> Execute(EFCorePersistUpdateDetachedObjectCommand cmd)
        {
            try
            {
                return cmd.Execute();
            }
            catch (OptimisticConcurrencyException )
            {
                //throw new ConcurrencyException(ConcatenateMessages(oce), oce) { SourceNakedObjectAdapter = cmd.OnObject() };
                throw;
            }
            catch (UpdateException )
            {
               // throw new DataUpdateException(ConcatenateMessages(ue), ue);
               throw;
            }
            
        }


        public IList<(object original, object updated)> ExecuteAttachObjectCommandUpdate(IDetachedObjects objects) =>
            Execute(new EFCorePersistUpdateDetachedObjectCommand(objects, this));

        public IList<(object original, object updated)> UpdateDetachedObjects(IDetachedObjects objects) {
            FunctionalPostSave = objects.PostSaveFunction;
            return SetFunctionalProxyMap(ExecuteAttachObjectCommandUpdate(objects));


        }

        public bool HasChanges() => context.ChangeTracker.HasChanges();

        public T ValidateProxy<T>(T toCheck) where T : class => throw new NotImplementedException();

        public DbContext GetContext(object o) => context;

        internal static bool EmptyKey(object key) =>
            key switch
            {
                // todo for all null keys
                string s => string.IsNullOrEmpty(s),
                int i => i == 0,
                null => true,
                _ => false
            };

        public bool EmptyKeys(object obj) => GetKeys(obj.GetType()).Select(k => k.GetValue(obj, null)).All(EmptyKey);
    }
}