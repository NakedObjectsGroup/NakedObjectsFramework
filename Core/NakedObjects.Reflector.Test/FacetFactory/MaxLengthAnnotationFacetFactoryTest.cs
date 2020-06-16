// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class MaxLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MaxLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IMaxLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestMaxLengthAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer8), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [TestMethod]
        public void TestMaxLengthAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer7), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestNofMaxLengthAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [TestMethod]
        public void TestNofMaxLengthAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        [TestMethod]
        public void TestStringLengthAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer5), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
        }

        [TestMethod]
        public void TestStringLengthAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer4), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
        }

        #region Nested type: Customer1

        private class Customer1 {
            [MaxLength(30)]

            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public void SomeAction([MaxLength(20)] string foo) { }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
            [StringLength(30)]
            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer5

        private class Customer5 {
            public void SomeAction([StringLength(20)] string foo) { }
        }

        #endregion

        #region Nested type: Customer7

        private class Customer7 {
            [MaxLength(30)]
            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer8

        private class Customer8 {
            public void SomeAction([MaxLength(20)] string foo) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaxLengthAnnotationFacetFactory(0, LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}