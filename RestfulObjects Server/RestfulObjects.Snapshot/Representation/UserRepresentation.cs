// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Principal;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class UserRepresentation : Representation {
        private UserRepresentation(HttpRequestMessage req, IPrincipal user, RestControlFlags flags) : base(flags) {
            SelfRelType = new UserRelType(RelValues.Self, new UriMtHelper(req));
            SetLinks(new HomePageRelType(RelValues.Up, new UriMtHelper(req)));
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
            UserName = user.Identity.Name;
            Roles = new string[] {};
        }

        private void SetHeader() {
            caching = CacheType.UserInfo;
        }

        private void SetLinks(HomePageRelType homePageRelType) {
            Links = new[] {LinkRepresentation.Create(SelfRelType, Flags), LinkRepresentation.Create(homePageRelType, Flags)};
        }

        private void SetExtensions() {
            Extensions = new MapRepresentation();
        }

        public static UserRepresentation Create(HttpRequestMessage req, IPrincipal user, RestControlFlags flags) {
            return new UserRepresentation(req, user, flags);
        }
    }
}