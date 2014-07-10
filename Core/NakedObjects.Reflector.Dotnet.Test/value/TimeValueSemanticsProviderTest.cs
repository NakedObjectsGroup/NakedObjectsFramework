// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NUnit.Framework;

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