// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class InvokeRelType : RelType {
        public InvokeRelType(UriMtHelper helper) : base(RelValues.Invoke, helper) {}

        public override string Name {
            get { return base.Name + (HasRelParameter ? helper.GetRelParameters() : ""); }
        }

        public override Uri GetUri() {
            return helper.GetInvokeUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            return UriMtHelper.GetJsonMediaType(helper.GetInvokeMediaType());
        }
    }
}