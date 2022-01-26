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
using NakedLegacy.About;
using NakedLegacy.Reflector.Component;
using NakedLegacy.Reflector.Configuration;
using NakedLegacy.Reflector.FacetFactory;
using NakedLegacy.Reflector.Reflect;

namespace NakedLegacy.Reflector.Extensions;

public static class LegacyExtensions {
    private static LegacyObjectReflectorConfiguration LegacyObjectReflectorConfig(NakedLegacyOptions options) {
        LegacyObjectReflectorConfiguration.NoValidate = options.NoValidate;
        return new LegacyObjectReflectorConfiguration(options.DomainModelTypes, options.DomainModelServices, options.ValueHolderTypes, options.ConcurrencyCheck);
    }

    public static void AddNakedLegacy(this NakedFrameworkOptions frameworkOptions, Action<NakedLegacyOptions> setupAction) {
        var options = new NakedLegacyOptions();
        setupAction(options);

        frameworkOptions.Services.RegisterFacetFactories<ILegacyFacetFactoryProcessor>(LegacyObjectFacetFactories.StandardFacetFactories());
        frameworkOptions.Services.RegisterFacetFactories<ISystemTypeFacetFactoryProcessor>(LegacyObjectFacetFactories.TypeFacetFactories());

        frameworkOptions.Services.AddDefaultSingleton<LegacyObjectFacetFactorySet, LegacyObjectFacetFactorySet>();
        frameworkOptions.Services.AddDefaultSingleton<LegacyObjectClassStrategy, LegacyObjectClassStrategy>();

        frameworkOptions.Services.AddSingleton(typeof(IReflectorOrder<>), typeof(LegacyObjectReflectorOrder<>));
        frameworkOptions.Services.AddSingleton<IReflector, LegacyObjectReflector>();
        var legacyObjectReflectorConfiguration = LegacyObjectReflectorConfig(options);
        frameworkOptions.Services.AddSingleton<ILegacyObjectReflectorConfiguration>(p => legacyObjectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<ITypeList>(p => legacyObjectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<IServiceList>(p => new ServiceList(options.DomainModelServices));

        frameworkOptions.Services.AddDefaultScoped<LegacyAboutCache, LegacyAboutCache>();
        frameworkOptions.Services.AddDefaultScoped<IDomainObjectInjector, LegacyObjectContainerInjector>();
        frameworkOptions.Services.AddDefaultSingleton<IAboutFactory, AboutFactory>();

        var additionalTypes = ReflectorDefaults.DefaultLegacyTypes.Union(options.ValueHolderTypes);

        frameworkOptions.AdditionalSystemTypes = frameworkOptions.AdditionalSystemTypes.Union(additionalTypes).ToArray();
    }
}