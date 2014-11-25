// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class PresentationHintAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PresentationHintAnnotationFacetFactory(0);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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
            public List<Customer3> Customers { get; set; }
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
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestPresentationHintAnnotationNotIgnoredForNonStringsProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IPresentationHintFacet)));
        }

        [Test]
        public void TestPresentationHintAnnotationNotIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof (IPresentationHintFacet)));
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnAction() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacet);
            var maskFacet = (PresentationHintFacet) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacet);
            var maskFacet = (PresentationHintFacet) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacet);
            var maskFacet = (PresentationHintFacet) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnCollectionProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Customers");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacet);
            var maskFacet = (PresentationHintFacet) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }

        [Test]
        public void TestPresentationHintAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPresentationHintFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PresentationHintFacet);
            var maskFacet = (PresentationHintFacet) facet;
            Assert.AreEqual("ahint", maskFacet.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}