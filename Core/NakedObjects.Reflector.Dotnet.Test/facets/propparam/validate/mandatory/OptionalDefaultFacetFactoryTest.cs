// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    [TestFixture]
    public class OptionalDefaultFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new OptionalDefaultFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private OptionalDefaultFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMandatoryFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void SomeAction(string foo) {}
        }

        private class Customer3 {
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer4 {
            public void SomeAction(int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestOptionalDefaultIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultIgnoredForPrimitiveOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OptionalFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OptionalFacetDefault);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}