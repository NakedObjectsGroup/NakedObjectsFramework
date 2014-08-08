// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Contributed;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class NotContributedActonAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ContributedActionAnnotationFacetFactory {Reflector = reflector};
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        private ContributedActionAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INotContributedActionFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [NotContributedAction]
            public void SomeAction() {}
        }

        private class Customer1 {
            public void SomeAction() {}
        }

        private class Customer2 {
            [NotContributedAction(typeof(Customer2))]
            public void SomeAction() {}
        }

        private class Customer3 {
            [NotContributedAction(typeof(Customer2), typeof(Customer3))]
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
        public void TestNotContributedAnnotationNullByDefault() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IExecutedFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNotContributedAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            var facet = (INotContributedActionFacet) facetHolder.GetFacet(typeof (INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacetImpl);

            Assert.IsTrue(facet.NeverContributed());

            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNotContributedAnnotationPickedUpWithType() {
            MethodInfo actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            var facet = (INotContributedActionFacet)facetHolder.GetFacet(typeof(INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacetImpl);

            var sp = reflector.LoadSpecification(typeof(Customer2));

            Assert.IsTrue(facet.NotContributedTo(sp) );

            AssertNoMethodsRemoved();
        }


        [Test]
        public void TestNotContributedAnnotationPickedUpWithTypes() {
            MethodInfo actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            var facet = (INotContributedActionFacet)facetHolder.GetFacet(typeof(INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacetImpl);

            var sp = reflector.LoadSpecification(typeof(Customer2));
            var sp1 = reflector.LoadSpecification(typeof(Customer3));

            Assert.IsTrue(facet.NotContributedTo(sp));
            Assert.IsTrue(facet.NotContributedTo(sp1));

            AssertNoMethodsRemoved();
        }

    }

    // Copyright (c) Naked Objects Group Ltd.
}