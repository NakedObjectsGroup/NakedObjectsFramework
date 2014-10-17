// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_GenericCollectionTest : AbstractDotNetReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(DotNetReflector reflector) {
            return  reflector.LoadSpecification(typeof (List<TestPojo>));
        }

        [Test]
        public void TestCollectionFacet() {
            IFacet facet = specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNotNull(facet);
            //Assert.AreEqual(typeof(ArrayList).FullName, facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(18, specification.FacetTypes.Length);
        }

        [Test]
        public void TestName() {
         
            Assert.AreEqual(typeof (List<TestPojo>).FullName, specification.FullName);
        }


        [Test]
        public void TestNamedFaced() {
            IFacet facet = specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestType() {
        
            Assert.IsTrue(specification.IsCollection);
        }

        [Test]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.AreEqual(typeof (TestPojo), ((TypeOfFacetAbstract) facet).Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}