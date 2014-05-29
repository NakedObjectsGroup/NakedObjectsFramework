// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class VersionRelType : RelType {
        public VersionRelType(UriMtHelper helper) : base(RelValues.Version, helper) {}
        public VersionRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override Uri GetUri() {
            return helper.GetWellKnownUri(SegmentValues.Version);
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            return UriMtHelper.GetJsonMediaType(RepresentationTypes.Version);
        }
    }
}