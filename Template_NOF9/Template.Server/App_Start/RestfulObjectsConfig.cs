// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web.Http;
using System.Web.Routing;
using NakedObjects.Rest;
using NakedObjects.Rest.Media;
using System;

namespace NakedObjects.Template {
    public class RestfulObjectsConfig {

        public static void RegisterRestfulObjectsRoutes(RouteCollection routes) {
            if (NakedObjectsRunSettings.RestRoot != null) {
                RestfulObjectsControllerBase.AddRestRoutes(routes, NakedObjectsRunSettings.RestRoot);
            }
        }

        public static void RestPostStart() {
            if (NakedObjectsRunSettings.RestRoot != null) {
                GlobalConfiguration.Configuration.Formatters.Clear();
                GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonNetFormatter(null));
                //GlobalConfiguration.Configuration.MessageHandlers.Add(new AccessControlExposeHeadersHandler());
                //GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationHandler());
            }
        }

        public static void RestPreStart() {
            if (NakedObjectsRunSettings.RestRoot != null) {
                // to make whole application 'read only' 
                //RestfulObjectsControllerBase.IsReadOnly = true;

                // to configure domain model options 
                //RestfulObjectsControllerBase.DomainModel = RestControlFlags.DomainModelType.Selectable; //or Simple, Formal, None

                // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
                // 0, 0, 0 are the defaults (see manual for more explanation)
                //RestfulObjectsControllerBase.CacheSettings = new Tuple<int, int, int>(0, 3600, 86400);

                // to set KeySeparator (for multi-part keys) defaults to "--"
                //RestfulObjectsControllerBase.KeySeparator = "--";

                // make Accept header handling non-strict (RO spec 2.4.4)
                //RestfulObjectsControllerBase.AcceptHeaderStrict = false;

                // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
                //RestfulObjectsControllerBase.DefaultPageSize = 50; 

                // These flags control Member Representations - if true the 'details' will be included 
                // in the the member. This will increase the size of the initial representation but reduce 
                // the number of messages.   

                RestfulObjectsControllerBase.InlineDetailsInActionMemberRepresentations = false;

                RestfulObjectsControllerBase.InlineDetailsInCollectionMemberRepresentations = false;

                RestfulObjectsControllerBase.InlineDetailsInPropertyMemberRepresentations = false;

                // If set to 'true' this will include the etag on immutable objects so that mutating actions 
                // can be invoked on the object without a missing If-Match header error.  
                RestfulObjectsControllerBase.AllowMutatingActionOnImmutableObject = false;
            }
        }
    }
}