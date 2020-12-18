using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System;
using NakedFunctions.Services;

namespace NakedFunctions.Services.Test
{
    [TestClass]
    public class TestRandomService
    {

        [TestMethod]
        public void Random1()
        {
            IRandomSeeder seeder = new RandomSeeder(521288629, 362436069);
            IRandom random = seeder.Seed;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(0,10)).Append(" ");
                random = random.Next();             
            }
            string gen1Results = sb.ToString();
            Assert.AreEqual("4 0 1 6 5 0 9 8 2 7 ", gen1Results);
        }

        [TestMethod]
        public void Random2()
        {
            IRandomSeeder seeder = new RandomSeeder(new DateTime(2020,12,31));
            IRandom random = seeder.Seed;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(0, 10)).Append(" ");
                random = random.Next();
            }
            string gen1Results = sb.ToString();
            Assert.AreEqual("4 0 7 4 6 3 7 8 7 8 ", gen1Results);
        }


        [TestMethod]
        public void Random3()
        {
            IRandomSeeder seeder = new RandomSeeder(521288629, 362436069);
            IRandom random = seeder.Seed;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(3, 5)).Append(" ");
                random = random.Next();
            }
            string gen1Results = sb.ToString();
            Assert.AreEqual("3 3 4 3 4 3 4 3 3 4 ", gen1Results);
        }

        [TestMethod]
        public void Random4()
        {
            IRandomSeeder seeder = new RandomSeeder(521288629, 362436069);
            IRandom random = seeder.Seed;
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(random.ValueInRange(4)).Append(" ");
                random = random.Next();
            }
            string gen1Results = sb.ToString();
            Assert.AreEqual("2 0 3 0 3 0 1 2 2 1 ", gen1Results);
        }

        [TestMethod] //Very crude test to check that there is an approximately even distribution of 0, 1 results
        public void RandomSeedFromClock()
        {
            IRandomSeeder seeder = new RandomSeeder(DateTime.Now);
            IRandom random = seeder.Seed;
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