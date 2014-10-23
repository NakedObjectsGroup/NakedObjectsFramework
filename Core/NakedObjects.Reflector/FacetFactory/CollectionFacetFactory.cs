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
using NakedObjects.Metamodel.Facet;
using NakedObjects.Metamodel.Utils;




namespace NakedObjects.Reflector.FacetFactory {
    public class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsPropertiesAndCollections) {}

        private bool ProcessArray(Type type, ISpecification holder) {
            holder.AddFacet(new ArrayFacet(holder));

            var elementType = type.GetElementType();
            var elementSpec = Reflector.LoadSpecification(elementType);
            holder.AddFacet(new TypeOfFacetInferredFromArray(holder, Reflector.Metamodel));
            holder.AddFacet(new ElementTypeFacet(holder, elementType, elementSpec));
            return true;
        }

        private bool ProcessGenericEnumerable(Type type, ISpecification holder) {
            var elementTypeFacet = holder.GetFacet<IElementTypeFacet>();
            bool isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable 
            bool isQueryable = CollectionUtils.IsGenericQueryable(type);
            bool isSet = CollectionUtils.IsSet(type);
            //Type collectionElementType;
            if (elementTypeFacet != null) {
                //collectionElementType = elementTypeFacet.Value;
            }
            else {
                //collectionElementType = CollectionUtils.ElementType(type);
                //var collectionElementSpec = Reflector.LoadSpecification(collectionElementType);
                holder.AddFacet(new TypeOfFacetInferredFromGenerics(holder, Reflector.Metamodel));
                //holder.AddFacet(new ElementTypeFacet(holder, collectionElementType, collectionElementSpec));
            }

            Type facetType = isQueryable ? typeof(GenericIQueryableFacet<>) : (isCollection ? typeof(GenericCollectionFacet<>) : typeof(GenericIEnumerableFacet<>));

            //Type genericFacet = facetType.GetGenericTypeDefinition();
            //var facet = (IFacet)Activator.CreateInstance(holder, isSet);
            //holder.AddFacet(facet);
            return true;
        }


        private bool ProcessCollection(ISpecification holder) {
            var elementTypeFacet = holder.GetFacet<IElementTypeFacet>();
            if (elementTypeFacet == null) {
                Type collectionElementType = typeof (object);
                var spec = Reflector.LoadSpecification(collectionElementType);
                holder.AddFacet(new TypeOfFacetDefaultToObject(holder, Reflector.Metamodel));
                holder.AddFacet(new ElementTypeFacet(holder, collectionElementType, spec));
            }
            holder.AddFacet(new CollectionFacet(holder));
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