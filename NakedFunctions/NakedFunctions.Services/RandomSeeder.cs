

using System;

namespace NakedFunctions.Services
{
    public class RandomSeeder : IRandomSeeder
    {
        public RandomSeeder(DateTime clockNow)
        {
            Seed = new RandomNumber((uint)(clockNow.ToFileTime() >> 16), (uint)(clockNow.ToFileTime() % 4294967296));
        }

        public RandomSeeder(uint u, uint v)
        {
            Seed = new RandomNumber(u,v);
        }

        public IRandom Seed { get; init; }
    }
}
