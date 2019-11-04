// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.ParallelReflect;

namespace NakedObjects.DependencyInjection
{
    public class ConfigHelpers {
        public static void RegisterFacetFactory(Type factory, IServiceCollection services, int order) {
            
            services.AddSingleton(typeof(IFacetFactory), p => Activator.CreateInstance(factory, factory.Name, order));
        }

        //public static void RegisterReplacementFacetFactory<TReplacement, TOriginal>(IServiceCollection services)
        //    where TReplacement : IFacetFactory
        //    where TOriginal : IFacetFactory
        //{
        //    int order = FacetFactories.StandardIndexOf(typeof(TOriginal));
        //    //container.RegisterType<IFacetFactory, TReplacement>(
        //    //    typeof(TOriginal).Name,
        //    //    new ContainerControlledLifetimeManager(),
        //    //    new InjectionConstructor(order));
        //}

        //// Helper method to, substitute a new implementation of a specific facet factory, but where the constructor
        //// of the new one takes: a numeric order, and the standard NOF implementation of that facet factory. 
        //public static void RegisterReplacementFacetFactoryDelegatingToOriginal<TReplacement, TOriginal>(IUnityContainer container)
        //    where TReplacement : IFacetFactory
        //    where TOriginal : IFacetFactory {
        //    int order = FacetFactories.StandardIndexOf(typeof(TOriginal));

        //    // Register the orginal (standard NOF implementation). Note that although already registered by StandardUnityConfig.RegisterStandardFacetFactories
        //    // that will be as a named impl of IFacetFactory.  This will be the only one registered as the concrete type
        //    // PropertyMethodsFacetFactory so doesn't need to be named.
        //    container.RegisterType<TOriginal>(
        //        new ContainerControlledLifetimeManager(),
        //        new InjectionConstructor(0)); // We don't care about the order, because this isn't called as a FacetFactory AS SUCH.
        //                                                   // but we still need one for the constructor

        //    // Now add replacement using the standard pattern but using the same Name and orderNumber as the one being superseded. 
        //    // The original one will be auto-injected into it because of the implementation registered above
        //    container.RegisterType<IFacetFactory, TReplacement>(
        //        typeof(TOriginal).Name, //Following standard pattern for all NOF factories
        //        new ContainerControlledLifetimeManager(),
        //        new InjectionConstructor(order, typeof(TOriginal)));
        //}
    }
}