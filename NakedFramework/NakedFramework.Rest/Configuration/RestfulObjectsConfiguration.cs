// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Rest.Configuration {
    public class RestfulObjectsConfiguration : IRestfulObjectsConfiguration {
        public bool DebugWarnings { get; set; }

        /// <summary>
        /// Set to true to limit availability of resources accessed via the HTTP Get method 
        /// i.e. to 'read only' functionality.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Permits cache settings  to be changed.
        /// Default values are: 0, 3600, 86400, for: transactional, user, non-expiring caches.
        /// </summary>
        public (int, int, int) CacheSettings { get; set; }

        /// <summary>
        /// Permits strict enforcement of Accept headers to be switched off - typically during
        /// early stages of development only.
        /// </summary>
        public bool AcceptHeaderStrict { get; set; }

        /// <summary>
        /// Permits the default page size (20) for returned queryables to be overridden.
        /// Specifying 0 means 'unlimited'.
        /// </summary>
        public int DefaultPageSize { get; set; }
        public bool InlineDetailsInActionMemberRepresentations { get; set; }
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; }
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; }
        public bool ProtoPersistentObjects { get; set; }
        public bool DeleteObjects { get; set; }
        public bool ValidateOnly { get; set; }
        public string DomainModel { get; set; }
        public bool BlobsClobs { get; set; }
        public bool InlinedMemberRepresentations { get; set; }
        public bool AllowMutatingActionOnImmutableObject { get; set; }
    }
}