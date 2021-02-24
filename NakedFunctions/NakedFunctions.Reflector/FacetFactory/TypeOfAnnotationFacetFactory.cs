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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class TypeOfAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        public TypeOfAnnotationFacetFactory(IFacetFactoryOrder<TypeOfAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.CollectionsAndActions) { }

        private static IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type methodReturnType, ISpecification holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!(CollectionUtils.IsCollection(methodReturnType) || CollectionUtils.IsQueryable(methodReturnType))) {
                return metamodel;
            }

            if (methodReturnType.IsArray) {
                var elementType = methodReturnType.GetElementType();
                IObjectSpecBuilder elementSpec;
                (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
                FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));
            }
            else if (methodReturnType.IsGenericType) {
                var actualTypeArguments = methodReturnType.GetGenericArguments();
                if (actualTypeArguments.Any()) {
                    var elementType = actualTypeArguments.First();
                    IObjectSpecBuilder elementSpec;
                    (elementSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(elementType, metamodel);
                    FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
                    FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(holder));
                }
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
            Process(reflector, method.ReturnType, specification, metamodel);

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) =>
            property.HasPublicGetter()
                ? Process(reflector, property.PropertyType, specification, metamodel)
                : metamodel;
    }
}