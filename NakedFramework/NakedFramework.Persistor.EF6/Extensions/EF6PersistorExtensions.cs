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
using NakedFramework.Persistor.EF6.Component;
using NakedFramework.Persistor.EF6.Configuration;

namespace NakedFramework.Persistor.EF6.Extensions; 

public static class EF6PersistorExtensions {
    private static EF6ObjectStoreConfiguration EF6ObjectStoreConfiguration(IConfiguration configuration, EF6PersistorOptions options) {
        var config = new EF6ObjectStoreConfiguration {
            EnforceProxies = options.EnforceProxies,
            CustomConfig = options.CustomConfig,
            DefaultMergeOption = options.DefaultMergeOption,
            MaximumCommitCycles = options.MaximumCommitCycles,
            NotPersistedTypes = options.NotPersistedTypes,
            RollBackOnError = options.RollBackOnError,
            RequireExplicitAssociationOfTypes = options.RequireExplicitAssociationOfTypes
        };

        var contexts = options.ContextCreators.Select<Func<IConfiguration, DbContext>, Func<DbContext>>(f => () => f(configuration));
        contexts.ForEach(c => config.UsingContext(c));
        return config;
    }

    public static void AddEF6Persistor(this NakedFrameworkOptions frameworkOptions, Action<EF6PersistorOptions> setupAction) {
        var options = new EF6PersistorOptions();
        setupAction(options);
        frameworkOptions.AdditionalSystemTypes = frameworkOptions.AdditionalSystemTypes.Append(typeof(ObjectQuery<>)).Append(typeof(EntityCollection<>)).ToArray();

        var unpersistedTypes = options.NotPersistedTypes();
        options.NotPersistedTypes = () => unpersistedTypes.Union(frameworkOptions.AdditionalUnpersistedTypes).ToArray();

        frameworkOptions.Services.AddSingleton<IEF6ObjectStoreConfiguration>(p => EF6ObjectStoreConfiguration(p.GetService<IConfiguration>(), options));
        frameworkOptions.Services.AddScoped<DatabaseOidGenerator, DatabaseOidGenerator>();
        frameworkOptions.Services.AddScoped<IOidGenerator, DatabaseOidGenerator>();
        frameworkOptions.Services.AddScoped<IObjectStore, EF6ObjectStore>();
    }
}