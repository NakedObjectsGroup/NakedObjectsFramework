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

namespace NakedObjects.Facade.Utility.Restricted {
    public static class SurfaceHelper {
        // todo cloned from typeutils move to common helper dll

        private const string NakedObjectsProxyPrefix = "NakedObjects.Proxy.";
        private const string EntityProxyPrefix = "System.Data.Entity.DynamicProxies.";
        private const string CastleProxyPrefix = "Castle.Proxies.";

        public static T GetDomainObject<T>(this IObjectFacade nakedObject) {
            return nakedObject == null ? default(T) : (T) nakedObject.Object;
        }

        public static void ForEach<T>(this T[] toIterate, Action<T> action) {
            Array.ForEach(toIterate, action);
        }

        public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T> action) {
            foreach (T obj in toIterate) {
                action(obj);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> toIterate, Action<T, int> action) {
            int num = 0;
            foreach (T obj in toIterate) {
                action(obj, num++);
            }
        }

        public static IEnumerable<IActionFacade> GetTopLevelActions(this IFrameworkFacade surface, IObjectFacade nakedObject) {
            if (nakedObject.Specification.IsQueryable) {
                var elementSpec = nakedObject.ElementSpecification;
                Trace.Assert(elementSpec != null);
                return elementSpec.GetCollectionContributedActions();
            }
            return nakedObject.Specification.GetActionLeafNodes().Where(a => a.IsVisible(nakedObject));
        }

        public static string GetObjectTypeShortName(this IFrameworkFacade surface, object model) {
            var nakedObject = surface.GetObject(model);
            return nakedObject.Specification.ShortName;
        }

        public static string IconName(IObjectFacade nakedObject) {
            string name = nakedObject.Specification.GetIconName(nakedObject);
            return name.Contains(".") ? name : name + ".png";
        }

        private static IObjectFacade GetNakedObjectFromId(IFrameworkFacade surface, string id) {
            var oid = surface.OidTranslator.GetOidTranslation(id);
            return surface.GetObject(oid).Target;
        }

        public static object GetTypedCollection(this IFrameworkFacade surface, IActionParameterFacade featureSpec, IEnumerable collectionValue) {
            var collectionitemSpec = featureSpec.ElementType;
            return GetTypedCollection(surface, collectionValue, collectionitemSpec);
        }

        public static object GetTypedCollection(this IFrameworkFacade surface, IAssociationFacade featureSpec, IEnumerable collectionValue) {
            var collectionitemSpec = featureSpec.ElementSpecification;
            return GetTypedCollection(surface, collectionValue, collectionitemSpec);
        }

        public static List<T> InList<T>(this T item) {
            return item == null ? new List<T>() : new List<T> {item};
        }

        private static IObjectFacade SafeGetObjectFromId(this IFrameworkFacade surface, string id) {
            return string.IsNullOrWhiteSpace(id) ? null : GetNakedObjectFromId(surface, id);
        }

        private static object GetTypedCollection(this IFrameworkFacade surface, IEnumerable collectionValue, ITypeFacade collectionitemSpec) {
            string[] rawCollection = collectionValue.Cast<string>().ToArray();

            Type instanceType = collectionitemSpec.GetUnderlyingType();
            var typedCollection = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(instanceType));

            if (collectionitemSpec.IsParseable) {
                return rawCollection.Select(s => string.IsNullOrEmpty(s) ? null : s).ToArray();
            }

            // need to check if collection is actually a collection memento 
            if (rawCollection.Count() == 1) {
                var id = rawCollection.First();
                var firstObj = SafeGetObjectFromId(surface, id);

                if (firstObj != null && firstObj.IsCollectionMemento) {
                    return firstObj.Object;
                }
            }

            var objCollection = rawCollection.Select(s => SafeGetObjectFromId(surface, s).GetDomainObject<object>()).ToArray();

            objCollection.Where(o => o != null).ForEach(o => typedCollection.Add(o));

            return typedCollection.AsQueryable();
        }

        private static bool IsNakedObjectsProxy(string typeName) {
            return typeName.StartsWith(NakedObjectsProxyPrefix);
        }

        private static bool IsCastleProxy(string typeName) {
            return typeName.StartsWith(CastleProxyPrefix);
        }

        private static bool IsEntityProxy(string typeName) {
            return typeName.StartsWith(EntityProxyPrefix);
        }

        private static bool IsProxy(Type type) {
            return IsProxy(type.FullName ?? "");
        }

        private static bool IsProxy(string typeName) {
            return IsEntityProxy(typeName) || IsNakedObjectsProxy(typeName) || IsCastleProxy(typeName);
        }

        public static string GetProxiedTypeFullName(this Type type) {
            return IsProxy(type) ? type.BaseType.FullName : type.FullName;
        }
    }
}