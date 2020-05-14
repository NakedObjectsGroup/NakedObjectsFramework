// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class DateOnlyFacetFactoryTest : AbstractFacetFactoryTest {
        private DateOnlyFacetFactory facetFactory;

        protected override Type[] SupportedTypes => new[] {typeof(IDateOnlyFacet)};

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate1");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnNullableProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate6");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnNonDateProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "NotADate");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate2");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnNullableProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate7");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate3");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeNullableProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate8");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnConcurrencyProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate4");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnConcurrencyProperty() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Test), "ADate5");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Test), "ADateMethod1", new[] {typeof(DateTime)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Test), "ADateMethod2", new[] {typeof(DateTime)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Test), "ADateMethod3", new[] {typeof(DateTime)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestNoFacetOnNotDateActionParameter() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var method = FindMethod(typeof(Test), "NotADateMethod", new[] {typeof(TimeSpan)});
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        #region Nested type: Test

        private class Test {
            public DateTime ADate1 => DateTime.Now;

            [DataType(DataType.Date)]
            public DateTime ADate2 => DateTime.Now;

            [DataType(DataType.DateTime)]
            public DateTime ADate3 => DateTime.Now;

            [ConcurrencyCheck]
            public DateTime ADate4 => DateTime.Now;

            [ConcurrencyCheck]
            [DataType(DataType.Date)]
            public DateTime ADate5 => DateTime.Now;

            public DateTime? ADate6 => DateTime.Now;

            [DataType(DataType.Date)]
            public DateTime? ADate7 => DateTime.Now;

            [DataType(DataType.DateTime)]
            public DateTime? ADate8 => DateTime.Now;

            [DataType(DataType.Date)]
            public TimeSpan NotADate => new TimeSpan();

            public void ADateMethod1(DateTime aDate1) { }

            public void ADateMethod2([DataType(DataType.Date)] DateTime aDate2) { }

            public void ADateMethod3([DataType(DataType.DateTime)] DateTime aDate3) { }

            public void NotADateMethod(TimeSpan notADate) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DateOnlyFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
}