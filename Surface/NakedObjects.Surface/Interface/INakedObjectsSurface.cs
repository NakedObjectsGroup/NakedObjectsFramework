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
        void Start();
        void End(bool success);
        IPrincipal GetUser();
        IOidStrategy OidStrategy { get; }
        ObjectContextSurface GetService(ILinkObjectId serviceName);
        ListContextSurface GetServices();

        IMenu[] GetMainMenus();

        ObjectContextSurface GetObject(INakedObjectSurface nakedObject);
        ObjectContextSurface RefreshObject(INakedObjectSurface nakedObject, ArgumentsContext arguments);
        ObjectContextSurface GetObject(ILinkObjectId objectId);
        ObjectContextSurface PutObject(ILinkObjectId objectId, ArgumentsContext arguments);
        PropertyContextSurface GetProperty(ILinkObjectId objectId, string propertyName);

        ListContextSurface GetPropertyCompletions(ILinkObjectId objectId, string propertyName, ArgumentsContext arguments);
        ListContextSurface GetParameterCompletions(ILinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments);
        ListContextSurface GetServiceParameterCompletions(ILinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments);

        ActionContextSurface GetServiceAction(ILinkObjectId serviceName, string actionName);
        ActionContextSurface GetObjectAction(ILinkObjectId objectId, string actionName);
        PropertyContextSurface PutProperty(ILinkObjectId objectId, string propertyName, ArgumentContext argument);
        PropertyContextSurface DeleteProperty(ILinkObjectId objectId, string propertyName, ArgumentContext argument);

        ActionResultContextSurface ExecuteObjectAction(ILinkObjectId objectId, string actionName, ArgumentsContext arguments);
        ActionResultContextSurface ExecuteServiceAction(ILinkObjectId serviceName, string actionName, ArgumentsContext arguments);
        ObjectContextSurface GetImage(string imageId);
        INakedObjectSpecificationSurface[] GetDomainTypes();
        INakedObjectSpecificationSurface GetDomainType(string typeName);
        PropertyTypeContextSurface GetPropertyType(string typeName, string propertyName);
        ActionTypeContextSurface GetActionType(string typeName, string actionName);
        ParameterTypeContextSurface GetActionParameterType(string typeName, string actionName, string parmName);
        ObjectContextSurface Persist(string typeName, ArgumentsContext arguments);
        UserCredentials Validate(string user, string password);

        // todo this to help the transition may be able to be removed after

        INakedObjectSurface GetObject(INakedObjectSpecificationSurface spec,  object domainObject);

    }
}