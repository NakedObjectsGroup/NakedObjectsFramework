// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using NakedObjects.Facade;
using NakedObjects.Rest;
using NakedObjects.Rest.Model;

namespace NakedObjects.Rest.App.Demo
{

    //[Authorize]
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade frameworkFacade) : base(frameworkFacade) {}

        [HttpGet]
        public override JsonResult GetHome( ReservedArguments arguments) {
            return base.GetHome(arguments);
        }

        [HttpGet]
        public override JsonResult GetUser( ReservedArguments arguments) {
            return base.GetUser(arguments);
        }

        [HttpGet]
        public override JsonResult GetServices( ReservedArguments arguments) {
            return base.GetServices(arguments);
        }

        [HttpGet]
        public override JsonResult GetMenus( ReservedArguments arguments) {
            return base.GetMenus(arguments);
        }

        [HttpGet]
        public override JsonResult GetVersion( ReservedArguments arguments) {
            return base.GetVersion(arguments);
        }

        [HttpGet]
        public override JsonResult GetService(string serviceName,  ReservedArguments arguments) {
            return base.GetService(serviceName, arguments);
        }

        [HttpGet]
        public override JsonResult GetMenu(string menuName,  ReservedArguments arguments) {
            return base.GetMenu(menuName, arguments);
        }

        [HttpGet]
        public override JsonResult GetServiceAction(string serviceName, string actionName,  ReservedArguments arguments) {
            return base.GetServiceAction(serviceName, actionName, arguments);
        }

        [HttpGet]
        public override JsonResult GetImage(string imageId,  ReservedArguments arguments) {
            return base.GetImage(imageId, arguments);
        }

        [HttpPost]
        public override JsonResult PostPersist(string domainType,  ArgumentMap arguments) {
            return base.PostPersist(domainType, arguments);
        }

        [HttpGet]
        public override JsonResult GetObject(string domainType, string instanceId,  ReservedArguments arguments) {
            return base.GetObject(domainType, instanceId, arguments);
        }

        [HttpPut]
        public override JsonResult PutObject(string domainType, string instanceId, ArgumentMap arguments) {
            return base.PutObject(domainType, instanceId, arguments);
        }

        [HttpGet]
        public override JsonResult GetProperty(string domainType, string instanceId, string propertyName,  ReservedArguments arguments) {
            return base.GetProperty(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override JsonResult GetCollection(string domainType, string instanceId, string propertyName,  ReservedArguments arguments) {
            return base.GetCollection(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override JsonResult GetAction(string domainType, string instanceId, string actionName,  ReservedArguments arguments) {
            return base.GetAction(domainType, instanceId, actionName, arguments);
        }


        [HttpPut]
        public override JsonResult PutProperty(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.PutProperty(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override JsonResult DeleteProperty(string domainType, string instanceId, string propertyName,  ReservedArguments arguments) {
            return base.DeleteProperty(domainType, instanceId, propertyName, arguments);
        }

        [HttpPost]
        public override JsonResult PostCollection(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.PostCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override JsonResult DeleteCollection(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.DeleteCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpGet]
        public override JsonResult GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.GetInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPost]
        public override JsonResult PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.PostInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPut]
        public override JsonResult PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.PutInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpGet]
        public override JsonResult GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.GetInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPut]
        public override JsonResult PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.PutInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPost]
        public override JsonResult PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.PostInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpGet]
        public override JsonResult GetInvokeTypeActions(string typeName, string actionName,  ArgumentMap arguments) {
            return base.GetInvokeTypeActions(typeName, actionName, arguments);
        }

        [HttpPut]
        public override JsonResult PutPersistPropertyPrompt(string domainType, string propertyName,  PromptArgumentMap promptArguments) {
            return base.PutPersistPropertyPrompt(domainType, propertyName, promptArguments);
        }

        [HttpGet]
        public override JsonResult GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
            return base.GetPropertyPrompt(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override JsonResult GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
            return base.GetParameterPrompt(domainType, instanceId, actionName, parmName, arguments);
        }

        [HttpGet]
        public override JsonResult GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
            return base.GetParameterPromptOnService(serviceName, actionName, parmName, arguments);
        }

        [HttpGet]
        public override JsonResult GetCollectionValue(string domainType, string instanceId, string propertyName,  ReservedArguments arguments) {
            return base.GetCollectionValue(domainType, instanceId, propertyName, arguments);
        }

    }
}