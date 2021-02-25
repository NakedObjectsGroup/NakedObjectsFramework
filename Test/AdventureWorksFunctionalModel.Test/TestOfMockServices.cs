using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AdventureWorksFunctionalModel.Test
{
    [TestClass]
    public class TestOfMockServices
    {
        [TestMethod]
        public void TestOfMockRandom()
        {
            var gen = new MockRandomSeedGenerator(27);
            var r = gen.Random;
            Assert.AreEqual(7, r.ValueInRange(10));
            Assert.AreEqual(7, r.ValueInRange(5, 10));
        }
        [TestMethod]
        public void TestMockRandomSeedGenerator()
        {
            var gen = new MockRandomSeedGenerator(3, 1, 4, 1, 5, 9);
            var r = gen.Random;
            Assert.AreEqual(3, r.Value);
             r = r.Next();
            Assert.AreEqual(1, r.Value);
            r = r.Next();
            Assert.AreEqual(4, r.Value);
            r = r.Next();
            Assert.AreEqual(1, r.Value);
            r = r.Next();
            Assert.AreEqual(5, r.Value);
            r = r.Next();
            Assert.AreEqual(9, r.Value);
            Assert.IsNull(r.Next());
        }
        [TestMethod]
        public void TestOfMockPrincipalProvider()
        {
            var pp = new MockPrincipalProvider("Foo");
            Assert.AreEqual("Foo", pp.Principal.Identity.Name);
        }

        [TestMethod]
        public void TestOfMockGuidGenerator()
        {
            var gen = new MockGuidGenerator();
            var g =  new System.Guid();
            gen.NextGuid = g;
            Assert.AreEqual(g, gen.NewGuid());
            try
            {
                gen.NewGuid();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Next Guid value has not been set up", e.Message);
            }          
        }

        [TestMethod]
        public void TestMockClock()
        {
            var d = new DateTime(2021, 2, 25, 15, 42, 30);
            var clock = new MockClock() { Time = d };
            Assert.AreEqual("25/02/2021 00:00:00", clock.Today().ToString());
            Assert.AreEqual("25/02/2021 15:42:30", clock.Now().ToString());
        }

        [TestMethod]
        public void TestMockAlert()
        {
            var a = new MockAlert();
            a.InformUser("Foo");
            a.WarnUser("Bar");
            a.InformUser("Yon");
            a.WarnUser("Qux");
            Assert.AreEqual("FooYon", a.Message_Inform);
            Assert.AreEqual("BarQux", a.Message_Warn);
        }
    }
}
