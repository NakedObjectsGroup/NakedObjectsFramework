// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Properties.Version;
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
            INakedObjectSpecification collectionSpec = objectRepresentingCollection.Specification;
            return collectionSpec.GetFacet<ICollectionFacet>();
        }

        public static IEnumerable<INakedObject> GetAsEnumerable(this INakedObject objectRepresentingCollection) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().AsEnumerable(objectRepresentingCollection);
        }

        public static IQueryable GetAsQueryable(this INakedObject objectRepresentingCollection) {
            return objectRepresentingCollection.GetCollectionFacetFromSpec().AsQueryable(objectRepresentingCollection);
        }


        public static ITypeOfFacet GetTypeOfFacetFromSpec(this INakedObject objectRepresentingCollection) {
            INakedObjectSpecification collectionSpec = objectRepresentingCollection.Specification;
            return collectionSpec.GetFacet<ITypeOfFacet>();
        }

        public static INakedObjectAction[] GetActionLeafNodes(this INakedObjectAction action) {
            return action.ActionType == NakedObjectActionType.Set ? action.Actions.SelectMany(GetActionLeafNodes).ToArray() : new[] {action};
        }

        public static INakedObjectAction[] GetActionLeafNodes(this INakedObject nakedObject) {
            return nakedObject.Specification.GetActionLeafNodes();
        }

        public static INakedObjectAction[] GetActionLeafNodes(this INakedObjectSpecification spec) {
            return spec.GetObjectActions().SelectMany(GetActionLeafNodes).ToArray();
        }

        public static INakedObjectAction GetActionLeafNode(this INakedObject nakedObject, string actionName) {
            return nakedObject.GetActionLeafNodes().Single(x => x.Id == actionName);
        }


        public static INakedObjectAssociation GetVersionProperty(this INakedObject nakedObject) {
            if (nakedObject.Specification == null) {
                return null; // only expect in testing 
            }
            return nakedObject.Specification.Properties.SingleOrDefault(x => x.ContainsFacet<IConcurrencyCheckFacet>());
        }

        private static DateTime StripMillis(this DateTime fullDateTime) {
            return new DateTime(fullDateTime.Year,
                                fullDateTime.Month,
                                fullDateTime.Day,
                                fullDateTime.Hour,
                                fullDateTime.Minute,
                                fullDateTime.Second);
        }

        public static object GetVersion(this INakedObject nakedObject) {
            INakedObjectAssociation versionProperty = nakedObject.GetVersionProperty();

            if (versionProperty != null) {
                object version = versionProperty.GetNakedObject(nakedObject).GetDomainObject();

                if (version is DateTime) {
                    return ((DateTime)version).StripMillis();
                }
                if (version != null) {
                    return version;
                }
            }

            return null;
        }

    }
}