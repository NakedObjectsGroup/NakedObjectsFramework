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
using NakedObjects.Metamodel.Facet;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class ExecutedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ExecutedAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ExecutedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IExecutedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [Executed(Where.Locally)]
            public void SomeAction() {}
        }

        private class Customer1 {
            [Executed(Where.Remotely)]
            public void SomeAction() {}
        }

        [Test]
        public void TestExecutedLocallyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IExecutedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ExecutedFacetAbstract);
            var executedFacetAbstract = (ExecutedFacetAbstract) facet;
            Assert.AreEqual(Where.Locally, executedFacetAbstract.ExecutedWhere());
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestExecutedRemotelyAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IExecutedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ExecutedFacetAbstract);
            var executedFacetAbstract = (ExecutedFacetAbstract) facet;
            Assert.AreEqual(Where.Remotely, executedFacetAbstract.ExecutedWhere());
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}