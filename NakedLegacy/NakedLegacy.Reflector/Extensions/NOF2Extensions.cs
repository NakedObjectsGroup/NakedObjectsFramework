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
using NakedFramework.Core.Component;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.DependencyInjection.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NOF2.About;
using NOF2.Reflector.Component;
using NOF2.Reflector.Configuration;
using NOF2.Reflector.FacetFactory;
using NOF2.Reflector.Reflect;

namespace NOF2.Reflector.Extensions;

public static class NOF2Extensions {
    private static INOF2ReflectorConfiguration LegacyObjectReflectorConfig(NOF2Options options) {
        NOF2ReflectorConfiguration.NoValidate = options.NoValidate;
        return new NOF2ReflectorConfiguration(options.DomainModelTypes, options.DomainModelServices, options.ValueHolderTypes, options.ConcurrencyCheck);
    }

    public static void AddNakedLegacy(this NakedFrameworkOptions frameworkOptions, Action<NOF2Options> setupAction) {
        var options = new NOF2Options();
        setupAction(options);

        frameworkOptions.Services.RegisterFacetFactories<INOF2FacetFactoryProcessor>(NOF2FacetFactories.StandardFacetFactories());
        frameworkOptions.Services.RegisterFacetFactories<ISystemTypeFacetFactoryProcessor>(NOF2FacetFactories.TypeFacetFactories());

        frameworkOptions.Services.AddDefaultSingleton<NOF2FacetFactorySet, NOF2FacetFactorySet>();
        frameworkOptions.Services.AddDefaultSingleton<NOF2ObjectClassStrategy, NOF2ObjectClassStrategy>();

        frameworkOptions.Services.AddSingleton(typeof(IReflectorOrder<>), typeof(NOF2ReflectorOrder<>));
        frameworkOptions.Services.AddSingleton<IReflector, NOF2Reflector>();
        var legacyObjectReflectorConfiguration = LegacyObjectReflectorConfig(options);
        frameworkOptions.Services.AddSingleton<INOF2ReflectorConfiguration>(p => legacyObjectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<ITypeList>(p => legacyObjectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<IServiceList>(p => new ServiceList(options.DomainModelServices));

        frameworkOptions.Services.AddDefaultScoped<AboutCache, AboutCache>();
        frameworkOptions.Services.AddDefaultScoped<IDomainObjectInjector, NOF2ObjectContainerInjector>();
        frameworkOptions.Services.AddDefaultSingleton<IAboutFactory, AboutFactory>();

        var additionalTypes = NOF2ReflectorDefaults.DefaultLegacyTypes.Union(options.ValueHolderTypes);

        frameworkOptions.AdditionalSystemTypes = frameworkOptions.AdditionalSystemTypes.Union(additionalTypes).ToArray();
    }
}