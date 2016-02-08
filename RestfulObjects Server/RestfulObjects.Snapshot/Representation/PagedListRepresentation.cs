// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class PagedListRepresentation : ListRepresentation {
        protected PagedListRepresentation(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(oidStrategy, Page(objectContext, flags), req, flags, actionContext) {
            SetPagination(objectContext.Target, flags);
            SetActions(oidStrategy, objectContext, req, flags);
        }

        [DataMember(Name = JsonPropertyNames.Pagination)]
        public MapRepresentation Pagination { get; set; }

        [DataMember(Name = JsonPropertyNames.Members)]
        public MapRepresentation Members { get; set; }

        private static IObjectFacade Page(ObjectContextFacade objectContext, RestControlFlags flags) {
            return objectContext.Target.Page(flags.Page, flags.PageSize);
        }

        // custom extension for pagination 
        private void SetPagination(IObjectFacade list, RestControlFlags flags) {
            Pagination = new MapRepresentation();

            var totalCount = list.Count();
            var pageSize = flags.PageSize;
            var page = flags.Page;
            var numPages = (int) Math.Round(totalCount/(decimal) pageSize + 0.5m);
            numPages = numPages == 0 ? 1 : numPages;

            var exts = new Dictionary<string, object> {
                {JsonPropertyNames.Page, page},
                {JsonPropertyNames.PageSize, pageSize},
                {JsonPropertyNames.NumPages, numPages},
                {JsonPropertyNames.TotalCount, totalCount}
            };

            Pagination = RestUtils.CreateMap(exts);
        }

        private void SetActions(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags) {
            InlineActionRepresentation[] actions = objectContext.VisibleActions.Select(a => InlineActionRepresentation.Create(oidStrategy, req, a, flags)).ToArray();
            Members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PagedListRepresentation(oidStrategy, actionResultContext.Result, req, flags, actionResultContext.ActionContext);
        }
    }
}