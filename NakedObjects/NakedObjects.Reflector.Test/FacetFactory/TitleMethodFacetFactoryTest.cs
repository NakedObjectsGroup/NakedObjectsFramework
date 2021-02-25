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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class TitleMethodFacetFactoryTest : AbstractFacetFactoryTest {
        private TitleMethodFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(ITitleFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestNoExplicitTitleOrToStringMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, typeof(Customer2), MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(ITitleFacet)));
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestTitleMethodPickedUpOnClassAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var titleMethod = FindMethod(typeof(Customer), "Title");
            metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaTitleMethod);
            var titleFacetViaTitleMethod = (TitleFacetViaTitleMethod) facet;
            Assert.AreEqual(titleMethod, titleFacetViaTitleMethod.GetMethod());
            AssertMethodRemoved(titleMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestToStringMethodPickedUpOnClassAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var toStringMethod = FindMethod(typeof(Customer1), "ToString", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, typeof(Customer1), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(ITitleFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TitleFacetViaToStringMethod);
            var titleFacetViaTitleMethod = (TitleFacetViaToStringMethod) facet;
            Assert.AreEqual(toStringMethod, titleFacetViaTitleMethod.GetMethod());
            AssertMethodRemoved(toStringMethod);
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer

        private class Customer {
            // ReSharper disable once UnusedMember.Local
            public string Title()
            {
                return "Some title";
            }
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            public override string ToString()
            {
                return "Some title via ToString";
            }

            // ReSharper disable once UnusedParameter.Local
            // ReSharper disable once UnusedMember.Local
            public string ToString(string mask)
            {
                return "Some title via ToString";
            }
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TitleMethodFacetFactory(GetOrder<TitleMethodFacetFactory>(), LoggerFactory);
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