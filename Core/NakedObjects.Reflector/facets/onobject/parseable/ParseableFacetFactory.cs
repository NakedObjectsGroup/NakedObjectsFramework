// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Parseable {
    public class ParseableFacetFactory : AnnotationBasedFacetFactoryAbstract, INakedObjectConfigurationAware {
        public ParseableFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsOnly) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(type, specification));
        }

        private static IFacet Create(Type type, ISpecification holder) {
            var annotation = type.GetCustomAttributeByReflection<ParseableAttribute>();

            // create from annotation, if present
            if (annotation != null) {
                var facet = TypeUtils.CreateGenericInstance<IParseableFacet>(typeof (ParseableFacetAnnotation<>),
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