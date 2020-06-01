using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class RangeFacetTest {
        [TestMethod]
        public void TestRangeFacet() {
            const int min = 1;
            const int max = 20;
            var facet = new RangeFacet(min, max, false, null);

            var inRangeMock = new Mock<INakedObjectAdapter>();
            var outRangeMock1 = new Mock<INakedObjectAdapter>();
            var outRangeMock2 = new Mock<INakedObjectAdapter>();
            inRangeMock.Setup(a => a.Object).Returns(5);
            outRangeMock1.Setup(a => a.Object).Returns(30);
            outRangeMock2.Setup(a => a.Object).Returns(0);

            Assert.AreEqual(0, facet.OutOfRange(inRangeMock.Object));
            Assert.AreEqual(1, facet.OutOfRange(outRangeMock1.Object));
            Assert.AreEqual(-1, facet.OutOfRange(outRangeMock2.Object));
        }
    }
}