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
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class AssociationSpecImmutable : MemberSpecImmutable, IAssociationSpecImmutable {
    protected IObjectSpecImmutable ReturnSpec { get; }

    protected AssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable returnSpec)
        : base(identifier) =>
        this.ReturnSpec = returnSpec;

    #region IAssociationSpecImmutable Members

    public abstract IObjectSpecImmutable OwnerSpec { get; }

    public override IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel) => ReturnSpec;

    #endregion

    #region ISerializable

    // The special constructor is used to deserialize values. 
    protected AssociationSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) => ReturnSpec = info.GetValue<IObjectSpecImmutable>("returnSpec");

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue<IObjectSpecImmutable>("returnSpec", ReturnSpec);
        base.GetObjectData(info, context);
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.