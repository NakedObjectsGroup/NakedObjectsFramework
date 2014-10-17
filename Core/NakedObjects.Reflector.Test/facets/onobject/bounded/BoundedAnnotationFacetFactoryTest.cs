// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Bounded {
    [TestFixture]
    public class BoundedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new BoundedAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private BoundedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IBoundedFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Bounded]
        private class Customer {}

        [Test]
        public void TestBoundedAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IBoundedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is BoundedFacetAbstract);
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}