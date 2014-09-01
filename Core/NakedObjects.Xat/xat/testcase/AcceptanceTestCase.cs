// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Common.Logging;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Bounded;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Boot;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Security;
using NakedObjects.EntityObjectStore;
using NakedObjects.Objects;
using NakedObjects.Persistor;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Reflector.DotNet.Facets;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Service;
using NakedObjects.Xat.Performance;

namespace NakedObjects.Xat {
    public abstract class AcceptanceTestCase {
        private static readonly ILog Log;
        private static Profiler classProfiler;
        private readonly Lazy<IUnityContainer> unityContainer;
        private readonly Profiler methodProfiler = new Profiler("method");
        private INakedObjectsFramework nakedObjectsFramework;
        private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
        private IPrincipal testPrincipal;
        private ISession testSession;
        private ITestObjectFactory testObjectFactory;

        static AcceptanceTestCase() {
            Log = LogManager.GetLogger(typeof (AcceptanceTestCase));
        }

        protected AcceptanceTestCase(string name) {
            Name = name;

            if (classProfiler == null) {
                classProfiler = new Profiler("class");
            }
            else {
                classProfiler.Reset();
                classProfiler.Start();
            }

            unityContainer = new Lazy<IUnityContainer>(() => {
                var c = new UnityContainer();
                RegisterTypes(c);
                return c;
            });
        }

        protected AcceptanceTestCase() : this("Unnamed") {}

        protected string Name { set; get; }

        private bool ProfilerOn { get; set; }

        protected virtual ITestObjectFactory TestObjectFactoryClass {
            get {
                if (testObjectFactory == null) {
                    testObjectFactory = new TestObjectFactory(NakedObjectsFramework.Reflector, NakedObjectsFramework.Session, NakedObjectsFramework.ObjectPersistor);
                }
                return testObjectFactory;
            }
        }

        protected virtual ISession TestSession {
            get {
                if (testSession == null) {
                    testSession = new SimpleSession(TestPrincipal);
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

        protected virtual IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected virtual IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected virtual IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected virtual IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected virtual IObjectPersistorInstaller Persistor {
            get { return null; }
        }

        protected void StartTest() {
            nakedObjectsFramework = GetConfiguredContainer().Resolve<INakedObjectsFramework>();
            Fixtures.InstallFixtures(nakedObjectsFramework.ObjectPersistor, nakedObjectsFramework.Injector);
        }

        protected ITestService GetTestService(Type type) {
            return NakedObjectsFramework.ObjectPersistor.GetServices().
                Where(no => type.IsAssignableFrom(no.Object.GetType())).
                Select(no => TestObjectFactoryClass.CreateTestService(no.Object)).
                FirstOrDefault();
        }

        protected ITestService GetTestService(string serviceName) {
            if (!servicesCache.ContainsKey(serviceName.ToLower())) {
                foreach (INakedObject service in NakedObjectsFramework.ObjectPersistor.GetServices()) {
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

        protected ITestObject GetBoundedInstance<T>(string title) {
            return GetBoundedInstance(typeof (T), title);
        }

        protected ITestObject GetBoundedInstance(Type type, string title) {
            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(type);
            return GetBoundedInstance(title, spec);
        }

        protected ITestObject GetBoundedInstance(string classname, string title) {
            INakedObjectSpecification spec = NakedObjectsFramework.Reflector.LoadSpecification(classname);
            return GetBoundedInstance(title, spec);
        }

        private ITestObject GetBoundedInstance(string title, INakedObjectSpecification spec) {
            if (spec.GetFacet<IBoundedFacet>() == null) {
                Assert.Fail(spec.SingularName + " is not a Bounded type");
            }
            IEnumerable allInstances = NakedObjectsFramework.ObjectPersistor.Instances(spec);
            object inst = allInstances.Cast<object>().Single(o => NakedObjectsFramework.ObjectPersistor.CreateAdapter(o, null, null).TitleString() == title);
            return TestObjectFactoryClass.CreateTestObject(NakedObjectsFramework.ObjectPersistor.CreateAdapter(inst, null, null));
        }

        private static IPrincipal CreatePrincipal(string name, string[] roles) {
            return new GenericPrincipal(new GenericIdentity(name), roles);
        }

        protected void SetUser(string username, params string[] roles) {
            testPrincipal = CreatePrincipal(username, roles);
        }

        protected void SetUser(string username) {
            SetUser(username, new string[] {});
        }

        protected void InitializeNakedObjectsFramework() {
            Log.Info("test initialize " + Name);
            servicesCache = new Dictionary<string, ITestService>();

            Log.Info("test initialize complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.Write(Name + ": \t" + methodProfiler.TimeLog());
            }

            var reflector = GetConfiguredContainer().Resolve<INakedObjectReflector>();

            List<Type> s1 = MenuServices.GetServices().Select(s => s.GetType()).ToList();
            List<Type> s2 = ContributedActions.GetServices().Select(s => s.GetType()).ToList();
            List<Type> s3 = SystemServices.GetServices().Select(s => s.GetType()).ToList();
            Type[] services = s1.Union(s2).Union(s3).ToArray();

            reflector.InstallServiceSpecifications(services);
        }

        protected void CleanupNakedObjectsFramework() {
            Log.Info("test cleanup " + Name);
            Log.Info("cleanup " + Name);

            servicesCache.Clear();
            servicesCache = null;

            testObjectFactory = null;
  
            Log.Info("test cleanup complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.WriteLine(" \t" + methodProfiler.TimeLog());
            }
        }

        protected virtual void StartMethodProfiling() {
            Log.Info("test run " + Name);
            methodProfiler.Reset();
            methodProfiler.Start();
        }

        protected virtual void StopMethodProfiling() {
            methodProfiler.Stop();
            Log.Info("test run complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.Write(" \t" + methodProfiler.TimeLog());
            }
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
        
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySetImpl>();
            container.RegisterType<INakedObjectReflector, DotNetReflector>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPrincipal>(new InjectionFactory(c => TestPrincipal));

            var config = new EntityObjectStoreConfiguration();

            //config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            //config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });

            container.RegisterInstance(typeof (EntityObjectStoreConfiguration), config, new ContainerControlledLifetimeManager());

            var serviceConfig = new ServicesConfiguration();

            serviceConfig.AddMenuServices(MenuServices.GetServices());
            serviceConfig.AddContributedActions(ContributedActions.GetServices());
            serviceConfig.AddSystemServices(SystemServices.GetServices());

            container.RegisterInstance(typeof (ServicesConfiguration), serviceConfig, new ContainerControlledLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));

            container.RegisterType<IContainerInjector, DotNetDomainObjectContainerInjector>(new PerResolveLifetimeManager());

            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerResolveLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectStore, EntityObjectStore.EntityObjectStore>(new PerResolveLifetimeManager());
            container.RegisterType<IIdentityMap, EntityIdentityMapImpl>(new PerResolveLifetimeManager());

            container.RegisterType<IAuthorizationManager, NullAuthorizationManager>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectPersistor, ObjectStorePersistor>(new PerResolveLifetimeManager());

            container.RegisterType<ISession>(new PerResolveLifetimeManager(), new InjectionFactory(c => TestSession));

            container.RegisterType<IUpdateNotifier, SimpleUpdateNotifier>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, SimpleMessageBroker>(new PerResolveLifetimeManager());

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