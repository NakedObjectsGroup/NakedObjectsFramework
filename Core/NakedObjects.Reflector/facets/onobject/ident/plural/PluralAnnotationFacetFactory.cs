// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural {
    public class PluralAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PluralAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<PluralAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IPluralFacet Create(PluralAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new PluralFacetAnnotation(attribute.Value, holder);
        }
    }
}