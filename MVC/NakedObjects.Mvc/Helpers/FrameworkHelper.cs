// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NakedObjects.Surface;
using NakedObjects.Surface.Utility;
using NakedObjects.Surface.Utility.Restricted;
using NakedObjects.Util;

namespace NakedObjects.Web.Mvc.Html {
    internal static class FrameworkHelper {
        public static IEnumerable<INakedObjectActionSurface> GetTopLevelActions(this INakedObjectsSurface surface, INakedObjectSurface nakedObject) {
            if (nakedObject.Specification.IsQueryable()) {
                var elementSpec = nakedObject.ElementSpecification;
                Trace.Assert(elementSpec != null);
                return elementSpec.GetCollectionContributedActions();
            }
            return nakedObject.Specification.GetActionLeafNodes().Where(a => a.IsVisible(nakedObject));
        }

        public static string GetObjectType(Type type) {
            return type.GetProxiedTypeFullName().Replace('.', '-');
        }

        public static string GetObjectType(object model) {
            return model == null ? string.Empty : GetObjectType(model.GetType());
        }

        //private static string Encode(this IEncodedToStrings encoder) {
        //    return encoder.ToShortEncodedStrings().Aggregate((a, b) => a + ";" + b);
        //}

        //public static string GetObjectId(IOid oid) {
        //    return ((IEncodedToStrings) oid).Encode();
        //}

        public static string GetObjectTypeName(this INakedObjectsSurface surface, object model) {
            var nakedObject = surface.GetObject(model);
            return nakedObject.Specification.FullName().Split('.').Last();
        }


        public static string IconName(INakedObjectSurface nakedObject) {
            string name = nakedObject.Specification.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        private static INakedObjectSurface GetNakedObjectFromId(INakedObjectsSurface surface, string id) {
            var oid = surface.OidStrategy.GetOid(id, "");
            return surface.GetObject(oid).Target;
        }

        public static object GetTypedCollection(this INakedObjectsSurface surface, INakedObjectActionParameterSurface featureSpec, IEnumerable collectionValue) {
            var collectionitemSpec = featureSpec.ElementType;
            return GetTypedCollection(surface, collectionValue, collectionitemSpec);
        }

        public static object GetTypedCollection(this INakedObjectsSurface surface, INakedObjectAssociationSurface featureSpec, IEnumerable collectionValue) {
            var collectionitemSpec = featureSpec.ElementSpecification;
            return GetTypedCollection(surface, collectionValue, collectionitemSpec);
        }

        private static object GetTypedCollection(INakedObjectsSurface surface, IEnumerable collectionValue, INakedObjectSpecificationSurface collectionitemSpec) {
            string[] rawCollection = collectionValue.Cast<string>().ToArray();

            Type instanceType = collectionitemSpec.GetUnderlyingType();
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));

            if (collectionitemSpec.IsParseable()) {
                return rawCollection.Select(s => string.IsNullOrEmpty(s) ? null : s).ToArray();
            }

            // need to check if collection is actually a collection memento 
            if (rawCollection.Count() == 1) {
                var firstObj = GetNakedObjectFromId(surface, rawCollection.First());

                if (firstObj != null && firstObj.IsCollectionMemento()) {
                    return firstObj.Object;
                }
            }

            var objCollection = rawCollection.Select(s => GetNakedObjectFromId(surface, s).Object).ToArray();

            SurfaceHelper.ForEach(objCollection.Where(o => o != null), o => typedCollection.Add(o));

            return typedCollection.AsQueryable();
        }
    }
}