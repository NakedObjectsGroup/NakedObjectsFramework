// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof {
    public class TypeOfAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypeOfAnnotationFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.CollectionsAndActions) { }

        private bool Process(Type methodReturnType, IFacetHolder holder) {
            if (!CollectionUtils.IsCollection(methodReturnType)) {
                return false;
            }

            if (methodReturnType.IsArray) {
                return FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(methodReturnType.GetElementType(), holder, Metadata));
            }

            if (methodReturnType.IsGenericType) {
                Type[] actualTypeArguments = methodReturnType.GetGenericArguments();
                if (actualTypeArguments.Length > 0) {
                    return FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(actualTypeArguments[0], holder, Metadata));
                }
            }
            return false;
        }


        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method.ReturnType, holder);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            if (property.GetGetMethod() != null) {
                return Process(property.PropertyType, holder);
            }
            return false;
        }
    }
}