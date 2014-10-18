// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Metamodel.SpecImmutable {
    public class ActionParameterSpecImmutable : Specification, IActionParameterSpecImmutable {
        private readonly IObjectSpecImmutable specification;

        public ActionParameterSpecImmutable(IObjectSpecImmutable specification) {
            this.specification = specification;
        }

        #region INakedObjectActionParamPeer Members

        public IObjectSpecImmutable Specification {
            get { return specification; }
        }

        public override IIdentifier Identifier {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}