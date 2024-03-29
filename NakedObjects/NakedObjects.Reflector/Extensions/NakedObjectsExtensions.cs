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
using NakedFramework.Core.Component;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.DependencyInjection.Utils;
using NakedFramework.Metamodel.Audit;
using NakedObjects.Core.Component;
using NakedObjects.Reflector.Audit;
using NakedObjects.Reflector.Authorization;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.Extensions;

public static class NakedObjectsExtensions {
    private static ObjectReflectorConfiguration ObjectReflectorConfig(NakedObjectsOptions options) {
        ObjectReflectorConfiguration.NoValidate = options.NoValidate;
        return new ObjectReflectorConfiguration(options.DomainModelTypes, options.DomainModelServices, options.UseNullableReferenceTypesForOptionality, options.ConcurrencyCheck);
    }

    public static void AddNakedObjects(this NakedFrameworkOptions frameworkOptions, Action<NakedObjectsOptions> setupAction) {
        var options = new NakedObjectsOptions();
        setupAction(options);

        if (options.DomainModelTypes.Any()) {
            // filter enums and add to SystemTypes 
            var enums = options.DomainModelTypes.Where(t => t.IsEnum).ToArray();

            options.DomainModelTypes = options.DomainModelTypes.Except(enums).ToArray();
            frameworkOptions.SupportedSystemTypes ??= t => t;
            frameworkOptions.AdditionalSystemTypes = frameworkOptions.AdditionalSystemTypes.Union(enums).ToArray();
        }

        frameworkOptions.Services.RegisterFacetFactories<IDomainObjectFacetFactoryProcessor>(ObjectFacetFactories.StandardFacetFactories());
        frameworkOptions.Services.AddDefaultSingleton<ObjectFacetFactorySet, ObjectFacetFactorySet>();
        frameworkOptions.Services.AddDefaultSingleton<ObjectClassStrategy, ObjectClassStrategy>();

        frameworkOptions.Services.AddDefaultSingleton(typeof(IReflectorOrder<>), typeof(ObjectReflectorOrder<>));
        frameworkOptions.Services.AddSingleton<IReflector, ObjectReflector>();
        var objectReflectorConfiguration = ObjectReflectorConfig(options);
        frameworkOptions.Services.AddSingleton<IObjectReflectorConfiguration>(p => objectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<ITypeList>(p => objectReflectorConfiguration);
        frameworkOptions.Services.AddSingleton<IServiceList>(p => new ServiceList(options.DomainModelServices));

        frameworkOptions.Services.AddDefaultScoped<IDomainObjectInjector, DomainObjectContainerInjector>();

        options.RegisterCustomTypes?.Invoke(frameworkOptions.Services);

        if (frameworkOptions.AuthorizationConfiguration is not null) {
            frameworkOptions.Services.AddSingleton(frameworkOptions.AuthorizationConfiguration);
            frameworkOptions.Services.AddDefaultSingleton<IFacetDecorator, AuthorizationDecorator>();
            frameworkOptions.Services.AddDefaultSingleton<IAuthorizationManager, AuthorizationManager>();
        }

        if (frameworkOptions.AuditConfiguration is not null) {
            frameworkOptions.Services.AddSingleton(frameworkOptions.AuditConfiguration);
            frameworkOptions.Services.AddDefaultSingleton<IFacetDecorator, AuditDecorator>();
            frameworkOptions.Services.AddDefaultSingleton<IAuditManager, AuditManager>();
        }
    }
}