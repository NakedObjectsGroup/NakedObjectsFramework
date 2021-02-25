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
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class HiddenAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private HiddenAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IHiddenFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer6), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, hiddenFacetAbstract.Value);
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

        [TestMethod]
        public void TestHiddenAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiddenAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiddenAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiddenWhenAlwaysAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Always, hiddenFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiddenWhenNeverAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer4), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Never, hiddenFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiddenWhenOncePersistedAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer5), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.OncePersisted, hiddenFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHiidenPriorityOverScaffoldAnnotation() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer10), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Always, hiddenFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestScaffoldAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer8), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestScaffoldAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer7), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestScaffoldTrueAnnotationPickedUpOn() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer9), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Never, hiddenFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer

        private class Customer {
            [Hidden(WhenTo.Always)]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [Hidden(WhenTo.Always)]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer10

        private class Customer10 {
            [Hidden(WhenTo.Always)]
            [ScaffoldColumn(true)]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            [Hidden(WhenTo.Always)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [Hidden(WhenTo.Always)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
            [Hidden(WhenTo.Never)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer5

        private class Customer5 {
            [Hidden(WhenTo.OncePersisted)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer6

        private class Customer6 {
            [Hidden(WhenTo.UntilPersisted)]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer7

        private class Customer7 {
            [ScaffoldColumn(false)]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer8

        private class Customer8 {
            [ScaffoldColumn(false)]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer9

        private class Customer9 {
            [ScaffoldColumn(true)]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new HiddenAnnotationFacetFactory(GetOrder<HiddenAnnotationFacetFactory>(), LoggerFactory);
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