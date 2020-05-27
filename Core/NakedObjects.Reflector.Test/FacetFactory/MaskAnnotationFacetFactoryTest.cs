// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class MaskAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MaskAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IMaskFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        #region Nested type: Customer

        [Mask("###")]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [Mask("###")]

            public string FirstName => null;
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaskAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer2 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([Mask("###")] string foo) { }
        }

        private class Customer3 {
            [Mask("###")]
            public int NumberOfOrders => 0;
        }

        private class Customer4 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([Mask("###")] int foo) { }
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
        public void TestMaskAnnotationNotIgnoredForNonStringsProperty() {
            var property = FindProperty(typeof(Customer3), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof(IMaskFacet)));
        }

        [TestMethod]
        public void TestMaskAnnotationNotIgnoredForPrimitiveOnActionParameter() {
            var method = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            Assert.IsNotNull(Specification.GetFacet(typeof(IMaskFacet)));
        }

        [TestMethod]
        public void TestMaskAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacet);
            var maskFacet = (MaskFacet) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [TestMethod]
        public void TestMaskAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacet);
            var maskFacet = (MaskFacet) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }

        [TestMethod]
        public void TestMaskAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMaskFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaskFacet);
            var maskFacet = (MaskFacet) facet;
            Assert.AreEqual("###", maskFacet.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}