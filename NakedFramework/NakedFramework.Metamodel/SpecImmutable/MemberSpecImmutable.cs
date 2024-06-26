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
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Spec;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class MemberSpecImmutable : Specification, IMemberSpecImmutable {
    protected MemberSpecImmutable(IIdentifier identifier) : base(identifier) { }

    public abstract IObjectSpecImmutable GetElementSpec(IMetamodel metamodel);

    #region IMemberSpecImmutable Members

    public abstract IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel);

    public string Name(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetFacet<IMemberNamedFacet>().FriendlyName(nakedObjectAdapter, framework);

    public string Description(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => GetFacet<IDescribedAsFacet>().Description(nakedObjectAdapter, framework);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.