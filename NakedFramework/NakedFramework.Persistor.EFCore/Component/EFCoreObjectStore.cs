using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NakedFramework.Architecture.Persist;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreObjectStore : IObjectStore, IDisposable {
        private readonly IOidGenerator oidGenerator;
        private readonly INakedObjectManager nakedObjectManager;
        private DbContext context;


        public EFCoreObjectStore(EFCorePersistorConfiguration config, IOidGenerator oidGenerator, INakedObjectManager nakedObjectManager) {
            this.oidGenerator = oidGenerator;
            this.nakedObjectManager = nakedObjectManager;
            context = config.Context();
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

        public void EndTransaction() {
            //throw new NotImplementedException();
        }

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
            // temp hack 

            return type.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() is not null).ToArray();

        }

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) => throw new NotImplementedException();

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {

            var obj = context.Find(type, keys);

            var eoid = oidGenerator.CreateOid(type.FullName, keys);
            var adapter = this.nakedObjectManager.CreateAdapter(obj, eoid, null);
            //adapter.UpdateVersion(session, nakedObjectManager);
            return adapter;

        }

        public INakedObjectAdapter CreateAdapter(object obj) {
            return this.nakedObjectManager.CreateAdapter(obj, null, null);
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