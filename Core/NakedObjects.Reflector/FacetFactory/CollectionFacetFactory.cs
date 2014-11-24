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
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory()
            : base(FeatureType.ObjectsPropertiesAndCollections) {}

        private void ProcessArray(IReflector reflector, Type type, ISpecification holder) {
            FacetUtils.AddFacet(new ArrayFacet(holder));

            var elementType = type.GetElementType();
            var elementSpec = reflector.LoadSpecification(elementType);
            FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));
            FacetUtils.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
        }

        private void ProcessGenericEnumerable(Type type, ISpecification holder) {
            var elementTypeFacet = holder.GetFacet<IElementTypeFacet>();
            bool isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable 
            bool isQueryable = CollectionUtils.IsGenericQueryable(type);
            bool isSet = CollectionUtils.IsSet(type);

            if (elementTypeFacet == null) {
                FacetUtils.AddFacet(new TypeOfFacetInferredFromGenerics(holder));
            }

            Type facetType = isQueryable ? typeof (GenericIQueryableFacet) : (isCollection ? typeof (GenericCollectionFacet) : typeof (GenericIEnumerableFacet));
            FacetUtils.AddFacet((IFacet) Activator.CreateInstance(facetType, holder, isSet));
        }


        private void ProcessCollection(IReflector reflector, ISpecification holder) {
            var elementTypeFacet = holder.GetFacet<IElementTypeFacet>();
            if (elementTypeFacet == null) {
                Type collectionElementType = typeof (object);
                var spec = reflector.LoadSpecification(collectionElementType);
                FacetUtils.AddFacet(new TypeOfFacetDefaultToType(holder, collectionElementType, spec));
                FacetUtils.AddFacet(new ElementTypeFacet(holder, collectionElementType, spec));
            }
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