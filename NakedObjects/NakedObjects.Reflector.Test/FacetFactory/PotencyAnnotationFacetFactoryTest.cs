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
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class PotencyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private PotencyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IQueryOnlyFacet), typeof(IIdempotentFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestIdempotentAnnotationPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer1), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestIdempotentPriorityAnnotationPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacet);
            facet = Specification.GetFacet(typeof(IQueryOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoAnnotationPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IQueryOnlyFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IIdempotentFacet));
            Assert.IsNull(facet);

            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestQueryOnlyAnnotationPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IQueryOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is QueryOnlyFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer

        private class Customer {
            [QueryOnly]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [Idempotent]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [QueryOnly]
            [Idempotent]
            public void SomeAction() { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PotencyAnnotationFacetFactory(GetOrder<PotencyAnnotationFacetFactory>(), LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}