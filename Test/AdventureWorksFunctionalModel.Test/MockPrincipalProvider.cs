using NakedFunctions;
using System.Security.Principal;

namespace AdventureWorksFunctionalModel.Test
{
    public class MockPrincipalProvider : IPrincipalProvider
    {
        public MockPrincipalProvider(IPrincipal p) => Principal = p;

        public MockPrincipalProvider(string name) => Principal = new MockPrincipal() { userName = name };

        public IPrincipal Principal { get; set; }

        public IPrincipal CurrentUser => Principal;

        class MockPrincipal : IPrincipal
        {
            public string userName;

            public IIdentity Identity => new MockIdentity() { UserName = userName };

            public bool IsInRole(string role)
            {
                throw new System.NotImplementedException();
            }

            record MockIdentity : IIdentity
            {
                public string UserName { get; init; }
                public string AuthenticationType => throw new System.NotImplementedException();

                public bool IsAuthenticated => throw new System.NotImplementedException();

                public string Name => UserName;
            }
        }
    }
}