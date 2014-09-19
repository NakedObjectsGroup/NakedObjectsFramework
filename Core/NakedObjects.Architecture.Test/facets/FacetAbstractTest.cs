// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

namespace INakedObjects.Architecture.Adapter {
    [TestFixture]
    public class FacetAbstractTest {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            facetHolder = new FacetHolderImpl();
            facetHolder2 = new FacetHolderImpl();
            fooFacet = new ConcreteFacet(typeof (IFooFacet), facetHolder);
            FacetUtils.AddFacet(fooFacet);
        }

        #endregion

        private IFacetHolder facetHolder;
        private IFacetHolder facetHolder2;
        private FacetAbstract fooFacet;

        public class ConcreteFacet : FacetAbstract, IFooFacet {
            public ConcreteFacet(Type facetType, IFacetHolder holder) : base(facetType, holder) {}
        }

        public interface IFooFacet : IFacet {}

        [Test]
        public void FacetType() {
            Assert.AreEqual(typeof (IFooFacet), fooFacet.FacetType);
        }

        [Test]
        public void GetFacetHolder() {
            Assert.AreEqual(facetHolder, fooFacet.FacetHolder);
        }

        [Test]
        public void Reparent() {
            Assert.AreEqual(facetHolder, fooFacet.FacetHolder);
            Assert.IsNotNull(facetHolder.GetFacet<IFooFacet>());
            Assert.IsNull(facetHolder2.GetFacet<IFooFacet>());
            fooFacet.Reparent(facetHolder2);
            Assert.AreEqual(facetHolder2, fooFacet.FacetHolder);
            Assert.IsNull(facetHolder.GetFacet<IFooFacet>());
            Assert.IsNotNull(facetHolder2.GetFacet<IFooFacet>());
        }

        [Test]
        public void SetFacetHolder() {
            fooFacet.FacetHolder = facetHolder2;
            Assert.AreEqual(facetHolder2, fooFacet.FacetHolder);
        }

        [Test]
        public void TestToString() {
            Assert.AreEqual("FacetAbstractTest+ConcreteFacet[type=FacetAbstractTest+IFooFacet]", fooFacet.ToString());
        }
    }
}