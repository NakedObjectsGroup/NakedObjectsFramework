// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Configuration;

namespace NakedFramework.Rest.Extensions {
    public static class RestfulObjectsExtensions {
        public static void AddRestfulObjects(this NakedCoreOptions coreOptions, Action<RestfulObjectsOptions> setupAction = null) {
            var options = new RestfulObjectsOptions();
            setupAction?.Invoke(options);

            coreOptions.Services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
            coreOptions.Services.AddSingleton<IRestfulObjectsConfiguration>(p => RestfulObjectsConfiguration(options));
        }

        private static RestfulObjectsConfiguration RestfulObjectsConfiguration(RestfulObjectsOptions options) {
            var config = new RestfulObjectsConfiguration {
                DebugWarnings = options.DebugWarnings,
                IsReadOnly = options.IsReadOnly,
                CacheSettings = options.CacheSettings,
                AcceptHeaderStrict = options.AcceptHeaderStrict,
                DefaultPageSize = options.DefaultPageSize,
                InlineDetailsInActionMemberRepresentations = options.InlineDetailsInActionMemberRepresentations,
                InlineDetailsInCollectionMemberRepresentations = options.InlineDetailsInCollectionMemberRepresentations,
                InlineDetailsInPropertyMemberRepresentations = options.InlineDetailsInPropertyMemberRepresentations,
                ProtoPersistentObjects = options.ProtoPersistentObjects,
                DeleteObjects = options.DeleteObjects,
                ValidateOnly = options.ValidateOnly,
                DomainModel = options.DomainModel,
                BlobsClobs = options.BlobsClobs,
                InlinedMemberRepresentations = options.InlinedMemberRepresentations,
                AllowMutatingActionOnImmutableObject = options.AllowMutatingActionOnImmutableObject
            };

            return config;
        }

        public static void UseRestfulObjects(this IApplicationBuilder app, string restRoot = "") {
            restRoot ??= "";
            app.UseMvc(routeBuilder => RestfulObjectsRouting.AddRestRoutes(routeBuilder, restRoot));
        }
    }
}