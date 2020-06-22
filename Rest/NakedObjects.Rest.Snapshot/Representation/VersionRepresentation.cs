// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Constants;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Snapshot.Representations {
    [DataContract]
    public class VersionRepresentation : Representation {
        private VersionRepresentation(IOidStrategy oidStrategy, HttpRequest req, IDictionary<string, string> capabilitiesMap, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new VersionRelType(RelValues.Self, new UriMtHelper(oidStrategy, req));
            SetScalars();
            SetLinks(new HomePageRelType(RelValues.Up, new UriMtHelper(oidStrategy, req)));
            SetOptionalCapabilities(capabilitiesMap);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.SpecVersion)]
        public string SpecVersion { get; set; }

        [DataMember(Name = JsonPropertyNames.ImplVersion)]
        public string ImplVersion { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        [DataMember(Name = JsonPropertyNames.OptionalCapabilities)]
        public MapRepresentation OptionalCapabilities { get; set; }

        private void SetHeader() => Caching = CacheType.NonExpiring;

        private static string GetVersion(Assembly assembly, string resource) {
            var resourceName = $"NakedObjects.Rest.Snapshot.{resource}.txt";
            var version = "";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null) {
                using var reader = new StreamReader(stream);
                version = reader.ReadToEnd();
            }

            return string.IsNullOrWhiteSpace(version) ? $"Failed to read {resource}" : version;
        }


        private void SetScalars() {
            var assembly = Assembly.GetExecutingAssembly();
            SpecVersion = GetVersion(assembly, "specversion");
            ImplVersion = GetVersion(assembly, "implversion");
        }

        private void SetOptionalCapabilities(IDictionary<string, string> capabilitiesMap) {
            var properties = capabilitiesMap.Select(kvp => new OptionalProperty(kvp.Key, kvp.Value)).ToArray();
            OptionalCapabilities = MapRepresentation.Create(properties);
        }

        private void SetExtensions() => Extensions = new MapRepresentation();

        private void SetLinks(HomePageRelType homePageRelType) => Links = new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags), LinkRepresentation.Create(OidStrategy, homePageRelType, Flags)};

        public static VersionRepresentation Create(IOidStrategy oidStrategy, HttpRequest req, IDictionary<string, string> capabilities, RestControlFlags flags) => new VersionRepresentation(oidStrategy, req, capabilities, flags);
    }
}