// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Component;
using NakedFramework.DependencyInjection.Configuration;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedObjects.Core.Component;
using NakedObjects.Reflector.Component;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Reflector.Reflect;

namespace NakedObjects.Reflector.Extensions {
    public static class NakedObjectsExtensions {
        private static ObjectReflectorConfiguration ObjectReflectorConfig(NakedObjectsOptions options) {
            ObjectReflectorConfiguration.NoValidate = options.NoValidate;
            return new ObjectReflectorConfiguration(options.Types, options.Services, options.ConcurrencyCheck);
        }

        public static void AddNakedObjects(this NakedCoreOptions coreOptions, Action<NakedObjectsOptions> setupAction) {
            var options = new NakedObjectsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(coreOptions.Services);

            coreOptions.Services.RegisterFacetFactories<IObjectFacetFactoryProcessor>(ObjectFacetFactories.StandardFacetFactories());
            coreOptions.Services.AddSingleton<ObjectFacetFactorySet, ObjectFacetFactorySet>();
            coreOptions.Services.AddSingleton<ObjectClassStrategy, ObjectClassStrategy>();

            coreOptions.Services.AddSingleton<IReflector, ObjectReflector>();
            coreOptions.Services.AddSingleton<IObjectReflectorConfiguration>(p => ObjectReflectorConfig(options));
            coreOptions.Services.AddSingleton<IServiceList>(p => new ServiceList(options.Services));

            coreOptions.Services.AddScoped<IDomainObjectInjector, DomainObjectContainerInjector>();
        }
    }
}