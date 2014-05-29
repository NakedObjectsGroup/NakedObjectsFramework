// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class ListRelType : RelType {
        private readonly string endPoint;

        public ListRelType(string name, string endPoint, UriMtHelper helper)
            : base(name, helper) {
            this.endPoint = endPoint;
        }

        public override Uri GetUri() {
            return helper.GetWellKnownUri(endPoint);
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            MediaTypeHeaderValue mediaType = UriMtHelper.GetJsonMediaType(RepresentationTypes.List);
            helper.AddListRepresentationParameter(mediaType, flags);
            return mediaType;
        }
    }
}