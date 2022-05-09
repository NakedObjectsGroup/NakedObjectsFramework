using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedFramework.Metamodel.Test.Serialization.SpecificationSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

[TestClass]
public class SpecificationBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeIdentifierImpl() => TestSerializeIdentifierImpl(BinaryRoundTripId);

    [TestMethod]
    public void TestBinarySerializeActionParameterSpecImmutable() => TestSerializeActionParameterSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeActionSpecImmutable() => TestSerializeActionSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeActionToAssociationSpecAdapter() => TestSerializeActionToAssociationSpecAdapter(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeActionToCollectionSpecAdapter() => TestSerializeActionToCollectionSpecAdapter(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeOneToManyAssociationSpecImmutable() => TestSerializeOneToManyAssociationSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeOneToOneAssociationSpecImmutable() => TestSerializeOneToOneAssociationSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeServiceSpecImmutable() => TestSerializeServiceSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeObjectSpecImmutable() => TestSerializeObjectSpecImmutable(BinaryRoundTripSpec);

    [TestMethod]
    public void TestBinarySerializeMenuAction() => TestSerializeMenuAction(BinaryRoundTripMenu);

    [TestMethod]
    public void TestBinarySerializeMenuImmutable() => TestSerializeMenuImmutable(BinaryRoundTripMenu);

    [TestMethod]
    public void TestBinarySerializeImmutableInMemorySpecCache() => TestSerializeImmutableInMemorySpecCache(BinaryRoundTripCache);
}