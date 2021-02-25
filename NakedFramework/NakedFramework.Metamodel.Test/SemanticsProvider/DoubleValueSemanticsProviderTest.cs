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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Exception;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class DoubleValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<double> {
        private double doubleObj;
        private ISpecification holder;

        [TestMethod]
        public void TestDecode() {
            var decoded = GetValue().FromEncodedString("3.042112234E6");
            Assert.AreEqual(3042112.234, decoded);
        }

        [TestMethod]
        public void TestEncode() {
            var encoded = GetValue().ToEncodedString(0.0000454566);
            Assert.AreEqual("4.54566E-05", encoded);
        }

        [TestMethod]
        public void TestInvalidParse() {
            try {
                GetValue().ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e, typeof(InvalidEntryException));
            }
        }

        [TestMethod]
        public void TestParse() {
            var newValue = GetValue().ParseTextEntry("120.56");
            Assert.AreEqual(120.56, newValue);
        }

        [TestMethod]
        public void TestParse2() {
            var newValue = GetValue().ParseTextEntry("1,20.0");
            Assert.AreEqual(120D, newValue);
        }

        [TestMethod]
        public override void TestParseEmptyString() {
            try {
                var newValue = GetValue().ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestParseInvariant() {
            const double c1 = 123.456;
            var s1 = c1.ToString(CultureInfo.InvariantCulture);
            var c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void TestTitleOf() {
            Assert.AreEqual("35000000", GetValue().DisplayTitleOf(35000000.0));
        }

        [TestMethod]
        public void TestTitleOfWithMantissa() {
            Assert.AreEqual("32.5", GetValue().DisplayTitleOf(doubleObj));
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
            var facet = (IDoubleFloatingPointValueFacet) GetValue();
            const double testValue = 100.100d;
            var mockNo = new Mock<INakedObjectAdapter>();
            mockNo.Setup(no => no.Object).Returns(testValue);
            Assert.AreEqual(testValue, facet.DoubleValue(mockNo.Object));
        }

        [TestMethod]
        public void TestAsParserInvariant() {
            var mgr = MockNakedObjectManager();
            IParseableFacet parser = new ParseableFacetUsingParser<double>(GetValue(), null);
            Assert.AreEqual(91d, parser.ParseInvariant("91", mgr.Object).Object);
        }

        [TestMethod]
        public void TestAsParserTitle() {
            IParseableFacet parser = new ParseableFacetUsingParser<double>(GetValue(), null);
            var mockAdapter = MockAdapter(101d);
            Assert.AreEqual("101", parser.ParseableTitle(mockAdapter));
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();

            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(new DoubleValueSemanticsProvider(spec, holder));

            doubleObj = 32.5;
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}