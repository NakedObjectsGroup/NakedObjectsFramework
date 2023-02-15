using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Facade.Interface;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Rattle.Helpers;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Configuration;

namespace NakedFramework.Rattle.TestCase;


public abstract class BaseRattleTestCase {
    private static IHost host;

    protected IServiceProvider RootServiceProvider;
    private IServiceProvider scopeServiceProvider;
    private IPrincipal testPrincipal;

    protected BaseRattleTestCase() {
    }

    protected virtual IDictionary<string, string> Configuration() =>
        new Dictionary<string, string>();

    protected virtual IServiceScope ServiceScope { set; get; }

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

    protected static void InitializeNakedObjectsFramework(BaseRattleTestCase tc) {
        host = tc.CreateHostBuilder(Array.Empty<string>()).Build();
        tc.RootServiceProvider = host.Services;
        tc.RootServiceProvider.GetService<IModelBuilder>().Build();
    }

    protected static void CleanupNakedObjectsFramework(BaseRattleTestCase tc) {
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

    protected virtual IPrincipal TestPrincipal {
        get { return testPrincipal ??= CreatePrincipal("Test", Array.Empty<string>()); }
    }
}

// Copyright (c) Naked Objects Group Ltd.