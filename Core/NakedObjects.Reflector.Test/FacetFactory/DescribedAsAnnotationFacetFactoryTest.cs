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
    public class DescribedAsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DescribedAsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IDescribedAsFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnActionParameter() {
            var actionMethod = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, actionMethod, 0, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer2), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnActionParameter() {
            var actionMethod = FindMethod(typeof(Customer9), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, actionMethod, 0, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer5), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer7), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer6), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        [DescribedAs("some description")]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [DescribedAs("some description")]

            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            [DescribedAs("some description")]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [DescribedAs("some description")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([DescribedAs("some description")] int x) { }
        }

        #endregion

        #region Nested type: Customer5

        [System.ComponentModel.Description("some description")]
        private class Customer5 { }

        #endregion

        #region Nested type: Customer6

        private class Customer6 {
            [System.ComponentModel.Description("some description")]
            public int NumberOfOrders => 0;
        }

        #endregion

        #region Nested type: Customer7

        private class Customer7 {
            [System.ComponentModel.Description("some description")]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer8

        private class Customer8 {
            [System.ComponentModel.Description("some description")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer9

        private class Customer9 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([System.ComponentModel.Description("some description")]
                                   int x) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DescribedAsAnnotationFacetFactory(0, LoggerFactory);
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