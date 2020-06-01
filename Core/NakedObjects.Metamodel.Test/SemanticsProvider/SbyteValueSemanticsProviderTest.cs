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
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class SbyteValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<sbyte> {
        private sbyte byteObj;
        private ISpecification holder;
        private SbyteValueSemanticsProvider value;

        [TestMethod]
        public void TestParseValidString() {
            var parsed = value.ParseTextEntry("21");
            Assert.AreEqual((sbyte) 21, parsed);
        }

        [TestMethod]
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("xs21z4xxx23");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
            }
        }

        [TestMethod]
        public void TestTitleOf() {
            Assert.AreEqual("102", value.DisplayTitleOf(byteObj));
        }

        [TestMethod]
        public void TestEncode() {
            Assert.AreEqual("102", value.ToEncodedString(byteObj));
        }

        [TestMethod]
        public void TestDecode() {
            object parsed = value.FromEncodedString("-91");
            Assert.AreEqual((sbyte) -91, parsed);
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
        public void TestParseInvariant() {
            const sbyte c1 = (sbyte) 11;
            var s1 = c1.ToString(CultureInfo.InvariantCulture);
            var c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
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

        [TestMethod]
        public void TestValue() {
            ISbyteValueFacet facet = value;
            const sbyte testValue = (sbyte) 101;
            var mockNo = new Mock<INakedObjectAdapter>();
            mockNo.Setup(no => no.Object).Returns(testValue);
            Assert.AreEqual(testValue, facet.SByteValue(mockNo.Object));
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            byteObj = 102;
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new SbyteValueSemanticsProvider(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }
}