// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc {
    public static class ObjectCache {
        #region ObjectFlag enum

        public enum ObjectFlag {
            None = 0,
            BreadCrumb = 1
        }

        #endregion

        [Serializable]
        private class CacheMemento {
            public DateTime Added { get; set; }
            public string Url { get; set; }
            public string Spec { get; set; }
        }

        private const string NoneBucket = "ObjectCache";
        private const string BreadCrumbBucket = "BreadCrumbCache";


        public const int CacheSize = 100;
        private static readonly string[] Bucket = new[] { NoneBucket, BreadCrumbBucket };

        public static void AddToCache(this HttpSessionStateBase session, object domainObject, string url, ObjectFlag flag = ObjectFlag.None) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.AddToCache(nakedObject, url, flag);
        }

        public static void AddOrUpdateInCache(this HttpSessionStateBase session, object domainObject, string url, ObjectFlag flag = ObjectFlag.None) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.AddOrUpdateInCache(nakedObject, url, flag);
        }

        public static void AddToCache(this HttpSessionStateBase session, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.AddToCache(nakedObject, flag);
        }

        public static void AddToCache(this HttpSessionStateBase session, INakedObject nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.AddToCache(nakedObject, null, flag);
        }

        private static void ClearPreviousTransients(this HttpSessionStateBase session, INakedObject nakedObject, ObjectFlag flag) {
            if (nakedObject.Oid.HasPrevious) {
                if (nakedObject.Oid.Previous.IsTransient) {
                    session.GetCache(flag).Remove(FrameworkHelper.GetObjectId(nakedObject.Oid.Previous));
                }
            }
        }

        public static void AddToCache(this HttpSessionStateBase session, INakedObject nakedObject, string url, ObjectFlag flag = ObjectFlag.None) {
            // only add transients if we are storing transients in the session 

            if ((!nakedObject.ResolveState.IsTransient() || MvcIdentityAdapterHashMap.StoringTransientsInSession) || nakedObject.Specification.IsCollection) {
                session.ClearPreviousTransients(nakedObject, flag);
                session.GetCache(flag).AddToCache(nakedObject, url, flag);
            }
        }

        public static void AddOrUpdateInCache(this HttpSessionStateBase session, INakedObject nakedObject, string url, ObjectFlag flag = ObjectFlag.None) {
            // only add transients if we are storing transients in the session 

            if ((!nakedObject.ResolveState.IsTransient() || MvcIdentityAdapterHashMap.StoringTransientsInSession) || nakedObject.Specification.IsCollection) {
                session.ClearPreviousTransients(nakedObject, flag);
                session.GetCache(flag).AddOrUpdateInCache(nakedObject, url, flag);
            }
        }

        internal static void TestAddToCache(this HttpSessionStateBase session, INakedObject nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).AddToCache(nakedObject, null, flag);
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.RemoveFromCache(nakedObject, flag);
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, object domainObject, ObjectFlag flag = ObjectFlag.None) {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.RemoveOthersFromCache(nakedObject, flag);
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, string objectId, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveOthersFromCache(objectId);
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, INakedObject nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveFromCache(nakedObject);
        }

        public static void RemoveFromCache(this HttpSessionStateBase session, string objectId, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveFromCache(objectId);
        }

        public static void RemoveOthersFromCache(this HttpSessionStateBase session, INakedObject nakedObject, ObjectFlag flag = ObjectFlag.None) {
            session.GetCache(flag).RemoveOthersFromCache(nakedObject);
        }

        public static object LastObject(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            KeyValuePair<string, CacheMemento> lastEntry = session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).LastOrDefault();

            if (lastEntry.Equals(default(KeyValuePair<string, CacheMemento>))) {
                return null;
            }

            INakedObject lastObject = SafeGetNakedObjectFromId(lastEntry.Key);

            if (lastObject.ResolveState.IsDestroyed()) {
                session.GetCache(flag).Remove(lastEntry.Key);
                return session.LastObject(flag);
            }

            return lastObject.Object;
        }

        // This is dangerous - retrieves all cached objects from the database - use with care !
        internal static IEnumerable<object> AllCachedObjects(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            return session.GetAndTidyCachedNakedObjects(flag).Where(no => !no.ResolveState.IsDestroyed()).Select(no => no.Object);
        }

        public static IEnumerable<string> AllCachedUrls(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            return session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).Where(kvp => kvp.Value.Url != null).Select(kvp => kvp.Value.Url);
        }

        // This is dangerous - retrieves all cached objects from the database - use with care !
        private static IEnumerable<INakedObject> GetAndTidyCachedNakedObjects(this HttpSessionStateBase session, ObjectFlag flag) {
            session.ClearDestroyedObjects(flag);
            return session.GetCache(flag).OrderBy(kvp => kvp.Value.Added).Select(kvp => FrameworkHelper.GetNakedObjectFromId(kvp.Key));
        }

        private static bool SameSpec(string name, INakedObjectSpecification otherSpec) {
            var thisSpec = NakedObjectsContext.Reflector.LoadSpecification(name);
            return thisSpec.IsOfType(otherSpec);
        }

        private static IEnumerable<INakedObject> GetAndTidyCachedNakedObjectsOfType(this HttpSessionStateBase session, INakedObjectSpecification spec, ObjectFlag flag) {
            session.ClearDestroyedObjectsOfType(spec, flag);
            return session.GetCache(flag).Where(cm => SameSpec(cm.Value.Spec, spec)).OrderBy(kvp => kvp.Value.Added).Select(kvp => FrameworkHelper.GetNakedObjectFromId(kvp.Key));
        }

        public static IEnumerable<object> CachedObjectsOfType(this HttpSessionStateBase session, INakedObjectSpecification spec, ObjectFlag flag = ObjectFlag.None) {
            return session.GetAndTidyCachedNakedObjectsOfType(spec, flag).Select(no => no.Object);
        }

        // This is dangerous - retrieves all cached objects from the database - use with care !
        private static void ClearDestroyedObjects(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Select(kvp => new { kvp.Key, no = SafeGetNakedObjectFromId(kvp.Key) }).Where(ao => ao.no.ResolveState.IsDestroyed()).Select(ao => ao.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        public static void ClearDestroyedObjectsOfType(this HttpSessionStateBase session, INakedObjectSpecification spec, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Where(cm => SameSpec(cm.Value.Spec, spec)).Select(kvp => new { kvp.Key, no = SafeGetNakedObjectFromId(kvp.Key) }).Where(ao => ao.no.ResolveState.IsDestroyed()).Select(ao => ao.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        public static void ClearCachedObjects(this HttpSessionStateBase session, ObjectFlag flag = ObjectFlag.None) {
            Dictionary<string, CacheMemento> cache = session.GetCache(flag);
            List<string> toRemove = cache.Select(kvp => kvp.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }

        private static INakedObject SafeGetNakedObjectFromId(string id) {
            try {
                return FrameworkHelper.GetNakedObjectFromId(id);
            }
            catch (Exception) {
                // create a NakedObject just to carry the 'Destroyed' state
                var no = FrameworkHelper.GetNakedObject(new object());
                no.ResolveState.Handle(Events.StartResolvingEvent);
                no.ResolveState.Handle(Events.DestroyEvent);
                return no;
            }
        }

        private static Dictionary<string, CacheMemento> GetCache(this HttpSessionStateBase session, ObjectFlag flag) {
            var objs = (Dictionary<string, CacheMemento>)session[Bucket[(int)flag]];
            if (objs == null) {
                objs = new Dictionary<string, CacheMemento>();
                session.Add(Bucket[(int)flag], objs);
            }
            return objs;
        }

        private static void AddOrUpdateInCache(this Dictionary<string, CacheMemento> cache, INakedObject nakedObject, string url, ObjectFlag flag) {
            string objectId = FrameworkHelper.GetObjectId(nakedObject);

            if (cache.ContainsKey(objectId)) {
                cache[objectId].Spec = nakedObject.Specification.FullName;
                cache[objectId].Url = url;
            }
            else {
                cache[objectId] = new CacheMemento { Added = DateTime.Now, Spec = nakedObject.Specification.FullName, Url = url };
                while (cache.Count > CacheSize) {
                    RemoveOldest(cache, flag);
                }
            }
        }

        private static void AddToCache(this Dictionary<string, CacheMemento> cache, INakedObject nakedObject, string url, ObjectFlag flag) {
            string objectId = FrameworkHelper.GetObjectId(nakedObject);
            cache[objectId] = new CacheMemento { Added = DateTime.Now, Spec = nakedObject.Specification.FullName, Url = url };
            while (cache.Count > CacheSize) {
                RemoveOldest(cache, flag);
            }
        }

        private static void RemoveOldest(Dictionary<string, CacheMemento> cache, ObjectFlag flag) {
            DateTime oldestEntry = cache.Values.Select(t => t.Added).Min();
            string oldestId = cache.Where(kvp => kvp.Value.Added == oldestEntry).Select(kvp => kvp.Key).First();
            cache.Remove(oldestId);
        }

        private static void RemoveFromCache(this Dictionary<string, CacheMemento> cache, INakedObject nakedObject) {
            cache.RemoveFromCache(FrameworkHelper.GetObjectId(nakedObject));
        }

        private static void RemoveFromCache(this Dictionary<string, CacheMemento> cache, string objectId) {
            cache.Remove(objectId);
        }

        private static void RemoveOthersFromCache(this Dictionary<string, CacheMemento> cache, INakedObject nakedObject) {
            string id = FrameworkHelper.GetObjectId(nakedObject);
            cache.RemoveOthersFromCache(id);
        }

        private static void RemoveOthersFromCache(this Dictionary<string, CacheMemento> cache, string id) {
            List<string> toRemove = cache.Where(kvp => kvp.Key != id).Select(kvp => kvp.Key).ToList();
            toRemove.ForEach(k => cache.Remove(k));
        }
    }
}