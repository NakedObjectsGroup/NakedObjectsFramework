// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Rest.Configuration {
    public interface IRestfulObjectsConfiguration {
        public bool DebugWarnings { get; set; }


        public bool IsReadOnly { get; set; }

        public (int, int, int) CacheSettings { get; set; }

        public bool AcceptHeaderStrict { get; set; }

        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http messages.
        /// </summary>
        public bool InlineDetailsInActionMemberRepresentations { get; set; }

        /// <summary>
        /// Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http messages.
        /// </summary> 
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; }

        /// <summary>
        /// Defaults to true. Setting to false will decrease size of representations, but typically increase the number of Http messages.
        /// </summary>
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; }

        /// <summary>
        /// Defaults to true for Naked Objects, where 'proto-peristent objects' are referred to as 'transient objects'.
        /// </summary>
        public bool ProtoPersistentObjects { get; set; }


        public bool DeleteObjects { get; set; }
        public bool ValidateOnly { get; set; }
        public string DomainModel { get; set; }
        public bool BlobsClobs { get; set; }
        public bool InlinedMemberRepresentations { get; set; }

        /// <summary>
        /// It is recommended that this flag remain set at the default (false).
        /// It should only be set to true if necesssary for backwards-compatibility
        /// with earlier versions of the framework.
        /// </summary>
        bool AllowMutatingActionOnImmutableObject { get; set; }
    }
}