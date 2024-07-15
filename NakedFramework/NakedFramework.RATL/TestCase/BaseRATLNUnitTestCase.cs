using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Extensions.Configuration.Memory;
using NakedFramework.Architecture.Component;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.API;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.TestCase;

public abstract class BaseRATLNUnitTestCase {
    private static IHost host;

    protected IServiceProvider RootServiceProvider;
    private IServiceProvider scopeServiceProvider;

    protected virtual IServiceScope ServiceScope { set; get; }

    protected static IPrincipal TestPrincipal => CreatePrincipal("Test", Array.Empty<string>());

    protected virtual IDictionary<string, string> Configuration() =>
        new Dictionary<string, string>();

    protected virtual IConfigurationBuilder AddUserSecrets(IConfigurationBuilder configBuilder) => configBuilder;

    private IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults((a) => { })
            .ConfigureAppConfiguration((hostContext, configBuilder) => {
                var config = new MemoryConfigurationSource {
                    InitialData = Configuration()
                };
                configBuilder.Add(config);
                AddUserSecrets(configBuilder);
            })
            .ConfigureServices((hostContext, services) => ConfigureServices(hostContext, services));

    protected virtual void StartTest() {
        ServiceScope = RootServiceProvider.CreateScope();
        scopeServiceProvider = ServiceScope.ServiceProvider;
    }

    protected virtual void EndTest() {
        ServiceScope?.Dispose();
        ServiceScope = null;
        scopeServiceProvider = null;
    }

    protected static void InitializeNakedObjectsFramework(BaseRATLNUnitTestCase tc) {
        host = tc.CreateHostBuilder(Array.Empty<string>()).Build();
        tc.RootServiceProvider = host.Services;
        tc.RootServiceProvider.GetService<IModelBuilder>().Build();
    }

    protected static void CleanupNakedObjectsFramework(BaseRATLNUnitTestCase tc) {
        ImmutableSpecFactory.ClearCache();
        tc.RootServiceProvider.GetService<ISpecificationCache>().Clear();
        tc.EndTest();

        tc.RootServiceProvider = null;
        host.StopAsync().GetAwaiter().GetResult();
        host.Dispose();
    }

    protected abstract void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services);

    public RestfulObjectsControllerBase Api() {
        var sp = scopeServiceProvider;
        var api = sp.GetService<RestfulObjectsController>();
        return TestHelpers.SetMockContext(api, sp);
    }

    protected static IPrincipal CreatePrincipal(string name, string[] roles) => new GenericPrincipal(new GenericIdentity(name), roles);

    public DomainObject GetObject(string type, string id) => ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;

    public DomainObject GetService(string type) => ROSIApi.GetService(new Uri("http://localhost/"), type, TestInvokeOptions()).Result;

    public string FullName<T>() => typeof(T).FullName;

    public TestInvokeOptions TestInvokeOptions(string? token = null, EntityTagHeaderValue? tag = null) =>
        new(Api) {
            Token = token,
            Tag = tag
        };
}

// Copyright (c) Naked Objects Group Ltd.