// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.SpecImmutable;

public abstract class AbstractAdapter : MemberSpecImmutable {
    private readonly IActionSpecImmutable action;

    protected AbstractAdapter(IActionSpecImmutable action) : base(action.Identifier) => this.action = action;

    public override IObjectSpecImmutable ElementSpec => action.ElementSpec;
    public override IObjectSpecImmutable ReturnSpec => action.ReturnSpec;
    public IObjectSpecImmutable OwnerSpec => action.OwnerSpec as IObjectSpecImmutable;

    public override Type[] FacetTypes => action.FacetTypes;

    public override IFacet GetFacet(Type facetType) => action.GetFacet(facetType);

    public override IEnumerable<IFacet> GetFacets() => action.GetFacets();

    public override void AddFacet(IFacet facet) => action.AddFacet(facet);
}

public class ActionToAssociationSpecAdapter : AbstractAdapter, IOneToOneAssociationSpecImmutable {
    public ActionToAssociationSpecAdapter(IActionSpecImmutable action) : base(action) { }
}

public class ActionToCollectionSpecAdapter : AbstractAdapter, IOneToManyAssociationSpecImmutable {
    public ActionToCollectionSpecAdapter(IActionSpecImmutable action) : base(action) { }

    public string[] ContributedActionNames => Array.Empty<string>();
}