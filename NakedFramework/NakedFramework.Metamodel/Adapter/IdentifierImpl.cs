// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Adapter {
    [Serializable]
    public class IdentifierImpl : IIdentifier {
        private readonly string className;
        private readonly string name;
        private readonly string[] parameterTypes;
        private string asString;
        private string identityString;

        public IdentifierImpl(string className)
            : this(className, "", Array.Empty<string>(), Array.Empty<string>(), false) { }

        public IdentifierImpl(string className, string fieldName)
            : this(className, fieldName, Array.Empty<string>(), Array.Empty<string>(), true) { }

        public IdentifierImpl(string className, string methodName, ParameterInfo[] parameters)
            : this(className, methodName, parameters.Select(p => p.Name).ToArray(), ToParameterStringArray(parameters.Select(p => p.ParameterType).ToArray()), false) { }

        public IdentifierImpl(string className, string methodName, string[] parameterTypeNames)
            : this(className, methodName, parameterTypeNames.Select(p => "").ToArray(), parameterTypeNames, false) { }

        private IdentifierImpl(string className, string fieldName, string[] parameterNames, string[] parameterTypeNames, bool isField) {
            this.className = className;
            name = fieldName;
            parameterTypes = parameterTypeNames;
            MemberParameterNames = parameterNames;
            IsField = isField;
        }

        private static string FullName(Type type) {
            if (type.IsGenericType) {
                if (CollectionUtils.IsGenericEnumerable(type)) {
                    return TypeNameUtils.EncodeGenericTypeName(type);
                }

                if (type.Name.StartsWith("Nullable")) {
                    return $"{type.GetGenericArguments()[0].FullName}?";
                }
            }

            return type.FullName;
        }

        private static string[] ToParameterStringArray(Type[] fromArray) => fromArray.Select(FullName).ToArray();

        private static bool Equals(string[] a, string[] b) {
            if (a == null && b == null) {
                return true;
            }

            if (a == null || b == null) {
                // since not both null must be different
                return false;
            }

            if (a.Length != b.Length) {
                return false;
            }

            return !b.Where((t, i) => !string.Equals(a[i], t)).Any();
        }

        private string ToClassIdentityString() => className;

        private string ToNameIdentityString() => name;

        private string ToClassAndNameIdentityString() => $"{ToClassIdentityString()}#{name}";

        private string ToParmsIdentityString() => !IsField ? $"({string.Join(",", parameterTypes)})" : "";

        private string ToFullIdentityString() {
            if (identityString is null) {
                identityString = name.Length == 0 
                    ? ToClassIdentityString() 
                    : $"{ToClassAndNameIdentityString()}{ToParmsIdentityString()}";
            }

            return identityString;
        }

        public static IdentifierImpl FromIdentityString(IMetamodel metamodel, string asString) {
            if (asString == null) {
                throw new NakedObjectSystemException("asString cannot be null");
            }

            var indexOfHash = asString.IndexOf("#", StringComparison.InvariantCulture);
            var indexOfOpenBracket = asString.IndexOf("(", StringComparison.InvariantCulture);
            var indexOfCloseBracket = asString.IndexOf(")", StringComparison.InvariantCulture);
            var className = asString[..(indexOfHash == -1 ? asString.Length : indexOfHash)];
            if (indexOfHash == -1 || indexOfHash == asString.Length - 1) {
                return new IdentifierImpl(className);
            }

            string name;
            if (indexOfOpenBracket == -1) {
                name = asString[(indexOfHash + 1)..];
                return new IdentifierImpl(className, name);
            }

            name = asString[(indexOfHash + 1)..indexOfOpenBracket];
            var allParms = asString[(indexOfOpenBracket + 1)..indexOfCloseBracket].Trim();
            var parms = allParms.Length > 0 ? allParms.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
            return new IdentifierImpl(className, name, parms);
        }

        #region IIdentifier Members

        public virtual string ClassName => className;

        public virtual string MemberName => name;

        public virtual string[] MemberParameterTypeNames => parameterTypes;

        public virtual string[] MemberParameterNames { get; }

        public virtual bool IsField { get; }

        public virtual string ToIdentityString(IdentifierDepth depth) =>
            depth switch {
                IdentifierDepth.Class => ToClassIdentityString(),
                IdentifierDepth.ClassName => ToClassAndNameIdentityString(),
                IdentifierDepth.ClassNameParams => ToFullIdentityString(),
                IdentifierDepth.Name => ToNameIdentityString(),
                IdentifierDepth.Parms => ToParmsIdentityString(),
                _ => throw new NakedObjectSystemException($"depth out of bounds: {depth}")
            };

        public virtual string ToIdentityStringWithCheckType(IdentifierDepth depth, CheckType checkType) => $"{ToIdentityString(depth)}:{checkType}";

        public int CompareTo(object o2) => string.CompareOrdinal(ToString(), o2.ToString());

        #endregion

        #region Object overrides

        public override bool Equals(object obj) {
            if (this == obj) {
                return true;
            }

            return obj is IdentifierImpl other &&
                   string.Equals(other.className, className) &&
                   string.Equals(other.name, name) &&
                   Equals(other.parameterTypes, parameterTypes);
        }

        public override string ToString() {
            if (asString is null) {
                asString = $"{className}#{name}({string.Join(", ", parameterTypes)})";
            }

            return asString;
        }

        public override int GetHashCode() => (className + name + parameterTypes.Aggregate("", (s, t) => $"{s}{t}")).GetHashCode();

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}