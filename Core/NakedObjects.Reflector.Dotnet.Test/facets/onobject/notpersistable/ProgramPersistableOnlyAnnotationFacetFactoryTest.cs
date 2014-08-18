// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.NotPersistable;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted {
    [TestFixture]
    public class ProgramPersistableOnlyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ProgramPersistableOnlyAnnotationFacetFactory (reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ProgramPersistableOnlyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IProgramPersistableOnlyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [ProgramPersistableOnly]
        private class Customer {}

        private class Customer1 {}

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
        public void TestProgramPersistableOnlyPickup() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IProgramPersistableOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ProgramPersistableOnlyFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestProgramPersistableOnlyNotPickup() {
            facetFactory.Process(typeof(Customer1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IProgramPersistableOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}