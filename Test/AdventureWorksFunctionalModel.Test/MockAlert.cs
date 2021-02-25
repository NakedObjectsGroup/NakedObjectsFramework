using NakedFunctions;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockAlert : IAlert
    {
        public string Message_Inform;

        public string Message_Warn;

        public void InformUser(string message) => Message_Inform += message;

        public void WarnUser(string message) => Message_Warn += message;
    }
}