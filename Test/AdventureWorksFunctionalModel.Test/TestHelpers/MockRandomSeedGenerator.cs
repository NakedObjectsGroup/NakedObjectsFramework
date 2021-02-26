using NakedFunctions;
using System.Linq;

namespace NakedFunctions.Test
{
    public class MockRandomSeedGenerator : IRandomSeedGenerator
    {
        public MockRandomSeedGenerator(params int[] r)
        {
            randoms = new MockRandom[r.Count()];
            int last = r.Count()-1;           
            var next = new MockRandom(r.Last(), null);
            randoms[last] = next;
            for (int i = last-1; i >= 0; i--)
            {
                var x = new MockRandom(r[i], next);
                randoms[i] = x;
                next = x;
            }
        }

        private IRandom[] randoms;

        private int pointer = 0;

        public IRandom Random => randoms[pointer++];


        class MockRandom : IRandom
        {
            public MockRandom(int value, MockRandom next)
            {
                this.value = value;
                this.next = next;
            }

            public MockRandom(int value) : this(value, null) { }

            private int value;

            public int Value => value;

            private MockRandom next;

            public IRandom Next() => next;

            public int ValueInRange(int max) => value % max;

            public int ValueInRange(int min, int max) => ValueInRange(max - min) + min;

            public override string ToString() => value.ToString();
        }
    }
}