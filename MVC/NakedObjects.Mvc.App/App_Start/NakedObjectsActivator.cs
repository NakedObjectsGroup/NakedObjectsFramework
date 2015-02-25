// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Mvc;
using NakedObjects.Mvc.App;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (NakedObjectsActivator), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (NakedObjectsActivator), "PostStart")]

namespace NakedObjects.Mvc.App {
    public static class NakedObjectsActivator {
        public static void PreStart() {
            //RestConfig.RestPreStart();           
        }

        public static void PostStart() {
            // TODO - need to sort this so we still support injection of container and services into controller and views

            //var injector = new DotNetDomainObjectContainerInjector(NakedObjectsContext.Reflector, NakedObjectsContext.LifecycleManager.GetServices().Select(no => no.Object).ToArray());
            //DependencyResolver.SetResolver(new NakedObjectsDependencyResolver(injector));
            //RestConfig.RestPostStart();

            // Without this any value type fields with a default value will be set to mandatory by the MS unobtrusive validation
            // - that overrides the required NOF behaviour based on the 'Optionally' attribute.
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }
    }
}