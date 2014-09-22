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
using NakedObjects.Surface;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ListRepresentation : Representation {
        protected ListRepresentation(ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            Value = listContext.List.Select(c => CreateObjectLink(req, c)).ToArray();
            SelfRelType = new ListRelType(RelValues.Self, SegmentValues.Services, new UriMtHelper(req, listContext.ElementType));
            SetLinks(req);
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        protected ListRepresentation(ObjectContextSurface objectContext, HttpRequestMessage req, RestControlFlags flags, ActionContextSurface actionContext)
            : base(flags) {
            INakedObjectSurface list;

            if (flags.PageSize > 0 && objectContext.Target.Count() > flags.PageSize) {
                warnings.Add(string.Format("Result contains more than {0} objects only returning the first {0}", flags.PageSize));
                list = objectContext.Target.Page(1, flags.PageSize);
            }
            else {
                list = objectContext.Target;
            }

            Value = list.ToEnumerable().Select(no => CreateObjectLink(req, no)).ToArray();

            SetLinks(req, actionContext);
            SetExtensions();
            SetHeader(false);
        }

        private ListRepresentation(INakedObjectSpecificationSurface[] specs, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            Value = specs.Select(s => CreateDomainLink(req, s)).ToArray();
            SelfRelType = new TypesRelType(RelValues.Self, new UriMtHelper(req));
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
            Links = new[] {LinkRepresentation.Create(SelfRelType, Flags), LinkRepresentation.Create(new HomePageRelType(RelValues.Up, new UriMtHelper(req)), Flags)};
        }

        private void SetLinks(HttpRequestMessage req, INakedObjectSpecificationSurface spec) {
            var tempLinks = new List<LinkRepresentation>();

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, spec)), Flags));
            }

            Links = tempLinks.ToArray();
        }

        private void SetLinks(HttpRequestMessage req, ActionContextSurface actionContext) {
            SetLinks(req, actionContext.Specification.ElementType);
        }

        private void SetHeader(bool isListOfServices) {
            caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;
        }

        private LinkRepresentation CreateObjectLink(HttpRequestMessage req, INakedObjectSurface no) {
            var helper = new UriMtHelper(req, no);
            ObjectRelType rt = no.Specification.IsService() ? new ServiceRelType(helper) : new ObjectRelType(RelValues.Element, helper);

            return LinkRepresentation.Create(rt, Flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        private LinkRepresentation CreateDomainLink(HttpRequestMessage req, INakedObjectSpecificationSurface spec) {
            return LinkRepresentation.Create(new DomainTypeRelType(new UriMtHelper(req, spec)), Flags);
        }

        public static ListRepresentation Create(ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ListRepresentation(listContext, req, flags);
        }

        public static ListRepresentation Create(ActionResultContextSurface actionResultContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ListRepresentation(actionResultContext.Result, req, flags, actionResultContext.ActionContext);
        }

        internal static Representation Create(INakedObjectSpecificationSurface[] specs, HttpRequestMessage req, RestControlFlags flags) {
            // filter out System types
            specs = specs.Where(s => !s.FullName().StartsWith("System.") && !s.FullName().StartsWith("Microsoft.")).ToArray();
            // filter out predefined types
            specs = specs.Where(s => !RestUtils.IsPredefined(s)).ToArray();
            return new ListRepresentation(specs, req, flags);
        }
    }
}