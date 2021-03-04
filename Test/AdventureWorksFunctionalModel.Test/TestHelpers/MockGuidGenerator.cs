using NakedFunctions;
using System;

namespace NakedFunctions.Test
{
    public class MockGuidGenerator : IGuidGenerator
    {
        public MockGuidGenerator(Guid g) => NextGuid = g;

        public Guid? NextGuid { get; set; }

        public Guid NewGuid()
        {
            if (NextGuid is null) throw new Exception("Next Guid value has not been set up");
            var guid = NextGuid.Value;
            NextGuid = null; //to force user to replace it with another
            return guid;
        }
    }
}