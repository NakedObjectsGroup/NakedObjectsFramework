// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Configuration;
using NakedFramework.Core.Component;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.DependencyInjection.Utils;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Configuration;
using NakedFunctions.Reflector.FacetFactory;
using NakedFunctions.Reflector.Reflect;
using NakedFunctions.Services;

namespace NakedFunctions.Reflector.Extensions {
    public static class NakedFunctionsExtensions {
        public static FunctionalReflectorConfiguration FunctionalReflectorConfig(NakedFunctionsOptions options) =>
            new(options.FunctionalTypes, options.Functions, options.ConcurrencyCheck);

        public static void AddNakedFunctions(this NakedFrameworkOptions frameworkOptions, Action<NakedFunctionsOptions> setupAction) {
            var options = new NakedFunctionsOptions();
            setupAction(options);

            if (options.FunctionalTypes.Any()) {
                // filter enums and add to SystemTypes 
                var enums = options.FunctionalTypes.Where(t => t.IsEnum).ToArray();
                var coreFunctionalTypes = new[] {typeof(FunctionalContext), typeof(IContext)};
                options.FunctionalTypes = options.FunctionalTypes.Except(enums).ToArray();
                frameworkOptions.SupportedSystemTypes ??= t => t;
                frameworkOptions.AdditionalSystemTypes = frameworkOptions.AdditionalSystemTypes.Union(enums).ToArray();
                frameworkOptions.AdditionalUnpersistedTypes = coreFunctionalTypes;
                options.FunctionalTypes = options.FunctionalTypes.Union(coreFunctionalTypes).Distinct().ToArray();
            }

            RegisterWellKnownServices(frameworkOptions.Services);
            frameworkOptions.Services.RegisterFacetFactories<IFunctionalFacetFactoryProcessor>(FunctionalFacetFactories.StandardFacetFactories());
            frameworkOptions.Services.AddDefaultSingleton<FunctionalFacetFactorySet, FunctionalFacetFactorySet>();
            frameworkOptions.Services.AddDefaultSingleton<FunctionClassStrategy, FunctionClassStrategy>();
            frameworkOptions.Services.AddDefaultSingleton(typeof(IReflectorOrder<>), typeof(FunctionalReflectorOrder<>));
            frameworkOptions.Services.AddSingleton<IReflector, FunctionalReflector>();
            frameworkOptions.Services.AddSingleton<IFunctionalReflectorConfiguration>(p => FunctionalReflectorConfig(options));
            frameworkOptions.Services.AddSingleton<IServiceList>(p => new ServiceList());

            frameworkOptions.Services.AddDefaultScoped<IDomainObjectInjector, NoOpDomainObjectInjector>();

            options.RegisterCustomTypes?.Invoke(frameworkOptions.Services);
        }

        public static void RegisterWellKnownServices(IServiceCollection services) {
            services.AddDefaultScoped<IAlert, Alert>();
            services.AddDefaultScoped<IClock, Clock>();
            services.AddDefaultScoped<IGuidGenerator, GuidGenerator>();
            services.AddDefaultScoped<IPrincipalProvider, PrincipalProvider>();
            services.AddDefaultScoped<IRandomSeedGenerator, RandomSeedGenerator>();
        }
    }
}