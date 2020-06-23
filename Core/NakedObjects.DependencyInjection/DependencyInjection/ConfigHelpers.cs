// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Reflect;

namespace NakedObjects.DependencyInjection {
    public static class ConfigHelpers {
        private static T CreateFacetFactory<T>(IServiceProvider p, int order) =>
            (T) Activator.CreateInstance(typeof(T), order, p.GetService<ILoggerFactory>());

        private static TReplacement CreateDelegatingFacetFactory<TReplacement, TOrginal>(IServiceProvider p, int order) =>
            (TReplacement) Activator.CreateInstance(typeof(TReplacement), order, p.GetService<TOrginal>(), p.GetService<ILoggerFactory>());

        private static MethodInfo GetRegisterMethod(Type type) =>
            typeof(ConfigHelpers).GetMethod(nameof(RegisterFacetFactory), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(type);

        public static void RegisterFacetFactory(Type factory, IServiceCollection services, int order) =>
            GetRegisterMethod(factory).Invoke(null, new object[] {services, order});

        // register with type so we can find to remove 
        // called by reflection
        // ReSharper disable once UnusedMember.Local
        private static void RegisterFacetFactory<T>(IServiceCollection services, int order) =>
            services.AddSingleton(typeof(IFacetFactory), p => CreateFacetFactory<T>(p, order));

        private static bool SafeMatch(this ServiceDescriptor descriptor, Type toMatch) {
            var types = descriptor.ImplementationFactory?.Target?.GetType().GenericTypeArguments;
            return types != null && types.Length == 1 && types[0] == toMatch;
        }

        private static void RemoveFactory<T>(this IServiceCollection services) {
            var serviceDescriptor = services.Where(descriptor => descriptor.ServiceType == typeof(IFacetFactory)).FirstOrDefault(descriptor => descriptor.SafeMatch(typeof(T)));
            if (serviceDescriptor != null) {
                services.Remove(serviceDescriptor);
            }
        }

        public static void RegisterReplacementFacetFactory<TReplacement, TOriginal>(IServiceCollection services)
            where TReplacement : IFacetFactory
            where TOriginal : IFacetFactory {
            var order = GetExistingOrder<TOriginal>();

            // remove the original and register replacement.
            services.RemoveFactory<TOriginal>();
            services.AddSingleton(typeof(IFacetFactory), p => CreateFacetFactory<TReplacement>(p, order));
        }

        private static int GetExistingOrder<TOriginal>() {
            var order = FacetFactories.StandardIndexOf(typeof(TOriginal));
            return order == -1 ? ParallelReflect.FacetFactories.StandardIndexOf(typeof(TOriginal)) : order;
        }

        // Helper method to, substitute a new implementation of a specific facet factory, but where the constructor
        // of the new one takes: a numeric order, and the standard NOF implementation of that facet factory. 
        public static void RegisterReplacementFacetFactoryDelegatingToOriginal<TReplacement, TOriginal>(IServiceCollection services)
            where TReplacement : IFacetFactory
            where TOriginal : IFacetFactory {
            var order = GetExistingOrder<TOriginal>();

            // remove original as an IFacetFactory
            services.RemoveFactory<TOriginal>();

            // Register the original (standard NOF implementation). Note that although already registered by StandardConfig.RegisterStandardFacetFactories
            // that will be as a named impl of IFacetFactory.  This will be the only one registered as the concrete type
            // PropertyMethodsFacetFactory so doesn't need to be named.

            // We don't care about the order, because this isn't called as a FacetFactory AS SUCH.
            // but we still need one for the constructor
            services.AddSingleton(typeof(TOriginal), p => CreateFacetFactory<TOriginal>(p, 0));

            // Now add replacement using the standard pattern but using the same Name and orderNumber as the one being superseded. 
            // The original one will be auto-injected into it because of the implementation registered above

            services.AddSingleton(typeof(IFacetFactory), p => CreateDelegatingFacetFactory<TReplacement, TOriginal>(p, order));
        }
    }
}