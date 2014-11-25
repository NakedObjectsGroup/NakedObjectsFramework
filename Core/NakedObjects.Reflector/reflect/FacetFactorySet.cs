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
    // to do make this configurable so the list of factories can be managed
    // eg need to implement 

    //    if (OptionalByDefault) {
    //      //  ((FacetFactorySetImpl)reflector.FacetFactorySet).ReplaceAndRegisterFactory(typeof(MandatoryDefaultFacetFactory), new OptionalDefaultFacetFactory(NakedObjectsContext.Reflector));
    //    }


    public class FacetFactorySet : IFacetFactorySet {
        public FacetFactorySet(IFacetFactory[] factories) {
            List<IFacetFactory> allFactories = factories.ToList();
            allFactories.Sort();

            Prefixes = allFactories.Where(factory => factory is IMethodPrefixBasedFacetFactory).
                Cast<IMethodPrefixBasedFacetFactory>().
                SelectMany(prefixfactory => prefixfactory.Prefixes).
                ToArray();

            foreach (FeatureType featureType in Enum.GetValues(typeof (FeatureType))) {
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

        //private void RegisterFactories(IReflector reflector) {
        //    // must be first, so any Facets created can be replaced by other FacetFactorys later.
        //    RegisterFactory(new FallbackFacetFactory(0));
        //    RegisterFactory(new IteratorFilteringFacetFactory(1));
        //    RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory(2));
        //    RegisterFactory(new RemoveSuperclassMethodsFacetFactory(3));
        //    RegisterFactory(new RemoveInitMethodFacetFactory(4));
        //    RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory(5));
        //    RegisterFactory(new RemoveEventHandlerMethodsFacetFactory(6));
        //    RegisterFactory(new TypeMarkerFacetFactory(7));
        //    // must be before any other FacetFactories that install MandatoryFacet.class facets
        //    RegisterFactory(new MandatoryDefaultFacetFactory(8));
        //    RegisterFactory(new PropertyValidateDefaultFacetFactory(9));
        //    RegisterFactory(new ComplementaryMethodsFilteringFacetFactory(10));
        //    RegisterFactory(new ActionMethodsFacetFactory(11));
        //    RegisterFactory(new CollectionFieldMethodsFacetFactory(12));
        //    RegisterFactory(new PropertyMethodsFacetFactory(13));
        //    RegisterFactory(new IconMethodFacetFactory(14));
        //    RegisterFactory(new CallbackMethodsFacetFactory(15));
        //    RegisterFactory(new TitleMethodFacetFactory(16));
        //    RegisterFactory(new ValidateObjectFacetFactory(17));
        //    RegisterFactory(new ActionOrderAnnotationFacetFactory(18));
        //    RegisterFactory(new ComplexTypeAnnotationFacetFactory(19));
        //    RegisterFactory(new ViewModelFacetFactory(20));
        //    RegisterFactory(new BoundedAnnotationFacetFactory(21));
        //    RegisterFactory(new EnumFacetFactory(22));
        //    RegisterFactory(new ActionDefaultAnnotationFacetFactory(23));
        //    RegisterFactory(new PropertyDefaultAnnotationFacetFactory(24));
        //    RegisterFactory(new DescribedAsAnnotationFacetFactory(25));
        //    RegisterFactory(new DisabledAnnotationFacetFactory(26));
        //    RegisterFactory(new PasswordAnnotationFacetFactory(27));
        //    RegisterFactory(new ExecutedAnnotationFacetFactory(28));
        //    RegisterFactory(new PotencyAnnotationFacetFactory(29));
        //    RegisterFactory(new PageSizeAnnotationFacetFactory(30));
        //    RegisterFactory(new FieldOrderAnnotationFacetFactory(31));
        //    RegisterFactory(new HiddenAnnotationFacetFactory(32));
        //    RegisterFactory(new HiddenDefaultMethodFacetFactory(33));
        //    RegisterFactory(new DisableDefaultMethodFacetFactory(34));
        //    RegisterFactory(new AuthorizeAnnotationFacetFactory(35));
        //    RegisterFactory(new ValidateProgrammaticUpdatesAnnotationFacetFactory(36));
        //    RegisterFactory(new ImmutableAnnotationFacetFactory(37));
        //    RegisterFactory(new MaxLengthAnnotationFacetFactory(38));
        //    RegisterFactory(new RangeAnnotationFacetFactory(39));
        //    RegisterFactory(new MemberOrderAnnotationFacetFactory(40));
        //    RegisterFactory(new MultiLineAnnotationFacetFactory(41));
        //    RegisterFactory(new NamedAnnotationFacetFactory(42));
        //    RegisterFactory(new NotPersistedAnnotationFacetFactory(43));
        //    RegisterFactory(new ProgramPersistableOnlyAnnotationFacetFactory(44));
        //    RegisterFactory(new OptionalAnnotationFacetFactory(45));
        //    RegisterFactory(new RequiredAnnotationFacetFactory(46));
        //    RegisterFactory(new PluralAnnotationFacetFactory(47));
        //    RegisterFactory(new DefaultNamingFacetFactory(48)); // must come after Named and Plural factories
        //    RegisterFactory(new KeyAnnotationFacetFactory(49));
        //    RegisterFactory(new ConcurrencyCheckAnnotationFacetFactory(50));
        //    RegisterFactory(new ContributedActionAnnotationFacetFactory(51));
        //    RegisterFactory(new ExcludeFromFindMenuAnnotationFacetFactory(52));
        //    // must come after any facets that install titles
        //    RegisterFactory(new MaskAnnotationFacetFactory(53));
        //    // must come after any facets that install titles, and after mask
        //    // if takes precedence over mask.
        //    RegisterFactory(new RegExAnnotationFacetFactory(54));
        //    RegisterFactory(new TypeOfAnnotationFacetFactory(55));
        //    RegisterFactory(new TableViewAnnotationFacetFactory(56));
        //    RegisterFactory(new TypicalLengthDerivedFromTypeFacetFactory(57));
        //    RegisterFactory(new TypicalLengthAnnotationFacetFactory(58));
        //    RegisterFactory(new EagerlyAnnotationFacetFactory(59));
        //    RegisterFactory(new PresentationHintAnnotationFacetFactory(60));
        //    RegisterFactory(new BooleanValueTypeFacetFactory(61));
        //    RegisterFactory(new ByteValueTypeFacetFactory(62));
        //    RegisterFactory(new SbyteValueTypeFacetFactory(63));
        //    RegisterFactory(new ShortValueTypeFacetFactory(64));
        //    RegisterFactory(new IntValueTypeFacetFactory(65));
        //    RegisterFactory(new LongValueTypeFacetFactory(66));
        //    RegisterFactory(new UShortValueTypeFacetFactory(67));
        //    RegisterFactory(new UIntValueTypeFacetFactory(68));
        //    RegisterFactory(new ULongValueTypeFacetFactory(69));
        //    RegisterFactory(new FloatValueTypeFacetFactory(70));
        //    RegisterFactory(new DoubleValueTypeFacetFactory(71));
        //    RegisterFactory(new DecimalValueTypeFacetFactory(72));
        //    RegisterFactory(new CharValueTypeFacetFactory(73));
        //    RegisterFactory(new DateTimeValueTypeFacetFactory(74));
        //    RegisterFactory(new TimeValueTypeFacetFactory(75));
        //    RegisterFactory(new StringValueTypeFacetFactory(76));
        //    RegisterFactory(new GuidValueTypeFacetFactory(77));
        //    RegisterFactory(new EnumValueTypeFacetFactory(78));
        //    RegisterFactory(new FileAttachmentValueTypeFacetFactory(79));
        //    RegisterFactory(new ImageValueTypeFacetFactory(80));
        //    RegisterFactory(new ArrayValueTypeFacetFactory<byte>(81));
        //    RegisterFactory(new CollectionFacetFactory(82)); // written to not trample over TypeOf if already installed
        //}
    }
}