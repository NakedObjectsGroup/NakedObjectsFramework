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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Configuration;
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
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Rest;

namespace NakedObjects.DependencyInjection.Extensions {
    public static class NakedFrameworkExtensions {
        private static EntityObjectStoreConfiguration EntityObjectStoreConfig(IConfiguration configuration, NakedCoreOptions options) {
            var config = new EntityObjectStoreConfiguration();
            var contexts = options.ContextInstallers.Select<Func<IConfiguration, DbContext>, Func<DbContext>>(f => () => f(configuration));
            contexts.ForEach(c => config.UsingContext(c));
            return config;
        }

        private static ObjectReflectorConfiguration ObjectReflectorConfig(NakedObjectsOptions options) {
            ObjectReflectorConfiguration.NoValidate = options.NoValidate;
            return new ObjectReflectorConfiguration(options.Types, options.Services, options.ConcurrencyCheck);
        }

        public static FunctionalReflectorConfiguration FunctionalReflectorConfig(NakedFunctionsOptions options) =>
            new FunctionalReflectorConfiguration(options.FunctionalTypes, options.Functions, options.ConcurrencyCheck);


        public static CoreConfiguration CoreConfig(NakedCoreOptions options) {
            var config = new CoreConfiguration(options.MainMenus);

            if (options.SupportedSystemTypes is not null) {
                config.SupportedSystemTypes = options.SupportedSystemTypes(config.SupportedSystemTypes.ToArray()).ToList();
            }

            return config;
        }

        private static void AddNakedCoreFramework(this IServiceCollection services, NakedCoreOptions options) {
            ParallelConfig.RegisterStandardFacetFactories(services);
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            services.AddSingleton<IEntityObjectStoreConfiguration>(p => EntityObjectStoreConfig(p.GetService<IConfiguration>(), options));

            if (options.AuthorizationConfiguration is not null) {
                services.AddSingleton(options.AuthorizationConfiguration);
            }

            if (options.AuditConfiguration is not null) {
                services.AddSingleton(options.AuditConfiguration);
            }

            services.AddSingleton<ICoreConfiguration>(p => CoreConfig(options));
            services.AddSingleton<IReflector, SystemTypeReflector>();


            // frameworkFacade
            services.AddTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();

            services.AddTransient<IOidStrategy, EntityOidStrategy>();
            services.AddTransient<IStringHasher, InvariantStringHasher>();
            services.AddTransient<IFrameworkFacade, FrameworkFacade>();

            //Externals
            services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
        }


        public static void AddNakedCore(this IServiceCollection services, Action<NakedCoreOptions> setupAction) {
            var options = new NakedCoreOptions(services);
            setupAction(options);

            services.AddNakedCoreFramework(options);
        }


        public static void AddNakedObjects(this IServiceCollection services, Action<NakedObjectsOptions> setupAction) {
            var options = new NakedObjectsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(services);

            services.AddSingleton<IReflector, ObjectReflector>();
            services.AddSingleton<IObjectReflectorConfiguration>(p => ObjectReflectorConfig(options));
        }

        public static void AddNakedObjects(this NakedCoreOptions coreOptions, Action<NakedObjectsOptions> setupAction) {
            var options = new NakedObjectsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(coreOptions.Services);

            coreOptions.Services.AddSingleton<IReflector, ObjectReflector>();
            coreOptions.Services.AddSingleton<IObjectReflectorConfiguration>(p => ObjectReflectorConfig(options));
        }

        public static void AddNakedFunctions(this IServiceCollection services, Action<NakedFunctionsOptions> setupAction) {
            var options = new NakedFunctionsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(services);

            ParallelConfig.RegisterWellKnownServices(services);
            services.AddSingleton<IReflector, FunctionalReflector>();
            services.AddSingleton<IFunctionalReflectorConfiguration>(p => FunctionalReflectorConfig(options));
        }

        public static void AddNakedFunctions(this NakedCoreOptions coreOptions, Action<NakedFunctionsOptions> setupAction) {
            var options = new NakedFunctionsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(coreOptions.Services);

            ParallelConfig.RegisterWellKnownServices(coreOptions.Services);
            coreOptions.Services.AddSingleton<IReflector, FunctionalReflector>();
            coreOptions.Services.AddSingleton<IFunctionalReflectorConfiguration>(p => FunctionalReflectorConfig(options));
        }

        public static void AddRestfulObjects(this IServiceCollection services, Action<RestfulObjectsOptions> setupAction = null) {
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

        public static void AddRestfulObjects(this NakedCoreOptions coreOptions, Action<RestfulObjectsOptions> setupAction = null) {
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

        public static void AddNakedFramework(this IServiceCollection services, Action<NakedCoreOptions> setupAction) {
            var options = new NakedCoreOptions(services);
            setupAction(options);

            services.AddNakedCoreFramework(options);
        }
    }
}