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
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public class ObjectClassStrategy : IClassStrategy {
        private readonly IObjectReflectorConfiguration config;
        private readonly IFunctionalReflectorConfiguration fConfig;

        public ObjectClassStrategy(IObjectReflectorConfiguration config,
                                    IFunctionalReflectorConfiguration fConfig = null)
        {
            this.config = config;
            this.fConfig = fConfig;
        }

        #region IClassStrategy Members

        public virtual bool IsTypeToBeIntrospected(Type type) {
            var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
            return !IsTypeIgnored(returnType) &&
                   !IsTypeUnsupportedByReflector(returnType) &&
                   IsTypeWhiteListed(returnType) &&
                   (!FasterTypeUtils.IsGenericCollection(type) || type.GetGenericArguments().All(IsTypeToBeIntrospected));
        }

        public bool IsIgnored(MemberInfo member) => member.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null;

        public virtual Type GetType(Type type) {
            var returnType = TypeKeyUtils.FilterNullableAndProxies(type);
            return IsTypeToBeIntrospected(returnType) ? returnType : null;
        }

        #endregion

        private bool IsTypeIgnored(Type type) => type.GetCustomAttribute<NakedObjectsIgnoreAttribute>() != null;

        private bool IsNamespaceMatch(Type type) {
            var ns = type.Namespace ?? "";
            return config.ModelNamespaces.Any(sn => ns.StartsWith(sn, StringComparison.Ordinal));
        }

        private bool IsTypeWhiteListed(Type type) => IsTypeSupportedSystemType(type) || IsNamespaceMatch(type) || IsTypeExplicitlyRequested(type);

        private bool IsTypeExplicitlyRequested(Type type) {
            var services = config.Services.Union(fConfig == null ? new Type[] { } : fConfig.Services).ToArray();
            return config.TypesToIntrospect.Any(t => t == type) ||
                   services.Any(t => t == type) ||
                   type.IsGenericType && config.TypesToIntrospect.Any(t => t == type.GetGenericTypeDefinition());
        }

        private Type ToMatch(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        private bool IsTypeSupportedSystemType(Type type) => config.SupportedSystemTypes.Any(t => t == ToMatch(type));

        private bool IsTypeUnsupportedByReflector(Type type) =>
            type.IsPointer ||
            type.IsByRef ||
            CollectionUtils.IsDictionary(type) ||
            type.IsGenericParameter ||
            type.ContainsGenericParameters;

        // because Sets don't implement IEnumerable<>
    }

    // Copyright (c) Naked Objects Group Ltd.
}