// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class RefValueRepresentation : Representation {
        protected RefValueRepresentation(IOidStrategy oidStrategy, RelType relType, RestControlFlags flags)
            : base(oidStrategy, flags) {
            Href = relType.GetUri().AbsoluteUri;
        }

        [DataMember(Name = JsonPropertyNames.Href)]
        public string Href { get; set; }

        public static RefValueRepresentation Create(IOidStrategy oidStrategy, RelType relType, RestControlFlags flags) {
            return new RefValueRepresentation(oidStrategy, relType, flags);
        }
    }
}