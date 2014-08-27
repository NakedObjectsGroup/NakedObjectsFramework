// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Bounded;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.Security;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Service;
using NakedObjects.Xat.Performance;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Xat {
    public abstract class AcceptanceTestCase : RunStandaloneBase {
        private static readonly ILog LOG;
        private static Profiler classProfiler;
        private readonly Profiler methodProfiler = new Profiler("method");
        private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
        private ITestObjectFactory testObjectFactory;

        static AcceptanceTestCase() {
            LOG = LogManager.GetLogger(typeof (AcceptanceTestCase));
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
        }

        protected AcceptanceTestCase() : this("Unnamed") {}

        protected string Name { set; get; }

        private bool ProfilerOn { get; set; }

        protected virtual ITestObjectFactory TestObjectFactoryClass {
            get {
                return new TestObjectFactory(NakedObjectsContext.Reflector, NakedObjectsContext.Session, NakedObjectsContext.ObjectPersistor);
            }
        }

        protected virtual ISession TestSession {
            get { return new SimpleSession(CreatePrincipal("Test", new string[] {})); }
        }

        protected override NakedObjectsContext Context {
            get {
                NakedObjectsContext ctx = StaticContext.CreateInstance();
                return ctx;
            }
        }

        // make framework available to tests 
        private readonly NakedObjectsFramework nakedObjectsFramework = null;//new NakedObjectsFramework();
        private IContainerInjector injector = null;


        protected INakedObjectsFramework NakedObjectsContext {
            get {
                return nakedObjectsFramework;
            }
        }

        protected IContainerInjector Injector {
            get {
                if (injector == null) {
                    //injector = new DotNetDomainObjectContainerInjector(NakedObjectsContext.Reflector, NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray());
                }
                return injector;
            }
        }

        protected ITestService GetTestService(Type type) {
            return NakedObjectsContext.ObjectPersistor.GetServices().
                                       Where(no => type.IsAssignableFrom(no.Object.GetType())).
                                       Select(no => testObjectFactory.CreateTestService(no.Object)).
                                       FirstOrDefault();
        }

        protected ITestService GetTestService(string serviceName) {
            if (!servicesCache.ContainsKey(serviceName.ToLower())) {
                foreach (INakedObject service in NakedObjectsContext.ObjectPersistor.GetServices()) {
                    if (service.TitleString().Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)) {
                        ITestService testService = testObjectFactory.CreateTestService(service.Object);
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
            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(type);
            return GetBoundedInstance(title, spec);
        }

        protected ITestObject GetBoundedInstance(string classname, string title) {
            INakedObjectSpecification spec = NakedObjectsContext.Reflector.LoadSpecification(classname);
            return GetBoundedInstance(title, spec);
        }

        private ITestObject GetBoundedInstance(string title, INakedObjectSpecification spec) {
            if (spec.GetFacet<IBoundedFacet>() == null) {
                Assert.Fail(spec.SingularName + " is not a Bounded type");
            }
            IEnumerable allInstances = NakedObjectsContext.ObjectPersistor.Instances(spec);
            object inst = allInstances.Cast<object>().Single(o => NakedObjectsContext.ObjectPersistor.CreateAdapter(o, null, null).TitleString() == title);
            return testObjectFactory.CreateTestObject(NakedObjectsContext.ObjectPersistor.CreateAdapter(inst, null, null));
        }

        private static IPrincipal CreatePrincipal(string name, string[] roles) {
            return new GenericPrincipal(new GenericIdentity(name), roles);
        }

        protected  void SetUser(string username, params string[] roles) {
            var staticContext = (StaticContext)  Core.Context.NakedObjectsContext.Instance;
            ISession session = new SimpleSession(CreatePrincipal(username, roles));
            staticContext.SetSession(session);
            testObjectFactory.Session = session;
            //NakedObjectsContext.ObjectPersistor.Session = session;
        }

        protected  void SetUser(string username) {
            SetUser(username, new string[] {});
        }


        protected void InitializeNakedObjectsFramework() {
            LOG.Info("test initialize " + Name);
            servicesCache = new Dictionary<string, ITestService>();
            
            methodProfiler.Start();

            SetupSystem();
            System.Init();

            System.Connect(TestSession);
            testObjectFactory = TestObjectFactoryClass;
            methodProfiler.Stop();

            LOG.Info("test initialize complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.Write(Name + ": \t" + methodProfiler.TimeLog());
            }
            NakedObjectsContext.ObjectPersistor.Reset();
        }

        protected void CleanupNakedObjectsFramework() {
            LOG.Info("test cleanup " + Name);
            methodProfiler.Reset();
            methodProfiler.Start();

            LOG.Info("cleanup " + Name);

            servicesCache.Clear();
            servicesCache = null;

            testObjectFactory = null;

            methodProfiler.Stop();
            System.Disconnect(TestSession);
            System.Shutdown();
            LOG.Info("test cleanup complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.WriteLine(" \t" + methodProfiler.TimeLog());
            }
        }

        public virtual void StartMethodProfiling() {
            LOG.Info("test run " + Name);
            methodProfiler.Reset();
            methodProfiler.Start();
        }

        public virtual void StopMethodProfiling() {
            methodProfiler.Stop();
            LOG.Info("test run complete " + Name + " " + methodProfiler.TimeLog());
            if (ProfilerOn) {
                Console.Out.Write(" \t" + methodProfiler.TimeLog());
            }
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}