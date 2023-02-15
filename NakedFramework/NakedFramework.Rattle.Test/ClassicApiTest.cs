using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Rattle.TestCase;

namespace NakedFramework.Rattle.Test; 

[TestClass]
public class ClassicApiTest : BaseRattleTestCase {
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