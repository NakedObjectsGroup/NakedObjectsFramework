// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;
using NakedObjects.Architecture.Facets.Objects.Key;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Key {
    [TestFixture]
    public class KeyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private KeyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IKeyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new KeyAnnotationFacetFactory { Reflector = reflector };
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
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
        public void TestKeyAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "CustomerKey");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IKeyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is KeyFacetAnnotation);
        }

        [Test]
        public void TestKeyAnnotationNotPickedUpOnPropertyIfAbsent() {
            PropertyInfo property = FindProperty(typeof(Customer1), "CustomerKey");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IKeyFacet));
            Assert.IsNull(facet);
        }

        private class Customer {
            [System.ComponentModel.DataAnnotations.Key]
            public int CustomerKey { get; set; }    
        }

        private class Customer1 {
            public int CustomerKey { get; set; }
        }

    }

    // Copyright (c) Naked Objects Group Ltd.
}