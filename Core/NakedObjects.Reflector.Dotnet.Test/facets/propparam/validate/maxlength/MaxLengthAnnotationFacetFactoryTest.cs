// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.MaxLength {
    [TestFixture]
    public class MaxLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaxLengthAnnotationFacetFactory (reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        private MaxLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMaxLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

       
        private class Customer {}

        private class Customer1 {
            [MaxLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void someAction([MaxLength(20)] string foo) {}
        }

        private class Customer4 {
            [StringLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer5 {
            public void someAction([StringLength(20)] string foo) {}
        }

        private class Customer7 {
            [MaxLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer8 {
            public void someAction([MaxLength(20)] string foo) {}
        }

        [Test]
        public void TestCMMaxLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer8), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestCMMaxLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer7), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
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
        public void TestNOFMaxLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        //[Test]
        //public void TestNOFMaxLengthAnnotationPickedUpOnClass() {
        //    facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
        //    IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is MaxLengthFacetAnnotation);
        //    var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
        //    Assert.AreEqual(16, maxLengthFacetAnnotation.Value);
        //}

        [Test]
        public void TestNOFMaxLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestStringLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer5), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestStringLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer4), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}