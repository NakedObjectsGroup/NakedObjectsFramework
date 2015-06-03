// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Principal;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Translation;

namespace NakedObjects.Facade {
    public interface IFrameworkFacade {
        IOidTranslator OidTranslator { get; }
        IOidStrategy OidStrategy { get; }
        IMessageBrokerSurface MessageBroker { get; }
        void Start();
        void End(bool success);
        IPrincipal GetUser();
        ObjectContextFacade GetService(IOidTranslation serviceName);
        ListContextFacade GetServices();
        IMenuFacade[] GetMainMenus();
        ObjectContextFacade GetObject(IObjectFacade nakedObject);
        ObjectContextFacade RefreshObject(IObjectFacade nakedObject, ArgumentsContextFacade arguments);
        ObjectContextFacade GetObject(IOidTranslation objectId);
        ObjectContextFacade PutObject(IOidTranslation objectId, ArgumentsContextFacade arguments);
        PropertyContextFacade GetProperty(IOidTranslation objectId, string propertyName);
        ListContextFacade GetPropertyCompletions(IOidTranslation objectId, string propertyName, ArgumentsContextFacade arguments);
        ListContextFacade GetParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments);
        ListContextFacade GetServiceParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContextFacade arguments);
        ActionContextFacade GetServiceAction(IOidTranslation serviceName, string actionName);
        ActionContextFacade GetObjectAction(IOidTranslation objectId, string actionName);
        PropertyContextFacade PutProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument);
        PropertyContextFacade DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContextFacade argument);
        ActionResultContextFacade ExecuteListAction(IOidTranslation[] objectId, ITypeFacade elementSpec, string actionName, ArgumentsContextFacade arguments);
        ActionResultContextFacade ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContextFacade arguments);
        ActionResultContextFacade ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContextFacade arguments);
        ObjectContextFacade GetImage(string imageId);
        ITypeFacade[] GetDomainTypes();
        ITypeFacade GetDomainType(string typeName);
        PropertyTypeContextFacade GetPropertyType(string typeName, string propertyName);
        ActionTypeContextFacade GetActionType(string typeName, string actionName);
        ParameterTypeContextFacade GetActionParameterType(string typeName, string actionName, string parmName);
        ObjectContextFacade Persist(string typeName, ArgumentsContextFacade arguments);
        UserCredentials Validate(string user, string password);
        // todo this to help the transition may be able to be removed after

        IObjectFacade GetObject(ITypeFacade spec, object domainObject);
        IObjectFacade GetObject(object domainObject);
        // todo temp wrap - probably remove actionresult model ? 
        object Wrap(object arm, IObjectFacade oldNakedObject);
    }
}