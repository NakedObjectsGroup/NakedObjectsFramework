﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.SpecImmutable;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.Test.Facet;

[TestClass]
public class ContributedFunctionFacetTest {
    private readonly Mock<ITypeSpecImmutable> mockSpec = new();

    public ContributedFunctionFacetTest() {
        mockSpec.Setup(s => s.IsOfType(mockSpec.Object)).Returns(true);
    }

    [TestMethod]
    public void TestContributee() {
        var testFacet = new ContributedFunctionFacet();

        testFacet.AddContributee(mockSpec.Object);
        var result = testFacet.IsContributedTo(mockSpec.Object);

        Assert.IsTrue(result);
    }
}