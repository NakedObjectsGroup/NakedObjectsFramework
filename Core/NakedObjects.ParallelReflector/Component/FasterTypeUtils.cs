// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Core.Util;

namespace NakedObjects.ParallelReflect.Component {
    public static class FasterTypeUtils {
        private const string SystemTypePrefix = "System.";
        private const string NakedObjectsTypePrefix = "NakedObjects.";
        private const string NakedObjectsProxyPrefix = "NakedObjects.Proxy.";
        private const string EntityProxyPrefix = "System.Data.Entity.DynamicProxies.";
        private const string CastleProxyPrefix = "Castle.Proxies.";

        public static bool IsNakedObjectsProxy(string typeName) => typeName.StartsWith(NakedObjectsProxyPrefix, StringComparison.Ordinal);

        public static bool IsCastleProxy(string typeName) => typeName.StartsWith(CastleProxyPrefix, StringComparison.Ordinal);

        public static bool IsEntityProxy(string typeName) => typeName.StartsWith(EntityProxyPrefix, StringComparison.Ordinal);

        public static bool IsProxy(Type type) => IsProxy(type.FullName ?? "");

        public static bool IsProxy(string typeName) => IsEntityProxy(typeName) || IsNakedObjectsProxy(typeName) || IsCastleProxy(typeName);

        public static bool IsSystem(string typeName) => typeName.StartsWith(SystemTypePrefix, StringComparison.Ordinal) && !IsEntityProxy(typeName);

        public static bool IsNakedObjects(string typeName) => typeName.StartsWith(NakedObjectsTypePrefix, StringComparison.Ordinal);

        public static bool IsSystem(Type type) => IsSystem(type.FullName ?? "");

        public static bool IsObjectArray(Type type) => type.IsArray && !(type.GetElementType()?.IsValueType == true || type.GetElementType() == typeof(string));

        public static bool IsGenericCollection(Type type) =>
            CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
            CollectionUtils.IsGenericType(type, typeof(ISet<>));
    }
}