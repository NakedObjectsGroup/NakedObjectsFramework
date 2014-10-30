// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Moq;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NUnit.Framework;
using NakedObjects.Meta.Spec;
using NakedObjects.Core.Util;
using Assert = NUnit.Framework.Assert; 

namespace NakedObjects.Reflect.DotNet.Value {
    [TestFixture]
    public class BoolValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<bool> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            booleanObj = true;
            booleanNO = CreateAdapter(booleanObj);
            specification = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new BooleanValueSemanticsProvider(spec, specification));
        }

        #endregion

        private INakedObject booleanNO;
        private object booleanObj;
        private ISpecification specification;
        private BooleanValueSemanticsProvider value;

        [Test]
        public void TestDecodeFalse() {
            object parsed = value.FromEncodedString("F");
            Assert.AreEqual(false, parsed);
        }

        [Test]
        public void TestDecodeTrue() {
            object parsed = value.FromEncodedString("T");
            Assert.AreEqual(true, parsed);
        }

        [Test]
        public void TestEncodeFalse() {
            Assert.AreEqual("F", value.ToEncodedString(false));
        }

        [Test]
        public void TestEncodeTrue() {
            Assert.AreEqual("T", value.ToEncodedString(true));
        }

        [Test]
        public void TestIsNotSet() {
            Assert.AreEqual(false, value.IsSet(CreateAdapter(false)));
        }

        [Test]
        public void TestIsSet() {
            Assert.AreEqual(true, value.IsSet(booleanNO));
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
        public void TestParseFalseString() {
            object parsed = value.ParseTextEntry("faLSe");
            Assert.AreEqual(false, parsed);
        }

        [Test]
        public void TestParseInvalidString() {
            try {
                value.ParseTextEntry("yes");
                Assert.Fail("Invalid string");
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParseInvariant() {
            new[] {true, false}.ForEach(b => {
                string b1 = b.ToString(CultureInfo.InvariantCulture);
                object b2 = value.ParseInvariant(b1);
                Assert.AreEqual(b, b2);
            });
        }

        [Test]
        public void TestParseTrueString() {
            object parsed = value.ParseTextEntry("TRue");
            Assert.AreEqual(true, parsed);
        }

        [Test]
        public void TestTitleFalse() {
            Assert.AreEqual("False", value.DisplayTitleOf(false));
        }

        [Test]
        public void TestTitleTrue() {
            Assert.AreEqual("True", value.DisplayTitleOf(true));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}