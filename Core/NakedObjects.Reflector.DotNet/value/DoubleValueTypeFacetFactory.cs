// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Value {
    public class DoubleValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<double> {
        public DoubleValueTypeFacetFactory()
            : base(typeof (IDoubleFloatingPointValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (DoubleValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new DoubleValueSemanticsProvider(holder));
                return true;
            }
            return false;
        }
    }
}