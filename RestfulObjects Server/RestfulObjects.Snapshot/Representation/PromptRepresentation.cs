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
    public class PromptRepresentation : Representation {

        private static UriMtHelper GetSelfHelper(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequestMessage req) {
            if (!RestControlFlags.ProtoPersistentObjects && propertyContext.Target.IsTransient) {
                return new UriMtHelper(oidStrategy, req, propertyContext, propertyContext.UniqueIdForTransient.GuidAsKey());
            }
            return new UriMtHelper(oidStrategy, req, propertyContext);
        }

        private static UriMtHelper GetParentHelper(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequestMessage req) {
            if (!RestControlFlags.ProtoPersistentObjects && propertyContext.Target.IsTransient) {
                return new UriMtHelper(oidStrategy, req, propertyContext.Target, propertyContext.UniqueIdForTransient.GuidAsKey());
            }
            return new UriMtHelper(oidStrategy, req, propertyContext.Target);
        }

        protected PromptRepresentation(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(propertyContext.Property.Id);
            SetChoices(listContext, propertyContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, GetSelfHelper(oidStrategy, propertyContext, req));
            SetLinks(req, listContext.ElementType, new ObjectRelType(RelValues.Up, GetParentHelper(oidStrategy, propertyContext, req)));
            SetExtensions();
            SetHeader(listContext.IsListOfServices);
        }

        protected PromptRepresentation(IOidStrategy oidStrategy, ParameterContextFacade parmContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(parmContext.Id);
            SetChoices(listContext, parmContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, parmContext));
            var helper = new UriMtHelper(oidStrategy, req, parmContext.Target);
            ObjectRelType parentRelType = parmContext.Target.Specification.IsService ? new ServiceRelType(RelValues.Up, helper) : new ObjectRelType(RelValues.Up, helper);
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

        private void SetChoices(ListContextFacade listContext, PropertyContextFacade propertyContext, HttpRequestMessage req) {
            Choices = listContext.List.Select(c => RestUtils.GetChoiceValue(OidStrategy, req, c, propertyContext.Property, Flags)).ToArray();
        }

        private void SetChoices(ListContextFacade listContext, ParameterContextFacade paramContext, HttpRequestMessage req) {
            Choices = listContext.List.Select(c => RestUtils.GetChoiceValue(OidStrategy, req, c, paramContext.Parameter, Flags)).ToArray();
        }

        private void SetScalars(string id) {
            Id = id;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req, ITypeFacade spec, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, parentRelType, Flags),
                LinkRepresentation.Create(OidStrategy, SelfRelType, Flags)
            };

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy, new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(OidStrategy, req, spec)), Flags));
            }

            Links = tempLinks.ToArray();
        }

        private void SetHeader(bool isListOfServices) {
            caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;
        }

        private LinkRepresentation CreateObjectLink(IOidStrategy oidStrategy, HttpRequestMessage req, IObjectFacade no) {
            var helper = new UriMtHelper(oidStrategy, req, no);
            var rt = new ObjectRelType(RelValues.Element, helper);

            return LinkRepresentation.Create(oidStrategy, rt, Flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(no)));
        }

        public static PromptRepresentation Create(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PromptRepresentation(oidStrategy, propertyContext, listContext, req, flags);
        }

        public static Representation Create(IOidStrategy oidStrategy, ParameterContextFacade parmContext, ListContextFacade listContext, HttpRequestMessage req, RestControlFlags flags) {
            return new PromptRepresentation(oidStrategy, parmContext, listContext, req, flags);
        }
    }
}