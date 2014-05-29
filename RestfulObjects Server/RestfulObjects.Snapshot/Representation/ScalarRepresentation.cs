// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ScalarRepresentation : Representation {
        protected ScalarRepresentation(HttpRequestMessage req, ObjectContextSurface objectContext, RestControlFlags flags) : base(flags) {
            SetScalars(objectContext);
            SetLinks(req, objectContext);
            SetExtensions();
        }

        [DataMember(Name = JsonPropertyNames.Value)]
        public object Value { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetScalars(ObjectContextSurface objectContext) {
            Value = RestUtils.ObjectToPredefinedType(objectContext.Target.Object);
        }

        private void SetLinks(HttpRequestMessage req, ObjectContextSurface objectContext) {
            var tempLinks = new List<LinkRepresentation>();

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, objectContext.Specification)), Flags));
            }
            Links = tempLinks.ToArray();
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        public static ScalarRepresentation Create(ObjectContextSurface objectContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ScalarRepresentation(req, objectContext, flags);
        }
    }
}