// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Presentation;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Presentation {
    [TestFixture]
    public class PresentationHintAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PresentationHintAnnotationFacetFactory {Reflector = reflector};
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        private PresentationHintAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPresentationHintFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [PresentationHint("ahint")]
        private class Customer {}

        private class Customer1 {
            [PresentationHint("ahint")]
            public string FirstName {
                get { return null; }
            }
            [PresentationHint("ahint")]
            public List<Customer3> Customers {
                get;
                set;
            }
        }

        private class Customer2 {
            [PresentationHint("ahint")]
            public void SomeAction([PresentationHint("ahint")] string foo) {}
        }

        private class Customer3 {
            [PresentationHint("ahint")]
            public int NumberOfOrders {
                get { return 0; }
            }
            
        }

        private class Customer4 {
            public void SomeAction([PresentationHint("ahint")] int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestPresentationHintAnnotationNotIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IPresentationHintFacet)));
        }

        [Test]
        public void TestPresentationHintAnnotationNotIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            Assert.IsNotNull(facetHolder.GetFacet(typeof (IPresentationHintFacet)));
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnAction() {
            MethodInfo method = FindMethod(typeof(Customer2), "SomeAction", new[] { typeof(string) });
            facetFactory.Process(method, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacetAnnotation);
            var maskFacet = (PresentationHintFacetAnnotation)facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacetAnnotation);
            var maskFacet = (PresentationHintFacetAnnotation) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacetAnnotation);
            var maskFacet = (PresentationHintFacetAnnotation) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacetAnnotation);
            var maskFacet = (PresentationHintFacetAnnotation) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnCollectionProperty() {
            PropertyInfo property = FindProperty(typeof(Customer1), "Customers");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacetAnnotation);
            var maskFacet = (PresentationHintFacetAnnotation)facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}