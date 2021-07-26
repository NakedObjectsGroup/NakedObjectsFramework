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

namespace NakedFramework.Persistor.EF6.Configuration {
    public interface IEF6ObjectStoreConfiguration {
        IEnumerable<EF6ContextConfiguration> ContextConfiguration { get; }
        IList<(Func<DbContext> getContexts, Func<Type[]> getTypes)> DbContextConstructors { get; set; }
        Func<Type[]> NotPersistedTypes { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that cannot be fully proxied.
        ///     This is true by default.
        /// </summary>
        bool EnforceProxies { get; set; }

        /// <summary>
        ///     If set the persistor will throw an exception if any type is seen that has not been associated via
        ///     SpecifyTypesNotAssociatedWithAnyContext or AssociateTypes
        ///     This is false by default
        /// </summary>
        bool RequireExplicitAssociationOfTypes { get; set; }

        /// <summary>
        ///     If set the persistor will rollback the context on update or concurrency errors
        /// </summary>
        bool RollBackOnError { get; set; }

        /// <summary>
        ///     Set the MergeOption to be used by default in all contexts and all queries
        /// </summary>
        MergeOption DefaultMergeOption { get; set; }

        /// <summary>
        ///     Persistor will loop over contexts commit changes a maximum of this number of times. If exceeded an exception will
        ///     be thrown.
        ///     This is to catch recursive changes triggered from Persisted or Updated that loop forever.
        ///     Default is 10
        /// </summary>
        int MaximumCommitCycles { get; set; }

        Func<bool> IsInitializedCheck { get; set; }

        /// <summary>
        ///     Registering a type here will instruct the framework not to search for the type in the DbContext(s)
        ///     and thus slightly improve efficiency. Typically used for abstract types that provide behaviour
        ///     to one or more entity types.
        /// </summary>
        /// <param name="types">A lambda or delegate that returns an array of Types</param>
        void SpecifyTypesNotAssociatedWithAnyContext(Func<Type[]> types);

        [Obsolete("use UsingContext")]
        EF6ObjectStoreConfiguration.EntityContextConfigurator UsingCodeFirstContext(Func<DbContext> f);

        /// <summary>
        ///     Call for each  context in solution.
        /// </summary>
        /// <param name="f">A lambda or delegate that returns a newly constructed DbContext </param>
        /// <returns>A ContextInstaller that allows further configuration.</returns>
        /// <example>UsingCodeFirstContext( () => new MyDbContext())</example>
        EF6ObjectStoreConfiguration.EntityContextConfigurator UsingContext(Func<DbContext> f);

        void ForceContextSet();

        [Obsolete("Use Validate")]
        void AssertSetup();

        /// <summary>
        ///     Throws if config not valid
        /// </summary>
        void Validate();
    }
}