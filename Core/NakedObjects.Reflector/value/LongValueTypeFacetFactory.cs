// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class LongValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<long> {
        public LongValueTypeFacetFactory(INakedObjectReflector reflector)
            :base(reflector, typeof (ILongValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            if (LongValueSemanticsProvider.IsAdaptedType(type)) {
                var spec = Reflector.LoadSpecification(LongValueSemanticsProvider.AdaptedType);
                AddFacets(new LongValueSemanticsProvider(spec, specification));
                return true;
            }
            return false;
        }
    }
}