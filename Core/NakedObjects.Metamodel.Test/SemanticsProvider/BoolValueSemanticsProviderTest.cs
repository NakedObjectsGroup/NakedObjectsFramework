// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SemanticsProvider;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class BoolValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<bool> {
        private INakedObjectAdapter booleanNO;
        private INakedObjectAdapter booleanNO1;
        private object booleanObj;
        private ISpecification specification;
        private BooleanValueSemanticsProvider value;
        private IBooleanValueFacet valueFacet;

        [TestMethod]
        public void TestDecodeFalse() {
            object parsed = value.FromEncodedString("F");
            Assert.AreEqual(false, parsed);
        }

        [TestMethod]
        public void TestDecodeTrue() {
            object parsed = value.FromEncodedString("T");
            Assert.AreEqual(true, parsed);
        }

        [TestMethod]
        public void TestEncodeFalse() {
            Assert.AreEqual("F", value.ToEncodedString(false));
        }

        [TestMethod]
        public void TestEncodeTrue() {
            Assert.AreEqual("T", value.ToEncodedString(true));
        }

        [TestMethod]
        public void TestIsNotSet() {
            Assert.AreEqual(false, valueFacet.IsSet(CreateAdapter(false)));
        }

        [TestMethod]
        public void TestIsSet() {
            Assert.AreEqual(true, valueFacet.IsSet(booleanNO));
        }

        [TestMethod]
        public void TestToggle() {
            Assert.IsTrue((bool) booleanNO1.Object);
            valueFacet.Toggle(booleanNO1);
            Assert.IsFalse((bool) booleanNO1.Object);
        }

        [TestMethod]
        public override void TestParseEmptyString() {
            try {
                var newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestParseFalseString() {
            var parsed = value.ParseTextEntry("faLSe");
            Assert.AreEqual(false, parsed);
        }

        [TestMethod]
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("yes");
                Assert.Fail("Invalid string");
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
            }
        }

        [TestMethod]
        public void TestParseInvariant() {
            new[] {true, false}.ForEach(b => {
                var b1 = b.ToString(CultureInfo.InvariantCulture);
                var b2 = value.ParseInvariant(b1);
                Assert.AreEqual(b, b2);
            });
        }

        [TestMethod]
        public void TestParseTrueString() {
            var parsed = value.ParseTextEntry("TRue");
            Assert.AreEqual(true, parsed);
        }

        [TestMethod]
        public void TestTitleFalse() {
            Assert.AreEqual("False", value.DisplayTitleOf(false));
        }

        [TestMethod]
        public void TestTitleTrue() {
            Assert.AreEqual("True", value.DisplayTitleOf(true));
        }

        [TestMethod]
        public override void TestParseNull() {
            base.TestParseNull();
        }

        [TestMethod]
        public override void TestDecodeNull() {
            base.TestDecodeNull();
        }

        [TestMethod]
        public override void TestEmptyEncoding() {
            base.TestEmptyEncoding();
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            booleanObj = true;
            booleanNO = CreateAdapter(booleanObj);
            booleanNO1 = CreateAdapter(true);
            specification = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new BooleanValueSemanticsProvider(spec, specification));
            valueFacet = value;
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}