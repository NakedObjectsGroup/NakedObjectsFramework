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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Spec;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class MemberSpecImmutable : Specification, IMemberSpecImmutable {
    private readonly IIdentifier identifier;

    protected MemberSpecImmutable(IIdentifier identifier) => this.identifier = identifier;

    public abstract IObjectSpecImmutable GetElementSpec(IMetamodel metamodel);

    #region IMemberSpecImmutable Members

    public override IIdentifier Identifier => identifier;

    public abstract IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel);

    public string Name(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetFacet<IMemberNamedFacet>().FriendlyName(nakedObjectAdapter, framework);

    public string Description(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetFacet<IDescribedAsFacet>().Description(nakedObjectAdapter, framework);

    #endregion

    #region ISerializable

    // The special constructor is used to deserialize values. 
    protected MemberSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) => identifier = info.GetValue<IIdentifier>("identifier");

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue<IIdentifier>("identifier", identifier);
        base.GetObjectData(info, context);
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.