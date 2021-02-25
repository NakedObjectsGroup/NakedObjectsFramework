// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Configuration;
using NakedFramework.Core.Configuration;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.Facade.Facade;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Impl.Translators;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Metamodel.Profile;
using NakedFramework.ParallelReflector.Component;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Reflect;

namespace NakedFramework.DependencyInjection.Extensions {
    public static class NakedFrameworkExtensions {
        public static CoreConfiguration CoreConfig(NakedCoreOptions options) {
            var config = new CoreConfiguration(options.MainMenus);

            if (options.SupportedSystemTypes is not null) {
                config.SupportedSystemTypes = options.SupportedSystemTypes(config.SupportedSystemTypes.ToArray()).Union(options.AdditionalSystemTypes).ToList();
            }

            config.HashMapCapacity = options.HashMapCapacity;
            return config;
        }

        private static void AddNakedCoreFramework(this IServiceCollection services, NakedCoreOptions options) {
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            if (options.AuthorizationConfiguration is not null) {
                services.AddSingleton(options.AuthorizationConfiguration);
                services.AddSingleton<IFacetDecorator, AuthorizationManager>();
            }

            if (options.AuditConfiguration is not null) {
                services.AddSingleton(options.AuditConfiguration);
                services.AddSingleton<IFacetDecorator, AuditManager>();
            }

            if (options.ProfileConfiguration is not null) {
                services.AddSingleton(options.ProfileConfiguration);
                services.AddSingleton<IFacetDecorator, ProfileManager>();
            }

            services.AddSingleton<ICoreConfiguration>(p => CoreConfig(options));

            services.RegisterFacetFactories<IObjectFacetFactoryProcessor>(TypeFacetFactories.FacetFactories());
            services.AddSingleton<SystemTypeFacetFactorySet, SystemTypeFacetFactorySet>();
            services.AddSingleton<SystemTypeClassStrategy, SystemTypeClassStrategy>();
            services.AddSingleton<IReflector, SystemTypeReflector>();

            // frameworkFacade
            services.AddTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
            services.AddTransient<IOidStrategy, EntityOidStrategy>();
            services.AddTransient<IStringHasher, InvariantStringHasher>();
            services.AddTransient<IFrameworkFacade, FrameworkFacade>();

            //Externals
            services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
        }

        public static void AddNakedFramework(this IServiceCollection services, Action<NakedCoreOptions> setupAction) {
            var options = new NakedCoreOptions(services);
            setupAction(options);

            services.AddNakedCoreFramework(options);
        }
    }
}