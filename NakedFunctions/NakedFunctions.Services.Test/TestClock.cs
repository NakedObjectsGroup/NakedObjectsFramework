using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NakedFunctions.Services.Test
{
    [TestClass]
    public class TestClock
    {

        [TestMethod]
        public void TestNow()
        {
            var c = new Clock();
            var actual = c.Now();
            var expected = DateTime.Now;
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
            Assert.IsTrue(Math.Abs(expected.Second - actual.Second) <= 1);
        }

        [TestMethod]
        public void TestToday()
        {
            var c = new Clock();
            Assert.AreEqual(DateTime.Today, c.Today());
        }
    }
}
