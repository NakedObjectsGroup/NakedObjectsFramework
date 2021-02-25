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
    public class NotNavigableAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NotNavigableAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(INotNavigableFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "Orders");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNotNavigableAnnotationPickedUpOnType() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer2), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNotNull(metamodel);
            var facet = Specification.GetFacet(typeof(INotNavigableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotNavigableFacet);
            AssertNoMethodsRemoved();
        }

        #region Nested type: Customer

        private class Customer {
            [NotNavigable]
// ReSharper disable once UnusedMember.Local
            public string FirstName => null;
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [NotNavigable]
// ReSharper disable once UnusedMember.Local
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            public NotNavigable FirstName => null;
        }

        #endregion

        #region Nested type: NotNavigable

        [NotNavigable]
        private class NotNavigable { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NotNavigableAnnotationFacetFactory(GetOrder<NotNavigableAnnotationFacetFactory>(), LoggerFactory);
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