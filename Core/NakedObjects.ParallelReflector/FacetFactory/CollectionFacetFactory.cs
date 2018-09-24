// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
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

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class CollectionFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public CollectionFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsInterfacesPropertiesAndCollections) {}

        private void ProcessArray(IReflector reflector, Type type, ISpecification holder, IMetamodelBuilder metamodel) {
            FacetUtils.AddFacet(new ArrayFacet(holder));
            FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));

            var elementType = type.GetElementType();
            reflector.LoadSpecification(elementType, metamodel);
        }

        private ImmutableDictionary<String, ITypeSpecBuilder> ProcessArray(IReflector reflector, Type type, ISpecification holder, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            FacetUtils.AddFacet(new ArrayFacet(holder));
            FacetUtils.AddFacet(new TypeOfFacetInferredFromArray(holder));

            var elementType = type.GetElementType();
            return reflector.LoadSpecification(elementType, metamodel).Item2;
        }

        private void ProcessGenericEnumerable(Type type, ISpecification holder) {
            bool isCollection = CollectionUtils.IsGenericCollection(type); // as opposed to IEnumerable
            bool isQueryable = CollectionUtils.IsGenericQueryable(type);
            bool isSet = CollectionUtils.IsSet(type);

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

        private void ProcessCollection(IReflector reflector, ISpecification holder, IMetamodelBuilder metamodel) {
            Type collectionElementType = typeof (object);
            var spec = reflector.LoadSpecification<IObjectSpecImmutable>(collectionElementType, metamodel);
            FacetUtils.AddFacet(new TypeOfFacetDefaultToType(holder, collectionElementType, spec));
            FacetUtils.AddFacet(new CollectionFacet(holder));
        }

        private ImmutableDictionary<String, ITypeSpecBuilder> ProcessCollection(IReflector reflector, ISpecification holder, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            Type collectionElementType = typeof(object);
            var result = reflector.LoadSpecification(collectionElementType, metamodel);
            metamodel = result.Item2;
            var spec = result.Item1 as IObjectSpecImmutable;
            FacetUtils.AddFacet(new TypeOfFacetDefaultToType(holder, collectionElementType, spec));
            FacetUtils.AddFacet(new CollectionFacet(holder));
            return metamodel;
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            if (CollectionUtils.IsGenericEnumerable(type)) {
                ProcessGenericEnumerable(type, specification);
            }
            else if (type.IsArray) {
                ProcessArray(reflector, type, specification, metamodel);
            }
            else if (CollectionUtils.IsCollectionButNotArray(type)) {
                ProcessCollection(reflector, specification, metamodel);
            }
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            if (CollectionUtils.IsCollectionButNotArray(property.PropertyType)) {
                specification.AddFacet(new CollectionResetFacet(property, specification));
            }
            else {
                base.Process(reflector, property, methodRemover, specification, metamodel);
            }
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            if (CollectionUtils.IsGenericEnumerable(type)) {
                ProcessGenericEnumerable(type, specification);
                return metamodel;
            }
            else if (type.IsArray) {
                return ProcessArray(reflector, type, specification, metamodel);
            }
            else if (CollectionUtils.IsCollectionButNotArray(type)) {
                return ProcessCollection(reflector, specification, metamodel);
            }

            return metamodel;
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            if (CollectionUtils.IsCollectionButNotArray(property.PropertyType)) {
                specification.AddFacet(new CollectionResetFacet(property, specification));
                return metamodel;
            }
            else {
                return base.Process(reflector, property, methodRemover, specification, metamodel);
            }
        }
    }
}