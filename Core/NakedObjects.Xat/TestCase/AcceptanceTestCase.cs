// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Common.Logging;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Fixture;
using NakedObjects.Core.Util;
using NakedObjects.DependencyInjection;
using NakedObjects.Menu;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Persistor.Entity.Configuration;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Xat {
    public abstract class AcceptanceTestCase {
        private FixtureServices fixtureServices;

        protected IServiceProvider RootServiceProvider;
        private IServiceProvider scopeServiceProvider;
        private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
        private ITestObjectFactory testObjectFactory;
        private IPrincipal testPrincipal;

        protected AcceptanceTestCase(string name) {
            Name = name;
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        }

        protected AcceptanceTestCase() : this("Unnamed") { }

        protected virtual IServiceScope ServiceScope { set; get; }
        protected string Name { set; get; }

        protected virtual ITestObjectFactory TestObjectFactoryClass => testObjectFactory ??= new TestObjectFactory(NakedObjectsFramework.MetamodelManager, NakedObjectsFramework.Session, NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.TransactionManager, NakedObjectsFramework.ServicesManager, NakedObjectsFramework.MessageBroker);

        protected virtual IPrincipal TestPrincipal {
            get { return testPrincipal ??= CreatePrincipal("Test", new string[] { }); }
            set => testPrincipal = value;
        }

        protected INakedObjectsFramework NakedObjectsFramework { get; set; }

        protected ILoggerFactory LoggerFactory { get; set; }

        protected virtual object[] Fixtures {
            get { return new object[] { }; }
        }

        /// <summary>
        ///     By default this returns the union of the types specified in MenuServices, ContributedActions
        ///     & SystemServices. This is for backwards compatibility only.
        ///     The property may be overridden to return a fresh list of types, in which case Menu Services etc
        ///     will be ignored.
        /// </summary>
        protected virtual Type[] Services {
            get { return new Type[] { }; }
        }

        protected virtual Type[] Types {
            get { return new Type[] { }; }
        }

        protected virtual string[] Namespaces {
            get { return new string[] { }; }
        }

        protected virtual EntityObjectStoreConfiguration Persistor => new EntityObjectStoreConfiguration();

        protected virtual ReflectorConfiguration Reflector {
            get {
                var reflectorConfig = new ReflectorConfiguration(
                    Types ?? new Type[] { },
                    Services,
                    Namespaces ?? new string[] { },
                    MainMenus);
                ReflectorConfiguration.NoValidate = true;
                return reflectorConfig;
            }
        }

        private IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configBuilder) => {
                    var config = new MemoryConfigurationSource {
                        InitialData = Configuration()
                    };
                    configBuilder.Add(config);
                })
                .ConfigureServices((hostContext, services) => {
                    RegisterTypes(services);
                });

        /// <summary>
        ///     Gets the configured service provider
        /// </summary>
        protected virtual IServiceProvider GetConfiguredContainer() => scopeServiceProvider;

        protected virtual IMenu[] MainMenus(IMenuFactory factory) => null; //Allows tests not to define menus if not needed.

        protected virtual void StartTest() {
            ServiceScope = RootServiceProvider.CreateScope();
            scopeServiceProvider = ServiceScope.ServiceProvider;
            NakedObjectsFramework = scopeServiceProvider.GetService<INakedObjectsFramework>();
            LoggerFactory = scopeServiceProvider.GetService<ILoggerFactory>();
        }

        protected virtual void EndTest() {
            ServiceScope?.Dispose();
            ServiceScope = null;
            scopeServiceProvider = null;
            servicesCache = new Dictionary<string, ITestService>();
            testObjectFactory = null;
            testPrincipal = null;
        }

        private void InstallFixtures(ITransactionManager transactionManager, IDomainObjectInjector injector, object[] newFixtures) {
            foreach (var fixture in newFixtures) {
                InstallFixture(transactionManager, injector, fixture);
            }
        }

        private static void SetValue(PropertyInfo property, object injectee, object value) {
            if (property != null) {
                try {
                    property.SetValue(injectee, value, null);
                }
                catch (TargetInvocationException e) {
                    InvokeUtils.InvocationException("Exception executing " + property, e);
                }
            }
        }

        protected virtual void PreInstallFixtures(ITransactionManager transactionManager) {
            fixtureServices = new FixtureServices();
        }

        private static MethodInfo GetInstallMethod(object fixture) =>
            fixture.GetType().GetMethod("Install", new Type[0]) ??
            fixture.GetType().GetMethod("install", new Type[0]);

        protected virtual object[] GetFixtures(object fixture) {
            var getFixturesMethod = fixture.GetType().GetMethod("GetFixtures", new Type[] { });
            return getFixturesMethod == null ? new object[] { } : (object[]) getFixturesMethod.Invoke(fixture, new object[] { });
        }

        protected virtual void InstallFixture(object fixture) {
            var property = fixture.GetType().GetProperty("Service");
            SetValue(property, fixture, fixtureServices);

            var installMethod = GetInstallMethod(fixture);
            try {
                installMethod.Invoke(fixture, new object[0]);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + installMethod, e);
            }
        }

        private void InstallFixture(ITransactionManager transactionManager, IDomainObjectInjector injector, object fixture) {
            injector.InjectInto(fixture);

            // first, install any child fixtures (if this is a composite.
            var childFixtures = GetFixtures(fixture);
            InstallFixtures(transactionManager, injector, childFixtures);

            // now, install the fixture itself
            try {
                transactionManager.StartTransaction();
                InstallFixture(fixture);
                transactionManager.EndTransaction();
            }
            catch (Exception e) {
                var msg = $"installing fixture {fixture.GetType().FullName} failed ({e.Message}); aborting fixture ";
                try {
                    transactionManager.AbortTransaction();
                }
                catch (Exception e2) {
                    msg = $"{msg} - {e2.Message} failure during abort";
                }

                throw new NakedObjectSystemException(msg, e);
            }
        }

        protected virtual void RunFixtures() {
            using var fixtureServiceScope = RootServiceProvider.CreateScope();
            NakedObjectsFramework = fixtureServiceScope.ServiceProvider.GetService<INakedObjectsFramework>();
            InstallFixtures(NakedObjectsFramework.TransactionManager, NakedObjectsFramework.DomainObjectInjector, Fixtures);
            NakedObjectsFramework = null;
        }

        protected ITestService GetTestService<T>() => GetTestService(typeof(T));

        protected virtual ITestService GetTestService(Type type) {
            var testService = NakedObjectsFramework.ServicesManager.GetServices().Where(no => type.IsInstanceOfType(no.Object)).Select(no => TestObjectFactoryClass.CreateTestService(no.Object)).FirstOrDefault();
            if (testService == null) {
                Assert.Fail("No service of type " + type);
            }

            return testService;
        }

        protected virtual ITestService GetTestService(string serviceName) {
            if (!servicesCache.ContainsKey(serviceName.ToLower())) {
                foreach (var service in NakedObjectsFramework.ServicesManager.GetServices()) {
                    if (service.TitleString().Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)) {
                        var testService = TestObjectFactoryClass.CreateTestService(service.Object);
                        if (testService == null) {
                            Assert.Fail("Invalid service name " + serviceName);
                        }

                        servicesCache[serviceName.ToLower()] = testService;
                        return testService;
                    }
                }

                Assert.Fail("No such service: " + serviceName);
                return null;
            }

            return servicesCache[serviceName.ToLower()];
        }

        protected virtual ITestMenu GetMainMenu(string menuName) {
            var mainMenus = NakedObjectsFramework.MetamodelManager.MainMenus();
            if (mainMenus.Any()) {
                var menu = mainMenus.FirstOrDefault(m => m.Name == menuName);
                if (menu == null) {
                    Assert.Fail("No such main menu " + menuName);
                }

                return TestObjectFactoryClass.CreateTestMenuMain(menu);
            }

            //Use the MenuServices to derive the menus
            var service = GetTestService(menuName);
            if (service == null) {
                Assert.Fail("No such main menu, or Service, " + menuName);
            }

            return service.GetMenu();
        }

        protected virtual ITestMenu[] AllMainMenus() {
            return NakedObjectsFramework.MetamodelManager.MainMenus().Select(m => TestObjectFactoryClass.CreateTestMenuMain(m)).ToArray();
        }

        protected virtual void AssertMainMenuCountIs(int expected) {
            var actual = NakedObjectsFramework.MetamodelManager.MainMenus().Length;
            Assert.AreEqual(expected, actual);
        }

        protected virtual ITestObject GetBoundedInstance<T>(string title) => GetBoundedInstance(typeof(T), title);

        protected virtual ITestObject GetBoundedInstance(Type type, string title) {
            var spec = (IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(type);
            return GetBoundedInstance(title, spec);
        }

        protected virtual ITestObject GetBoundedInstance(string classname, string title) {
            var spec = (IObjectSpec) NakedObjectsFramework.MetamodelManager.GetSpecification(classname);
            return GetBoundedInstance(title, spec);
        }

        private ITestObject GetBoundedInstance(string title, IObjectSpec spec) {
            if (spec.GetFacet<IBoundedFacet>() == null) {
                Assert.Fail(spec.SingularName + " is not a Bounded type");
            }

            IEnumerable allInstances = NakedObjectsFramework.Persistor.Instances(spec);
            var inst = allInstances.Cast<object>().Single(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null).TitleString() == title);
            return TestObjectFactoryClass.CreateTestObject(NakedObjectsFramework.NakedObjectManager.CreateAdapter(inst, null, null));
        }

        private IPrincipal CreatePrincipal(string name, string[] roles) {
            return testPrincipal = new GenericPrincipal(new GenericIdentity(name), roles);
        }

        protected virtual void SetUser(string username, params string[] roles) {
            testPrincipal = CreatePrincipal(username, roles);
            var ts = NakedObjectsFramework == null ? null : NakedObjectsFramework.Session as TestSession;
            ts?.ReplacePrincipal(testPrincipal);
        }

        protected virtual void SetUser(string username) {
            SetUser(username, new string[] { });
        }

        protected static void InitializeNakedObjectsFramework(AcceptanceTestCase tc) {
            var host = tc.CreateHostBuilder(new string[] { }).Build();
            tc.RootServiceProvider = host.Services;
            tc.servicesCache = new Dictionary<string, ITestService>();
            tc.RootServiceProvider.GetService<IReflector>().Reflect();
        }

        protected static void CleanupNakedObjectsFramework(AcceptanceTestCase tc) {
            // TODO THIS IS A SMELL !!
            ImmutableSpecFactory.ClearCache();
            tc.RootServiceProvider.GetService<ISpecificationCache>().Clear();
            tc.EndTest();
            tc.servicesCache.Clear();
            tc.servicesCache = null;
            tc.testObjectFactory = null;
            tc.RootServiceProvider = null;
        }

        protected virtual void RegisterTypes(IServiceCollection services) {
            //Standard configuration
            ParallelConfig.RegisterStandardFacetFactories(services);
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            // config 
            services.AddSingleton<IReflectorConfiguration>(Reflector);
            services.AddSingleton<IEntityObjectStoreConfiguration>(Persistor);

            //Externals
            services.AddScoped(p => TestPrincipal);
            services.AddScoped<ISession, TestSession>();
        }

        protected virtual IDictionary<string, string> Configuration() =>
            new Dictionary<string, string> {
                {"NakedObjects:HashMapCapacity", "10"}
            };
    }

    // Copyright (c) Naked Objects Group Ltd.
}