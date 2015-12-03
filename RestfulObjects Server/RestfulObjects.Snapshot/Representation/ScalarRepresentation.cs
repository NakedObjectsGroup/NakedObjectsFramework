// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ScalarRepresentation : Representation {
        protected ScalarRepresentation(HttpRequestMessage req, ObjectContextSurface objectContext, RestControlFlags flags) : base(flags) {
            SetScalars(objectContext);
            SetLinks(req, objectContext);
            SetExtensions();
        }

        [DataMember(Name = JsonPropertyNames.Value)]
        public object Value { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetScalars(ObjectContextSurface objectContext) {
            Value = RestUtils.ObjectToPredefinedType(objectContext.Target.Object);
        }

        private void SetLinks(HttpRequestMessage req, ObjectContextSurface objectContext) {
            var tempLinks = new List<LinkRepresentation>();

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, objectContext.Specification, Flags.OidStrategy)), Flags));
            }
            Links = tempLinks.ToArray();
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create(Flags.OidStrategy);
        }

        public static ScalarRepresentation Create(ObjectContextSurface objectContext, HttpRequestMessage req, RestControlFlags flags) {
            return new ScalarRepresentation(req, objectContext, flags);
        }
    }
}