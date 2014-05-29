// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NUnit.Framework;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class TimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<TimeSpan> {
        [SetUp]
        public override void SetUp() {
            base.SetUp();
            SetupSpecification(typeof (TimeSpan));
            time = new TimeSpan(8, 13, 0);
            holder = new FacetHolderImpl();
            SetValue(adapter = new TimeValueSemanticsProvider(holder));
        }

        private TimeValueSemanticsProvider adapter;
        private IFacetHolder holder;
        private TimeSpan time;

        [Test]
        public void TestParseEntryOfHoursAfterNow() {
            object parsed = adapter.ParseTextEntry(new TimeSpan(TestClock.GetTicks()), "+5H");
            Assert.AreEqual(new TimeSpan(2, 30, 25), parsed);
        }

        [Test]
        public void TestParseEntryOfHoursAfterTime() {
            object parsed = adapter.ParseTextEntry(time, "+5H");
            Assert.AreEqual(new TimeSpan(13, 13, 00), parsed);
        }

        [Test]
        public void TestParseEntryOfHoursAfterTimePartEntry() {
            object parsed = adapter.ParseTextEntry(time, "+");
            Assert.AreEqual(new TimeSpan(8, 13, 00), parsed);

            parsed = adapter.ParseTextEntry(time, "+5");
            Assert.AreEqual(new TimeSpan(8, 13, 00), parsed);
        }

        [Test]
        public void TestParseEntryOfHoursBeforeTime() {
            object parsed = adapter.ParseTextEntry(time, "-7H");
            Assert.AreEqual(new TimeSpan(1, 13, 00), parsed);
        }

        [Test]
        public void TestParseEntryOfHoursBeforeToNow() {
            object parsed = adapter.ParseTextEntry(new TimeSpan(TestClock.GetTicks()), "-5H");
            Assert.AreEqual(new TimeSpan(16, 30, 25), parsed);
        }

        [Test]
        public void TestParseEntryOfKeywordNow() {
            var now = new DateTime(2013, 10, 24, 15, 21, 56);
            TimeValueSemanticsProvider.TestDateTime = now;

            try {
                object parsed = adapter.ParseTextEntry(time, "now");
                Assert.AreEqual(now.TimeOfDay, parsed);
            }
            finally {
                TimeValueSemanticsProvider.TestDateTime = null;
            }
        }

        [Test]
        public void TestRestoreOfInvalidDatal() {
            try {
                adapter.FromEncodedString("two ten");
                Assert.Fail();
            }
            catch (FormatException /*expected*/) {}
        }

        [Test]
        public void TestRestoreTime() {
            object parsed = adapter.FromEncodedString("21:30:00");
            Assert.AreEqual(new TimeSpan(21, 30, 0), parsed);
        }

        [Test]
        public void TestTimeAsEncodedString() {
            Assert.AreEqual("08:13:00", adapter.ToEncodedString(time));
        }
    }

    public class TestClock {
        public static long GetTicks() {
            return new DateTime(2003, 8, 17, 21, 30, 25).Ticks;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}