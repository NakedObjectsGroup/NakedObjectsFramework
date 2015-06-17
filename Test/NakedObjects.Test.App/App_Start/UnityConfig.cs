// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Test.App;
using NakedObjects.Test.App.Controllers;
using NakedObjects.Unity;

namespace NakedObjects.Mvc.App {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Framework Configuration
        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            //Standard configuration
            StandardUnityConfig.RegisterStandardFacetFactories(container);
            StandardUnityConfig.RegisterCoreContainerControlledTypes(container);
            StandardUnityConfig.RegisterCorePerTransactionTypes<PerRequestLifetimeManager>(container);

            // config
            container.RegisterInstance<IReflectorConfiguration>(NakedObjectsRunSettings.ReflectorConfig(), (new ContainerControlledLifetimeManager()));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(NakedObjectsRunSettings.EntityObjectStoreConfig(), new ContainerControlledLifetimeManager());

            // facade
            container.RegisterType<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>("KeyOid", new PerRequestLifetimeManager());
            container.RegisterType<IOidTranslator, OidTranslatorSemiColonSeparatedList>(new PerRequestLifetimeManager());

            container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerRequestLifetimeManager());

            container.RegisterType<IIdHelper, IdHelper>(new PerRequestLifetimeManager());

            container.RegisterType<IFrameworkFacade, FrameworkFacade>("RestFacade", new PerRequestLifetimeManager(), new InjectionConstructor(typeof(IOidStrategy), new ResolvedParameter<IOidTranslator>("KeyOid"), typeof(INakedObjectsFramework)));
            container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerRequestLifetimeManager());

            container.RegisterType<RestfulObjectsController, RestfulObjectsController>(new PerResolveLifetimeManager(), new InjectionConstructor(new ResolvedParameter<IFrameworkFacade>("RestFacade")));


            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));

            // Facet decorators 
            if (NakedObjectsRunSettings.AuditConfig() != null) {
                container.RegisterType(typeof(IFacetDecorator), typeof(AuditManager), "AuditManager", new ContainerControlledLifetimeManager());
                container.RegisterInstance(typeof(IAuditConfiguration), NakedObjectsRunSettings.AuditConfig(), new ContainerControlledLifetimeManager());
            }

            if (NakedObjectsRunSettings.AuthorizationConfig() != null) {
                container.RegisterType(typeof(IFacetDecorator), typeof(AuthorizationManager), "AuthorizationManager", new ContainerControlledLifetimeManager());
                container.RegisterInstance(typeof(IAuthorizationConfiguration), NakedObjectsRunSettings.AuthorizationConfig(), new ContainerControlledLifetimeManager());
            }
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