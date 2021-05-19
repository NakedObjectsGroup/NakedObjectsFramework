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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedObjects.Reflector.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory {
    [TestClass]
    public class DisplayAsPropertyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DisplayAsPropertyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IDisplayAsPropertyFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestDisplayAsPropertyAnnotationPickedUpOnAction()
        {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer), nameof(Customer.DisplayAsPropertyTest));
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet1 = Specification.GetFacet(typeof(IDisplayAsPropertyFacet));
            var facet2 = Specification.GetFacet(typeof(IPropertyAccessorFacet));
            var facet3 = Specification.GetFacet(typeof(IMandatoryFacet));
            Assert.IsNotNull(facet1);
            Assert.IsNotNull(facet2);
            Assert.IsNotNull(facet3);
            Assert.IsTrue(facet1 is DisplayAsPropertyFacet);
            Assert.IsTrue(facet2 is PropertyAccessorFacetViaMethod);
            Assert.IsTrue(facet3 is MandatoryFacetDefault);
            AssertMethodRemoved(actionMethod);
            Assert.IsNotNull(metamodel);
        }


        [TestMethod]
        public override void TestFeatureTypes()
        {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: Customer

        private class Customer {
            [DisplayAsProperty]
            public Customer DisplayAsPropertyTest() => this;
        }

        
        #endregion


        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DisplayAsPropertyAnnotationFacetFactory(GetOrder<DisplayAsPropertyAnnotationFacetFactory>(), LoggerFactory);
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