// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Reflect.Actions {
    public class DotNetNakedObjectActionParamPeer : FacetHolderImpl, INakedObjectActionParamPeer {
        private readonly INakedObjectSpecification specification;

        public DotNetNakedObjectActionParamPeer(INakedObjectSpecification specification) {
            this.specification = specification;
        }

        #region INakedObjectActionParamPeer Members

        public INakedObjectSpecification Specification {
            get { return specification; }
        }

        public override IIdentifier Identifier {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}