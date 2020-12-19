using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace NakedFunctions.Services.Test
{
    [TestClass]
    public class TestRandomService
    {

        [TestMethod]
        public void Random1()
        {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            IRandom random = seeder.Random;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(0,10)).Append(" ");
                random = random.Next();             
            }
            string actual = sb.ToString();
            Assert.AreEqual("4 0 1 6 5 0 9 8 2 7 ", actual);
        }

        [TestMethod]
        public void Random2()
        {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(362436069, 521288629);
            IRandom random = seeder.Random;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(0, 10)).Append(" ");
                random = random.Next();
            }
            string actual = sb.ToString();
            Assert.AreEqual("6 3 8 8 5 2 4 1 3 8 ", actual);
        }


        [TestMethod]
        public void Random3()
        {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            IRandom random = seeder.Random;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(3, 5)).Append(" ");
                random = random.Next();
            }
            string actual = sb.ToString();
            Assert.AreEqual("3 3 4 3 4 3 4 3 3 4 ", actual);
        }

        [TestMethod]
        public void Random4()
        {
            IRandomSeedGenerator seeder = new RandomSeedGenerator(521288629, 362436069);
            IRandom random = seeder.Random;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(4)).Append(" ");
                random = random.Next();
            }
            string actual = sb.ToString();
            Assert.AreEqual("2 0 3 0 3 0 1 2 2 1 ", actual);
        }

        [TestMethod] //Very crude test to check that there is an approximately even distribution of 0, 1 results
        public void RandomSeedFromClock()
        {
            IRandomSeedGenerator seeder = new RandomSeedGenerator();
            IRandom random = seeder.Random;
            int zeros = 0;
            int ones = 0;
            for (int i = 0; i < 1000; i++)
            {
                random = random.Next();
                if (random.ValueInRange(0,2) == 0)
                {
                    zeros += 1;
                }
                else
                {
                    ones += 1;
                }
            }
            Assert.AreEqual(1000, zeros + ones);
            Assert.IsTrue(zeros < 550);
            Assert.IsTrue(ones < 550);
        }
    }
}