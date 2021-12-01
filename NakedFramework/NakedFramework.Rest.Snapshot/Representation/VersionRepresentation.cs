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
using NakedFramework.Facade.Interface;
using NakedFramework.Rest.Snapshot.Constants;
using NakedFramework.Rest.Snapshot.RelTypes;
using NakedFramework.Rest.Snapshot.Utility;

namespace NakedFramework.Rest.Snapshot.Representation; 

[DataContract]
public class VersionRepresentation : Representation {
    protected VersionRepresentation(IFrameworkFacade frameworkFacade, HttpRequest req, IDictionary<string, string> capabilitiesMap, RestControlFlags flags)
        : base(frameworkFacade.OidStrategy, flags) {
        SelfRelType = new VersionRelType(RelValues.Self, new UriMtHelper(frameworkFacade.OidStrategy, req));
        SetScalars(frameworkFacade);
        SetLinks(new HomePageRelType(RelValues.Up, new UriMtHelper(frameworkFacade.OidStrategy, req)));
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
        var resourceName = $"NakedFramework.Rest.Snapshot.{resource}.txt";
        var version = "";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream != null) {
            using var reader = new StreamReader(stream);
            version = reader.ReadToEnd().Trim();
        }

        return string.IsNullOrWhiteSpace(version) ? $"Failed to read {resource}" : version;
    }

    private void SetScalars(IFrameworkFacade frameworkFacade) {
        var serverTypes = frameworkFacade.ServerTypes;
        var assembly = Assembly.GetExecutingAssembly();
        SpecVersion = GetVersion(assembly, "specversion");
        var versions = GetVersion(assembly, "implversion").Split(",");

        var sv = versions.Where(v => serverTypes.Any(st => v.StartsWith(st)));

        ImplVersion = string.Join(", ", sv);
    }

    private void SetOptionalCapabilities(IDictionary<string, string> capabilitiesMap) {
        var properties = capabilitiesMap.Select(kvp => new OptionalProperty(kvp.Key, kvp.Value)).ToArray();
        OptionalCapabilities = MapRepresentation.Create(properties);
    }

    private void SetExtensions() => Extensions = new MapRepresentation();

    private void SetLinks(HomePageRelType homePageRelType) => Links = new[] {LinkRepresentation.Create(OidStrategy, SelfRelType, Flags), LinkRepresentation.Create(OidStrategy, homePageRelType, Flags)};

    public static VersionRepresentation Create(IFrameworkFacade frameworkFacade, HttpRequest req, IDictionary<string, string> capabilities, RestControlFlags flags) => new(frameworkFacade, req, capabilities, flags);
}