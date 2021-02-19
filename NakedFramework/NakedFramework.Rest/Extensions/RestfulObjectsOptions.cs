// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Rest.Extensions {
    public class RestfulObjectsOptions {
        public bool DebugWarnings { get; set; } = true;

        // to make whole application 'read only' 
        public bool IsReadOnly { get; set; } = false;

        // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
        // 0, 3600, 86400 are the defaults 
        // no caching makes debugging easier
        public (int, int, int) CacheSettings { get; set; } = (0, 3600, 86400);

        // make Accept header handling non-strict (RO spec 2.4.4)
        public bool AcceptHeaderStrict { get; set; } = true;

        // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
        public int DefaultPageSize { get; set; } = 20;

        // These flags control Member Representations - if true the 'details' will be included 
        // in the the member. This will increase the size of the initial representation but reduce 
        // the number of messages.   
        public bool InlineDetailsInActionMemberRepresentations { get; set; } = true;
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; } = true;
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; } = true;

        public bool AllowMutatingActionOnImmutableObject { get; set; } = false;

        // overrides for flags 
        public bool ProtoPersistentObjects { get; set; } = true;
        public bool DeleteObjects { get; set; }  = false;
        public bool ValidateOnly { get; set; } = true;
        public string DomainModel { get; set; } = "simple";
        public string BlobsClobs { get; set; } = "attachments";
        public bool InlinedMemberRepresentations { get; set; } = true;
    }
}