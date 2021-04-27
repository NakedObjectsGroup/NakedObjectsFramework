// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class PagedListRepresentation : ListRepresentation {
        protected PagedListRepresentation(IFrameworkFacade frameworkFacade, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(frameworkFacade, Page(objectContext, flags, actionContext), req, flags, actionContext) {
            SetPagination(objectContext.Target, flags, actionContext);
            SetActions(frameworkFacade.OidStrategy, objectContext, req, flags);
        }

        [DataMember(Name = JsonPropertyNames.Pagination)]
        public MapRepresentation Pagination { get; set; }

        [DataMember(Name = JsonPropertyNames.Members)]
        public MapRepresentation Members { get; set; }

        private static IObjectFacade Page(ObjectContextFacade objectContext, RestControlFlags flags, ActionContextFacade actionContext) => objectContext.Target.Page(flags.Page, PageSize(flags, actionContext), true);

        private static int PageSize(RestControlFlags flags, ActionContextFacade actionContext) => actionContext.Action.PageSize > 0 ? actionContext.Action.PageSize : flags.PageSize;

        // custom extension for pagination 
        private void SetPagination(IObjectFacade list, RestControlFlags flags, ActionContextFacade actionContext) {
            Pagination = new MapRepresentation();

            var totalCount = list.Count();
            var pageSize = PageSize(flags, actionContext);
            var page = flags.Page;
            var numPages = (int) Math.Round(totalCount / (decimal) pageSize + 0.5m);
            numPages = numPages == 0 ? 1 : numPages;

            var exts = new Dictionary<string, object> {
                {JsonPropertyNames.Page, page},
                {JsonPropertyNames.PageSize, pageSize},
                {JsonPropertyNames.NumPages, numPages},
                {JsonPropertyNames.TotalCount, totalCount}
            };

            Pagination = RestUtils.CreateMap(exts);
        }

        private void SetActions(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags) {
            var actions = objectContext.VisibleActions.Select(a => InlineActionRepresentation.Create(oidStrategy, req, a, flags)).ToArray();
            Members = RestUtils.CreateMap(actions.ToDictionary(m => m.Id, m => (object) m));
        }

        public static ListRepresentation Create(IFrameworkFacade frameworkFacade, ActionResultContextFacade actionResultContext, HttpRequest req, RestControlFlags flags) => new PagedListRepresentation(frameworkFacade, actionResultContext.Result, req, flags, actionResultContext.ActionContext);

        private LinkRepresentation CreateTableRowValueLink(IFrameworkFacade frameworkFacade, HttpRequest req, IObjectFacade no, ActionContextFacade actionContext) => RestUtils.CreateTableRowValueLink(no, actionContext, frameworkFacade, req, Flags);

        protected override LinkRepresentation CreateObjectLink(IFrameworkFacade frameworkFacade, HttpRequest req, IObjectFacade no, ActionContextFacade actionContext = null) => !Flags.InlineCollectionItems ? base.CreateObjectLink(frameworkFacade, req, no, actionContext) : CreateTableRowValueLink(frameworkFacade, req, no, actionContext);
    }
}