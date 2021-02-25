// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.Facet;

namespace NakedObjects.Metamodel.Test.Facet {
    [TestClass]
    public class RangeFacetTest {
        [TestMethod]
        public void TestRangeFacet() {
            const int min = 1;
            const int max = 20;
            IRangeFacet facet = new RangeFacet(min, max, false, null);

            var inRangeMock = new Mock<INakedObjectAdapter>();
            var outRangeMock1 = new Mock<INakedObjectAdapter>();
            var outRangeMock2 = new Mock<INakedObjectAdapter>();
            inRangeMock.Setup(a => a.Object).Returns(5);
            outRangeMock1.Setup(a => a.Object).Returns(30);
            outRangeMock2.Setup(a => a.Object).Returns(0);

            Assert.AreEqual(0, facet.OutOfRange(inRangeMock.Object));
            Assert.AreEqual(1, facet.OutOfRange(outRangeMock1.Object));
            Assert.AreEqual(-1, facet.OutOfRange(outRangeMock2.Object));
        }
    }
}