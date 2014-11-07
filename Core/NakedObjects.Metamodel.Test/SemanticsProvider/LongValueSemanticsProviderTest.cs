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
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;
using NakedObjects.Meta.Spec;
using NUnit.Framework;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestFixture]
    public class LongValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<long> {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            l = 32;
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(value = new LongValueSemanticsProvider(spec, holder));
        }

        #endregion

        private ISpecification holder;
        private long l;
        private LongValueSemanticsProvider value;

        [Test]
        public void TestDecode() {
            long decoded = GetValue().FromEncodedString("304211223");
            Assert.AreEqual(304211223L, decoded);
        }

        [Test]
        public void TestEncode() {
            string encoded = GetValue().ToEncodedString(213434790L);
            Assert.AreEqual("213434790", encoded);
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
            Assert.AreEqual(120L, newValue);
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
            const long c1 = 123456L;
            string s1 = c1.ToString(CultureInfo.InvariantCulture);
            object c2 = GetValue().ParseInvariant(s1);
            Assert.AreEqual(c1, c2);
        }

        [Test]
        public void TestParseOddlyFormedEntry() {
            object newValue = value.ParseTextEntry("1,20.0");
            Assert.AreEqual(120L, newValue);
        }

        [Test]
        public void TestTitleString() {
            Assert.AreEqual("32", value.DisplayTitleOf(l));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}