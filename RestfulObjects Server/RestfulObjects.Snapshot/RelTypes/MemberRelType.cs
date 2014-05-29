// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class MemberRelType : RelType {
        public MemberRelType(UriMtHelper helper) : base(RelValues.Details, helper) {}
        public MemberRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override string Name {
            get { return base.Name + (HasRelParameter ? helper.GetRelParameters() : ""); }
        }

        public override Uri GetUri() {
            return helper.GetDetailsUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            MediaTypeHeaderValue mediaType = UriMtHelper.GetJsonMediaType(helper.GetMemberMediaType());
            helper.AddObjectCollectionRepresentationParameter(mediaType, flags);
            return mediaType;
        }
    }
}