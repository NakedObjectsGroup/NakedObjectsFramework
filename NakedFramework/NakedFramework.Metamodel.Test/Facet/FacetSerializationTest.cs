using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.Facet;

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
}

public class TestSerializationClass {
    public int TestProperty { get; set; } = 1;
}