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
        public ComplexTypeAnnotationFacetFactory()
            : base(NakedObjectFeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            Attribute ctAttribute = type.GetCustomAttributeByReflection<ComplexTypeAttribute>();
            return FacetUtils.AddFacet(Create(ctAttribute, holder));
        }

        private static IComplexTypeFacet Create(Attribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new ComplexTypeFacetAnnotation(holder);
        }
    }
}