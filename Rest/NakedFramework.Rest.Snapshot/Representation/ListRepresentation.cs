// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class ListRepresentation : Representation {
        protected ListRepresentation(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequest req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Value = listContext.List.Select(c => CreateObjectLink(oidStrategy, req, c)).ToArray();
            SelfRelType = new ListRelType(RelValues.Self, SegmentValues.Services, new UriMtHelper(oidStrategy, req, listContext.ElementType));
            SetLinks(req);
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        protected ListRepresentation(IOidStrategy oidStrategy, MenuContextFacade menus, HttpRequest req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Value = menus.List.Where(m => m.MenuItems.Any()).Select(c => CreateMenuLink(oidStrategy, req, c)).ToArray();
            SelfRelType = new ListRelType(RelValues.Self, SegmentValues.Menus, new UriMtHelper(oidStrategy, req, menus.ElementType));
            SetLinks(req);
            SetExtensions();
            SetHeader(true);
        }

        protected ListRepresentation(IOidStrategy oidStrategy, IObjectFacade list, HttpRequest req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(oidStrategy, flags) {
            Value = list.ToEnumerable().Select(no => CreateObjectLink(oidStrategy, req, no, actionContext)).ToArray();

            SetLinks(req, actionContext);
            SetExtensions(oidStrategy, actionContext);
            SetHeader(false);
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        private void SetExtensions() => Extensions = new MapRepresentation();

        private void SetExtensions(IOidStrategy oidStrategy, ActionContextFacade actionContext) {
            var exts = new Dictionary<string, object> {
                {
                    JsonPropertyNames.ElementType, RestUtils.SpecToTypeAndFormatString(actionContext.ElementSpecification, oidStrategy, false).typeString
                }
            };

            Extensions = RestUtils.CreateMap(exts);
        }

        private void SetLinks(HttpRequest req) => Links = new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags), LinkRepresentation.Create(OidStrategy, new HomePageRelType(RelValues.Up, new UriMtHelper(OidStrategy, req)), Flags)};

        private void SetLinks(HttpRequest req, ITypeFacade spec) => Links = new LinkRepresentation[] { };

        private void SetLinks(HttpRequest req, ActionContextFacade actionContext) => SetLinks(req, actionContext.ElementSpecification);

        private void SetHeader(bool isListOfServices) => Caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;

        protected virtual LinkRepresentation CreateObjectLink(IOidStrategy oidStrategy, HttpRequest req, IObjectFacade no, ActionContextFacade actionContext = null) {
            var helper = new UriMtHelper(oidStrategy, req, no);
            var rt = no.Specification.IsService ? new ServiceRelType(helper) : new ObjectRelType(RelValues.Element, helper);

            return LinkRepresentation.Create(oidStrategy, rt, Flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        private LinkRepresentation CreateMenuLink(IOidStrategy oidStrategy, HttpRequest req, IMenuFacade menu) {
            var helper = new UriMtHelper(oidStrategy, req, menu);
            var rt = new MenuRelType(helper);

            return LinkRepresentation.Create(oidStrategy, rt, Flags, new OptionalProperty(JsonPropertyNames.Title, menu.Name));
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequest req, RestControlFlags flags) => new ListRepresentation(oidStrategy, listContext, req, flags);

        public static ListRepresentation Create(IOidStrategy oidStrategy, MenuContextFacade menus, HttpRequest req, RestControlFlags flags) => new ListRepresentation(oidStrategy, menus, req, flags);
    }
}