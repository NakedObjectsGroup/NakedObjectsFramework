// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Rest.Snapshot.Utility;

public static class SnapshotFactory {
    public static Func<RestSnapshot> ServicesSnapshot(IFrameworkFacade frameworkFacade, Func<ListContextFacade> listContext, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, listContext(), req, flags);

    public static Func<RestSnapshot> HomeSnapshot(IFrameworkFacade frameworkFacade, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, req, flags);

    public static Func<RestSnapshot> UserSnapshot(IFrameworkFacade frameworkFacade, Func<IPrincipal> user, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, user(), req, flags);

    public static Func<RestSnapshot> ObjectSnapshot(IFrameworkFacade frameworkFacade, Func<ObjectContextFacade> objectContext, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => () => new RestSnapshot(frameworkFacade, objectContext(), req, flags, httpStatusCode);

    public static Func<RestSnapshot> MenuSnapshot(IFrameworkFacade frameworkFacade, Func<IMenuFacade> menu, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        => () => new RestSnapshot(frameworkFacade, menu(), req, flags, httpStatusCode);

    public static Func<RestSnapshot> MenusSnapshot(IFrameworkFacade frameworkFacade, Func<MenuContextFacade> menus, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade.OidStrategy, menus(), req, flags);

    public static Func<RestSnapshot> VersionSnapshot(IFrameworkFacade frameworkFacade, Func<IDictionary<string, string>> capabilities, HttpRequest req, RestControlFlags flags)
        => () => new RestSnapshot(frameworkFacade, capabilities(), req, flags);

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