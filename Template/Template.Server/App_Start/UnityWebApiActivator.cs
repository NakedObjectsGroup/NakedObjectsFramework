// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Common.Logging;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;
using NakedObjects.Template;
using NakedObjects.Architecture.Component;
using WebActivatorEx;
using System;

[assembly: PreApplicationStartMethod(typeof (UnityWebApiActivator), "Start")]
[assembly: ApplicationShutdownMethod(typeof (UnityWebApiActivator), "Shutdown")]

namespace NakedObjects.Template {
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UnityWebApiActivator));

        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            try
            {
                // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
                // var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());
                var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
                GlobalConfiguration.Configuration.DependencyResolver = resolver;
                UnityConfig.GetConfiguredContainer().Resolve<IReflector>().Reflect();
            }
            catch (Exception e)
            {
                Logger.Error($"Error on UnityWebApiActivator:Start : {e.Message}");
                throw;
            }
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown() {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}