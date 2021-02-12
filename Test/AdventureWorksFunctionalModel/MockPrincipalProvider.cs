using NakedFunctions;
using System.Security.Principal;

namespace AW
{
    public class MockPrincipalProvider : IPrincipalProvider
    {
        public IPrincipal CurrentUser =>
            new GenericPrincipal(new GenericIdentity(@"adventure-works\ken0"), new[] { "root" });

      
    }
}
