// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc {
    public static class SessionCache {

        public static void AddObjectToSession(this HttpSessionStateBase session, string key, object domainObject)  {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.Add(key, (nakedObject.ResolveState.IsTransient() ? domainObject : FrameworkHelper.GetObjectId(domainObject)));
        }

        public static void AddObjectToSession<T>(this HttpSessionStateBase session, string key, T domainObject) where T : class {
            INakedObject nakedObject = FrameworkHelper.GetNakedObject(domainObject);
            session.Add(key,(nakedObject.ResolveState.IsTransient() ? (object)domainObject : FrameworkHelper.GetObjectId(domainObject)));
        }

        public static void AddValueToSession<T>(this HttpSessionStateBase session, string key, T value)   where T : struct{       
            session.Add(key, value);
        }

        public static void AddObjectToSession(this HttpSessionStateBase session, string key, string value) {
            session.Add(key, value);
        }

        public static T? GetValueFromSession<T>(this HttpSessionStateBase session, string key) where T : struct {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (typeof(T).IsAssignableFrom(rawValue.GetType())) {
                return (T)rawValue;
            }

            return null; 
        }

        public static object GetObjectFromSession(this HttpSessionStateBase session, string key) {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (rawValue is string) {
                return FrameworkHelper.GetObjectFromId((string) rawValue);
            }

            return rawValue;
        }


        public static T GetObjectFromSession<T>(this HttpSessionStateBase session, string key) where  T : class {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (typeof(T).IsAssignableFrom(rawValue.GetType())) {
                return (T)rawValue; 
            }

            if (rawValue is string) {
                var obj = FrameworkHelper.GetObjectFromId((string) rawValue);

                if (typeof(T).IsAssignableFrom(obj.GetType())) {
                    return (T)obj;
                }
            }

            return null;
        }


        public static void ClearFromSession(this HttpSessionStateBase session, string key) {
            session.Remove(key);
        }
    }
}
