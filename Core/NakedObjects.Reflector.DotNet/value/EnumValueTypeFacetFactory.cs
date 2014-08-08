// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.Value;

namespace NakedObjects.Reflector.DotNet.Value {
    public class EnumValueTypeFacetFactory : FacetFactoryAbstract {
        public EnumValueTypeFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            if (typeof (Enum).IsAssignableFrom(type)) {
                Type semanticsProviderType = typeof (EnumValueSemanticsProvider<>).MakeGenericType(type);
                object semanticsProvider = Activator.CreateInstance(semanticsProviderType, holder);
                Type facetType = typeof (ValueFacetUsingSemanticsProvider<>).MakeGenericType(type);
                var facet = (IFacet) Activator.CreateInstance(facetType, semanticsProvider, semanticsProvider);
                FacetUtils.AddFacet(facet);
                return true;
            }
            return false;
        }
    }
}