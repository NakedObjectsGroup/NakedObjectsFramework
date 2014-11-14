using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Security;
using NakedObjects.Core.spec;
using NakedObjects.EntityObjectStore;
using NakedObjects.Managers;
using NakedObjects.Meta;
using NakedObjects.Reflect;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using RestfulObjects.Test.Data;

namespace RestfulObjects.Test.App
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion


        private static object[] MenuServices {
            get {
                return new object[] {
                    new RestDataRepository(),
                    new WithActionService()
                };
            }
        }


        private static object[] ContributedActions {
            get {
                return new object[] {
                    new ContributorService()
                };
            }
        }


        private static object[] SystemServices {
            get {
                return new object[] {
                    new TestTypeCodeMapper()
                };
            }
        }

        public static Type[] Services() {
            return new[] {
                typeof (RestDataRepository),
                typeof (WithActionService),
                typeof (ContributorService),
                typeof (TestTypeCodeMapper)
            };
        }


        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));

            var config = new EntityObjectStoreConfiguration();

            //config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            //config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });

            config.UsingCodeFirstContext(() => new CodeFirstContext("RestTest"));

            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, new ContainerControlledLifetimeManager());

            var reflectorConfig = new ReflectorConfiguration(new Type[] { },
               MenuServices.Select(s => s.GetType()).ToArray(),
               ContributedActions.Select(s => s.GetType()).ToArray(),
               SystemServices.Select(s => s.GetType()).ToArray());

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IServicesConfiguration, ServicesConfiguration>(new ContainerControlledLifetimeManager());

            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerResolveLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerResolveLifetimeManager());
            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));


            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerResolveLifetimeManager());

            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerResolveLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerResolveLifetimeManager());
            container.RegisterType<IObjectStore, EntityObjectStore>(new PerResolveLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerResolveLifetimeManager());

            container.RegisterType<ITransactionManager, TransactionManager>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerResolveLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerResolveLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerResolveLifetimeManager());

            container.RegisterType<IAuthorizationManager, NullAuthorizationManager>(new PerResolveLifetimeManager());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerResolveLifetimeManager());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());
            container.RegisterType<ISession, WindowsSession>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());

            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerResolveLifetimeManager());

            container.RegisterType<IOidStrategy, ExternalOid>(new PerResolveLifetimeManager());

            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
        }
    }
}