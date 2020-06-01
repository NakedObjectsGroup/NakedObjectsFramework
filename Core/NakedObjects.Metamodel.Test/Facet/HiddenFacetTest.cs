using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class HiddenFacetTest {
    

        [TestMethod]
        public void TestHiddenFacetAnnotationAlways() {
            IHiddenFacet facet = new HiddenFacet(WhenTo.Always, null);
            Assert.AreEqual(Resources.NakedObjects.AlwaysHidden, facet.HiddenReason(null));
        }

        [TestMethod]
        public void TestHiddenFacetAnnotationNever() {
            IHiddenFacet facet = new HiddenFacet(WhenTo.Never, null);
            Assert.IsNull(facet.HiddenReason(null));
        }
    }
}