// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NakedObjects.Facade;
using NakedObjects.Facade.Utility.Restricted;

namespace NakedObjects.Web.Mvc {
    public static class ObjectCache {
        #region ObjectFlag enum

        public enum ObjectFlag {
            None = 0,
            BreadCrumb = 1
        }

        #endregion

        private const string NoneBucket = "ObjectCache";
        private const string BreadCrumbBucket = "BreadCrumbCache";
        public const int CacheSize = 100;
        private static readonly string[] Bucket = {NoneBucket, BreadCrumbBucket};

        public static void AddToCache(this HttpSessionStateBase session, IFrameworkFacade facade, object domainObject, string url, ObjectFlag flag = ObjectFlag.None) {
            var nakedObject = facade.GetObject(domainObject);
            session.AddToCache(facade, nakedObject, url, flag);
        }

        public static void AddOrUpdateInCache(this HttpSessionStateBase session, IFrameworkFacade facade, object domainObject, string url, ObjectFlag flag = ObjectFlag.None) {
            var nakedObject = facade.GetObject(domainObject);
            session.AddOrUpdateInCache(facade, nakedObject, url, flag);
        }

        public static void AddToCache(this HttpSessionStateBase session, IFrameworkFacade facade, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            var nakedObject = facade.GetObject(domainObject);
            session.AddToCache(facade, nakedObject, flag);
        }

        public static void AddToCache(this HttpSessionStateBase session, IFrameworkFacade facade, IObjectFacade nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.AddToCache(facade, nakedObject, null, flag);
        }

        public static void AddToCache(this HttpSessionStateBase session, IFrameworkFacade facade, IObjectFacade nakedObject, string url, ObjectFlag flag = ObjectFlag.None) {
            // only add transients if we are storing transients in the session 

            if (!nakedObject.IsTransient || nakedObject.Specification.IsCollection) {
                //session.ClearPreviousTransients(nakedObject, flag);
                session.GetCache(flag).AddToCache(facade, nakedObject, url, flag);
            }
        }

        public static void AddOrUpdateInCache(this HttpSessionStateBase session, IFrameworkFacade facade, IObjectFacade nakedObject, string url, ObjectFlag flag = ObjectFlag.None) {
            // only add transients if we are storing transients in the session 

            if (!nakedObject.IsTransient || nakedObject.Specification.IsCollection) {
                //session.ClearPreviousTransients(nakedObject, flag);
                session.GetCache(flag).AddOrUpdateInCache(facade, nakedObject, url, flag);
            }
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, IFrameworkFacade facade, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            var nakedObject = facade.GetObject(domainObject);
            session.RemoveFromCache(facade, nakedObject, flag);
        }

        private static IObjectFacade GetNakedObject(IFrameworkFacade facade, object domainObject) {
            return facade.GetObject(domainObject);
        }

        private static IObjectFacade GetNakedObjectFromId(IFrameworkFacade facade, string id) {
            var oid = facade.OidTranslator.GetOidTranslation(id);
            return facade.GetObject(oid).Target;
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, IFrameworkFacade facade, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            var nakedObject = GetNakedObject(facade, domainObject);
            session.RemoveOthersFromCache(facade, nakedObject, flag);
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, string objectId, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveOthersFromCache(objectId);
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, IFrameworkFacade facade, IObjectFacade nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveFromCache(facade, nakedObject);
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, string objectId, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveFromCache(objectId);
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, IFrameworkFacade facade, IObjectFacade nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveOthersFromCache(facade, nakedObject);
        }

        public static object LastObject(this HttpSessionStateBase session, IFrameworkFacade facade, ObjectFlag flag = ObjectFlag.None) {
            KeyValuePair<string, CacheMemento> lastEntry = session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).LastOrDefault();

            if (lastEntry.Equals(default(KeyValuePair<string, CacheMemento>))) {
                return null;
            }

            var lastObject = SafeGetNakedObjectFromId(lastEntry.Key, facade);

            if (lastObject == null) {
                session.GetCache(flag).Remove(lastEntry.Key);
                return session.LastObject(facade, flag);
            }

            return lastObject.GetDomainObject();
        }

        internal static IEnumerable<object> AllCachedObjects(this HttpSessionStateBase session, IFrameworkFacade facade, ObjectFlag flag = ObjectFlag.None) {
            return session.GetAndTidyCachedNakedObjects(facade, flag).Where(no => !no.IsDestroyed).Select(no => no.GetDomainObject());
        }

        public static IEnumerable<string> AllCachedUrls(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            return session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).Where(kvp => kvp.Value.Url != null).Select(kvp => kvp.Value.Url);
        }

        // This is dangerous - retrieves all cached objects from the database - use with care !

        private static IEnumerable<IObjectFacade> GetAndTidyCachedNakedObjects(this HttpSessionStateBase session, IFrameworkFacade facade, ObjectFlag flag) {
            session.ClearDestroyedObjects(facade, flag);
            return session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).Select(kvp => GetNakedObjectFromId(facade, kvp.Key));
        }

        private static bool SameSpec(string name, ITypeFacade otherSpec, IFrameworkFacade facade) {
            var thisSpec = facade.GetDomainType(name);
            return thisSpec.IsOfType(otherSpec);
        }

        private static IEnumerable<IObjectFacade> GetAndTidyCachedNakedObjectsOfType(this HttpSessionStateBase session, IFrameworkFacade facade, ITypeFacade spec, ObjectFlag flag) {
            session.ClearDestroyedObjectsOfType(facade, spec, flag);
            return session.GetCache(flag).Where(cm => SameSpec(cm.Value.Spec, spec, facade)).OrderBy(kvp => kvp.Value.Added).Select(kvp => GetNakedObjectFromId(facade, kvp.Key));
        }

        public static IEnumerable<object> CachedObjectsOfType(this HttpSessionStateBase session, IFrameworkFacade facade, ITypeFacade spec, ObjectFlag flag = ObjectFlag.None) {
            return session.GetAndTidyCachedNakedObjectsOfType(facade, spec, flag).Select(no => no.GetDomainObject());
        }

        // This is dangerous - retrieves all cached objects from the database - use with care !

        private static void ClearDestroyedObjects(this HttpSessionStateBase session, IFrameworkFacade facade, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Select(kvp => new {kvp.Key, no = SafeGetNakedObjectFromId(kvp.Key, facade)}).Where(ao => ao.no == null).Select(ao => ao.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        public static void ClearDestroyedObjectsOfType(this HttpSessionStateBase session, IFrameworkFacade facade, ITypeFacade spec, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Where(cm => SameSpec(cm.Value.Spec, spec, facade)).Select(kvp => new {kvp.Key, no = SafeGetNakedObjectFromId(kvp.Key, facade)}).Where(ao => ao.no == null).Select(ao => ao.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        public static void ClearCachedObjects(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Select(kvp => kvp.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        private static IObjectFacade SafeGetNakedObjectFromId(string id, IFrameworkFacade facade) {
            try {
                var oid = facade.OidTranslator.GetOidTranslation(id);
                return facade.GetObject(oid).Target;
            }
            catch (Exception) {
                return null;
            }
        }

        private static Dictionary<string, CacheMemento> GetCache(this HttpSessionStateBase session, ObjectFlag flag) {
            var objs = (Dictionary<string, CacheMemento>) session[Bucket[(int) flag]];
            if (objs == null) {
                objs = new Dictionary<string, CacheMemento>();
                session.Add(Bucket[(int) flag], objs);
            }
            return objs;
        }

        private static void AddOrUpdateInCache(this Dictionary<string, CacheMemento> cache, IFrameworkFacade facade, IObjectFacade nakedObject, string url, ObjectFlag flag) {
            string objectId = facade.OidTranslator.GetOidTranslation(nakedObject).Encode();

            if (cache.ContainsKey(objectId)) {
                cache[objectId].Spec = nakedObject.Specification.FullName;
                cache[objectId].Url = url;
            }
            else {
                cache[objectId] = new CacheMemento {Added = DateTime.Now, Spec = nakedObject.Specification.FullName, Url = url};
                while (cache.Count > CacheSize) {
                    RemoveOldest(cache, flag);
                }
            }
        }

        private static void AddToCache(this Dictionary<string, CacheMemento> cache, IFrameworkFacade facade, IObjectFacade nakedObject, string url, ObjectFlag flag) {
            var loid = facade.OidTranslator.GetOidTranslation(nakedObject);

            string objectId = loid == null ? "" : loid.Encode();
            cache[objectId] = new CacheMemento {Added = DateTime.Now, Spec = nakedObject.Specification.FullName, Url = url};
            while (cache.Count > CacheSize) {
                RemoveOldest(cache, flag);
            }
        }

        private static void RemoveOldest(Dictionary<string, CacheMemento> cache, ObjectFlag flag) {
            DateTime oldestEntry = cache.Values.Select(t => t.Added).Min();
            string oldestId = cache.Where(kvp => kvp.Value.Added == oldestEntry).Select(kvp => kvp.Key).First();
            cache.Remove(oldestId);
        }

        private static void RemoveFromCache(this Dictionary<string, CacheMemento> cache, IFrameworkFacade facade, IObjectFacade nakedObject) {
            cache.RemoveFromCache(facade.OidTranslator.GetOidTranslation(nakedObject).ToString());
        }

        private static void RemoveFromCache(this Dictionary<string, CacheMemento> cache, string objectId) {
            cache.Remove(objectId);
        }

        private static void RemoveOthersFromCache(this Dictionary<string, CacheMemento> cache, IFrameworkFacade facade, IObjectFacade nakedObject) {
            string id = facade.OidTranslator.GetOidTranslation(nakedObject).ToString();
            cache.RemoveOthersFromCache(id);
        }

        private static void RemoveOthersFromCache(this Dictionary<string, CacheMemento> cache, string id) {
            List<string> toRemove = cache.Where(kvp => kvp.Key != id).Select(kvp => kvp.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        #region Nested type: CacheMemento

        [Serializable]
        private class CacheMemento {
            public DateTime Added { get; set; }
            public string Url { get; set; }
            public string Spec { get; set; }
        }

        #endregion
    }
}