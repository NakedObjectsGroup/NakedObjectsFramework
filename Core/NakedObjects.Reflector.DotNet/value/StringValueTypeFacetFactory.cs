// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class StringValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<string> {
        public StringValueTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, typeof (IStringValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (StringValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new StringValueSemanticsProvider(Reflector, holder));
                return true;
            }
            return false;
        }
    }
}