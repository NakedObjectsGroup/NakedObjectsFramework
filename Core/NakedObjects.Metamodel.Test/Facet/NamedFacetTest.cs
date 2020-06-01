using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class NamedFacetTest {
        [TestMethod]
        public void TestNamedFacetAnnotation() {
            const string testName = "a Name";
            INamedFacet facet = new NamedFacetAnnotation(testName, null);
            Assert.AreEqual(testName, facet.CapitalizedName);
            Assert.AreEqual(testName, facet.ShortName);
            Assert.AreEqual(testName, facet.SimpleName);
            Assert.AreEqual(testName, facet.NaturalName);
        }


        [TestMethod]
        public void TestNamedFacetInferred()
        {
            const string testName = "a nAme";
            INamedFacet facet = new NamedFacetInferred(testName, null);
            Assert.AreEqual("A nAme", facet.CapitalizedName);
            Assert.AreEqual("a nAme", facet.ShortName);
            Assert.AreEqual("aname", facet.SimpleName);
            Assert.AreEqual("A n Ame", facet.NaturalName);
        }

    }
}