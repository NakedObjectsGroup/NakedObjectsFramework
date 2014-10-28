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
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;

using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Facets {
    [TestFixture]
    public class FacetsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new FacetsAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private FacetsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IFacetsFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [Facets(FacetFactoryNames = new[] {
            "NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerFacetFactory",
            "NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerNotAFacetFactory"
        })]
        private class Customer1 {}

        [Facets(FacetFactoryClasses = new[] {typeof (CustomerFacetFactory), typeof (CustomerNotAFacetFactory)})]
        private class Customer2 {}

        [Facets(FacetFactoryNames = new[] {"NakedObjects.Reflector.DotNet.Facets.Objects.Facets.FacetsAnnotationFacetFactoryTest+CustomerFacetFactory"},
            FacetFactoryClasses = new[] {typeof (CustomerFacetFactory2)})]
        private class Customer3 {}

        [Facets]
        private class Customer4 {}

        public class CustomerFacetFactory : FacetFactoryAbstract {
            public CustomerFacetFactory() : base(null, null) {}


            public override FeatureType[] FeatureTypes {
                get { return null; }
            }

            public override bool Process(Type clazz, IMethodRemover methodRemover, ISpecificationBuilder specification) {
                return false;
            }

            public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
                return false;
            }

            public override bool ProcessParams(MethodInfo method, int paramNum, ISpecificationBuilder holder) {
                return false;
            }
        }

        public class CustomerFacetFactory2 : FacetFactoryAbstract {
            public CustomerFacetFactory2() : base(null, null) {}


            public override FeatureType[] FeatureTypes {
                get { return null; }
            }

            public override bool Process(Type clazz, IMethodRemover methodRemover, ISpecificationBuilder specification) {
                return false;
            }

            public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
                return false;
            }

            public override bool ProcessParams(MethodInfo method, int paramNum, ISpecificationBuilder holder) {
                return false;
            }
        }

        public class CustomerNotAFacetFactory {}

        [Test]
        public void TestFacetsFactoryClass() {
            facetFactory.Process(typeof (Customer2), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            var facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(1, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
        }

        [Test]
        public void TestFacetsFactoryNameAndClass() {
            facetFactory.Process(typeof (Customer3), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            var facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(2, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
            Assert.AreEqual(typeof (CustomerFacetFactory2), facetFactories[1]);
        }

        [Test]
        public void TestFacetsFactoryNames() {
            facetFactory.Process(typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IFacetsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FacetsFacetAnnotation);
            var facetsFacet = (FacetsFacetAnnotation) facet;
            Type[] facetFactories = facetsFacet.FacetFactories;
            Assert.AreEqual(1, facetFactories.Length);
            Assert.AreEqual(typeof (CustomerFacetFactory), facetFactories[0]);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestFacetsFactoryNoop() {
            facetFactory.Process(typeof (Customer4), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IFacetsFacet));
            Assert.IsNull(facet);
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