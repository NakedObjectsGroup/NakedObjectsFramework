// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Util;

namespace NakedFramework.ParallelReflector.Component;

/// <summary>
///     Standard way of determining which fields are to be exposed in a Naked Objects system.
/// </summary>
[Serializable]
public abstract class AbstractClassStrategy : IClassStrategy {
    protected abstract bool IsTypeIgnored(Type type);

    private bool IsTypeWhiteListed(Type type) => IsTypeExplicitlyRequested(type);

    protected abstract bool IsTypeExplicitlyRequested(Type type);

    protected virtual Type ToMatch(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

    private static bool IsTypeUnsupportedByReflector(Type type) =>
        type.IsPointer ||
        type.IsByRef ||
        CollectionUtils.IsDictionary(type) ||
        type.IsGenericParameter ||
        type.ContainsGenericParameters;

    #region IClassStrategy Members

    public virtual bool IsIgnored(Type type) {
        var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
        return IsTypeIgnored(returnType) ||
               IsTypeUnsupportedByReflector(returnType) ||
               FasterTypeUtils.IsGenericCollection(type) && type.GetGenericArguments().Any(IsIgnored);
    }

    public bool IsTypeRecognized(Type type) {
        var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
        return !IsTypeIgnored(returnType) &&
               !IsTypeUnsupportedByReflector(returnType) &&
               IsTypeWhiteListed(returnType) &&
               (!FasterTypeUtils.IsGenericCollection(type) || type.GetGenericArguments().All(IsTypeRecognized));
    }

    public abstract bool IsIgnored(MemberInfo member);
    public abstract bool IsService(Type type);
    public abstract bool LoadReturnType(MethodInfo method);

    #endregion

    // because Sets don't implement IEnumerable<>
}

// Copyright (c) Naked Objects Group Ltd.