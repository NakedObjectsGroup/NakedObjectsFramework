// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class HiddenAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private HiddenAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IHiddenFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        #region Nested type: Customer

        private class Customer {
            [Hidden(WhenTo.Always)]
// ReSharper disable UnusedMember.Local
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new HiddenAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer1 {
            [Hidden(WhenTo.Always)]
            public IList Orders => null;
        }

        private class Customer2 {
            [Hidden(WhenTo.Always)]
            public void SomeAction() { }
        }

        private class Customer3 {
            [Hidden(WhenTo.Always)]
            public void SomeAction() { }
        }

        private class Customer4 {
            [Hidden(WhenTo.Never)]
            public void SomeAction() { }
        }

        private class Customer5 {
            [Hidden(WhenTo.OncePersisted)]
            public void SomeAction() { }
        }

        private class Customer6 {
            [Hidden(WhenTo.UntilPersisted)]
            public void SomeAction() { }
        }

        private class Customer7 {
            [ScaffoldColumn(false)]
            public int NumberOfOrders => 0;
        }

        private class Customer8 {
            [ScaffoldColumn(false)]
            public IList Orders => null;
        }

        private class Customer9 {
            [ScaffoldColumn(true)]
            public int NumberOfOrders => 0;
        }

        private class Customer10 {
            [Hidden(WhenTo.Always)]
            [ScaffoldColumn(true)]
            public int NumberOfOrders => 0;
        }

        [TestMethod]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer6), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, hiddenFacetAbstract.Value);
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
            var actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestHiddenAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer1), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestHiddenAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestHiddenWhenAlwaysAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Always, hiddenFacetAbstract.Value);
        }

        [TestMethod]
        public void TestHiddenWhenNeverAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer4), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Never, hiddenFacetAbstract.Value);
        }

        [TestMethod]
        public void TestHiddenWhenOncePersistedAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer5), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.OncePersisted, hiddenFacetAbstract.Value);
        }

        [TestMethod]
        public void TestHiidenPriorityOverScaffoldAnnotation() {
            var property = FindProperty(typeof(Customer10), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Always, hiddenFacetAbstract.Value);
        }

        [TestMethod]
        public void TestScaffoldAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer8), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestScaffoldAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer7), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HiddenFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestScaffoldTrueAnnotationPickedUpOn() {
            var property = FindProperty(typeof(Customer9), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHiddenFacet));
            var hiddenFacetAbstract = (HiddenFacet) facet;
            Assert.AreEqual(WhenTo.Never, hiddenFacetAbstract.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}