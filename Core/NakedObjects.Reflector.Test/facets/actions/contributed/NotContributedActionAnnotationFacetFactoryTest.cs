// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Moq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class NotContributedActonAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ContributedActionAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ContributedActionAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (INotContributedActionFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [NotContributedAction]
            public void SomeAction() {}
        }

        private class Customer1 {
            public void SomeAction() {}
        }

        private class Customer2 {
            [NotContributedAction(typeof (Customer2))]
            public void SomeAction() {}
        }

        private class Customer3 {
            [NotContributedAction(typeof (Customer2), typeof (Customer3))]
            public void SomeAction() {}
        }


        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag( FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag( FeatureType.ActionParameter));
        }

        [Test]
        public void TestNotContributedAnnotationNullByDefault() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IExecutedFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNotContributedAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = (INotContributedActionFacet) Specification.GetFacet(typeof (INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacet);

            Assert.IsTrue(facet.NeverContributed());

            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNotContributedAnnotationPickedUpWithType() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = (INotContributedActionFacet) Specification.GetFacet(typeof (INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacet);

            var sp = new Mock<IObjectSpecImmutable>();
            sp.Setup(s => s.IsOfType(null)).Returns(true);

            Assert.IsTrue(facet.NotContributedTo(sp.Object));

            AssertNoMethodsRemoved();
        }


        [Test]
        public void TestNotContributedAnnotationPickedUpWithTypes() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = (INotContributedActionFacet) Specification.GetFacet(typeof (INotContributedActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotContributedActionFacet);

            var sp = new Mock<IObjectSpecImmutable>();
            sp.Setup(s => s.IsOfType(null)).Returns(true);
            var sp1 = new Mock<IObjectSpecImmutable>();
            sp1.Setup(s => s.IsOfType(null)).Returns(true);

            Assert.IsTrue(facet.NotContributedTo(sp.Object));
            Assert.IsTrue(facet.NotContributedTo(sp1.Object));

            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}