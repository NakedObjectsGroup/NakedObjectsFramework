// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Metamodel.SpecImmutable {
    public abstract class MemberSpecImmutable : Specification, IMemberSpecImmutable {
        private readonly IIdentifier identifier;

        protected MemberSpecImmutable(IIdentifier identifier) {
            this.identifier = identifier;
        }

        #region INakedObjectMemberPeer Members

        public override IIdentifier Identifier {
            get { return identifier; }
        }

        public abstract IObjectSpecImmutable Specification { get; }

        #endregion

    }

    // Copyright (c) Naked Objects Group Ltd.
}