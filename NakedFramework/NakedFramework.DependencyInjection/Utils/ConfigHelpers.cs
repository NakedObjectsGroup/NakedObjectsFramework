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
using NakedFramework.DependencyInjection.FacetFactory;

namespace NakedFramework.DependencyInjection.Utils {
    public static class ConfigHelpers {
        private static bool ServiceImplementationExists(this IServiceCollection services, Type tService) => services.Any(descriptor => descriptor.ServiceType == tService);

        private static bool ServiceImplementationExists<TService>(this IServiceCollection services) => services.ServiceImplementationExists(typeof(TService));

        public static void AddDefaultSingleton(this IServiceCollection services, Type tService, Type tImplementation) {
            if (!services.ServiceImplementationExists(tService)) {
                services.AddSingleton(tService, tImplementation);
            }
        }

        public static void AddDefaultSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService {
            if (!services.ServiceImplementationExists<TService>()) {
                services.AddSingleton<TService, TImplementation>();
            }
        }

        public static void AddDefaultScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService {
            if (!services.ServiceImplementationExists<TService>()) {
                services.AddScoped<TService, TImplementation>();
            }
        }

        public static void AddDefaultScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory) where TService : class {
            if (!services.ServiceImplementationExists<TService>()) {
                services.AddScoped(implementationFactory);
            }
        }

        public static void AddDefaultTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService {
            if (!services.ServiceImplementationExists<TService>()) {
                services.AddTransient<TService, TImplementation>();
            }
        }

        public static void RegisterFacetFactory<T>(this IServiceCollection services, Type factory) where T : IFacetFactory {
            FacetFactoryTypesProvider.AddType(factory);
            services.AddSingleton(typeof(T), factory);
        }

        private static bool SafeMatch(this ServiceDescriptor descriptor, Type toMatch) => descriptor.ImplementationType == toMatch;

        private static void RemoveFactory<TInterface, TOriginal>(this IServiceCollection services) {
            var serviceDescriptor = services.Where(descriptor => descriptor.ServiceType == typeof(TInterface)).FirstOrDefault(descriptor => descriptor.SafeMatch(typeof(TOriginal)));
            if (serviceDescriptor != null) {
                services.Remove(serviceDescriptor);
            }
        }

        public static void RegisterReplacementFacetFactory<TInterface, TReplacement, TOriginal>(IServiceCollection services)
            where TReplacement : IFacetFactory
            where TOriginal : IFacetFactory {
            // remove the original and register replacement.
            services.RemoveFactory<TInterface, TOriginal>();
            services.AddSingleton(typeof(TInterface), typeof(TReplacement));
        }

        // Helper method to, substitute a new implementation of a specific facet factory, but where the constructor
        // of the new one takes: a numeric order, and the standard NOF implementation of that facet factory. 
        public static void RegisterReplacementFacetFactoryDelegatingToOriginal<TInterface, TReplacement, TOriginal>(IServiceCollection services)
            where TReplacement : IFacetFactory
            where TOriginal : IFacetFactory {
            // remove original as an IFacetFactory
            services.RemoveFactory<TInterface, TOriginal>();

            // Register the original (standard NOF implementation). Note that although already registered by StandardConfig.RegisterStandardFacetFactories
            // that will be as a named impl of IFacetFactory.  This will be the only one registered as the concrete type
            // PropertyMethodsFacetFactory so doesn't need to be named.

            // We don't care about the order, because this isn't called as a FacetFactory AS SUCH.
            // but we still need one for the constructor
            services.AddSingleton(typeof(TOriginal), typeof(TOriginal));

            // Now add replacement using the standard pattern but using the same Name and orderNumber as the one being superseded. 
            // The original one will be auto-injected into it because of the implementation registered above

            services.AddSingleton(typeof(TInterface), typeof(TReplacement));
        }
    }
}