// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    [TestFixture]
    public class MemberOrderAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MemberOrderAnnotationFacetFactory(Metadata);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private MemberOrderAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMemberOrderFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [MemberOrder(Sequence = "1")]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer1 {
            [MemberOrder(Sequence = "2")]
            public IList Orders {
                get { return null; }
            }

            public void AddToOrders(Order o) {}
        }

        private class Customer2 {
            [MemberOrder(Sequence = "3")]
            public void SomeAction() {}
        }

        private class Order {}

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
        public void TestMemberOrderAnnotationPickedUpOnAction() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(method, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMemberOrderFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MemberOrderFacetAnnotation);
            var memberOrderFacetAnnotation = (MemberOrderFacetAnnotation) facet;
            Assert.AreEqual("3", memberOrderFacetAnnotation.Sequence);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestMemberOrderAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Orders");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMemberOrderFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MemberOrderFacetAnnotation);
            var memberOrderFacetAnnotation = (MemberOrderFacetAnnotation) facet;
            Assert.AreEqual("2", memberOrderFacetAnnotation.Sequence);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestMemberOrderAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "FirstName");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMemberOrderFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MemberOrderFacetAnnotation);
            var memberOrderFacetAnnotation = (MemberOrderFacetAnnotation) facet;
            Assert.AreEqual("1", memberOrderFacetAnnotation.Sequence);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}