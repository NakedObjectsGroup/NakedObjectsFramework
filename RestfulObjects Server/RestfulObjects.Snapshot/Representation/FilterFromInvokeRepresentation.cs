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
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class FilterFromInvokeRepresentation : Representation {
        protected FilterFromInvokeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, FilterFromInvokeContext context, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new TypeActionFilterInvokeRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, context));
            SetScalar(context);
            SetValue(req, context);
            SetLinks(req, context);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public LinkRepresentation[] Value { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private LinkRepresentation CreateDomainLink(IOidStrategy oidStrategy, HttpRequestMessage req, ITypeFacade spec) {
            return LinkRepresentation.Create(oidStrategy, new DomainTypeRelType(new UriMtHelper(oidStrategy, req, spec)), Flags);
        }

        private void SetScalar(FilterFromInvokeContext context) {
            Id = context.Id;
        }

        private void SetValue(HttpRequestMessage req, FilterFromInvokeContext context) {
            Id = context.Id;
            Value = context.Value.Select(v => CreateDomainLink(OidStrategy, req, v)).ToArray();
        }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetLinks(HttpRequestMessage req, FilterFromInvokeContext context) {
            var uris = context.OtherSpecifications.Select(os => new DomainTypeRelType(new UriMtHelper(OidStrategy, req, os)).GetUri().AbsoluteUri).ToArray();

            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, SelfRelType,
                    Flags,
                    new OptionalProperty(JsonPropertyNames.Arguments,
                        MapRepresentation.Create(new OptionalProperty(context.ParameterId, uris.Select(uri =>
                            MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Href,
                                uri))).ToArray()))))
            };

            Links = tempLinks.ToArray();
        }

        public static FilterFromInvokeRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, FilterFromInvokeContext context, RestControlFlags flags) {
            return new FilterFromInvokeRepresentation(oidStrategy, req, context, flags);
        }
    }
}