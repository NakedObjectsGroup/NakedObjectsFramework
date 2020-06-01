using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class RegexFacetTest {
        [TestMethod]
        public void TestRegexFacet() {
            IRegExFacet facet = new RegExFacet(@"\d", "", false, "", null);

            Assert.IsTrue(facet.DoesNotMatch("a"));
            Assert.IsFalse(facet.DoesNotMatch("1"));
        }
    }
}