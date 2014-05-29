// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class VersionRepresentation : Representation {
        private VersionRepresentation(HttpRequestMessage req, IDictionary<string, string> capabilitiesMap, RestControlFlags flags) : base(flags) {
            SelfRelType = new VersionRelType(RelValues.Self, new UriMtHelper(req));
            SetScalars();
            SetLinks(new HomePageRelType(RelValues.Up, new UriMtHelper(req)));
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

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetScalars() {
            SpecVersion = "1.1";
            ImplVersion = "1.5.0";
        }

        private void SetOptionalCapabilities(IDictionary<string, string> capabilitiesMap) {
            OptionalProperty[] properties = capabilitiesMap.Select(kvp => new OptionalProperty(kvp.Key, kvp.Value)).ToArray();
            OptionalCapabilities = MapRepresentation.Create(properties);
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HomePageRelType homePageRelType) {
            Links = new[] {LinkRepresentation.Create(SelfRelType, Flags), LinkRepresentation.Create(homePageRelType, Flags)};
        }

        public static VersionRepresentation Create(HttpRequestMessage req, IDictionary<string, string> capabilities, RestControlFlags flags) {
            return new VersionRepresentation(req, capabilities, flags);
        }
    }
}