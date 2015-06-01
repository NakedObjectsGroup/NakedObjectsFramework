// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class InlineActionRepresentation : InlineMemberAbstractRepresentation {
        protected InlineActionRepresentation(IOidStrategy oidStrategy, ActionRepresentationStrategy strategy)
            : base(oidStrategy, strategy.GetFlags()) {
            MemberType = MemberTypes.Action;
            Id = strategy.GetId();
            Parameters = strategy.GetParameters();
            Links = strategy.GetLinks(false);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public MapRepresentation Parameters { get; set; }

        public static InlineActionRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, ActionContextSurface actionContext, RestControlFlags flags) {
            IConsentFacade consent = actionContext.Action.IsUsable(actionContext.Target);

            var actionRepresentationStrategy = new ActionRepresentationStrategy(oidStrategy ,req, actionContext, flags);
            if (consent.IsVetoed) {
                var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason)};
                return CreateWithOptionals<InlineActionRepresentation>(new object[] {oidStrategy, actionRepresentationStrategy}, optionals);
            }

            return new InlineActionRepresentation(oidStrategy, actionRepresentationStrategy);
        }
    }
}