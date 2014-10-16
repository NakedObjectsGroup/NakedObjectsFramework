// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public class EncodeableFacetFactory : AnnotationBasedFacetFactoryAbstract, INakedObjectConfigurationAware {
        public EncodeableFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(type, specification));
        }

        /// <summary>
        ///     Returns a <see cref="IEncodeableFacet" /> implementation.
        /// </summary>
        private static IEncodeableFacet Create(Type type, ISpecification holder) {
            // create from annotation, if present
            var annotation = type.GetCustomAttributeByReflection<EncodeableAttribute>();
            if (annotation != null) {
                var facet = TypeUtils.CreateGenericInstance<IEncodeableFacet>(typeof (EncodeableFacetAnnotation<>),
                                                                              new[] {type},
                                                                              new object[] {type, holder});


                if (facet.IsValid) {
                    return facet;
                }
            }
            return null;
        }
    }
}