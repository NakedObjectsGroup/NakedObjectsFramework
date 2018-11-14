// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
    public sealed class CollectionFieldMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ModifyPrefix
        };

        public CollectionFieldMethodsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Collections) { }

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder collection, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            string capitalizedName = property.Name;
            Type type = property.DeclaringType;

            var facets = new List<IFacet> {new PropertyAccessorFacet(property, collection)};

            AddSetFacet(facets, property, collection);

            AddHideForSessionFacetNone(facets, collection);
            AddDisableFacetAlways(facets, collection);
            FindDefaultHideMethod(reflector, facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", collection);
            FindAndRemoveHideMethod(reflector, facets, methodRemover, type, MethodType.Object, capitalizedName, collection);
            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        private static void AddSetFacet(ICollection<IFacet> collectionFacets, PropertyInfo property, ISpecification collection) {
            if (CollectionUtils.IsSet(property.PropertyType)) {
                collectionFacets.Add(new IsASetFacet(collection));
            }
        }

        public bool IsCollectionAccessor(MethodInfo method) {
            Type methodReturnType = method.ReturnType;
            return CollectionUtils.IsCollection(methodReturnType);
        }

        public IList<Type> BuildCollectionTypes(IEnumerable<PropertyInfo> properties) {
            return properties.Where(property => property.GetGetMethod() != null &&
                                                CollectionUtils.IsCollection(property.PropertyType) &&
                                                !CollectionUtils.IsBlobOrClob(property.PropertyType) &&
                                                property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                                                !CollectionUtils.IsQueryable(property.PropertyType)).Select(p => p.PropertyType).ToArray();
        }

        public override IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            IList<Type> collectionTypes = BuildCollectionTypes(candidates);
            candidates = candidates.Where(property => collectionTypes.Contains(property.PropertyType)).ToArray();
            return PropertiesToBeIntrospected(candidates, classStrategy);
        }
    }
}