// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public class FacetsAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public FacetsAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<FacetsAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        /// <summary>
        ///     Returns a <see cref="IFacetsFacet" /> impl provided that at least one valid
        ///     factory <see cref="IFacetsFacet.FacetFactories" /> was specified.
        /// </summary>
        private static IFacetsFacet Create(FacetsAttribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            var facetsFacetAnnotation = new FacetsFacetAnnotation(attribute, holder);
            return facetsFacetAnnotation.FacetFactories.Length > 0 ? facetsFacetAnnotation : null;
        }
    }
}