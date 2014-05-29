// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public class ValueFacetUsingSemanticsProvider<T> : ValueFacetAbstract<T> {
        public ValueFacetUsingSemanticsProvider(IValueSemanticsProvider<T> adapter, IFacet underlyingValueTypeFacet)
            : base(adapter, true, underlyingValueTypeFacet.FacetHolder) {
            // add the adapter in as its own facet (eg StringFacet).
            // This facet is almost certainly superfluous; there is nothing in the
            // viewers that needs to get hold of such a facet, for example.
            FacetUtils.AddFacet(underlyingValueTypeFacet);
        }
    }
}