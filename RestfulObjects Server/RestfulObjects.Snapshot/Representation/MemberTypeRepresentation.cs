// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade.Contexts;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public abstract class MemberTypeRepresentation : Representation {
        protected MemberTypeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyTypeContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(propertyContext);
            SelfRelType = new TypeMemberRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, propertyContext));
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.MemberOrder)]
        public int MemberOrder { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetScalars(PropertyTypeContextFacade propertyContext) {
            Id = propertyContext.Property.Id;
            FriendlyName = propertyContext.Property.Name;
            Description = propertyContext.Property.Description;
            MemberOrder = propertyContext.Property.MemberOrder;
        }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        protected IList<LinkRepresentation> CreateLinks(HttpRequestMessage req, PropertyTypeContextFacade propertyContext) {
            var domainTypeUri = new UriMtHelper(OidStrategy, req, propertyContext);
            return new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy ,SelfRelType, Flags),
                LinkRepresentation.Create(OidStrategy ,new DomainTypeRelType(RelValues.Up, domainTypeUri), Flags)
            };
        }


        public static MemberTypeRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyTypeContextFacade propertyContext, RestControlFlags flags) {
            if (propertyContext.Property.IsCollection) {
                return CollectionTypeRepresentation.Create(oidStrategy , req, propertyContext, flags);
            }

            Tuple<string, string> typeAndFormat = RestUtils.SpecToTypeAndFormatString( propertyContext.Property.Specification, oidStrategy);

            if (typeAndFormat.Item1 == PredefinedType.String.ToRoString()) {
                var exts = new Dictionary<string, object>();

                AddStringProperties(propertyContext.Property.Specification, propertyContext.Property.MaxLength, propertyContext.Property.Pattern, exts);

                OptionalProperty[] parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();

                return CreateWithOptionals<PropertyTypeRepresentation>(new object[] {oidStrategy ,req, propertyContext, flags}, parms);
            }


            return PropertyTypeRepresentation.Create(oidStrategy ,req, propertyContext, flags);
        }
    }
}