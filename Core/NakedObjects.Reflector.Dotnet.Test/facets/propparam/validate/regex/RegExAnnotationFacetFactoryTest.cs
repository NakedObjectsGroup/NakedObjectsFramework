// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx {
    [TestFixture]
    public class RegExAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private RegExAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new Type[] {typeof (IRegExFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new RegExAnnotationFacetFactory { Reflector = reflector };
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #region regex tests

        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Class message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Property message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new Type[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegExAnnotationMessageNullWhenNotSpecified() {
            PropertyInfo property = FindProperty(typeof(Customer5), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IRegExFacet));
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation)facet;
            Assert.AreEqual(null, regExFacet.FailureMessage);
        }

        [Test]
        public void TestRegExAnnotationIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (IRegExFacet)));
        }

        [Test]
        public void TestRegExAnnotationIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new Type[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof (IRegExFacet)));
        }

        #region Nested Type: Customer

        [RegEx(Validation = "^A.*", Message="Class message", CaseSensitive = false)]
        private class Customer {}

        #endregion

        #region Nested Type: Customer1

        private class Customer1 {
            [RegEx(Validation = "^A.*",  Message="Property message", CaseSensitive = false)]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer2

        private class Customer2 {
            public void SomeAction([RegEx(Validation = "^A.*",  Message="Parameter message", CaseSensitive = false)] string foo) {}
        }

        #endregion

        #region Nested Type: Customer3

        private class Customer3 {
            [RegEx(Validation = "^A.*", CaseSensitive = false)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer4

        private class Customer4 {
            public void SomeAction([RegEx(Validation = "^A.*", CaseSensitive = false)] int foo) {}
        }

        #endregion

        #region Nested Type: Customer5

        private class Customer5 {
            [RegEx(Validation = "^A.*", CaseSensitive = false)]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #endregion

        #region regular expression tests

        [Test]
        public void TestRegularExpressionAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof(Customer7), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation)facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Property message", regExFacet.FailureMessage);
            Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegularExpressionAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof(Customer8), "SomeAction", new Type[] { typeof(string) });
            facetFactory.ProcessParams(method, 0, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation)facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
            Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegularExpressionAnnotationMessageNullWhenNotSpecified() {
            PropertyInfo property = FindProperty(typeof(Customer11), "FirstName");
            facetFactory.Process(property, methodRemover, facetHolder);
            IFacet facet = facetHolder.GetFacet(typeof(IRegExFacet));
            RegExFacetAnnotation regExFacet = (RegExFacetAnnotation)facet;
            Assert.AreEqual(null, regExFacet.FailureMessage);
        }

        [Test]
        public void TestRegularExpressionAnnotationIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof(Customer9), "NumberOfOrders");
            facetFactory.Process(property, methodRemover, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof(IRegExFacet)));
        }

        [Test]
        public void TestRegularExpressionAnnotationIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof(Customer10), "SomeAction", new Type[] { typeof(int) });
            facetFactory.ProcessParams(method, 0, facetHolder);
            Assert.IsNull(facetHolder.GetFacet(typeof(IRegExFacet)));
        }

      

        #region Nested Type: Customer7

        private class Customer7 {
            [RegularExpression("^A.*", ErrorMessage = "Property message")]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion

        #region Nested Type: Customer8

        private class Customer8 {
            public void SomeAction([RegularExpression("^A.*", ErrorMessage = "Parameter message")] string foo) { }
        }

        #endregion

        #region Nested Type: Customer9

        private class Customer9 {
            [RegularExpression("^A.*")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        #endregion

        #region Nested Type: Customer10

        private class Customer10 {
            public void SomeAction([RegularExpression("^A.*")] int foo) { }
        }

        #endregion

        #region Nested Type: Customer11

        private class Customer11 {
            [RegularExpression("^A.*")]
            public string FirstName {
                get { return null; }
            }
        }

        #endregion


        #endregion 
    }

    // Copyright (c) Naked Objects Group Ltd.
}