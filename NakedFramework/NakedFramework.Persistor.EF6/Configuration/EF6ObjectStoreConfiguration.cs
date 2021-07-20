// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using NakedFramework.Core.Error;

namespace NakedFramework.Persistor.EF6.Configuration {
    public class EF6ObjectStoreConfiguration : IEF6ObjectStoreConfiguration {
        private bool isContextSet;

        public EF6ObjectStoreConfiguration() {
            EnforceProxies = true;
            RollBackOnError = false;
            DefaultMergeOption = MergeOption.AppendOnly;
            DbContextConstructors = new List<(Func<DbContext>, Func<Type[]>)>();
            NotPersistedTypes = Array.Empty<Type>;
            MaximumCommitCycles = 10;
            IsInitializedCheck = () => true;
            CustomConfig = oc => { };
        }

        public static bool NoValidate { get; set; }

        public Action<ObjectContext> CustomConfig { get; set; }

        #region Nested type: EntityContextConfigurator

        public class EntityContextConfigurator {
            private readonly int contextIndex;
            private readonly EF6ObjectStoreConfiguration ef6ObjectStoreConfiguration;

            private EntityContextConfigurator(EF6ObjectStoreConfiguration ef6ObjectStoreConfiguration) => this.ef6ObjectStoreConfiguration = ef6ObjectStoreConfiguration;

            public EntityContextConfigurator(EF6ObjectStoreConfiguration ef6ObjectStoreConfiguration, Func<DbContext> f)
                : this(ef6ObjectStoreConfiguration) {
                ef6ObjectStoreConfiguration.DbContextConstructors.Add((f, Array.Empty<Type>));
                contextIndex = ef6ObjectStoreConfiguration.DbContextConstructors.Count - 1;
            }

            /// <summary>
            ///     Associates each of the array of Types passed-in with the context, and caches this information on the
            ///     session.  This is to avoid the overhead of the framework polling contexts to see if they known
            ///     about a given type.
            /// </summary>
            /// <param name="types">A lambda or delegate that returns an array of Types</param>
            /// <returns>The ContextInstaller on which it was called, allowing further configuration.</returns>
            public EntityContextConfigurator AssociateTypes(Func<Type[]> types) {
                var (getContexts, _) = ef6ObjectStoreConfiguration.DbContextConstructors[contextIndex];
                ef6ObjectStoreConfiguration.DbContextConstructors[contextIndex] = (getContexts, types);

                return this;
            }

            public EntityContextConfigurator SetCustomConfiguration(Action<ObjectContext> customConfig) {
                ef6ObjectStoreConfiguration.CustomConfig = customConfig;
                return this;
            }
        }

        #endregion

        #region IEF6ObjectStoreConfiguration Members

        public IEnumerable<EF6ContextConfiguration> ContextConfiguration =>
            DbContextConstructors.Select(f => new EF6ContextConfiguration {
                DbContext = f.getContexts,
                PreCachedTypes = f.getTypes,
                NotPersistedTypes = NotPersistedTypes,
                CustomConfig = CustomConfig,
                DefaultMergeOption = DefaultMergeOption
            });

        public IList<(Func<DbContext> getContexts, Func<Type[]> getTypes)> DbContextConstructors { get; set; }
        public Func<Type[]> NotPersistedTypes { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that cannot be fully proxied.
        ///     This is true by default.
        /// </summary>
        public bool EnforceProxies { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that has not been associated via
        ///     SpecifyTypesNotAssociatedWithAnyContext or AssociateTypes
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
        public void SpecifyTypesNotAssociatedWithAnyContext(Func<Type[]> types) => NotPersistedTypes = types;

        [Obsolete]
        public EntityContextConfigurator UsingCodeFirstContext(Func<DbContext> f) => UsingContext(f);

        /// <summary>
        ///     Call for each context in solution.
        /// </summary>
        /// <param name="f">A lambda or delegate that returns a newly constructed DbContext </param>
        /// <returns>A ContextInstaller that allows further configuration.</returns>
        /// <example>UsingContext( () => new MyDbContext())</example>
        public EntityContextConfigurator UsingContext(Func<DbContext> f) {
            isContextSet = true;
            return new EntityContextConfigurator(this, f);
        }

        // for testing
        public void ForceContextSet() => isContextSet = true;

        [Obsolete("Use Validate")]
        public void AssertSetup() => Validate();

        public void Validate() {
            if (!NoValidate && !isContextSet) {
                throw new InitialisationException(@"No context set on EF6ObjectStoreConfiguration, must call ""UsingContext""");
            }
        }

        #endregion
    }
}