using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class PropertyValidationFacetTest {
        private static void DelegateFuncTest(MethodInfo method) {
            IImperativeFacet validationFacet = new PropertyValidateFacetViaMethod(method, null);
            var facet = (IPropertyValidateFacet) validationFacet;
            Assert.IsNotNull(validationFacet.GetMethodDelegate(), method.Name);
            var target = MockParm(new TestDelegateClass());
            var value = MockParm("astring");
            Assert.AreEqual("Validation", facet.InvalidReason(target, value));
        }

        [TestMethod]
        public void TestDelegateCreation() {
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func2"));
        }

        #region Nested type: TestDelegateClass

        // ReSharper disable UnusedMember.Global
        public class TestDelegateClass {
            public string Func2(string p1) => "Validation";
        }
        // ReSharper restore UnusedMember.Global

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() { }

        private static INakedObjectAdapter MockParm(object obj) {
            var mockParm = new Mock<INakedObjectAdapter>();
            mockParm.Setup(p => p.Object).Returns(obj);
            return mockParm.Object;
        }

        #endregion
    }
}