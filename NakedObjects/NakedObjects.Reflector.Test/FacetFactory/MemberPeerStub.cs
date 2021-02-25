// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Reflect;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Spec;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    internal class MemberPeerStub : Specification, IMemberSpecImmutable {
        public MemberPeerStub(string name)
            : this(name, null) { }

        public MemberPeerStub(string name, string description) {
            Name = name;
            Description = description;
        }

        public MemberPeerStub Spec => this;

        public IList<MemberPeerStub> Set => null;

        public string GroupFullName => "";

        public string GetHelp()
        {
            return null;
        }

        public IConsent IsUsableDeclaratively()
        {
            return Allow.Default;
        }

        public IConsent IsUsableForSession(ISession session)
        {
            return Allow.Default;
        }

        public IConsent IsUsable(INakedObjectAdapter target)
        {
            return null;
        }

        public bool IsVisibleDeclaratively()
        {
            return false;
        }

        public bool IsVisibleForSession(ISession session)
        {
            return false;
        }

        public bool IsVisible(INakedObjectAdapter target)
        {
            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        #region Nested type: IdentifierNull

        #region Nested Type: IdentifierNull

        private class IdentifierNull : IdentifierImpl {
            private readonly MemberPeerStub owner;

            public IdentifierNull(MemberPeerStub owner)
                : base("", "") =>
                this.owner = owner;

            public override string ToString()
            {
                return owner.Name;
            }
        }

        #endregion

        #endregion

        #region IMemberSpecImmutable Members

        public override IIdentifier Identifier => new IdentifierNull(this);

        public string Name { get; }

        public string Description { get; }

        #endregion

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public IObjectSpecImmutable ReturnSpec { get; private set; }
        public IObjectSpecImmutable ElementSpec { get; private set; }

        public IObjectSpecImmutable OwnerSpec { get; private set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }
}