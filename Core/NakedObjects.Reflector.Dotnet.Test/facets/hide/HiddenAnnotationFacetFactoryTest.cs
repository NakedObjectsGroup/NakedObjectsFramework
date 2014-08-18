// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Hide {
    [TestFixture]
    public class HiddenAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new HiddenAnnotationFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private HiddenAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IHiddenFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [Hidden]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer1 {
            [Hidden]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer2 {
            [Hidden]
            public void SomeAction() {}
        }

        private class Customer3 {
            [Hidden(WhenTo.Always)]
            public void SomeAction() {}
        }

        private class Customer4 {
            [Hidden(WhenTo.Never)]
            public void SomeAction() {}
        }

        private class Customer5 {
            [Hidden(WhenTo.OncePersisted)]
            public void SomeAction() {}
        }

        private class Customer6 {
            [Hidden(WhenTo.UntilPersisted)]
            public void SomeAction() {}
        }

        private class Customer7 {
            [ScaffoldColumn(false)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer8 {
            [ScaffoldColumn(false)]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer9 {
            [ScaffoldColumn(true)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer10 {
            [Hidden]
            [ScaffoldColumn(true)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        [Test]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer6), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract) facet;
            Assert.AreEqual(When.UntilPersisted, hiddenFacetAbstract.Value);
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestHiddenAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestHiddenAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestHiddenAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestHiddenWhenAlwaysAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract) facet;
            Assert.AreEqual(When.Always, hiddenFacetAbstract.Value);
        }

        [Test]
        public void TestHiddenWhenNeverAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract) facet;
            Assert.AreEqual(When.Never, hiddenFacetAbstract.Value);
        }

        [Test]
        public void TestHiddenWhenOncePersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer5), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract) facet;
            Assert.AreEqual(When.OncePersisted, hiddenFacetAbstract.Value);
        }

        [Test]
        public void TestScaffoldAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestScaffoldAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer7), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestScaffoldTrueAnnotationPickedUpOn() {
            PropertyInfo property = FindProperty(typeof(Customer9), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract) facet;
            Assert.AreEqual(When.Never, hiddenFacetAbstract.Value);
        }


        [Test]
        public void TestHiidenPriorityOverScaffoldAnnotation() {
            PropertyInfo property = FindProperty(typeof(Customer10), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacetAbstract)facet;
            Assert.AreEqual(When.Always, hiddenFacetAbstract.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}