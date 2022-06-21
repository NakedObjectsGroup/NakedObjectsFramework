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
public abstract class AssociationSpecImmutable : MemberSpecImmutable, IAssociationSpecImmutable {
    private TypeSerializationWrapper returnType;

    protected AssociationSpecImmutable(IIdentifier identifier, Type returnType)
        : base(identifier) =>
        this.returnType = returnType is null ? null : SerializationFactory.Wrap(returnType);

    protected Type ReturnType => returnType?.Type;

    #region IAssociationSpecImmutable Members

    public abstract Type OwnerType { get; }

    public override IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel) => metamodel.GetSpecification(ReturnType) as IObjectSpecImmutable;

    public virtual IObjectSpecImmutable GetOwnerSpec(IMetamodel metamodel) => metamodel.GetSpecification(OwnerType) as IObjectSpecImmutable;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.