// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.Test.Facet; 

[TestClass]
public class AutoCompleteViaFunctionFacetTest {
    private static readonly string[] TestValue = {"one", "two"};

    [TestMethod]
    public void TestGetCompletions() {
        var method = typeof(TestClass).GetMethod(nameof(TestClass.Completions));
        var testFacet = new AutoCompleteViaFunctionFacet(method, 0, 0, null, null);

        var result = testFacet.GetCompletions(null, null, null);

        Assert.AreEqual(TestValue.Length, result.Length);
        Assert.AreEqual(TestValue[0], result[0]);
        Assert.AreEqual(TestValue[1], result[1]);
    }

    [TestMethod]
    public void TestGetSingleCompletions() {
        var method = typeof(TestClass).GetMethod(nameof(TestClass.Completion));
        var testFacet = new AutoCompleteViaFunctionFacet(method, 0, 0, null, null);

        var result = testFacet.GetCompletions(null, null, null);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(TestValue[0], result[0]);
    }

    public static class TestClass {
        public static string[] Completions() => TestValue;
        public static string Completion() => TestValue.First();
    }
}