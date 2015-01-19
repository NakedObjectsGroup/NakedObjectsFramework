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
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class CollectionValueRepresentation : Representation {
        protected CollectionValueRepresentation(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags)
            : base(flags) {
            SetScalars(propertyContext);
            SetValue(propertyContext, req, flags);
            SelfRelType = new CollectionValueRelType(RelValues.Self, new UriMtHelper(req, propertyContext));
            SetLinks(req, propertyContext, new ObjectRelType(RelValues.Up, new UriMtHelper(req, propertyContext.Target)));
            SetExtensions();
            SetHeader(propertyContext.Target);
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        private void SetValue(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags) {
            IEnumerable<INakedObjectSurface> collectionItems = propertyContext.Property.GetNakedObject(propertyContext.Target).ToEnumerable();
            Value = collectionItems.Select(i => LinkRepresentation.Create(new ValueRelType(propertyContext.Property, new UriMtHelper(req, i)), flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(i)))).ToArray();
        }

        private void SetScalars(PropertyContextSurface propertyContext) {
            Id = propertyContext.Property.Id;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req, PropertyContextSurface propertyContext, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(parentRelType, Flags),
                LinkRepresentation.Create(SelfRelType, Flags)
            };

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, propertyContext.Property)), Flags));
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(req, propertyContext.Property.ElementSpecification)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        private void SetHeader(INakedObjectSurface target) {
            caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static CollectionValueRepresentation Create(PropertyContextSurface propertyContext, HttpRequestMessage req, RestControlFlags flags) {
            return new CollectionValueRepresentation(propertyContext, req, flags);
        }
    }
}