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
    public class TypicalLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private TypicalLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(ITypicalLengthFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

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
        public void TestTypicalLengthAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(20, typicalLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTypicalLengthAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(16, typicalLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTypicalLengthAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(30, typicalLengthFacetAnnotation.Value);
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer

        [TypicalLength(16)]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [TypicalLength(30)]
            // ReSharper disable once UnusedMember.Local
            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void SomeAction([TypicalLength(20)] int foo) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypicalLengthAnnotationFacetFactory(0, LoggerFactory);
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