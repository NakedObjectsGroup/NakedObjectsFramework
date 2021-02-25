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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class IntValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<int> {
        private ISpecification holder;
        private int integer;
        private IntValueSemanticsProvider value;

        [TestMethod]
        public void TestDecode() {
            var decoded = GetValue().FromEncodedString("304211223");
            Assert.AreEqual(304211223, decoded);
        }

        [TestMethod]
        public void TestEncode() {
            var encoded = GetValue().ToEncodedString(213434790);
            Assert.AreEqual("213434790", encoded);
        }

        [TestMethod]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
            }
        }

        [TestMethod]
        public void TestParse() {
            var newValue = value.ParseTextEntry("120");
            Assert.AreEqual(120, newValue);
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
            const int c1 = 123;
            var s1 = c1.ToString(CultureInfo.InvariantCulture);
            var c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void TestParseOddlyFormedEntry() {
            var newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual(120, newValue);
        }

        [TestMethod]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(integer));
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
            var facet = (IIntegerValueFacet) GetValue();
            const int testValue = 121;
            var mockNo = new Mock<INakedObjectAdapter>();
            mockNo.Setup(no => no.Object).Returns(testValue);
            Assert.AreEqual(testValue, facet.IntegerValue(mockNo.Object));
        }

        [TestMethod]
        public void TestAsParserInvariant() {
            var mgr = MockNakedObjectManager();
            IParseableFacet parser = new ParseableFacetUsingParser<int>(value, null);
            Assert.AreEqual(91, parser.ParseInvariant("91", mgr.Object).Object);
        }

        [TestMethod]
        public void TestAsParserTitle() {
            IParseableFacet parser = new ParseableFacetUsingParser<int>(value, null);
            var mockAdapter = MockAdapter(101);
            Assert.AreEqual("101", parser.ParseableTitle(mockAdapter));
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            integer = 32;
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new IntValueSemanticsProvider(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}