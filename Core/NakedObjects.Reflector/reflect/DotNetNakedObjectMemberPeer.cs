// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public abstract class DotNetNakedObjectMemberPeer : FacetHolderImpl, INakedObjectMemberPeer {
        private readonly IIdentifier identifier;

        protected DotNetNakedObjectMemberPeer(IIdentifier identifier) {
            this.identifier = identifier;
        }

        #region INakedObjectMemberPeer Members

        public override IIdentifier Identifier {
            get { return identifier; }
        }

        #endregion

    }

    // Copyright (c) Naked Objects Group Ltd.
}