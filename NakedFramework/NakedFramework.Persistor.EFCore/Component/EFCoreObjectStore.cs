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
using NakedFramework.Persistor.Entity.Util;

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreObjectStore : IObjectStore, IDisposable {
        private readonly DbContext context;
        internal readonly ILogger<EFCoreObjectStore> Logger;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IOidGenerator oidGenerator;
        private readonly ISession session;

        public Func<IDictionary<object, object>, bool> FunctionalPostSave = _ => false;

        private IDictionary<object, object> functionalProxyMap = new Dictionary<object, object>();

        public EFCoreObjectStore(EFCorePersistorConfiguration config, IOidGenerator oidGenerator, INakedObjectManager nakedObjectManager, ISession session, ILogger<EFCoreObjectStore> logger) {
            this.oidGenerator = oidGenerator;
            this.nakedObjectManager = nakedObjectManager;
            this.session = session;
            Logger = logger;
            context = config.Context();
            MaximumCommitCycles = config.MaximumCommitCycles;

            context.ChangeTracker.StateChanged += (_, args) => {
                if (args.OldState == EntityState.Added) {
                    HandleAdded(args.Entry.Entity, context);
                }
            };

            context.ChangeTracker.Tracked += (_, args) => { LoadObjectIntoNakedObjectsFramework(args.Entry.Entity, context); };
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
            try {
                using var transaction = CreateTransactionScope();
                RecurseUntilAllChangesApplied(1);
                transaction.Complete();
            }
            catch (OptimisticConcurrencyException oce) {
                throw new ConcurrencyException(ConcatenateMessages(oce), oce);
            }
            catch (UpdateException ue) {
                throw new DataUpdateException(ConcatenateMessages(ue), ue);
            }
        }

        public IQueryable<T> GetInstances<T>(bool tracked = true) where T : class => context.Set<T>();

        public IQueryable GetInstances(Type type) {
            var mi = context.GetType().GetMethod("Set", Array.Empty<Type>())?.MakeGenericMethod(type);
            return (IQueryable) mi?.Invoke(context, Array.Empty<object>());
        }

        public IQueryable GetInstances(IObjectSpec spec) {
            var type = NakedObjects.TypeUtils.GetType(spec.FullName);
            return GetInstances(type);
        }

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

        public void StartTransaction() { }

        public PropertyInfo[] GetKeys(Type type) => context.SafeGetKeys(type);

        public void Refresh(INakedObjectAdapter nakedObjectAdapter) {
            throw new NotImplementedException();
        }

        public int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec) {
            var type = NakedObjects.TypeUtils.GetType(associationSpec.GetFacet<IElementTypeFacet>().ValueSpec.FullName);
            var countMethod = GetType().GetMethod("Count")?.GetGenericMethodDefinition().MakeGenericMethod(type);
            return (int) (countMethod?.Invoke(this, new object[] {nakedObjectAdapter, associationSpec, nakedObjectManager}) ?? 0);
        }

        public INakedObjectAdapter FindByKeys(Type type, object[] keys) {
            var obj = context.Find(type, keys);

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

        public bool HasChanges() => context.ChangeTracker.HasChanges();

        public T ValidateProxy<T>(T toCheck) where T : class => toCheck;

        private static TransactionScope CreateTransactionScope() {
            var transactionOptions = new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.MaxValue};
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }

        private void Save() {
            context.SaveChanges();
        }

        private bool PostSave() {
            FunctionalPostSave(functionalProxyMap);
            return context.ChangeTracker.HasChanges();
        }

        private void RecurseUntilAllChangesApplied(int depth) {
            if (depth > MaximumCommitCycles) {
                throw new NakedObjectDomainException(Logger.LogAndReturn(string.Format(NakedObjects.Resources.NakedObjects.EntityCommitError, "")));
            }

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

        private void LoadObjectIntoNakedObjectsFramework(object domainObject, DbContext context) {
            var oid = oidGenerator.CreateOid(EntityUtils.GetEntityProxiedTypeName(domainObject), context.GetKeyValues(domainObject));
            var nakedObjectAdapter = nakedObjectManager.CreateAdapter(domainObject, oid, null);
            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsGhost()) {
                StartResolving(nakedObjectAdapter);
            }
        }

        private void HandleAdded(object domainObject, DbContext context) {
            var nakedObjectAdapter = nakedObjectManager.CreateAdapter(domainObject, null, null);
            nakedObjectAdapter.UpdateVersion(session, nakedObjectManager);

            if (nakedObjectAdapter.ResolveState.IsNotPersistent()) {
                var eoid = (IEntityOid) nakedObjectAdapter.Oid;
                eoid.MakePersistentAndUpdateKey(context.GetKeyValues(domainObject));
                Resolve(nakedObjectAdapter);
            }
        }

        private static bool FieldIsPersisted(IAssociationSpec field) => !(field.ContainsFacet<INotPersistedFacet>() || field.ContainsFacet<IDisplayAsPropertyFacet>());

        // invoked reflectively; do not remove !
        public int Count<T>(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field, INakedObjectManager manager) where T : class {
            if (!nakedObjectAdapter.ResolveState.IsTransient() && FieldIsPersisted(field)) {
                // check this is an EF collection 
                try {
                    return context.Entry(nakedObjectAdapter.Object).Collection(field.Id).Query().Cast<T>().Count();
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

        public IList<(object original, object updated)> ExecuteAttachObjectCommandUpdate(IDetachedObjects objects) =>
            Execute(new EFCorePersistUpdateDetachedObjectCommand(objects, this));

        public DbContext GetContext(object o) => context;

        internal static bool EmptyKey(object key) =>
            key switch {
                // todo for all null keys
                string s => string.IsNullOrEmpty(s),
                int i => i == 0,
                null => true,
                _ => false
            };

        public bool EmptyKeys(object obj) => GetKeys(obj.GetType()).Select(k => k.GetValue(obj, null)).All(EmptyKey);
    }
}