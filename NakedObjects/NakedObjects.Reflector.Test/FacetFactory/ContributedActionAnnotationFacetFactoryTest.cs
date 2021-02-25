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

namespace NakedObjects.Reflector.Test.FacetFactory {
    /// <summary>
    ///     Note: This is a limited test;  it does not test collection-contributed actions
    ///     due to dependency on other facets.  That is done in system tests
    /// </summary>
    [TestClass]
    public class ContributedActionAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private ContributedActionAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IContributedActionFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        //Note: The [ContributedAction] annotation is applied to the parameter,
        //but the facet is applied to the Action (if any of its params have that annotation)
        [TestMethod]
        public void TestContributedAnnotationNullByDefault1() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Service), "Action1");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet<IContributedActionFacet>();
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestContributedAnnotationNullByDefault2() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethodIgnoreParms(typeof(Service), "Action2");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet<IContributedActionFacet>();
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestContributedAnnotationPickedUp3() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethodIgnoreParms(typeof(Service), "Action3");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet<IContributedActionFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ContributedActionFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestContributedAnnotationPickedUp4() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethodIgnoreParms(typeof(Service), "Action4");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet<IContributedActionFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ContributedActionFacet);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Customer

        // ReSharper disable once ClassNeverInstantiated.Local
        private class Customer { }

        #endregion

        #region Nested type: Service

        private class Service {
            // ReSharper disable once UnusedMember.Local
            public void Action1() { }

            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void Action2(Customer cust1) { }

            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            public void Action3([ContributedAction] Customer cust1) { }

            // ReSharper disable once UnusedMember.Local
            // ReSharper disable UnusedParameter.Local
            public void Action4(string str1, [ContributedAction] Customer cust1) { }
            // ReSharper restore UnusedParameter.Local
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ContributedActionAnnotationFacetFactory(GetOrder<ContributedActionAnnotationFacetFactory>(), LoggerFactory);
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