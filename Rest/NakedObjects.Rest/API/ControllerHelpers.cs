// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Rest.API {
    public static class ControllerHelpers {
        public static ObjectContextFacade GetServiceByName(this IFrameworkFacade frameworkFacade, string serviceName) {
            var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            return frameworkFacade.GetService(oid);
        }

        public static IMenuFacade GetMenuByName(this IFrameworkFacade frameworkFacade, string menuName) {
            if (string.IsNullOrEmpty(menuName)) {
                throw new BadRequestNOSException();
            }

            var menu = frameworkFacade.GetMainMenus().List.SingleOrDefault(m => m.Id == menuName);
            return menu ?? throw new MenuResourceNotFoundNOSException(menuName);
        }

        public static ActionContextFacade GetServiceActionByName(this IFrameworkFacade frameworkFacade, string serviceName, string actionName) {
            var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
            return frameworkFacade.GetServiceAction(oid, actionName);
        }

        public static ObjectContextFacade GetObjectByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId) {
            var oidTranslation = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            return frameworkFacade.GetObject(oidTranslation);
        }

        public static ActionContextFacade GetObjectActionByName(this IFrameworkFacade frameworkFacade, string domainType, string instanceId, string actionName) {
            var oidTranslation = frameworkFacade.OidTranslator.GetOidTranslation(domainType, instanceId);
            return frameworkFacade.GetObjectAction(oidTranslation, actionName);
        }
    }
}