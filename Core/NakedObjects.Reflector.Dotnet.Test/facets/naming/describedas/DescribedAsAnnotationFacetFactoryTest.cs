// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.DescribedAs {
    [TestFixture]
    public class DescribedAsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DescribedAsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] { typeof(IDescribedAsFacet) }; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DescribedAsAnnotationFacetFactory(reflector);
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
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        #region DescribedAsTests

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof(Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof(Customer1), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof(Customer2), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof(Customer4), "SomeAction", new Type[] { typeof(int) });
            facetFactory.ProcessParams(actionMethod, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        #endregion 

        #region Description Tests 

        [Test]
        public void TestDescriptionAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof(Customer5), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof(Customer6), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof(Customer7), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof(Customer9), "SomeAction", new Type[] { typeof(int) });
            facetFactory.ProcessParams(actionMethod, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract)facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        #endregion

        #region DescribedAs test data

        #region Nested Type: Customer

        [DescribedAs("some description")]
        private class Customer { }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [DescribedAs("some description")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            [DescribedAs("some description")]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            [DescribedAs("some description")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public void SomeAction([DescribedAs("some description")] int x) { }
        }

        #endregion

        #endregion

        #region Description test data

        #region Nested Type: Customer5

        [System.ComponentModel.Description("some description")]
        private class Customer5 { }

        #endregion

        #region Nested Type: Customer6

        private class Customer6 {
            [System.ComponentModel.Description("some description")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer7

        private class Customer7 {
            [System.ComponentModel.Description("some description")]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer8

        private class Customer8 {
            [System.ComponentModel.Description("some description")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested Type: Customer9

        private class Customer9 {
            public void SomeAction([System.ComponentModel.Description("some description")] int x) { }
        }

        #endregion

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}