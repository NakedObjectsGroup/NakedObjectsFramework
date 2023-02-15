using System.Net.Http.Headers;
using System.Security.Principal;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Component;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.API;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.TestCase;

public abstract class BaseRATLTestCase {
    private static IHost host;

    protected IServiceProvider RootServiceProvider;
    private IServiceProvider scopeServiceProvider;
    private IPrincipal testPrincipal;

    protected virtual IServiceScope ServiceScope { set; get; }

    protected virtual IPrincipal TestPrincipal {
        get { return testPrincipal ??= CreatePrincipal("Test", Array.Empty<string>()); }
    }

    protected virtual IDictionary<string, string> Configuration() =>
        new Dictionary<string, string>();

    private IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configBuilder) => {
                var config = new MemoryConfigurationSource {
                    InitialData = Configuration()
                };
                configBuilder.Add(config);
            })
            .ConfigureServices((hostContext, services) => ConfigureServices(services));

    protected virtual void StartTest() {
        ServiceScope = RootServiceProvider.CreateScope();
        scopeServiceProvider = ServiceScope.ServiceProvider;
    }

    protected virtual void EndTest() {
        ServiceScope?.Dispose();
        ServiceScope = null;
        scopeServiceProvider = null;
        testPrincipal = null;
    }

    protected static void InitializeNakedObjectsFramework(BaseRATLTestCase tc) {
        host = tc.CreateHostBuilder(Array.Empty<string>()).Build();
        tc.RootServiceProvider = host.Services;
        tc.RootServiceProvider.GetService<IModelBuilder>().Build();
    }

    protected static void CleanupNakedObjectsFramework(BaseRATLTestCase tc) {
        ImmutableSpecFactory.ClearCache();
        tc.RootServiceProvider.GetService<ISpecificationCache>().Clear();
        tc.EndTest();

        tc.RootServiceProvider = null;
        host.StopAsync().GetAwaiter().GetResult();
        host.Dispose();
    }

    protected abstract void ConfigureServices(IServiceCollection services);

    public RestfulObjectsControllerBase Api() {
        var sp = scopeServiceProvider;
        var api = sp.GetService<RestfulObjectsController>();
        return TestHelpers.SetMockContext(api, sp);
    }

    private IPrincipal CreatePrincipal(string name, string[] roles) {
        return testPrincipal = new GenericPrincipal(new GenericIdentity(name), roles);
    }

    public DomainObject GetObject(string type, string id) => ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;

    public string FullName<T>() => typeof(T).FullName;

    public InvokeOptions TestInvokeOptions(string token = null, EntityTagHeaderValue tag = null) =>
        new() {
            Token = token,
            Tag = tag,
            HttpClient = new HttpClient(new StubHttpMessageHandler(Api()))
        };
}

// Copyright (c) Naked Objects Group Ltd.