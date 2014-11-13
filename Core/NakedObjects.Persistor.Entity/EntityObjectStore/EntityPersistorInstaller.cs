// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Resources;

namespace NakedObjects.EntityObjectStore {
    public class EntityPersistorInstaller  {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityPersistorInstaller));

        //private bool isContextSet = false;

        public EntityPersistorInstaller() {
            EnforceProxies = true;
            RollBackOnError = false;
            DefaultMergeOption = MergeOption.AppendOnly;
            DbContextConstructors = new List<Tuple<Func<DbContext>, Func<Type[]>>>();
            NamedContextTypes = new Dictionary<string, Func<Type[]>>();
            NotPersistedTypes = () => new Type[] {};
            MaximumCommitCycles = 10;
            IsInitializedCheck = () => true;
        }


        protected IList<Tuple<Func<DbContext>, Func<Type[]>>> DbContextConstructors { get; set; }

        protected IDictionary<string, Func<Type[]>> NamedContextTypes { get; set; }
        protected Func<Type[]> NotPersistedTypes { get; set; }

        /// <summary>
        ///     Indicates that persistor will run in code first mode.
        ///     1. Any connection strings in App.Config will still be picked up for model first contexts
        ///     2. If CodeFirstConfig is NOT set then the entry assembly will be used
        /// </summary>
        protected bool CodeFirst { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that cannot be fully proxied.
        ///     This is true by default.
        /// </summary>
        public bool EnforceProxies { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that has not been associated via
        ///     SpecifyTypesNotAssociatedWithAnyContext or AssociateTypes; 
        ///     This is false by default;
        /// </summary>
        public bool RequireExplicitAssociationOfTypes { get; set; }

        /// <summary>
        ///     If set the persistor will rollback the context on update or concurrency errors
        /// </summary>
        public bool RollBackOnError { get; set; }

        /// <summary>
        ///     Set the MergeOption to be used by default in all contexts and all queries
        /// </summary>
        public MergeOption DefaultMergeOption { get; set; }


        /// <summary>
        ///     Persistor will loop over contexts commit changes a maximum of this number of times. If exceeded an exception will be thrown.
        ///     This is to catch recursive changes triggered from Persisted or Updated that loop forever.
        ///     Default is 10
        /// </summary>
        public int MaximumCommitCycles { get; set; }

        public  string Name {
            get { return "entity"; }
        }

        public Func<bool> IsInitializedCheck { get; set; }

        /// <summary>
        ///     Registering a type here will instruct the framework not to search for the type in the DbContext(s)
        ///     and thus slightly improve efficiency. Typically used for abstract types that provide behaviour
        ///     to one or more entity types.
        /// </summary>
        /// <param name="types">A lambda or delegate that returns an array of Types</param>
        public void SpecifyTypesNotAssociatedWithAnyContext(Func<Type[]> types) {
            NotPersistedTypes = types;
        }


        /// <summary>
        ///     Call for each code first context in solution.
        /// </summary>
        /// <param name="f">A lambda or delegate that returns a newly constructed DbContext </param>
        /// <returns>A ContextInstaller that allows further configuration.</returns>
        /// <example>UsingCodeFirstContext( () => new MyDbContext())</example>
        public ContextInstaller UsingCodeFirstContext(Func<DbContext> f) {
            //isContextSet = true;
            CodeFirst = true;
            return new ContextInstaller(this, f);
        }

        /// <summary>
        ///     Call for each .edmx (ie Model First or Database First) context in solution. Warns if context name does not exist if Web.Config
        ///     or if contexts in Web.Config are not identified by this method.
        /// </summary>
        /// <param name="name">Name of context in Web.Config file</param>
        /// <returns>A ContextInstaller that allows further configuration.</returns>
        /// <example>UsingEdmxContext("Model1")</example>
        public ContextInstaller UsingEdmxContext(string name) {
           // isContextSet = true;
            return new ContextInstaller(this, name);
        }

        // for testing
        public void ForceContextSet() {
           // isContextSet = true;
        }

        public  ILifecycleManager CreateObjectPersistor() {
            //if (!isContextSet) {
            //    throw new InitialisationException(@"No context set on EntityPersistorInstaller, must call either ""UsingCodeFirstContext"" or ""UsingEdmxContext""");
            //}

            //IEnumerable<CodeFirstEntityContextConfiguration> cfConfigs = DbContextConstructors.Select(f => new CodeFirstEntityContextConfiguration {DbContext = f.Item1, PreCachedTypes = f.Item2, NotPersistedTypes = NotPersistedTypes});
            //IEnumerable<EntityContextConfiguration> config = PocoConfiguration().Union(cfConfigs);
            //var reflector = NakedObjectsContext.Reflector;
            //var oidGenerator = new EntityOidGenerator(reflector);
            //var cfg = new EntityObjectStoreConfiguration();// {ContextConfiguration = config.ToArray()};
            //var objectStore = new EntityObjectStore(cfg, oidGenerator, reflector);
            ////EntityObjectStore.EnforceProxies = EnforceProxies;
            ////EntityObjectStore.RequireExplicitAssociationOfTypes = RequireExplicitAssociationOfTypes;
            ////EntityObjectStore.RollBackOnError = RollBackOnError;
            ////EntityObjectStore.MaximumCommitCycles = MaximumCommitCycles;
            ////EntityObjectStore.IsInitializedCheck = IsInitializedCheck;

            //var identityMap = new EntityIdentityMapImpl(oidGenerator, identityAdapterMap ?? new IdentityAdapterHashMap(), pocoAdapterMap ?? new PocoAdapterHashMap(), objectStore);

            //var op = new LifeCycleManager(
            //    reflector,             
            //    objectStore, 
            //    new EntityPersistAlgorithm(),
            //    oidGenerator, 
            //    identityMap);

            //objectStore.Manager = op;

            //return op;
            return null;
        }

        protected IEnumerable<EntityContextConfiguration> PocoConfiguration() {
            string[] connectionStringNames = GetConnectionStringNamesFromConfig();

            FlagConnectionStringMismatches(connectionStringNames);

            if (connectionStringNames.Any()) {
                Dictionary<string, Func<Type[]>> defaultedData = connectionStringNames.ToDictionary(s => s, s => NamedContextTypes.ContainsKey(s) ? NamedContextTypes[s] : () => new Type[] {});

                return connectionStringNames.Select(s => new PocoEntityContextConfiguration {
                    DefaultMergeOption = DefaultMergeOption,
                    ContextName = s,
                    PreCachedTypes = defaultedData[s],
                    NotPersistedTypes = NotPersistedTypes
                });
            }
            return new EntityContextConfiguration[] {};
        }

        private void FlagConnectionStringMismatches(string[] connectionStringNames) {
            ICollection<string> configuredContextNames = NamedContextTypes.Keys;

            IEnumerable<string> configuredButNotUsed = configuredContextNames.Where(s => !connectionStringNames.Contains(s));
            IEnumerable<string> usedButNotConfigured = connectionStringNames.Where(s => !configuredContextNames.Contains(s));

            configuredButNotUsed.ForEach(s => Log.WarnFormat(Model.ContextNameMismatch1, s));
            usedButNotConfigured.ForEach(s => Log.WarnFormat(Model.ContextNameMismatch2, s));
        }


        private string[] GetConnectionStringNamesFromConfig() {
            ConnectionStringSettings[] connectionStrings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Where(x => x.ProviderName == "System.Data.EntityClient").ToArray();

            if (!connectionStrings.Any() && !CodeFirst) {
                throw new InitialisationException(Resources.NakedObjects.NoConnectionString);
            }

            return connectionStrings.Select(cs => cs.Name).ToArray();
        }

        public IReflector SetupReflector(IReflector reflector) {
            return reflector;
        }

        /// <summary>
        ///     A wrapper for an entity context that offers configuration options in relation to that context.
        /// </summary>
        public class ContextInstaller {
            private readonly int contextIndex;
            private readonly string contextName;
            private readonly EntityPersistorInstaller persistorInstaller;

            private ContextInstaller(EntityPersistorInstaller persistorInstaller) {
                this.persistorInstaller = persistorInstaller;
            }

            public ContextInstaller(EntityPersistorInstaller persistorInstaller, Func<DbContext> f) : this(persistorInstaller) {
                persistorInstaller.DbContextConstructors.Add(new Tuple<Func<DbContext>, Func<Type[]>>(f, () => new Type[] {}));
                contextIndex = persistorInstaller.DbContextConstructors.Count() - 1;
            }

            public ContextInstaller(EntityPersistorInstaller persistorInstaller, string contextName) : this(persistorInstaller) {
                this.contextName = contextName;

                if (!persistorInstaller.NamedContextTypes.ContainsKey(contextName)) {
                    persistorInstaller.NamedContextTypes.Add(contextName, () => new Type[] {});
                }
            }

            /// <summary>
            ///     Associates each of the array of Types passed-in with the context, and caches this information on the
            ///     session.  This is to avoid the overhead of the framework polling contexts to see if they known
            ///     about a given type.
            /// </summary>
            /// <param name="types">A lambda or delegate that returns an array of Types</param>
            /// <returns>The ContextInstaller on which it was called, allowing further configration.</returns>
            public ContextInstaller AssociateTypes(Func<Type[]> types) {
                if (string.IsNullOrEmpty(contextName)) {
                    Tuple<Func<DbContext>, Func<Type[]>> entry = persistorInstaller.DbContextConstructors[contextIndex];
                    persistorInstaller.DbContextConstructors[contextIndex] = new Tuple<Func<DbContext>, Func<Type[]>>(entry.Item1, types);
                }
                else {
                    persistorInstaller.NamedContextTypes[contextName] = types;
                }

                return this;
            }
        }
    }
}