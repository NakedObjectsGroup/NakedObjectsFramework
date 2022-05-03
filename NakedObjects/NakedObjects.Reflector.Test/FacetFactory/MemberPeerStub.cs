// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Reflect;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Spec;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory;

internal class MemberPeerStub : Specification, IMemberSpecImmutable {
    private readonly string description;
    private readonly string name;

    public MemberPeerStub(string name)
        : this(name, null) { }

    public MemberPeerStub(string name, string description) :base(null) {
        this.name = name;
        this.description = description;
    }

    public MemberPeerStub Spec => this;

    public IList<MemberPeerStub> Set => null;

    public string GroupFullName => "";

    public string GetHelp() => null;

    public IConsent IsUsableDeclaratively() => Allow.Default;

    public IConsent IsUsableForSession(ISession session) => Allow.Default;

    public IConsent IsUsable(INakedObjectAdapter target) => null;

    public bool IsVisibleDeclaratively() => false;

    public bool IsVisibleForSession(ISession session) => false;

    public bool IsVisible(INakedObjectAdapter target) => false;

    public override string ToString() => name;

    #region Nested type: IdentifierNull

    #region Nested Type: IdentifierNull

    private class IdentifierNull : IdentifierImpl {
        private readonly MemberPeerStub owner;

        public IdentifierNull(MemberPeerStub owner)
            : base("", "") =>
            this.owner = owner;

        public override string ToString() => owner.Name(null, null);
    }

    #endregion

    #endregion

    #region IMemberSpecImmutable Members

    public override IIdentifier Identifier => new IdentifierNull(this);

    public string Name(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => name;

    public string Description(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => description;

    #endregion

    // ReSharper disable UnusedAutoPropertyAccessor.Local

    public IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel) => null;

    public IObjectSpecImmutable ElementSpec { get; private set; }

    public IObjectSpecImmutable OwnerSpec { get; private set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Local
}