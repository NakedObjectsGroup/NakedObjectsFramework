// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Core.Error;
using NakedFramework.ParallelReflector.Utils;

namespace NOF2.Reflector.Configuration;

[Serializable]
public class NOF2ReflectorConfiguration : INOF2ReflectorConfiguration {
    public NOF2ReflectorConfiguration(Type[] typesToIntrospect,
                                      Type[] services,
                                      Type[] valueHolderTypes,
                                      bool concurrencyChecking = true) {
        TypesToIntrospect = typesToIntrospect;
        Services = services;
        ValueHolderTypes = valueHolderTypes;
        IgnoreCase = true;
        ConcurrencyChecking = concurrencyChecking;
    }

    public Type[] Types => ObjectTypes;

    private Type[] GetObjectTypesToIntrospect() {
        var types = TypesToIntrospect.Select(ReflectorHelpers.EnsureGenericTypeIsComplete);
        return types.ToArray();
    }

    #region NOF2ReflectorConfiguration Members

    public Type[] TypesToIntrospect { get; }
    public Type[] ValueHolderTypes { get; }
    public bool IgnoreCase { get; }
    public bool ConcurrencyChecking { get; }
    public bool HasConfig() => TypesToIntrospect.Any() && Services.Any();

    public Type[] Services { get; }

    public Type[] ObjectTypes => Services.Union(GetObjectTypesToIntrospect()).ToArray();

    #endregion
}