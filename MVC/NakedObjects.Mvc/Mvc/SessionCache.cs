// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web;
using NakedObjects.Surface;

namespace NakedObjects.Web.Mvc {
    public static class SessionCache {
        public static void AddObjectToSession(this HttpSessionStateBase session, INakedObjectsSurface surface, string key, object domainObject) {
            var nakedObject = surface.GetObject(domainObject);
            session.Add(key, (nakedObject.IsTransient ? domainObject : surface.OidFactory.GetLinkOid(nakedObject).Encode()));
        }

        public static void AddObjectToSession<T>(this HttpSessionStateBase session, INakedObjectsSurface surface, string key, T domainObject) where T : class {
            var nakedObject = surface.GetObject(domainObject);
            session.Add(key, (nakedObject.IsTransient ? (object)domainObject : surface.OidFactory.GetLinkOid(nakedObject).Encode()));
        }

        public static void AddValueToSession<T>(this HttpSessionStateBase session, string key, T value) where T : struct {
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

            if (typeof (T).IsAssignableFrom(rawValue.GetType())) {
                return (T) rawValue;
            }

            return null;
        }

        private static INakedObjectSurface GetNakedObjectFromId(string id, INakedObjectsSurface surface) {
            if (string.IsNullOrEmpty(id)) {
                return null;
            }

            var oid = surface.OidFactory.GetLinkOid(id);
            return surface.GetObject(oid).Target;
        }

        public static object GetObjectFromSession(this HttpSessionStateBase session, INakedObjectsSurface surface, string key) {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            var s = rawValue as string;
            if (s != null) {
                return GetNakedObjectFromId(s, surface).Object;
            }

            return rawValue;
        }

        public static T GetObjectFromSession<T>(this HttpSessionStateBase session, INakedObjectsSurface surface, string key) where T : class {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (rawValue is T) {
                return (T) rawValue;
            }

            if (rawValue is string) {
                var obj = GetNakedObjectFromId((string) rawValue, surface).Object;

                var fromSession = obj as T;
                return fromSession;
            }

            return null;
        }

        public static void ClearFromSession(this HttpSessionStateBase session, string key) {
            session.Remove(key);
        }
    }
}