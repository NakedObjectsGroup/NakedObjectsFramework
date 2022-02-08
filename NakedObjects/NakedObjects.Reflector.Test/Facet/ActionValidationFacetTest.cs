// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Error;
using NakedObjects.Reflector.Facet;

namespace NakedObjects.Reflector.Test.Facet;

[TestClass]
public class ActionValidationFacetTest {
    private static readonly ILogger<ActionValidationFacet> mockLogger = new Mock<ILogger<ActionValidationFacet>>().Object;

    private static void DelegateFuncTest(MethodInfo method) {
        IImperativeFacet actionValidationFacet = new ActionValidationFacet(method, null, mockLogger);
        var facet = (IActionValidationFacet)actionValidationFacet;
        Assert.IsNotNull(actionValidationFacet.GetMethodDelegate(), method.Name);
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().Select(MockParm).ToArray();
        var target = MockParm(new TestDelegateClass());
        Assert.AreEqual($"Validation{parms.Length}", facet.InvalidReason(target, null, parms));
    }

    private static void InvokeFuncTest(MethodInfo method) {
        IImperativeFacet actionValidationFacet = new ActionValidationFacet(method, null, mockLogger);
        var facet = (IActionValidationFacet)actionValidationFacet;
        Assert.IsNull(actionValidationFacet.GetMethodDelegate());
        Assert.IsNotNull(actionValidationFacet.GetMethod());
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().Select(MockParm).ToArray();
        var target = MockParm(new TestDelegateClass());
        Assert.AreEqual($"Validation{parms.Length}", facet.InvalidReason(target, null, parms));
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

    private static INakedObjectAdapter Mock(object obj) {
        var mockParm = new Mock<INakedObjectAdapter>();
        mockParm.Setup(p => p.Object).Returns(obj);
        return mockParm.Object;
    }

    [TestMethod]
    public void TestDisabledFacetAnnotationAsInteraction() {
        var method = typeof(TestDelegateClass).GetMethod("Func2");
        IValidatingInteractionAdvisor facet = new ActionValidationFacet(method, null, mockLogger);
        var target = Mock(new TestDelegateClass());
        var mockIc = new Mock<IInteractionContext>();
        mockIc.Setup(ic => ic.Target).Returns(target);
        var parms = new[] { "a", "b" }.Select(MockParm).ToArray();
        mockIc.Setup(ic => ic.ProposedArguments).Returns(parms);

        var e = facet.CreateExceptionFor(mockIc.Object);

        Assert.IsInstanceOfType(e, typeof(InvalidException));
        Assert.AreEqual("Validation2", e.Message);
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