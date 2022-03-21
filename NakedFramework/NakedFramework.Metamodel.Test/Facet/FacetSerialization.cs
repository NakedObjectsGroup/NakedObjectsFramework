using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.Facet;

#pragma warning disable CS0618
#pragma warning disable SYSLIB0011

namespace NakedFramework.Metamodel.Test.Facet;

[TestClass]
public class FacetSerialization {
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

    private static T RoundTrip<T>(T facet) where T : IFacet {
        using var stream = BinarySerialize(facet);
        return (T)BinaryDeserialize(stream);
    }

    [TestMethod]
    public void TestSerializeActionChoicesFacetNone() {
        var f = ActionChoicesFacetNone.Instance;
        var dsf = RoundTrip(f);

        AssertIFacet(f, dsf);

        Assert.AreEqual(f.ParameterNamesAndTypes, dsf.ParameterNamesAndTypes);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
    }
}