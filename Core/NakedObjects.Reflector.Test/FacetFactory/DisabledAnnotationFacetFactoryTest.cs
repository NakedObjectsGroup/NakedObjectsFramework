// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
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
    public class DisabledAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DisabledAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IDisabledFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer2), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer1), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisabledAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisabledWhenAlwaysAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Always, disabledFacetAbstract.Value);
        }

        [TestMethod]
        public void TestDisabledWhenNeverAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer4), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.Never, disabledFacetAbstract.Value);
        }

        [TestMethod]
        public void TestDisabledWhenOncePersistedAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer5), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.OncePersisted, disabledFacetAbstract.Value);
        }

        [TestMethod]
        public void TestDisabledWhenUntilPersistedAnnotationPickedUpOn() {
            var actionMethod = FindMethod(typeof(Customer6), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDisabledFacet));
            var disabledFacetAbstract = (DisabledFacetAbstract) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, disabledFacetAbstract.Value);
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
            facetFactory = new DisabledAnnotationFacetFactory(0, LoggerFactory);
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