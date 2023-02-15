using Microsoft.Extensions.DependencyInjection;
using NakedFramework.RATL.TestCase;

namespace NakedFramework.RATL.Test; 

[TestClass]
public class ClassicApiTest : BaseRATLTestCase {
    protected override void ConfigureServices(IServiceCollection services) {
        throw new NotImplementedException();
    }

    [TestInitialize]
    public void SetUp() {
        StartTest();
    }

    [TestCleanup]
    public void TearDown() {
        EndTest();
    }

    [ClassInitialize]
    public  void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [ClassCleanup]
    public  void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    [TestMethod]
    public void TestMethod1() { }
}