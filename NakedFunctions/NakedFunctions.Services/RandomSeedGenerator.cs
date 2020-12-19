

using System;

namespace NakedFunctions.Services
{
    public class RandomSeedGenerator : IRandomSeedGenerator
    {
        public RandomSeedGenerator()
        {
            var now = DateTime.Now.ToFileTime();
            Random = new RandomNumber((uint)(now >> 16), (uint)(now % 4294967296));
        }

        public RandomSeedGenerator(uint u, uint v)
        {
            Random = new RandomNumber(u,v);
        }

        public IRandom Random { get; init; }
    }
}
