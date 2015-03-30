// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class ParameterTypeRepresentation : Representation {
        protected ParameterTypeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetScalars(parameterTypeContext);
            SelfRelType = new ParamTypeRelType(RelValues.Self, new UriMtHelper(req, parameterTypeContext));
            SetLinks(req, parameterTypeContext);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.Number)]
        public int Number { get; set; }

        [DataMember(Name = JsonPropertyNames.Name)]
        public string Name { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.Optional)]
        public bool Optional { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetScalars(ParameterTypeContextSurface parameterTypeContext) {
            Id = parameterTypeContext.Parameter.Id;
            Number = parameterTypeContext.Parameter.Number();
            Name = parameterTypeContext.Parameter.Id;
            FriendlyName = parameterTypeContext.Parameter.Name();
            Description = parameterTypeContext.Parameter.Description();
            Optional = !parameterTypeContext.Parameter.IsMandatory();
        }

        private void SetLinks(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext) {
            var domainTypeUri = new UriMtHelper(req, parameterTypeContext);

            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new TypeMemberRelType(RelValues.Up, domainTypeUri), Flags),
                LinkRepresentation.Create(new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(req, parameterTypeContext.Parameter.Specification)), Flags)
            };

            Links = tempLinks.ToArray();
        }


        public static ParameterTypeRepresentation Create(HttpRequestMessage req, ParameterTypeContextSurface parameterTypeContext, RestControlFlags flags) {
            if (!parameterTypeContext.Parameter.Specification.IsParseable()) {
                return new ParameterTypeRepresentation(req, parameterTypeContext, flags);
            }
            var exts = new Dictionary<string, object>();
            AddStringProperties(parameterTypeContext.Parameter.Specification, parameterTypeContext.Parameter.MaxLength(), parameterTypeContext.Parameter.Pattern(), exts);

            OptionalProperty[] parms = exts.Select(e => new OptionalProperty(e.Key, e.Value)).ToArray();

            return CreateWithOptionals<ParameterTypeRepresentation>(new object[] {req, parameterTypeContext, flags}, parms);
        }
    }
}