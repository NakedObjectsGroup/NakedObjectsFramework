// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;


namespace NakedObjects.Reflector.DotNet.Value {
    [TestFixture]
    public class DateTimeValueSemanticsProviderTest : ValueSemanticsProviderAbstractTestCase<DateTime> {
        private DateTimeValueSemanticsProvider adapter;
        private IFacetHolder holder;

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            SetupSpecification(typeof (DateTime));
            holder = new FacetHolderImpl();
            SetValue(adapter = new DateTimeValueSemanticsProvider(holder));
        }

        [Test]
        public void TestNow() {
            DateTime now = new DateTime(2013, 10, 24, 15, 21, 56);
            DateTimeValueSemanticsProvider.TestDateTime = now;

            try {
                AssertEntry("now", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            }
            finally {
                DateTimeValueSemanticsProvider.TestDateTime = null;
            }
        }

        [Test]
        public void TestToday() {
            DateTime now = new DateTime(2013, 10, 24, 15, 21, 56);
            DateTimeValueSemanticsProvider.TestDateTime = now;

            try {
                AssertEntry("today", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            }
            finally {
                DateTimeValueSemanticsProvider.TestDateTime = null;
            }
        }

        [Test]
        public void TestEntryWithShortFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            string entry = dt.ToString("g");
            const int year = 2007;
            const int month = 5;
            const int day = 21;
            const int hour = 10;
            const int minute = 30;
            AssertEntry(entry, year, month, day, hour, minute, 0);
        }

        private void AssertEntry(string entry, int year, int month, int day, int hour, int minute, int second) {
            object obj = adapter.ParseTextEntry(new DateTime(TestClock.GetTicks()), entry);
            Assert.AreEqual(new DateTime(year, month, day, hour, minute, second), obj);
        }

        [Test]
        public void TestEntryWithMediumFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            string entry = dt.ToString("f");
            // "21-May-2007 10:30"
            AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
        }

        [Test]
        public void TestEntryWithShortISOFormat() {
            // not currently recognised
            //assertEntry("20070521T1030", 2007, 5, 21, 10, 30, 0);
        }

        [Test]
        public void TestEntryWithLongISOFormat() {
            var dt = new DateTime(2007, 5, 21, 10, 30, 0);
            dt = dt.ToUniversalTime();
            string entry = dt.ToString("u");
            AssertEntry(entry, 2007, 5, 21, 10, 30, 0);
        }

        [Test]
        public void TestEmptyClears() {
            Assert.IsNull(adapter.ParseTextEntry(DateTime.Now, ""));
        }

        [Test]
        public void TestAddPartialEntry() {
            try {
                object obj = adapter.ParseTextEntry(DateTime.Now, "+");
            } catch(InvalidEntryException) {}

            try {
                object obj = adapter.ParseTextEntry(DateTime.Now, "+2 -3");
            } catch (InvalidEntryException) { }
        }

        [Test]
        public void TestAddDayAndMonthIncomplete() {
            AssertEntry("+1 3", 2003, 8, 21, 21, 30, 25);
            AssertEntry("+1 3m", 2003, 11, 18, 21, 30, 25);
        }

        [Test]
        public void TestAddDay() {
            AssertEntry("+1", 2003, 8, 18, 21, 30, 25);
        }

        [Test]
        public void TestSubtractDay() {
            AssertEntry("-1", 2003, 8, 16, 21, 30, 25);
        }

        [Test]
        public void TestAddDayAndMonth() {
            AssertEntry("+1d 1m", 2003, 9, 18, 21, 30, 25);
        }

        [Test]
        public void TestSubtractDayAndMonth() {
            AssertEntry("-1d 1m", 2003, 7, 16, 21, 30, 25);
        }

        [Test]
        public void TestAddOneDay() {
            AssertEntry("+1d", 2003, 8, 18, 21, 30, 25);
        }

        [Test]
        public void TestSubtractOneDay() {
            AssertEntry("-1d", 2003, 8, 16, 21, 30, 25);
        }

        [Test]
        public void TestAddOneMonth() {
            AssertEntry("+1m", 2003, 9, 17, 21, 30, 25);
        }

        [Test]
        public void TestAddOneYear() {
            AssertEntry("+1y", 2004, 8, 17, 21, 30, 25);
        }

        [Test]
        public void TestAddOneHour() {
            AssertEntry("+1H", 2003, 8, 17, 22, 30, 25);
        }

        [Test]
        public void TestAddOneMinute() {
            AssertEntry("+1M", 2003, 8, 17, 21, 31, 25);
        }

        [Test]
        public void TestEncode() {
            string encoded = adapter.ToEncodedString(new DateTime(TestClock.GetTicks()));
            Assert.AreEqual("2003-08-17T21:30:25", encoded);
        }

        [Test]
        public void TestDecode() {
            DateTime decoded = adapter.FromEncodedString("2003-08-17T21:30:25");
            Assert.AreEqual(new DateTime(TestClock.GetTicks()), decoded);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}