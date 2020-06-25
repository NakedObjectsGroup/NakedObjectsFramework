// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NakedObjects.Facade;
using NakedObjects.Rest.Model;

namespace NakedObjects.Rest.App.Demo
{

    //[Authorize]
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade frameworkFacade,
                                        ILogger<RestfulObjectsController> logger,
                                        ILoggerFactory loggerFactory) : base(frameworkFacade, logger, loggerFactory) { }

        [HttpGet]
        public override ActionResult GetHome() {
            return base.GetHome();
        }

        [HttpGet]
        public override ActionResult GetUser() {
            return base.GetUser();
        }

        [HttpGet]
        public override ActionResult GetServices() {
            return base.GetServices();
        }

        [HttpGet]
        public override ActionResult GetMenus() {
            return base.GetMenus();
        }

        [HttpGet]
        public override ActionResult GetVersion() {
            return base.GetVersion();
        }

        [HttpGet]
        public override ActionResult GetService(string serviceName) {
            return base.GetService(serviceName);
        }

        [HttpGet]
        public override ActionResult GetMenu(string menuName) {
            return base.GetMenu(menuName);
        }

        [HttpGet]
        public override ActionResult GetServiceAction(string serviceName, string actionName) {
            return base.GetServiceAction(serviceName, actionName);
        }

        [HttpGet]
        public override ActionResult GetImage(string imageId) {
            return base.GetImage(imageId);
        }

        [HttpPost]
        public override ActionResult PostPersist(string domainType, PersistArgumentMap arguments) {
            return base.PostPersist(domainType, arguments);
        }

        [HttpGet]
        public override ActionResult GetObject(string domainType, string instanceId) {
            return base.GetObject(domainType, instanceId);
        }

        [HttpPut]
        public override ActionResult PutObject(string domainType, string instanceId, ArgumentMap arguments) {
            return base.PutObject(domainType, instanceId, arguments);
        }

        [HttpGet]
        public override ActionResult GetProperty(string domainType, string instanceId, string propertyName) {
            return base.GetProperty(domainType, instanceId, propertyName);
        }

        [HttpGet]
        public override ActionResult GetCollection(string domainType, string instanceId, string propertyName) {
            return base.GetCollection(domainType, instanceId, propertyName);
        }

        [HttpGet]
        public override ActionResult GetAction(string domainType, string instanceId, string actionName) {
            return base.GetAction(domainType, instanceId, actionName);
        }


        [HttpPut]
        public override ActionResult PutProperty(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.PutProperty(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override ActionResult DeleteProperty(string domainType, string instanceId, string propertyName) {
            return base.DeleteProperty(domainType, instanceId, propertyName);
        }

        [HttpPost]
        public override ActionResult PostCollection(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.PostCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpDelete]
        public override ActionResult DeleteCollection(string domainType, string instanceId, string propertyName,  SingleValueArgument argument) {
            return base.DeleteCollection(domainType, instanceId, propertyName, argument);
        }

        [HttpGet]
        public override ActionResult GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.GetInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPost]
        public override ActionResult PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.PostInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpPut]
        public override ActionResult PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) {
            return base.PutInvoke(domainType, instanceId, actionName, arguments);
        }

        [HttpGet]
        public override ActionResult GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.GetInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPut]
        public override ActionResult PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.PutInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpPost]
        public override ActionResult PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) {
            return base.PostInvokeOnService(serviceName, actionName, arguments);
        }

        [HttpGet]
        public override ActionResult GetInvokeTypeActions(string typeName, string actionName,  ArgumentMap arguments) {
            return base.GetInvokeTypeActions(typeName, actionName, arguments);
        }

        [HttpPut]
        public override ActionResult PutPersistPropertyPrompt(string domainType, string propertyName,  PromptArgumentMap promptArguments) {
            return base.PutPersistPropertyPrompt(domainType, propertyName, promptArguments);
        }

        [HttpGet]
        public override ActionResult GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) {
            return base.GetPropertyPrompt(domainType, instanceId, propertyName, arguments);
        }

        [HttpGet]
        public override ActionResult GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) {
            return base.GetParameterPrompt(domainType, instanceId, actionName, parmName, arguments);
        }

        [HttpGet]
        public override ActionResult GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) {
            return base.GetParameterPromptOnService(serviceName, actionName, parmName, arguments);
        }

        [HttpGet]
        public override ActionResult GetCollectionValue(string domainType, string instanceId, string propertyName) {
            return base.GetCollectionValue(domainType, instanceId, propertyName);
        }

    }
}