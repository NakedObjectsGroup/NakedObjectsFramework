// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.AspNetCore.Routing;

namespace NakedObjects.Rest.App.Demo {
    public class RestfulObjectsConfig {
        public static string RestRoot {
            get { return ""; }
        }

        public static void RegisterRestfulObjectsRoutes(IRouteBuilder routes) {
            if (RestRoot != null) {
                RestfulObjectsRouting.AddRestRoutes(routes, RestRoot);
            }
        }

        public static void RestPreStart() {
            if (RestRoot != null) {
                // to show debug information in HTTP message headers 
                RestfulObjectsControllerBase.DebugWarnings = true;

                // to make whole application 'read only' 
                // RestfulObjectsControllerBase.IsReadOnly = true;

                // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
                // 0, 3600, 86400 are the defaults 
                // no caching makes debugging easier
                RestfulObjectsControllerBase.CacheSettings = (0, 0, 0);

                // make Accept header handling non-strict (RO spec 2.4.4)
                // RestfulObjectsControllerBase.AcceptHeaderStrict = false;

                // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
                // RestfulObjectsControllerBase.DefaultPageSize = 50; 

                // These flags control Member Representations - if true the 'details' will be included 
                // in the the member. This will increase the size of the initial representation but reduce 
                // the number of messages.   

                RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations = false;

                RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations = false;

                RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations = false;
            }
        }
    }
}