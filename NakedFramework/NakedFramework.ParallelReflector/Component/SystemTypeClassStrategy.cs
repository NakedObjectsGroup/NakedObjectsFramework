// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Configuration;
using NakedFramework.Core.Util;

namespace NakedFramework.ParallelReflector.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public class SystemTypeClassStrategy : AbstractClassStrategy {
        private Type[] supportedSystemTypes;

        public SystemTypeClassStrategy(ICoreConfiguration coreConfiguration) => supportedSystemTypes = coreConfiguration.SupportedSystemTypes.ToArray();

        protected bool IsUnSupportedSystemType(Type type) => FasterTypeUtils.IsSystem(type) && !IsTypeSupportedSystemType(type);

        protected override bool IsTypeIgnored(Type type) => IsUnSupportedSystemType(type);

        protected override bool IsTypeExplicitlyRequested(Type type) => IsTypeSupportedSystemType(type);

        protected bool IsTypeSupportedSystemType(Type type) => supportedSystemTypes.Any(t => t == ToMatch(type));

        #region IClassStrategy Members

        public override bool IsIgnored(MemberInfo member) => false;
        public override bool IsService(Type type) => false;
        public override bool LoadReturnType(MethodInfo method) => method.ReturnType != typeof(void);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}