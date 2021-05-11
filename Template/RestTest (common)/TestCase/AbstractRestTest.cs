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
using Template.RestTest.Helpers;

namespace Template.RestTest.TestCase {
    public abstract class AbstractRestTest {
        protected AbstractRestTest() {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        }

        private static IHost Host { get; set; }
        private static IServiceProvider RootServiceProvider { get; set; }

        private IServiceProvider ScopeServiceProvider { get; set; }
        private IServiceScope ServiceScope { set; get; }

        private static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection> configureServices, Func<IDictionary<string, string>> configuration) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                     .ConfigureAppConfiguration((_, configBuilder) => {
                         var config = new MemoryConfigurationSource {
                             InitialData = configuration()
                         };
                         configBuilder.Add(config);
                     })
                     .ConfigureServices((_, services) => { configureServices(services); });

        #region used by subclasses

        protected static void ConfigureServicesBase(IServiceCollection services) {
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddScoped(_ => TestHelpers.TestPrincipal);
            services.AddScoped<ISession, TestSession>();
        }

        protected static void InitializeNakedFramework(Action<IServiceCollection> configureServices, Func<IDictionary<string, string>> configuration) {
            Host = CreateHostBuilder(Array.Empty<string>(), configureServices, configuration).Build();
            RootServiceProvider = Host.Services;
            RootServiceProvider.GetService<IModelBuilder>().Build();
        }

        protected static void CleanupNakedFramework() {
            ImmutableSpecFactory.ClearCache();
            RootServiceProvider.GetService<ISpecificationCache>().Clear();
            RootServiceProvider = null;
            Host.StopAsync().GetAwaiter().GetResult();
            Host.Dispose();
        }

        protected static IDictionary<string, string> ConfigurationBase() => new Dictionary<string, string>();

        #endregion

        #region used by helpers

        public void StartServerTransaction() {
            ServiceScope = RootServiceProvider.CreateScope();
            ScopeServiceProvider = ServiceScope.ServiceProvider;
        }

        public void EndServerTransaction() {
            ServiceScope?.Dispose();
            ServiceScope = null;
            ScopeServiceProvider = null;
        }

        public IServiceProvider GetScopeServices() => ScopeServiceProvider;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}