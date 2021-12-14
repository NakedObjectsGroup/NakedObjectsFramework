// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Reflector.FacetFactory;

namespace NakedObjects.Reflector.Reflect;

public static class ObjectFacetFactories {
    public static Type[] StandardFacetFactories() =>
        new[] {
            typeof(RemoveDynamicProxyMethodsFacetFactory),
            typeof(RemoveEventHandlerMethodsFacetFactory),
            typeof(RemoveIgnoredMethodsFacetFactory),
            typeof(DomainObjectMethodFilteringFactory),
            typeof(DomainObjectPropertyFilteringFactory),
            typeof(FallbackFacetFactory),
            typeof(TypeMarkerFacetFactory),
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            typeof(MandatoryDefaultFacetFactory),
            typeof(PropertyValidateDefaultFacetFactory),
            typeof(ComplementaryMethodsFilteringFacetFactory),
            typeof(ActionMethodsFacetFactory),
            typeof(CollectionFieldMethodsFacetFactory),
            typeof(PropertyMethodsFacetFactory),
            typeof(CallbackMethodsFacetFactory),
            typeof(TitleMethodFacetFactory),
            typeof(ValidateObjectFacetFactory),
            typeof(ComplexTypeAnnotationFacetFactory),
            typeof(ViewModelFacetFactory),
            typeof(RedirectedObjectFacetFactory),
            typeof(BoundedAnnotationFacetFactory),
            typeof(EnumFacetFactory),
            typeof(ActionDefaultAnnotationFacetFactory),
            typeof(PropertyDefaultAnnotationFacetFactory),
            typeof(DescribedAsAnnotationFacetFactory),
            typeof(DisabledAnnotationFacetFactory),
            typeof(PasswordAnnotationFacetFactory),
            typeof(DateOnlyFacetFactory),
            typeof(DataTypeAnnotationFacetFactory),
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
            typeof(OptionalAnnotationFacetFactory),
            typeof(RequiredAnnotationFacetFactory),
            typeof(PluralAnnotationFacetFactory),
            typeof(RegExAnnotationFacetFactory),
            typeof(DefaultNamingFacetFactory), // must come after Named and Plural factories
            typeof(ConcurrencyCheckAnnotationFacetFactory),
            typeof(ContributedActionAnnotationFacetFactory),
            typeof(FinderActionFacetFactory),
            typeof(FindMenuFacetFactory),
            typeof(CreateNewAnnotationFacetFactory),
            typeof(EditAnnotationFacetFactory),
            typeof(DisplayAsPropertyAnnotationFacetFactory),
            // must come after any facets that install titles
            typeof(MaskAnnotationFacetFactory),
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            typeof(TypeOfAnnotationFacetFactory),
            typeof(TableViewAnnotationFacetFactory),
            typeof(EagerlyAnnotationFacetFactory),
            typeof(PresentationHintAnnotationFacetFactory),
            typeof(MenuFacetFactory)
        };
}