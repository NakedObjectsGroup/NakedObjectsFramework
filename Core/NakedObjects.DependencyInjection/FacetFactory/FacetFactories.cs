// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions.Reflector.FacetFactory;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.TypeFacetFactory;

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
                typeof(NakedObjects.Reflector.FacetFactory.FallbackFacetFactory),
                typeof(IteratorFilteringFacetFactory),
                typeof(SystemClassMethodFilteringFactory),
                typeof(SystemClassPropertyFilteringFactory),
                typeof(RemoveSuperclassMethodsFacetFactory),
                typeof(RemoveDynamicProxyMethodsFacetFactory),
                typeof(RemoveEventHandlerMethodsFacetFactory),
                typeof(RemoveIgnoredMethodsFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.TypeMarkerFacetFactory),
                // must be before any other FacetFactories that install MandatoryFacet.class facets
                typeof(NakedObjects.Reflector.FacetFactory.MandatoryDefaultFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PropertyValidateDefaultFacetFactory),
                typeof(ComplementaryMethodsFilteringFacetFactory),
                typeof(ActionMethodsFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.CollectionFieldMethodsFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PropertyMethodsFacetFactory),
                typeof(IconMethodFacetFactory),
                typeof(CallbackMethodsFacetFactory),
                typeof(TitleMethodFacetFactory),
                typeof(ValidateObjectFacetFactory),
                typeof(ComplexTypeAnnotationFacetFactory),
                typeof(ViewModelFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.BoundedAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.EnumFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.ActionDefaultAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PropertyDefaultAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.DescribedAsAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.DisabledAnnotationFacetFactory),
                typeof(PasswordAnnotationFacetFactory),
                typeof(DateOnlyFacetFactory),
                typeof(DataTypeAnnotationFacetFactory),
                typeof(ExecutedAnnotationFacetFactory),
                typeof(PotencyAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PageSizeAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.HiddenAnnotationFacetFactory),
                typeof(HiddenDefaultMethodFacetFactory),
                typeof(DisableDefaultMethodFacetFactory),
                typeof(AuthorizeAnnotationFacetFactory),
                typeof(ValidateProgrammaticUpdatesAnnotationFacetFactory),
                typeof(ImmutableAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.MaxLengthAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.RangeAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.MemberOrderAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.MultiLineAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.NamedAnnotationFacetFactory),
                typeof(NotPersistedAnnotationFacetFactory),
                typeof(ProgramPersistableOnlyAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.OptionalAnnotationFacetFactory),
                typeof(RequiredAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PluralAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.RegExAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.DefaultNamingFacetFactory), // must come after Named and Plural factories
                typeof(ConcurrencyCheckAnnotationFacetFactory),
                typeof(ContributedActionAnnotationFacetFactory),
                typeof(FinderActionFacetFactory),
                typeof(FindMenuFacetFactory),
                // must come after any facets that install titles
                typeof(NakedObjects.Reflector.FacetFactory.MaskAnnotationFacetFactory),
                // must come after any facets that install titles, and after mask
                // if takes precedence over mask.
                typeof(NakedObjects.Reflector.FacetFactory.TypeOfAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.TableViewAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.TypicalLengthDerivedFromTypeFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.TypicalLengthAnnotationFacetFactory),
                typeof(EagerlyAnnotationFacetFactory),
                typeof(NakedObjects.Reflector.FacetFactory.PresentationHintAnnotationFacetFactory),
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
                typeof(NakedObjects.Reflector.FacetFactory.MenuFacetFactory),

                typeof(NakedFunctions.Reflector.FacetFactory.FallbackFacetFactory),
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
                typeof(ViewModelAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.BoundedAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.MenuFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.TypeMarkerFacetFactory),
                // must be before any other FacetFactories that install MandatoryFacet.class facets
                typeof(NakedFunctions.Reflector.FacetFactory.MandatoryDefaultFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PropertyValidateDefaultFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.CollectionFieldMethodsFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PropertyMethodsFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.EnumFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.ActionDefaultAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PropertyDefaultAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.DescribedAsAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.DisabledAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PageSizeAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.HiddenAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.MaxLengthAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.RangeAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.MemberOrderAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.MultiLineAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.NamedAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.OptionalAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PluralAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.RegExAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.DefaultNamingFacetFactory), // must come after Named and Plural factories
                // must come after any facets that install titles
                typeof(NakedFunctions.Reflector.FacetFactory.MaskAnnotationFacetFactory),
                // must come after any facets that install titles, and after mask
                // if takes precedence over mask.
                typeof(NakedFunctions.Reflector.FacetFactory.TypeOfAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.TableViewAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.TypicalLengthDerivedFromTypeFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.TypicalLengthAnnotationFacetFactory),
                typeof(EagerlyAnnotationFacetFactory),
                typeof(NakedFunctions.Reflector.FacetFactory.PresentationHintAnnotationFacetFactory),
            };

        public static int StandardIndexOf(Type factory) => Array.FindIndex(StandardFacetFactories(), x => x == factory);
    }
}