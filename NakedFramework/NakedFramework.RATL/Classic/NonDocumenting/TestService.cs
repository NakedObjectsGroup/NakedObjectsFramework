using NakedFramework.RATL.Classic.Interface;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.Classic.NonDocumenting;

internal class TestService : TestHasActions, ITestService {
    public TestService(DomainObject service) : base(service) { }

    public override string Title {
        get {
            Assert.IsNotNull(DomainObject, "Cannot get title for null service");
            return DomainObject?.GetTitle();
        }
    }
}