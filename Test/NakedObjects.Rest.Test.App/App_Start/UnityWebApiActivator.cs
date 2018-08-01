// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Http;
using NakedObjects.Rest.Test.App;
using NakedObjects.Architecture.Component;
using WebActivatorEx;
using Unity.AspNet.WebApi;

[assembly: PreApplicationStartMethod(typeof(UnityWebApiActivator), "Start")]
[assembly: ApplicationShutdownMethod(typeof(UnityWebApiActivator), "Shutdown")]

namespace NakedObjects.Rest.Test.App {
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() {
            // Use UnityHierarchicalDependencyResolver if you want to use a new child container for each IHttpController resolution.
            // var resolver = new UnityHierarchicalDependencyResolver(UnityConfig.GetConfiguredContainer());
            var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            var reflector = UnityConfig.GetConfiguredContainer().Resolve(typeof(IReflector), null) as IReflector;
            reflector.Reflect();
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown() {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}