// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class FacetFactorySetAbstract : IFacetFactorySet {
        private readonly INakedObjectReflector reflector;

        protected FacetFactorySetAbstract(INakedObjectReflector reflector) {
            this.reflector = reflector;
        }

        private readonly object cacheLock = true;

        /// <summary>
        ///     Factories (in the order they were <see cref="RegisterFactory" /> registered)
        /// </summary>
        private readonly IList<IFacetFactory> factories = new List<IFacetFactory>();

        // Lazily initialized, then cached. The lists remain in the same order that the factories were registered.
        private readonly IDictionary<Type, IFacetFactory> factoryByFactoryType = new Dictionary<Type, IFacetFactory>();
        private IDictionary<NakedObjectFeatureType, IList<IFacetFactory>> factoriesByFeatureType;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see cref="IMethodFilteringFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Filters" />
        /// </para>
        private IList<IMethodFilteringFacetFactory> methodFilteringFactories;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see
        ///         cref="IPropertyOrCollectionIdentifyingFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Recognizes" />
        /// </para>
        private IList<IFacetFactory> propertyOrCollectionIdentifyingFactories;

        #region IFacetFactorySet Members

        private string[] prefixes;

        private string[] Prefixes {
            get {
                if (prefixes == null) {
                    prefixes = factories.Where(factory => factory is IMethodPrefixBasedFacetFactory).
                                         Cast<IMethodPrefixBasedFacetFactory>().
                                         SelectMany(prefixfactory => prefixfactory.Prefixes).
                                         ToArray();
                }
                return prefixes;
            }
        }

        public INakedObjectReflector Reflector {
            get { return reflector; }
        }

        public void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            CachePropertyOrCollectionIdentifyingFacetFactoriesIfRequired();
            foreach (IFacetFactory facetFactory in propertyOrCollectionIdentifyingFactories) {
                facetFactory.FindCollectionProperties(candidates, methodListToAppendTo);
            }
        }

        public void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            foreach (IFacetFactory facetFactory in propertyOrCollectionIdentifyingFactories) {
                facetFactory.FindProperties(candidates, methodListToAppendTo);
            }
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
        public bool Filters(MethodInfo method) {
            CacheMethodFilteringFacetFactoriesIfRequired();
            return methodFilteringFactories.Any(factory => factory.Filters(method));
        }

        public bool Recognizes(MethodInfo method) {
            return Prefixes.Any(prefix => method.Name.StartsWith(prefix));
        }

        public bool Process(Type type, IMethodRemover methodRemover, IFacetHolder facetHolder) {
            bool facetsAdded = false;
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(NakedObjectFeatureType.Objects)) {
                facetsAdded = facetFactory.Process(type, RemoverElseNullRemover(methodRemover), facetHolder) | facetsAdded;
            }
            return facetsAdded;
        }

        public bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder facetHolder, NakedObjectFeatureType featureType) {
            bool facetsAdded = false;
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetsAdded = facetFactory.Process(method, RemoverElseNullRemover(methodRemover), facetHolder) | facetsAdded;
            }
            return facetsAdded;
        }

        public bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder facetHolder, NakedObjectFeatureType featureType) {
            bool facetsAdded = false;
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetsAdded = facetFactory.Process(property, RemoverElseNullRemover(methodRemover), facetHolder) | facetsAdded;
            }
            return facetsAdded;
        }

        public bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder facetHolder) {
            bool facetsAdded = false;
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(NakedObjectFeatureType.ActionParameter)) {
                facetsAdded = facetFactory.ProcessParams(method, paramNum, facetHolder) | facetsAdded;
            }
            return facetsAdded;
        }

        #endregion

      

        public void RegisterFactory(IFacetFactory factory) {
            lock (cacheLock) {
                ClearCaches();
                factoryByFactoryType.Add(factory.GetType(), factory);
                factories.Add(factory);

            }
        }

        public void ReplaceAndRegisterFactory(Type oldFactoryType, IFacetFactory newFactory) {
            lock (cacheLock) {
                ClearCaches();

                IFacetFactory oldFactory = factoryByFactoryType[oldFactoryType];
                factoryByFactoryType.Remove(oldFactoryType);
                factoryByFactoryType.Add(newFactory.GetType(), newFactory);

                factories[factories.IndexOf(oldFactory)] = newFactory;

            }
        }

        private IList<IFacetFactory> GetFactoryByFeatureType(NakedObjectFeatureType featureType) {
            CacheByFeatureTypeIfRequired();
            return factoriesByFeatureType[featureType];
        }

        private void ClearCaches() {
            factoriesByFeatureType = null;
            methodFilteringFactories = null;
            propertyOrCollectionIdentifyingFactories = null;
        }

        private void CacheByFeatureTypeIfRequired() {
            lock (cacheLock) {
                if (factoriesByFeatureType == null) {
                    factoriesByFeatureType = new Dictionary<NakedObjectFeatureType, IList<IFacetFactory>>();
                    foreach (IFacetFactory factory in factories) {
                        foreach (NakedObjectFeatureType featureType in factory.FeatureTypes) {
                            IList<IFacetFactory> factoryList = GetList(factoriesByFeatureType, featureType);
                            factoryList.Add(factory);
                        }
                    }
                }
            }
        }

        private void CacheMethodFilteringFacetFactoriesIfRequired() {
            lock (cacheLock) {
                if (methodFilteringFactories == null) {
                    methodFilteringFactories = new List<IMethodFilteringFacetFactory>();
                    foreach (IFacetFactory facetFactory in factories) {
                        if (facetFactory is IMethodFilteringFacetFactory) {
                            methodFilteringFactories.Add(facetFactory as IMethodFilteringFacetFactory);
                        }
                    }
                }
            }
        }

        private void CachePropertyOrCollectionIdentifyingFacetFactoriesIfRequired() {
            lock (cacheLock) {
                if (propertyOrCollectionIdentifyingFactories == null) {
                    propertyOrCollectionIdentifyingFactories = new List<IFacetFactory>();
                    foreach (IFacetFactory facetFactory in factories) {
                        if (facetFactory is IPropertyOrCollectionIdentifyingFacetFactory) {
                            propertyOrCollectionIdentifyingFactories.Add(facetFactory);
                        }
                    }
                }
            }
        }

        private static IList<IFacetFactory> GetList<TKey>(IDictionary<TKey, IList<IFacetFactory>> map, TKey key) {
            if (!map.ContainsKey(key)) {
                map.Add(key, new List<IFacetFactory>());
            }
            return map[key];
        }

        private static IMethodRemover RemoverElseNullRemover(IMethodRemover methodRemover) {
            return methodRemover ?? MethodRemoverConstants.NULL;
        }
    }
}