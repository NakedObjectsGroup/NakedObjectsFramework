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

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class FinderActionAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private FinderActionFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IFinderActionFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestFinderActionFacetNullByDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IExecutedFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestFinderActionAnnotationPickedUp() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer1), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IFinderActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FinderActionFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        private class Customer {
            // ReSharper disable once UnusedMember.Local
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [FinderAction]
            // ReSharper disable once UnusedMember.Local
            public void SomeAction() { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new FinderActionFacetFactory(GetOrder<FinderActionFacetFactory>(), LoggerFactory);
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