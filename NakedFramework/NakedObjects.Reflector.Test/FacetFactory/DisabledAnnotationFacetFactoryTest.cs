// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.DependencyInjection.FacetFactory;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflector.FacetFactory;
using NakedObjects.Reflector.Reflect;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class DisabledAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DisabledAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IDisabledFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            metamodel = facetFactory.Process(Reflector,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders");
            metamodel = facetFactory.Process(Reflector,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledWhenAlwaysAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            metamodel = facetFactory.Process(Reflector,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Always, disabledFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledWhenNeverAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer4), "SomeAction");
            metamodel = facetFactory.Process(Reflector,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Never, disabledFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledWhenOncePersistedAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer5), "SomeAction");
            metamodel = facetFactory.Process(Reflector,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.OncePersisted, disabledFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer6), "SomeAction");
            metamodel = facetFactory.Process(Reflector,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, disabledFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        private class Customer {
            [Disabled]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [Disabled]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            [Disabled]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [Disabled(WhenTo.Always)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
            [Disabled(WhenTo.Never)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer5

        private class Customer5 {
            [Disabled(WhenTo.OncePersisted)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer6

        private class Customer6 {
            [Disabled(WhenTo.UntilPersisted)]
            public void SomeAction() { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DisabledAnnotationFacetFactory(GetOrder<DisabledAnnotationFacetFactory>(), LoggerFactory);
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