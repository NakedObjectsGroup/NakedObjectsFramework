using NakedFunctions;
using System;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockClock : IClock
    {
        public DateTime Now() => throw new NotImplementedException();
        
        public DateTime Today() => throw new NotImplementedException();

    }
}
