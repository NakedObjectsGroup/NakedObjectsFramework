// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Persist;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Component;
using NakedFramework.Persistor.EFCore.Configuration;

namespace NakedFramework.Persistor.EFCore.Extensions {
    public static class EFCorePersistorExtensions {
        private static EFCorePersistorConfiguration EF6ObjectStoreConfiguration(IConfiguration configuration, EFCorePersistorOptions options) {
            var config = new EFCorePersistorConfiguration {
                MaximumCommitCycles = options.MaximumCommitCycles
            };

            var contexts = options.ContextCreators.Select(ci => (Func<DbContext>) (() => ci(configuration))).ToArray();
            config.Contexts = contexts;
            return config;
        }

        public static void AddEFCorePersistor(this NakedCoreOptions coreOptions, Action<EFCorePersistorOptions> setupAction) {
            var options = new EFCorePersistorOptions();
            setupAction(options);
#pragma warning disable EF1001 // Internal EF Core API usage.
            coreOptions.AdditionalSystemTypes = coreOptions.AdditionalSystemTypes.Append(typeof(InternalDbSet<>)).Append(typeof(EntityQueryable<>)).ToArray();
#pragma warning restore EF1001 // Internal EF Core API usage.

            coreOptions.Services.AddSingleton(p => EF6ObjectStoreConfiguration(p.GetService<IConfiguration>(), options));
            coreOptions.Services.AddScoped<IOidGenerator, DatabaseOidGenerator>();
            coreOptions.Services.AddScoped<IObjectStore, EFCoreObjectStore>();
        }
    }
}