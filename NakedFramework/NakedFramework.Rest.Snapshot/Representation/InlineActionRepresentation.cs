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
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Strategies;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class InlineActionRepresentation : InlineMemberAbstractRepresentation {
        protected InlineActionRepresentation(IOidStrategy oidStrategy, AbstractActionRepresentationStrategy strategy)
            : base(oidStrategy, strategy.GetFlags()) {
            MemberType = MemberTypes.Action;
            Id = strategy.GetId();
            Links = strategy.GetLinks();
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        public static InlineActionRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, ActionContextFacade actionContext, RestControlFlags flags) {
            var consent = actionContext.Action.IsUsable(actionContext.Target);

            var strategy = AbstractActionRepresentationStrategy.GetStrategy(true, oidStrategy, req, actionContext, flags);
            var optionals = new List<OptionalProperty>();

            if (consent.IsVetoed) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason));
            }

            if (strategy.ShowParameters()) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.Parameters, strategy.GetParameters()));
            }

            return optionals.Any()
                ? CreateWithOptionals<InlineActionRepresentation>(new object[] {oidStrategy, strategy}, optionals)
                : new InlineActionRepresentation(oidStrategy, strategy);
        }
    }
}