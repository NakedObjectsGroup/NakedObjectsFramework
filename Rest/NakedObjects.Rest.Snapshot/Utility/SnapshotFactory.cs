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
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Rest.Snapshot.Utility {
    public static class SnapshotFactory {
        public static Func<RestSnapshot> ServicesSnapshot(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequest req, RestControlFlags flags)
            => () => new RestSnapshot(oidStrategy, listContext, req, flags);

        public static Func<RestSnapshot> HomeSnapshot(IOidStrategy oidStrategy, HttpRequest req, RestControlFlags flags)
            => () => new RestSnapshot(oidStrategy, req, flags);

        public static Func<RestSnapshot> UserSnapshot(IOidStrategy oidStrategy, IPrincipal user, HttpRequest req, RestControlFlags flags)
            => () => new RestSnapshot(oidStrategy, user, req, flags);

        public static Func<RestSnapshot> ObjectSnapshot(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
            => () => new RestSnapshot(oidStrategy, objectContext, req, flags, httpStatusCode);

        public static Func<RestSnapshot> MenuSnapshot(IOidStrategy oidStrategy, IMenuFacade menu, HttpRequest req, RestControlFlags flags, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
            => () => new RestSnapshot(oidStrategy, menu, req, flags, httpStatusCode);

        public static Func<RestSnapshot> MenusSnapshot(IOidStrategy oidStrategy, MenuContextFacade menus, HttpRequest req, RestControlFlags flags)
            => () => new RestSnapshot(oidStrategy, menus, req, flags);

        public static Func<RestSnapshot> VersionSnapshot(IOidStrategy oidStrategy, IDictionary<string, string> capabilities, HttpRequest req, RestControlFlags flags)
            => () => new RestSnapshot(oidStrategy, capabilities, req, flags);
    }
}