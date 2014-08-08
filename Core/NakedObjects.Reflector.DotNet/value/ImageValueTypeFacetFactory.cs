// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public class ImageValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<Image> {
        public ImageValueTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, typeof(IImageValueFacet)) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (ImageValueSemanticsProvider.IsAdaptedType(type)) {
                AddFacets(new ImageValueSemanticsProvider(Reflector, holder));
                return true;
            }
            return false;
        }
    }
}