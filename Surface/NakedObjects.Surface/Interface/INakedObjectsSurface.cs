// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;
using NakedObjects.Surface.Context;

namespace NakedObjects.Surface {
    public interface INakedObjectsSurface {
        void Start();
        void End(bool success);
        IPrincipal GetUser();
        ObjectContextSurface GetService(LinkObjectId serviceName);
        ListContextSurface GetServices();

        ObjectContextSurface GetObject(INakedObjectSurface nakedObject);
        ObjectContextSurface GetObject(LinkObjectId objectId);
        ObjectContextSurface PutObject(LinkObjectId objectId, ArgumentsContext arguments);
        PropertyContextSurface GetProperty(LinkObjectId objectId, string propertyName);

        ListContextSurface GetPropertyCompletions(LinkObjectId objectId, string propertyName, ArgumentsContext arguments);
        ListContextSurface GetParameterCompletions(LinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments);
        ListContextSurface GetServiceParameterCompletions(LinkObjectId objectId, string actionName, string parmName, ArgumentsContext arguments);

        ActionContextSurface GetServiceAction(LinkObjectId serviceName, string actionName);
        ActionContextSurface GetObjectAction(LinkObjectId objectId, string actionName);
        PropertyContextSurface PutProperty(LinkObjectId objectId, string propertyName, ArgumentContext argument);
        PropertyContextSurface DeleteProperty(LinkObjectId objectId, string propertyName, ArgumentContext argument);
     
        ActionResultContextSurface ExecuteObjectAction(LinkObjectId objectId, string actionName, ArgumentsContext arguments);
        ActionResultContextSurface ExecuteServiceAction(LinkObjectId serviceName, string actionName, ArgumentsContext arguments);
        ObjectContextSurface GetImage(string imageId);
        INakedObjectSpecificationSurface[] GetDomainTypes();
        INakedObjectSpecificationSurface GetDomainType(string typeName);
        PropertyTypeContextSurface GetPropertyType(string typeName, string propertyName);
        ActionTypeContextSurface GetActionType(string typeName, string actionName);
        ParameterTypeContextSurface GetActionParameterType(string typeName, string actionName, string parmName);
        ObjectContextSurface Persist(string typeName, ArgumentsContext arguments);
        UserCredentials Validate(string user, string password);
    }
}