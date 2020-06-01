using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class HideForContextFacetTest {
        private static void DelegateFuncTest(MethodInfo method, bool isHidden) {
            var hideForContextFacet = new HideForContextFacet(method, null);
            IHideForContextFacet facet = hideForContextFacet;
            Assert.IsNotNull(hideForContextFacet.GetMethodDelegate(), method.Name);
            var target = MockParm(new TestDelegateClass());
            var result = isHidden ? Resources.NakedObjects.Hidden : null;
            Assert.AreEqual(result, facet.HiddenReason(target));
        }

        [TestMethod]
        public void TestDelegateCreation() {
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("FuncTrue"), true);
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("FuncFalse"), false);
        }

        #region Nested type: TestDelegateClass

        // ReSharper disable UnusedMember.Global
        public class TestDelegateClass {
            public bool FuncTrue() => true;
            public bool FuncFalse() => false;
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