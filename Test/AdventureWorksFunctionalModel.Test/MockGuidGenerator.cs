using NakedFunctions;
using System;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockGuidGenerator : IGuidGenerator
    {
        public Guid NewGuid()
        {
            throw new NotImplementedException();
        }
    }
}