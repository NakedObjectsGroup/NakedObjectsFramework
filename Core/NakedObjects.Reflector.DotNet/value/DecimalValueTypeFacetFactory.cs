// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class DecimalValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<decimal> {
        public DecimalValueTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, typeof(IDecimalValueFacet)) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (DecimalValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new DecimalValueSemanticsProvider(Reflector, holder));
                return true;
            }
            return false;
        }
    }
}