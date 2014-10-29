// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Metamodel.Adapter;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Spec;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    internal class MemberPeerStub : NamedAndDescribedSpecification, IMemberSpecImmutable, IOrderableElement<MemberPeerStub> {
        private readonly ILifecycleManager persistor;

        public MemberPeerStub(string name, ILifecycleManager persistor)
            : base(name) {
            this.persistor = persistor;
        }

        #region IMemberSpecImmutable Members

        public override IIdentifier Identifier {
            get { return new IdentifierNull(this, persistor); }
        }

        public IObjectSpecImmutable Specification { get; private set; }

        #endregion

        #region IOrderableElement<MemberPeerStub> Members

        public MemberPeerStub Spec {
            get { return this; }
        }

        public IList<IOrderableElement<MemberPeerStub>> Set { get { return null; } }
        public string GroupFullName { get { return ""; } }

        #endregion

        public string GetHelp() {
            return null;
        }

        public IConsent IsUsableDeclaratively() {
            return Allow.Default;
        }

        public IConsent IsUsableForSession(ISession session) {
            return Allow.Default;
        }

        public IConsent IsUsable(INakedObject target) {
            return null;
        }

        public bool IsVisibleDeclaratively() {
            return false;
        }

        public bool IsVisibleForSession(ISession session) {
            return false;
        }

        public bool IsVisible(INakedObject target) {
            return false;
        }

        public override string ToString() {
            return GetName();
        }

        #region Nested Type: IdentifierNull

        private class IdentifierNull : IdentifierImpl {
            private readonly MemberPeerStub owner;
            private readonly ILifecycleManager persistor;

            public IdentifierNull(MemberPeerStub owner, ILifecycleManager persistor)
                : base(null, "", "") {
                this.owner = owner;
                this.persistor = persistor;
            }

            public override string ToString() {
                return owner.GetName();
            }
        };

        #endregion
    }
}