// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    public class ErrorRepresentation : Representation {
        public ErrorRepresentation(Exception e) : base(RestControlFlags.DefaultFlags()) {
            Exception exception = GetInnermostException(e);
            Message = exception.Message;
            StackTrace = exception.StackTrace.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            Links = new LinkRepresentation[] {};
            Extensions = new MapRepresentation();
        }

        [DataMember(Name = JsonPropertyNames.Message)]
        public string Message { get; set; }

        [DataMember(Name = JsonPropertyNames.StackTrace)]
        public string[] StackTrace { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        public override MediaTypeHeaderValue GetContentType() {
            return UriMtHelper.GetJsonMediaType(RepresentationTypes.Error);
        }

        private static Exception GetInnermostException(Exception e) {
            if (e.InnerException == null) {
                return e;
            }
            return GetInnermostException(e.InnerException);
        }

        public static Representation Create(Exception exception) {
            return new ErrorRepresentation(exception);
        }
    }
}