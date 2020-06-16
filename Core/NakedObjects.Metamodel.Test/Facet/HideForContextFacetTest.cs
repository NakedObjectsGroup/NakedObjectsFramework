// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Core;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class HideForContextFacetTest {
        private static readonly ILogger<HideForContextFacet> mockLogger = new Mock<ILogger<HideForContextFacet>>().Object;

        private static void DelegateFuncTest(MethodInfo method, bool isHidden) {
            var hideForContextFacet = new HideForContextFacet(method, null, mockLogger);
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

        private static INakedObjectAdapter Mock(object obj) {
            var mockParm = new Mock<INakedObjectAdapter>();
            mockParm.Setup(p => p.Object).Returns(obj);
            return mockParm.Object;
        }

        [TestMethod]
        public void TestDisabledFacetAnnotationAsInteraction() {
            var method = typeof(TestDelegateClass).GetMethod("FuncTrue");
            IHidingInteractionAdvisor facet = new HideForContextFacet(method, null, mockLogger);
            var target = Mock(new TestDelegateClass());
            var mockIc = new Mock<IInteractionContext>();
            mockIc.Setup(ic => ic.Target).Returns(target);

            var e = facet.CreateExceptionFor(mockIc.Object, null, null);

            Assert.IsInstanceOfType(e, typeof(HiddenException));
            Assert.AreEqual("Hidden", e.Message);
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