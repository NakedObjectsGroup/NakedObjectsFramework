// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Spec;
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
            IFacet facet = specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestDescriptionFaced() {
            IFacet facet = specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestFacets() {
            Assert.AreEqual(16, specification.FacetTypes.Length);
        }

        [Test]
        public void TestName() {
       
            Assert.AreEqual(typeof (TestDomainObject).FullName, specification.FullName);
        }

        [Test]
        public void TestNamedFaced() {
            IFacet facet = specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestNoCollectionFacet() {
            IFacet facet = specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestNoTypeOfFacet() {
            var facet = (ITypeOfFacet) specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestNotAnAssociatedMetadataTypeTypeDescriptionProvider() {
            Assert.IsFalse(TypeDescriptor.GetProvider(typeof (TestDomainObject)) is AssociatedMetadataTypeTypeDescriptionProvider);
        }

        [Test]
        public void TestPluralFaced() {
            IFacet facet = specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [Test]
        public void TestType() {

       
            Assert.IsTrue(specification.IsObject);
        }

        [Test]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}