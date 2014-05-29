// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.MultiLine;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.MultiLine {
    [TestFixture]
    public class MultiLineAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MultiLineAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IMultiLineFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MultiLineAnnotationFacetFactory { Reflector = reflector };
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
        public void TestMultiLineAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            MultiLineFacetAnnotation multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(3, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(9, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            MultiLineFacetAnnotation multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(12, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(36, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new Type[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            MultiLineFacetAnnotation multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(8, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(24, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationDefaults() {
            facetFactory.Process(typeof (Customer3), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMultiLineFacet));
            MultiLineFacetAnnotation multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(6, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(0, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationIgnoredForNonStringProperties() {
            PropertyInfo property = FindProperty(typeof (Customer5), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestMultiLineAnnotationIgnoredForNonStringActionParameters() {
            MethodInfo method = FindMethod(typeof (Customer6), "SomeAction", new Type[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (IMultiLineFacet)));
        }

        #region Nested Type: Customer

        [MultiLine(NumberOfLines = 3, Width = 9)]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [MultiLine(NumberOfLines = 12, Width = 36)]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)] string foo) {}
        }

        #endregion

        #region Nested Type: Customer3

        [MultiLine()]
        private class Customer3 {}

        #endregion

        #region Nested Type: Customer5

        private class Customer5 {
            [MultiLine(NumberOfLines = 8, Width = 24)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer6

        private class Customer6 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)] int foo) {}
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}