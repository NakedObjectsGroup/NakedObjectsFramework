// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;

namespace RestfulObjects.Snapshot.Utility {
    public class TypeMemberRelType : RelType {
        public TypeMemberRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override Uri GetUri() {
            return helper.GetTypeDetailsUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            return UriMtHelper.GetJsonMediaType(helper.GetTypeMemberMediaType());
        }
    }
}