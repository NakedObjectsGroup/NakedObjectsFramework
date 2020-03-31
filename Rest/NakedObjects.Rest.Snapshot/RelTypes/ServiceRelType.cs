// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Net.Http.Headers;
using NakedObjects.Rest.Snapshot.Constants;

namespace NakedObjects.Rest.Snapshot.Utility {
    public class ServiceRelType : ObjectRelType {
        public ServiceRelType(UriMtHelper helper) : base(RelValues.Service, helper) { }
        public ServiceRelType(string name, UriMtHelper helper) : base(name, helper) { }

        public override string Name => $"{base.Name}{(HasRelParameter ? Helper.GetServiceRelParameter() : "")}";

        public override Uri GetUri() => Helper.GetServiceUri();

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            var mediaType = UriMtHelper.GetJsonMediaType(Helper.GetObjectMediaType());
            Helper.AddObjectRepresentationParameter(mediaType, flags);
            return mediaType;
        }
    }
}