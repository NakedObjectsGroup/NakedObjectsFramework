// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Rest.Configuration {
    public interface IRestfulObjectsConfiguration {
        public bool DebugWarnings { get; set; }

        // to make whole application 'read only' 
        public bool IsReadOnly { get; set; }

        // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
        // 0, 3600, 86400 are the defaults 
        // no caching makes debugging easier
        public (int, int, int) CacheSettings { get; set; }

        // make Accept header handling non-strict (RO spec 2.4.4)
        public bool AcceptHeaderStrict { get; set; }

        // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
        public int DefaultPageSize { get; set; }

        // These flags control Member Representations - if true the 'details' will be included 
        // in the the member. This will increase the size of the initial representation but reduce 
        // the number of messages.   
        public bool InlineDetailsInActionMemberRepresentations { get; set; }
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; }
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; }

        // overrides for flags 
        public bool ProtoPersistentObjects { get; set; }
        public bool DeleteObjects { get; set; }
        public bool ValidateOnly { get; set; }
        public string DomainModel { get; set; }
        public string BlobsClobs { get; set; }
        public bool InlinedMemberRepresentations { get; set; }
        bool AllowMutatingActionOnImmutableObject { get; set; }
    }
}