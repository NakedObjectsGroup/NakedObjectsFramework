// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Common.Logging;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Resources;

namespace NakedObjects.Persistor.Entity.Configuration {
    public class EntityObjectStoreConfiguration : IEntityObjectStoreConfiguration {
        private static readonly ILog Log = LogManager.GetLogger(typeof(EntityObjectStoreConfiguration));
        private bool isContextSet;

        public EntityObjectStoreConfiguration() {
            EnforceProxies = true;
            RollBackOnError = false;
            DefaultMergeOption = MergeOption.AppendOnly;
            DbContextConstructors = new List<Tuple<Func<DbContext>, Func<Type[]>>>();
            NamedContextTypes = new Dictionary<string, Func<Type[]>>();
            NotPersistedTypes = () => new Type[] { };
            MaximumCommitCycles = 10;
            IsInitializedCheck = () => true;
            CustomConfig = oc => { };
        }

        public static bool NoValidate { get; set; }

        public Action<ObjectContext> CustomConfig { get; set; }

        #region IEntityObjectStoreConfiguration Members

        public IEnumerable<CodeFirstEntityContextConfiguration> ContextConfiguration {
            get {
                IEnumerable<CodeFirstEntityContextConfiguration> cfConfigs = DbContextConstructors.Select(f => new CodeFirstEntityContextConfiguration {
                    DbContext = f.Item1,
                    PreCachedTypes = f.Item2,
                    NotPersistedTypes = NotPersistedTypes,
                    CustomConfig = CustomConfig,
                    DefaultMergeOption = DefaultMergeOption
                });
                return cfConfigs;
            }
            set {
                // leave for moment for compiling
            }
        }

        public IList<Tuple<Func<DbContext>, Func<Type[]>>> DbContextConstructors { get; set; }
        public IDictionary<string, Func<Type[]>> NamedContextTypes { get; set; }
        public Func<Type[]> NotPersistedTypes { get; set; }

        [Obsolete("No longer used as always code first")]
        public bool CodeFirst {
            get { return true; }
            set { }
        }

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
        ///     Persistor will loop over contexts commit changes a maximum of this number of times. If exceeded an exception will
        ///     be thrown.
        ///     This is to catch recursive changes triggered from Persisted or Updated that loop forever.
        ///     Default is 10
        /// </summary>
        public int MaximumCommitCycles { get; set; }

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
        public EntityContextConfigurator UsingCodeFirstContext(Func<DbContext> f) {
            isContextSet = true;
            return new EntityContextConfigurator(this, f);
        }

        [Obsolete("Use Code First")]
        public EntityContextConfigurator UsingEdmxContext(string name) {
            throw new NotImplementedException(Log.LogAndReturn("edmx configuration of EF no longer supported - use code first"));
        }

        // for testing
        public void ForceContextSet() {
            isContextSet = true;
        }

        [Obsolete("No longer used")]
        public IEnumerable<EntityContextConfiguration> PocoConfiguration() {
            throw new NotImplementedException(Log.LogAndReturn("edmx configuration of EF no longer supported - use code first"));
        }

        public void FlagConnectionStringMismatches(string[] connectionStringNames) {
            ICollection<string> configuredContextNames = NamedContextTypes.Keys;

            IEnumerable<string> configuredButNotUsed = configuredContextNames.Where(s => !connectionStringNames.Contains(s));
            IEnumerable<string> usedButNotConfigured = connectionStringNames.Where(s => !configuredContextNames.Contains(s));

            configuredButNotUsed.ForEach(s => Log.WarnFormat(Model.ContextNameMismatch1, s));
            usedButNotConfigured.ForEach(s => Log.WarnFormat(Model.ContextNameMismatch2, s));
        }

        public string[] GetConnectionStringNamesFromConfig() {
            ConnectionStringSettings[] connectionStrings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().Where(x => x.ProviderName == "System.Data.EntityClient").ToArray();

            return connectionStrings.Select(cs => cs.Name).ToArray();
        }

        public void AssertSetup() {
            if (!NoValidate && !isContextSet) {
                throw new InitialisationException(Log.LogAndReturn(@"No context set on EntityObjectStoreConfiguration, must call ""UsingCodeFirstContext"""));
            }
        }

        #endregion

        #region Nested type: EntityContextConfigurator

        public class EntityContextConfigurator {
            private readonly int contextIndex;
            private readonly string contextName;
            private readonly EntityObjectStoreConfiguration entityObjectStoreConfiguration;

            private EntityContextConfigurator(EntityObjectStoreConfiguration entityObjectStoreConfiguration) {
                this.entityObjectStoreConfiguration = entityObjectStoreConfiguration;
            }

            public EntityContextConfigurator(EntityObjectStoreConfiguration entityObjectStoreConfiguration, Func<DbContext> f)
                : this(entityObjectStoreConfiguration) {
                entityObjectStoreConfiguration.DbContextConstructors.Add(new Tuple<Func<DbContext>, Func<Type[]>>(f, () => new Type[] { }));
                contextIndex = entityObjectStoreConfiguration.DbContextConstructors.Count() - 1;
            }

            public EntityContextConfigurator(EntityObjectStoreConfiguration entityObjectStoreConfiguration, string contextName)
                : this(entityObjectStoreConfiguration) {
                this.contextName = contextName;

                if (!entityObjectStoreConfiguration.NamedContextTypes.ContainsKey(contextName)) {
                    entityObjectStoreConfiguration.NamedContextTypes.Add(contextName, () => new Type[] { });
                }
            }

            /// <summary>
            ///     Associates each of the array of Types passed-in with the context, and caches this information on the
            ///     session.  This is to avoid the overhead of the framework polling contexts to see if they known
            ///     about a given type.
            /// </summary>
            /// <param name="types">A lambda or delegate that returns an array of Types</param>
            /// <returns>The ContextInstaller on which it was called, allowing further configration.</returns>
            public EntityContextConfigurator AssociateTypes(Func<Type[]> types) {
                if (string.IsNullOrEmpty(contextName)) {
                    Tuple<Func<DbContext>, Func<Type[]>> entry = entityObjectStoreConfiguration.DbContextConstructors[contextIndex];
                    entityObjectStoreConfiguration.DbContextConstructors[contextIndex] = new Tuple<Func<DbContext>, Func<Type[]>>(entry.Item1, types);
                }
                else {
                    entityObjectStoreConfiguration.NamedContextTypes[contextName] = types;
                }

                return this;
            }

            public EntityContextConfigurator SetCustomConfiguration(Action<ObjectContext> customConfig) {
                entityObjectStoreConfiguration.CustomConfig = customConfig;
                return this;
            }
        }

        #endregion
    }
}