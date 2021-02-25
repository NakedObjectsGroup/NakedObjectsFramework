// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EFCore.Component;
using NakedFramework.Persistor.EFCore.Configuration;
using NakedFramework.Persistor.Entity.Component;

namespace NakedFramework.Persistor.EFCore.Extensions {
    public static class EFCorePersistorExtensions {
        private static EFCorePersistorConfiguration EntityObjectStoreConfiguration(IConfiguration configuration, EFCorePersistorOptions options) {
            var config = new EFCorePersistorConfiguration();
            //{
            //    //EnforceProxies = options.EnforceProxies,
            //    //CustomConfig = options.CustomConfig,
            //    //DefaultMergeOption = options.DefaultMergeOption,
            //    //MaximumCommitCycles = options.MaximumCommitCycles,
            //    //NotPersistedTypes = options.NotPersistedTypes,
            //    //RollBackOnError = options.RollBackOnError,
            //    //RequireExplicitAssociationOfTypes = options.RequireExplicitAssociationOfTypes
            //};

            Func<DbContext> context =  () => options.ContextInstaller(configuration);
            config.Context = context;
            return config;
        }

        public static void AddEFCorePersistor(this NakedCoreOptions coreOptions, Action<EFCorePersistorOptions> setupAction) {
            var options = new EFCorePersistorOptions();
            setupAction(options);

            //var unpersistedTypes = options.NotPersistedTypes();
            //options.NotPersistedTypes = () => unpersistedTypes.Union(coreOptions.AdditionalUnpersistedTypes).ToArray();

            coreOptions.Services.AddSingleton<EFCorePersistorConfiguration>(p => EntityObjectStoreConfiguration(p.GetService<IConfiguration>(), options));
            //coreOptions.Services.AddScoped<EntityOidGenerator, EntityOidGenerator>();
            coreOptions.Services.AddScoped<IOidGenerator, EntityOidGenerator>();
            coreOptions.Services.AddScoped<IObjectStore, EFCoreObjectStore>();
        }
    }
}