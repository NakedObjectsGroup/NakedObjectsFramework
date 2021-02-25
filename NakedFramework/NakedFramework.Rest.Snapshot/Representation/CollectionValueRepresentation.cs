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
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Facade;
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class CollectionValueRepresentation : Representation {
        protected CollectionValueRepresentation(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(propertyContext);
            SetValue(propertyContext, req, flags);
            SelfRelType = new CollectionValueRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, propertyContext));
            SetLinks(req, propertyContext, new ObjectRelType(RelValues.Up, new UriMtHelper(oidStrategy, req, propertyContext.Target)));
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

        private void SetValue(PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags) {
            var collectionItems = propertyContext.Property.GetValue(propertyContext.Target).ToEnumerable();
            Value = collectionItems.Select(i => LinkRepresentation.Create(OidStrategy, new ValueRelType(propertyContext.Property, new UriMtHelper(OidStrategy, req, i)), flags, new OptionalProperty(JsonPropertyNames.Title, RestUtils.SafeGetTitle(i)))).ToArray();
        }

        private void SetScalars(PropertyContextFacade propertyContext) => Id = propertyContext.Property.Id;

        private void SetExtensions() => Extensions = new MapRepresentation();

        private void SetLinks(HttpRequest req, PropertyContextFacade propertyContext, RelType parentRelType) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, parentRelType, Flags),
                LinkRepresentation.Create(OidStrategy, SelfRelType, Flags)
            };

            Links = tempLinks.ToArray();
        }

        private void SetHeader(IObjectFacade target) {
            Caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static CollectionValueRepresentation Create(IOidStrategy oidStrategy, PropertyContextFacade propertyContext, HttpRequest req, RestControlFlags flags) => new CollectionValueRepresentation(oidStrategy, propertyContext, req, flags);
    }
}