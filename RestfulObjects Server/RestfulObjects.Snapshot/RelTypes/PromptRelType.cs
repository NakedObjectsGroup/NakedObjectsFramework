// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class PromptRelType : RelType {
        public PromptRelType(UriMtHelper helper) : base(RelValues.Prompt, helper) {}
        public PromptRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override string Name {
            get { return base.Name + (HasRelParameter ? helper.GetRelParameters() : ""); }
        }

        public override Uri GetUri() {
            return helper.GetPromptUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            MediaTypeHeaderValue mediaType = UriMtHelper.GetJsonMediaType(RepresentationTypes.Prompt);
            return mediaType;
        }
    }
}