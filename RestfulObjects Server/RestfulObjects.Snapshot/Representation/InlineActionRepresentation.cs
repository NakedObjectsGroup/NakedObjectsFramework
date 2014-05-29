// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
        protected InlineActionRepresentation(ActionRepresentationStrategy strategy) : base(strategy.GetFlags()) {
            MemberType = MemberTypes.Action;
            Id = strategy.GetId();
            Parameters = strategy.GetParameters();
            Links = strategy.GetLinks(false);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public MapRepresentation Parameters { get; set; }

        public static InlineActionRepresentation Create(HttpRequestMessage req, ActionContextSurface actionContext, RestControlFlags flags) {
            IConsentSurface consent = actionContext.Action.IsUsable(actionContext.Target);

            var actionRepresentationStrategy = new ActionRepresentationStrategy(req, actionContext, flags);
            if (consent.IsVetoed) {
                var optionals = new List<OptionalProperty> {new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason)};
                return CreateWithOptionals<InlineActionRepresentation>(new object[] {actionRepresentationStrategy}, optionals);
            }

            return new InlineActionRepresentation(actionRepresentationStrategy);
        }
    }
}