// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Common.Logging;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Fixture;
using NakedObjects.Core.spec;
using NakedObjects.Core.Util;
using NakedObjects.EntityObjectStore;
using NakedObjects.Managers;
using NakedObjects.Meta;
using NakedObjects.Reflect;
using NakedObjects.Service;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Xat {
    public abstract class AcceptanceTestCase {
        private static readonly ILog Log;
        private readonly Lazy<IUnityContainer> unityContainer;
        private INakedObjectsFramework nakedObjectsFramework;
        private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
        private ITestObjectFactory testObjectFactory;
        private IPrincipal testPrincipal;
        private ISession testSession;

        static AcceptanceTestCase() {
            Log = LogManager.GetLogger(typeof (AcceptanceTestCase));
        }

        protected AcceptanceTestCase(string name) {
            Name = name;

            unityContainer = new Lazy<IUnityContainer>(() => {
                var c = new UnityContainer();
                RegisterTypes(c);
                return c;
            });
        }

        protected AcceptanceTestCase() : this("Unnamed") {}

        protected string Name { set; get; }

        protected virtual ITestObjectFactory TestObjectFactoryClass {
            get {
                if (testObjectFactory == null) {
                    testObjectFactory = new TestObjectFactory(NakedObjectsFramework.Metamodel, NakedObjectsFramework.Session, NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.Manager, NakedObjectsFramework.TransactionManager, NakedObjectsFramework.Services);
                }
                return testObjectFactory;
            }
        }

        protected virtual ISession TestSession {
            get {
                if (testSession == null) {
                    testSession = new TestSession(TestPrincipal);
                }
                return testSession;
            }
            set { testSession = value; }
        }

        protected virtual IPrincipal TestPrincipal {
            get {
                if (testPrincipal == null) {
                    testPrincipal = CreatePrincipal("Test", new string[] {});
                }
                return testPrincipal;
            }
            set { testPrincipal = value; }
        }

        [Obsolete("Use NakedObjectsFramework")]
        protected INakedObjectsFramework NakedObjectsContext {
            get { return nakedObjectsFramework; }
        }

        protected INakedObjectsFramework NakedObjectsFramework {
            get { return nakedObjectsFramework; }
        }

        protected virtual object[] Fixtures {
            get { return new object[] {}; }
        }

        protected virtual object[] MenuServices {
            get { return new object[] {}; }
        }

        protected virtual object[] ContributedActions {
            get { return new object[] {}; }
        }

        protected virtual object[] SystemServices {
            get { return new object[] {}; }
        }

        protected void StartTest() {
            nakedObjectsFramework = GetConfiguredContainer().Resolve<INakedObjectsFramework>();
        }

        private void InstallFixtures(ITransactionManager transactionManager, IContainerInjector injector, object[] newFixtures) {
            foreach (object fixture in newFixtures) {
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
            else {
            }
        }

        private FixtureServices fixtureServices;

       

        protected  void PreInstallFixtures(ITransactionManager transactionManager) {
            fixtureServices = new FixtureServices();
        }


        private static MethodInfo GetInstallMethod(object fixture) {
            return fixture.GetType().GetMethod("Install", new Type[0]) ??
                   fixture.GetType().GetMethod("install", new Type[0]);
        }

        protected object[] GetFixtures(object fixture) {
            MethodInfo getFixturesMethod = fixture.GetType().GetMethod("GetFixtures", new Type[] { });
            return getFixturesMethod == null ? new object[] { } : (object[])getFixturesMethod.Invoke(fixture, new object[] { });
        }

        protected  void InstallFixture(object fixture) {
            PropertyInfo property = fixture.GetType().GetProperty("Service");
            SetValue(property, fixture, fixtureServices);

            MethodInfo installMethod = GetInstallMethod(fixture);
            Log.Debug("Invoking install method");
            try {
                installMethod.Invoke(fixture, new object[0]);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + installMethod, e);
            }
        }

        private void InstallFixture(ITransactionManager transactionManager, IContainerInjector injector, object fixture) {
            injector.InitDomainObject(fixture);

            // first, install any child fixtures (if this is a composite.
            object[] childFixtures = GetFixtures(fixture);
            InstallFixtures(transactionManager, injector, childFixtures);

            // now, install the fixture itself
            try {
                Log.Info("installing fixture: " + fixture);
                transactionManager.StartTransaction();
                InstallFixture(fixture);
                transactionManager.EndTransaction();
                Log.Info("fixture installed");
            }
            catch (Exception e) {
                Log.Error("installing fixture " + fixture.GetType().FullName + " failed (" + e.Message + "); aborting fixture ", e);
                try {
                    transactionManager.AbortTransaction();
                }
                catch (Exception e2) {
                    Log.Error("failure during abort", e2);
                }
                throw;
            }
        }

        protected void RunFixtures() {
            if (nakedObjectsFramework == null) {
                nakedObjectsFramework = GetConfiguredContainer().Resolve<INakedObjectsFramework>();
            }
            InstallFixtures(NakedObjectsFramework.TransactionManager, NakedObjectsFramework.Injector, Fixtures);
        }

        protected ITestService GetTestService(Type type) {
            return NakedObjectsFramework.Services.GetServices().
                Where(no => type.IsAssignableFrom(no.Object.GetType())).
                Select(no => TestObjectFactoryClass.CreateTestService(no.Object)).
                FirstOrDefault();
        }

        protected ITestService GetTestService(string serviceName) {
            if (!servicesCache.ContainsKey(serviceName.ToLower())) {
                foreach (INakedObject service in NakedObjectsFramework.Services.GetServices()) {
                    if (service.TitleString().Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)) {
                        ITestService testService = TestObjectFactoryClass.CreateTestService(service.Object);
                        if (testService == null) {
                            Assert.Fail("Invalid service name " + serviceName);
                        }
                        servicesCache[serviceName.ToLower()] = testService;
                        return testService;
                    }
                }
                Assert.Fail("No such service: " + serviceName);
            }
            return servicesCache[serviceName.ToLower()];
        }

        protected ITestMenu GetMainMenu(string menuName) {
            IMenuImmutable menu = NakedObjectsFramework.Metamodel.MainMenus().FirstOrDefault(m => m.Name == menuName);
            if (menu == null) {
                Assert.Fail("No such main menu " + menuName);
            }
            return TestObjectFactoryClass.CreateTestMenuMain(menu);
        }

        protected ITestMenu[] AllMainMenus() {
            return NakedObjectsFramework.Metamodel.MainMenus().Select(m => TestObjectFactoryClass.CreateTestMenuMain(m)).ToArray();
        }

        protected void AssertMainMenuCountIs(int expected) {
            var actual = NakedObjectsFramework.Metamodel.MainMenus().Count();
            Assert.AreEqual(expected, actual);
        }

        protected ITestObject GetBoundedInstance<T>(string title) {
            return GetBoundedInstance(typeof (T), title);
        }

        protected ITestObject GetBoundedInstance(Type type, string title) {
            IObjectSpec spec = NakedObjectsFramework.Metamodel.GetSpecification(type);
            return GetBoundedInstance(title, spec);
        }

        protected ITestObject GetBoundedInstance(string classname, string title) {
            IObjectSpec spec = NakedObjectsFramework.Metamodel.GetSpecification(classname);
            return GetBoundedInstance(title, spec);
        }

        private ITestObject GetBoundedInstance(string title, IObjectSpec spec) {
            if (spec.GetFacet<IBoundedFacet>() == null) {
                Assert.Fail(spec.SingularName + " is not a Bounded type");
            }
            IEnumerable allInstances = NakedObjectsFramework.Persistor.Instances(spec);
            object inst = allInstances.Cast<object>().Single(o => NakedObjectsFramework.Manager.CreateAdapter(o, null, null).TitleString() == title);
            return TestObjectFactoryClass.CreateTestObject(NakedObjectsFramework.Manager.CreateAdapter(inst, null, null));
        }

        private static IPrincipal CreatePrincipal(string name, string[] roles) {
            return new GenericPrincipal(new GenericIdentity(name), roles);
        }

        protected void SetUser(string username, params string[] roles) {
            testPrincipal = CreatePrincipal(username, roles);
            var ts = TestSession as TestSession;
            if (ts != null) {
                ts.ReplacePrincipal(testPrincipal);
            }
        }

        protected void SetUser(string username) {
            SetUser(username, new string[] {});
        }

        protected static void InitializeNakedObjectsFramework(AcceptanceTestCase tc) {
            Log.Info("test initialize " + tc.Name);
            tc.servicesCache = new Dictionary<string, ITestService>();

            tc.GetConfiguredContainer().Resolve<IReflector>().Reflect();
        }

        protected static void CleanupNakedObjectsFramework(AcceptanceTestCase tc) {
            Log.Info("test cleanup " + tc.Name);
            Log.Info("cleanup " + tc.Name);

            tc.servicesCache.Clear();
            tc.servicesCache = null;
            tc.testObjectFactory = null;
        }

        public class NullMainMenuDefinition : IMainMenuDefinition {
            public IMenu[] MainMenus(IMenuFactory factory) {
                return new IMenu[]{};
            }
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<IMainMenuDefinition, NullMainMenuDefinition>();

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFacetFactorySet, FacetFactorySet>(new ContainerControlledLifetimeManager());

            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPrincipal>(new InjectionFactory(c => TestPrincipal));

            var config = new EntityObjectStoreConfiguration();

            //config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            //config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });

            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            // TODO still done for backward compatibility - 
            var reflectorConfig = new ReflectorConfiguration(new Type[] { },
                MenuServices.Select(s => s.GetType()).ToArray(),
                ContributedActions.Select(s => s.GetType()).ToArray(),
                SystemServices.Select(s => s.GetType()).ToArray());

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, (new ContainerControlledLifetimeManager()));

            container.RegisterType<IServicesConfiguration, ServicesConfiguration>(new ContainerControlledLifetimeManager());

            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerResolveLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerResolveLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));

            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerResolveLifetimeManager());

            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerResolveLifetimeManager());

            container.RegisterType<IObjectStore, EntityObjectStore.EntityObjectStore>(new PerResolveLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerResolveLifetimeManager());

            container.RegisterType<ITransactionManager, TransactionManager>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerResolveLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerResolveLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerResolveLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerResolveLifetimeManager());

            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());

            container.RegisterType<IAuthorizationManager, NullAuthorizationManager>(new PerResolveLifetimeManager());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerResolveLifetimeManager());

            container.RegisterType<ISession>(new PerResolveLifetimeManager(), new InjectionFactory(c => TestSession));

            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>();
        }

        /// <summary>
        ///     Gets the configured Unity unityContainer.
        /// </summary>
        protected IUnityContainer GetConfiguredContainer() {
            return unityContainer.Value;
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}