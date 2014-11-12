// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Security.Principal;
using System.Web;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;
using NakedObjects.Core.spec;
using NakedObjects.EntityObjectStore;
using NakedObjects.Managers;
using NakedObjects.Meta;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Reflect;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Models;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menus;

namespace NakedObjects.Mvc.App.App_Start {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Application Configuration

        //TODO: Add similar Configuration mechanisms for Authentication, Auditing
        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 


        private static Type[] Types {
            get {
                return new Type[] { typeof(EntityCollection<object>), typeof(ObjectQuery<object>), typeof(ActionResultModelQ<object>) };
            }
        }


        private static object[] MenuServices {
            get {
                return new object[] {
                new CustomerRepository(),
                new OrderRepository(),
                new ProductRepository(),
                new EmployeeRepository(),
                new SalesRepository(),
                new SpecialOfferRepository(),
                new ContactRepository(),
                new VendorRepository(),
                new PurchaseOrderRepository(),
                new WorkOrderRepository()
                };
            }
        }


        private static object[] ContributedActions {
            get {
                return new object[] {
                     new OrderContributedActions(),
                new CustomerContributedActions()
                };
            }
        }


        private static object[] SystemServices {
            get {
                return new object[] {
                   new SimpleEncryptDecrypt()
                };
            }
        }

        private static EntityObjectStoreConfiguration EntityObjectStore() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });
            return config;
        }

        private static Type[] AdventureWorksTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof(AWDomainObject) && !t.IsAbstract).ToArray();
        }



        #endregion

        #region Framework Configuration

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            var reflectorConfig = new ReflectorConfiguration(Types,
               MenuServices.Select(s => s.GetType()).ToArray(),
               ContributedActions.Select(s => s.GetType()).ToArray(),
               SystemServices.Select(s => s.GetType()).ToArray());

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IServicesConfiguration, ServicesConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(EntityObjectStore(), new ContainerControlledLifetimeManager());
            container.RegisterType<IMainMenuDefinition, MyMainMenuDefinition>(new ContainerControlledLifetimeManager());

            // in architecture
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFacetFactorySet, FacetFactorySet>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerRequestLifetimeManager());
            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerRequestLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectStore, EntityObjectStore.EntityObjectStore>(new PerRequestLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerRequestLifetimeManager());
            container.RegisterType<ITransactionManager, ObjectStoreTransactionManager>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerRequestLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerRequestLifetimeManager());
            container.RegisterType<IAuthorizationManager, NullAuthorizationManager>(new PerRequestLifetimeManager());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerRequestLifetimeManager());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());
            container.RegisterType<ISession, WindowsSession>(new PerRequestLifetimeManager());
            container.RegisterType<IMessageBroker, SimpleMessageBroker>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerRequestLifetimeManager());

            // surface
            container.RegisterType<IOidStrategy, ExternalOid>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerRequestLifetimeManager());

            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerRequestLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerRequestLifetimeManager());
            container.RegisterType<IUpdateNotifier, SimpleUpdateNotifier>(new PerRequestLifetimeManager());

            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));
        }

        #endregion

        #region Unity Container

        private static readonly Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() => {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer() {
            return container.Value;
        }

        #endregion
    }

    public class MyMainMenuDefinition : IMainMenuDefinition {

        public IMenu[] MainMenus(IMenuFactory factory) {
            var menu1 = factory.NewMenu<CustomerRepository>(true);
            var menu2 = factory.NewMenu<OrderRepository>(true);
            var menu3 = factory.NewMenu<ProductRepository>(true);
            var menu4 = factory.NewMenu<EmployeeRepository>(true);
            var menu5 = factory.NewMenu<SalesRepository>(true);
            var menu6 = factory.NewMenu<SpecialOfferRepository>(true);
            var menu7 = factory.NewMenu<ContactRepository>(true);
            var menu8 = factory.NewMenu<VendorRepository>(true);
            var menu9 = factory.NewMenu<PurchaseOrderRepository>(true);
            var menu10 = factory.NewMenu<WorkOrderRepository>(true);

            return new IMenu[] { menu1, menu2, menu3, menu4, menu5, menu6, menu7, menu8, menu9, menu10};
        }
    }
}