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
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider {
    [TestClass]
    public class DateTimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<DateTime> {
        private DateTimeValueSemanticsProvider adapter;
        private ISpecification holder;

        private void AssertEntry(string entry, int year, int month, int day, int hour, int minute, int second) {
            var obj = adapter.ParseTextEntry(entry);
            Assert.AreEqual(new DateTime(year, month, day, hour, minute, second), obj);
        }

        [TestMethod]
        public void TestDecode() {
            var decoded = adapter.FromEncodedString("2003-08-17T21:30:25");
            Assert.AreEqual(new DateTime(TestClock.GetTicks()), decoded);
        }

        [TestMethod]
        public void TestEmptyClears() {
            Assert.IsNull(adapter.ParseTextEntry(""));
        }

        [TestMethod]
        public void TestEncode() {
            var encoded = adapter.ToEncodedString(new DateTime(TestClock.GetTicks()));
            Assert.AreEqual("2003-08-17T21:30:25", encoded);
        }

        [TestMethod]
        public void TestEntryWithLongISOFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            dt = dt.ToUniversalTime();
            var entry = dt.ToString("u");
            AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
        }

        [TestMethod]
        public void TestEntryWithMediumFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            var entry = dt.ToString("f");
            // "21-May-2007 10:30"
            AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
        }

        [TestMethod]
        public void TestEntryWithShortFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            var entry = dt.ToString("g");
            const int year = 2007;
            const int month = 5;
            const int day = 21;
            const int hour = 10;
            const int minute = 30;
            AssertEntry(entry, year, month, day, hour, minute, 0);
        }

        [TestMethod]
        public void TestParseInvariant() {
            var d1 = new DateTime(2014, 7, 10, 14, 52, 0, DateTimeKind.Utc);
            var s1 = d1.ToString(CultureInfo.InvariantCulture);
            var d2 = adapter.ParseInvariant(s1);
            Assert.AreEqual(d1, d2);
        }

        [TestMethod]
        public override void TestParseNull() {
            base.TestParseNull();
        }

        [TestMethod]
        public override void TestParseEmptyString() {
            base.TestParseEmptyString();
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
            holder = new Mock<ISpecification>().Object;
            var spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(adapter = new DateTimeValueSemanticsProvider(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}