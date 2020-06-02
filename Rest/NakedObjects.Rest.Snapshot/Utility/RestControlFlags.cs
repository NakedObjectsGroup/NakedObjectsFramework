// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Rest.Snapshot.Constants;

namespace NakedObjects.Rest.Snapshot.Utility {
    public class RestControlFlags {
        #region DomainModelType enum

        public enum DomainModelType {
            Selectable, // default
            None,
            Simple,
            Formal
        }

        #endregion

        public const string ReservedPrefix = "x-ro-";
        public const string ValidateOnlyReserved = ReservedPrefix + "validate-only";
        public const string DomainTypeReserved = ReservedPrefix + "domain-type";
        public const string ElementTypeReserved = ReservedPrefix + "element-type";
        public const string DomainModelReserved = ReservedPrefix + "domain-model";
        public const string FollowLinksReserved = ReservedPrefix + "follow-links";
        public const string SortByReserved = ReservedPrefix + "sort-by";

        public const string SearchTermReserved = ReservedPrefix + "searchTerm";

        // custom extensions 
        public const string PageReserved = ReservedPrefix + JsonPropertyNames.Page;
        public const string PageSizeReserved = ReservedPrefix + JsonPropertyNames.PageSize;
        public const string InlinePropertyDetailsReserved = ReservedPrefix + "inline-property-details";
        public const string InlineCollectionItemsReserved = ReservedPrefix + "inline-collection-items";

        protected RestControlFlags() { }
        public static int ConfiguredPageSize { get; set; }

        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public bool ValidateOnly { get; private set; }
        public bool FollowLinks { get; private set; }
        public bool SortBy { get; private set; }
        public bool BlobsClobs { get; private set; }
        public bool InlineDetailsInActionMemberRepresentations { get; set; }
        public bool InlineDetailsInCollectionMemberRepresentations { get; set; }
        public bool InlineDetailsInPropertyMemberRepresentations { get; set; }
        public bool InlineCollectionItems { get; set; }
        public bool AllowMutatingActionsOnImmutableObject { get; set; }

        private static bool GetBool(object value) =>
            value switch {
                null => false,
                string s => bool.Parse(s),
                bool b => b,
                _ => false
            };

        private static int GetInt(object value) =>
            value switch {
                null => 0,
                string s => int.Parse(s),
                int i => i,
                _ => 0
            };

        private static int DefaultPageSize(int pageSize) => pageSize == 0 ? ConfiguredPageSize : pageSize;

        private static int DefaultPage(int page) => page == 0 ? 1 : page;

        private static int GetPageSize(object value) => DefaultPageSize(GetInt(value));

        private static int GetPage(object value) => DefaultPage(GetInt(value));

        private static RestControlFlags GetFlags(Func<string, object> getValue) {
            var controlFlags = new RestControlFlags {
                ValidateOnly = GetBool(getValue(ValidateOnlyReserved)),
                FollowLinks = GetBool(getValue(FollowLinksReserved)),
                SortBy = GetBool(getValue(SortByReserved)),
                BlobsClobs = false,
                Page = GetPage(getValue(PageReserved)),
                PageSize = GetPageSize(getValue(PageSizeReserved))
            };

            return controlFlags;
        }

        public static RestControlFlags DefaultFlags() => GetFlags(s => null);

        public static RestControlFlags FlagsFromArguments(bool validateOnly,
                                                          int page,
                                                          int pageSize,
                                                          string domainModel,
                                                          bool inlineDetailsInActionMemberRepresentations,
                                                          bool inlineDetailsInCollectionMemberRepresentations,
                                                          bool inlineDetailsInPropertyMemberRepresentations,
                                                          bool inlineCollectionItems,
                                                          bool allowMutatingActionsOnImmutableObjects) =>
            new RestControlFlags {
                ValidateOnly = validateOnly,
                FollowLinks = false,
                SortBy = false,
                BlobsClobs = false,
                PageSize = DefaultPageSize(pageSize),
                Page = DefaultPage(page),
                InlineDetailsInActionMemberRepresentations = inlineDetailsInActionMemberRepresentations,
                InlineDetailsInCollectionMemberRepresentations = inlineDetailsInCollectionMemberRepresentations,
                InlineDetailsInPropertyMemberRepresentations = inlineDetailsInPropertyMemberRepresentations,
                InlineCollectionItems = inlineCollectionItems,
                AllowMutatingActionsOnImmutableObject = allowMutatingActionsOnImmutableObjects
            };
    }
}