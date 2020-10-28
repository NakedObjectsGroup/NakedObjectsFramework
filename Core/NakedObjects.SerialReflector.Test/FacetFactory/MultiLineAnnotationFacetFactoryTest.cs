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
using NakedObjects.ParallelReflect.Component;
using NakedObjects.Reflect.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class MultiLineAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MultiLineAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IMultiLineFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestMultiLineAnnotationDefaults() {
            facetFactory.Process(Reflector, typeof(Customer3), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(6, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(0, multiLineFacetAnnotation.Width);
        }

        [TestMethod]
        public void TestMultiLineAnnotationIgnoredForNonStringActionParameters() {
            var method = FindMethod(typeof(Customer6), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            Assert.IsNull(Specification.GetFacet(typeof(IMultiLineFacet)));
        }

        [TestMethod]
        public void TestMultiLineAnnotationIgnoredForNonStringProperties() {
            var property = FindProperty(typeof(Customer5), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnActionParameter() {
            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(8, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(24, multiLineFacetAnnotation.Width);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(3, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(9, multiLineFacetAnnotation.Width);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(12, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(36, multiLineFacetAnnotation.Width);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnAction() {
            var method = FindMethodIgnoreParms(typeof(Customer7), nameof(Customer7.SomeAction));
            facetFactory.Process(Reflector, method, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(1, multiLineFacetAnnotation.NumberOfLines);
        }

        #region Nested type: Customer

        [MultiLine(NumberOfLines = 3, Width = 9)]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [MultiLine(NumberOfLines = 12, Width = 36)]

            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)]
                                   string foo) { }
        }

        #endregion

        #region Nested type: Customer3

        [MultiLine]
        private class Customer3 { }

        #endregion

        #region Nested type: Customer5

        private class Customer5 {
            [MultiLine(NumberOfLines = 8, Width = 24)]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer6

        private class Customer6 {
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)]
                                   int foo) { }
        }

        #endregion

        #region Nested type: Customer7

        private class Customer7 {
            [MultiLine(NumberOfLines = 1)]
            public void SomeAction() { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MultiLineAnnotationFacetFactory(new FacetFactoryOrder<MultiLineAnnotationFacetFactory>(), LoggerFactory);
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