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
    public class TimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TimeSpan> {
        private TimeValueSemanticsProvider adapter;
        private ISpecification holder;
        private TimeSpan time;

        [TestMethod]
        public void TestParseInvariant() {
            var d1 = new TimeSpan(1, 5, 1, 25);
            string s1 = d1.ToString(null, CultureInfo.InvariantCulture);
            object d2 = adapter.ParseInvariant(s1);
            Assert.AreEqual(d1, d2);
        }

        [TestMethod]
        public void TestRestoreOfInvalidDatal() {
            try {
                adapter.FromEncodedString("two ten");
                Assert.Fail();
            }
            catch (FormatException /*expected*/) {}
        }

        [TestMethod]
        public void TestRestoreTime() {
            object parsed = adapter.FromEncodedString("21:30:00");
            Assert.AreEqual(new TimeSpan(21, 30, 0), parsed);
        }

        [TestMethod]
        public void TestTimeAsEncodedString() {
            Assert.AreEqual("08:13:00", adapter.ToEncodedString(time));
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
            SetupSpecification(typeof (TimeSpan));
            time = new TimeSpan(8, 13, 0);
            holder = new Mock<ISpecification>().Object;
            IObjectSpecImmutable spec = new Mock<IObjectSpecImmutable>().Object;
            SetValue(adapter = new TimeValueSemanticsProvider(spec, holder));
        }

        [TestCleanup]
        public override void TearDown() {
            base.TearDown();
        }

        #endregion
    }

    public class TestClock {
        public static long GetTicks() {
            return new DateTime(2003, 8, 17, 21, 30, 25).Ticks;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}