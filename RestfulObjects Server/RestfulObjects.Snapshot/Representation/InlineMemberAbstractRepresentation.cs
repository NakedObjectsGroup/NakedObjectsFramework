// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public abstract class InlineMemberAbstractRepresentation : Representation {
        protected InlineMemberAbstractRepresentation(RestControlFlags flags) : base(flags) {}

        [DataMember(Name = JsonPropertyNames.MemberType)]
        public string MemberType { get; set; }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        protected void SetHeader(INakedObjectSurface target) {
            SetEtag(target);
        }

        public static InlineMemberAbstractRepresentation Create(HttpRequestMessage req, PropertyContextSurface propertyContext, RestControlFlags flags) {
            IConsentSurface consent = propertyContext.Property.IsUsable(propertyContext.Target);
            var optionals = new List<OptionalProperty>();
            if (consent.IsVetoed) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason));
            }

            if (propertyContext.Property.IsCollection()) {
                return InlineCollectionRepresentation.Create(req, propertyContext, optionals, flags);
            }

            return InlinePropertyRepresentation.Create(req, propertyContext, optionals, flags);
        }
    }
}