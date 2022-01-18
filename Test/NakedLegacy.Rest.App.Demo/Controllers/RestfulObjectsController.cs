// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using AdventureWorksLegacy.AppLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Framework;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Configuration;
using NakedFramework.Rest.Model;
using NakedLegacy.Reflector.Component;
using Microsoft.Extensions.DependencyInjection;

namespace Legacy.Rest.App.Demo.Controllers {
    //[Authorize]
    public class RestfulObjectsController : RestfulObjectsControllerBase {
        public RestfulObjectsController(IFrameworkFacade frameworkFacade,
                                        ILogger<RestfulObjectsController> logger,
                                        ILoggerFactory loggerFactory,
                                        IRestfulObjectsConfiguration config) : base(frameworkFacade, logger, loggerFactory, config) {
            //ThreadLocals.Initialize(frameworkFacade.GetScopedServiceProvider, sp => new Container(sp.GetService<INakedFramework>()));
            ThreadLocals.InitializeContainer(frameworkFacade.GetScopedServiceProvider);
        }

        //public void Dispose() => ThreadLocals.Reset();

        [HttpGet]
        public override ActionResult GetHome() => base.GetHome();

        [HttpGet]
        public override ActionResult GetUser() => base.GetUser();

        [HttpGet]
        public override ActionResult GetServices() => base.GetServices();

        [HttpGet]
        public override ActionResult GetMenus() => base.GetMenus();

        [HttpGet]
        public override ActionResult GetVersion() => base.GetVersion();

        [HttpGet]
        public override ActionResult GetService(string serviceName) => base.GetService(serviceName);

        [HttpGet]
        public override ActionResult GetMenu(string menuName) => base.GetMenu(menuName);

        [HttpGet]
        public override ActionResult GetServiceAction(string serviceName, string actionName) => base.GetServiceAction(serviceName, actionName);

        [HttpGet]
        public override ActionResult GetImage(string imageId) => base.GetImage(imageId);

        [HttpPost]
        public override ActionResult PostPersist(string domainType, PersistArgumentMap arguments) => base.PostPersist(domainType, arguments);

        [HttpGet]
        public override ActionResult GetObject(string domainType, string instanceId) => base.GetObject(domainType, instanceId);

        [HttpPut]
        public override ActionResult PutObject(string domainType, string instanceId, ArgumentMap arguments) => base.PutObject(domainType, instanceId, arguments);

        [HttpGet]
        public override ActionResult GetProperty(string domainType, string instanceId, string propertyName) => base.GetProperty(domainType, instanceId, propertyName);

        [HttpGet]
        public override ActionResult GetCollection(string domainType, string instanceId, string propertyName) => base.GetCollection(domainType, instanceId, propertyName);

        [HttpGet]
        public override ActionResult GetAction(string domainType, string instanceId, string actionName) => base.GetAction(domainType, instanceId, actionName);

        [HttpPut]
        public override ActionResult PutProperty(string domainType, string instanceId, string propertyName, SingleValueArgument argument) => base.PutProperty(domainType, instanceId, propertyName, argument);

        [HttpDelete]
        public override ActionResult DeleteProperty(string domainType, string instanceId, string propertyName) => base.DeleteProperty(domainType, instanceId, propertyName);

        [HttpPost]
        public override ActionResult PostCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) => base.PostCollection(domainType, instanceId, propertyName, argument);

        [HttpDelete]
        public override ActionResult DeleteCollection(string domainType, string instanceId, string propertyName, SingleValueArgument argument) => base.DeleteCollection(domainType, instanceId, propertyName, argument);

        [HttpGet]
        public override ActionResult GetInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => base.GetInvoke(domainType, instanceId, actionName, arguments);

        [HttpPost]
        public override ActionResult PostInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => base.PostInvoke(domainType, instanceId, actionName, arguments);

        [HttpPut]
        public override ActionResult PutInvoke(string domainType, string instanceId, string actionName, ArgumentMap arguments) => base.PutInvoke(domainType, instanceId, actionName, arguments);

        [HttpGet]
        public override ActionResult GetInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => base.GetInvokeOnService(serviceName, actionName, arguments);

        [HttpPut]
        public override ActionResult PutInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => base.PutInvokeOnService(serviceName, actionName, arguments);

        [HttpPost]
        public override ActionResult PostInvokeOnService(string serviceName, string actionName, ArgumentMap arguments) => base.PostInvokeOnService(serviceName, actionName, arguments);

        [HttpGet]
        public override ActionResult GetInvokeTypeActions(string typeName, string actionName, ArgumentMap arguments) => base.GetInvokeTypeActions(typeName, actionName, arguments);

        [HttpPut]
        public override ActionResult PutPersistPropertyPrompt(string domainType, string propertyName, PromptArgumentMap promptArguments) => base.PutPersistPropertyPrompt(domainType, propertyName, promptArguments);

        [HttpGet]
        public override ActionResult GetPropertyPrompt(string domainType, string instanceId, string propertyName, ArgumentMap arguments) => base.GetPropertyPrompt(domainType, instanceId, propertyName, arguments);

        [HttpGet]
        public override ActionResult GetParameterPrompt(string domainType, string instanceId, string actionName, string parmName, ArgumentMap arguments) => base.GetParameterPrompt(domainType, instanceId, actionName, parmName, arguments);

        [HttpGet]
        public override ActionResult GetParameterPromptOnService(string serviceName, string actionName, string parmName, ArgumentMap arguments) => base.GetParameterPromptOnService(serviceName, actionName, parmName, arguments);

        [HttpGet]
        public override ActionResult GetCollectionValue(string domainType, string instanceId, string propertyName) => base.GetCollectionValue(domainType, instanceId, propertyName);

     
    }
}