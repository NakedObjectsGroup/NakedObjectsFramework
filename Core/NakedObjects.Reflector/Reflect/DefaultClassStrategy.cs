// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    public class DefaultClassStrategy : IClassStrategy {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DefaultClassStrategy));

        #region IClassStrategy Members

        public virtual void Init() {}

        public virtual Type GetType(Type type) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>)) {
                // use type inside nullable wrapper
                Log.DebugFormat("Using wrapped type instead of {0}", type);
                return type.GetGenericArguments()[0];
            }

            if (TypeUtils.IsProxy(type)) {
                Log.DebugFormat("Using proxied type instead of {0}", type);
                return type.BaseType;
            }

            return type;
        }

        public virtual bool IsSystemClass(Type type) {
            return TypeUtils.IsSystem(type);
        }

        public virtual bool IsTypeUnsupportedByReflector(Type type) {
            return type.IsPointer ||
                   type.IsByRef ||
                   CollectionUtils.IsDictionary(type) ||
                   type.IsGenericParameter ||
                   type.ContainsGenericParameters ||
                   (type.IsGenericType && !(TypeUtils.IsNullableType(type) || CollectionUtils.IsGenericEnumerable(type)));
        }


        public string GetKeyForType(Type type) {
            // becauase Sets don't implement IEnumerable<>
            if (CollectionUtils.IsGenericType(type, typeof (IEnumerable<>)) ||
                CollectionUtils.IsGenericType(type, typeof (ISet<>))) {
                return type.Namespace + "." + type.Name;
            }

            if (type.IsArray) {
                return "System.Array";
            }

            return type.GetProxiedTypeFullName();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}