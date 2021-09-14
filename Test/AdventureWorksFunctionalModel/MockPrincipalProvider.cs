using System.Security.Principal;
using NakedFunctions;

namespace AW {
    public class MockPrincipalProvider : IPrincipalProvider {
        public IPrincipal CurrentUser =>
            new GenericPrincipal(new GenericIdentity(@"adventure-works\ken0"), new[] { "root" });
    }
}