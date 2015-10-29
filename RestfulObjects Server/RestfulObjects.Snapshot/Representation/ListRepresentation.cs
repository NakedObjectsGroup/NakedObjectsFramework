// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
    public class ListRepresentation : Representation {
        protected ListRepresentation(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Value = listContext.List.Select(c => CreateObjectLink(oidStrategy, req, c)).ToArray();
            SelfRelType = new ListRelType(RelValues.Self, SegmentValues.Services, new UriMtHelper(oidStrategy, req, listContext.ElementType));
            SetLinks(req);
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        protected ListRepresentation(IOidStrategy oidStrategy, IMenuFacade[] menus, HttpRequestMessage req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Value = menus.Where(m => m.MenuItems.Any()) .Select(c => CreateMenuLink(oidStrategy, req, c)).ToArray();
            SelfRelType = new ListRelType(RelValues.Self, SegmentValues.Services, new UriMtHelper(oidStrategy, req));
            SetLinks(req);
            SetExtensions();
            SetHeader(true);
        }

        protected ListRepresentation(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequestMessage req, RestControlFlags flags, ActionContextFacade actionContext)
            : base(oidStrategy, flags) {
            IObjectFacade list;

            if (flags.PageSize > 0 && objectContext.Target.Count() > flags.PageSize) {
                warnings.Add(string.Format("Result contains more than {0} objects only returning the first {0}", flags.PageSize));
                list = objectContext.Target.Page(1, flags.PageSize);
            }
            else {
                list = objectContext.Target;
            }

            Value = list.ToEnumerable().Select(no => CreateObjectLink(oidStrategy, req, no)).ToArray();

            SetLinks(req, actionContext);
            SetExtensions();
            SetHeader(false);
        }

        private ListRepresentation(IOidStrategy oidStrategy, ITypeFacade[] specs, HttpRequestMessage req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Value = specs.Select(s => CreateDomainLink(oidStrategy, req, s)).ToArray();
            SelfRelType = new TypesRelType(RelValues.Self, new UriMtHelper(oidStrategy, req));
            SetLinks(req);
            SetExtensions();
            SetHeader(true);
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req) {
            Links = new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags), LinkRepresentation.Create(OidStrategy, new HomePageRelType(RelValues.Up, new UriMtHelper(OidStrategy, req)), Flags)};
        }

        private void SetLinks(HttpRequestMessage req, ITypeFacade spec) {
            var tempLinks = new List<LinkRepresentation>();

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy, new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(OidStrategy, req, spec)), Flags));
            }

            Links = tempLinks.ToArray();
        }

        private void SetLinks(HttpRequestMessage req, ActionContextFacade actionContext) {
            SetLinks(req, actionContext.ElementSpecification);
        }

        private void SetHeader(bool isListOfServices) {
            caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;
        }

        private LinkRepresentation CreateObjectLink(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade no) {
            var helper = new UriMtHelper(oidStrategy, req, no);
            ObjectRelType rt = no.Specification.IsService ? new ServiceRelType(helper) : new ObjectRelType(RelValues.Element, helper);

            return LinkRepresentation.Create(oidStrategy, rt, Flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        private LinkRepresentation CreateMenuLink(IOidStrategy oidStrategy, HttpRequestMessage req, IMenuFacade menu) {
            var helper = new UriMtHelper(oidStrategy, req, menu);
            var rt = new MenuRelType(helper);

            return LinkRepresentation.Create(oidStrategy, rt, Flags, new OptionalProperty(JsonPropertyNames.Title, menu.Name));
        }

        private LinkRepresentation CreateDomainLink(IOidStrategy oidStrategy, HttpRequestMessage req, ITypeFacade spec) {
            return LinkRepresentation.Create(oidStrategy, new DomainTypeRelType(new UriMtHelper(oidStrategy, req, spec)), Flags);
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ListRepresentation(oidStrategy, listContext, req, flags);
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, ActionResultContextFacade actionResultContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ListRepresentation(oidStrategy, actionResultContext.Result, req, flags, actionResultContext.ActionContext);
        }

        internal static Representation Create(IOidStrategy oidStrategy, ITypeFacade[] specs, HttpRequestMessage req, RestControlFlags flags) {
            // filter out System types
            specs = specs.Where(s => !s.FullName.StartsWith("System.") && !s.FullName.StartsWith("Microsoft.")).ToArray();
            // filter out predefined types
            specs = specs.Where(s => !RestUtils.IsPredefined(s)).ToArray();
            return new ListRepresentation(oidStrategy, specs, req, flags);
        }

        public static ListRepresentation Create(IOidStrategy oidStrategy, IMenuFacade[] menus, HttpRequestMessage req, RestControlFlags flags) {
            return new ListRepresentation(oidStrategy, menus, req, flags);
        }
    }
}