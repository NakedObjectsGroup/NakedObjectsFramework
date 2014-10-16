// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.NotPersistable {
    public class ProgramPersistableOnlyAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ProgramPersistableOnlyAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<ProgramPersistableOnlyAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IProgramPersistableOnlyFacet Create(ProgramPersistableOnlyAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new ProgramPersistableOnlyFacetAnnotation(holder);
        }
    }
}