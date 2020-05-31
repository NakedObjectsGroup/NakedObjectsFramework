// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
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
    public class MaxLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private MaxLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IMaxLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        #region Nested type: Customer1

        private class Customer1 {
            [MaxLength(30)]

            public string FirstName => null;
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new MaxLengthAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer2 {
            // ReSharper disable UnusedParameter.Local
            public void SomeAction([MaxLength(20)] string foo) { }
        }

        private class Customer4 {
            [StringLength(30)]
            public string FirstName => null;
        }

        private class Customer5 {
            public void SomeAction([StringLength(20)] string foo) { }
        }

        private class Customer7 {
            [MaxLength(30)]
            public string FirstName => null;
        }

        private class Customer8 {
            public void SomeAction([MaxLength(20)] string foo) { }
        }

        [TestMethod]
        public void TestMaxLengthAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer8), "SomeAction", new[] {typeof(string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestMaxLengthAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer7), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNofMaxLengthAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestStringLengthAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer5), "SomeAction", new[] {typeof(string)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(20, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestStringLengthAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer4), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IMaxLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MaxLengthFacetAnnotation);
            var maxLengthFacetAnnotation = (MaxLengthFacetAnnotation) facet;
            Assert.AreEqual(30, maxLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}