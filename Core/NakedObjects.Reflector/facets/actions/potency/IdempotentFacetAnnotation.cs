// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Potency;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Potency {
    public class IdempotentFacetAnnotation : IdempotentFacetImpl {
        public IdempotentFacetAnnotation(ISpecification holder)
            : base(holder) {}
    }
}