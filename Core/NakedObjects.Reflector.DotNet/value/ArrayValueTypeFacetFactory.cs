// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ArrayValueTypeFacetFactory<T> : ValueUsingValueSemanticsProviderFacetFactory<T[]> {
        public ArrayValueTypeFacetFactory()
            : base(typeof (IArrayValueFacet<T>)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (ArrayValueSemanticsProvider<T>.IsAdaptedType(type)) {
                AddFacets(new ArrayValueSemanticsProvider<T>(holder));
                return true;
            }
            return false;
        }
    }
}