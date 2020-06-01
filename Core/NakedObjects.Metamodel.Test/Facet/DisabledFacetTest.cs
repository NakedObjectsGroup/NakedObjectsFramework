using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class DisabledFacetTest {
        [TestMethod]
        public void TestDisabledFacetAlways() {
            IDisabledFacet facet = new DisabledFacetAlways(null);
            Assert.AreEqual(Resources.NakedObjects.AlwaysDisabled, facet.DisabledReason(null));
        }

        [TestMethod]
        public void TestDisabledFacetAnnotationAlways() {
            IDisabledFacet facet = new DisabledFacetAnnotation(WhenTo.Always, null);
            Assert.AreEqual(Resources.NakedObjects.AlwaysDisabled, facet.DisabledReason(null));
        }

        [TestMethod]
        public void TestDisabledFacetAnnotationNever() {
            IDisabledFacet facet = new DisabledFacetAnnotation(WhenTo.Never, null);
            Assert.IsNull(facet.DisabledReason(null));
        }
    }
}