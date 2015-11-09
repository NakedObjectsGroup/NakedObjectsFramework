// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class PagedListRepresentation : ListRepresentation {

        private static IObjectFacade Page(ObjectContextFacade objectContext, RestControlFlags flags) {
         
            return objectContext.Target.Page(flags.Page, flags.PageSize);
        }

        protected PagedListRepresentation(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(oidStrategy, Page(objectContext, flags), req, flags, actionContext) {
          
            SetPagination(objectContext.Target, flags);
        }

        [DataMember(Name = JsonPropertyNames.Pagination)]
        public MapRepresentation Pagination { get; set; }

      
        // custom extension for pagination 
        private void SetPagination(IObjectFacade list, RestControlFlags flags) {
            Pagination = new MapRepresentation();

            var totalCount = list.Count();
            var pageSize = flags.PageSize ;
            var page = flags.Page;
            var numPages = (int)Math.Round(totalCount / (decimal)pageSize + 0.5m);
            numPages = numPages == 0 ? 1 : numPages;

            var exts = new Dictionary<string, object> {
                {"page", page},
                {"pageSize", pageSize},
                {"numPages", numPages},
                {"totalCount", totalCount}
            };

            Pagination = RestUtils.CreateMap(exts);
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PagedListRepresentation(oidStrategy, actionResultContext.Result, req, flags, actionResultContext.ActionContext);
        }
    }
}