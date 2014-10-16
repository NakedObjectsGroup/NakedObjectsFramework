// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations.Schema;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Aggregated {
    public class ComplexTypeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ComplexTypeAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            Attribute ctAttribute = type.GetCustomAttributeByReflection<ComplexTypeAttribute>();
            return FacetUtils.AddFacet(Create(ctAttribute, specification));
        }

        private static IComplexTypeFacet Create(Attribute attribute, ISpecification holder) {
            return attribute == null ? null : new ComplexTypeFacetAnnotation(holder);
        }
    }
}