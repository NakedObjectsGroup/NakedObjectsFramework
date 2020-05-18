// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class TypeOfAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private TypeOfAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(ITypeOfFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestTypeOfFacetInferredForActionWithArrayReturnType() {
            var actionMethod = FindMethod(typeof(Customer9), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var typeOfFacet = Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromArray);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof(Order), elementTypeFacet.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestTypeOfFacetInferredForActionWithGenericCollectionReturnType() {
            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var typeOfFacet = Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromGenerics);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof(Order), elementTypeFacet.Value);
        }

        [TestMethod]
        public void TestTypeOfFacetInferredForCollectionWithGenericCollectionReturnType() {
            var property = FindProperty(typeof(Customer4), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var typeOfFacet = Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromGenerics);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof(Order), elementTypeFacet.Value);
        }

        [TestMethod]
        public void TestTypeOfFacetIsInferredForCollectionFromOrderArray() {
            var property = FindProperty(typeof(Customer10), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var typeOfFacet = Specification.GetFacet(typeof(ITypeOfFacet));
            Assert.IsNotNull(typeOfFacet);
            Assert.IsTrue(typeOfFacet is TypeOfFacetInferredFromArray);

            var elementTypeFacet = Specification.GetFacet<IElementTypeFacet>();
            Assert.IsNotNull(elementTypeFacet);
            Assert.IsTrue(elementTypeFacet is ElementTypeFacet);
            Assert.AreEqual(typeof(Order), elementTypeFacet.Value);
        }

        #region Nested type: Customer10

        private class Customer10 {
// ReSharper disable once UnusedMember.Local
            public Order[] Orders => null;
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
// ReSharper disable once UnusedMember.Local
            public IList<Order> SomeAction() => null;
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
// ReSharper disable once UnusedMember.Local
            public IList<Order> Orders => null;
        }

        #endregion

        #region Nested type: Customer9

        private class Customer9 {
// ReSharper disable once UnusedMember.Local
            public Order[] SomeAction() => null;
        }

        #endregion

        #region Nested type: Order

        private class Order { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypeOfAnnotationFacetFactory(0);
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