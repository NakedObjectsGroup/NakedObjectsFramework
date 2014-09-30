// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.PageSize;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Actions.PageSize;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class PageSizeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PageSizeAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PageSizeAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPageSizeFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [PageSize(7)]
            public IQueryable<Customer> SomeAction() {
                return null;
            }
        }

        private class Customer1 {
            public IQueryable<Customer1> SomeAction() {
                return null;
            }
        }

        [Test]
        public void TestDefaultPageSizePickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            var actionPeer = new DotNetNakedObjectActionPeer(null, null);
            new FallbackFacetFactory(Reflector).Process(actionMethod, MethodRemover, actionPeer);
            IFacet facet = actionPeer.GetFacet(typeof (IPageSizeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PageSizeFacetDefault);
            var pageSizeFacet = (IPageSizeFacet) facet;
            Assert.AreEqual(20, pageSizeFacet.Value);
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

        [Test]
        public void TestPageSizeAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IPageSizeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PageSizeFacetAnnotation);
            var pageSizeFacet = (IPageSizeFacet) facet;
            Assert.AreEqual(7, pageSizeFacet.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}