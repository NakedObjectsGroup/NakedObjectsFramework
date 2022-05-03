using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedFramework.Metamodel.Test.Serialization.SpecificationSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

[TestClass]
public class SpecificationBinarySerializationTests {
    [TestMethod]
    public void TestBinarySerializeSerializeServiceSpecImmutable() => TestSerializeServiceSpecImmutable(BinaryRoundTripSpec);
}