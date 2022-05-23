using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NakedFramework.Metamodel.Test.Serialization.SpecificationSerializationTests;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

[TestClass]
public class SpecificationXmlSerializationTests {
    [TestMethod]
    public void TestXmlSerializeIdentifierImpl() => TestSerializeIdentifierImpl(XmlRoundTripId);

    [TestMethod]
    public void TestXmlSerializeActionParameterSpecImmutable() => TestSerializeActionParameterSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeActionSpecImmutable() => TestSerializeActionSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeActionToAssociationSpecAdapter() => TestSerializeActionToAssociationSpecAdapter(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeActionToCollectionSpecAdapter() => TestSerializeActionToCollectionSpecAdapter(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeOneToManyAssociationSpecImmutable() => TestSerializeOneToManyAssociationSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeOneToOneAssociationSpecImmutable() => TestSerializeOneToOneAssociationSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeServiceSpecImmutable() => TestSerializeServiceSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeObjectSpecImmutable() => TestSerializeObjectSpecImmutable(XmlRoundTripSpec);

    [TestMethod]
    public void TestXmlSerializeMenuAction() => TestSerializeMenuAction(XmlRoundTripMenu);

    [TestMethod]
    public void TestXmlSerializeMenuImmutable() => TestSerializeMenuImmutable(XmlRoundTripMenu);
}