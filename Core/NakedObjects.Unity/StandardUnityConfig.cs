using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Spec;
using NakedObjects.Meta;
using NakedObjects.Meta.Menu;
using NakedObjects.Persistor.Entity;
using NakedObjects.Reflect;
using NakedObjects.Service;
using System;
using System.Linq;

namespace NakedObjects.Unity {
    public static class StandardUnityConfig {

        public static void RegisterCoreContainerControlledTypes(IUnityContainer container) {
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());
        }

        public static void RegisterCorePerTransactionTypes<T>(IUnityContainer container)
            where T : LifetimeManager, new() {
            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new T(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new T(), new InjectionConstructor(10));
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new T());
            container.RegisterType<IOidGenerator, EntityOidGenerator>(new T());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new T());
            container.RegisterType<IObjectStore, EntityObjectStore>(new T());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new T());
            container.RegisterType<ITransactionManager, TransactionManager>(new T());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new T());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new T());
            container.RegisterType<IServicesManager, ServicesManager>(new T());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new T());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new T());
            container.RegisterType<IMessageBroker, MessageBroker>(new T());
            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new T());
            container.RegisterType<ISession, WindowsSession>(new T());
            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new T());

            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new T());
            container.RegisterType<SpecFactory, SpecFactory>(new T());
        }

        public static void RegisterStandardFacetFactories(IUnityContainer container) {
            var factoryTypes = FacetFactories.StandardFacetFactories();
            for (int i = 0; i < factoryTypes.Count(); i++) {
                RegisterFacetFactory(factoryTypes[i], container, i);
            }
        }

        private static void RegisterFacetFactory(Type factory, IUnityContainer container, int order) {
            container.RegisterType(typeof(IFacetFactory), factory, factory.Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order));
        }
    }
}
