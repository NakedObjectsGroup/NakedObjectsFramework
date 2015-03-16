// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Core.Util {
    public static class AdapterUtils {
        /// <summary>
        ///     Safe (returns null if INakedObjectAdapter is null) getter
        /// </summary>
        public static object GetDomainObject(this INakedObjectAdapter inObjectAdapter) {
            return inObjectAdapter == null ? null : inObjectAdapter.Object;
        }

        /// <summary>
        /// Return spec as object spec if is otherwise null
        /// </summary>
        /// <param name="nakedObjectAdapter"></param>
        /// <returns></returns>
        public static IObjectSpec GetObjectSpec(this INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.Spec as IObjectSpec;
        }

        /// <summary>
        /// Return spec as service spec if is otherwise null
        /// </summary>
        /// <param name="nakedObjectAdapter"></param>
        /// <returns></returns>
        public static IServiceSpec GetServiceSpec(this INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.Spec as IServiceSpec;
        }

        /// <summary>
        ///     Safe (returns null if INakedObjectAdapter is null) generic getter
        /// </summary>
        public static T GetDomainObject<T>(this INakedObjectAdapter inObjectAdapter) {
            return inObjectAdapter == null ? default(T) : (T) inObjectAdapter.Object;
        }

        public static bool Exists(this INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter != null && nakedObjectAdapter.Object != null;
        }

        public static ICollectionFacet GetCollectionFacetFromSpec(this INakedObjectAdapter objectAdapterRepresentingCollection) {
            ITypeSpec collectionSpec = objectAdapterRepresentingCollection.Spec;
            return collectionSpec.GetFacet<ICollectionFacet>();
        }

        public static IEnumerable<INakedObjectAdapter> GetAsEnumerable(this INakedObjectAdapter objectAdapterRepresentingCollection, INakedObjectManager manager) {
            return objectAdapterRepresentingCollection.GetCollectionFacetFromSpec().AsEnumerable(objectAdapterRepresentingCollection, manager);
        }

        public static IQueryable GetAsQueryable(this INakedObjectAdapter objectAdapterRepresentingCollection) {
            return objectAdapterRepresentingCollection.GetCollectionFacetFromSpec().AsQueryable(objectAdapterRepresentingCollection);
        }

        public static ITypeOfFacet GetTypeOfFacetFromSpec(this INakedObjectAdapter objectAdapterRepresentingCollection) {
            ITypeSpec collectionSpec = objectAdapterRepresentingCollection.Spec;
            return collectionSpec.GetFacet<ITypeOfFacet>();
        }

        public static IActionSpec[] GetActionLeafNodes(this IActionSpec actionSpec) {
            return new[] {actionSpec};
        }

        public static IActionSpec[] GetActionLeafNodes(this INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.Spec.GetActionLeafNodes();
        }

        public static IActionSpec[] GetActionLeafNodes(this ITypeSpec spec) {
            return spec.GetActions().SelectMany(GetActionLeafNodes).ToArray();
        }

        public static IActionSpec GetActionLeafNode(this INakedObjectAdapter nakedObjectAdapter, string actionName) {
            return nakedObjectAdapter.GetActionLeafNodes().Single(x => x.Id == actionName);
        }

        public static IAssociationSpec GetVersionProperty(this INakedObjectAdapter nakedObjectAdapter) {
            var spec = nakedObjectAdapter.Spec as IObjectSpec;
            if (spec == null) {
                return null; // only expect in testing 
            }
            return spec.Properties.SingleOrDefault(x => x.ContainsFacet<IConcurrencyCheckFacet>());
        }

        private static DateTime StripMillis(this DateTime fullDateTime) {
            return new DateTime(fullDateTime.Year,
                fullDateTime.Month,
                fullDateTime.Day,
                fullDateTime.Hour,
                fullDateTime.Minute,
                fullDateTime.Second);
        }

        public static object GetVersion(this INakedObjectAdapter nakedObjectAdapter, INakedObjectManager manager) {
            IAssociationSpec versionProperty = nakedObjectAdapter.GetVersionProperty();

            if (versionProperty != null) {
                object version = versionProperty.GetNakedObject(nakedObjectAdapter).GetDomainObject();

                if (version is DateTime) {
                    return ((DateTime) version).StripMillis();
                }
                return version;
            }

            return null;
        }
    }
}