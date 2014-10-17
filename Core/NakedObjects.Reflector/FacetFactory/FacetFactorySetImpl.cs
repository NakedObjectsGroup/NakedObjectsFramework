// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Reflector.DotNet.Facets.Objects.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title;
using NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.DotNet.Facets.Propparam.MultiLine;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mask;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Reflector.DotNet.Value;

namespace NakedObjects.Reflector.FacetFactory {
    public class FacetFactorySetImpl : FacetFactorySetAbstract {
        public override void Init(IReflector reflector) {
            RegisterFactories(reflector);
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
            RegisterFactory(new ActionOrderAnnotationFacetFactory(reflector));
            RegisterFactory(new ComplexTypeAnnotationFacetFactory(reflector));
            RegisterFactory(new ViewModelFacetFactory(reflector));
            RegisterFactory(new BoundedAnnotationFacetFactory(reflector));
            RegisterFactory(new EnumFacetFactory(reflector));
            RegisterFactory(new ActionDefaultAnnotationFacetFactory(reflector));
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory(reflector));
            RegisterFactory(new DefaultedFacetFactory(reflector));
            RegisterFactory(new DescribedAsAnnotationFacetFactory(reflector));
            RegisterFactory(new DisabledAnnotationFacetFactory(reflector));
            RegisterFactory(new PasswordAnnotationFacetFactory(reflector));
            RegisterFactory(new EncodeableFacetFactory(reflector));
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
            RegisterFactory(new ParseableFacetFactory(reflector));
            RegisterFactory(new PluralAnnotationFacetFactory(reflector));
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
            RegisterFactory(new ValueFacetFactory(reflector)); // so we can dogfood the NO applib "value" types
            RegisterFactory(new FacetsAnnotationFacetFactory(reflector));
        }
    }
}