using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Translation;
using NakedObjects.Unity;
using Common.Logging;
using NakedObjects.Facade.Interface;

namespace Template.Server.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        private static readonly ILog Logger = LogManager.GetLogger<UnityConfig>();


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
            try {
                return container.Value;
            }
            catch (Exception e) {
                Logger.Error($"Error on Unity GetConfiguredContainer : {e.Message}");
                throw;
            }
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
     
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            //Standard configuration
            try {
                StandardUnityConfig.RegisterStandardFacetFactories(container);
                StandardUnityConfig.RegisterCoreContainerControlledTypes(container);
                StandardUnityConfig.RegisterCorePerTransactionTypes<PerResolveLifetimeManager>(container);

                // config 
                container.RegisterInstance<IReflectorConfiguration>(NakedObjectsRunSettings.ReflectorConfig(), (new ContainerControlledLifetimeManager()));
                container.RegisterInstance<IEntityObjectStoreConfiguration>(NakedObjectsRunSettings.EntityObjectStoreConfig(), new ContainerControlledLifetimeManager());

                // frameworkFacade
                container.RegisterType<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>(new PerResolveLifetimeManager());

                container.RegisterType<IOidStrategy, EntityOidStrategy>(new PerResolveLifetimeManager());
                container.RegisterType<IStringHasher, SimpleStringHasher>(new PerResolveLifetimeManager());
                container.RegisterType<IFrameworkFacade, FrameworkFacade>(new PerResolveLifetimeManager());

                //Externals
                container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));
            }
            catch (Exception e) {
                Logger.Error($"Error on Unity RegisterTypes : {e.Message}");
                throw;
            }
        }
    }
}
