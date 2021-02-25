using NakedFunctions;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockRandomSeedGenerator : IRandomSeedGenerator
    {
          public IRandom Random => throw new System.NotImplementedException();
    }
}