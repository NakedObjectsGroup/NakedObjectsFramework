// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Error;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Rest.Snapshot.Utility;

public static class SnapshotFactory {
    private static ActionContextFacade GetServiceActionByName(this IFrameworkFacade frameworkFacade, string serviceName, string actionName)
    {
        var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
        return frameworkFacade.GetServiceAction(oid, actionName);
    }

    private static ActionContextFacade GetMenuActionByName(this IFrameworkFacade frameworkFacade, string menuName, string actionName) => frameworkFacade.GetMenuAction(menuName, actionName);


    private static ObjectContextFacade GetServiceByName(this IFrameworkFacade frameworkFacade, string serviceName)
    {
        var oid = frameworkFacade.OidTranslator.GetOidTranslation(serviceName);
        return frameworkFacade.GetService(oid);
    }

    private static IMenuFacade GetMenuByName(this IFrameworkFacade frameworkFacade, string menuName)
    {
        if (string.IsNullOrEmpty(menuName))
        {
            throw new BadRequestNOSException();
        }

        var menu = frameworkFacade.GetMainMenus().List.SingleOrDefault(m => m.Id == menuName);
        return menu ?? throw new MenuResourceNotFoundNOSException(menuName);
    }


    private static bool isNakedFunctions = false;


    public static Func<RestSnapshot> ServicesSnapshot(IFrameworkFacade frameworkFacade, HttpRequest req, RestControlFlags flags)
        =>  (isNakedFunctions) 
            ? MenusSnapshot(frameworkFacade, frameworkFacade.GetMainMenus, req, flags)    
            : ServicesSnapshot(frameworkFacade, frameworkFacade.GetServices, req, flags);

    private static Func<RestSnapshot> ServicesSnapshot(IFrameworkFacade frameworkFacade, Func<ListContextFacade> listContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, listContext(), req, flags);

    public static Func<RestSnapshot> HomeSnapshot(IFrameworkFacade frameworkFacade, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, req, flags);

    public static Func<RestSnapshot> UserSnapshot(IFrameworkFacade frameworkFacade, Func<IPrincipal> user, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, user(), req, flags);

    public static Func<RestSnapshot> ServiceSnapshot(IFrameworkFacade frameworkFacade, string name, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => (isNakedFunctions)
            ? MenuSnapshot(frameworkFacade, name, req, flags)
            : ObjectSnapshot(frameworkFacade, () => frameworkFacade.GetServiceByName(name), req, flags, httpStatusCode);

    public static Func<RestSnapshot> ObjectSnapshot(IFrameworkFacade frameworkFacade, Func<ObjectContextFacade> objectContext, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => () => new RestSnapshot(frameworkFacade, objectContext(), req, flags, httpStatusCode);

    public static Func<RestSnapshot> MenuSnapshot(IFrameworkFacade frameworkFacade, string menuName, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => () => new RestSnapshot(frameworkFacade, frameworkFacade.GetMenuByName(menuName), req, flags, httpStatusCode);

    public static Func<RestSnapshot> MenusSnapshot(IFrameworkFacade frameworkFacade, Func<MenuContextFacade> menus, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, menus(), req, flags);

    public static Func<RestSnapshot> VersionSnapshot(IFrameworkFacade frameworkFacade, Func<IDictionary<string, string>> capabilities, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, capabilities(), req, flags);

    public static Func<RestSnapshot> ActionSnapshot(IFrameworkFacade frameworkFacade, string name, string actionName, HttpRequest req, RestControlFlags flags)
        => isNakedFunctions
        ? MenuActionSnapshot(frameworkFacade, name, actionName, req, flags)
        : ServiceActionSnapshot(frameworkFacade, name, actionName, req, flags);

    public static Func<RestSnapshot> MenuActionSnapshot(IFrameworkFacade frameworkFacade, string name, string actionName, HttpRequest req, RestControlFlags flags)
        => ActionSnapshot(frameworkFacade, () => frameworkFacade.GetMenuActionByName(name, actionName), req, flags);

    public static Func<RestSnapshot> ServiceActionSnapshot(IFrameworkFacade frameworkFacade, string name, string actionName, HttpRequest req, RestControlFlags flags)
        => ActionSnapshot(frameworkFacade, () => frameworkFacade.GetServiceActionByName(name, actionName), req, flags);

    public static Func<RestSnapshot> ActionSnapshot(IFrameworkFacade frameworkFacade, Func<ActionContextFacade> actionContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, actionContext(), req, flags);

    public static Func<RestSnapshot> PromptSnaphot(IFrameworkFacade frameworkFacade, Func<PropertyContextFacade> propertyContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, propertyContext(), req, flags);

    public static Func<RestSnapshot> PromptSnaphot(IFrameworkFacade frameworkFacade, Func<ParameterContextFacade> parameterContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, parameterContext(), req, flags);

    public static Func<RestSnapshot> PropertySnapshot(IFrameworkFacade frameworkFacade, Func<PropertyContextFacade> propertyContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, propertyContext(), req, flags, false);

    public static Func<RestSnapshot> CollectionValueSnapshot(IFrameworkFacade frameworkFacade, Func<PropertyContextFacade> propertyContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, propertyContext(), req, flags, true);

    public static Func<RestSnapshot> ActionResultSnapshot(IFrameworkFacade frameworkFacade, Func<ActionResultContextFacade> actionResultContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, actionResultContext(), req, flags);

    public static Func<RestSnapshot> TypeActionSnapshot(IFrameworkFacade frameworkFacade, Func<TypeActionInvokeContext> typeActionInvokeContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, typeActionInvokeContext(), req, flags);

    public static Func<RestSnapshot> ErrorSnapshot(IFrameworkFacade frameworkFacade, Exception ex, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, frameworkFacade, ex, req, flags);
}