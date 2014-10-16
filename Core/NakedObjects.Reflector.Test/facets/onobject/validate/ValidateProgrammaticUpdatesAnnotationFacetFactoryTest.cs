// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Validation;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.Immutable;
using NakedObjects.Reflector.DotNet.Facets.Objects.Validation;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted {
    [TestFixture]
    public class ValidateProgrammaticUpdatesAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ValidateProgrammaticUpdatesAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ValidateProgrammaticUpdatesAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IValidateProgrammaticUpdatesFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [ValidateProgrammaticUpdates]
        private class Customer {}

        private class Customer1 {}


        [Test]
        public void TestApplyValidationNotPickup() {
            facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IValidateProgrammaticUpdatesFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestApplyValidationPickup() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IValidateProgrammaticUpdatesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ValidateProgrammaticUpdatesFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}