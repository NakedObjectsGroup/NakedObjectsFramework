using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class MandatoryFacetTest {
        [TestMethod]
        public void TestMandatoryFacet() {
            IMandatoryFacet facet = new MandatoryFacet(null);
            Assert.IsTrue(facet.IsMandatory);
            Assert.IsFalse(facet.IsOptional);
            Assert.IsTrue(facet.IsRequiredButNull(null));
        }

        [TestMethod]
        public void TestMandatoryFacetDefault() {
            IMandatoryFacet facet = new MandatoryFacetDefault(null);
            Assert.IsTrue(facet.IsMandatory);
            Assert.IsFalse(facet.IsOptional);
            Assert.IsTrue(facet.IsRequiredButNull(null));
        }

        [TestMethod]
        public void TestOptionalFacet() {
            IMandatoryFacet facet = new OptionalFacet(null);
            Assert.IsFalse(facet.IsMandatory);
            Assert.IsTrue(facet.IsOptional);
            Assert.IsFalse(facet.IsRequiredButNull(null));
        }

        [TestMethod]
        public void TestOptionsFacetDefault() {
            IMandatoryFacet facet = new OptionalFacetDefault(null);
            Assert.IsFalse(facet.IsMandatory);
            Assert.IsTrue(facet.IsOptional);
            Assert.IsFalse(facet.IsRequiredButNull(null));
        }
    }
}