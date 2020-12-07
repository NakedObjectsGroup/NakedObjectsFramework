// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.AspNetCore.Builder;
using NakedObjects.DependencyInjection.Extensions;

namespace NakedObjects.Rest.Extensions {
    public static class RestfulObjectsExtensions {
        public static void AddRestfulObjects(this NakedCoreOptions coreOptions, Action<RestfulObjectsOptions> setupAction = null) {
            var options = new RestfulObjectsOptions();
            setupAction?.Invoke(options);

            // TODO configure with config object ?
            RestfulObjectsControllerBase.DebugWarnings = options.DebugWarnings;
            RestfulObjectsControllerBase.IsReadOnly = options.IsReadOnly;
            RestfulObjectsControllerBase.CacheSettings = options.CacheSettings;
            RestfulObjectsControllerBase.AcceptHeaderStrict = options.AcceptHeaderStrict;
            RestfulObjectsControllerBase.DefaultPageSize = options.DefaultPageSize;
            RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations = options.InlineDetailsInActionMemberRepresentations;
            RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations = options.InlineDetailsInCollectionMemberRepresentations;
            RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations = options.InlineDetailsInPropertyMemberRepresentations;
        }


        public static void UseRestfulObjects(this IApplicationBuilder app, string restRoot = "") {
            restRoot ??= "";
            app.UseMvc(routeBuilder => RestfulObjectsRouting.AddRestRoutes(routeBuilder, restRoot));
        }
    }
}