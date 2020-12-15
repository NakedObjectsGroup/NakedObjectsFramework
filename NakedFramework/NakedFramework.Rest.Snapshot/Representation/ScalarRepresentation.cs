// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class ScalarRepresentation : Representation {
        protected ScalarRepresentation(IOidStrategy oidStrategy, HttpRequest req, ObjectContextFacade objectContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
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

        private void SetScalars(ObjectContextFacade objectContext) => Value = RestUtils.ObjectToPredefinedType(objectContext.Target.Object);

        private void SetLinks(HttpRequest req, ObjectContextFacade objectContext) => Links = new LinkRepresentation[] { };

        private void SetExtensions() => Extensions = MapRepresentation.Create();

        public static ScalarRepresentation Create(IOidStrategy oidStrategy, ObjectContextFacade objectContext, HttpRequest req, RestControlFlags flags) => new ScalarRepresentation(oidStrategy, req, objectContext, flags);
    }
}