// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.Test.Facet;

[TestClass]
public class HideForContextViaFunctionFacetTest {
    private static readonly string TestValue = NakedObjects.Resources.NakedObjects.Hidden;

    [TestMethod]
    public void TestHidden() {
        var method = typeof(TestClass).GetMethod(nameof(TestClass.Hides));
        var testFacet = new HideForContextViaFunctionFacet(method, null, null);

        var result = testFacet.HiddenReason(new Mock<INakedObjectAdapter>().Object, null);

        Assert.AreEqual(TestValue, result);
    }

    public static class TestClass {
        public static bool Hides() => true;
    }
}