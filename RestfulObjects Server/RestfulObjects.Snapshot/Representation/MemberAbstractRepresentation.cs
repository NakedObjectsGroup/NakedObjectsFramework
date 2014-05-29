// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public abstract class MemberAbstractRepresentation : Representation {
        protected MemberAbstractRepresentation(MemberRepresentationStrategy strategy) : base(strategy.GetFlags()) {
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

        private void SetHeader(INakedObjectSurface target) {
            caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static MemberAbstractRepresentation Create(HttpRequestMessage req, PropertyContextSurface propertyContext, RestControlFlags flags) {
            IConsentSurface consent = propertyContext.Property.IsUsable(propertyContext.Target);
            var optionals = new List<OptionalProperty>();

            if (consent.IsVetoed) {
                optionals.Add(new OptionalProperty(JsonPropertyNames.DisabledReason, consent.Reason));
            }

            if (propertyContext.Property.IsCollection()) {
                return CollectionRepresentation.Create(req, propertyContext, optionals, flags);
            }

            return PropertyRepresentation.Create(req, propertyContext, optionals, flags);
        }
    }
}