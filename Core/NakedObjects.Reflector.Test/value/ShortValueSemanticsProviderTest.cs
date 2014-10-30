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
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;
using NUnit.Framework;
using NakedObjects.Meta.Spec;

namespace NakedObjects.Reflect.DotNet.Value {
    [TestFixture]
    public class ShortValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<short> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            s = 32;
            holder = new Specification();
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new ShortValueSemanticsProvider(spec, holder));
        }

        #endregion

        private ISpecification holder;
        private short s;
        private ShortValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            long decoded = GetValue().FromEncodedString("30421");
            Assert.AreEqual(30421, decoded);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(21343);
            Assert.AreEqual("21343", encoded);
        }

        [Test]
        public void TestInvalidParse() {
            try {
                value.ParseTextEntry("one");
                Assert.Fail();
            }
            catch (Exception e) {
                Assert.IsInstanceOf(typeof (InvalidEntryException), e);
            }
        }

        [Test]
        public void TestParse() {
            object newValue = value.ParseTextEntry("120");
            Assert.AreEqual((short) 120, newValue);
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
        public void TestParseInvariant() {
            const short c1 = (short) 12346;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseOddlyFormedEntry() {
            object newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual((short) 120, newValue);
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(s));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}