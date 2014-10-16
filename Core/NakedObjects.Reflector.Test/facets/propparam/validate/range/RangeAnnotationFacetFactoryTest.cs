// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Range;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Range;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Range {
    [TestFixture]
    public class RangeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new RangeAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private RangeAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IRangeFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            [System.ComponentModel.DataAnnotations.Range(1, 10)]
            public int Prop { get; set; }
        }

        private class Customer2 {
            public void someAction([System.ComponentModel.DataAnnotations.Range(1, 10)] int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestRangeAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRangeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RangeFacetAnnotation);
            var RangeFacetAnnotation = (RangeFacetAnnotation) facet;
            Assert.AreEqual(1, RangeFacetAnnotation.Min);
            Assert.AreEqual(10, RangeFacetAnnotation.Max);
        }

        [Test]
        public void TestRangeAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Prop");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRangeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RangeFacetAnnotation);
            var RangeFacetAnnotation = (RangeFacetAnnotation) facet;
            Assert.AreEqual(1, RangeFacetAnnotation.Min);
            Assert.AreEqual(10, RangeFacetAnnotation.Max);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}