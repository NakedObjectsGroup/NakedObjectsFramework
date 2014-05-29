// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;

namespace RestfulObjects.Snapshot.Utility {
    public class ObjectRelType : RelType {
        //public ObjectRelType(UriMtHelper helper) : base(RelValues.Object, helper) {}
        public ObjectRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override Uri GetUri() {
            return helper.GetObjectUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            MediaTypeHeaderValue mediaType = UriMtHelper.GetJsonMediaType(helper.GetObjectMediaType());
            helper.AddObjectRepresentationParameter(mediaType, flags);
            return mediaType;
        }
    }
}