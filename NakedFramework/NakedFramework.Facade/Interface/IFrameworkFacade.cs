// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Translation;

namespace NakedFramework.Facade.Interface;

public interface IFrameworkFacade {
    IOidTranslator OidTranslator { get; }
    IOidStrategy OidStrategy { get; }
    IMessageBrokerFacade MessageBroker { get; }
    string[] ServerTypes { get; }

    IServiceProvider GetScopedServiceProvider { get; }
    void Start();
    void End(bool success);
    IPrincipal GetUser();
    ObjectContextFacade GetService(IOidTranslation serviceName);
    ListContextFacade GetServices();
    MenuContextFacade GetMainMenus();
    ObjectContextFacade GetObject(IObjectFacade objectFacade);
    ObjectContextFacade GetObject(IOidTranslation objectId);
    ObjectContextFacade PutObject(IOidTranslation objectId, ArgumentsContextFacade arguments);
    PropertyContextFacade GetProperty(IOidTranslation objectId, string propertyName);
    PropertyContextFacade GetPropertyWithCompletions(IObjectFacade transient, string propertyName, ArgumentsContextFacade arguments);
    ActionContextFacade GetServiceAction(IOidTranslation serviceName, string actionName);
    ActionContextFacade GetMenuAction(string menuName, string actionName);
    ActionContextFacade GetObjectAction(IOidTranslation objectId, string actionName);
    ActionContextFacade GetServiceActionWithCompletions(IOidTranslation serviceName, string actionName, string parmName, ArgumentsContextFacade arguments);
    ActionContextFacade GetMenuActionWithCompletions(string menuName, string actionName, string parmName, ArgumentsContextFacade arguments);
    ActionContextFacade GetObjectActionWithCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments);
    PropertyContextFacade PutProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument);
    PropertyContextFacade DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument);
    ActionResultContextFacade ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContextFacade arguments);
    ActionResultContextFacade ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContextFacade arguments);
    ActionResultContextFacade ExecuteMenuAction(string menuName, string actionName, ArgumentsContextFacade argsContext);
    ObjectContextFacade GetImage(string imageId);
    ITypeFacade GetDomainType(string typeName);
    ObjectContextFacade Persist(string typeName, ArgumentsContextFacade arguments);
    ObjectContextFacade GetTransient(string typeName, ArgumentsContextFacade arguments);

    IObjectFacade GetObject(object domainObject);

    // Do not remove; used in custom code
    void Inject(object toInject);
    (string, ActionContextFacade)[] GetMenuItem(IMenuItemFacade item, string parent = "");

    ActionContextFacade[] GetLocallyContributedActions(PropertyContextFacade propertyContext);
}