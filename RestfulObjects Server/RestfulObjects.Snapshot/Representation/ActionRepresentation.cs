// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Strategies;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ActionRepresentation : Representation {
        protected ActionRepresentation(ActionRepresentationStrategy strategy) : base(strategy.GetFlags()) {
            SelfRelType = strategy.GetSelf();
            Id = strategy.GetId();
            Parameters = strategy.GetParameters();
            Links = strategy.GetLinks(true);
            Extensions = strategy.GetExtensions();
            SetHeader(strategy.GetTarget());
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public MapRepresentation Parameters { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        private void SetHeader(INakedObjectSurface target) {
            caching = CacheType.Transactional;
            SetEtag(target);
        }

        public static ActionRepresentation Create(HttpRequestMessage req, ActionContextSurface actionContext, RestControlFlags flags) {
            return new ActionRepresentation(new ActionRepresentationStrategy(req, actionContext, flags));
        }
    }
}