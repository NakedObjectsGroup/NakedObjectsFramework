// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    internal class MemberPeerStub : NamedAndDescribedFacetHolderImpl, INakedObjectMemberPeer {
        public MemberPeerStub(string name)
            : base(name) {}

        #region INakedObjectMemberPeer Members

        public override IIdentifier Identifier {
            get { return new IdentifierNull(this); }
        }

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
            return Name;
        }

        #region Nested Type: IdentifierNull

        private class IdentifierNull : IdentifierImpl {
            private readonly MemberPeerStub owner;

            public IdentifierNull(MemberPeerStub owner)
                : base(null, "", "") {
                this.owner = owner;
            }

            public override string ToString() {
                return owner.Name;
            }
        };

        #endregion
    }
}