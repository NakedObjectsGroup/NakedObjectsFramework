// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class DataTypeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private DataTypeAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IDataTypeFacet)}; }
        }

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
        public void TestNoAnnotationOnProperty() {
            var property = FindProperty(typeof(Test), nameof(Test.NoAnnotation));
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDataTypeFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestDataTypeAnnotationProperty() {
            var property = FindProperty(typeof(Test), nameof(Test.DataTypeAnnotation));
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDataTypeFacet)) as IDataTypeFacet;
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DataTypeFacetAnnotation);
            Assert.IsTrue(facet.DataType() == DataType.PostalCode);
            Assert.IsTrue(facet.CustomDataType() == "");
        }

        [TestMethod]
        public void TestCustomDataTypeAnnotationOnProperty() {
            var property = FindProperty(typeof(Test), nameof(Test.CustomDataTypeAnnotation));
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IDataTypeFacet)) as IDataTypeFacet;
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DataTypeFacetAnnotation);
            Assert.IsTrue(facet.DataType() == DataType.Custom);
            Assert.IsTrue(facet.CustomDataType() == "CustomDataType");
        }

        [TestMethod]
        public void TestNoFacetOnActionParameter() {
            var method = FindMethod(typeof(Test), nameof(Test.NoAnnotationMethod), new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IDateOnlyFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestAnnotatedDataTypeOnActionParameter() {
            var method = FindMethod(typeof(Test), nameof(Test.DataTypeAnnotationMethod), new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IDataTypeFacet)) as IDataTypeFacet;
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DataTypeFacetAnnotation);
            Assert.IsTrue(facet.DataType() == DataType.CreditCard);
            Assert.IsTrue(facet.CustomDataType() == "");
        }

        [TestMethod]
        public void TestAnnotatedCustomDataTypeOnActionParameter() {
            var method = FindMethod(typeof(Test), nameof(Test.CustomDataTypeMethod), new[] {typeof(string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            var facet = Specification.GetFacet(typeof(IDataTypeFacet)) as IDataTypeFacet;
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DataTypeFacetAnnotation);
            Assert.IsTrue(facet.DataType() == DataType.Custom);
            Assert.IsTrue(facet.CustomDataType() == "CustomDataType");
        }

        #region Nested type: Test

        private class Test {
            public DateTime NoAnnotation => DateTime.Now;

            [DataType(DataType.PostalCode)]
            public string DataTypeAnnotation => DateTime.Now.ToString(CultureInfo.CurrentCulture);

            [DataType("CustomDataType")]
            public string CustomDataTypeAnnotation => DateTime.Now.ToString(CultureInfo.CurrentCulture);

            public void NoAnnotationMethod(string aDate1) { }

            public void DataTypeAnnotationMethod([DataType(DataType.CreditCard)] string aDate2) { }

            public void CustomDataTypeMethod([DataType("CustomDataType")] string aDate3) { }
        }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new DataTypeAnnotationFacetFactory(0);
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