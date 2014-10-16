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
            facetFactory = new ProgramPersistableOnlyAnnotationFacetFactory(Reflector);
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
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestProgramPersistableOnlyNotPickup() {
            facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IProgramPersistableOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestProgramPersistableOnlyPickup() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IProgramPersistableOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ProgramPersistableOnlyFacetAnnotation);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}