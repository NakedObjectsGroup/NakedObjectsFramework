using NakedFunctions;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockAlert : IAlert
    {
        public void InformUser(string message)
        {
            throw new System.NotImplementedException();
        }

        public void WarnUser(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}