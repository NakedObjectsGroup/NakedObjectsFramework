// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Defaults {
    [TestFixture]
    public class PropertyDefaultAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PropertyDefaultAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PropertyDefaultAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPropertyDefaultFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            [DefaultValue(1)]
            public int Prop { get; set; }
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
        public void TestPropertyDefaultAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Prop");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetAnnotation);
            var propertyDefaultFacetAnnotation = (PropertyDefaultFacetAnnotation) facet;
            Assert.AreEqual(1, propertyDefaultFacetAnnotation.GetDefault(null));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}