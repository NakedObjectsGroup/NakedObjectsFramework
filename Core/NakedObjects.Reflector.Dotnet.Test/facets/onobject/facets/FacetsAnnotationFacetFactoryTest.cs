// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Objects.Value;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Facets {
    [TestFixture]
    public class FacetsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private FacetsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IFacetsFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new FacetsAnnotationFacetFactory(reflector);
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
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

       [Test]
        public void TestFacetsFactoryNames() {
            facetFactory.Process(typeof (Customer1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            FacetsFacetAnnotation facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(1, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
            AssertNoMethodsRemoved();
        }

       [Test]
        public void TestFacetsFactoryClass() {
            facetFactory.Process(typeof (Customer2), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            FacetsFacetAnnotation facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(1, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
        }

       [Test]
        public void TestFacetsFactoryNameAndClass() {
            facetFactory.Process(typeof (Customer3), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            FacetsFacetAnnotation facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(2, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
            Assert.AreEqual(typeof (CustomerFacetFactory2), facetFactories[1]);
        }

       [Test]
        public void TestFacetsFactoryNoop() {
            facetFactory.Process(typeof (Customer4), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IFacetsFacet));
            Assert.IsNull(facet);
        }

        #region Nested Type: Customer1

        [Facets(FacetFactoryNames = new string[] {
                                                     "NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerFacetFactory",
                                                     "NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerNotAFacetFactory"
                                                 })]
        private class Customer1 {}

        #endregion

        #region Nested Type: Customer2

        [Facets(FacetFactoryClasses = new Type[] {typeof (CustomerFacetFactory), typeof (CustomerNotAFacetFactory)})]
        private class Customer2 {}

        #endregion

        #region Nested Type: Customer3

        [Facets(FacetFactoryNames = new string[] {"NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerFacetFactory"},
            FacetFactoryClasses = new Type[] {typeof (CustomerFacetFactory2)})]
        private class Customer3 {}

        #endregion

        #region Nested Type: Customer4

        [Facets]
        private class Customer4 {}

        #endregion

        #region Nested Type: CustomerFacetFactory

        public class CustomerFacetFactory : FacetFactoryAbstract {
            public CustomerFacetFactory() : base(null, null) {}


            public override NakedObjectFeatureType[] FeatureTypes {
                get { return null; }
            }

            public override bool Process(Type clazz, IMethodRemover methodRemover, IFacetHolder holder) {
                return false;
            }

            public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
                return false;
            }

            public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
                return false;
            }
        }

        #endregion

        #region Nested Type: CustomerFacetFactory2

        public class CustomerFacetFactory2 : FacetFactoryAbstract {
            public CustomerFacetFactory2() : base(null, null) {}


            public override NakedObjectFeatureType[] FeatureTypes {
                get { return null; }
            }

            public override bool Process(Type clazz, IMethodRemover methodRemover, IFacetHolder holder) {
                return false;
            }

            public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
                return false;
            }

            public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
                return false;
            }
        }

        #endregion

        #region Nested Type: CustomerNotAFacetFactory

        public class CustomerNotAFacetFactory {}

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}