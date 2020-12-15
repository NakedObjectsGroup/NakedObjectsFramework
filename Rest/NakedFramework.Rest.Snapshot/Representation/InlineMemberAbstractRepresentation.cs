// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public abstract class InlineMemberAbstractRepresentation : Representation {
        protected InlineMemberAbstractRepresentation(IOidStrategy oidStrategy, RestControlFlags flags) : base(oidStrategy, flags) { }

        [DataMember(Name = JsonPropertyNames.MemberType)]
        public string MemberType { get; set; }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        protected void SetHeader(IObjectFacade target) => SetEtag(target);

        public static InlineMemberAbstractRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags, bool asTableColumn) {
            var consent = propertyContext.Property.IsUsable(propertyContext.Target);
            var optionals = new List<OptionalProperty>();
            if (consent.IsVetoed) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason));
            }

            if (propertyContext.Property.IsCollection) {
                return InlineCollectionRepresentation.Create(oidStrategy, req, propertyContext, optionals, flags, asTableColumn);
            }

            return InlinePropertyRepresentation.Create(oidStrategy, req, propertyContext, optionals, flags);
        }
    }
}