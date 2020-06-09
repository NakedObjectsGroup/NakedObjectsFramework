// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Spec;
using NakedObjects.Reflect.FacetFactory;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class NamedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NamedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(INamedFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        #region Nested type: Customer

        [Named("some name")]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 {
            [Named("some name")]

            public int NumberOfOrders => 0;
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NamedAnnotationFacetFactory(0, null);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer11 {
            [Named("some name")]
            public int NumberOfOrders => 0;

            [Named("some name")]
            public int NumberOfOrders1 => 0;
        }

        private class Customer2 {
            [Named("some name")]
            public IList Orders => null;
        }

        private class Customer3 {
            [Named("some name")]
            public void SomeAction() { }
        }

        private class Customer13 {
            [Named("some name")]
            public void SomeAction() { }

            [Named("some name")]
            public void SomeAction1() { }
        }

        private class Customer4 {
            // ReSharper disable UnusedParameter.Local
            public void SomeAction([Named("some name")] int x) { }
        }

        [DisplayName("some name")]
        private class Customer5 { }

        private class Customer6 {
            [DisplayName("some name")]
            public int NumberOfOrders => 0;
        }

        private class Customer16 {
            [DisplayName("some name")]
            public int NumberOfOrders => 0;

            [DisplayName("some name")]
            public int NumberOfOrders1 => 0;
        }

        private class Customer7 {
            [DisplayName("some name")]
            public IList Orders => null;
        }

        private class Customer8 {
            [DisplayName("some name")]
            public void SomeAction() { }
        }

        private class Customer18 {
            [DisplayName("some name")]
            public void SomeAction() { }

            [DisplayName("some name")]
            public void SomeAction1() { }
        }

        private class Customer19 {
            [Named("Property")]
            public int SomeProperty { get; set; }

            public int Property { get; set; }
        }

        private readonly ISpecificationBuilder facetHolder1 = new TestSpecification();

        [TestMethod]
        public void ATestDisplayNameAnnotationOnActionIgnoresDuplicate() {
            // these need to run before logs are added by other tests 
            var actionMethod = FindMethod(typeof(Customer18), "SomeAction");
            var actionMethod1 = FindMethod(typeof(Customer18), "SomeAction1");

            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(Reflector, actionMethod1, MethodRemover, facetHolder1);
            facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void ATestNamedAnnotationOnActionIgnoresDuplicate() {
            // these need to run before logs are added by other tests 
            var actionMethod = FindMethod(typeof(Customer13), "SomeAction");
            var actionMethod1 = FindMethod(typeof(Customer13), "SomeAction1");

            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facetFactory.Process(Reflector, actionMethod1, MethodRemover, facetHolder1);
            facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationOnPropertyMarksDuplicate() {
            var property = FindProperty(typeof(Customer16), "NumberOfOrders");
            var property1 = FindProperty(typeof(Customer16), "NumberOfOrders1");

            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            facetFactory.Process(Reflector, property1, MethodRemover, facetHolder1);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer5), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer7), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer6), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestNamedAnnotationOnPropertyMarksDuplicate() {
            var property = FindProperty(typeof(Customer11), "NumberOfOrders");
            var property1 = FindProperty(typeof(Customer11), "NumberOfOrders1");

            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            facetFactory.Process(Reflector, property1, MethodRemover, facetHolder1);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnActionParameter() {
            var actionMethod = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            facetFactory.ProcessParams(Reflector, actionMethod, 0, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnCollection() {
            var property = FindProperty(typeof(Customer2), "Orders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnProperty() {
            var property = FindProperty(typeof(Customer1), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        private class TestSpecificationWithId : Specification {
            public TestSpecificationWithId(IIdentifier identifier) => Identifier = identifier;

            public override IIdentifier Identifier { get; }
        }

        [TestMethod] // for bug #156
        public void TestAnnotationNotPickedUpOnInferredPropertyAfterAnnotatedProperty() {
            var specification1 = new TestSpecification();
            var specification2 = new TestSpecificationWithId(new IdentifierImpl("Customer19", "Property"));
            var property1 = FindProperty(typeof(Customer19), "SomeProperty");
            var property2 = FindProperty(typeof(Customer19), "Property");
            facetFactory.Process(Reflector, property1, MethodRemover, specification1);
            facetFactory.Process(Reflector, property2, MethodRemover, specification2);
            var facet1 = specification1.GetFacet(typeof(INamedFacet));
            var facet2 = specification2.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet1);
            Assert.IsNull(facet2);
            Assert.IsTrue(facet1 is NamedFacetAnnotation);
            var namedFacet1 = (NamedFacetAnnotation) facet1;
            Assert.AreEqual("Property", namedFacet1.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}