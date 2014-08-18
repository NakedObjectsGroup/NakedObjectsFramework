// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural {
    [TestFixture]
    public class PluralAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private PluralAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IPluralFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PluralAnnotationFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestPluralAnnotationMethodPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PluralFacetAnnotation);
            PluralFacetAnnotation pluralFacet = (PluralFacetAnnotation) facet;
            Assert.AreEqual("Some plural name", pluralFacet.Value);
            AssertNoMethodsRemoved();
        }

        #region Nested Type: Customer

        [Plural("Some plural name")]
        private class Customer {}

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}