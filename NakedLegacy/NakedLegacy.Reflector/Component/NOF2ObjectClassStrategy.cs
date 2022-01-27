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
using NakedFramework.ParallelReflector.Component;
using NOF2.Reflector.Configuration;
using NakedObjects;

namespace NOF2.Reflector.Component;

/// <summary>
///     Standard way of determining which fields are to be exposed in a Naked Objects system.
/// </summary>
[Serializable]
public class NOF2ObjectClassStrategy : AbstractClassStrategy {
    private readonly INOF2ReflectorConfiguration config;

    public NOF2ObjectClassStrategy(INOF2ReflectorConfiguration config, IAllTypeList allTypeList) : base(allTypeList) => this.config = config;

    protected override bool IsTypeIgnored(Type type) => type.GetCustomAttribute<NakedObjectsIgnoreAttribute>() is not null;

    protected override bool IsTypeExplicitlyRequested(Type type) {
        var services = config.Services.ToArray();
        return NOF2ReflectorDefaults.DefaultNOF2Types.Contains(type) ||
               config.ValueHolderTypes.Any(t => t == type) ||
               config.TypesToIntrospect.Any(t => t == type) ||
               services.Any(t => t == type);
    }

    public override bool IsTypeRecognizedByReflector(Type type) => IsTypeExplicitlyRequested(type);

    #region IClassStrategy Members

    public override bool IsIgnored(MemberInfo member) => member.GetCustomAttribute<NakedObjectsIgnoreAttribute>() is not null;
    public override bool IsService(Type type) => config.Services.Contains(type);
    public override bool LoadReturnType(MethodInfo method) => method.ReturnType != typeof(void);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.