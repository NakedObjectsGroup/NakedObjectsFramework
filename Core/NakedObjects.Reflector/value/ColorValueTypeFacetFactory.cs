// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Drawing;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ColorValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<Color> {
        public ColorValueTypeFacetFactory(INakedObjectReflector reflector)
            :base(reflector, typeof (IColorValueFacet)) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (ColorValueSemanticsProvider.IsAdaptedType(type)) {
                var spec = Reflector.LoadSpecification(ColorValueSemanticsProvider.AdaptedType);
                AddFacets(new ColorValueSemanticsProvider(spec, holder));
                return true;
            }
            return false;
        }
    }
}