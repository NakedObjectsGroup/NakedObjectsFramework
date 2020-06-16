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
    public class RequiredAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private RequiredAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IMandatoryFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestRequiredAnnotationOnPrimitiveOnActionParameter() {
            var method = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacet);
        }

        [TestMethod]
        public void TestRequiredAnnotationOnPrimitiveOnProperty() {
            var property = FindProperty(typeof(Customer3), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacet);
        }

        [TestMethod]
        public void TestRequiredAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacet);
        }

        [TestMethod]
        public void TestRequiredAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacet);
        }

        #region Nested type: Customer1

        private class Customer1 {
            [Required]
// ReSharper disable once UnusedMember.Local
            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
// ReSharper disable once UnusedMember.Local
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([Required] string foo) { }
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [Required]
// ReSharper disable once UnusedMember.Local
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
// ReSharper disable once UnusedMember.Local
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([Required] int foo) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new RequiredAnnotationFacetFactory(0, LoggerFactory);
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