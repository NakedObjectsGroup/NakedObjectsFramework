// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Immutable {
    [TestFixture]
    public class ImmutableAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private ImmutableAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IImmutableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ImmutableAnnotationFacetFactory(reflector);
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

        public void TestImmutableAnnotationPickedUpOnClassAndDefaultsToAlways() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            ImmutableFacetAnnotation immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(When.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationAlwaysPickedUpOnClass() {
            facetFactory.Process(typeof (Customer1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            ImmutableFacetAnnotation immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(When.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationNeverPickedUpOnClass() {
            facetFactory.Process(typeof (Customer2), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            ImmutableFacetAnnotation immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(When.Never, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestImmutableAnnotationOncePersistedPickedUpOnClass() {
            facetFactory.Process(typeof (Customer3), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            ImmutableFacetAnnotation immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(When.OncePersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestImmutableAnnotationUntilPersistedPickedUpOnClass() {
            facetFactory.Process(typeof (Customer4), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            ImmutableFacetAnnotation immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(When.UntilPersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        #region Nested Type: Customer

        [Immutable()]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        [Immutable(WhenTo.Always)]
        private class Customer1 {}

        #endregion

        #region Nested Type: Customer2

        [Immutable(WhenTo.Never)]
        private class Customer2 {}

        #endregion

        #region Nested Type: Customer3

        [Immutable(WhenTo.OncePersisted)]
        private class Customer3 {}

        #endregion

        #region Nested Type: Customer4

        [Immutable(WhenTo.UntilPersisted)]
        private class Customer4 {}

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}