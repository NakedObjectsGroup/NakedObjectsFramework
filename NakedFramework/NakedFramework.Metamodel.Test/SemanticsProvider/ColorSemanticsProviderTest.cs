// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class ColorValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<Color> {
        private Color colorObj;
        private ISpecification holder;
        private ColorValueSemanticsProvider value;

        [TestMethod]
        public void TestParseValidString() {
            var str = Color.Beige.ToArgb().ToString();
            var parsed = (Color) value.ParseTextEntry(str);
            Assert.AreEqual(Color.Beige.ToArgb(), parsed.ToArgb());
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
            Assert.AreEqual("Color [White]", value.DisplayTitleOf(colorObj));
        }

        [TestMethod]
        public void TestEncode() {
            Assert.AreEqual("-983041", value.ToEncodedString(Color.Azure));
        }

        [TestMethod]
        public void TestDecode() {
            var parsed = value.FromEncodedString("-983041");
            Assert.AreEqual(Color.Azure.ToArgb(), parsed.ToArgb());
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
            IColorValueFacet facet = value;
            var testValue = Color.Blue;
            var mockNo = new Mock<INakedObjectAdapter>();
            mockNo.Setup(no => no.Object).Returns(testValue);
            Assert.AreEqual(testValue.ToArgb(), facet.ColorValue(mockNo.Object));
        }

        [TestMethod]
        public void TestAsParserInvariant() {
            var mgr = MockNakedObjectManager();
            var str = Color.Beige.ToArgb().ToString();
            IParseableFacet parser = new ParseableFacetUsingParser<Color>(value, null);
            var parsed = (Color) parser.ParseInvariant(str, mgr.Object).Object;
            Assert.AreEqual(Color.Beige.ToArgb(), parsed.ToArgb());
        }

        [TestMethod]
        public void TestAsParserTitle() {
            IParseableFacet parser = new ParseableFacetUsingParser<Color>(value, null);
            var mockAdapter = MockAdapter(Color.Beige);
            Assert.AreEqual("Color [Beige]", parser.ParseableTitle(mockAdapter));
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            colorObj = Color.White;
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new ColorValueSemanticsProvider(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }
}