// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Immutable {
    public class ImmutableAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ImmutableAnnotationFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            var attribute = type.GetCustomAttributeByReflection<ImmutableAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IImmutableFacet Create(ImmutableAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new ImmutableFacetAnnotation(attribute.Value, holder);
        }
    }
}