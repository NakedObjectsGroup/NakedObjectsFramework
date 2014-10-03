// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class UIntValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<uint> {
        public UIntValueTypeFacetFactory(INakedObjectReflector reflector)
            :base(reflector, typeof (IUnsignedIntegerValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (UIntValueSemanticsProvider.IsAdaptedType(type)) {
                var spec = Reflector.LoadSpecification(UIntValueSemanticsProvider.AdaptedType);
                AddFacets(new UIntValueSemanticsProvider(spec, holder));
                return true;
            }
            return false;
        }
    }
}