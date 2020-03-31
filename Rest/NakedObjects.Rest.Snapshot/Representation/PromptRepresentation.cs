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
    public class PromptRepresentation : Representation {
        protected PromptRepresentation(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(propertyContext.Property.Id);
            SetChoices(propertyContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, GetSelfHelper(oidStrategy, propertyContext, req));
            SetLinks(req, propertyContext.Completions.ElementType, new ObjectRelType(RelValues.Up, GetParentHelper(oidStrategy, propertyContext, req)));
            SetExtensions();
            SetHeader(propertyContext.Completions.IsListOfServices);
        }

        protected PromptRepresentation(IOidStrategy oidStrategy, ParameterContextFacade parmContext, HttpRequest req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(parmContext.Id);
            SetChoices(parmContext, req);
            SelfRelType = new PromptRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, parmContext));
            var helper = new UriMtHelper(oidStrategy, req, parmContext.Target);
            var parentRelType = parmContext.Target.Specification.IsService ? new ServiceRelType(RelValues.Up, helper) : new ObjectRelType(RelValues.Up, helper);
            SetLinks(req, parmContext.Completions.ElementType, parentRelType);
            SetExtensions();
            SetHeader(parmContext.Completions.IsListOfServices);
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Choices)]
        public object[] Choices { get; set; }

        private static UriMtHelper GetSelfHelper(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req) => new UriMtHelper(oidStrategy, req, propertyContext);

        private static UriMtHelper GetParentHelper(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req) => new UriMtHelper(oidStrategy, req, propertyContext.Target);

        private void SetChoices(PropertyContextFacade propertyContext, HttpRequest req) {
            Choices = propertyContext.Completions.List.Select(c => RestUtils.GetChoiceValue(OidStrategy, req, c, propertyContext.Property, Flags)).ToArray();
        }

        private void SetChoices(ParameterContextFacade paramContext, HttpRequest req) {
            Choices = paramContext.Completions.List.Select(c => RestUtils.GetChoiceValue(OidStrategy, req, c, paramContext.Parameter, Flags)).ToArray();
        }

        private void SetScalars(string id) {
            Id = id;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequest req, ITypeFacade spec, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, parentRelType, Flags),
                LinkRepresentation.Create(OidStrategy, SelfRelType, Flags)
            };

            Links = tempLinks.ToArray();
        }

        private void SetHeader(bool isListOfServices) {
            Caching = isListOfServices ? CacheType.NonExpiring : CacheType.Transactional;
        }

        public static PromptRepresentation Create(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags) => new PromptRepresentation(oidStrategy, propertyContext, req, flags);

        public static Representation Create(IOidStrategy oidStrategy, ParameterContextFacade parmContext, HttpRequest req, RestControlFlags flags) => new PromptRepresentation(oidStrategy, parmContext, req, flags);
    }
}