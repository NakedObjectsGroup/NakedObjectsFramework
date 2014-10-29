// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NUnit.Framework;
using NakedObjects.Meta.Utils;
using NakedObjects.Meta.Spec;

namespace INakedObjects.Architecture.Adapter {
    [TestFixture]
    public class FacetAbstractTest {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            specification = new Specification();
            facetHolder2 = new Specification();
            fooFacet = new ConcreteFacet(typeof (IFooFacet), specification);
            FacetUtils.AddFacet(fooFacet);
        }

        #endregion

        private ISpecification specification;
        private ISpecification facetHolder2;
        private FacetAbstract fooFacet;

        public class ConcreteFacet : FacetAbstract, IFooFacet {
            public ConcreteFacet(Type facetType, ISpecification holder) : base(facetType, holder) {}
        }

        public interface IFooFacet : IFacet {}

        [Test]
        public void FacetType() {
            Assert.AreEqual(typeof (IFooFacet), fooFacet.FacetType);
        }

        [Test]
        public void GetFacetHolder() {
            Assert.AreEqual(specification, fooFacet.Specification);
        }

        //[Test]
        //public void Reparent() {
        //    Assert.AreEqual(specification, fooFacet.Specification);
        //    Assert.IsNotNull(specification.GetFacet<IFooFacet>());
        //    Assert.IsNull(facetHolder2.GetFacet<IFooFacet>());
        //    fooFacet.Reparent(facetHolder2);
        //    Assert.AreEqual(facetHolder2, fooFacet.Specification);
        //    Assert.IsNull(specification.GetFacet<IFooFacet>());
        //    Assert.IsNotNull(facetHolder2.GetFacet<IFooFacet>());
        //}

        [Test]
        public void SetFacetHolder() {
            fooFacet.Specification = facetHolder2;
            Assert.AreEqual(facetHolder2, fooFacet.Specification);
        }

        [Test]
        public void TestToString() {
            Assert.AreEqual("FacetAbstractTest+ConcreteFacet[type=FacetAbstractTest+IFooFacet]", fooFacet.ToString());
        }
    }
}