// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.I18N;
using NakedFramework.Metamodel.SemanticsProvider;
using static NakedFramework.Metamodel.Test.Facet.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Facet;

public static class FacetSerializationTests {
    public static void TestSerializeActionChoicesFacetNone(Func<ActionChoicesFacetNone, ActionChoicesFacetNone> roundTripper) {
        var f = ActionChoicesFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.ParameterNamesAndTypes, dsf.ParameterNamesAndTypes);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
    }

    public static void TestSerializeActionDefaultsFacetAnnotation(Func<ActionDefaultsFacetAnnotation, ActionDefaultsFacetAnnotation> roundTripper) {
        var f = new ActionDefaultsFacetAnnotation(17, false);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetDefault(null, null), dsf.GetDefault(null, null));
    }

    public static void TestSerializeActionDefaultsFacetNone(Func<ActionDefaultsFacetNone, ActionDefaultsFacetNone> roundTripper) {
        var f = ActionDefaultsFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetDefault(null, null), dsf.GetDefault(null, null));
    }

    public static void TestSerializeActionDefaultsFacetViaProperty(Func<ActionDefaultsFacetViaProperty, ActionDefaultsFacetViaProperty> roundTripper) {
        var f = new ActionDefaultsFacetViaProperty(GetProperty(), null, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeAggregatedFacetAlways(Func<AggregatedFacetAlways, AggregatedFacetAlways> roundTripper) {
        var f = AggregatedFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeArrayFacet(Func<ArrayFacet, ArrayFacet> roundTripper) {
        var f = ArrayFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeAuthorizationDisableForSessionFacet(Func<AuthorizationDisableForSessionFacet, AuthorizationDisableForSessionFacet> roundTripper) {
        var f = new AuthorizationDisableForSessionFacet("r1,r2", "u1,u2");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(string.Join(',', f.Roles), string.Join(',', dsf.Roles));
        Assert.AreEqual(string.Join(',', f.Users), string.Join(',', dsf.Users));
    }

    public static void TestSerializeAuthorizationHideForSessionFacet(Func<AuthorizationHideForSessionFacet, AuthorizationHideForSessionFacet> roundTripper) {
        var f = new AuthorizationHideForSessionFacet("r1,r2", "u1,u2");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(string.Join(',', f.Roles), string.Join(',', dsf.Roles));
        Assert.AreEqual(string.Join(',', f.Users), string.Join(',', dsf.Users));
    }

    public static void TestSerializeBoundedFacet(Func<BoundedFacet, BoundedFacet> roundTripper) {
        var f = BoundedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeCollectionFacet(Func<CollectionFacet, CollectionFacet> roundTripper) {
        var f = CollectionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeComplexTypeFacet(Func<ComplexTypeFacet, ComplexTypeFacet> roundTripper) {
        var f = ComplexTypeFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeConcurrencyCheckFacet(Func<ConcurrencyCheckFacet, ConcurrencyCheckFacet> roundTripper) {
        var f = ConcurrencyCheckFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeContributedActionFacet(Func<ContributedActionFacet, ContributedActionFacet> roundTripper) {
        var f = ContributedActionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeCreatedCallbackFacetNull(Func<CreatedCallbackFacetNull, CreatedCallbackFacetNull> roundTripper) {
        var f = CreatedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeCreateNewFacet(Func<CreateNewFacet, CreateNewFacet> roundTripper) {
        var f = new CreateNewFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Type, dsf.Type);
    }

    public static void TestSerializeDataTypeFacet(Func<DataTypeFacetAnnotation, DataTypeFacetAnnotation> roundTripper) {
        static void AssertDataType(DataTypeFacetAnnotation f, DataTypeFacetAnnotation dsf) {
            Assert.AreEqual(f.DataType(), dsf.DataType());
            Assert.AreEqual(f.CustomDataType(), dsf.CustomDataType());
        }

        var f = new DataTypeFacetAnnotation(DataType.EmailAddress);
        var dsf = roundTripper(f);
        var f1 = new DataTypeFacetAnnotation("custom data type");
        var dsf1 = roundTripper(f1);

        AssertIFacet(f, dsf);
        AssertDataType(f, dsf);
        AssertIFacet(f1, dsf1);
        AssertDataType(f1, dsf1);
    }

    public static void TestSerializeDateOnlyFacet(Func<DateOnlyFacet, DateOnlyFacet> roundTripper) {
        var f = DateOnlyFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeDefaultedFacetUsingDefaultsProvider(Func<DefaultedFacetUsingDefaultsProvider<bool>, DefaultedFacetUsingDefaultsProvider<bool>> roundTripper) {
        var f = new DefaultedFacetUsingDefaultsProvider<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Default, dsf.Default);
    }

    public static void TestSerializeDescribedAsFacetAnnotation(Func<DescribedAsFacetAnnotation, DescribedAsFacetAnnotation> roundTripper) {
        var f = new DescribedAsFacetAnnotation("default value");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeDescribedAsFacetNone(Func<DescribedAsFacetNone, DescribedAsFacetNone> roundTripper) {
        var f = DescribedAsFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeDisabledFacetAnnotation(Func<DisabledFacetAnnotation, DisabledFacetAnnotation> roundTripper) {
        var f = new DisabledFacetAnnotation(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeDisabledFacetAlways(Func<DisabledFacetAlways, DisabledFacetAlways> roundTripper) {
        var f = DisabledFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeDisableForSessionFacetNone(Func<DisableForSessionFacetNone, DisableForSessionFacetNone> roundTripper) {
        var f = DisableForSessionFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.DisabledReason(null, null), dsf.DisabledReason(null, null));
    }

    public static void TestSerializeDisplayAsPropertyFacet(Func<DisplayAsPropertyFacet, DisplayAsPropertyFacet> roundTripper) {
        var f = new DisplayAsPropertyFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.ContributedTo, dsf.ContributedTo);
    }

    public static void TestSerializeEagerlyFacet(Func<EagerlyFacet, EagerlyFacet> roundTripper) {
        var f = EagerlyFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.What, dsf.What);
    }

    public static void TestSerializeEditPropertiesFacet(Func<EditPropertiesFacet, EditPropertiesFacet> roundTripper) {
        var f = new EditPropertiesFacet(new[] { "s1", "s2" });
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(string.Join(',', f.Properties), string.Join(',', dsf.Properties));
    }

    public static void TestSerializeElementTypeFacet(Func<ElementTypeFacet, ElementTypeFacet> roundTripper) {
        var f = new ElementTypeFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeEnumFacet(Func<EnumFacet, EnumFacet> roundTripper) {
        var f = new EnumFacet(typeof(TestEnum));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.TypeOfEnum, dsf.TypeOfEnum);
    }

    public static void TestSerializeFinderActionFacet(Func<FinderActionFacet, FinderActionFacet> roundTripper) {
        var f = new FinderActionFacet("action name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeFindMenuFacet(Func<FindMenuFacet, FindMenuFacet> roundTripper) {
        var f = FindMenuFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeFromStreamFacetUsingFromStream(Func<FromStreamFacetUsingFromStream, FromStreamFacetUsingFromStream> roundTripper) {
        var f = new FromStreamFacetUsingFromStream(ImageValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeGenericCollectionFacet(Func<GenericCollectionFacet, GenericCollectionFacet> roundTripper) {
        var f = GenericCollectionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeGenericCollectionSetFacet(Func<GenericCollectionSetFacet, GenericCollectionSetFacet> roundTripper) {
        var f = GenericCollectionSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeGenericIEnumerableFacet(Func<GenericIEnumerableFacet, GenericIEnumerableFacet> roundTripper) {
        var f = GenericIEnumerableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeGenericIEnumerableSetFacet(Func<GenericIEnumerableSetFacet, GenericIEnumerableSetFacet> roundTripper) {
        var f = GenericIEnumerableSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeGenericIQueryableFacet(Func<GenericIQueryableFacet, GenericIQueryableFacet> roundTripper) {
        var f = GenericIQueryableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeGenericIQueryableSetFacet(Func<GenericIQueryableSetFacet, GenericIQueryableSetFacet> roundTripper) {
        var f = GenericIQueryableSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    public static void TestSerializeHiddenFacet(Func<HiddenFacet, HiddenFacet> roundTripper) {
        var f = new HiddenFacet(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeHideForSessionFacetNone(Func<HideForSessionFacetNone, HideForSessionFacetNone> roundTripper) {
        var f = HideForSessionFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.HiddenReason(null, null), dsf.HiddenReason(null, null));
    }

    public static void TestSerializeIdempotentFacet(Func<IdempotentFacet, IdempotentFacet> roundTripper) {
        var f = IdempotentFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeImmutableFacetAlways(Func<ImmutableFacetAlways, ImmutableFacetAlways> roundTripper) {
        var f = ImmutableFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeImmutableFacetAnnotation(Func<ImmutableFacetAnnotation, ImmutableFacetAnnotation> roundTripper) {
        var f = new ImmutableFacetAnnotation(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeImmutableFacetNever(Func<ImmutableFacetNever, ImmutableFacetNever> roundTripper) {
        var f = ImmutableFacetNever.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeIsASetFacet(Func<IsASetFacet, IsASetFacet> roundTripper) {
        var f = IsASetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeMandatoryFacet(Func<MandatoryFacet, MandatoryFacet> roundTripper) {
        var f = MandatoryFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMandatory, dsf.IsMandatory);
    }

    public static void TestSerializeMandatoryFacetDefault(Func<MandatoryFacetDefault, MandatoryFacetDefault> roundTripper) {
        var f = MandatoryFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMandatory, dsf.IsMandatory);
    }

    public static void TestSerializeMaskFacet(Func<MaskFacet, MaskFacet> roundTripper) {
        var f = new MaskFacet("a mask");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeMaxLengthFacetAnnotation(Func<MaxLengthFacetAnnotation, MaxLengthFacetAnnotation> roundTripper) {
        var f = new MaxLengthFacetAnnotation(167);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeMaxLengthFacetZero(Func<MaxLengthFacetZero, MaxLengthFacetZero> roundTripper) {
        var f = MaxLengthFacetZero.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeMemberNamedFacetAnnotation(Func<MemberNamedFacetAnnotation, MemberNamedFacetAnnotation> roundTripper) {
        var f = new MemberNamedFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName(), dsf.FriendlyName());
    }

    public static void TestSerializeMemberNamedFacetInferred(Func<MemberNamedFacetInferred, MemberNamedFacetInferred> roundTripper) {
        var f = new MemberNamedFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName(), dsf.FriendlyName());
    }

    public static void TestSerializeMemberOrderFacet(Func<MemberOrderFacet, MemberOrderFacet> roundTripper) {
        var f = new MemberOrderFacet("name", "sequence", "group");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Name, dsf.Name);
        Assert.AreEqual(f.Sequence, dsf.Sequence);
        Assert.AreEqual(f.Grouping, dsf.Grouping);
    }

    public static void TestSerializeMenuFacetDefault(Func<MenuFacetDefault, MenuFacetDefault> roundTripper) {
        var f = new MenuFacetDefault();
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeMenuFacetViaMethod(Func<MenuFacetViaMethod, MenuFacetViaMethod> roundTripper) {
        var f = new MenuFacetViaMethod(GetMenuMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeMultiLineFacetAnnotation(Func<MultiLineFacetAnnotation, MultiLineFacetAnnotation> roundTripper) {
        var f = new MultiLineFacetAnnotation(10, 20);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.NumberOfLines, dsf.NumberOfLines);
        Assert.AreEqual(f.Width, dsf.Width);
    }

    public static void TestSerializeMultiLineFacetNone(Func<MultiLineFacetNone, MultiLineFacetNone> roundTripper) {
        var f = MultiLineFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.NumberOfLines, dsf.NumberOfLines);
        Assert.AreEqual(f.Width, dsf.Width);
    }

    public static void TestSerializeNamedFacetAnnotation(Func<NamedFacetAnnotation, NamedFacetAnnotation> roundTripper) {
        var f = new NamedFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName, dsf.FriendlyName);
    }

    public static void TestSerializeNamedFacetInferred(Func<NamedFacetInferred, NamedFacetInferred> roundTripper) {
        var f = new NamedFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName, dsf.FriendlyName);
    }

    public static void TestSerializeNotCountedFacet(Func<NotCountedFacet, NotCountedFacet> roundTripper) {
        var f = NotCountedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeNotNavigableFacet(Func<NotNavigableFacet, NotNavigableFacet> roundTripper) {
        var f = NotNavigableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeNotPersistedFacet(Func<NotPersistedFacet, NotPersistedFacet> roundTripper) {
        var f = NotPersistedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeNullableFacetAlways(Func<NullableFacetAlways, NullableFacetAlways> roundTripper) {
        var f = NullableFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeOnPersistingErrorCallbackFacetNull(Func<OnPersistingErrorCallbackFacetNull, OnPersistingErrorCallbackFacetNull> roundTripper) {
        var f = OnPersistingErrorCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeOnUpdatingErrorCallbackFacetNull(Func<OnUpdatingErrorCallbackFacetNull, OnUpdatingErrorCallbackFacetNull> roundTripper) {
        var f = OnUpdatingErrorCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeOptionalFacet(Func<OptionalFacet, OptionalFacet> roundTripper) {
        var f = OptionalFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsOptional, dsf.IsOptional);
    }

    public static void TestSerializeOptionalFacetDefault(Func<OptionalFacetDefault, OptionalFacetDefault> roundTripper) {
        var f = OptionalFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsOptional, dsf.IsOptional);
    }

    public static void TestSerializePageSizeFacetAnnotation(Func<PageSizeFacetAnnotation, PageSizeFacetAnnotation> roundTripper) {
        var f = new PageSizeFacetAnnotation(10);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializePageSizeFacetDefault(Func<PageSizeFacetDefault, PageSizeFacetDefault> roundTripper) {
        var f = PageSizeFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeParseableFacetUsingParser(Func<ParseableFacetUsingParser<bool>, ParseableFacetUsingParser<bool>> roundTripper) {
        var f = new ParseableFacetUsingParser<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializePasswordFacet(Func<PasswordFacet, PasswordFacet> roundTripper) {
        var f = PasswordFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializePluralFacetAnnotation(Func<PluralFacetAnnotation, PluralFacetAnnotation> roundTripper) {
        var f = new PluralFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializePluralFacetInferred(Func<PluralFacetInferred, PluralFacetInferred> roundTripper) {
        var f = new PluralFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializePresentationHintFacet(Func<PresentationHintFacet, PresentationHintFacet> roundTripper) {
        var f = new PresentationHintFacet("a hint");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializePropertyAccessorFacet(Func<PropertyAccessorFacet, PropertyAccessorFacet> roundTripper) {
        var f = new PropertyAccessorFacet(GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyDefaultFacetAnnotation(Func<PropertyDefaultFacetAnnotation, PropertyDefaultFacetAnnotation> roundTripper) {
        var f = new PropertyDefaultFacetAnnotation("default");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetDefault(null), dsf.GetDefault(null));
    }

    public static void TestSerializePropertyDefaultFacetNone(Func<PropertyDefaultFacetNone, PropertyDefaultFacetNone> roundTripper) {
        var f = PropertyDefaultFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetDefault(null), dsf.GetDefault(null));
    }

    public static void TestSerializePropertyInitializationFacet(Func<PropertyInitializationFacet, PropertyInitializationFacet> roundTripper) {
        var f = new PropertyInitializationFacet(GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertySetterFacetViaSetterMethod(Func<PropertySetterFacetViaSetterMethod, PropertySetterFacetViaSetterMethod> roundTripper) {
        var f = new PropertySetterFacetViaSetterMethod(GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyValidateFacetDefault(Func<PropertyValidateFacetDefault, PropertyValidateFacetDefault> roundTripper) {
        var f = PropertyValidateFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializePropertyValidateFacetNone(Func<PropertyValidateFacetNone, PropertyValidateFacetNone> roundTripper) {
        var f = PropertyValidateFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeQueryOnlyFacet(Func<QueryOnlyFacet, QueryOnlyFacet> roundTripper) {
        var f = QueryOnlyFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeRangeFacet(Func<RangeFacet, RangeFacet> roundTripper) {
        var f = new RangeFacet(1, 2, false);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Min, dsf.Min);
        Assert.AreEqual(f.Max, dsf.Max);
    }

    public static void TestSerializeRedirectedFacet(Func<RedirectedFacet, RedirectedFacet> roundTripper) {
        var f = new RedirectedFacet(GetProperty(), GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(0), dsf.GetMethod(0));
        Assert.AreEqual(f.GetMethodDelegate(0).GetType(), dsf.GetMethodDelegate(0).GetType());
        Assert.AreEqual(f.GetMethod(1), dsf.GetMethod(1));
        Assert.AreEqual(f.GetMethodDelegate(1).GetType(), dsf.GetMethodDelegate(1).GetType());
    }

    public static void TestSerializeRegExFacet(Func<RegExFacet, RegExFacet> roundTripper) {
        var f = new RegExFacet("d", "c", true, "");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Pattern.ToString(), dsf.Pattern.ToString());
        Assert.AreEqual(f.ValidationPattern, dsf.ValidationPattern);
        Assert.AreEqual(f.FailureMessage, dsf.FailureMessage);
        Assert.AreEqual(f.IsCaseSensitive, dsf.IsCaseSensitive);
    }

    public static void TestSerializeRestExtensionFacet(Func<RestExtensionFacet, RestExtensionFacet> roundTripper) {
        var f = new RestExtensionFacet("var1", "var2");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Name, dsf.Name);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeSaveFacet(Func<SaveFacet, SaveFacet> roundTripper) {
        var f = SaveFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTableViewFacet(Func<TableViewFacet, TableViewFacet> roundTripper) {
        var f = new TableViewFacet(true, new[] { "col1", "col2" });
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Title, dsf.Title);
        Assert.AreEqual(string.Join(',', f.Columns), string.Join(',', dsf.Columns));
    }

    public static void TestSerializeTitleFacetNone(Func<TitleFacetNone, TitleFacetNone> roundTripper) {
        var f = TitleFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTitleFacetUsingParser(Func<TitleFacetUsingParser<bool>, TitleFacetUsingParser<bool>> roundTripper) {
        var f = new TitleFacetUsingParser<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTitleFacetViaToStringMethod(Func<TitleFacetViaToStringMethod, TitleFacetViaToStringMethod> roundTripper) {
        var f = TitleFacetViaToStringMethod.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTitleFacetViaMaskedToStringMethod(Func<TitleFacetViaMaskedToStringMethod, TitleFacetViaMaskedToStringMethod> roundTripper) {
        var f = new TitleFacetViaMaskedToStringMethod(typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.ToString), new[] { typeof(string) }), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate()?.GetType(), dsf.GetMethodDelegate()?.GetType());
    }

    public static void TestSerializeTypeFacet(Func<TypeFacet, TypeFacet> roundTripper) {
        var f = new TypeFacet(typeof(TestEnum));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.TypeOrUnderlyingType, dsf.TypeOrUnderlyingType);
    }

    public static void TestSerializeTypeIsAbstractFacet(Func<TypeIsAbstractFacet, TypeIsAbstractFacet> roundTripper) {
        var f = TypeIsAbstractFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeIsInterfaceFacet(Func<TypeIsInterfaceFacet, TypeIsInterfaceFacet> roundTripper) {
        var f = TypeIsInterfaceFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeIsSealedFacet(Func<TypeIsSealedFacet, TypeIsSealedFacet> roundTripper) {
        var f = TypeIsSealedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeIsStaticFacet(Func<TypeIsStaticFacet, TypeIsStaticFacet> roundTripper) {
        var f = TypeIsStaticFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeIsVoidFacet(Func<TypeIsVoidFacet, TypeIsVoidFacet> roundTripper) {
        var f = TypeIsVoidFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeOfFacetDefaultToObject(Func<TypeOfFacetDefaultToObject, TypeOfFacetDefaultToObject> roundTripper) {
        var f = TypeOfFacetDefaultToObject.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetValue(null), dsf.GetValue(null));
    }

    public static void TestSerializeTypeOfFacetInferredFromArray(Func<TypeOfFacetInferredFromArray, TypeOfFacetInferredFromArray> roundTripper) {
        var f = TypeOfFacetInferredFromArray.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTypeOfFacetInferredFromGenerics(Func<TypeOfFacetInferredFromGenerics, TypeOfFacetInferredFromGenerics> roundTripper) {
        var f = TypeOfFacetInferredFromGenerics.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeValidateObjectFacet(Func<ValidateObjectFacet, ValidateObjectFacet> roundTripper) {
        var f = new ValidateObjectFacet(new[] { typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.ValidateTest)) }, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(0), dsf.GetMethod(0));
        Assert.AreEqual(f.GetMethodDelegate(0).GetType(), dsf.GetMethodDelegate(0).GetType());
    }

    public static void TestSerializeValidateObjectFacetNull(Func<ValidateObjectFacetNull, ValidateObjectFacetNull> roundTripper) {
        var f = ValidateObjectFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeValidateProgrammaticUpdatesFacetAnnotation(Func<ValidateProgrammaticUpdatesFacetAnnotation, ValidateProgrammaticUpdatesFacetAnnotation> roundTripper) {
        var f = ValidateProgrammaticUpdatesFacetAnnotation.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeValueFacet(Func<ValueFacet, ValueFacet> roundTripper) {
        var f = ValueFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeValueFacetFromSemanticProvider(Func<ValueFacetFromSemanticProvider<bool>, ValueFacetFromSemanticProvider<bool>> roundTripper) {
        var f = new ValueFacetFromSemanticProvider<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeDescribedAsFacetI18N(Func<DescribedAsFacetI18N, DescribedAsFacetI18N> roundTripper) {
        var f = new DescribedAsFacetI18N("default value");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    public static void TestSerializeMemberNamedFacetI18N(Func<MemberNamedFacetI18N, MemberNamedFacetI18N> roundTripper) {
        var f = new MemberNamedFacetI18N("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName(), dsf.FriendlyName());
    }
}