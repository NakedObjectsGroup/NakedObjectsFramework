// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class MultiLineAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MultiLineAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IMultiLineFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

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

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MultiLineAnnotationFacetFactory(0, null);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer2 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)]
                                   string foo) { }
        }

        [MultiLine]
        private class Customer3 { }

        private class Customer5 {
            [MultiLine(NumberOfLines = 8, Width = 24)]
            public int NumberOfOrders => 0;
        }

        private class Customer6 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([MultiLine(NumberOfLines = 8, Width = 24)]
                                   int foo) { }
        }

        private class Customer7 {
            [MultiLine(NumberOfLines = 1)]
            public void SomeAction() { }
        }

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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer3), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(6, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(0, multiLineFacetAnnotation.Width);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationIgnoredForNonStringActionParameters() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer6), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(IMultiLineFacet)));
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationIgnoredForNonStringProperties() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer5), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(8, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(24, multiLineFacetAnnotation.Width);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(3, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(9, multiLineFacetAnnotation.Width);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(12, multiLineFacetAnnotation.NumberOfLines);
            Assert.AreEqual(36, multiLineFacetAnnotation.Width);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMultiLineAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethodIgnoreParms(typeof(Customer7), nameof(Customer7.SomeAction));
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMultiLineFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MultiLineFacetAnnotation);
            var multiLineFacetAnnotation = (MultiLineFacetAnnotation) facet;
            Assert.AreEqual(1, multiLineFacetAnnotation.NumberOfLines);
            Assert.IsNotNull(metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}