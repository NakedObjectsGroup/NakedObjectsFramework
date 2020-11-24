// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Util;

namespace NakedFunctions.Reflector.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public class FunctionClassStrategy : IClassStrategy {
        private readonly IFunctionalReflectorConfiguration config;

        public FunctionClassStrategy(IFunctionalReflectorConfiguration config) => this.config = config;

        private bool IsTypeIgnored(Type type) => type.GetCustomAttribute<NakedFunctionsIgnoreAttribute>() is not null || IsUnSupportedSystemType(type);


        private bool IsTypeWhiteListed(Type type) => IsTypeSupportedSystemType(type) || IsTypeExplicitlyRequested(type);

        private bool IsTypeExplicitlyRequested(Type type) {
            var types = config.Types.Union(config.Functions).ToArray();
            return types.Any(t => t == type) ||
                   type.IsGenericType && types.Any(t => t == type.GetGenericTypeDefinition());
        }

        private Type ToMatch(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        private bool IsTypeSupportedSystemType(Type type) => config.SupportedSystemTypes.Any(t => t == ToMatch(type));

        private bool IsUnSupportedSystemType(Type type) => FasterTypeUtils.IsSystem(type) && !IsTypeSupportedSystemType(type);

        private bool IsTypeUnsupportedByReflector(Type type) =>
            type.IsPointer ||
            type.IsByRef ||
            CollectionUtils.IsDictionary(type) ||
            type.IsGenericParameter ||
            type.ContainsGenericParameters;

        #region IClassStrategy Members

        public virtual bool IsNotIgnored(Type type) {
            var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
            return !IsTypeIgnored(returnType) &&
                   !IsTypeUnsupportedByReflector(returnType) &&
                   (!FasterTypeUtils.IsGenericCollection(type) || type.GetGenericArguments().All(IsNotIgnored));
        }

        public bool IsTypeRecognized(Type type) {
            var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
            return !IsTypeIgnored(returnType) &&
                   !IsTypeUnsupportedByReflector(returnType) &&
                   IsTypeWhiteListed(returnType) &&
                   (!FasterTypeUtils.IsGenericCollection(type) || type.GetGenericArguments().All(IsTypeRecognized));
        }

        public virtual Type GetType(Type type) {
            var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
            return IsNotIgnored(returnType) ? returnType : null;
        }

        public bool IsIgnored(MemberInfo member) => member.GetCustomAttribute<NakedFunctionsIgnoreAttribute>() is not null;
        public bool IsService(Type type) => false;
        public bool LoadReturnType(MethodInfo method) => false;

        #endregion

        // because Sets don't implement IEnumerable<>
    }

    // Copyright (c) Naked Objects Group Ltd.
}