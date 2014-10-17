// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx {
    [TestFixture]
    public class RegExAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new RegExAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private RegExAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IRegExFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [RegEx(Validation = "^A.*", Message = "Class message", CaseSensitive = false)]
        private class Customer {}

        private class Customer1 {
            [RegEx(Validation = "^A.*", Message = "Property message", CaseSensitive = false)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void SomeAction([RegEx(Validation = "^A.*", Message = "Parameter message", CaseSensitive = false)] string foo) {}
        }

        private class Customer3 {
            [RegEx(Validation = "^A.*", CaseSensitive = false)]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer4 {
            public void SomeAction([RegEx(Validation = "^A.*", CaseSensitive = false)] int foo) {}
        }

        private class Customer5 {
            [RegEx(Validation = "^A.*", CaseSensitive = false)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer7 {
            [RegularExpression("^A.*", ErrorMessage = "Property message")]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer8 {
            public void SomeAction([RegularExpression("^A.*", ErrorMessage = "Parameter message")] string foo) {}
        }

        private class Customer9 {
            [RegularExpression("^A.*")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer10 {
            public void SomeAction([RegularExpression("^A.*")] int foo) {}
        }

        private class Customer11 {
            [RegularExpression("^A.*")]
            public string FirstName {
                get { return null; }
            }
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestRegExAnnotationIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IRegExFacet)));
        }

        [Test]
        public void TestRegExAnnotationIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IRegExFacet)));
        }

        [Test]
        public void TestRegExAnnotationMessageNullWhenNotSpecified() {
            PropertyInfo property = FindProperty(typeof (Customer5), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual(null, regExFacet.FailureMessage);
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Class message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegExAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Property message", regExFacet.FailureMessage);
            Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegularExpressionAnnotationIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer9), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IRegExFacet)));
        }

        [Test]
        public void TestRegularExpressionAnnotationIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer10), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IRegExFacet)));
        }

        [Test]
        public void TestRegularExpressionAnnotationMessageNullWhenNotSpecified() {
            PropertyInfo property = FindProperty(typeof (Customer11), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual(null, regExFacet.FailureMessage);
        }

        [Test]
        public void TestRegularExpressionAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer8), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
            Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        }

        [Test]
        public void TestRegularExpressionAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer7), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IRegExFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is RegExFacetAnnotation);
            var regExFacet = (RegExFacetAnnotation) facet;
            Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
            Assert.AreEqual("Property message", regExFacet.FailureMessage);
            Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}