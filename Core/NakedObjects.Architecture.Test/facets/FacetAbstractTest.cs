// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

namespace INakedObjects.Architecture.Adapter {

    [TestFixture]
    public class FacetAbstractTest {
        private IFacetHolder facetHolder;
        private IFacetHolder facetHolder2;
        private FacetAbstract fooFacet;


        [SetUp] 
        public void SetUp() {
            facetHolder = new FacetHolderImpl();
            facetHolder2 = new FacetHolderImpl();
            fooFacet = new ConcreteFacet(typeof (IFooFacet), facetHolder);
            FacetUtils.AddFacet(fooFacet);
        }

        [Test]
        public void FacetType() {
            Assert.AreEqual(typeof (IFooFacet), fooFacet.FacetType);
        }

        [Test]
        public void GetFacetHolder() {
            Assert.AreEqual(facetHolder, fooFacet.FacetHolder);
        }

        [Test]
        public void SetFacetHolder() {
            fooFacet.FacetHolder = facetHolder2;
            Assert.AreEqual(facetHolder2, fooFacet.FacetHolder);
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
        public void TestToString() {
            Assert.AreEqual("FacetAbstractTest+ConcreteFacet[type=FacetAbstractTest+IFooFacet]", fooFacet.ToString());
        }

        #region Nested type: ConcreteFacet

        public class ConcreteFacet : FacetAbstract, IFooFacet {
            public ConcreteFacet(Type facetType, IFacetHolder holder) : base(facetType, holder) {}
        }

        #endregion

        #region Nested type: FooFacet

        public interface IFooFacet : IFacet {}

        #endregion
    }
}