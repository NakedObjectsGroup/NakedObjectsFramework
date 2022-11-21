using System.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Facade.Utility;
using NakedFramework.Menu;
using NakedFramework.Test.TestCase;
using NakedFunctions.Rest.Test;
using Newtonsoft.Json;
using NUnit.Framework;
using RestfulObjects.Test.Data;
using RestfulObjects.Test.Data.Context;
using RestfulObjects.Test.Data.Wrapper;

namespace ROSITest; 

[TestClass]
public class ApiTest : AcceptanceTestCase {
    private const string AppveyorServer = @"Data Source=(local)\SQL2017;";
    private const string LocalServer = @"Data Source=(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        static string server = appveyorServer;
#else
    private static readonly string Server = LocalServer;
#endif

    private static readonly string CsRta = Server + @"Initial Catalog=RestTestA;Integrated Security=True;";
    //private static string csRtb = Server + @"Initial Catalog=RestTestB;Integrated Security=True;";
    //private static string csRtz = Server + @"Initial Catalog=RestTestZ;Integrated Security=True;";
    //private static string csRtd = Server + @"Initial Catalog=RestTestD;Integrated Security=True;";
    //private static string csRtdt = Server + @"Initial Catalog=RestTestDT;Integrated Security=True;";

    protected override Action<NakedFrameworkOptions> AddNakedFunctions { get; } = b => { };

    protected override Type[] ObjectTypes { get; } = {
        typeof(Immutable),
        typeof(WithActionViewModel),
        typeof(WithCollectionViewModel),
        typeof(WithValueViewModel),
        typeof(WithNestedViewModel),
        typeof(WithValueViewModelEdit),
        typeof(WithNestedViewModelEdit),
        typeof(RedirectedObject),
        typeof(WithScalars),
        typeof(VerySimple),
        typeof(VerySimpleEager),
        typeof(WithAction),
        typeof(WithActionObject),
        typeof(WithAttachments),
        typeof(WithCollection),
        typeof(WithDateTimeKey),
        typeof(WithGuidKey),
        typeof(WithError),
        typeof(WithGetError),
        typeof(WithNestedViewModel),
        typeof(WithReference),
        typeof(WithReferencePersist),
        typeof(MostSimplePersist),
        typeof(WithValuePersist),
        typeof(WithCollectionPersist),
        typeof(WithReferenceViewModel),
        typeof(WithReferenceViewModelEdit),
        typeof(MostSimple),
        typeof(MostSimpleViewModel),
        typeof(WithValue),
        typeof(TestEnum),
        typeof(FormViewModel)
    };

    protected override Type[] Services { get; } = {
        typeof(RestDataRepository),
        typeof(WithActionService),
        typeof(ContributorService)
    };

    protected override IMenu[] MainMenus(IMenuFactory factory) => new[] {
        factory.NewMenu<RestDataRepository>(true),
        factory.NewMenu<WithActionService>(true),
        factory.NewMenu<ContributorService>(true)
    };

    protected override bool EnforceProxies { get; } = false;

    protected override Func<Type[]> NotPersistedTypes { get; } = () => new[]
        { typeof(WithAction) };

    protected override Func<Type[], Type[]> SupportedSystemTypes { get; } = t => t.Union(new[] { typeof(SetWrapper<MostSimple>).GetGenericTypeDefinition() }).ToArray();

    protected override Func<IConfiguration, DbContext>[] ContextCreators { get; } = {
        c => new CodeFirstContextLocal(CsRta)
    };

    protected override void RegisterTypes(IServiceCollection services) {
        base.RegisterTypes(services);
        services.AddScoped<IStringHasher, NullStringHasher>();
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
    }

    [OneTimeSetUp]
    public void FixtureSetup() {
        Database.SetInitializer(new CodeFirstInitializer());
        InitializeNakedObjectsFramework(this);
    }

    [SetUp]
    public void Setup() {
        StartTest();
    }

    [TearDown]
    public void TearDown() {
        // resetCache(Api());
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    public RestfulObjectsController Api() {
        var sp = GetConfiguredContainer();
        var api = sp.GetService<RestfulObjectsController>();
        //setDebugWarnings(api, false);
        //setMockContext(api, sp);
        return api!;
    }

    //private void ResetCache(api : RestfulObjectsControllerBase) {
    //    config.CacheSettings = (0, 3600, 86400);
    //    Api().ResetConfig(config);
    //}

    //private void setDebugWarnings (RestfulObjectsController api, bool flag) {
    //    var config = api.GetConfig();
    //    config.DebugWarnings = flag;
    //    Api().ResetConfig(config);
    //}

    [TestMethod]
    public void TestGetObject() { }
}