// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class AbstractSpecAdapter : MemberSpecImmutable {
    private readonly IActionSpecImmutable action;

    protected AbstractSpecAdapter(IActionSpecImmutable action) : base(action.Identifier) => this.action = action;
    public Type OwnerType => action.OwnerType;

    public override Type[] FacetTypes => action.FacetTypes;

    public override IObjectSpecImmutable GetElementSpec(IMetamodel metamodel) => action.GetElementSpec(metamodel);
    public override IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel) => action.GetReturnSpec(metamodel);

    public IObjectSpecImmutable GetOwnerSpec(IMetamodel metamodel) => action.GetOwnerSpec(metamodel) as IObjectSpecImmutable;

    public override IFacet GetFacet(Type facetType) => action.GetFacet(facetType);

    public override IEnumerable<IFacet> GetFacets() => action.GetFacets();

    public override void AddFacet(IFacet facet) => action.AddFacet(facet);
}