// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Potency;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Actions.Potency;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class PotencyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PotencyAnnotationFacetFactory {Reflector = reflector};
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PotencyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IQueryOnlyFacet), typeof (IIdempotentFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [QueryOnly]
            public void SomeAction() {}
        }

        private class Customer1 {
            [Idempotent]
            public void SomeAction() {}
        }

        private class Customer2 {
            public void SomeAction() {}
        }

        private class Customer3 {
            [QueryOnly, Idempotent]
            public void SomeAction() {}
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestIdempotentAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestQueryOnlyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IQueryOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is QueryOnlyFacetAnnotation);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNoAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IQueryOnlyFacet));
            Assert.IsNull(facet);
            facet = facetHolder.GetFacet(typeof(IIdempotentFacet));
            Assert.IsNull(facet);
            
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestIdempotentPriorityAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof(Customer1), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacetAnnotation);
            facet = facetHolder.GetFacet(typeof(IQueryOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }


    }

    // Copyright (c) Naked Objects Group Ltd.
}