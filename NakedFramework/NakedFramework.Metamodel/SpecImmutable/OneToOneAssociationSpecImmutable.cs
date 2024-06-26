// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class OneToOneAssociationSpecImmutable : AssociationSpecImmutable, IOneToOneAssociationSpecImmutable {
    private readonly TypeSerializationWrapper typeWrapper;

    public OneToOneAssociationSpecImmutable(IIdentifier identifier, Type ownerType, Type returnType)
        : base(identifier, returnType) =>
        typeWrapper = OwnerType is null ? null : SerializationFactory.Wrap(ownerType);

    #region IOneToOneAssociationSpecImmutable Members

    public override Type OwnerType => typeWrapper?.Type;

    #endregion

    public override IObjectSpecImmutable GetOwnerSpec(IMetamodel metamodel) => metamodel.GetSpecification(OwnerType) as IObjectSpecImmutable;

    public override IObjectSpecImmutable GetElementSpec(IMetamodel metamodel) => null;

    public override string ToString() => $"Reference Association [name=\"{Identifier}\", Type={ReturnType} ]";
}

// Copyright (c) Naked Objects Group Ltd.