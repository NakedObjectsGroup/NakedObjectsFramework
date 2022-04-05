using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

#pragma warning disable CS0618
#pragma warning disable SYSLIB0011

namespace NakedFramework.Metamodel.Test.Facet;

[TestClass]
public class FacetSerializationTest {
    private static void AssertIFacet(IFacet facet, IFacet facet1) {
        Assert.AreNotEqual(facet, facet1);
        Assert.AreEqual(facet.GetType(), facet1.GetType());
        Assert.AreEqual(facet.IsNoOp, facet1.IsNoOp);
        Assert.AreEqual(facet.FacetType, facet1.FacetType);
        Assert.AreEqual(facet.CanAlwaysReplace, facet1.CanAlwaysReplace);
        Assert.AreEqual(facet.CanNeverBeReplaced, facet1.CanNeverBeReplaced);
    }

    private static Stream BinarySerialize(IFacet facet) {
        Stream memoryStream = new MemoryStream();
        var serializer = new BinaryFormatter();
        serializer.Serialize(memoryStream, facet);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static IFacet BinaryDeserialize(Stream stream) {
        var deserializer = new BinaryFormatter();
        return (IFacet)deserializer.Deserialize(stream);
    }

    private static T BinaryRoundTrip<T>(T facet) where T : IFacet {
        using var stream = BinarySerialize(facet);
        return (T)BinaryDeserialize(stream);
    }

    private static PropertyInfo GetProperty() => typeof(TestSerializationClass).GetProperty(nameof(TestSerializationClass.TestProperty));

    private static MethodInfo GetMenuMethod() => typeof(TestMenuClass).GetMethod(nameof(TestMenuClass.Menu));

    private static void TestSerializeActionChoicesFacetNone(Func<ActionChoicesFacetNone, ActionChoicesFacetNone> roundTripper) {
        var f = ActionChoicesFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.ParameterNamesAndTypes, dsf.ParameterNamesAndTypes);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
    }

    private static void TestSerializeActionDefaultsFacetAnnotation(Func<ActionDefaultsFacetAnnotation, ActionDefaultsFacetAnnotation> roundTripper) {
        var f = new ActionDefaultsFacetAnnotation(17, false);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetDefault(null, null), dsf.GetDefault(null, null));
    }

    private static void TestSerializeActionDefaultsFacetNone(Func<ActionDefaultsFacetNone, ActionDefaultsFacetNone> roundTripper) {
        var f = ActionDefaultsFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetDefault(null, null), dsf.GetDefault(null, null));
    }

    private static void TestSerializeActionDefaultsFacetViaProperty(Func<ActionDefaultsFacetViaProperty, ActionDefaultsFacetViaProperty> roundTripper) {
        var f = new ActionDefaultsFacetViaProperty(GetProperty(), null, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    private static void TestSerializeAggregatedFacetAlways(Func<AggregatedFacetAlways, AggregatedFacetAlways> roundTripper) {
        var f = AggregatedFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeArrayFacet(Func<ArrayFacet, ArrayFacet> roundTripper) {
        var f = ArrayFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeAuthorizationDisableForSessionFacet(Func<AuthorizationDisableForSessionFacet, AuthorizationDisableForSessionFacet> roundTripper) {
        var f = new AuthorizationDisableForSessionFacet("r1,r2", "u1,u2");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(string.Join(',', f.Roles), string.Join(',', dsf.Roles));
        Assert.AreEqual(string.Join(',', f.Users), string.Join(',', dsf.Users));
    }

    private static void TestSerializeAuthorizationHideForSessionFacet(Func<AuthorizationHideForSessionFacet, AuthorizationHideForSessionFacet> roundTripper) {
        var f = new AuthorizationHideForSessionFacet("r1,r2", "u1,u2");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(string.Join(',', f.Roles), string.Join(',', dsf.Roles));
        Assert.AreEqual(string.Join(',', f.Users), string.Join(',', dsf.Users));
    }

    private static void TestSerializeBoundedFacet(Func<BoundedFacet, BoundedFacet> roundTripper) {
        var f = BoundedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeCollectionFacet(Func<CollectionFacet, CollectionFacet> roundTripper) {
        var f = CollectionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeComplexTypeFacet(Func<ComplexTypeFacet, ComplexTypeFacet> roundTripper) {
        var f = ComplexTypeFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeConcurrencyCheckFacet(Func<ConcurrencyCheckFacet, ConcurrencyCheckFacet> roundTripper) {
        var f = ConcurrencyCheckFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeContributedActionFacet(Func<ContributedActionFacet, ContributedActionFacet> roundTripper) {
        var f = ContributedActionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeCreatedCallbackFacetNull(Func<CreatedCallbackFacetNull, CreatedCallbackFacetNull> roundTripper) {
        var f = CreatedCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeCreateNewFacet(Func<CreateNewFacet, CreateNewFacet> roundTripper) {
        var f = new CreateNewFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Type, dsf.Type);
    }

    private static void TestSerializeDataTypeFacet(Func<DataTypeFacetAnnotation, DataTypeFacetAnnotation> roundTripper) {
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

    private static void TestSerializeDateOnlyFacet(Func<DateOnlyFacet, DateOnlyFacet> roundTripper) {
        var f = DateOnlyFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeDefaultedFacetUsingDefaultsProvider(Func<DefaultedFacetUsingDefaultsProvider<bool>, DefaultedFacetUsingDefaultsProvider<bool>> roundTripper) {
        var f = new DefaultedFacetUsingDefaultsProvider<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Default, dsf.Default);
    }

    private static void TestSerializeDescribedAsFacetAnnotation(Func<DescribedAsFacetAnnotation, DescribedAsFacetAnnotation> roundTripper) {
        var f = new DescribedAsFacetAnnotation("default value");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeDescribedAsFacetNone(Func<DescribedAsFacetNone, DescribedAsFacetNone> roundTripper) {
        var f = DescribedAsFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeDisabledFacetAnnotation(Func<DisabledFacetAnnotation, DisabledFacetAnnotation> roundTripper) {
        var f = new DisabledFacetAnnotation(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeDisabledFacetAlways(Func<DisabledFacetAlways, DisabledFacetAlways> roundTripper) {
        var f = DisabledFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeDisableForSessionFacetNone(Func<DisableForSessionFacetNone, DisableForSessionFacetNone> roundTripper) {
        var f = DisableForSessionFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.DisabledReason(null, null), dsf.DisabledReason(null, null));
    }

    private static void TestSerializeDisplayAsPropertyFacet(Func<DisplayAsPropertyFacet, DisplayAsPropertyFacet> roundTripper) {
        var f = new DisplayAsPropertyFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.ContributedTo, dsf.ContributedTo);
    }

    private static void TestSerializeEagerlyFacet(Func<EagerlyFacet, EagerlyFacet> roundTripper) {
        var f = EagerlyFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.What, dsf.What);
    }

    private static void TestSerializeEditPropertiesFacet(Func<EditPropertiesFacet, EditPropertiesFacet> roundTripper) {
        var f = new EditPropertiesFacet(new[] { "s1", "s2" });
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(string.Join(',', f.Properties), string.Join(',', dsf.Properties));
    }

    private static void TestSerializeElementTypeFacet(Func<ElementTypeFacet, ElementTypeFacet> roundTripper) {
        var f = new ElementTypeFacet(typeof(TestSerializationClass));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeEnumFacet(Func<EnumFacet, EnumFacet> roundTripper) {
        var f = new EnumFacet(typeof(TestEnum));
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.TypeOfEnum, dsf.TypeOfEnum);
    }

    private static void TestSerializeFinderActionFacet(Func<FinderActionFacet, FinderActionFacet> roundTripper) {
        var f = new FinderActionFacet("action name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeFindMenuFacet(Func<FindMenuFacet, FindMenuFacet> roundTripper) {
        var f = FindMenuFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeFromStreamFacetUsingFromStream(Func<FromStreamFacetUsingFromStream, FromStreamFacetUsingFromStream> roundTripper) {
        var f = new FromStreamFacetUsingFromStream(ImageValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeGenericCollectionFacet(Func<GenericCollectionFacet, GenericCollectionFacet> roundTripper) {
        var f = GenericCollectionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeGenericCollectionSetFacet(Func<GenericCollectionSetFacet, GenericCollectionSetFacet> roundTripper) {
        var f = GenericCollectionSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeGenericIEnumerableFacet(Func<GenericIEnumerableFacet, GenericIEnumerableFacet> roundTripper) {
        var f = GenericIEnumerableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeGenericIEnumerableSetFacet(Func<GenericIEnumerableSetFacet, GenericIEnumerableSetFacet> roundTripper) {
        var f = GenericIEnumerableSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeGenericIQueryableFacet(Func<GenericIQueryableFacet, GenericIQueryableFacet> roundTripper) {
        var f = GenericIQueryableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeGenericIQueryableSetFacet(Func<GenericIQueryableSetFacet, GenericIQueryableSetFacet> roundTripper) {
        var f = GenericIQueryableSetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsQueryable, dsf.IsQueryable);
        Assert.AreEqual(f.IsASet, dsf.IsASet);
    }

    private static void TestSerializeHiddenFacet(Func<HiddenFacet, HiddenFacet> roundTripper) {
        var f = new HiddenFacet(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeHideForSessionFacetNone(Func<HideForSessionFacetNone, HideForSessionFacetNone> roundTripper) {
        var f = HideForSessionFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.HiddenReason(null, null), dsf.HiddenReason(null, null));
    }

    private static void TestSerializeIdempotentFacet(Func<IdempotentFacet, IdempotentFacet> roundTripper) {
        var f = IdempotentFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeImmutableFacetAlways(Func<ImmutableFacetAlways, ImmutableFacetAlways> roundTripper) {
        var f = ImmutableFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeImmutableFacetAnnotation(Func<ImmutableFacetAnnotation, ImmutableFacetAnnotation> roundTripper) {
        var f = new ImmutableFacetAnnotation(WhenTo.OncePersisted);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeImmutableFacetNever(Func<ImmutableFacetNever, ImmutableFacetNever> roundTripper) {
        var f = ImmutableFacetNever.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeIsASetFacet(Func<IsASetFacet, IsASetFacet> roundTripper) {
        var f = IsASetFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeMandatoryFacet(Func<MandatoryFacet, MandatoryFacet> roundTripper) {
        var f = MandatoryFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMandatory, dsf.IsMandatory);
    }

    private static void TestSerializeMandatoryFacetDefault(Func<MandatoryFacetDefault, MandatoryFacetDefault> roundTripper) {
        var f = MandatoryFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMandatory, dsf.IsMandatory);
    }

    private static void TestSerializeMaskFacet(Func<MaskFacet, MaskFacet> roundTripper) {
        var f = new MaskFacet("a mask");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeMaxLengthFacetAnnotation(Func<MaxLengthFacetAnnotation, MaxLengthFacetAnnotation> roundTripper) {
        var f = new MaxLengthFacetAnnotation(167);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeMaxLengthFacetZero(Func<MaxLengthFacetZero, MaxLengthFacetZero> roundTripper) {
        var f = MaxLengthFacetZero.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeMemberNamedFacetAnnotation(Func<MemberNamedFacetAnnotation, MemberNamedFacetAnnotation> roundTripper) {
        var f = new MemberNamedFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName(), dsf.FriendlyName());
    }

    private static void TestSerializeMemberNamedFacetInferred(Func<MemberNamedFacetInferred, MemberNamedFacetInferred> roundTripper) {
        var f = new MemberNamedFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName(), dsf.FriendlyName());
    }

    private static void TestSerializeMemberOrderFacet(Func<MemberOrderFacet, MemberOrderFacet> roundTripper) {
        var f = new MemberOrderFacet("name", "sequence", "group");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Name, dsf.Name);
        Assert.AreEqual(f.Sequence, dsf.Sequence);
        Assert.AreEqual(f.Grouping, dsf.Grouping);
    }

    private static void TestSerializeMenuFacetDefault(Func<MenuFacetDefault, MenuFacetDefault> roundTripper) {
        var f = new MenuFacetDefault();
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeMenuFacetViaMethod(Func<MenuFacetViaMethod, MenuFacetViaMethod> roundTripper) {
        var f = new MenuFacetViaMethod(GetMenuMethod(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    private static void TestSerializeMultiLineFacetAnnotation(Func<MultiLineFacetAnnotation, MultiLineFacetAnnotation> roundTripper) {
        var f = new MultiLineFacetAnnotation(10, 20);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.NumberOfLines, dsf.NumberOfLines);
        Assert.AreEqual(f.Width, dsf.Width);
    }

    private static void TestSerializeMultiLineFacetNone(Func<MultiLineFacetNone, MultiLineFacetNone> roundTripper) {
        var f = MultiLineFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.NumberOfLines, dsf.NumberOfLines);
        Assert.AreEqual(f.Width, dsf.Width);
    }

    private static void TestSerializeNamedFacetAnnotation(Func<NamedFacetAnnotation, NamedFacetAnnotation> roundTripper) {
        var f = new NamedFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName, dsf.FriendlyName);
    }

    private static void TestSerializeNamedFacetInferred(Func<NamedFacetInferred, NamedFacetInferred> roundTripper) {
        var f = new NamedFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
        Assert.AreEqual(f.FriendlyName, dsf.FriendlyName);
    }

    private static void TestSerializeNotCountedFacet(Func<NotCountedFacet, NotCountedFacet> roundTripper) {
        var f = NotCountedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeNotNavigableFacet(Func<NotNavigableFacet, NotNavigableFacet> roundTripper) {
        var f = NotNavigableFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeNotPersistedFacet(Func<NotPersistedFacet, NotPersistedFacet> roundTripper) {
        var f = NotPersistedFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeNullableFacetAlways(Func<NullableFacetAlways, NullableFacetAlways> roundTripper) {
        var f = NullableFacetAlways.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeOnPersistingErrorCallbackFacetNull(Func<OnPersistingErrorCallbackFacetNull, OnPersistingErrorCallbackFacetNull> roundTripper) {
        var f = OnPersistingErrorCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeOnUpdatingErrorCallbackFacetNull(Func<OnUpdatingErrorCallbackFacetNull, OnUpdatingErrorCallbackFacetNull> roundTripper) {
        var f = OnUpdatingErrorCallbackFacetNull.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializeOptionalFacet(Func<OptionalFacet, OptionalFacet> roundTripper) {
        var f = OptionalFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsOptional, dsf.IsOptional);
    }

    private static void TestSerializeOptionalFacetDefault(Func<OptionalFacetDefault, OptionalFacetDefault> roundTripper) {
        var f = OptionalFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsOptional, dsf.IsOptional);
    }

    private static void TestSerializePageSizeFacetAnnotation(Func<PageSizeFacetAnnotation, PageSizeFacetAnnotation> roundTripper) {
        var f = new PageSizeFacetAnnotation(10);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializePageSizeFacetDefault(Func<PageSizeFacetDefault, PageSizeFacetDefault> roundTripper) {
        var f = PageSizeFacetDefault.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializeParseableFacetUsingParser(Func<ParseableFacetUsingParser<bool>, ParseableFacetUsingParser<bool>> roundTripper) {
        var f = new ParseableFacetUsingParser<bool>(BooleanValueSemanticsProvider.Instance);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializePasswordFacet(Func<PasswordFacet, PasswordFacet> roundTripper) {
        var f = PasswordFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    private static void TestSerializePluralFacetAnnotation(Func<PluralFacetAnnotation, PluralFacetAnnotation> roundTripper) {
        var f = new PluralFacetAnnotation("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializePluralFacetInferred(Func<PluralFacetInferred, PluralFacetInferred> roundTripper) {
        var f = new PluralFacetInferred("a name");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializePresentationHintFacet(Func<PresentationHintFacet, PresentationHintFacet> roundTripper) {
        var f = new PresentationHintFacet("a hint");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.Value, dsf.Value);
    }

    private static void TestSerializePropertyAccessorFacet(Func<PropertyAccessorFacet, PropertyAccessorFacet> roundTripper) {
        var f = new PropertyAccessorFacet(GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    private static void TestSerializePropertyDefaultFacetAnnotation(Func<PropertyDefaultFacetAnnotation, PropertyDefaultFacetAnnotation> roundTripper) {
        var f = new PropertyDefaultFacetAnnotation("default");
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetDefault(null), dsf.GetDefault(null));
    }

    private static void TestSerializePropertyDefaultFacetNone(Func<PropertyDefaultFacetNone, PropertyDefaultFacetNone> roundTripper) {
        var f = PropertyDefaultFacetNone.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetDefault(null), dsf.GetDefault(null));
    }

    private static void TestSerializePropertyInitializationFacet(Func<PropertyInitializationFacet, PropertyInitializationFacet> roundTripper) {
        var f = new PropertyInitializationFacet(GetProperty(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    [TestMethod]
    public void TestBinarySerializeActionChoicesFacetNone() => TestSerializeActionChoicesFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetAnnotation() => TestSerializeActionDefaultsFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetNone() => TestSerializeActionDefaultsFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeActionDefaultsFacetViaProperty() => TestSerializeActionDefaultsFacetViaProperty(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAggregatedFacetAlways() => TestSerializeAggregatedFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeArrayFacet() => TestSerializeArrayFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationDisableForSessionFacet() => TestSerializeAuthorizationDisableForSessionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeAuthorizationHideForSessionFacet() => TestSerializeAuthorizationHideForSessionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeBoundedFacet() => TestSerializeBoundedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCollectionFacet() => TestSerializeCollectionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeComplexTypeFacet() => TestSerializeComplexTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeConcurrencyCheckFacet() => TestSerializeConcurrencyCheckFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeContributedActionFacet() => TestSerializeContributedActionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCreatedCallbackFacetNull() => TestSerializeCreatedCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeCreateNewFacet() => TestSerializeCreateNewFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDataTypeFacet() => TestSerializeDataTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDateOnlyFacet() => TestSerializeDateOnlyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDefaultedFacetUsingDefaultsProvider() => TestSerializeDefaultedFacetUsingDefaultsProvider(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsFacetAnnotation() => TestSerializeDescribedAsFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDescribedAsFacetNone() => TestSerializeDescribedAsFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisabledFacetAnnotation() => TestSerializeDisabledFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisabledFacetAlways() => TestSerializeDisabledFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisableForSessionFacetNone() => TestSerializeDisableForSessionFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeDisplayAsPropertyFacet() => TestSerializeDisplayAsPropertyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEagerlyFacet() => TestSerializeEagerlyFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEditPropertiesFacet() => TestSerializeEditPropertiesFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeElementTypeFacet() => TestSerializeElementTypeFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeEnumFacet() => TestSerializeEnumFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFinderActionFacet() => TestSerializeFinderActionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFindMenuFacet() => TestSerializeFindMenuFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeFromStreamFacetUsingFromStream() => TestSerializeFromStreamFacetUsingFromStream(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericCollectionFacet() => TestSerializeGenericCollectionFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericCollectionSetFacet() => TestSerializeGenericCollectionSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIEnumerableFacet() => TestSerializeGenericIEnumerableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIEnumerableSetFacet() => TestSerializeGenericIEnumerableSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIQueryableFacet() => TestSerializeGenericIQueryableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeGenericIQueryableSetFacet() => TestSerializeGenericIQueryableSetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHiddenFacet() => TestSerializeHiddenFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeHideForSessionFacetNone() => TestSerializeHideForSessionFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeIdempotentFacet() => TestSerializeIdempotentFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetAlways() => TestSerializeImmutableFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetAnnotation() => TestSerializeImmutableFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeImmutableFacetNever() => TestSerializeImmutableFacetNever(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeIsASetFacet() => TestSerializeIsASetFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMandatoryFacet() => TestSerializeMandatoryFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMandatoryFacetDefault() => TestSerializeMandatoryFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaskFacet() => TestSerializeMaskFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaxLengthFacetAnnotation() => TestSerializeMaxLengthFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMaxLengthFacetZero() => TestSerializeMaxLengthFacetZero(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedFacetAnnotation() => TestSerializeMemberNamedFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberNamedFacetInferred() => TestSerializeMemberNamedFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMemberOrderFacet() => TestSerializeMemberOrderFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMenuFacetDefault() => TestSerializeMenuFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMenuFacetViaMethod() => TestSerializeMenuFacetViaMethod(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMultiLineFacetAnnotation() => TestSerializeMultiLineFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeMultiLineFacetNone() => TestSerializeMultiLineFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNamedFacetAnnotation() => TestSerializeNamedFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNamedFacetInferred() => TestSerializeNamedFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotCountedFacet() => TestSerializeNotCountedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotNavigableFacet() => TestSerializeNotNavigableFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNotPersistedFacet() => TestSerializeNotPersistedFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeNullableFacetAlways() => TestSerializeNullableFacetAlways(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnPersistingErrorCallbackFacetNull() => TestSerializeOnPersistingErrorCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOnUpdatingErrorCallbackFacetNull() => TestSerializeOnUpdatingErrorCallbackFacetNull(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOptionalFacet() => TestSerializeOptionalFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeOptionalFacetDefault() => TestSerializeOptionalFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePageSizeFacetAnnotation() => TestSerializePageSizeFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePageSizeFacetDefault() => TestSerializePageSizeFacetDefault(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializeParseableFacetUsingParser() => TestSerializeParseableFacetUsingParser(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePasswordFacet() => TestSerializePasswordFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePluralFacetAnnotation() => TestSerializePluralFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePluralFacetInferred() => TestSerializePluralFacetInferred(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePresentationHintFacet() => TestSerializePresentationHintFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyAccessorFacet() => TestSerializePropertyAccessorFacet(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyDefaultFacetAnnotation() => TestSerializePropertyDefaultFacetAnnotation(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyDefaultFacetNone() => TestSerializePropertyDefaultFacetNone(BinaryRoundTrip);

    [TestMethod]
    public void TestBinarySerializePropertyInitializationFacet() => TestSerializePropertyInitializationFacet(BinaryRoundTrip);
}

public class TestSerializationClass {
    public int TestProperty { get; set; } = 1;
}

public class TestMenuClass {
    public static void Menu(IMenu menu) { }
}

public enum TestEnum {
    EOne,
    ETwo
}