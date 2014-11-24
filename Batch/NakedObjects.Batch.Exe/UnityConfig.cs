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
using System.Threading;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Async;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Spec;
using NakedObjects.Meta;
using NakedObjects.Meta.Menus;
using NakedObjects.Persistor.Entity;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflect;
using NakedObjects.Service;

namespace NakedObjects.Batch.Exe {
    // here to avoid coupling framework to unity
    public class UnityFrameworkResolver : IFrameworkResolver {
        private readonly IUnityContainer unityContainer;

        public UnityFrameworkResolver(IUnityContainer unityContainer) {
            this.unityContainer = unityContainer;
        }

        #region IFrameworkResolver Members

        public INakedObjectsFramework GetFramework() {
            return unityContainer.Resolve<INakedObjectsFramework>();
        }

        #endregion
    }

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Application Configuration

        //TODO: Add similar Configuration mechanisms for Authentication, Auditing
        //Any other simple configuration options (e.g. bool or string) on the old Run classes should be
        //moved onto a single SystemConfiguration, which can delegate e.g. to Web.config 


        private static Type[] Types {
            get { return new Type[] {typeof (EntityCollection<object>), typeof (ObjectQuery<object>)}; }
        }


        private static object[] MenuServices {
            get { return new object[] {}; }
        }


        private static object[] ContributedActions {
            get { return new object[] {}; }
        }


        private static object[] SystemServices {
            get {
                return new object[] {
                    new AsyncService()
                };
            }
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

        #endregion

        #region Framework Configuration

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            AssemblyHook.EnsureAssemblyLoaded();

            var reflectorConfig = new ReflectorConfiguration(Types,
                MenuServices.Select(s => s.GetType()).ToArray(),
                ContributedActions.Select(s => s.GetType()).ToArray(),
                SystemServices.Select(s => s.GetType()).ToArray());

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IServicesConfiguration, ServicesConfiguration>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(EntityObjectStore(), new ContainerControlledLifetimeManager());
            container.RegisterType<IMainMenuDefinition, NullMenuDefinition>(new ContainerControlledLifetimeManager());

            // in architecture
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFacetFactorySet, FacetFactorySet>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());

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
            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new PerResolveLifetimeManager());

            container.RegisterType<IBatchController, BatchController>(new PerResolveLifetimeManager());


            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerResolveLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerResolveLifetimeManager());

            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => Thread.CurrentPrincipal));
        }

        public class NullMenuDefinition : IMainMenuDefinition {
            #region IMainMenuDefinition Members

            public IMenu[] MainMenus(IMenuFactory factory) {
                return new IMenu[] {};
            }

            #endregion
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