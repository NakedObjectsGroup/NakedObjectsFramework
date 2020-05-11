// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.DependencyInjection;

namespace NakedObjects.Rest.App.Demo
{

    public class InvariantStringHasher : IStringHasher {
        public string GetHash(string toHash) {
            return "1234";
        }
    }

    /// <summary>
    /// Specifies the configuration of Naked Objects for use within the DI framework
    /// </summary>
    public static class DIConfig {

    public static void AddNakedObjects(this IServiceCollection services, IConfiguration configuration) {


            ParallelConfig.RegisterStandardFacetFactories(services);
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            // config 
            services.AddSingleton<IReflectorConfiguration>(p => NakedObjectsRunSettings.ReflectorConfig());
            services.AddSingleton<IEntityObjectStoreConfiguration>(p => NakedObjectsRunSettings.EntityObjectStoreConfig(configuration));

            // frameworkFacade
            services.AddTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();

            services.AddTransient<IOidStrategy, EntityOidStrategy>();
            services.AddTransient<IStringHasher, InvariantStringHasher>();
            services.AddTransient<IFrameworkFacade, FrameworkFacade>();

            
            //Externals
            services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
        }
    }
}