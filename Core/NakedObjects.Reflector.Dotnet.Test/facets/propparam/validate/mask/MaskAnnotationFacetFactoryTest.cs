// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mask {
    [TestFixture]
    public class MaskAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MaskAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IMaskFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaskAnnotationFacetFactory { Reflector = reflector };
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
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            MaskFacetAnnotation maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            MaskFacetAnnotation maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new Type[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            MaskFacetAnnotation maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [Test]
        public void TestMaskAnnotationNotIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IMaskFacet)));
        }

        [Test]
        public void TestMaskAnnotationNotIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new Type[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IMaskFacet)));
        }

        #region Nested Type: Customer

        [Mask("###")]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [Mask("###")]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            public void SomeAction([Mask("###")] string foo) {}
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            [Mask("###")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public void SomeAction([Mask("###")] int foo) {}
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}