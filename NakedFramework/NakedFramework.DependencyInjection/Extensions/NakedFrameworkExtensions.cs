﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
using NakedFramework.Core.Configuration;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.DependencyInjection.Utils;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Impl.Translators;
using NakedFramework.Facade.Impl.Utility;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Facade.Utility;
using NakedFramework.Metamodel.I18N;
using NakedFramework.Metamodel.Profile;
using NakedFramework.ParallelReflector.Component;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Reflect;

namespace NakedFramework.DependencyInjection.Extensions;

public static class NakedFrameworkExtensions {
    public static CoreConfiguration CoreConfig(NakedFrameworkOptions options) {
        var config = new CoreConfiguration(options.MainMenus);

        options.SupportedSystemTypes ??= t => t;
        config.SupportedSystemTypes = options.SupportedSystemTypes(config.SupportedSystemTypes.ToArray()).Union(options.AdditionalSystemTypes).ToList();

        config.HashMapCapacity = options.HashMapCapacity;

        config.UsePlaceholderForUnreflectedType = options.UsePlaceholderForUnreflectedType;
        return config;
    }

    private static void AddNakedCoreFramework(this IServiceCollection services, NakedFrameworkOptions options) {
        ParallelConfig.RegisterCoreSingletonTypes(services);
        ParallelConfig.RegisterCoreScopedTypes(services);

        if (options.ProfileConfiguration is not null) {
            services.AddSingleton(options.ProfileConfiguration);
            services.AddDefaultSingleton<IFacetDecorator, ProfileDecorator>();
            services.AddDefaultSingleton<IProfileManager, ProfileManager>();
        }

        if (options.UseI18N) {
            services.AddDefaultSingleton<IFacetDecorator, I18NDecorator>();
        }

        var coreConfiguration = CoreConfig(options);
        services.AddSingleton<ICoreConfiguration>(p => coreConfiguration);
        services.AddSingleton<ITypeList>(p => coreConfiguration);

        services.RegisterFacetFactories<ISystemTypeFacetFactoryProcessor>(TypeFacetFactories.FacetFactories());
        services.AddDefaultSingleton<SystemTypeFacetFactorySet, SystemTypeFacetFactorySet>();
        services.AddDefaultSingleton<SystemTypeClassStrategy, SystemTypeClassStrategy>();
        services.AddSingleton<IReflector, SystemTypeReflector>();
        services.AddSingleton<IAllTypeList, AllTypeList>();

        // frameworkFacade
        services.AddDefaultTransient<ITypeCodeMapper, DefaultTypeCodeMapper>();
        services.AddDefaultTransient<IKeyCodeMapper, DefaultKeyCodeMapper>();
        services.AddDefaultTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
        services.AddDefaultTransient<IOidStrategy, EntityOidStrategy>();
        services.AddDefaultTransient<IStringHasher, InvariantStringHasher>();
        services.AddDefaultTransient<IFrameworkFacade, FrameworkFacade>();
    }

    public static void AddNakedFramework(this IServiceCollection services, Action<NakedFrameworkOptions> setupAction) {
        var options = new NakedFrameworkOptions(services);
        setupAction(options);
        services.AddNakedCoreFramework(options);
    }
}