// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Value {
    public class CharValueTypeFacetFactory : ValueUsingValueSemanticsProviderFacetFactory<char> {
        public CharValueTypeFacetFactory(INakedObjectReflector reflector)
            :base(reflector, typeof(ICharValueFacet)) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (CharValueSemanticsProvider.IsAdaptedType(type)) {
                var spec = Reflector.LoadSpecification(CharValueSemanticsProvider.AdaptedType);
                AddFacets(new CharValueSemanticsProvider(spec, holder));
                return true;
            }
            return false;
        }
    }
}