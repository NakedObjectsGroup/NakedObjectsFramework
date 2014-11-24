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
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;

namespace NakedObjects.Reflect {
    // to do make this configurable so the list of factories can be managed
    // eg need to implement 

    //    if (OptionalByDefault) {
    //      //  ((FacetFactorySetImpl)reflector.FacetFactorySet).ReplaceAndRegisterFactory(typeof(MandatoryDefaultFacetFactory), new OptionalDefaultFacetFactory(NakedObjectsContext.Reflector));
    //    }


    public class FacetFactorySet : IFacetFactorySet {
        /// <summary>
        ///     Factories (in the order they were <see cref="RegisterFactory" /> registered)
        /// </summary>
        private readonly IList<IFacetFactory> factories = new List<IFacetFactory>();

        // Lazily initialized, then cached. The lists remain in the same order that the factories were registered.
        private readonly IDictionary<Type, IFacetFactory> factoryByFactoryType = new Dictionary<Type, IFacetFactory>();
        private IDictionary<FeatureType, IList<IFacetFactory>> factoriesByFeatureType;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see cref="IMethodFilteringFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Filters" />
        /// </para>
        private IList<IMethodFilteringFacetFactory> methodFilteringFactories;

        private string[] prefixes;

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see
        ///         cref="IPropertyOrCollectionIdentifyingFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Recognizes" />
        /// </para>
        private IList<IFacetFactory> propertyOrCollectionIdentifyingFactories;

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

        #region IFacetFactorySet Members

        public IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates) {
            CachePropertyOrCollectionIdentifyingFacetFactoriesIfRequired();
            return propertyOrCollectionIdentifyingFactories.SelectMany(fact => fact.FindCollectionProperties(candidates)).ToList();
        }

        public IList<PropertyInfo> FindProperties(IList<PropertyInfo> candidates, IClassStrategy classStrategy) {
            return propertyOrCollectionIdentifyingFactories.SelectMany(fact => fact.FindProperties(candidates, classStrategy)).ToList();
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
            CacheMethodFilteringFacetFactoriesIfRequired();
            return methodFilteringFactories.Any(factory => factory.Filters(method, classStrategy));
        }

        public bool Recognizes(MethodInfo method) {
            return Prefixes.Any(prefix => method.Name.StartsWith(prefix));
        }

        public void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(FeatureType.Objects)) {
                facetFactory.Process(reflector, type, RemoverElseNullRemover(methodRemover), specification);
            }
        }

        public void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, FeatureType featureType) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetFactory.Process(reflector, method, RemoverElseNullRemover(methodRemover), specification);
            }
        }

        public void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, FeatureType featureType) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(featureType)) {
                facetFactory.Process(reflector, property, RemoverElseNullRemover(methodRemover), specification);
            }
        }

        public void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder specification) {
            foreach (IFacetFactory facetFactory in GetFactoryByFeatureType(FeatureType.ActionParameter)) {
                facetFactory.ProcessParams(reflector, method, paramNum, specification);
            }
        }

        public void Init(IReflector reflector) {
            RegisterFactories(reflector);
        }

        #endregion

        public void RegisterFactory(IFacetFactory factory) {
            ClearCaches();
            factoryByFactoryType.Add(factory.GetType(), factory);
            factories.Add(factory);
        }

        public void ReplaceAndRegisterFactory(Type oldFactoryType, IFacetFactory newFactory) {
            ClearCaches();

            IFacetFactory oldFactory = factoryByFactoryType[oldFactoryType];
            factoryByFactoryType.Remove(oldFactoryType);
            factoryByFactoryType.Add(newFactory.GetType(), newFactory);

            factories[factories.IndexOf(oldFactory)] = newFactory;
        }

        private IList<IFacetFactory> GetFactoryByFeatureType(FeatureType featureType) {
            CacheByFeatureTypeIfRequired();
            return factoriesByFeatureType[featureType];
        }

        private void ClearCaches() {
            factoriesByFeatureType = null;
            methodFilteringFactories = null;
            propertyOrCollectionIdentifyingFactories = null;
        }

        private void CacheByFeatureTypeIfRequired() {
            if (factoriesByFeatureType == null) {
                factoriesByFeatureType = new Dictionary<FeatureType, IList<IFacetFactory>>();
                foreach (IFacetFactory factory in factories) {
                    foreach (FeatureType featureType in Enum.GetValues(typeof (FeatureType))) {
                        if (factory.FeatureTypes.HasFlag(featureType)) {
                            IList<IFacetFactory> factoryList = GetList(factoriesByFeatureType, featureType);
                            factoryList.Add(factory);
                        }
                    }
                }
            }
        }

        private void CacheMethodFilteringFacetFactoriesIfRequired() {
            if (methodFilteringFactories == null) {
                methodFilteringFactories = new List<IMethodFilteringFacetFactory>();
                foreach (IFacetFactory facetFactory in factories) {
                    if (facetFactory is IMethodFilteringFacetFactory) {
                        methodFilteringFactories.Add(facetFactory as IMethodFilteringFacetFactory);
                    }
                }
            }
        }

        private void CachePropertyOrCollectionIdentifyingFacetFactoriesIfRequired() {
            if (propertyOrCollectionIdentifyingFactories == null) {
                propertyOrCollectionIdentifyingFactories = new List<IFacetFactory>();
                foreach (IFacetFactory facetFactory in factories) {
                    if (facetFactory is IPropertyOrCollectionIdentifyingFacetFactory) {
                        propertyOrCollectionIdentifyingFactories.Add(facetFactory);
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
            return methodRemover ?? MethodRemoverConstants.Null;
        }


        private void RegisterFactories(IReflector reflector) {
            // must be first, so any Facets created can be replaced by other FacetFactorys later.
            RegisterFactory(new FallbackFacetFactory());
            RegisterFactory(new IteratorFilteringFacetFactory());
            RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory());
            RegisterFactory(new RemoveSuperclassMethodsFacetFactory());
            RegisterFactory(new RemoveInitMethodFacetFactory());
            RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory());
            RegisterFactory(new RemoveEventHandlerMethodsFacetFactory());
            RegisterFactory(new TypeMarkerFacetFactory());
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFactory(new MandatoryDefaultFacetFactory());
            RegisterFactory(new PropertyValidateDefaultFacetFactory());
            RegisterFactory(new ComplementaryMethodsFilteringFacetFactory());
            RegisterFactory(new ActionMethodsFacetFactory());
            RegisterFactory(new CollectionFieldMethodsFacetFactory());
            RegisterFactory(new PropertyMethodsFacetFactory());
            RegisterFactory(new IconMethodFacetFactory());
            RegisterFactory(new CallbackMethodsFacetFactory());
            RegisterFactory(new TitleMethodFacetFactory());
            RegisterFactory(new ValidateObjectFacetFactory());
            RegisterFactory(new ActionOrderAnnotationFacetFactory());
            RegisterFactory(new ComplexTypeAnnotationFacetFactory());
            RegisterFactory(new ViewModelFacetFactory());
            RegisterFactory(new BoundedAnnotationFacetFactory());
            RegisterFactory(new EnumFacetFactory());
            RegisterFactory(new ActionDefaultAnnotationFacetFactory());
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory());
            RegisterFactory(new DescribedAsAnnotationFacetFactory());
            RegisterFactory(new DisabledAnnotationFacetFactory());
            RegisterFactory(new PasswordAnnotationFacetFactory());
            RegisterFactory(new ExecutedAnnotationFacetFactory());
            RegisterFactory(new PotencyAnnotationFacetFactory());
            RegisterFactory(new PageSizeAnnotationFacetFactory());
            RegisterFactory(new FieldOrderAnnotationFacetFactory());
            RegisterFactory(new HiddenAnnotationFacetFactory());
            RegisterFactory(new HiddenDefaultMethodFacetFactory());
            RegisterFactory(new DisableDefaultMethodFacetFactory());
            RegisterFactory(new AuthorizeAnnotationFacetFactory());
            RegisterFactory(new ValidateProgrammaticUpdatesAnnotationFacetFactory());
            RegisterFactory(new ImmutableAnnotationFacetFactory());
            RegisterFactory(new MaxLengthAnnotationFacetFactory());
            RegisterFactory(new RangeAnnotationFacetFactory());
            RegisterFactory(new MemberOrderAnnotationFacetFactory());
            RegisterFactory(new MultiLineAnnotationFacetFactory());
            RegisterFactory(new NamedAnnotationFacetFactory());
            RegisterFactory(new NotPersistedAnnotationFacetFactory());
            RegisterFactory(new ProgramPersistableOnlyAnnotationFacetFactory());
            RegisterFactory(new OptionalAnnotationFacetFactory());
            RegisterFactory(new RequiredAnnotationFacetFactory());
            RegisterFactory(new PluralAnnotationFacetFactory());
            RegisterFactory(new DefaultNamingFacetFactory()); // must come after Named and Plural factories
            RegisterFactory(new KeyAnnotationFacetFactory());
            RegisterFactory(new ConcurrencyCheckAnnotationFacetFactory());
            RegisterFactory(new ContributedActionAnnotationFacetFactory());
            RegisterFactory(new ExcludeFromFindMenuAnnotationFacetFactory());
            // must come after any facets that install titles
            RegisterFactory(new MaskAnnotationFacetFactory());
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFactory(new RegExAnnotationFacetFactory());
            RegisterFactory(new TypeOfAnnotationFacetFactory());
            RegisterFactory(new TableViewAnnotationFacetFactory());
            RegisterFactory(new TypicalLengthDerivedFromTypeFacetFactory());
            RegisterFactory(new TypicalLengthAnnotationFacetFactory());
            RegisterFactory(new EagerlyAnnotationFacetFactory());
            RegisterFactory(new PresentationHintAnnotationFacetFactory());
            RegisterFactory(new BooleanValueTypeFacetFactory());
            RegisterFactory(new ByteValueTypeFacetFactory());
            RegisterFactory(new SbyteValueTypeFacetFactory());
            RegisterFactory(new ShortValueTypeFacetFactory());
            RegisterFactory(new IntValueTypeFacetFactory());
            RegisterFactory(new LongValueTypeFacetFactory());
            RegisterFactory(new UShortValueTypeFacetFactory());
            RegisterFactory(new UIntValueTypeFacetFactory());
            RegisterFactory(new ULongValueTypeFacetFactory());
            RegisterFactory(new FloatValueTypeFacetFactory());
            RegisterFactory(new DoubleValueTypeFacetFactory());
            RegisterFactory(new DecimalValueTypeFacetFactory());
            RegisterFactory(new CharValueTypeFacetFactory());
            RegisterFactory(new DateTimeValueTypeFacetFactory());
            RegisterFactory(new TimeValueTypeFacetFactory());
            RegisterFactory(new StringValueTypeFacetFactory());
            RegisterFactory(new GuidValueTypeFacetFactory());
            RegisterFactory(new EnumValueTypeFacetFactory());
            RegisterFactory(new FileAttachmentValueTypeFacetFactory());
            RegisterFactory(new ImageValueTypeFacetFactory());
            RegisterFactory(new ArrayValueTypeFacetFactory<byte>());
            RegisterFactory(new CollectionFacetFactory()); // written to not trample over TypeOf if already installed
        }
    }
}