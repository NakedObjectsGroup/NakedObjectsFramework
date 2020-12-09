// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;
using NakedObjects.Persistor.Entity.Component;
using NakedObjects.Persistor.Entity.Configuration;

namespace NakedObjects.DependencyInjection.Extensions {
    public static class NakedEntityPersistorExtensions {
        private static EntityObjectStoreConfiguration EntityObjectStoreConfiguration(IConfiguration configuration, EntityPersistorOptions options) {
            var config = new EntityObjectStoreConfiguration {EnforceProxies = options.EnforceProxies};

            var contexts = options.ContextInstallers.Select<Func<IConfiguration, DbContext>, Func<DbContext>>(f => () => f(configuration));
            contexts.ForEach(c => config.UsingContext(c));
            return config;
        }

        public static void AddEntityPersistor(this NakedCoreOptions coreOptions, Action<EntityPersistorOptions> setupAction) {
            var options = new EntityPersistorOptions();
            setupAction(options);

            coreOptions.Services.AddSingleton<IEntityObjectStoreConfiguration>(p => EntityObjectStoreConfiguration(p.GetService<IConfiguration>(), options));
            coreOptions.Services.AddScoped<EntityOidGenerator, EntityOidGenerator>();
            coreOptions.Services.AddScoped<IOidGenerator, EntityOidGenerator>();
            coreOptions.Services.AddScoped<IObjectStore, EntityObjectStore>();
        }
    }
}