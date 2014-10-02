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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DisabledAnnotationFacetFactory(Metadata);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private DisabledAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IDisabledFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [Disabled]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer1 {
            [Disabled]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer2 {
            [Disabled]
            public void SomeAction() {}
        }

        private class Customer3 {
            [Disabled(WhenTo.Always)]
            public void SomeAction() {}
        }

        private class Customer4 {
            [Disabled(WhenTo.Never)]
            public void SomeAction() {}
        }

        private class Customer5 {
            [Disabled(WhenTo.OncePersisted)]
            public void SomeAction() {}
        }

        private class Customer6 {
            [Disabled(WhenTo.UntilPersisted)]
            public void SomeAction() {}
        }

        [Test]
        public void TestDisabledAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDisabledWhenAlwaysAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Always, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenNeverAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Never, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenOncePersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer5), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.OncePersisted, disabledFacetAbstract.Value);
        }

        [Test]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            MethodInfo actionMethod = FindMethod(typeof (Customer6), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, disabledFacetAbstract.Value);
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}