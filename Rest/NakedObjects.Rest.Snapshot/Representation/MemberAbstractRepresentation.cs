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
using NakedObjects.Rest.Snapshot.Strategies;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public abstract class MemberAbstractRepresentation : Representation {
        protected MemberAbstractRepresentation(IOidStrategy oidStrategy, MemberRepresentationStrategy strategy)
            : base(oidStrategy, strategy.GetFlags()) {
            SelfRelType = strategy.GetSelf();
            Id = strategy.GetId();
            Links = strategy.GetLinks(false);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader(IObjectFacade target) {
            Caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static MemberAbstractRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, PropertyContextFacade propertyContext, RestControlFlags flags) {
            var consent = propertyContext.Property.IsUsable(propertyContext.Target);
            var optionals = new List<OptionalProperty>();

            if (consent.IsVetoed) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason));
            }

            if (propertyContext.Property.IsCollection) {
                return CollectionRepresentation.Create(oidStrategy, req, propertyContext, optionals, flags);
            }

            return PropertyRepresentation.Create(oidStrategy, req, propertyContext, optionals, flags);
        }
    }
}