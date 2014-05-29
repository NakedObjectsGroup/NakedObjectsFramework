// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using Common.Logging;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class HomePageRepresentation : Representation {
        private static readonly ILog Log = LogManager.GetLogger(typeof (HomePageRepresentation));

        protected HomePageRepresentation(HttpRequestMessage req, RestControlFlags flags) : base(flags) {
            Log.DebugFormat("HomePageRepresentation");
            SelfRelType = new HomePageRelType(RelValues.Self, new UriMtHelper(req));
            SetLinks(req);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        private void SetLinks(HttpRequestMessage req) {
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(SelfRelType, Flags),
                LinkRepresentation.Create(new UserRelType(new UriMtHelper(req)), Flags),
                LinkRepresentation.Create(new ListRelType(RelValues.Services, SegmentValues.Services, new UriMtHelper(req)), Flags),
                LinkRepresentation.Create(new VersionRelType(new UriMtHelper(req)), Flags)
            };

            if (Flags.FormalDomainModel) {
                tempLinks.Add(LinkRepresentation.Create(new TypesRelType(new UriMtHelper(req)), Flags));
            }

            Links = tempLinks.ToArray();
        }

        public static HomePageRepresentation Create(HttpRequestMessage req, RestControlFlags flags) {
            return new HomePageRepresentation(req, flags);
        }
    }
}