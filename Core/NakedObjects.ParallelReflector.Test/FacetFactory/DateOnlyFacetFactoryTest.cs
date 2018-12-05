// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class DateOnlyFacetFactoryTest : AbstractFacetFactoryTest {
        private DateOnlyFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IDateOnlyFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "ADate1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnNullableProperty() {
            PropertyInfo property = FindProperty(typeof(Test), "ADate6");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestNoFacetOnNonDateProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "NotADate");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "ADate2");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnNullableProperty() {
            PropertyInfo property = FindProperty(typeof(Test), "ADate7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "ADate3");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeNullableProperty() {
            PropertyInfo property = FindProperty(typeof(Test), "ADate8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestNoFacetOnConcurrencyProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "ADate4");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnConcurrencyProperty() {
            PropertyInfo property = FindProperty(typeof (Test), "ADate5");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestDefaultDateOnlyOnActionParameter() {
            MethodInfo method = FindMethod(typeof(Test), "ADateMethod1", new[] { typeof(DateTime) });
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestAnnotatedDateOnlyOnActionParameter() {
            MethodInfo method = FindMethod(typeof(Test), "ADateMethod2", new[] { typeof(DateTime) });
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DateOnlyFacet);
        }

        [TestMethod]
        public void TestNoFacetOnDateTimeActionParameter() {
            MethodInfo method = FindMethod(typeof(Test), "ADateMethod3", new[] { typeof(DateTime) });
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestNoFacetOnNotDateActionParameter() {
            MethodInfo method = FindMethod(typeof(Test), "NotADateMethod", new[] { typeof(TimeSpan) });
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
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

            public void ADateMethod1(DateTime aDate1) {}

            public void ADateMethod2([DataType(DataType.Date)] DateTime aDate2) {}

            public void ADateMethod3([DataType(DataType.DateTime)] DateTime aDate3) {}

            public void NotADateMethod(TimeSpan notADate) {}
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