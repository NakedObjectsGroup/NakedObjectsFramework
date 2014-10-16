// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Properties.Version;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Adapter {
    public static class AdapterUtils {
        /// <summary>
        ///     Safe (returns null if INakedObject is null) getter
        /// </summary>
        public static object GetDomainObject(this INakedObject inObject) {
            return inObject == null ? null : inObject.Object;
        }

        /// <summary>
        ///     Safe (returns null if INakedObject is null) generic getter
        /// </summary>
        public static T GetDomainObject<T>(this INakedObject inObject) {
            return inObject == null ? default(T) : (T) inObject.Object;
        }

        public static bool Exists(this INakedObject nakedObject) {
            return nakedObject != null && nakedObject.Object != null;
        }

        public static ICollectionFacet GetCollectionFacetFromSpec(this INakedObject objectRepresentingCollection) {
            IObjectSpec collectionSpec = objectRepresentingCollection.Spec;
            return collectionSpec.GetFacet<ICollectionFacet>();
        }

        public static IEnumerable<INakedObject> GetAsEnumerable(this INakedObject objectRepresentingCollection, INakedObjectManager manager) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().AsEnumerable(objectRepresentingCollection, manager);
        }

        public static IQueryable GetAsQueryable(this INakedObject objectRepresentingCollection) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().AsQueryable(objectRepresentingCollection);
        }


        public static ITypeOfFacet GetTypeOfFacetFromSpec(this INakedObject objectRepresentingCollection) {
            IObjectSpec collectionSpec = objectRepresentingCollection.Spec;
            return collectionSpec.GetFacet<ITypeOfFacet>();
        }

        public static INakedObjectAction[] GetActionLeafNodes(this INakedObjectAction action) {
            return action.ActionType == NakedObjectActionType.Set ? action.Actions.SelectMany(GetActionLeafNodes).ToArray() : new[] {action};
        }

        public static INakedObjectAction[] GetActionLeafNodes(this INakedObject nakedObject) {
            return nakedObject.Spec.GetActionLeafNodes();
        }

        public static INakedObjectAction[] GetActionLeafNodes(this IObjectSpec spec) {
            return spec.GetAllActions().SelectMany(GetActionLeafNodes).ToArray();
        }

        public static INakedObjectAction GetActionLeafNode(this INakedObject nakedObject, string actionName) {
            return nakedObject.GetActionLeafNodes().Single(x => x.Id == actionName);
        }


        public static INakedObjectAssociation GetVersionProperty(this INakedObject nakedObject) {
            if (nakedObject.Spec == null) {
                return null; // only expect in testing 
            }
            return nakedObject.Spec.Properties.SingleOrDefault(x => x.ContainsFacet<IConcurrencyCheckFacet>());
        }

        private static DateTime StripMillis(this DateTime fullDateTime) {
            return new DateTime(fullDateTime.Year,
                fullDateTime.Month,
                fullDateTime.Day,
                fullDateTime.Hour,
                fullDateTime.Minute,
                fullDateTime.Second);
        }

        public static object GetVersion(this INakedObject nakedObject, INakedObjectManager manager) {
            INakedObjectAssociation versionProperty = nakedObject.GetVersionProperty();

            if (versionProperty != null) {
                object version = versionProperty.GetNakedObject(nakedObject).GetDomainObject();

                if (version is DateTime) {
                    return ((DateTime) version).StripMillis();
                }
                if (version != null) {
                    return version;
                }
            }

            return null;
        }
    }
}