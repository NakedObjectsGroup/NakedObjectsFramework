// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace NakedObjects.Core.Util {
    public static class TypeKeyUtils {
        // because Sets don't implement IEnumerable<>
        private static bool IsGenericCollection(Type type) =>
            CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
            CollectionUtils.IsGenericType(type, typeof(ISet<>));


        public static Type FilterNullableAndProxies(Type type) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                // use type inside nullable wrapper
                return type.GetGenericArguments()[0];
            }

            return TypeUtils.IsProxy(type) ? type.BaseType : type;
        }

        public static bool IsSystemClass(Type introspectedType) => introspectedType.FullName?.StartsWith("System.", StringComparison.Ordinal) == true;

        public static string GetKeyForType(Type type) =>
            FasterTypeUtils.IsGenericCollection(type)
                ? $"{type.Namespace}.{type.Name}"
                : FasterTypeUtils.IsObjectArray(type)
                    ? "System.Array"
                    : type.GetProxiedTypeFullName();
    }
}