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
     
        public override void Init(INakedObjectReflector reflector) {
            RegisterFactories(reflector);
        }

        private void RegisterFactories(INakedObjectReflector reflector) {
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