// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Linq;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NUnit.Framework;
using NakedObjects.Metamodel.Spec;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class ByteArrayValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<byte[]> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            byteArray = new byte[0];
            byteArrayNakedObject = CreateAdapter(byteArray);
            specification = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new ArrayValueSemanticsProvider<byte>(spec, specification));
        }

        #endregion

        private INakedObject byteArrayNakedObject;
        private object byteArray;
        private ISpecification specification;
        private ArrayValueSemanticsProvider<byte> value;


        public void TestEncodeDecode(byte[] toTest) {
            byte[] originalValue = toTest;
            string encodedValue = value.ToEncodedString(originalValue);
            byte[] decodedValue = value.FromEncodedString(encodedValue);

            Assert.AreEqual(decodedValue, originalValue);
        }


        [Test]
        public void TestEncodeDecode() {
            TestEncodeDecode(new byte[] {1, 2, 100});
        }

        [Test]
        public void TestEncodeDecodeEmpty() {
            TestEncodeDecode(new byte[] {});
        }

        [Test]
        public void TestEncodeDecodeNull() {
            TestEncodeDecode(null);
        }

        [Test]
        public new void TestParseEmptyString() {
            try {
                object newValue = value.ParseTextEntry("");
                Assert.IsNull(newValue);
            }
            catch (Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("fred");
                Assert.Fail("Invalid string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParseInvariant() {
            var b1 = new byte[] {1, 2, 3, 4};
            string s1 = b1.Aggregate("", (s, t) => s + ' ' + t.ToString(CultureInfo.InvariantCulture));
            object b2 = value.ParseInvariant(s1);
            Assert.AreEqual(b1, b2);
        }

        [Test]
        public void TestParseOutOfRangeString() {
            try {
                value.ParseTextEntry("1 2 1000");
                Assert.Fail("out of range string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParseString() {
            object parsed = value.ParseTextEntry("0 0 1 100 255");
            Assert.AreEqual(new byte[] {0, 0, 1, 100, 255}, parsed);
        }

        [Test]
        public void TestTitle() {
            Assert.AreEqual("1 2 100", value.DisplayTitleOf(new byte[] {1, 2, 100}));
        }

        [Test]
        public void TestTitleEmpty() {
            Assert.AreEqual("", value.DisplayTitleOf(new byte[] {}));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}