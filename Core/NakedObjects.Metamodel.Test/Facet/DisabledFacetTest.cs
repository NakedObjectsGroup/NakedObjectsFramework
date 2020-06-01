using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Core;
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

        private static INakedObjectAdapter Mock(object obj) {
            var mockParm = new Mock<INakedObjectAdapter>();
            mockParm.Setup(p => p.Object).Returns(obj);
            return mockParm.Object;
        }

        [TestMethod]
        public void TestDisabledFacetAnnotationAsInteraction() {
            IDisablingInteractionAdvisor facet = new DisabledFacetAnnotation(WhenTo.Never, null);
            var target = Mock(new object());
            var mockIc = new Mock<IInteractionContext>();
            mockIc.Setup(ic => ic.Target).Returns(target);

            var e = facet.CreateExceptionFor(mockIc.Object);

            Assert.IsInstanceOfType(e, typeof(DisabledException));
            Assert.AreEqual("Exception of type 'NakedObjects.Core.DisabledException' was thrown.", e.Message);
        }
    }
}