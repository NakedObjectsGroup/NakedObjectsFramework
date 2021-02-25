// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class NamedFacetTest {
        [TestMethod]
        public void TestNamedFacetAnnotation() {
            const string testName = "a Name";
            INamedFacet facet = new NamedFacetAnnotation(testName, null);
            Assert.AreEqual(testName, facet.CapitalizedName);
            Assert.AreEqual(testName, facet.ShortName);
            Assert.AreEqual(testName, facet.SimpleName);
            Assert.AreEqual(testName, facet.NaturalName);
        }

        [TestMethod]
        public void TestNamedFacetInferred() {
            const string testName = "a nAme";
            INamedFacet facet = new NamedFacetInferred(testName, null);
            Assert.AreEqual("A nAme", facet.CapitalizedName);
            Assert.AreEqual("a nAme", facet.ShortName);
            Assert.AreEqual("aname", facet.SimpleName);
            Assert.AreEqual("A n Ame", facet.NaturalName);
        }
    }
}