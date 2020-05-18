// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsInterfacesPropertiesAndCollections) { }

        private static void ProcessArray(IReflector reflector, Type type, ISpecification holder) {
            FacetUtils.AddFacet(new ArrayFacet(holder));
            FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));

            var elementType = type.GetElementType();
            reflector.LoadSpecification(elementType);
        }

        private static void ProcessGenericEnumerable(Type type, ISpecification holder) {
            var isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable 
            var isQueryable = CollectionUtils.IsGenericQueryable(type);
            var isSet = CollectionUtils.IsSet(type);

            FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(holder));

            IFacet facet;
            if (isQueryable) {
                facet = new GenericIQueryableFacet(holder, isSet);
            }
            else if (isCollection) {
                facet = new GenericCollectionFacet(holder, isSet);
            }
            else {
                facet = new GenericIEnumerableFacet(holder, isSet);
            }

            FacetUtils.AddFacet(facet);
        }

        private static void ProcessCollection(IReflector reflector, ISpecification holder) {
            var collectionElementType = typeof(object);
            var spec = reflector.LoadSpecification<IObjectSpecImmutable>(collectionElementType);
            FacetUtils.AddFacet(new TypeOfFacetDefaultToType(holder, collectionElementType, spec));
            FacetUtils.AddFacet(new CollectionFacet(holder));
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (CollectionUtils.IsGenericEnumerable(type)) {
                ProcessGenericEnumerable(type, specification);
            }
            else if (type.IsArray) {
                ProcessArray(reflector, type, specification);
            }
            else if (CollectionUtils.IsCollectionButNotArray(type)) {
                ProcessCollection(reflector, specification);
            }
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            if (CollectionUtils.IsCollectionButNotArray(property.PropertyType)) {
                specification.AddFacet(new CollectionResetFacet(property, specification));
            }
            else {
                base.Process(reflector, property, methodRemover, specification);
            }
        }
    }
}