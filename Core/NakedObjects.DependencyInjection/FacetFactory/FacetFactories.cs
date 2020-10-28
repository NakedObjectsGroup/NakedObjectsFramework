// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.ParallelReflect.FunctionalFacetFactory;
using NakedObjects.ParallelReflect.TypeFacetFactory;

namespace NakedObjects.ParallelReflect {
    /// <summary>
    ///     Provides access to the standard set of Facet Factories maintained by the framework
    /// </summary>
    public static class FacetFactories {
        /// <summary>
        ///     Returns an array of all the types of FacetFactory actively maintained by the framework,
        ///     in the order in which they should be registered for injection.
        /// </summary>
        /// <returns></returns>
        public static Type[] StandardFacetFactories() =>
            new[] {
                typeof(FallbackFacetFactory),
                typeof(IteratorFilteringFacetFactory),
                typeof(SystemClassMethodFilteringFactory),
                typeof(SystemClassPropertyFilteringFactory),
                typeof(RemoveSuperclassMethodsFacetFactory),
                typeof(RemoveDynamicProxyMethodsFacetFactory),
                typeof(RemoveEventHandlerMethodsFacetFactory),
                typeof(RemoveIgnoredMethodsFacetFactory),
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
                typeof(DateOnlyFacetFactory),
                typeof(DataTypeAnnotationFacetFactory),
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
                typeof(RegExAnnotationFacetFactory),
                typeof(DefaultNamingFacetFactory), // must come after Named and Plural factories
                typeof(ConcurrencyCheckAnnotationFacetFactory),
                typeof(ContributedActionAnnotationFacetFactory),
                typeof(FinderActionFacetFactory),
                typeof(FindMenuFacetFactory),
                // must come after any facets that install titles
                typeof(MaskAnnotationFacetFactory),
                // must come after any facets that install titles, and after mask
                // if takes precedence over mask.
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
                typeof(MenuFacetFactory),

                typeof(FunctionsFacetFactory),
                typeof(ContributedFunctionFacetFactory),
                typeof(InjectedParameterFacetFactory),
                typeof(InjectedAnnotationFacetFactory),
                typeof(TitleToStringMethodFacetFactory),
                typeof(LifeCycleFunctionsFacetFactory),
                typeof(DisableFunctionFacetFactory),
                typeof(ActionValidateViaFunctionFacetFactory),
                typeof(HideFunctionFacetFactory),
                typeof(ActionDefaultViaFunctionFacetFactory),
                typeof(ActionChoicesViaFunctionFacetFactory),
                typeof(AutocompleteViaFunctionFacetFactory),
                typeof(PotencyDerivedFromSignatureFacetFactory),
                typeof(ViewModelAnnotationFacetFactory)
            };

        public static int StandardIndexOf(Type factory) => Array.FindIndex(StandardFacetFactories(), x => x == factory);
    }
}