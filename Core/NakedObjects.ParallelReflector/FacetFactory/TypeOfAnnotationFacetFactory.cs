// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class TypeOfAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypeOfAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.CollectionsAndActions) {}

        private IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type methodReturnType, ISpecification holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!CollectionUtils.IsCollection(methodReturnType)) {
                return metamodel;
            }

            if (methodReturnType.IsArray) {
                Type elementType = methodReturnType.GetElementType();
                var result = reflector.LoadSpecification(elementType, metamodel);
                metamodel = result.Item2;
                var elementSpec = result.Item1 as IObjectSpecImmutable;
                FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));
            }
            else if (methodReturnType.IsGenericType) {
                Type[] actualTypeArguments = methodReturnType.GetGenericArguments();
                if (actualTypeArguments.Any()) {
                    Type elementType = actualTypeArguments.First();
                    var result = reflector.LoadSpecification(elementType, metamodel);

                    metamodel = result.Item2;
                    var elementSpec = result.Item1 as IObjectSpecImmutable;
                    FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                    FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(holder));
                }
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            return Process(reflector, method.ReturnType, specification, metamodel);
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (property.GetGetMethod() != null) {
                return Process(reflector, property.PropertyType, specification, metamodel);
            }

            return metamodel;
        }
    }
}