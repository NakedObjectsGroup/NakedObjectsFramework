// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class TypeActionRelType : RelType {
        private readonly string action;

        public TypeActionRelType(UriMtHelper helper, string action) : this(RelValues.Invoke, helper, action) {}

        public TypeActionRelType(string name, UriMtHelper helper, string action)
            : base(name, helper) {
            this.action = action;
        }

        public override string Name {
            get { return base.Name + helper.GetRelParametersFor(action); }
        }

        public override Uri GetUri() {
            return helper.GetTypeActionsUri(action);
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            return UriMtHelper.GetJsonMediaType(helper.GetTypeActionMediaType());
        }
    }
}