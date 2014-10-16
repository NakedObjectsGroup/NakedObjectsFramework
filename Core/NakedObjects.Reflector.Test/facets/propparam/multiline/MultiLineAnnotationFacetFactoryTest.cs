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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MultiLineAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private MultiLineAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMultiLineFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [MultiLine(NumberOfLines = 3, Width = 9)]
        private class Customer {}

        private class Customer1 {
            [MultiLine(NumberOfLines = 12, Width = 36)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)] string foo) {}
        }

        [MultiLine]
        private class Customer3 {}

        private class Customer5 {
            [MultiLine(NumberOfLines = 8, Width = 24)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer6 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)] int foo) {}
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
        public void TestMultiLineAnnotationDefaults() {
            facetFactory.Process(typeof (Customer3), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMultiLineFacet));
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(6, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(0, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationIgnoredForNonStringActionParameters() {
            MethodInfo method = FindMethod(typeof (Customer6), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IMultiLineFacet)));
        }

        [Test]
        public void TestMultiLineAnnotationIgnoredForNonStringProperties() {
            PropertyInfo property = FindProperty(typeof (Customer5), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNull(facet);
        }

        [Test]
        public void TestMultiLineAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(8, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(24, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(3, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(9, multiLineFacetAnnotation.Width);
        }

        [Test]
        public void TestMultiLineAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(12, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(36, multiLineFacetAnnotation.Width);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}