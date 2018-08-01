using System.Web.Http;
using NakedObjects.Architecture.Component;
using NakedObjects.Rest.App.Demo.App_Start;
using Unity.AspNet.WebApi;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(UnityWebApiActivator), "Shutdown")]

namespace NakedObjects.Rest.App.Demo.App_Start {
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
            var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());
            //var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            UnityConfig.GetConfiguredContainer().Resolve<IReflector>().Reflect();
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
