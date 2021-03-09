// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class TypeActionInvokeRepresentation : Representation {
        protected TypeActionInvokeRepresentation(IOidStrategy oidStrategy, HttpRequest req, TypeActionInvokeContext context, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new TypeActionInvokeRelType(RelValues.Self, new UriMtHelper(oidStrategy, req, context));
            SetScalars(context);
            SetLinks(req, context);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Value)]
        public bool Value { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetScalars(TypeActionInvokeContext context) {
            Id = context.Id;
            Value = context.Value;
        }

        private void SetHeader() => Caching = CacheType.NonExpiring;

        private void SetExtensions() => Extensions = MapRepresentation.Create();

        private void SetLinks(HttpRequest req, TypeActionInvokeContext context) {
            var uri = new DomainTypeRelType(new UriMtHelper(OidStrategy, req, context.OtherSpecification)).GetUri().AbsoluteUri;

            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, SelfRelType,
                                          Flags,
                                          new OptionalProperty(JsonPropertyNames.Arguments,
                                                               MapRepresentation.Create(new OptionalProperty(context.ParameterId,
                                                                                                             MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Href,
                                                                                                                                                           uri))))))
            };

            Links = tempLinks.ToArray();
        }

        public static TypeActionInvokeRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, TypeActionInvokeContext context, RestControlFlags flags) => new(oidStrategy, req, context, flags);
    }
}