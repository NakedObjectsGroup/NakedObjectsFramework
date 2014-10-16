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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaskAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private MaskAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMaskFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Mask("###")]
        private class Customer {}

        private class Customer1 {
            [Mask("###")]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void SomeAction([Mask("###")] string foo) {}
        }

        private class Customer3 {
            [Mask("###")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer4 {
            public void SomeAction([Mask("###")] int foo) {}
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
        public void TestMaskAnnotationNotIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IMaskFacet)));
        }

        [Test]
        public void TestMaskAnnotationNotIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IMaskFacet)));
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            var maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            var maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [Test]
        public void TestMaskAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacetAnnotation);
            var maskFacet = (MaskFacetAnnotation) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}