// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    [TestFixture]
    public class TitleMethodFacetFactoryTest : AbstractFacetFactoryTest {
        private TitleMethodFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (ITitleFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TitleMethodFacetFactory { Reflector = reflector };
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
        public void TestTitleMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo titleMethod = FindMethod(typeof (Customer), "Title");
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaTitleMethod);
            TitleFacetViaTitleMethod titleFacetViaTitleMethod = (TitleFacetViaTitleMethod) facet;
            Assert.AreEqual(titleMethod, titleFacetViaTitleMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(titleMethod));
        }

        [Test]
        public void TestToStringMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo toStringMethod = FindMethod(typeof (Customer1), "ToString");
            facetFactory.Process(typeof (Customer1), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaToStringMethod);
            TitleFacetViaToStringMethod titleFacetViaTitleMethod = (TitleFacetViaToStringMethod) facet;
            Assert.AreEqual(toStringMethod, titleFacetViaTitleMethod.GetMethod());
            Assert.IsTrue(methodRemover.GetRemoveMethodMethodCalls().Contains(toStringMethod));
        }

        [Test]
        public void TestNoExplicitTitleOrToStringMethod() {
            facetFactory.Process(typeof (Customer2), methodRemover, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (ITitleFacet)));
            AssertNoMethodsRemoved();
        }

        #region Nested Type: Customer

        private class Customer {
            public string Title() {
                return "Some title";
            }
        }

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            public override string ToString() {
                return "Some title via ToString";
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {}

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}