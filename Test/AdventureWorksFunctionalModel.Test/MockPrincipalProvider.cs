using NakedFunctions;
using System.Security.Principal;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockPrincipalProvider : IPrincipalProvider
    {
        public IPrincipal CurrentUser => throw new System.NotImplementedException();
    }
}