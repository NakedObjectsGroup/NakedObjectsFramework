// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Core.Util {
    public static class TypeNameUtils {
        public static string DecodeTypeName(string typeName, string separator = "-") {
            if (typeName.Contains("-")) {
                var rootType = typeName.Substring(0, typeName.IndexOf('`') + 2);
                var args = typeName.Substring(typeName.IndexOf('`') + 3).Split('-');

                var genericType = TypeUtils.GetType(rootType);
                var argTypes = args.Select(TypeUtils.GetType).ToArray();

                var newType = genericType.MakeGenericType(argTypes);

                return newType.FullName;
            }

            return typeName;
        }

        public static string EncodeTypeName(this IObjectSpec spec, params IObjectSpec[] elements) => EncodeTypeName(spec.FullName, "-", elements);

        public static string EncodeTypeName(string typeName, string separator = "-", params IObjectSpec[] elements) {
            var type = TypeUtils.GetType(typeName);

            if (type.IsGenericType) {
                return EncodeGenericTypeName(type, separator, elements);
            }

            return type.FullName;
        }

        public static string EncodeGenericTypeName(Type type, string separator = "-", params IObjectSpec[] elements) {
            var rootType = type.GetGenericTypeDefinition().FullName;

            var args = type.GetGenericArguments().Where(t => !string.IsNullOrEmpty(t.FullName)).Select(t => t.FullName).ToArray();

            args = args.Any() ? args : elements.Select(e => e.FullName).ToArray();

            return args.Aggregate(rootType, (s, t) => s + separator + t);
        }

        public static string GetShortName(string name) {
            name = name.Substring(name.LastIndexOf('.') + 1);
            if (name.LastIndexOf('`') > 0) {
                name = name.Substring(0, name.LastIndexOf('`'));
            }

            return name;
        }
    }
}