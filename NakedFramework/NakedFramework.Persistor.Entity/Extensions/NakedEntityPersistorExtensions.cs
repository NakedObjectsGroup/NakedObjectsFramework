// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Persist;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.Entity.Component;
using NakedFramework.Persistor.Entity.Configuration;

namespace NakedFramework.Persistor.Entity.Extensions {
    public static class NakedEntityPersistorExtensions {
        private static EntityObjectStoreConfiguration EntityObjectStoreConfiguration(IConfiguration configuration, EntityPersistorOptions options) {
            var config = new EntityObjectStoreConfiguration {
                EnforceProxies = options.EnforceProxies,
                CustomConfig = options.CustomConfig,
                DefaultMergeOption = options.DefaultMergeOption,
                MaximumCommitCycles = options.MaximumCommitCycles,
                NotPersistedTypes = options.NotPersistedTypes,
                RollBackOnError = options.RollBackOnError,
                RequireExplicitAssociationOfTypes = options.RequireExplicitAssociationOfTypes
            };

            var contexts = options.ContextInstallers.Select<Func<IConfiguration, DbContext>, Func<DbContext>>(f => () => f(configuration));
            contexts.ForEach(c => config.UsingContext(c));
            return config;
        }

        public static void AddEF6Persistor(this NakedCoreOptions coreOptions, Action<EntityPersistorOptions> setupAction) {
            var options = new EntityPersistorOptions();
            setupAction(options);
            coreOptions.AdditionalSystemTypes = coreOptions.AdditionalSystemTypes.Append(typeof(ObjectQuery<>)).Append(typeof(EntityCollection<>)).ToArray();

            var unpersistedTypes = options.NotPersistedTypes();
            options.NotPersistedTypes = () => unpersistedTypes.Union(coreOptions.AdditionalUnpersistedTypes).ToArray();

            coreOptions.Services.AddSingleton<IEntityObjectStoreConfiguration>(p => EntityObjectStoreConfiguration(p.GetService<IConfiguration>(), options));
            coreOptions.Services.AddScoped<DatabaseOidGenerator, DatabaseOidGenerator>();
            coreOptions.Services.AddScoped<IOidGenerator, DatabaseOidGenerator>();
            coreOptions.Services.AddScoped<IObjectStore, EntityObjectStore>();
        }
    }
}