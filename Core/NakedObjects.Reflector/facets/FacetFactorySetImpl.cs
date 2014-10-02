// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Propparam.TypicalLength;
using NakedObjects.Architecture.Reflect;
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

        public override void Init(IMetadata metadata) {
            RegisterFactories(metadata);
        }

        private void RegisterFactories(IMetadata metadata) {
            // must be first, so any Facets created can be replaced by other FacetFactorys later.
            RegisterFactory(new FallbackFacetFactory(metadata));
            RegisterFactory(new IteratorFilteringFacetFactory(metadata));
            RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory(metadata));
            RegisterFactory(new RemoveSuperclassMethodsFacetFactory(metadata));
            RegisterFactory(new RemoveInitMethodFacetFactory(metadata));
            RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory(metadata));
            RegisterFactory(new RemoveEventHandlerMethodsFacetFactory(metadata));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFactory(new MandatoryDefaultFacetFactory(metadata));
            RegisterFactory(new PropertyValidateDefaultFacetFactory(metadata));
            RegisterFactory(new ComplementaryMethodsFilteringFacetFactory(metadata));
            RegisterFactory(new ActionMethodsFacetFactory(metadata));
            RegisterFactory(new CollectionFieldMethodsFacetFactory(metadata));
            RegisterFactory(new PropertyMethodsFacetFactory(metadata));
            RegisterFactory(new IconMethodFacetFactory(metadata));
            RegisterFactory(new CallbackMethodsFacetFactory(metadata));
            RegisterFactory(new TitleMethodFacetFactory(metadata));
            RegisterFactory(new ActionOrderAnnotationFacetFactory(metadata));
            RegisterFactory(new ComplexTypeAnnotationFacetFactory(metadata));
            RegisterFactory(new ViewModelFacetFactory(metadata));
            RegisterFactory(new BoundedAnnotationFacetFactory(metadata));
            RegisterFactory(new EnumFacetFactory(metadata));
            RegisterFactory(new ActionDefaultAnnotationFacetFactory(metadata));
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory(metadata));
            RegisterFactory(new DefaultedFacetFactory(metadata));
            RegisterFactory(new DescribedAsAnnotationFacetFactory(metadata));
            RegisterFactory(new DisabledAnnotationFacetFactory(metadata));
            RegisterFactory(new PasswordAnnotationFacetFactory(metadata));
            RegisterFactory(new EncodeableFacetFactory(metadata));
            RegisterFactory(new ExecutedAnnotationFacetFactory(metadata));
            RegisterFactory(new PotencyAnnotationFacetFactory(metadata));
            RegisterFactory(new PageSizeAnnotationFacetFactory(metadata));
            RegisterFactory(new FieldOrderAnnotationFacetFactory(metadata));
            RegisterFactory(new HiddenAnnotationFacetFactory(metadata));
            RegisterFactory(new HiddenDefaultMethodFacetFactory(metadata));
            RegisterFactory(new DisableDefaultMethodFacetFactory(metadata));
            RegisterFactory(new AuthorizeAnnotationFacetFactory(metadata));
            RegisterFactory(new ValidateProgrammaticUpdatesAnnotationFacetFactory(metadata));
            RegisterFactory(new ImmutableAnnotationFacetFactory(metadata));
            RegisterFactory(new MaxLengthAnnotationFacetFactory(metadata));
            RegisterFactory(new RangeAnnotationFacetFactory(metadata));
            RegisterFactory(new MemberOrderAnnotationFacetFactory(metadata));
            RegisterFactory(new MultiLineAnnotationFacetFactory(metadata));
            RegisterFactory(new NamedAnnotationFacetFactory(metadata));
            RegisterFactory(new NotPersistedAnnotationFacetFactory(metadata));
            RegisterFactory(new ProgramPersistableOnlyAnnotationFacetFactory(metadata));
            RegisterFactory(new OptionalAnnotationFacetFactory(metadata));
            RegisterFactory(new RequiredAnnotationFacetFactory(metadata));
            RegisterFactory(new ParseableFacetFactory(metadata));
            RegisterFactory(new PluralAnnotationFacetFactory(metadata));
            RegisterFactory(new KeyAnnotationFacetFactory(metadata));
            RegisterFactory(new ConcurrencyCheckAnnotationFacetFactory(metadata));
            RegisterFactory(new ContributedActionAnnotationFacetFactory(metadata));
            RegisterFactory(new ExcludeFromFindMenuAnnotationFacetFactory(metadata));
            // must come after any facets that install titles
            RegisterFactory(new MaskAnnotationFacetFactory(metadata));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFactory(new RegExAnnotationFacetFactory(metadata));
            RegisterFactory(new TypeOfAnnotationFacetFactory(metadata));
            RegisterFactory(new TableViewAnnotationFacetFactory(metadata));
            RegisterFactory(new TypicalLengthDerivedFromTypeFacetFactory(metadata));
            RegisterFactory(new TypicalLengthAnnotationFacetFactory(metadata));
            RegisterFactory(new EagerlyAnnotationFacetFactory(metadata));
            RegisterFactory(new PresentationHintAnnotationFacetFactory(metadata));
            RegisterFactory(new BooleanValueTypeFacetFactory(metadata));
            RegisterFactory(new ByteValueTypeFacetFactory(metadata));
            RegisterFactory(new SbyteValueTypeFacetFactory(metadata));
            RegisterFactory(new ShortValueTypeFacetFactory(metadata));
            RegisterFactory(new IntValueTypeFacetFactory(metadata));
            RegisterFactory(new LongValueTypeFacetFactory(metadata));
            RegisterFactory(new UShortValueTypeFacetFactory(metadata));
            RegisterFactory(new UIntValueTypeFacetFactory(metadata));
            RegisterFactory(new ULongValueTypeFacetFactory(metadata));
            RegisterFactory(new FloatValueTypeFacetFactory(metadata));
            RegisterFactory(new DoubleValueTypeFacetFactory(metadata));
            RegisterFactory(new DecimalValueTypeFacetFactory(metadata));
            RegisterFactory(new CharValueTypeFacetFactory(metadata));
            RegisterFactory(new DateTimeValueTypeFacetFactory(metadata));
            RegisterFactory(new TimeValueTypeFacetFactory(metadata));
            RegisterFactory(new StringValueTypeFacetFactory(metadata));
            RegisterFactory(new GuidValueTypeFacetFactory(metadata));
            RegisterFactory(new EnumValueTypeFacetFactory(metadata));
            RegisterFactory(new FileAttachmentValueTypeFacetFactory(metadata));
            RegisterFactory(new ImageValueTypeFacetFactory(metadata));
            RegisterFactory(new ArrayValueTypeFacetFactory<byte>(metadata));
            RegisterFactory(new CollectionFacetFactory(metadata)); // written to not trample over TypeOf if already installed
            RegisterFactory(new ValueFacetFactory(metadata)); // so we can dogfood the NO applib "value" types
            RegisterFactory(new FacetsAnnotationFacetFactory(metadata));
        }
    }
}