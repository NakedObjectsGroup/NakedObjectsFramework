// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Disable {
    [TestFixture]
    public class DisabledAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DisabledAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IDisabledFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DisabledAnnotationFacetFactory(reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
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
        public void TestDisabledAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledWhenAlwaysAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(When.Always, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenNeverAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(When.Never, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenOncePersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer5), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(When.OncePersisted, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer6), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(When.UntilPersisted, disabledFacetAbstract.Value);
        }

        #region Nested Type: Customer

        private class Customer {
            [Disabled]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [Disabled]
            public IList Orders {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            [Disabled]
            public void SomeAction() {}
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            [Disabled(WhenTo.Always)]
            public void SomeAction() {}
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            [Disabled(WhenTo.Never)]
            public void SomeAction() {}
        }

        #endregion

        #region Nested Type: Customer5

        private class Customer5 {
            [Disabled(WhenTo.OncePersisted)]
            public void SomeAction() {}
        }

        #endregion

        #region Nested Type: Customer6

        private class Customer6 {
            [Disabled(WhenTo.UntilPersisted)]
            public void SomeAction() {}
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}