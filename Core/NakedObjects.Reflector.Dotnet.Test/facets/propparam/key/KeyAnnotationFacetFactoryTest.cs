// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Key;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Key {
    [TestFixture]
    public class KeyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new KeyAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private KeyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IKeyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [Key]
            public int CustomerKey { get; set; }
        }

        private class Customer1 {
            public int CustomerKey { get; set; }
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestKeyAnnotationNotPickedUpOnPropertyIfAbsent() {
            PropertyInfo property = FindProperty(typeof (Customer1), "CustomerKey");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IKeyFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestKeyAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "CustomerKey");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IKeyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is KeyFacetAnnotation);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}