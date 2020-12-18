using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NakedFunctions.Services.Test
{
    [TestClass]
    public class TestGuidGenerator
    {

        [TestMethod]
        public void Test1()
        {
            IGuidGenerator gen = new GuidGenerator();
            Guid g1 = gen.NewGuid();
            Assert.AreNotEqual(Guid.Empty, g1);
            Assert.AreNotEqual(Guid.NewGuid(), g1);

            Guid g2 = gen.NewGuid();
            Assert.AreNotEqual(g1, g2);

        }
    }
}
