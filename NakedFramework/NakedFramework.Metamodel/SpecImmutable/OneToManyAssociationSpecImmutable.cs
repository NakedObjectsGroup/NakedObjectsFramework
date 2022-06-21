// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class OneToManyAssociationSpecImmutable : AssociationSpecImmutable, IOneToManyAssociationSpecBuilder {
    private readonly TypeSerializationWrapper defaultElementType;
    private readonly TypeSerializationWrapper ownerType;

    public OneToManyAssociationSpecImmutable(IIdentifier name, Type ownerType, Type returnType, Type defaultElementType)
        : base(name, returnType) {
        this.ownerType = ownerType is null ? null : SerializationFactory.Wrap(ownerType);
        this.defaultElementType = defaultElementType is null ? null : SerializationFactory.Wrap(defaultElementType);
    }

    public string[] ContributedActionNames { get; private set; } = Array.Empty<string>();

    public override string ToString() => $"OneToManyAssociation [name=\"{Identifier}\",Type={ReturnType} ]";

    #region IOneToManyAssociationSpecImmutable Members

    /// <summary>
    ///     Return the <see cref="IObjectSpec" /> for the  Type that the collection holds.
    /// </summary>
    public override IObjectSpecImmutable GetElementSpec(IMetamodel metamodel) {
        var typeOfFacet = GetFacet<IElementTypeFacet>();
        return typeOfFacet != null ? typeOfFacet.GetElementSpec(metamodel) : metamodel.GetSpecification(defaultElementType.Type) as IObjectSpecImmutable;
    }

    public void AddLocalContributedActions(string[] contributedActionNames) => ContributedActionNames = contributedActionNames;

    public override Type OwnerType => ownerType?.Type;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.