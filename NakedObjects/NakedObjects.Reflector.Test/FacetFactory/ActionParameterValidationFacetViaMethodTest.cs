// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedObjects.Reflector.Facet;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.Reflector.Test.FacetFactory;

[TestClass]
public class ActionParameterValidationFacetViaMethodTest {
    private IActionParameterValidationFacet facet;
    private INakedObjectAdapter target;

    [TestMethod]
    public void Test1() {
        var mock = new Mock<INakedObjectAdapter>();
        var value = mock.Object;
        mock.Setup(no => no.Object).Returns(10);
        Assert.IsNull(facet.InvalidReason(target, null, value));
    }

    [TestMethod]
    public void Test2() {
        var mock = new Mock<INakedObjectAdapter>();
        var value = mock.Object;
        mock.Setup(no => no.Object).Returns(-7);
        Assert.AreEqual(facet.InvalidReason(target, null, value), "must be positive");
    }

    #region Setup/Teardown

    [TestInitialize]
    public void SetUp() {
        var holder = new Mock<ISpecification>().Object;
        var customer = new Customer17();

        var mock = new Mock<INakedObjectAdapter>();
        target = mock.Object;

        mock.Setup(no => no.Object).Returns(customer);
        var mockLogger = new Mock<ILogger<ActionParameterValidation>>().Object;

        var method = typeof(Customer17).GetMethod("Validate0SomeAction");
        facet = new ActionParameterValidation(method, holder, mockLogger);
    }

    [TestCleanup]
    public void TearDown() { }

    #endregion
}

internal class Customer17 {
    public void SomeAction(int x, long y, long z) { }

    public string Validate0SomeAction(int x) => x > 0 ? null : "must be positive";

    public string Validate1SomeAction(long x) => null;
}