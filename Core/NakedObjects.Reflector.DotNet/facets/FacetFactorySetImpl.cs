// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Propparam.TypicalLength;
using NakedObjects.Reflector.DotNet.Facets.Actcoll.Table;
using NakedObjects.Reflector.DotNet.Facets.Actcoll.Typeof;
using NakedObjects.Reflector.DotNet.Facets.Actions;
using NakedObjects.Reflector.DotNet.Facets.Actions.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Actions.Executed;
using NakedObjects.Reflector.DotNet.Facets.Actions.PageSize;
using NakedObjects.Reflector.DotNet.Facets.Actions.Potency;
using NakedObjects.Reflector.DotNet.Facets.Authorize;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.DotNet.Facets.Disable;
using NakedObjects.Reflector.DotNet.Facets.Hide;
using NakedObjects.Reflector.DotNet.Facets.Naming.DescribedAs;
using NakedObjects.Reflector.DotNet.Facets.Naming.Named;
using NakedObjects.Reflector.DotNet.Facets.Objects.Aggregated;
using NakedObjects.Reflector.DotNet.Facets.Objects.Bounded;
using NakedObjects.Reflector.DotNet.Facets.Objects.Callbacks;
using NakedObjects.Reflector.DotNet.Facets.Objects.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title;
using NakedObjects.Reflector.DotNet.Facets.Objects.Immutable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Key;
using NakedObjects.Reflector.DotNet.Facets.Objects.NotPersistable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Parseable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Value;
using NakedObjects.Reflector.DotNet.Facets.Objects.ViewModel;
using NakedObjects.Reflector.DotNet.Facets.Ordering.ActionOrder;
using NakedObjects.Reflector.DotNet.Facets.Ordering.FieldOrder;
using NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.DotNet.Facets.Password;
using NakedObjects.Reflector.DotNet.Facets.Presentation;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted;
using NakedObjects.Reflector.DotNet.Facets.Properties;
using NakedObjects.Reflector.DotNet.Facets.Properties.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Properties.Eagerly;
using NakedObjects.Reflector.DotNet.Facets.Properties.Enums;
using NakedObjects.Reflector.DotNet.Facets.Properties.Validate;
using NakedObjects.Reflector.DotNet.Facets.Properties.Version;
using NakedObjects.Reflector.DotNet.Facets.Propparam.MultiLine;
using NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mask;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Range;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx;
using NakedObjects.Reflector.DotNet.Value;

namespace NakedObjects.Reflector.DotNet.Facets {
    public class FacetFactorySetImpl : FacetFactorySetAbstract {
        public void Init() {
            RegisterFactories();
        }

        private void RegisterFactories() {
            // must be first, so any Facets created can be replaced by other FacetFactorys later.
            RegisterFactory(new FallbackFacetFactory());
            RegisterFactory(new IteratorFilteringFacetFactory());
            RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory());
            RegisterFactory(new RemoveSuperclassMethodsFacetFactory());
            RegisterFactory(new RemoveInitMethodFacetFactory());
            RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory());
            RegisterFactory(new RemoveEventHandlerMethodsFacetFactory());
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
            RegisterFactory(new ActionOrderAnnotationFacetFactory());
            RegisterFactory(new ComplexTypeAnnotationFacetFactory());
            RegisterFactory(new ViewModelFacetFactory());
            RegisterFactory(new BoundedAnnotationFacetFactory());
            RegisterFactory(new EnumFacetFactory());
            RegisterFactory(new ActionDefaultAnnotationFacetFactory());
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory());
            RegisterFactory(new DefaultedFacetFactory());
            RegisterFactory(new DescribedAsAnnotationFacetFactory());
            RegisterFactory(new DisabledAnnotationFacetFactory());
            RegisterFactory(new PasswordAnnotationFacetFactory());
            RegisterFactory(new EncodeableFacetFactory());
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
            RegisterFactory(new ParseableFacetFactory());
            RegisterFactory(new PluralAnnotationFacetFactory());
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
            RegisterFactory(new ValueFacetFactory()); // so we can dogfood the NO applib "value" types
            RegisterFactory(new FacetsAnnotationFacetFactory());
        }
    }
}