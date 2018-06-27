using Common.Logging;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;
using NakedObjects.Template;
using NakedObjects.Architecture.Component;
using WebActivatorEx;
using System;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Template.Server.App_Start.UnityWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Template.Server.App_Start.UnityWebApiActivator), "Shutdown")]

namespace Template.Server.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
            var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());

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
