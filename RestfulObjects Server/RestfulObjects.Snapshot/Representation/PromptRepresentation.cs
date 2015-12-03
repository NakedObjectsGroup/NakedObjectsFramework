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
    public class PromptRepresentation : Representation {
        protected PromptRepresentation(PropertyContextSurface propertyContext, ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            SetScalars(propertyContext.Property.Id);
            SetChoices(listContext, propertyContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, new UriMtHelper(req, propertyContext, flags.OidStrategy));
            SetLinks(req, listContext.ElementType, new ObjectRelType(RelValues.Up, new UriMtHelper(req, propertyContext.Target, flags.OidStrategy)));
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        protected PromptRepresentation(ParameterContextSurface parmContext, ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            SetScalars(parmContext.Id);
            SetChoices(listContext, parmContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, new UriMtHelper(req, parmContext, flags.OidStrategy));
            var helper = new UriMtHelper(req, parmContext.Target, flags.OidStrategy);
            ObjectRelType parentRelType = parmContext.Target.Specification.IsService() ? new ServiceRelType(RelValues.Up, helper) : new ObjectRelType(RelValues.Up, helper);
            SetLinks(req, listContext.ElementType, parentRelType);
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Choices)]
        public object[] Choices { get; set; }

        private void SetChoices(ListContextSurface listContext, PropertyContextSurface propertyContext, HttpRequestMessage req) {
            Choices = listContext.List.Select(c => RestUtils.GetChoiceValue(req, c, propertyContext.Property, Flags)).ToArray();
        }

        private void SetChoices(ListContextSurface listContext, ParameterContextSurface paramContext, HttpRequestMessage req) {
            Choices = listContext.List.Select(c => RestUtils.GetChoiceValue(req, c, paramContext.Parameter, Flags)).ToArray();
        }

        private void SetScalars(string id) {
            Id = id;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation(Flags.OidStrategy);
        }

        private void SetLinks(HttpRequestMessage req, INakedObjectSpecificationSurface spec, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(parentRelType, Flags),
                LinkRepresentation.Create(SelfRelType, Flags)
            };

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, spec, Flags.OidStrategy)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        private void SetHeader(bool isListOfServices) {
            caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;
        }

        private LinkRepresentation CreateObjectLink(HttpRequestMessage req, INakedObjectSurface no) {
            var helper = new UriMtHelper(req, no, Flags.OidStrategy);
            var rt = new ObjectRelType(RelValues.Element, helper);

            return LinkRepresentation.Create(rt, Flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        public static PromptRepresentation Create(PropertyContextSurface propertyContext, ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PromptRepresentation(propertyContext, listContext, req, flags);
        }

        public static Representation Create(ParameterContextSurface parmContext, ListContextSurface listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PromptRepresentation(parmContext, listContext, req, flags);
        }
    }
}