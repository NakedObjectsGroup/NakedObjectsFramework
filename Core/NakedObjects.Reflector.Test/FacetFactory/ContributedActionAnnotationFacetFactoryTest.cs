// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;


namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass, Ignore] //TODO: Write tests (the code here is just borrowed from 'NotContributed')
    public class ContributedActonAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ContributedActionAnnotationFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ContributedActionAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IContributedActionFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            public void SomeAction() {}
        }

        private class Customer1 {
            public void SomeAction() {}
        }

        private class Customer2 {
            public void SomeAction() {}
        }

        private class Customer3 {
            public void SomeAction() {}
        }


        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [TestMethod]
        public void TestContributedAnnotationNullByDefault() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IExecutedFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestContributedAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = (IContributedActionFacet)Specification.GetFacet(typeof(IContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ContributedActionFacet);

          
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestContributedAnnotationPickedUpWithType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = (IContributedActionFacet) Specification.GetFacet(typeof (IContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ContributedActionFacet);

            var sp = new Mock<IObjectSpecImmutable>();
            sp.Setup(s => s.IsOfType(null)).Returns(true);


            AssertNoMethodsRemoved();
        }


        [TestMethod]
        public void TestNotContributedAnnotationPickedUpWithTypes() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = (IContributedActionFacet) Specification.GetFacet(typeof (IContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ContributedActionFacet);

            var sp = new Mock<IObjectSpecImmutable>();
            sp.Setup(s => s.IsOfType(null)).Returns(true);
            var sp1 = new Mock<IObjectSpecImmutable>();
            sp1.Setup(s => s.IsOfType(null)).Returns(true);

            Assert.IsTrue(facet.IsContributedTo(sp.Object));
            Assert.IsTrue(facet.IsContributedTo(sp1.Object));

            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}