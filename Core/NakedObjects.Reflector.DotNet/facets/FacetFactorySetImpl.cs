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
     
        public FacetFactorySetImpl(INakedObjectReflector reflector) :base(reflector) {
           
        }

        public void Init() {
            RegisterFactories();
        }

        private void RegisterFactories() {
            // must be first, so any Facets created can be replaced by other FacetFactorys later.
            RegisterFactory(new FallbackFacetFactory(Reflector));
            RegisterFactory(new IteratorFilteringFacetFactory(Reflector));
            RegisterFactory(new UnsupportedParameterTypesMethodFilteringFactory(Reflector));
            RegisterFactory(new RemoveSuperclassMethodsFacetFactory(Reflector));
            RegisterFactory(new RemoveInitMethodFacetFactory(Reflector));
            RegisterFactory(new RemoveDynamicProxyMethodsFacetFactory(Reflector));
            RegisterFactory(new RemoveEventHandlerMethodsFacetFactory(Reflector));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFactory(new MandatoryDefaultFacetFactory(Reflector));
            RegisterFactory(new PropertyValidateDefaultFacetFactory(Reflector));
            RegisterFactory(new ComplementaryMethodsFilteringFacetFactory(Reflector));
            RegisterFactory(new ActionMethodsFacetFactory(Reflector));
            RegisterFactory(new CollectionFieldMethodsFacetFactory(Reflector));
            RegisterFactory(new PropertyMethodsFacetFactory(Reflector));
            RegisterFactory(new IconMethodFacetFactory(Reflector));
            RegisterFactory(new CallbackMethodsFacetFactory(Reflector));
            RegisterFactory(new TitleMethodFacetFactory(Reflector));
            RegisterFactory(new ActionOrderAnnotationFacetFactory(Reflector));
            RegisterFactory(new ComplexTypeAnnotationFacetFactory(Reflector));
            RegisterFactory(new ViewModelFacetFactory(Reflector));
            RegisterFactory(new BoundedAnnotationFacetFactory(Reflector));
            RegisterFactory(new EnumFacetFactory(Reflector));
            RegisterFactory(new ActionDefaultAnnotationFacetFactory(Reflector));
            RegisterFactory(new PropertyDefaultAnnotationFacetFactory(Reflector));
            RegisterFactory(new DefaultedFacetFactory(Reflector));
            RegisterFactory(new DescribedAsAnnotationFacetFactory(Reflector));
            RegisterFactory(new DisabledAnnotationFacetFactory(Reflector));
            RegisterFactory(new PasswordAnnotationFacetFactory(Reflector));
            RegisterFactory(new EncodeableFacetFactory(Reflector));
            RegisterFactory(new ExecutedAnnotationFacetFactory(Reflector));
            RegisterFactory(new PotencyAnnotationFacetFactory(Reflector));
            RegisterFactory(new PageSizeAnnotationFacetFactory(Reflector));
            RegisterFactory(new FieldOrderAnnotationFacetFactory(Reflector));
            RegisterFactory(new HiddenAnnotationFacetFactory(Reflector));
            RegisterFactory(new HiddenDefaultMethodFacetFactory(Reflector));
            RegisterFactory(new DisableDefaultMethodFacetFactory(Reflector));
            RegisterFactory(new AuthorizeAnnotationFacetFactory(Reflector));
            RegisterFactory(new ValidateProgrammaticUpdatesAnnotationFacetFactory(Reflector));
            RegisterFactory(new ImmutableAnnotationFacetFactory(Reflector));
            RegisterFactory(new MaxLengthAnnotationFacetFactory(Reflector));
            RegisterFactory(new RangeAnnotationFacetFactory(Reflector));
            RegisterFactory(new MemberOrderAnnotationFacetFactory(Reflector));
            RegisterFactory(new MultiLineAnnotationFacetFactory(Reflector));
            RegisterFactory(new NamedAnnotationFacetFactory(Reflector));
            RegisterFactory(new NotPersistedAnnotationFacetFactory(Reflector));
            RegisterFactory(new ProgramPersistableOnlyAnnotationFacetFactory(Reflector));
            RegisterFactory(new OptionalAnnotationFacetFactory(Reflector));
            RegisterFactory(new RequiredAnnotationFacetFactory(Reflector));
            RegisterFactory(new ParseableFacetFactory(Reflector));
            RegisterFactory(new PluralAnnotationFacetFactory(Reflector));
            RegisterFactory(new KeyAnnotationFacetFactory(Reflector));
            RegisterFactory(new ConcurrencyCheckAnnotationFacetFactory(Reflector));
            RegisterFactory(new ContributedActionAnnotationFacetFactory(Reflector));
            RegisterFactory(new ExcludeFromFindMenuAnnotationFacetFactory(Reflector));
            // must come after any facets that install titles
            RegisterFactory(new MaskAnnotationFacetFactory(Reflector));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            RegisterFactory(new RegExAnnotationFacetFactory(Reflector));
            RegisterFactory(new TypeOfAnnotationFacetFactory(Reflector));
            RegisterFactory(new TableViewAnnotationFacetFactory(Reflector));
            RegisterFactory(new TypicalLengthDerivedFromTypeFacetFactory(Reflector));
            RegisterFactory(new TypicalLengthAnnotationFacetFactory(Reflector));
            RegisterFactory(new EagerlyAnnotationFacetFactory(Reflector));
            RegisterFactory(new PresentationHintAnnotationFacetFactory(Reflector));
            RegisterFactory(new BooleanValueTypeFacetFactory(Reflector));
            RegisterFactory(new ByteValueTypeFacetFactory(Reflector));
            RegisterFactory(new SbyteValueTypeFacetFactory(Reflector));
            RegisterFactory(new ShortValueTypeFacetFactory(Reflector));
            RegisterFactory(new IntValueTypeFacetFactory(Reflector));
            RegisterFactory(new LongValueTypeFacetFactory(Reflector));
            RegisterFactory(new UShortValueTypeFacetFactory(Reflector));
            RegisterFactory(new UIntValueTypeFacetFactory(Reflector));
            RegisterFactory(new ULongValueTypeFacetFactory(Reflector));
            RegisterFactory(new FloatValueTypeFacetFactory(Reflector));
            RegisterFactory(new DoubleValueTypeFacetFactory(Reflector));
            RegisterFactory(new DecimalValueTypeFacetFactory(Reflector));
            RegisterFactory(new CharValueTypeFacetFactory(Reflector));
            RegisterFactory(new DateTimeValueTypeFacetFactory(Reflector));
            RegisterFactory(new TimeValueTypeFacetFactory(Reflector));
            RegisterFactory(new StringValueTypeFacetFactory(Reflector));
            RegisterFactory(new GuidValueTypeFacetFactory(Reflector));
            RegisterFactory(new EnumValueTypeFacetFactory(Reflector));
            RegisterFactory(new FileAttachmentValueTypeFacetFactory(Reflector));
            RegisterFactory(new ImageValueTypeFacetFactory(Reflector));
            RegisterFactory(new ArrayValueTypeFacetFactory<byte>(Reflector));
            RegisterFactory(new CollectionFacetFactory(Reflector)); // written to not trample over TypeOf if already installed
            RegisterFactory(new ValueFacetFactory(Reflector)); // so we can dogfood the NO applib "value" types
            RegisterFactory(new FacetsAnnotationFacetFactory(Reflector));
        }
    }
}