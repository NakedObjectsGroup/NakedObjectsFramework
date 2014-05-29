// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class TypeActionInvokeRepresentation : Representation {
        protected TypeActionInvokeRepresentation(HttpRequestMessage req, TypeActionInvokeContext context, RestControlFlags flags) : base(flags) {
            SelfRelType = new TypeActionInvokeRelType(RelValues.Self, new UriMtHelper(req, context));
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

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetLinks(HttpRequestMessage req, TypeActionInvokeContext context) {
            string uri = new DomainTypeRelType(new UriMtHelper(req, context.OtherSpecification)).GetUri().AbsoluteUri;

            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType,
                                          Flags,
                                          new OptionalProperty(JsonPropertyNames.Arguments,
                                                               MapRepresentation.Create(new OptionalProperty(context.ParameterId,
                                                                                                             MapRepresentation.Create(new OptionalProperty(JsonPropertyNames.Href,
                                                                                                                                                           uri))))))
            };

            Links = tempLinks.ToArray();
        }


        public static TypeActionInvokeRepresentation Create(HttpRequestMessage req, TypeActionInvokeContext context, RestControlFlags flags) {
            return new TypeActionInvokeRepresentation(req, context, flags);
        }
    }
}