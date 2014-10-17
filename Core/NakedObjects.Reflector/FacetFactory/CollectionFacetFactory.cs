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
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.DotNet.Facets.Collections.Modify;

namespace NakedObjects.Reflector.FacetFactory {
    public class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsPropertiesAndCollections) {}

        private bool ProcessArray(Type type, ISpecification holder) {
            holder.AddFacet(new DotNetArrayFacet(holder, type.GetElementType()));

            var elementType = type.GetElementType();
            var elementSpec = Reflector.LoadSpecification(elementType);
            holder.AddFacet(new TypeOfFacetInferredFromArray(elementType, holder, elementSpec));
            return true;
        }

        private bool ProcessGenericEnumerable(Type type, ISpecification holder) {
            var typeOfFacet = holder.GetFacet<ITypeOfFacet>();
            bool isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable 
            bool isQueryable = CollectionUtils.IsGenericQueryable(type);
            bool isSet = CollectionUtils.IsSet(type);
            Type collectionElementType;
            if (typeOfFacet != null) {
                collectionElementType = typeOfFacet.Value;
            }
            else {
                collectionElementType = CollectionUtils.ElementType(type);
                var collectionElementSpec = Reflector.LoadSpecification(collectionElementType);
                holder.AddFacet(new TypeOfFacetInferredFromGenerics(collectionElementType, holder, collectionElementSpec));
            }

            Type facetType = isQueryable ? typeof (DotNetGenericIQueryableFacet<>) : (isCollection ? typeof (DotNetGenericCollectionFacet<>) : typeof (DotNetGenericIEnumerableFacet<>));

            Type genericFacet = facetType.GetGenericTypeDefinition();
            Type genericCollectionFacetType = genericFacet.MakeGenericType(CollectionUtils.ElementType(type));
            var facet = (IFacet) Activator.CreateInstance(genericCollectionFacetType, holder, collectionElementType, isSet);
            holder.AddFacet(facet);

            return true;
        }


        private bool ProcessCollection(ISpecification holder) {
            var typeOfFacet = holder.GetFacet<ITypeOfFacet>();
            Type collectionElementType;
            if (typeOfFacet != null) {
                collectionElementType = typeOfFacet.Value;
            }
            else {
                collectionElementType = typeof (object);
                var spec = Reflector.LoadSpecification(collectionElementType);
                holder.AddFacet(new TypeOfFacetDefaultToObject(holder, collectionElementType, spec));
            }
            holder.AddFacet(new DotNetCollectionFacet(holder, collectionElementType));
            return true;
        }


        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            if (CollectionUtils.IsGenericEnumerable(type)) {
                return ProcessGenericEnumerable(type, specification);
            }
            if (type.IsArray) {
                return ProcessArray(type, specification);
            }
            if (CollectionUtils.IsCollectionButNotArray(type)) {
                return ProcessCollection(specification);
            }

            return false;
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if (CollectionUtils.IsCollectionButNotArray(property.PropertyType)) {
                specification.AddFacet(new CollectionResetFacet(property, specification));
                return true;
            }
            return base.Process(property, methodRemover, specification);
        }
    }
}