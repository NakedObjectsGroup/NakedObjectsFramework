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

namespace NakedFramework.Metamodel.Adapter;

[Serializable]
public class IdentifierImpl : IIdentifier {
    [NonSerialized]
    private string asString;

    [NonSerialized]
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
        ClassName = className;
        MemberName = fieldName;
        MemberParameterTypeNames = parameterTypeNames;
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
        if (a is null && b is null) {
            return true;
        }

        if (a is null || b is null) {
            // since not both null must be different
            return false;
        }

        if (a.Length != b.Length) {
            return false;
        }

        return !b.Where((t, i) => !string.Equals(a[i], t)).Any();
    }

    private string ToClassAndNameIdentityString() => $"{ClassName}#{MemberName}";

    private string ToParmsIdentityString() => !IsField ? $"({string.Join(",", MemberParameterTypeNames)})" : "";

    private string ToFullIdentityString() =>
        identityString ??= MemberName.Length is 0
            ? ClassName
            : $"{ToClassAndNameIdentityString()}{ToParmsIdentityString()}";

    public static IdentifierImpl FromIdentityString(IMetamodel metamodel, string idString) {
        if (idString is null) {
            throw new NakedObjectSystemException("idString cannot be null");
        }

        var indexOfHash = idString.IndexOf("#", StringComparison.InvariantCulture);
        var indexOfOpenBracket = idString.IndexOf("(", StringComparison.InvariantCulture);
        var indexOfCloseBracket = idString.IndexOf(")", StringComparison.InvariantCulture);
        var className = idString[..(indexOfHash == -1 ? idString.Length : indexOfHash)];
        if (indexOfHash == -1 || indexOfHash == idString.Length - 1) {
            return new IdentifierImpl(className);
        }

        string name;
        if (indexOfOpenBracket == -1) {
            name = idString[(indexOfHash + 1)..];
            return new IdentifierImpl(className, name);
        }

        name = idString[(indexOfHash + 1)..indexOfOpenBracket];
        var allParms = idString[(indexOfOpenBracket + 1)..indexOfCloseBracket].Trim();
        var parms = allParms.Length > 0 ? allParms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
        return new IdentifierImpl(className, name, parms);
    }

    #region IIdentifier Members

    public virtual string ClassName { get; }

    public virtual string MemberName { get; }

    public virtual string[] MemberParameterTypeNames { get; }

    public virtual string[] MemberParameterNames { get; }

    public virtual bool IsField { get; }

    public virtual string ToIdentityString(IdentifierDepth depth) =>
        depth switch {
            IdentifierDepth.Class => ClassName,
            IdentifierDepth.ClassName => ToClassAndNameIdentityString(),
            IdentifierDepth.ClassNameParams => ToFullIdentityString(),
            IdentifierDepth.Name => MemberName,
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
               string.Equals(other.ClassName, ClassName) &&
               string.Equals(other.MemberName, MemberName) &&
               Equals(other.MemberParameterTypeNames, MemberParameterTypeNames);
    }

    public override string ToString() => asString ??= $"{ClassName}#{MemberName}({string.Join(", ", MemberParameterTypeNames)})";

    public override int GetHashCode() => (ClassName + MemberName + MemberParameterTypeNames.Aggregate("", (s, t) => $"{s}{t}")).GetHashCode();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.