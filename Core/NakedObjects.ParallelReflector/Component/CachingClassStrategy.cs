// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Component;

namespace NakedObjects.ParallelReflector.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public sealed class CachingClassStrategy : IClassStrategy {
        private static readonly IDictionary<Type, Flags> Cache = new Dictionary<Type, Flags>();
        private readonly IClassStrategy classStrategy;

        public CachingClassStrategy(IClassStrategy classStrategy) {
            this.classStrategy = classStrategy;
            lock (Cache) {
                Cache.Clear();
            }
        }

        #region IClassStrategy Members

        public bool IsTypeToBeIntrospected(Type type) {
            lock (Cache) {
                var flags = Setup(type);
                flags.IsTypeToBeIntrospected ??= classStrategy.IsTypeToBeIntrospected(type);
                return flags.IsTypeToBeIntrospected.Value;
            }
        }

        public bool IsIgnored(MemberInfo member) => classStrategy.IsIgnored(member);
        public bool IsService(Type type) => classStrategy.IsService(type);
        public bool LoadReturnType(MethodInfo method) => classStrategy.LoadReturnType(method);

        public Type GetType(Type type) {
            lock (Cache) {
                var flags = Setup(type);
                flags.Type ??= classStrategy.GetType(type);
                return flags.Type;
            }
        }

        #endregion

        private static Flags Setup(Type type) {
            if (!Cache.ContainsKey(type)) {
                Cache[type] = new Flags();
            }

            return Cache[type];
        }

        #region Nested type: Flags

        private class Flags {
            public bool? IsTypeToBeIntrospected { get; set; }
            public Type Type { get; set; }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}