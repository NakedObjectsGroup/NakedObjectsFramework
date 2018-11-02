// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect {
    public sealed class FacetFactorySet : IFacetFactorySet {
        private readonly IDictionary<FeatureType, IList<IFacetFactory>> factoriesByFeatureType = new Dictionary<FeatureType, IList<IFacetFactory>>();

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see cref="IMethodFilteringFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Filters(MethodInfo, IClassStrategy)" />
        /// </para>
        private readonly IList<IMethodFilteringFacetFactory> methodFilteringFactories;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see cref="IPropertyFilteringFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Filters(PropertyInfo, IClassStrategy)" />
        /// </para>
        private readonly IList<IPropertyFilteringFacetFactory> propertyFilteringFactories;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see
        ///         cref="IPropertyOrCollectionIdentifyingFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Recognizes" />
        /// </para>
        private readonly IList<IPropertyOrCollectionIdentifyingFacetFactory> propertyOrCollectionIdentifyingFactories;

        private readonly List<IMethodIdentifyingFacetFactory> actionIdentifyingFactories;

        public FacetFactorySet(IFacetFactory[] factories) {
            List<IFacetFactory> allFactories = factories.ToList();
            allFactories.Sort();

            Prefixes = allFactories.OfType<IMethodPrefixBasedFacetFactory>().SelectMany(prefixfactory => prefixfactory.Prefixes).ToArray();

            foreach (FeatureType featureType in Enum.GetValues(typeof (FeatureType))) {
                factoriesByFeatureType[featureType] = allFactories.Where(f => f.FeatureTypes.HasFlag(featureType)).ToList();
            }

            methodFilteringFactories = allFactories.OfType<IMethodFilteringFacetFactory>().ToList();
            propertyFilteringFactories = allFactories.OfType<IPropertyFilteringFacetFactory>().ToList();
            propertyOrCollectionIdentifyingFactories = allFactories.OfType<IPropertyOrCollectionIdentifyingFacetFactory>().ToList();
            actionIdentifyingFactories = allFactories.OfType<IMethodIdentifyingFacetFactory>().ToList();

        }

        private string[] Prefixes { get; set; }

        #region IFacetFactorySet Members

        public IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return propertyOrCollectionIdentifyingFactories.SelectMany(fact => fact.FindCollectionProperties(candidates, classStrategy)).ToList();
        }

        public IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return propertyOrCollectionIdentifyingFactories.SelectMany(fact => fact.FindProperties(candidates, classStrategy)).ToList();
        }

        public IList<MethodInfo> FindActions(IList<MethodInfo> candidates, IClassStrategy classStrategy) {
            return actionIdentifyingFactories.SelectMany(fact => fact.FindActions(candidates, classStrategy)).ToList();

        }

        /// <summary>
        ///     Whether this method is recognized (and should be ignored) by
        ///     any of the registered <see cref="IFacetFactory" />
        /// </summary>
        /// <para>
        ///     Checks:
        /// </para>
        /// <list type="bullet">
        ///     <item>
        ///         the method's prefix against the prefixes supplied by any <see cref="IMethodPrefixBasedFacetFactory" />
        ///     </item>
        ///     <item>
        ///         the method against any <see cref="IMethodFilteringFacetFactory" />
        ///     </item>
        /// </list>
        /// <para>
        ///     The design of <see cref="IMethodPrefixBasedFacetFactory" /> (whereby this
        ///     facet factory set does the work) is a slight performance optimization
        ///     for when there are multiple facet factories that search for the
        ///     same prefix.
        /// </para>
        public bool Filters(MethodInfo method, IClassStrategy classStrategy) {
            return methodFilteringFactories.Any(factory => factory.Filters(method, classStrategy));
        }

        public bool Filters(PropertyInfo property, IClassStrategy classStrategy) {
            return propertyFilteringFactories.Any(factory => factory.Filters(property, classStrategy));
        }

        public bool Recognizes(MethodInfo method) {
            return Prefixes.Any(prefix => method.Name.StartsWith(prefix));
        }

        public void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(FeatureType.Objects)) {
                facetFactory.Process(reflector, type, methodRemover, specification);
            }
        }

        public void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, FeatureType featureType) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetFactory.Process(reflector, method, methodRemover, specification);
            }
        }

        public void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, FeatureType featureType) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetFactory.Process(reflector, property, methodRemover, specification);
            }
        }

        public void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder specification) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(FeatureType.ActionParameters)) {
                facetFactory.ProcessParams(reflector, method, paramNum, specification);
            }
        }

        #endregion

        private IList<IFacetFactory> GetFactoryByFeatureType(FeatureType featureType) {
            return factoriesByFeatureType[featureType];
        }
    }
}