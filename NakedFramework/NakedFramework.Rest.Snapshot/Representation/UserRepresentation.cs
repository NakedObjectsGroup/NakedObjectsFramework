// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Runtime.Serialization;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using NakedFramework.Facade.Translation;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation {
    [DataContract]
    public class UserRepresentation : Representation {
        private UserRepresentation(IOidStrategy oidStrategy, HttpRequest req, IPrincipal user, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new UserRelType(RelValues.Self, new UriMtHelper(oidStrategy, req));
            SetLinks(new HomePageRelType(RelValues.Up, new UriMtHelper(oidStrategy, req)));
            SetScalars(user);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.UserName)]
        public string UserName { get; set; }

        [DataMember(Name = JsonPropertyNames.Roles)]
        public string[] Roles { get; set; }

        private void SetScalars(IPrincipal user) {
            UserName = user.Identity?.Name ?? "";
            Roles = System.Array.Empty<string>();
        }

        private void SetHeader() => Caching = CacheType.UserInfo;

        private void SetLinks(HomePageRelType homePageRelType) => Links = new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags), LinkRepresentation.Create(OidStrategy, homePageRelType, Flags)};

        private void SetExtensions() => Extensions = new MapRepresentation();

        public static UserRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, IPrincipal user, RestControlFlags flags) => new(oidStrategy, req, user, flags);
    }
}