// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet; 

[Serializable]
public sealed class RedirectedFacet : FacetAbstract, IRedirectedFacet {
    private readonly PropertyInfo oid;
    private readonly PropertyInfo serverName;

    public RedirectedFacet(ISpecification holder, PropertyInfo serverName, PropertyInfo oid)
        : base(Type, holder) {
        this.serverName = serverName;
        this.oid = oid;
    }

    private static Type Type => typeof(IRedirectedFacet);
    public (string serverName, string oid)? GetRedirection(object target) => (ServerName(target), Oid(target));

    private string Oid(object target) => (string) oid.GetValue(target, null);

    private string ServerName(object target) => (string) serverName.GetValue(target, null);
}