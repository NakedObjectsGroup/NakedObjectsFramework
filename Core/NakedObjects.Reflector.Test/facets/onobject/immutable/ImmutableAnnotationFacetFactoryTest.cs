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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ImmutableAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ImmutableAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IImmutableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        public void TestImmutableAnnotationPickedUpOnClassAndDefaultsToAlways() {
            facetFactory.Process(typeof (Customer), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationAlwaysPickedUpOnClass() {
            facetFactory.Process(typeof (Customer1), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationNeverPickedUpOnClass() {
            facetFactory.Process(typeof (Customer2), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Never, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Immutable]
        private class Customer {}

        [Immutable(WhenTo.Always)]
        private class Customer1 {}

        [Immutable(WhenTo.Never)]
        private class Customer2 {}

        [Immutable(WhenTo.OncePersisted)]
        private class Customer3 {}

        [Immutable(WhenTo.UntilPersisted)]
        private class Customer4 {}

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
        public void TestImmutableAnnotationOncePersistedPickedUpOnClass() {
            facetFactory.Process(typeof (Customer3), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.OncePersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestImmutableAnnotationUntilPersistedPickedUpOnClass() {
            facetFactory.Process(typeof (Customer4), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}