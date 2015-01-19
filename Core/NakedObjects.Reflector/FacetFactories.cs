using NakedObjects.Architecture.Component;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;
using System;
using System.Collections.Generic;

namespace NakedObjects.Reflect {
    /// <summary>
    /// Provides access to the standard set of Facet Factories maintained by the framework
    /// </summary>
    public static class FacetFactories {

        /// <summary>
        /// Returns an array of all the types of FacetFactory actively maintained by the framework,
        /// in the order in which they should be registered for injection.
        /// </summary>
        /// <returns></returns>
        public static Type[] StandardFacetFactories() {
            return new Type[] {
            typeof(FallbackFacetFactory),
            typeof(IteratorFilteringFacetFactory),
            typeof(UnsupportedParameterTypesMethodFilteringFactory),
            typeof(RemoveSuperclassMethodsFacetFactory),
            typeof(RemoveInitMethodFacetFactory),
            typeof(RemoveDynamicProxyMethodsFacetFactory),
            typeof(RemoveEventHandlerMethodsFacetFactory),
            typeof(TypeMarkerFacetFactory),
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            typeof(MandatoryDefaultFacetFactory),
            typeof(PropertyValidateDefaultFacetFactory),
            typeof(ComplementaryMethodsFilteringFacetFactory),
            typeof(ActionMethodsFacetFactory),
            typeof(CollectionFieldMethodsFacetFactory),
            typeof(PropertyMethodsFacetFactory),
            typeof(IconMethodFacetFactory),
            typeof(CallbackMethodsFacetFactory),
            typeof(TitleMethodFacetFactory),
            typeof(ValidateObjectFacetFactory),
            typeof(ComplexTypeAnnotationFacetFactory),
            typeof(ViewModelFacetFactory),
            typeof(BoundedAnnotationFacetFactory),
            typeof(EnumFacetFactory),
            typeof(ActionDefaultAnnotationFacetFactory),
            typeof(PropertyDefaultAnnotationFacetFactory),
            typeof(DescribedAsAnnotationFacetFactory),
            typeof(DisabledAnnotationFacetFactory),
            typeof(PasswordAnnotationFacetFactory),
            typeof(ExecutedAnnotationFacetFactory),
            typeof(PotencyAnnotationFacetFactory),
            typeof(PageSizeAnnotationFacetFactory),
            typeof(HiddenAnnotationFacetFactory),
            typeof(HiddenDefaultMethodFacetFactory),
            typeof(DisableDefaultMethodFacetFactory),
            typeof(AuthorizeAnnotationFacetFactory),
            typeof(ValidateProgrammaticUpdatesAnnotationFacetFactory),
            typeof(ImmutableAnnotationFacetFactory),
            typeof(MaxLengthAnnotationFacetFactory),
            typeof(RangeAnnotationFacetFactory),
            typeof(MemberOrderAnnotationFacetFactory),
            typeof(MultiLineAnnotationFacetFactory),
            typeof(NamedAnnotationFacetFactory),
            typeof(NotPersistedAnnotationFacetFactory),
            typeof(ProgramPersistableOnlyAnnotationFacetFactory),
            typeof(OptionalAnnotationFacetFactory),
            typeof(RequiredAnnotationFacetFactory),
            typeof(PluralAnnotationFacetFactory),
            typeof(DefaultNamingFacetFactory),// must come after Named and Plural factories
            typeof(ConcurrencyCheckAnnotationFacetFactory),
            typeof(ContributedActionAnnotationFacetFactory),
            typeof(FinderActionFacetFactory),
            // must come after any facets that install titles
            typeof(MaskAnnotationFacetFactory),
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            typeof(FinderActionFacetFactory),
            typeof(TypeOfAnnotationFacetFactory),
            typeof(TableViewAnnotationFacetFactory),
            typeof(TypicalLengthDerivedFromTypeFacetFactory),
            typeof(TypicalLengthAnnotationFacetFactory),
            typeof(EagerlyAnnotationFacetFactory),
            typeof(PresentationHintAnnotationFacetFactory),
            typeof(BooleanValueTypeFacetFactory),
            typeof(ByteValueTypeFacetFactory),
            typeof(SbyteValueTypeFacetFactory),
            typeof(ShortValueTypeFacetFactory),
            typeof(IntValueTypeFacetFactory),
            typeof(LongValueTypeFacetFactory),
            typeof(UShortValueTypeFacetFactory),
            typeof(UIntValueTypeFacetFactory),
            typeof(ULongValueTypeFacetFactory),
            typeof(FloatValueTypeFacetFactory),
            typeof(DoubleValueTypeFacetFactory),
            typeof(DecimalValueTypeFacetFactory),
            typeof(CharValueTypeFacetFactory),
            typeof(DateTimeValueTypeFacetFactory),
            typeof(TimeValueTypeFacetFactory),
            typeof(StringValueTypeFacetFactory),
            typeof(GuidValueTypeFacetFactory),
            typeof(EnumValueTypeFacetFactory),
            typeof(FileAttachmentValueTypeFacetFactory),
            typeof(ImageValueTypeFacetFactory),
            typeof(ArrayValueTypeFacetFactory<byte>),
            typeof(CollectionFacetFactory),
            typeof(MenuFacetFactory)
            };
        }
    }
}
