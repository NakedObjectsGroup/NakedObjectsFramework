// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Component;
using NakedFramework.Metamodel.SpecImmutable;
using Template.Test.Helpers;

namespace Template.Test.TestCase {
    public abstract class MSTestRestTestCase : ITestCase {
        private static IHost host;
        private static IServiceProvider rootServiceProvider;

        private IServiceProvider scopeServiceProvider;

        protected MSTestRestTestCase() {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        }

        private IServiceScope ServiceScope { set; get; }

        public IServiceProvider GetConfiguredContainer() => scopeServiceProvider;

        private static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection> configureServices, Func<IDictionary<string, string>> configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configBuilder) => {
                    var config = new MemoryConfigurationSource {
                        InitialData = configuration()
                    };
                    configBuilder.Add(config);
                })
                .ConfigureServices((_, services) => { configureServices(services); });

        protected static void ConfigureServicesBase(IServiceCollection services) {
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddScoped(_ => TestHelpers.TestPrincipal);
            services.AddScoped<ISession, TestSession>();
        }

        public void StartServerTransaction() {
            ServiceScope = rootServiceProvider.CreateScope();
            scopeServiceProvider = ServiceScope.ServiceProvider;
        }

        public void EndServerTransaction() {
            ServiceScope?.Dispose();
            ServiceScope = null;
            scopeServiceProvider = null;
        }

        protected static void InitializeNakedObjectsFramework(Action<IServiceCollection> configureServices, Func<IDictionary<string, string>> configuration) {
            host = CreateHostBuilder(Array.Empty<string>(), configureServices, configuration).Build();
            rootServiceProvider = host.Services;
            rootServiceProvider.GetService<IModelBuilder>().Build();
        }

        protected static void CleanupNakedObjectsFramework() {
            ImmutableSpecFactory.ClearCache();
            rootServiceProvider.GetService<ISpecificationCache>().Clear();
            rootServiceProvider = null;
            host.StopAsync().GetAwaiter().GetResult();
            host.Dispose();
        }

        protected static IDictionary<string, string> ConfigurationBase() =>
            new Dictionary<string, string>();
    }

    // Copyright (c) Naked Objects Group Ltd.
}