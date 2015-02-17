// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Web;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Core.Resolve;
using NakedObjects.Web.Mvc.Html;

namespace NakedObjects.Web.Mvc {
    public static class SessionCache {
        public static void AddObjectToSession(this HttpSessionStateBase session, INakedObjectsFramework framework, string key, object domainObject) {
            INakedObject nakedObject = framework.GetNakedObject(domainObject);
            session.Add(key, (nakedObject.ResolveState.IsTransient() ? domainObject : framework.GetObjectId(domainObject)));
        }

        public static void AddObjectToSession<T>(this HttpSessionStateBase session, INakedObjectsFramework framework, string key, T domainObject) where T : class {
            INakedObject nakedObject = framework.GetNakedObject(domainObject);
            session.Add(key, (nakedObject.ResolveState.IsTransient() ? (object) domainObject : framework.GetObjectId(domainObject)));
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

        public static object GetObjectFromSession(this HttpSessionStateBase session, INakedObjectsFramework framework, string key) {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (rawValue is string) {
                return framework.GetObjectFromId((string) rawValue);
            }

            return rawValue;
        }

        public static T GetObjectFromSession<T>(this HttpSessionStateBase session, INakedObjectsFramework framework, string key) where T : class {
            object rawValue = session[key];

            if (rawValue == null) {
                return null;
            }

            if (typeof (T).IsAssignableFrom(rawValue.GetType())) {
                return (T) rawValue;
            }

            if (rawValue is string) {
                var obj = framework.GetObjectFromId((string) rawValue);

                if (typeof (T).IsAssignableFrom(obj.GetType())) {
                    return (T) obj;
                }
            }

            return null;
        }

        public static void ClearFromSession(this HttpSessionStateBase session, string key) {
            session.Remove(key);
        }
    }
}