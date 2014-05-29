// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Value {
    public class LongValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<long> {
        public LongValueTypeFacetFactory()
            : base(typeof (ILongValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (LongValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new LongValueSemanticsProvider(holder));
                return true;
            }
            return false;
        }
    }
}