// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Metamodel.Facet;


namespace NakedObjects.Reflector.FacetFactory {
    public class CollectionFieldMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {
        private static readonly ILog Log;
        private static readonly string[] FixedPrefixes;

        static CollectionFieldMethodsFacetFactory() {
            Log = LogManager.GetLogger(typeof (CollectionFieldMethodsFacetFactory));

            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.ClearPrefix,
                PrefixesAndRecognisedMethods.ModifyPrefix
            };
        }

        public CollectionFieldMethodsFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.CollectionsOnly) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification collection) {
            string capitalizedName = property.Name;
            Type type = property.DeclaringType;


            var facets = new List<IFacet> {new PropertyAccessorFacet(property, collection)};

            AddSetFacet(facets, property, collection);


            AddHideForSessionFacetNone(facets, collection);
            AddDisableFacetAlways(facets, collection);
            FindDefaultHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], collection);
            FindAndRemoveHideMethod(facets, methodRemover, type, MethodType.Object, capitalizedName, collection);
            return FacetUtils.AddFacets(facets);
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

        private static IList<Type> BuildCollectionTypes(IEnumerable<PropertyInfo> properties) {
            IList<Type> types = new List<Type>();

            foreach (PropertyInfo property in properties) {
                if (property.GetGetMethod() != null &&
                    CollectionUtils.IsCollection(property.PropertyType) &&
                    !CollectionUtils.IsBlobOrClob(property.PropertyType) &&
                    property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                    !CollectionUtils.IsQueryable(property.PropertyType)) {
                    types.Add(property.PropertyType);
                }
            }
            return types;
        }

        public override void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            var propertiesToRemove = new List<PropertyInfo>();
            IList<Type> collectionTypes = BuildCollectionTypes(candidates);
            foreach (PropertyInfo property in candidates) {
                foreach (Type returnType in collectionTypes) {
                    if (property.GetGetMethod() != null && property.PropertyType == returnType) {
                        propertiesToRemove.Add(property);
                        methodListToAppendTo.Add(property);
                        break;
                    }
                }
            }

            foreach (PropertyInfo property in propertiesToRemove) {
                candidates.Remove(property);
            }
        }
    }
}