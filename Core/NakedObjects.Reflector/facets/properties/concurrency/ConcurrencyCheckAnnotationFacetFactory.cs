// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Version;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Version {
    public class ConcurrencyCheckAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ConcurrencyCheckAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.PropertiesOnly) { }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            Attribute attribute = property.GetCustomAttribute<ConcurrencyCheckAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IConcurrencyCheckFacet Create(Attribute attribute, ISpecification holder) {
            return attribute == null ? null : new ConcurrencyCheckFacetAnnotation(holder);
        }
    }
}