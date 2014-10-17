// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Naming.DescribedAs {
    [TestFixture]
    public class DescribedAsAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DescribedAsAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private DescribedAsAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IDescribedAsFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [DescribedAs("some description")]
        private class Customer {}

        private class Customer1 {
            [DescribedAs("some description")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer2 {
            [DescribedAs("some description")]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer3 {
            [DescribedAs("some description")]
            public void SomeAction() {}
        }

        private class Customer4 {
            public void SomeAction([DescribedAs("some description")] int x) {}
        }

        [System.ComponentModel.Description("some description")]
        private class Customer5 {}

        private class Customer6 {
            [System.ComponentModel.Description("some description")]
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer7 {
            [System.ComponentModel.Description("some description")]
            public IList Orders {
                get { return null; }
            }
        }

        private class Customer8 {
            [System.ComponentModel.Description("some description")]
            public void SomeAction() {}
        }

        private class Customer9 {
            public void SomeAction([System.ComponentModel.Description("some description")] int x) {}
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer3), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(actionMethod, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescribedAsAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer8), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnActionParameter() {
            MethodInfo actionMethod = FindMethod(typeof (Customer9), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(actionMethod, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer5), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnCollection() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Orders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestDescriptionAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer6), "NumberOfOrders");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DescribedAsFacetAbstract);
            var describedAsFacetAbstract = (DescribedAsFacetAbstract) facet;
            Assert.AreEqual("some description", describedAsFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}