// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections;

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_CollectionTest : AbstractDotNetReflectorTest {
        protected override DotNetSpecification LoadSpecification(DotNetReflector reflector) {
            return (DotNetSpecification)reflector.LoadSpecification(typeof(ArrayList));
        }

        [Test]
        public void TestType() {
            Assert.IsTrue(specification.IsCollection);
        }

        [Test]
        public void TestName() {
            Assert.AreEqual(typeof(ArrayList).FullName, specification.FullName);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(18, specification.FacetTypes.Length);
        }

        [Test]
        public void TestCollectionFacet() {
            IFacet facet = specification.GetFacet(typeof(ICollectionFacet));
            Assert.IsNotNull(facet);
            //Assert.AreEqual(typeof(ArrayList).FullName, facet);
        }

        [Test]
        public void TestTypeOfFacet() {
            ITypeOfFacet facet = (ITypeOfFacet)specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(facet);
            Assert.AreEqual(typeof(object), ((TypeOfFacetAbstract)facet).Value);
        }





        [Test]
        public void TestNamedFaced() {
            IFacet facet = specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = specification.GetFacet(typeof(IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

    }

    // Copyright (c) Naked Objects Group Ltd.
}