// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class TestPojo {}

    [TestFixture]
    public class Reflector_ArrayTest : AbstractDotNetReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return  reflector.LoadSpecification(typeof (TestPojo[]));
        }

        [Test]
        public void TestCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNotNull(facet);
            //Assert.AreEqual(typeof(ArrayList).getName(), facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(19, Specification.FacetTypes.Length);
        }

        [Test]
        public void TestName() {           
            Assert.AreEqual(typeof (TestPojo[]).FullName, Specification.FullName);
        }


        [Test]
        public void TestNamedFaced() {
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = Specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestType() {
           
            Assert.IsTrue(Specification.IsCollection);
        }

        [Test]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.AreEqual(typeof (TestPojo), ((TypeOfFacetAbstract) facet).Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}