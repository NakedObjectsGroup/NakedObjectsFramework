// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions.Reflector.FacetFactory;

namespace NakedFunctions.Reflector.Reflect {
    public static class FunctionalFacetFactories {
        public static Type[] StandardFacetFactories() =>
            new[] {
                typeof(FallbackFacetFactory),
                typeof(FunctionsFacetFactory),
                typeof(RecordFacetFactory),
                typeof(ContributedFunctionFacetFactory),
                typeof(InjectedParameterFacetFactory),
                typeof(InjectedAnnotationFacetFactory),
                typeof(TitleToStringMethodFacetFactory),
                typeof(DisableFunctionFacetFactory),
                typeof(ActionValidateViaFunctionFacetFactory),
                typeof(HideFunctionFacetFactory),
                typeof(ActionDefaultViaFunctionFacetFactory),
                typeof(ActionChoicesViaFunctionFacetFactory),
                typeof(AutocompleteViaFunctionFacetFactory),
                typeof(PotencyDerivedFromSignatureFacetFactory),
                typeof(ViewModelAnnotationFacetFactory),
                typeof(BoundedAnnotationFacetFactory),
                typeof(MenuFacetFactory),
                typeof(TypeMarkerFacetFactory),
                // must be before any other FacetFactories that install MandatoryFacet.class facets
                typeof(MandatoryDefaultFacetFactory),
                typeof(PropertyValidateDefaultFacetFactory),
                typeof(CollectionFieldMethodsFacetFactory),
                typeof(PropertyMethodsFacetFactory),
                typeof(EnumFacetFactory),
                typeof(ActionDefaultAnnotationFacetFactory),
                typeof(PropertyDefaultAnnotationFacetFactory),
                typeof(DescribedAsAnnotationFacetFactory),
                typeof(PageSizeAnnotationFacetFactory),
                typeof(PasswordAnnotationFacetFactory),
                typeof(VersionedAnnotationFacetFactory),
                typeof(RenderEagerlyAnnotationFacetFactory),
                typeof(PluralAnnotationFacetFactory),
                typeof(HiddenAnnotationFacetFactory),
                typeof(MaxLengthAnnotationFacetFactory),
                typeof(RangeAnnotationFacetFactory),
                typeof(MemberOrderAnnotationFacetFactory),
                typeof(MultiLineAnnotationFacetFactory),
                typeof(NamedAnnotationFacetFactory),
                typeof(OptionalAnnotationFacetFactory),
                typeof(RegExAnnotationFacetFactory),
                typeof(CreateNewAnnotationFacetFactory),
                typeof(DisplayAsPropertyAnnotationFacetFactory),
                typeof(SystemClassMethodFilteringFactory),
                typeof(SystemClassPropertyFilteringFactory),
                typeof(DefaultNamingFacetFactory), // must come after Named and Plural factories
                // must come after any facets that install titles
                typeof(MaskAnnotationFacetFactory),
                // must come after any facets that install titles, and after mask
                // if takes precedence over mask.
                typeof(TypeOfAnnotationFacetFactory),
                typeof(TableViewAnnotationFacetFactory),
                typeof(PresentationHintAnnotationFacetFactory),
                typeof(EditAnnotationFacetFactory)
            };
    }
}