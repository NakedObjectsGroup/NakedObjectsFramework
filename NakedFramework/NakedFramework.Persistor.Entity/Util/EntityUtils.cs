// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Util;

namespace NakedFramework.Persistor.Entity.Util {
    public static class EntityUtils {
        public static void UpdateVersion(this INakedObjectAdapter nakedObjectAdapter, ISession session, INakedObjectManager manager) {
            var versionObject = nakedObjectAdapter?.GetVersion(manager);
            if (versionObject != null) {
                nakedObjectAdapter.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
            }
        }

        public static bool IsEntityProxy(Type type) => IsEntityProxy(type.FullName ?? "");

        public static bool IsEntityProxy(string typeName) => typeName.StartsWith("System.Data.Entity.DynamicProxies.");

        public static string GetEntityProxiedTypeName(object domainObject) => domainObject.GetEntityProxiedType().FullName;

        public static Type GetEntityProxiedType(this object domainObject) =>
            domainObject.GetType() switch {
                { } t when IsEntityProxy(t) => t.BaseType,
                { } t => t
            };
    }
}