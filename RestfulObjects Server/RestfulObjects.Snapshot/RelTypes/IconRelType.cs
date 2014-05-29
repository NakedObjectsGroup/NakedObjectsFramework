// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class IconRelType : RelType {
        public IconRelType(UriMtHelper helper) : base(RelValues.Icon, helper) {}

        public override Uri GetUri() {
            return helper.GetIconUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            return helper.GetIconMediaType();
        }
    }
}