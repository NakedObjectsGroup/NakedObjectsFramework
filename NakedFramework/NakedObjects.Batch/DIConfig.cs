// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Component;
using NakedObjects.DependencyInjection;
using NakedObjects.Persistor.Entity.Configuration;

namespace NakedObjects.Batch {
    //public class InvariantStringHasher : IStringHasher {
    //    public string GetHash(string toHash) {
    //        return "1234";
    //    }
    //}

    /// <summary>
    ///     Specifies the Unity configuration for the main container.
    /// </summary>
    public static class DIConfig {
        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        ///     There is no need to register concrete types such as controllers or API controllers (unless you want to
        ///     change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
        /// </remarks>
        public static void AddNakedObjects(this IServiceCollection services, IConfiguration configuration) {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            //Standard configuration

            ParallelConfig.RegisterStandardFacetFactories(services);
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            // config 
            services.AddSingleton<IReflectorConfiguration>(p => NakedObjectsRunSettings.ReflectorConfig());
            services.AddSingleton<IEntityObjectStoreConfiguration>(p => NakedObjectsRunSettings.EntityObjectStoreConfig(configuration));

            services.AddScoped<IBatchRunner, BatchRunner>();

            //Externals
            services.AddScoped(p => Thread.CurrentPrincipal);
        }
    }
}