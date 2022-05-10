// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Configuration;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class RedirectedFacet : FacetAbstract, IRedirectedFacet, IMultipleImperativeFacet {
    private readonly PropertySerializationWrapper oid;
    private readonly PropertySerializationWrapper serverName;

    public RedirectedFacet(PropertyInfo serverName, PropertyInfo oid, ILogger<RedirectedFacet> logger) {
        this.serverName = new PropertySerializationWrapper(serverName, logger, ReflectorDefaults.JitSerialization);
        this.oid = new PropertySerializationWrapper(oid, logger, ReflectorDefaults.JitSerialization);
    }

    public MethodInfo GetMethod(int index) =>
        index switch {
            0 => serverName.GetMethod(),
            1 => oid.GetMethod(),
            _ => null
        };

    public Func<object, object[], object> GetMethodDelegate(int index) =>
        index switch {
            0 => serverName.GetMethodDelegate,
            1 => oid.GetMethodDelegate,
            _ => null
        };

    public int Count => 2;

    public override Type FacetType => typeof(IRedirectedFacet);
    public (string serverName, string oid)? GetRedirection(object target) => (ServerName(target), Oid(target));

    private string Oid(object target) => (string)oid.GetMethodDelegate(target, null);

    private string ServerName(object target) => (string)serverName.GetMethodDelegate(target, null);
}