// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;
using NakedObjects.Core.spec;
using NakedObjects.EntityObjectStore;
using NakedObjects.Managers;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Web.Mvc.Helpers;

namespace NakedObjects.Mvc.App.App_Start {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Application Configuration

        //TODO: Add similar Configuration mechanisms for Authentication, Auditing
        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 

        //TODO: Rename this method to Services() once the other Services() function is removed.
        private static ServicesConfiguration ServicesConfiguration() {
            var config = new ServicesConfiguration();
            config.AddMenuServices(
                new CustomerRepository(),
                new OrderRepository(),
                new ProductRepository(),
                new EmployeeRepository(),
                new SalesRepository(),
                new SpecialOfferRepository(),
                new ContactRepository(),
                new VendorRepository(),
                new PurchaseOrderRepository(),
                new WorkOrderRepository());
            config.AddContributedActions(
                new OrderContributedActions(),
                new CustomerContributedActions());
            config.AddSystemServices(
                new SimpleEncryptDecrypt());
            return config;
        }

        private static EntityObjectStoreConfiguration EntityObjectStore() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] {typeof (AWDomainObject)});
            return config;
        }

        private static Type[] AdventureWorksTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        }

        //TODO: The need for this (in addition to the ServicesConfiguration) should be removed.
        public static Type[] Services() {
            return new[] {
                typeof (CustomerRepository),
                typeof (OrderRepository),
                typeof (ProductRepository),
                typeof (EmployeeRepository),
                typeof (SalesRepository),
                typeof (SpecialOfferRepository),
                typeof (ContactRepository),
                typeof (VendorRepository),
                typeof (PurchaseOrderRepository),
                typeof (WorkOrderRepository),
                typeof (OrderContributedActions),
                typeof (CustomerContributedActions)
            };
        }

        #endregion

        #region Framework Configuration

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            container.RegisterInstance<IServicesConfiguration>(ServicesConfiguration(), new ContainerControlledLifetimeManager());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(EntityObjectStore(), new ContainerControlledLifetimeManager());

            // in architecture
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, DotNetReflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, DotNetReflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IContainerInjector, DotNetDomainObjectContainerInjector>(new PerRequestLifetimeManager());
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
}