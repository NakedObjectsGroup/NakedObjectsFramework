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
    public class DomainTypeRepresentation : Representation {
        protected DomainTypeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, ITypeFacade spec, RestControlFlags flags)
            : base(oidStrategy, flags) {
            var helper = new UriMtHelper(oidStrategy, req, spec);
            SelfRelType = new DomainTypeRelType(RelValues.Self, helper);
            SetScalars(spec);
            SetLinks(helper);
            SetMembers(spec, req);
            SetTypeActions(spec, req);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Name)]
        public string Name { get; set; }

        [DataMember(Name = JsonPropertyNames.DomainType)]
        public string DomainType { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.PluralName)]
        public string PluralName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.IsService)]
        public bool IsService { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Members)]
        public LinkRepresentation[] Members { get; set; }

        [DataMember(Name = JsonPropertyNames.TypeActions)]
        public LinkRepresentation[] TypeActions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetLinks(UriMtHelper helper) {
            Links = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy , SelfRelType, Flags)
                // temp disable icons 
                //LinkRepresentation.Create(new IconRelType(helper), Flags) 
            }.ToArray();
        }

        private void SetScalars(ITypeFacade spec) {
            Name = spec.FullName;
            DomainType = spec.DomainTypeName(OidStrategy);
            FriendlyName = spec.SingularName;
            PluralName = spec.PluralName;
            Description = spec.Description;
            IsService = spec.IsService;
        }

        private void SetTypeActions(ITypeFacade spec, HttpRequestMessage req) {
            TypeActions = new[] {
                LinkRepresentation.Create(OidStrategy,new TypeActionRelType(new UriMtHelper(OidStrategy, req, spec), WellKnownIds.IsSubtypeOf), Flags,
                    new OptionalProperty(JsonPropertyNames.Id, WellKnownIds.IsSubtypeOf),
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.SubType, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Href, null, typeof (object))))))),
                LinkRepresentation.Create(OidStrategy,new TypeActionRelType(new UriMtHelper(OidStrategy, req, spec), WellKnownIds.IsSupertypeOf), Flags,
                    new OptionalProperty(JsonPropertyNames.Id, WellKnownIds.IsSupertypeOf),
                    new OptionalProperty(JsonPropertyNames.Arguments, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.SuperType, MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Href, null, typeof (object)))))))
            };
        }

        private void SetMembers(ITypeFacade spec, HttpRequestMessage req) {
            IAssociationFacade[] properties = spec.Properties.Where(p => !p.IsCollection).ToArray();
            IAssociationFacade[] collections = spec.Properties.Where(p => p.IsCollection).ToArray();
            IActionFacade[] actions = spec.GetActionLeafNodes();

            IEnumerable<LinkRepresentation> propertyMembers = properties.Select(p => LinkRepresentation.Create(OidStrategy, new TypeMemberRelType(RelValues.Property, new UriMtHelper(OidStrategy, req, new PropertyTypeContextFacade { Property = p, OwningSpecification = spec })), Flags));
            IEnumerable<LinkRepresentation> collectionMembers = collections.Select(c => LinkRepresentation.Create(OidStrategy ,new TypeMemberRelType(RelValues.Collection, new UriMtHelper(OidStrategy, req, new PropertyTypeContextFacade { Property = c, OwningSpecification = spec })), Flags));
            IEnumerable<LinkRepresentation> actionMembers = actions.Select(a => LinkRepresentation.Create(OidStrategy ,new TypeMemberRelType(RelValues.Action, new UriMtHelper(OidStrategy, req, new ActionTypeContextFacade { ActionContext = new ActionContextFacade { Action = a }, OwningSpecification = spec })), Flags));

            Members = propertyMembers.Union(collectionMembers).Union(actionMembers).ToArray();
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        public static DomainTypeRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, ITypeFacade spec, RestControlFlags flags) {
            return new DomainTypeRepresentation(oidStrategy ,req, spec, flags);
        }
    }
}