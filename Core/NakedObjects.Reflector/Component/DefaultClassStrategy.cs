// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflect.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public sealed class DefaultClassStrategy : IClassStrategy {
        private readonly IObjectReflectorConfiguration config;

        // only intended for use during initial reflection
        [NonSerialized] private IImmutableDictionary<Type, bool> namespaceScratchPad = ImmutableDictionary<Type, bool>.Empty;

        public DefaultClassStrategy(IObjectReflectorConfiguration config) => this.config = config;

        #region IClassStrategy Members

        public bool IsTypeToBeIntrospected(Type type) {
            var returnType = FilterNullableAndProxies(type);
            return !IsTypeMarkedUpToBeIgnored(returnType) &&
                   !IsTypeUnsupportedByReflector(returnType) &&
                   IsTypeWhiteListed(returnType) &&
                   (!IsGenericCollection(type) || type.GetGenericArguments().All(IsTypeToBeIntrospected));
        }

        public Type GetType(Type type) {
            var returnType = FilterNullableAndProxies(type);
            return IsTypeToBeIntrospected(returnType) ? returnType : null;
        }

        public Type FilterNullableAndProxies(Type type) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                // use type inside nullable wrapper
                return type.GetGenericArguments()[0];
            }

            return TypeUtils.IsProxy(type) ? type.BaseType : type;
        }

        public bool IsSystemClass(Type introspectedType) => introspectedType.FullName.StartsWith("System.");

        public string GetKeyForType(Type type) {
            if (IsGenericCollection(type)) {
                return type.Namespace + "." + type.Name;
            }

            if (type.IsArray && !(type.GetElementType().IsValueType || type.GetElementType() == typeof(string))) {
                return "System.Array";
            }

            return type.GetProxiedTypeFullName();
        }

        #endregion

        private bool IsTypeMarkedUpToBeIgnored(Type type) => false;

        //var attr = type.GetCustomAttribute<NakedObjectsTypeAttribute>();
        //return attr != null && attr.ReflectionScope == ReflectOver.None;
        private bool IsNamespaceMatch(Type type) {
            if (!namespaceScratchPad.ContainsKey(type)) {
                var ns = type.Namespace ?? "";
                var match = config.ModelNamespaces.Any(ns.StartsWith);
                namespaceScratchPad = namespaceScratchPad.Add(type, match);
            }

            return namespaceScratchPad[type];
        }

        private bool IsTypeWhiteListed(Type type) => IsTypeSupportedSystemType(type) || IsNamespaceMatch(type) || IsTypeExplicitlyRequested(type);

        private bool IsTypeExplicitlyRequested(Type type) {
            IEnumerable<Type> services = config.Services;
            return config.TypesToIntrospect.Any(t => t == type) || services.Any(t => t == type) || type.IsGenericType && config.TypesToIntrospect.Any(t => t == type.GetGenericTypeDefinition());
        }

        private Type ToMatch(Type type) => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        private bool IsTypeSupportedSystemType(Type type) {
            return config.SupportedSystemTypes.Any(t => t == ToMatch(type));
        }

        private bool IsTypeUnsupportedByReflector(Type type) =>
            type.IsPointer ||
            type.IsByRef ||
            CollectionUtils.IsDictionary(type) ||
            type.IsGenericParameter ||
            type.ContainsGenericParameters;

        // because Sets don't implement IEnumerable<>
        private static bool IsGenericCollection(Type type) =>
            CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
            CollectionUtils.IsGenericType(type, typeof(ISet<>));
    }

    // Copyright (c) Naked Objects Group Ltd.
}