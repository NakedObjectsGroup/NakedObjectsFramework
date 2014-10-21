// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Test;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Reflect {
    [TestFixture]
    public class Reflector_ObjectTest : AbstractDotNetReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(DotNetReflector reflector) {
            return reflector.LoadSpecification(typeof(TestDomainObject));
        }

        [Test]
        public void TestCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(16, Specification.FacetTypes.Length);
        }

        [Test]
        public void TestName() {
       
            Assert.AreEqual(typeof (TestDomainObject).FullName, Specification.FullName);
        }

        [Test]
        public void TestNamedFaced() {
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestNoCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestNoTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestNotAnAssociatedMetadataTypeTypeDescriptionProvider() {
            Assert.IsFalse(TypeDescriptor.GetProvider(typeof (TestDomainObject)) is AssociatedMetadataTypeTypeDescriptionProvider);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = Specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestType() {

       
            Assert.IsTrue(Specification.IsObject);
        }

        [Test]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}