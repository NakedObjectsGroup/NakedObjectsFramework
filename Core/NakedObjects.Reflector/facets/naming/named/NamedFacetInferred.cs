// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.Named;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.Named {
    public class NamedFacetInferred : NamedFacetImpl {
        public NamedFacetInferred(string value, ISpecification holder)
            : base(value, holder) {}


        public override bool CanAlwaysReplace {
            get { return false; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}