// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Principal;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Interface;

namespace NakedObjects.Surface {
    public interface INakedObjectsSurface {
        IOidTranslator OidTranslator { get; }
        IOidStrategy OidStrategy { get; }
        IMessageBrokerSurface MessageBroker { get; }
        void Start();
        void End(bool success);
        IPrincipal GetUser();
        ObjectContextSurface GetService(IOidTranslation serviceName);
        ListContextSurface GetServices();
        IMenuFacade[] GetMainMenus();
        ObjectContextSurface GetObject(INakedObjectSurface nakedObject);
        ObjectContextSurface RefreshObject(INakedObjectSurface nakedObject, ArgumentsContext arguments);
        ObjectContextSurface GetObject(IOidTranslation objectId);
        ObjectContextSurface PutObject(IOidTranslation objectId, ArgumentsContext arguments);
        PropertyContextSurface GetProperty(IOidTranslation objectId, string propertyName);
        ListContextSurface GetPropertyCompletions(IOidTranslation objectId, string propertyName, ArgumentsContext arguments);
        ListContextSurface GetParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContext arguments);
        ListContextSurface GetServiceParameterCompletions(IOidTranslation objectId, string actionName, string parmName, ArgumentsContext arguments);
        ActionContextSurface GetServiceAction(IOidTranslation serviceName, string actionName);
        ActionContextSurface GetObjectAction(IOidTranslation objectId, string actionName);
        PropertyContextSurface PutProperty(IOidTranslation objectId, string propertyName, ArgumentContext argument);
        PropertyContextSurface DeleteProperty(IOidTranslation objectId, string propertyName, ArgumentContext argument);
        ActionResultContextSurface ExecuteListAction(IOidTranslation[] objectId, INakedObjectSpecificationSurface elementSpec, string actionName, ArgumentsContext arguments);
        ActionResultContextSurface ExecuteObjectAction(IOidTranslation objectId, string actionName, ArgumentsContext arguments);
        ActionResultContextSurface ExecuteServiceAction(IOidTranslation serviceName, string actionName, ArgumentsContext arguments);
        ObjectContextSurface GetImage(string imageId);
        INakedObjectSpecificationSurface[] GetDomainTypes();
        INakedObjectSpecificationSurface GetDomainType(string typeName);
        PropertyTypeContextSurface GetPropertyType(string typeName, string propertyName);
        ActionTypeContextSurface GetActionType(string typeName, string actionName);
        ParameterTypeContextSurface GetActionParameterType(string typeName, string actionName, string parmName);
        ObjectContextSurface Persist(string typeName, ArgumentsContext arguments);
        UserCredentials Validate(string user, string password);
        // todo this to help the transition may be able to be removed after

        INakedObjectSurface GetObject(INakedObjectSpecificationSurface spec, object domainObject);
        INakedObjectSurface GetObject(object domainObject);
        // todo temp wrap - probably remove actionresult model ? 
        object Wrap(object arm, INakedObjectSurface oldNakedObject);
    }
}