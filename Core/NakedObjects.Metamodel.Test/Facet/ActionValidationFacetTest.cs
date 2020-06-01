using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class ActionValidationFacetTest {
        private static void DelegateFuncTest(MethodInfo method) {
            IImperativeFacet actionValidationFacet = new ActionValidationFacet(method, null);
            var facet = (IActionValidationFacet) actionValidationFacet;
            Assert.IsNotNull(actionValidationFacet.GetMethodDelegate(), method.Name);
            var parms = method.GetParameters().Select(p => "astring").Cast<object>().Select(MockParm).ToArray();
            var target = MockParm(new TestDelegateClass());
            Assert.AreEqual($"Validation{parms.Length}", facet.InvalidReason(target, parms));
        }

        private static void InvokeFuncTest(MethodInfo method) {
            IImperativeFacet actionValidationFacet = new ActionValidationFacet(method, null);
            var facet = (IActionValidationFacet) actionValidationFacet;
            Assert.IsNull(actionValidationFacet.GetMethodDelegate());
            Assert.IsNotNull(actionValidationFacet.GetMethod());
            var parms = method.GetParameters().Select(p => "astring").Cast<object>().Select(MockParm).ToArray();
            var target = MockParm(new TestDelegateClass());
            Assert.AreEqual($"Validation{parms.Length}", facet.InvalidReason(target, parms));
        }

        [TestMethod]
        public void TestDelegateCreation() {
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func2"));
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func3"));
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func4"));
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func5"));
            DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func6"));
        }

        [TestMethod]
        public void TestDelegateNonCreation() {
            InvokeFuncTest(typeof(TestDelegateClass).GetMethod("Func7"));
        }

        #region Nested type: TestDelegateClass

        // ReSharper disable UnusedMember.Global
        public class TestDelegateClass {
            public string Func2(string p1, string p2) => "Validation2";

            public string Func3(string p1, string p2, string p3) => "Validation3";

            public string Func4(string p1, string p2, string p3, string p4) => "Validation4";

            public string Func5(string p1, string p2, string p3, string p4, string p5) => "Validation5";

            public string Func6(string p1, string p2, string p3, string p4, string p5, string p6) => "Validation6";

            public string Func7(string p1, string p2, string p3, string p4, string p5, string p6, string p7) => "Validation7";
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