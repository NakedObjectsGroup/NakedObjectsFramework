// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;
using NakedObjects.Metamodel.SpecImmutable;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    [TestFixture]
    public class PageSizeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PageSizeAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private PageSizeAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IPageSizeFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer {
            [PageSize(7)]
            public IQueryable<Customer> SomeAction() {
                return null;
            }
        }

        private class Customer1 {
            public IQueryable<Customer1> SomeAction() {
                return null;
            }
        }

        [Test]
        public void TestDefaultPageSizePickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            var actionPeer = new ActionSpecImmutable(null, null, null);
            new FallbackFacetFactory(Reflector).Process(actionMethod, MethodRemover, actionPeer);
            IFacet facet = actionPeer.GetFacet(typeof (IPageSizeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PageSizeFacetDefault);
            var pageSizeFacet = (IPageSizeFacet) facet;
            Assert.AreEqual(20, pageSizeFacet.Value);
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

        [Test]
        public void TestPageSizeAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPageSizeFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PageSizeFacetAnnotation);
            var pageSizeFacet = (IPageSizeFacet) facet;
            Assert.AreEqual(7, pageSizeFacet.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}