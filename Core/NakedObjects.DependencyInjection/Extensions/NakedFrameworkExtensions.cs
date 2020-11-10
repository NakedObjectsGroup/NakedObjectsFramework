// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFunctions.Reflector.Component;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection.DependencyInjection;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Menu;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflector.Component;
using NakedObjects.Rest;

namespace NakedObjects.DependencyInjection.Extensions {
    public class InvariantStringHasher : IStringHasher {
        public string GetHash(string toHash) => "1234";
    }

    public class NakedCoreOptions {
        public Func<IConfiguration, DbContext>[] ContextInstallers { get; set; }
        public IAuthorizationConfiguration AuthorizationConfiguration { get; set; }
        public IAuditConfiguration AuditConfiguration { get; set; }
        public Func<IMenuFactory, IMenu[]> MainMenus { get; set; }
    }

    public class NakedObjectsOptions {
        public Type[] Types { get; set; } = Array.Empty<Type>();
        public Type[] Services { get; set; } = Array.Empty<Type>();
        public string[] ModelNamespaces { get; set; } = Array.Empty<string>();
       
        public bool NoValidate { get; set; }
    }

    public class NakedFunctionsOptions {
        public Type[] FunctionalTypes { get; set; } = Array.Empty<Type>();
        public Type[] Functions { get; set; } = Array.Empty<Type>();
    }

    public class RestfulObjectsOptions {

        public bool DebugWarnings { get; set; } = true;

        // to make whole application 'read only' 
        public bool IsReadOnly { get; set; }

        // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
        // 0, 3600, 86400 are the defaults 
        // no caching makes debugging easier
        public (int,int,int)  CacheSettings { get; set; } = (0, 3600, 86400);

        // make Accept header handling non-strict (RO spec 2.4.4)
        public bool AcceptHeaderStrict { get; set; }

        // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
        public int DefaultPageSize { get; set; } = 50; 

        // These flags control Member Representations - if true the 'details' will be included 
        // in the the member. This will increase the size of the initial representation but reduce 
        // the number of messages.   
        public bool InlineDetailsInActionMemberRepresentations { get; set; }
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; }
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; }
    }


    public static class NakedFrameworkExtensions {
        private static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration, NakedCoreOptions options) {
            var config = new EntityObjectStoreConfiguration();
            var contexts = options.ContextInstallers.Select<Func<IConfiguration, DbContext>, Func<DbContext>>(f => () => f(configuration));
            contexts.ForEach(c => config.UsingContext(c));
            return config;
        }

        private static ObjectReflectorConfiguration ObjectReflectorConfig(NakedObjectsOptions options) {
            ObjectReflectorConfiguration.NoValidate = options.NoValidate;
            return new ObjectReflectorConfiguration(options.Types, options.Services, options.ModelNamespaces);
        }

        public static FunctionalReflectorConfiguration FunctionalReflectorConfig(NakedFunctionsOptions options) =>
            new FunctionalReflectorConfiguration(options.FunctionalTypes, options.Functions);


        public static CoreConfiguration CoreConfig(NakedCoreOptions options) => new CoreConfiguration(options.MainMenus);

        private static void AddNakedCoreFramework(this IServiceCollection services, NakedCoreOptions options) {
            ParallelConfig.RegisterStandardFacetFactories(services);
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            services.AddSingleton<IEntityObjectStoreConfiguration>(p => EntityObjectStoreConfig(p.GetService<IConfiguration>(), options));

            if (options.AuthorizationConfiguration is not null) {
                services.AddSingleton<IAuthorizationConfiguration>(options.AuthorizationConfiguration);
            }

            if (options.AuditConfiguration is not null) {
                services.AddSingleton<IAuditConfiguration>(options.AuditConfiguration);
            }

            services.AddSingleton<ICoreConfiguration>(p => CoreConfig(options));


            // frameworkFacade
            services.AddTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();

            services.AddTransient<IOidStrategy, EntityOidStrategy>();
            services.AddTransient<IStringHasher, InvariantStringHasher>();
            services.AddTransient<IFrameworkFacade, FrameworkFacade>();

            //Externals
            services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
        }


        public static void AddNakedCore(this IServiceCollection services, Action<NakedCoreOptions> setupAction) {
            var options = new NakedCoreOptions();
            setupAction(options);

            services.AddNakedCoreFramework(options);
        }


        public static void AddNakedObjects(this IServiceCollection services, Action<NakedObjectsOptions> setupAction) {
            var options = new NakedObjectsOptions();
            setupAction(options);
            services.AddSingleton<IReflector, ObjectReflector>();
            services.AddSingleton<IObjectReflectorConfiguration>(p => ObjectReflectorConfig(options));
        }

        public static void AddNakedFunctions(this IServiceCollection services, Action<NakedFunctionsOptions> setupAction) {
            var options = new NakedFunctionsOptions();
            setupAction(options);

            ParallelConfig.RegisterWellKnownServices(services);
            services.AddSingleton<IReflector, FunctionalReflector>();
            services.AddSingleton<IFunctionalReflectorConfiguration>(p => FunctionalReflectorConfig(options));
        }

        public static void AddRestfulObjects(this IServiceCollection services, Action<RestfulObjectsOptions> setupAction = null)
        {
            var options = new RestfulObjectsOptions();
            setupAction?.Invoke(options);

            // TODO configure with config object ?
            RestfulObjectsControllerBase.DebugWarnings = options.DebugWarnings;
            RestfulObjectsControllerBase.IsReadOnly = options.IsReadOnly;
            RestfulObjectsControllerBase.CacheSettings = options.CacheSettings;
            RestfulObjectsControllerBase.AcceptHeaderStrict = options.AcceptHeaderStrict;
            RestfulObjectsControllerBase.DefaultPageSize = options.DefaultPageSize;
            RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations = options.InlineDetailsInActionMemberRepresentations;
            RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations = options.InlineDetailsInCollectionMemberRepresentations;
            RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations = options.InlineDetailsInPropertyMemberRepresentations;
        }

        public static void UseRestfulObjects(this IApplicationBuilder app, string restRoot = "") {
            restRoot ??= "";
            app.UseMvc(routeBuilder => RestfulObjectsRouting.AddRestRoutes(routeBuilder, restRoot));
        }
    }
}