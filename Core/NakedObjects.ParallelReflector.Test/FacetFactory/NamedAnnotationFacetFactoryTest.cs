// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Spec;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class NamedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private NamedAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(INamedFacet)};

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

        #region Nested type: Customer11

        private class Customer11 {
            [Named("some name")]
            public int NumberOfOrders => 0;

            [Named("some name")]
            public int NumberOfOrders1 => 0;
        }

        #endregion

        #region Nested type: Customer13

        private class Customer13 {
            [Named("some name")]
            public void SomeAction() { }

            [Named("some name")]
            public void SomeAction1() { }
        }

        #endregion

        #region Nested type: Customer2

        private class Customer2 {
            [Named("some name")]
            public IList Orders => null;
        }

        #endregion

        #region Nested type: Customer3

        private class Customer3 {
            [Named("some name")]
            public void SomeAction() { }
        }

        #endregion

        #region Nested type: Customer4

        private class Customer4 {
            // ReSharper disable UnusedParameter.Local
            public void SomeAction([Named("some name")] int x) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new NamedAnnotationFacetFactory(new FacetFactoryOrder<NamedAnnotationFacetFactory>(), LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            // these need to run before logs are added by other tests 
            var actionMethod = FindMethod(typeof(Customer18), "SomeAction");
            var actionMethod1 = FindMethod(typeof(Customer18), "SomeAction1");

            metamodel = facetFactory.Process(Reflector, null,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);

            metamodel = facetFactory.Process(Reflector, null,actionMethod1, MethodRemover, facetHolder1, metamodel);
            facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void ATestNamedAnnotationOnActionIgnoresDuplicate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            // these need to run before logs are added by other tests 
            var actionMethod = FindMethod(typeof(Customer13), "SomeAction");
            var actionMethod1 = FindMethod(typeof(Customer13), "SomeAction1");

            metamodel = facetFactory.Process(Reflector, null,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);

            metamodel = facetFactory.Process(Reflector, null,actionMethod1, MethodRemover, facetHolder1, metamodel);
            facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisplayNameAnnotationOnPropertyMarksDuplicate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer16), "NumberOfOrders");
            var property1 = FindProperty(typeof(Customer16), "NumberOfOrders1");

            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            Assert.IsNotNull(metamodel);

            metamodel = facetFactory.Process(Reflector, null,property1, MethodRemover, facetHolder1, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer8), "SomeAction");
            metamodel = facetFactory.Process(Reflector, null,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, null,typeof(Customer5), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer7), "Orders");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisplayNameAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer6), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer11), "NumberOfOrders");
            var property1 = FindProperty(typeof(Customer11), "NumberOfOrders1");

            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            Assert.IsNotNull(metamodel);

            metamodel = facetFactory.Process(Reflector, null,property1, MethodRemover, facetHolder1, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);

            facet = facetHolder1.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnAction() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer3), "SomeAction");
            metamodel = facetFactory.Process(Reflector, null,actionMethod, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var actionMethod = FindMethod(typeof(Customer4), "SomeAction", new[] {typeof(int)});
            metamodel = facetFactory.ProcessParams(Reflector, null,actionMethod, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnClass() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            metamodel = facetFactory.Process(Reflector, null,typeof(Customer), MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnCollection() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer2), "Orders");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNamedAnnotationPickedUpOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer1), "NumberOfOrders");
            metamodel = facetFactory.Process(Reflector, null,property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NamedFacetAbstract);
            var namedFacetAbstract = (NamedFacetAbstract) facet;
            Assert.AreEqual("some name", namedFacetAbstract.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }

        private class TestSpecificationWithId : Specification {
            public TestSpecificationWithId(IIdentifier identifier) => Identifier = identifier;

            public override IIdentifier Identifier { get; }
        }

        [TestMethod] // for bug #156
        public void TestAnnotationNotPickedUpOnInferredPropertyAfterAnnotatedProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var specification1 = new TestSpecification();
            var specification2 = new TestSpecificationWithId(new IdentifierImpl("Customer19", "Property"));

            var property1 = FindProperty(typeof(Customer19), "SomeProperty");
            var property2 = FindProperty(typeof(Customer19), "Property");
            metamodel = facetFactory.Process(Reflector, null,property1, MethodRemover, specification1, metamodel);
            Assert.IsNotNull(metamodel);

            metamodel = facetFactory.Process(Reflector, null,property2, MethodRemover, specification2, metamodel);
            var facet1 = specification1.GetFacet(typeof(INamedFacet));
            var facet2 = specification2.GetFacet(typeof(INamedFacet));
            Assert.IsNotNull(facet1);
            Assert.IsNull(facet2);
            Assert.IsTrue(facet1 is NamedFacetAnnotation);
            var namedFacet1 = (NamedFacetAnnotation) facet1;
            Assert.AreEqual("Property", namedFacet1.Value);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}