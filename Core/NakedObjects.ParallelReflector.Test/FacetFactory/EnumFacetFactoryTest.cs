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
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class EnumFacetFactoryTest : AbstractFacetFactoryTest {
        private EnumFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IEnumFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        private static void CheckChoices(IFacet facet) {
            var facetAsEnumFacet = facet as IEnumFacet;
            Assert.IsNotNull(facetAsEnumFacet);
            Assert.AreEqual(3, facetAsEnumFacet.GetChoices(null).Length);
            Assert.AreEqual(Cities.London, facetAsEnumFacet.GetChoices(null)[0]);
            Assert.AreEqual(Cities.NewYork, facetAsEnumFacet.GetChoices(null)[1]);
            Assert.AreEqual(Cities.Paris, facetAsEnumFacet.GetChoices(null)[2]);

            Assert.AreEqual(1, facetAsEnumFacet.GetChoices(null, new object[] {Cities.NewYork}).Length);
            Assert.AreEqual(Cities.NewYork, facetAsEnumFacet.GetChoices(null, new object[] {Cities.NewYork})[0]);

            var mock = new Mock<INakedObjectAdapter>();
            var nakedObjectAdapter = mock.Object;
            mock.Setup(no => no.Object).Returns(Cities.NewYork);

            Assert.AreEqual("New York", facetAsEnumFacet.GetTitle(nakedObjectAdapter));
        }

        #region Nested type: Cities

        private enum Cities {
            London,
            Paris,
            NewYork
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [EnumDataType(typeof(Cities))]

            public int City { get; set; }
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            // ReSharper disable UnusedParameter.Local
            public void SomeAction([EnumDataType(typeof(Cities))] int city) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new EnumFacetFactory(new FacetFactoryOrder<EnumFacetFactory>(), LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer3 {
            public Cities City { get; set; }
        }

        private class Customer4 {
            public void SomeAction(Cities city) { }
        }

        private class Customer5 {
            public Cities? City { get; set; }
        }

        private class Customer6 {
            public void SomeAction(Cities? city) { }
        }

        [TestMethod]
        public void TestEnumAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, null,method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestEnumAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "City");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestEnumTypePickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(Cities)});
            metamodel = facetFactory.ProcessParams(Reflector, null,method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestEnumTypePickedUpOnNullableActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Customer6), "SomeAction", new[] {typeof(Cities?)});
            metamodel = facetFactory.ProcessParams(Reflector, null,method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestEnumTypePickedUpOnNullableProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer5), "City");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestEnumTypePickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer3), "City");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IEnumFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is EnumFacet);
            CheckChoices(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}