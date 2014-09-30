// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Version;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Version {
    public class ConcurrencyCheckFacetAnnotation : ConcurrencyCheckFacetAbstract {
        public ConcurrencyCheckFacetAnnotation(IFacetHolder holder)
            : base(holder) {}
    }
}