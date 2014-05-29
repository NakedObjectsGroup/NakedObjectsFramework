// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class ExecutedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ExecutedAnnotationFacetFactory {Reflector = reflector};
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        private ExecutedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IExecutedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [Executed(Where.Locally)]
            public void SomeAction() {}
        }

        private class Customer1 {
            [Executed(Where.Remotely)]
            public void SomeAction() {}
        }

        [Test]
        public void TestExecutedLocallyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IExecutedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ExecutedFacetAbstract);
            var executedFacetAbstract = (ExecutedFacetAbstract) facet;
            Assert.AreEqual(Target.Local, executedFacetAbstract.Target);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestExecutedRemotelyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IExecutedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ExecutedFacetAbstract);
            var executedFacetAbstract = (ExecutedFacetAbstract) facet;
            Assert.AreEqual(Target.Remote, executedFacetAbstract.Target);
            AssertNoMethodsRemoved();
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}