// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength {
    [TestFixture]
    public class TypicalLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private TypicalLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (ITypicalLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypicalLengthAnnotationFacetFactory(reflector);
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
        public void TestTypicalLengthAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            TypicalLengthFacetAnnotation typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(16, typicalLengthFacetAnnotation.Value);
        }

       [Test]
        public void TestTypicalLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            TypicalLengthFacetAnnotation typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(30, typicalLengthFacetAnnotation.Value);
        }

       [Test]
        public void TestTypicalLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new Type[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            TypicalLengthFacetAnnotation typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(20, typicalLengthFacetAnnotation.Value);
        }

        #region Nested Type: Customer

        [TypicalLength(16)]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [TypicalLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            public void someAction([TypicalLength(20)] int foo) {}
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}