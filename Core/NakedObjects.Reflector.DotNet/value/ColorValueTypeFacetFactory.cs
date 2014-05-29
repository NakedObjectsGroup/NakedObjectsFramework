// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Drawing;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ColorValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<Color> {
        public ColorValueTypeFacetFactory()
            : base(typeof (IColorValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (ColorValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new ColorValueSemanticsProvider(holder));
                return true;
            }
            return false;
        }
    }
}