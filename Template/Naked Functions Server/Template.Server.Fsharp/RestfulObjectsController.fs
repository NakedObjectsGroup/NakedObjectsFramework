// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


namespace NakedObjects.Rest.App.Demo 

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open NakedFramework.Rest.Configuration;
open NakedFramework.Rest.Model;
open NakedFramework.Facade;
open NakedFramework.Rest.API;
open NakedFramework.Facade.Interface;

    //[Authorize]
    type RestfulObjectsController (frameworkFacade : IFrameworkFacade,
                                   logger : ILogger<RestfulObjectsController>,
                                   loggerFactory : ILoggerFactory,
                                   config : IRestfulObjectsConfiguration ) = 
        inherit RestfulObjectsControllerBase(frameworkFacade, logger, loggerFactory, config)
    
        [<HttpGet>]
        override this.GetHome() = base.GetHome();

        [<HttpGet>]
        override this.GetUser() = base.GetUser();

        [<HttpGet>]
        override this.GetServices() = base.GetServices();

        [<HttpGet>]
        override this.GetMenus() = base.GetMenus();

        [<HttpGet>]
        override this.GetVersion() = base.GetVersion();

        [<HttpGet>]
        override this.GetService(serviceName) = base.GetService(serviceName);

        [<HttpGet>]
        override this.GetMenu(menuName) = base.GetMenu(menuName);

        [<HttpGet>]
        override this.GetServiceAction( serviceName,  actionName) = base.GetServiceAction(serviceName, actionName);

        [<HttpGet>]
        override this.GetImage( imageId) = base.GetImage(imageId);

        [<HttpPost>]
        override this.PostPersist( domainType,  arguments) = base.PostPersist(domainType, arguments);

        [<HttpGet>]
        override this.GetObject( domainType,  instanceId) = base.GetObject(domainType, instanceId);

        [<HttpPut>]
        override this.PutObject( domainType,  instanceId,  arguments) = base.PutObject(domainType, instanceId, arguments);

        [<HttpGet>]
        override this.GetProperty( domainType,  instanceId,  propertyName) = base.GetProperty(domainType, instanceId, propertyName);

        [<HttpGet>]
        override this.GetCollection( domainType,  instanceId,  propertyName) = base.GetCollection(domainType, instanceId, propertyName);

        [<HttpGet>]
        override this.GetAction( domainType,  instanceId,  actionName) = base.GetAction(domainType, instanceId, actionName);

        [<HttpPut>]
        override this.PutProperty( domainType,  instanceId,  propertyName,  argument) = base.PutProperty(domainType, instanceId, propertyName, argument);

        [<HttpDelete>]
        override this.DeleteProperty( domainType,  instanceId,  propertyName) = base.DeleteProperty(domainType, instanceId, propertyName);

        [<HttpPost>]
        override this.PostCollection( domainType,  instanceId,  propertyName,  argument) = base.PostCollection(domainType, instanceId, propertyName, argument);

        [<HttpDelete>]
        override this.DeleteCollection( domainType,  instanceId,  propertyName,  argument) = base.DeleteCollection(domainType, instanceId, propertyName, argument);

        [<HttpGet>]
        override this.GetInvoke( domainType,  instanceId,  actionName,  arguments) = base.GetInvoke(domainType, instanceId, actionName, arguments);

        [<HttpPost>]
        override this.PostInvoke( domainType,  instanceId,  actionName,  arguments) = base.PostInvoke(domainType, instanceId, actionName, arguments);

        [<HttpPut>]
        override this.PutInvoke( domainType,  instanceId,  actionName,  arguments) = base.PutInvoke(domainType, instanceId, actionName, arguments);

        [<HttpGet>]
        override this.GetInvokeOnService( serviceName,  actionName,  arguments) = base.GetInvokeOnService(serviceName, actionName, arguments);

        [<HttpPut>]
        override this.PutInvokeOnService( serviceName,  actionName,  arguments) = base.PutInvokeOnService(serviceName, actionName, arguments);

        [<HttpPost>]
        override this.PostInvokeOnService( serviceName,  actionName,  arguments) = base.PostInvokeOnService(serviceName, actionName, arguments);

        [<HttpGet>]
        override this.GetInvokeTypeActions( typeName,  actionName,  arguments) = base.GetInvokeTypeActions(typeName, actionName, arguments);

        [<HttpPut>]
        override this.PutPersistPropertyPrompt( domainType,  propertyName,  promptArguments) = base.PutPersistPropertyPrompt(domainType, propertyName, promptArguments);

        [<HttpGet>]
        override this.GetPropertyPrompt( domainType,  instanceId,  propertyName,  arguments) = base.GetPropertyPrompt(domainType, instanceId, propertyName, arguments);

        [<HttpGet>]
        override this.GetParameterPrompt( domainType,  instanceId,  actionName,  parmName,  arguments) = base.GetParameterPrompt(domainType, instanceId, actionName, parmName, arguments);

        [<HttpGet>]
        override this.GetParameterPromptOnService( serviceName,  actionName,  parmName,  arguments) = base.GetParameterPromptOnService(serviceName, actionName, parmName, arguments);

        [<HttpGet>]
        override this.GetCollectionValue( domainType,  instanceId,  propertyName) = base.GetCollectionValue(domainType, instanceId, propertyName);
