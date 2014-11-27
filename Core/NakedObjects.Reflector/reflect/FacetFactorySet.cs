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

namespace NakedObjects.Reflect {
    internal class FacetFactorySet : IFacetFactorySet {
        public FacetFactorySet(IFacetFactory[] factories) {
            List<IFacetFactory> allFactories = factories.ToList();
            allFactories.Sort();

            Prefixes = allFactories.OfType<IMethodPrefixBasedFacetFactory>().SelectMany(prefixfactory => prefixfactory.Prefixes).ToArray();

            foreach (FeatureType featureType in Enum.GetValues(typeof(FeatureType))) {
                factoriesByFeatureType[featureType] = allFactories.Where(f => f.FeatureTypes.HasFlag(featureType)).ToList();
            }

            methodFilteringFactories = allFactories.OfType<IMethodFilteringFacetFactory>().ToList();
            propertyOrCollectionIdentifyingFactories = allFactories.OfType<IPropertyOrCollectionIdentifyingFacetFactory>().ToList();
        }


        private readonly IDictionary<FeatureType, IList<IFacetFactory>> factoriesByFeatureType = new Dictionary<FeatureType, IList<IFacetFactory>>();

        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see cref="IMethodFilteringFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Filters" />
        /// </para>
        private readonly IList<IMethodFilteringFacetFactory> methodFilteringFactories;


        /// <summary>
        ///     All registered <see cref="IFacetFactory" />s that implement
        ///     <see
        ///         cref="IPropertyOrCollectionIdentifyingFacetFactory" />
        /// </summary>
        /// <para>
        ///     Used within <see cref="IFacetFactorySet.Recognizes" />
        /// </para>
        private readonly IList<IPropertyOrCollectionIdentifyingFacetFactory> propertyOrCollectionIdentifyingFactories;

        private string[] Prefixes { get; set; }

        #region IFacetFactorySet Members

        public IList<PropertyInfo> FindCollectionProperties(IList<PropertyInfo> candidates) {
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

        #endregion

        private IList<IFacetFactory> GetFactoryByFeatureType(FeatureType featureType) {
            return factoriesByFeatureType[featureType];
        }

        private static IMethodRemover RemoverElseNullRemover(IMethodRemover methodRemover) {
            return methodRemover ?? MethodRemoverConstants.Null;
        }


        private void RegisterFactories(IReflector reflector) {
            // must be first, so any Facets created can be replaced by other FacetFactorys later.
            RegisterFactory(new FallbackFacetFactory(reflector));
            RegisterFactory(new IteratorFilteringFacetFactory(reflector));
            RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory(reflector));
            RegisterFactory(new RemoveSuperclassMethodsFacetFactory(reflector));
            RegisterFactory(new RemoveInitMethodFacetFactory(reflector));
            RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory(reflector));
            RegisterFactory(new RemoveEventHandlerMethodsFacetFactory(reflector));
            RegisterFactory(new TypeMarkerFacetFactory(reflector));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFactory(new MandatoryDefaultFacetFactory(reflector));
            RegisterFactory(new PropertyValidateDefaultFacetFactory(reflector));
            RegisterFactory(new ComplementaryMethodsFilteringFacetFactory(reflector));
            RegisterFactory(new ActionMethodsFacetFactory(reflector));
            RegisterFactory(new CollectionFieldMethodsFacetFactory(reflector));
            RegisterFactory(new PropertyMethodsFacetFactory(reflector));
            RegisterFactory(new IconMethodFacetFactory(reflector));
            RegisterFactory(new CallbackMethodsFacetFactory(reflector));
            RegisterFactory(new TitleMethodFacetFactory(reflector));
            RegisterFactory(new ValidateObjectFacetFactory(reflector));
            RegisterFactory(new ActionOrderAnnotationFacetFactory(reflector));
            RegisterFactory(new ComplexTypeAnnotationFacetFactory(reflector));
            RegisterFactory(new ViewModelFacetFactory(reflector));
            RegisterFactory(new BoundedAnnotationFacetFactory(reflector));
            RegisterFactory(new EnumFacetFactory(reflector));
            RegisterFactory(new ActionDefaultAnnotationFacetFactory(reflector));
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory(reflector));
            RegisterFactory(new DescribedAsAnnotationFacetFactory(reflector));
            RegisterFactory(new DisabledAnnotationFacetFactory(reflector));
            RegisterFactory(new PasswordAnnotationFacetFactory(reflector));
            RegisterFactory(new ExecutedAnnotationFacetFactory(reflector));
            RegisterFactory(new PotencyAnnotationFacetFactory(reflector));
            RegisterFactory(new PageSizeAnnotationFacetFactory(reflector));
            RegisterFactory(new FieldOrderAnnotationFacetFactory(reflector));
            RegisterFactory(new HiddenAnnotationFacetFactory(reflector));
            RegisterFactory(new HiddenDefaultMethodFacetFactory(reflector));
            RegisterFactory(new DisableDefaultMethodFacetFactory(reflector));
            RegisterFactory(new AuthorizeAnnotationFacetFactory(reflector));
            RegisterFactory(new ValidateProgrammaticUpdatesAnnotationFacetFactory(reflector));
            RegisterFactory(new ImmutableAnnotationFacetFactory(reflector));
            RegisterFactory(new MaxLengthAnnotationFacetFactory(reflector));
            RegisterFactory(new RangeAnnotationFacetFactory(reflector));
            RegisterFactory(new MemberOrderAnnotationFacetFactory(reflector));
            RegisterFactory(new MultiLineAnnotationFacetFactory(reflector));
            RegisterFactory(new NamedAnnotationFacetFactory(reflector));
            RegisterFactory(new NotPersistedAnnotationFacetFactory(reflector));
            RegisterFactory(new ProgramPersistableOnlyAnnotationFacetFactory(reflector));
            RegisterFactory(new OptionalAnnotationFacetFactory(reflector));
            RegisterFactory(new RequiredAnnotationFacetFactory(reflector));
            RegisterFactory(new PluralAnnotationFacetFactory(reflector));
            RegisterFactory(new DefaultNamingFacetFactory(reflector)); // must come after Named and Plural factories
            RegisterFactory(new KeyAnnotationFacetFactory(reflector));
            RegisterFactory(new ConcurrencyCheckAnnotationFacetFactory(reflector));
            RegisterFactory(new ContributedActionAnnotationFacetFactory(reflector));
            RegisterFactory(new ExcludeFromFindMenuAnnotationFacetFactory(reflector));
            // must come after any facets that install titles
            RegisterFactory(new MaskAnnotationFacetFactory(reflector));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFactory(new RegExAnnotationFacetFactory(reflector));
            RegisterFactory(new TypeOfAnnotationFacetFactory(reflector));
            RegisterFactory(new TableViewAnnotationFacetFactory(reflector));
            RegisterFactory(new TypicalLengthDerivedFromTypeFacetFactory(reflector));
            RegisterFactory(new TypicalLengthAnnotationFacetFactory(reflector));
            RegisterFactory(new EagerlyAnnotationFacetFactory(reflector));
            RegisterFactory(new PresentationHintAnnotationFacetFactory(reflector));
            RegisterFactory(new BooleanValueTypeFacetFactory(reflector));
            RegisterFactory(new ByteValueTypeFacetFactory(reflector));
            RegisterFactory(new SbyteValueTypeFacetFactory(reflector));
            RegisterFactory(new ShortValueTypeFacetFactory(reflector));
            RegisterFactory(new IntValueTypeFacetFactory(reflector));
            RegisterFactory(new LongValueTypeFacetFactory(reflector));
            RegisterFactory(new UShortValueTypeFacetFactory(reflector));
            RegisterFactory(new UIntValueTypeFacetFactory(reflector));
            RegisterFactory(new ULongValueTypeFacetFactory(reflector));
            RegisterFactory(new FloatValueTypeFacetFactory(reflector));
            RegisterFactory(new DoubleValueTypeFacetFactory(reflector));
            RegisterFactory(new DecimalValueTypeFacetFactory(reflector));
            RegisterFactory(new CharValueTypeFacetFactory(reflector));
            RegisterFactory(new DateTimeValueTypeFacetFactory(reflector));
            RegisterFactory(new TimeValueTypeFacetFactory(reflector));
            RegisterFactory(new StringValueTypeFacetFactory(reflector));
            RegisterFactory(new GuidValueTypeFacetFactory(reflector));
            RegisterFactory(new EnumValueTypeFacetFactory(reflector));
            RegisterFactory(new FileAttachmentValueTypeFacetFactory(reflector));
            RegisterFactory(new ImageValueTypeFacetFactory(reflector));
            RegisterFactory(new ArrayValueTypeFacetFactory<byte>(reflector));
            RegisterFactory(new CollectionFacetFactory(reflector)); // written to not trample over TypeOf if already installed
        }
    }
}