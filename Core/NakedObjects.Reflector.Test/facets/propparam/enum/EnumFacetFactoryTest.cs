// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Enums {
    [TestFixture]
    public class EnumFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new EnumFacetFactory(Metadata);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private EnumFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IEnumFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private static void CheckChoices(IFacet facet) {
            var facetAsEnumFacet = facet as IEnumFacet;
            Assert.AreEqual(3, facetAsEnumFacet.GetChoices(null).Length);
            Assert.AreEqual(Cities.London, facetAsEnumFacet.GetChoices(null)[0]);
            Assert.AreEqual(Cities.NewYork, facetAsEnumFacet.GetChoices(null)[1]);
            Assert.AreEqual(Cities.Paris, facetAsEnumFacet.GetChoices(null)[2]);

            Assert.AreEqual(1, facetAsEnumFacet.GetChoices(null, new object[] {Cities.NewYork}).Length);
            Assert.AreEqual(Cities.NewYork, facetAsEnumFacet.GetChoices(null, new object[] {Cities.NewYork})[0]);

            INakedObject nakedObject = new ProgrammableNakedObject(Cities.NewYork, null);

            Assert.AreEqual("New York", facetAsEnumFacet.GetTitle(nakedObject));
        }


        private enum Cities {
            London,
            Paris,
            NewYork
        }

        private class Customer1 {
            [EnumDataType(typeof (Cities))]
            public int City { get; set; }
        }

        private class Customer2 {
            public void SomeAction([EnumDataType(typeof (Cities))] int city) {}
        }

        private class Customer3 {
            public Cities City { get; set; }
        }


        private class Customer4 {
            public void SomeAction(Cities city) {}
        }

        private class Customer5 {
            public Cities? City { get; set; }
        }


        private class Customer6 {
            public void SomeAction(Cities? city) {}
        }

        [Test]
        public void TestEnumAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
        }

        [Test]
        public void TestEnumAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "City");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
        }

        [Test]
        public void TestEnumTypePickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (Cities)});
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
        }

        [Test]
        public void TestEnumTypePickedUpOnNullableActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer6), "SomeAction", new[] {typeof (Cities?)});
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
        }

        [Test]
        public void TestEnumTypePickedUpOnNullableProperty() {
            PropertyInfo property = FindProperty(typeof (Customer5), "City");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
        }

        [Test]
        public void TestEnumTypePickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "City");
            facetFactory.Process(property, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}