// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.Net.Http.Headers;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    public class MapRepresentation : Representation {
        private MediaTypeHeaderValue mediaType;
        public MapRepresentation() : base(null, RestControlFlags.DefaultFlags()) { }

        public static MapRepresentation Create(params OptionalProperty[] properties) {
            return properties.Any() ? CreateWithOptionals<MapRepresentation>(new object[] { }, properties) : new MapRepresentation();
        }

        public void SetContentType(MediaTypeHeaderValue mt) {
            mediaType = mt;
        }

        public override MediaTypeHeaderValue GetContentType() => mediaType ?? base.GetContentType();
    }
}