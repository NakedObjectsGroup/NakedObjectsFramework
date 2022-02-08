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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedObjects.Reflector.Facet;

namespace NakedObjects.Reflector.Test.Facet;

[TestClass]
public class PropertyValidationFacetTest {
    private static readonly ILogger<PropertyValidateFacetViaMethod> mockLogger = new Mock<ILogger<PropertyValidateFacetViaMethod>>().Object;

    private static void DelegateFuncTest(MethodInfo method) {
        IImperativeFacet validationFacet = new PropertyValidateFacetViaMethod(method, null, mockLogger);
        var facet = (IPropertyValidateFacet)validationFacet;
        Assert.IsNotNull(validationFacet.GetMethodDelegate(), method.Name);
        var target = MockParm(new TestDelegateClass());
        var value = MockParm("astring");
        Assert.AreEqual("Validation", facet.InvalidReason(target, null, value));
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