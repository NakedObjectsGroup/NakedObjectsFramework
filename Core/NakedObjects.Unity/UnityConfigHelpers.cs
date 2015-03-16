using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Reflect;
using System;
namespace NakedObjects.Unity {
    public class UnityConfigHelpers {

        public static void RegisterFacetFactory(Type factory, IUnityContainer container, int order) {
            container.RegisterType(typeof(IFacetFactory), factory, factory.Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order));
        }

        public static void RegisterReplacementFacetFactory<Treplacement, Toriginal>(IUnityContainer container)
            where Treplacement : IFacetFactory
            where Toriginal : IFacetFactory {

            int order = FacetFactories.StandardIndexOf(typeof(Toriginal));


            container.RegisterType<Treplacement>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(order)
                );

            RegisterFacetFactory(typeof(Treplacement), container, order);
        }

        //Helper method to, subsistute a new implementation of a specific facet factory, but where the constructor
        // of the new one takes: a numeric order, and the standard NOF implementation of that facet factory. 
        public static void RegisterReplacementFacetFactoryDelegatingToOriginal<Treplacement, Toriginal>(IUnityContainer container)
            where Treplacement : IFacetFactory
            where Toriginal : IFacetFactory {

            int order = FacetFactories.StandardIndexOf(typeof(Toriginal));

            //Register the standard NOF implementation. Note that although already registered by StandardUnityConfig.RegisterStandardFacetFactories
            //that will be as a named impl of IFacetFactory.  This will be the only one registered as the very specific
            //PropertyMethodsFacetFactory so doesn't need to be named.
            container.RegisterType<Treplacement>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(order)
                );

            // Now add replacement using the standard pattern but using the same Name and orderNumber as the one we are superseding. 
            // The old one will be auto-injected into it because of the implementation registered above
            container.RegisterType<IFacetFactory, Treplacement>(
                typeof(Treplacement).Name, //Following standard pattern for all NOF factories
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(order, typeof(Toriginal)));
        }
    }
}
