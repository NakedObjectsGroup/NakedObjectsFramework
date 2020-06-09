// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class TypeOfAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypeOfAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.CollectionsAndActions) { }

        private static void Process(IReflector reflector, Type methodReturnType, ISpecification holder) {
            if (!CollectionUtils.IsCollection(methodReturnType)) {
                return;
            }

            if (methodReturnType.IsArray) {
                var elementType = methodReturnType.GetElementType();
                var elementSpec = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
                FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));
            }
            else if (methodReturnType.IsGenericType) {
                var actualTypeArguments = methodReturnType.GetGenericArguments();
                if (actualTypeArguments.Any()) {
                    var elementType = actualTypeArguments.First();
                    var elementSpec = reflector.LoadSpecification<IObjectSpecImmutable>(elementType);
                    FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                    FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(holder));
                }
            }
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) => Process(reflector, method.ReturnType, specification);

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (property.GetGetMethod() != null) {
                Process(reflector, property.PropertyType, specification);
            }
        }
    }
}