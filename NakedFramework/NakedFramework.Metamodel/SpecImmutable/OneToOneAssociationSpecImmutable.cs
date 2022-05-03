// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class OneToOneAssociationSpecImmutable : AssociationSpecImmutable, IOneToOneAssociationSpecImmutable {
    public OneToOneAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec)
        : base(identifier, returnSpec) =>
        OwnerSpec = ownerSpec;

    

    public override IObjectSpecImmutable GetElementSpec(IMetamodel metamodel) => null;

    #region IOneToOneAssociationSpecImmutable Members

    public override IObjectSpecImmutable OwnerSpec { get; }

    #endregion

    public override string ToString() => $"Reference Association [name=\"{Identifier}\", Type={ReturnSpec} ]";
}

// Copyright (c) Naked Objects Group Ltd.