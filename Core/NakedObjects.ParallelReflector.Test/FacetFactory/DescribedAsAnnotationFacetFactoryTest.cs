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
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class DescribedAsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DescribedAsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IDescribedAsFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

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

        private class Customer2 {
            [DescribedAs("some description")]
            public IList Orders => null;
        }

        private class Customer3 {
            [DescribedAs("some description")]
            public void SomeAction() { }
        }

        private class Customer4 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([DescribedAs("some description")] int x) { }
        }

        [System.ComponentModel.Description("some description")]
        private class Customer5 { }

        private class Customer6 {
            [System.ComponentModel.Description("some description")]
            public int NumberOfOrders => 0;
        }

        private class Customer7 {
            [System.ComponentModel.Description("some description")]
            public IList Orders => null;
        }

        private class Customer8 {
            [System.ComponentModel.Description("some description")]
            public void SomeAction() { }
        }

        private class Customer9 {
// ReSharper disable once UnusedParameter.Local
            public void SomeAction([System.ComponentModel.Description("some description")]
                                   int x) { }
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, actionMethod, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer2), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescribedAsAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer9), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, actionMethod, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer5), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer7), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDescriptionAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer6), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
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
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}