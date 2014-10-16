// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.MaxLength;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.MaxLength {
    [TestFixture]
    public class MaxLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaxLengthAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestCMMaxLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer7), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestNOFMaxLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        //[Test]
        //public void TestNOFMaxLengthAnnotationPickedUpOnClass() {
        //    facetFactory.Process(typeof (Customer), methodRemover, specification);
        //    IFacet facet = specification.GetFacet(typeof (IMaxLengthFacet));
        //    Assert.IsNotNull(facet);
        //    Assert.IsTrue(facet is MaxLengthFacetAnnotation);
        //    var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
        //    Assert.AreEqual(16, maxLengthFacetAnnotation.Value);
        //}

        [Test]
        public void TestNOFMaxLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestStringLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer5), "someAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestStringLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer4), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}