// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Net.Http.Headers;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    public class ErrorRepresentation : Representation {
        public ErrorRepresentation(IOidStrategy oidStrategy, Exception e)
            : base(oidStrategy, RestControlFlags.DefaultFlags()) {
            var exception = GetInnermostException(e);
            Message = exception.Message;
            StackTrace = exception.StackTrace?.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray() ?? new string[] { };

            Links = new LinkRepresentation[] { };
            Extensions = new MapRepresentation();
        }

        [DataMember(Name = JsonPropertyNames.Message)]
        public string Message { get; set; }

        [DataMember(Name = JsonPropertyNames.StackTrace)]
        public string[] StackTrace { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        public override MediaTypeHeaderValue GetContentType() => UriMtHelper.GetJsonMediaType(RepresentationTypes.Error);

        private static Exception GetInnermostException(Exception e) => e.InnerException == null ? e : GetInnermostException(e.InnerException);

        public static Representation Create(IOidStrategy oidStrategy, Exception exception) => new ErrorRepresentation(oidStrategy, exception);
    }
}