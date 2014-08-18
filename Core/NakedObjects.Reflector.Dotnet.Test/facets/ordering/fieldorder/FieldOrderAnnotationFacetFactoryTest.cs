// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.FieldOrder {
    [TestFixture]
    public class FieldOrderAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private FieldOrderAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IFieldOrderFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new FieldOrderAnnotationFacetFactory(reflector);
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
        public void TestFieldOrderAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IFieldOrderFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FieldOrderFacetAnnotation);
            FieldOrderFacetAnnotation fieldOrderFacetAnnotation = (FieldOrderFacetAnnotation) facet;
            Assert.AreEqual("foo,bar", fieldOrderFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        #region Nested Type: Customer

        [FieldOrder("foo,bar")]
        private class Customer {}

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}