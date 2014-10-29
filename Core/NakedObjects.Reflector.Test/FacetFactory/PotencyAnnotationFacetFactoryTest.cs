// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class PotencyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PotencyAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PotencyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IQueryOnlyFacet), typeof (IIdempotentFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [QueryOnly]
            public void SomeAction() {}
        }

        private class Customer1 {
            [Idempotent]
            public void SomeAction() {}
        }

        private class Customer2 {
            public void SomeAction() {}
        }

        private class Customer3 {
            [QueryOnly, Idempotent]
            public void SomeAction() {}
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestIdempotentAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestIdempotentPriorityAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IIdempotentFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IdempotentFacet);
            facet = Specification.GetFacet(typeof (IQueryOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestNoAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer2), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IQueryOnlyFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IIdempotentFacet));
            Assert.IsNull(facet);

            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestQueryOnlyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IQueryOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is QueryOnlyFacet);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}