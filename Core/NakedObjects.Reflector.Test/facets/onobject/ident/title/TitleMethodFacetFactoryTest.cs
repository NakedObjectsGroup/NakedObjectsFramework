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
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TitleMethodFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private TitleMethodFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (ITitleFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            public string Title() {
                return "Some title";
            }
        }

        private class Customer1 {
            public override string ToString() {
                return "Some title via ToString";
            }
        }

        private class Customer2 {}

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
        public void TestNoExplicitTitleOrToStringMethod() {
            facetFactory.Process(typeof (Customer2), MethodRemover, FacetHolder);
            Assert.IsNull(FacetHolder.GetFacet(typeof (ITitleFacet)));
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestTitleMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo titleMethod = FindMethod(typeof (Customer), "Title");
            facetFactory.Process(typeof (Customer), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaTitleMethod);
            var titleFacetViaTitleMethod = (TitleFacetViaTitleMethod) facet;
            Assert.AreEqual(titleMethod, titleFacetViaTitleMethod.GetMethod());
            AssertMethodRemoved(titleMethod);
        }

        [Test]
        public void TestToStringMethodPickedUpOnClassAndMethodRemoved() {
            MethodInfo toStringMethod = FindMethod(typeof (Customer1), "ToString");
            facetFactory.Process(typeof (Customer1), MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaToStringMethod);
            var titleFacetViaTitleMethod = (TitleFacetViaToStringMethod) facet;
            Assert.AreEqual(toStringMethod, titleFacetViaTitleMethod.GetMethod());
            AssertMethodRemoved(toStringMethod);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}