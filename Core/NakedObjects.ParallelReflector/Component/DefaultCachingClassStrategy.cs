// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Configuration;

namespace NakedObjects.ParallelReflect.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public sealed class DefaultCachingClassStrategy : DefaultClassStrategy {
      
        public DefaultCachingClassStrategy(IReflectorConfiguration config) : base (config) {
           
        }

        private class Flags {
            public bool? IsTypeToBeIntrospected { get; set; }
            public Type Type { get; set; }
            public Type FilterNullableAndProxies { get; set; }
            public bool? IsSystemClass { get; set; }
            public string KeyForType { get; set; }
        }

        private static readonly IDictionary<Type, Flags> Cache = new Dictionary<Type, Flags>();  

        #region IClassStrategy Members

        public override bool IsTypeToBeIntrospected(Type type) {
            lock (Cache) {
                if (!Cache.ContainsKey(type)) {
                    Cache[type] = new Flags();
                }

                if (!Cache[type].IsTypeToBeIntrospected.HasValue) {
                    Cache[type].IsTypeToBeIntrospected = base.IsTypeToBeIntrospected(type);
                }

                return Cache[type].IsTypeToBeIntrospected.Value;
            }
        }

        public override Type GetType(Type type) {
            lock (Cache) {
                if (!Cache.ContainsKey(type)) {
                    Cache[type] = new Flags();
                }

                if (Cache[type].Type == null) {
                    Cache[type].Type = base.GetType(type);
                }

                return Cache[type].Type;
            }
        }

        public override Type FilterNullableAndProxies(Type type) {
            lock (Cache) {
                if (!Cache.ContainsKey(type)) {
                    Cache[type] = new Flags();
                }

                if (Cache[type].FilterNullableAndProxies == null) {
                    Cache[type].FilterNullableAndProxies = base.FilterNullableAndProxies(type);
                }

                return Cache[type].FilterNullableAndProxies;
            }
        }

        public override bool IsSystemClass(Type introspectedType) {
            lock (Cache) {
                if (!Cache.ContainsKey(introspectedType)) {
                    Cache[introspectedType] = new Flags();
                }

                if (!Cache[introspectedType].IsSystemClass.HasValue) {
                    Cache[introspectedType].IsSystemClass = base.IsSystemClass(introspectedType);
                }

                return Cache[introspectedType].IsSystemClass.Value;
            }
        }

        public override string GetKeyForType(Type type) {
            lock (Cache) {
                if (!Cache.ContainsKey(type)) {
                    Cache[type] = new Flags();
                }

                if (Cache[type].KeyForType == null) {
                    Cache[type].KeyForType = base.GetKeyForType(type);
                }

                return Cache[type].KeyForType;
            }
        }

        #endregion

    }

    // Copyright (c) Naked Objects Group Ltd.
}