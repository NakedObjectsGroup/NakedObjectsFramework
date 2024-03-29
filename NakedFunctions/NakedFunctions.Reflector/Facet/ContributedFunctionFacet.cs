﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;

namespace NakedFunctions.Reflector.Facet;

// Not Serializable. Should be removed from metamodel at end of model integration. 

public sealed class ContributedFunctionFacet : FacetAbstract, IContributedFunctionFacet, IRemovableFacet {
    private readonly List<IObjectSpecImmutable> collectionContributees = new();
    private readonly List<ITypeSpecImmutable> objectContributees = new();

    public override Type FacetType => typeof(IContributedFunctionFacet);

    public bool IsContributedToCollectionOf(IObjectSpecImmutable objectSpec) => collectionContributees.Any(objectSpec.IsOfType);

    public bool IsContributedTo(ITypeSpecImmutable spec) => objectContributees.Any(spec.IsOfType);

    public void Remove(ISpecificationBuilder specification) => specification.RemoveFacet(this);

    public void AddContributee(ITypeSpecImmutable type) => objectContributees.Add(type);

    public void AddCollectionContributee(IObjectSpecBuilder type) => collectionContributees.Add(type);
}