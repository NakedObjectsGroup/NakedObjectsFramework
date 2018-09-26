// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using AWCustom;
using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Unity;


namespace NakedObjects.Rest.App.DemoOwin {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
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
        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            //Standard configuration

            StandardUnityConfig.RegisterStandardFacetFactories(container);
            StandardUnityConfig.RegisterCoreContainerControlledTypes(container);
            StandardUnityConfig.RegisterCorePerTransactionTypes<HierarchicalLifetimeManager>(container);

            // add custom notnavigable factory

            UnityConfigHelpers.RegisterFacetFactory(typeof(AWNotNavigableFacetFactory), container, 1000);
            UnityConfigHelpers.RegisterFacetFactory(typeof(AWNotCountedAnnotationFacetFactory), container, 1001);

            // config 
            container.RegisterInstance<IReflectorConfiguration>(NakedObjectsRunSettings.ReflectorConfig(), (new ContainerControlledLifetimeManager()));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(NakedObjectsRunSettings.EntityObjectStoreConfig(), new ContainerControlledLifetimeManager());

            // surface
            container.RegisterType<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>(new HierarchicalLifetimeManager());

            container.RegisterType<IOidStrategy, EntityOidStrategy>(new HierarchicalLifetimeManager());
            container.RegisterType<IStringHasher, SimpleStringHasher>(new HierarchicalLifetimeManager());
            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new HierarchicalLifetimeManager());

            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));
        }
    }
}