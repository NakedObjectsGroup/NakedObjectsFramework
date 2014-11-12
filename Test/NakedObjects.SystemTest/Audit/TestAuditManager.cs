// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Audit;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Audit;
using NakedObjects.Services;
using NakedObjects.Xat;
using System.Data.Entity;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.SystemTest.Audit {
    [TestClass]
    public class TestAuditManager : AbstractSystemTest<AuditDbContext> {

        #region Run Configuration
        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new AuditConfiguration {DefaultAuditor = typeof (MyDefaultAuditor)};
            config.SetNameSpaceAuditors(fooAuditor, quxAuditor);
            container.RegisterInstance<IAuditConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IFacetDecorator, AuditManager>("AuditManager", new ContainerControlledLifetimeManager());

            var reflectorConfig = new ReflectorConfiguration(new Type[] {
                typeof(MyDefaultAuditor),
                typeof(FooAuditor),
                typeof(QuxAuditor),
                typeof(QueryableList<Foo>)
            },
                new[] {
                    typeof (SimpleRepository<Foo>),
                    typeof (SimpleRepository<Bar>),
                    typeof (SimpleRepository<Qux>), typeof (FooService),
                    typeof (BarService), typeof (QuxService)
                },
                new Type[] {},
                new Type[] {});

            

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
        }
        #endregion


        #region Setup/Teardown
      

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupNakedObjectsFramework(new TestAuditManager());
            Database.Delete(AuditDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            InitializeNakedObjectsFramework(this);
            StartTest();
            SetUser("sven");
        }

        [TestCleanup()]
        public void TestCleanup()
        {
        }
        #endregion

        #region "Services & Fixtures"

        private static FooAuditor fooAuditor = new FooAuditor();
        protected static MyDefaultAuditor myDefaultAuditor = new MyDefaultAuditor();
        protected static QuxAuditor quxAuditor = new QuxAuditor();

       
        

        public class Da : IAuditor {


            public IDomainObjectContainer Container { get; set; }
            public SimpleRepository<Foo> Service { get; set; }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                myDefaultAuditor.ActionInvoked(byPrincipal, actionName, onObject, queryOnly, withParameters);
            }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                myDefaultAuditor.ActionInvoked(byPrincipal, actionName, serviceName, queryOnly, withParameters);
            }

            public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                myDefaultAuditor.ObjectUpdated(byPrincipal, updatedObject);
            }

            public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                myDefaultAuditor.ObjectPersisted(byPrincipal, updatedObject);
            }
        }

        public class Fa : INamespaceAuditor {

            public IDomainObjectContainer Container { get; set; }
            public SimpleRepository<Foo> Service { get; set; }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                fooAuditor.ActionInvoked(byPrincipal, actionName, onObject, queryOnly, withParameters);
            }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                fooAuditor.ActionInvoked(byPrincipal, actionName, serviceName, queryOnly, withParameters);
            }

            public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                fooAuditor.ObjectUpdated(byPrincipal, updatedObject);
            }

            public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                fooAuditor.ObjectPersisted(byPrincipal, updatedObject);
            }

            public string NamespaceToAudit {
                get { return fooAuditor.NamespaceToAudit; }
            
            }
        }

        public class Qa : INamespaceAuditor {

            public IDomainObjectContainer Container { get; set; }
            public SimpleRepository<Foo> Service { get; set; }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                quxAuditor.ActionInvoked(byPrincipal, actionName, onObject, queryOnly, withParameters);
            }

            public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                quxAuditor.ActionInvoked(byPrincipal, actionName, serviceName, queryOnly, withParameters);
            }

            public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                quxAuditor.ObjectUpdated(byPrincipal, updatedObject);
            }

            public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {
                Assert.IsNotNull(Container);
                Assert.IsNotNull(Service);
                quxAuditor.ObjectPersisted(byPrincipal, updatedObject);
            }

            public string NamespaceToAudit {
                get { return quxAuditor.NamespaceToAudit; }
            
            }
        }

        #endregion

        private static void UnexpectedCall(string auditor) {
            Assert.Fail("Unexpected call to {0} auditor", auditor);
        }

        public static Action<IPrincipal, object> UnexpectedCallback(string auditor) {
            return (p, o) => UnexpectedCall(auditor);
        }

        public static Action<IPrincipal, string, object, bool, object[]> UnexpectedActionCallback(string auditor) {
            return (p, a, o, b, pp) => UnexpectedCall(auditor);
        }

        public static Action<IPrincipal, string, string, bool, object[]> UnexpectedServiceActionCallback(string auditor) {
            return (p, a, s, b, pp) => UnexpectedCall(auditor);
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorAction() {
            ITestObject foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();

            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorServiceAction() {
            ITestService foo = GetTestService("Foo Service");

            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.AreEqual("Foo Service", s);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorQueryOnlyAction() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AQueryOnlyAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsTrue(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("A Query Only Action").InvokeReturnObject();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorImplicitQueryOnlyAction() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnotherQueryOnlyAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsTrue(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("Another Query Only Action").InvokeReturnCollection();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorQueryOnlyServiceAction() {
            ITestService foo = GetTestService("Foo Service");

            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AQueryOnlyAction", a);
                Assert.AreEqual("Foo Service", s);
                Assert.IsTrue(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("A Query Only Action").InvokeReturnObject();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }


        [TestMethod]
        public void AuditUsingSpecificTypeAuditorActionWithParm() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParm", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsFalse(b);
                Assert.AreEqual(1, pp.Count());
                Assert.AreEqual(1, pp[0]);
                fooCalledCount++;
            };

            foo.GetAction("An Action With Parm").InvokeReturnObject(1);
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorServiceActionWithParm() {
            ITestService foo = GetTestService("Foo Service");
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParm", a);
                Assert.AreEqual("Foo Service", s);
                Assert.IsFalse(b);
                Assert.AreEqual(1, pp.Count());
                Assert.AreEqual(1, pp[0]);
                fooCalledCount++;
            };

            foo.GetAction("An Action With Parm").InvokeReturnObject(1);
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }


        [TestMethod]
        public void AuditUsingSpecificTypeAuditorActionWithParms() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParms", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsFalse(b);
                Assert.AreEqual(2, pp.Count());
                Assert.AreEqual(1, pp[0]);
                Assert.AreSame(foo.NakedObject.Object, pp[1]);
                fooCalledCount++;
            };

            foo.GetAction("An Action With Parms").InvokeReturnObject(1, foo.NakedObject.Object);
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorServiceActionWithParms() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestService foo = GetTestService("Foo Service");
            ITestObject fooObj = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParms", a);
                Assert.AreEqual("Foo Service", s);
                Assert.IsFalse(b);
                Assert.AreEqual(2, pp.Count());
                Assert.AreEqual(1, pp[0]);
                Assert.AreSame(fooObj.NakedObject.Object, pp[1]);
                fooCalledCount++;
            };

            foo.GetAction("An Action With Parms").InvokeReturnObject(1, fooObj);
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }


        [TestMethod]
        public void AuditUsingSpecificTypeAuditorUpdate() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int fooUpdatedCount = 0;
            int fooPersistedCount = 0;

            string newValue = Guid.NewGuid().ToString();

            FooAuditor.Auditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsNull(((Foo) o).Prop1);
                fooPersistedCount++;
            };


            foo.Save();

            FooAuditor.Auditor.objectUpdatedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.AreEqual(newValue, ((Foo) o).Prop1);
                fooUpdatedCount++;
            };

            foo.GetPropertyByName("Prop1").SetValue(newValue);
            Assert.AreEqual(1, fooUpdatedCount, "expect foo auditor to be called for updates");
            Assert.AreEqual(1, fooPersistedCount, "expect foo auditor to be called for persists");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorActionQux() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int quxCalledCound = 0;

            QuxAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.SystemTest.Audit.Qux", o.GetType().FullName);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                quxCalledCound++;
            };

            qux.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, quxCalledCound, "expect qux auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorServiceActionQux() {
            ITestService qux = GetTestService("Qux Service");
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int quxCalledCound = 0;

            QuxAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.AreEqual("Qux Service", s);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                quxCalledCound++;
            };

            qux.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, quxCalledCound, "expect qux auditor to be called");
        }


        [TestMethod]
        public void AuditUsingSpecificTypeAuditorUpdateQux() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int quxUpdatedCount = 0;
            int quxPersistedCount = 0;

            string newValue = Guid.NewGuid().ToString();

            QuxAuditor.Auditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Qux", o.GetType().FullName);
                Assert.IsNull(((Qux) o).Prop1);
                quxPersistedCount++;
            };

            qux.Save();

            QuxAuditor.Auditor.objectUpdatedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Qux", o.GetType().FullName);
                Assert.AreEqual(newValue, ((Qux) o).Prop1);
                quxUpdatedCount++;
            };


            qux.GetPropertyByName("Prop1").SetValue(newValue);
            Assert.AreEqual(1, quxUpdatedCount, "expect qux auditor to be called for updates");
            Assert.AreEqual(1, quxPersistedCount, "expect qux auditor to be called for persists");
        }

        [TestMethod]
        public void DefaultAuditorCalledForNonSpecificTypeAction() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultCalledCount = 0;

            MyDefaultAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Bar", o.GetType().FullName);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                defaultCalledCount++;
            };

            bar.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, defaultCalledCount, "expect default auditor to be called");
        }

        [TestMethod]
        public void DefaultAuditorCalledForNonSpecificTypeServiceAction() {
            ITestService bar = GetTestService("Bar Service");
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultCalledCount = 0;

            MyDefaultAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.AreEqual("Bar Service", s);
                Assert.IsFalse(b);
                Assert.AreEqual(0, pp.Count());
                defaultCalledCount++;
            };

            bar.GetAction("An Action").InvokeReturnObject();
            Assert.AreEqual(1, defaultCalledCount, "expect default auditor to be called");
        }


        [TestMethod]
        public void DefaultAuditorCalledForNonSpecificTypeUpdate() {
            MyDefaultAuditor.SetActionCallbacksExpected();
            ITestObject bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
            MyDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultUpdatedCount = 0;
            int defaultPersistedCount = 0;


            string newValue = Guid.NewGuid().ToString();

            MyDefaultAuditor.Auditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Bar", o.GetType().FullName);
                Assert.IsNull(((Bar) o).Prop1);
                defaultPersistedCount++;
            };

            bar.Save();

            MyDefaultAuditor.Auditor.objectUpdatedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Bar", o.GetType().FullName);
                Assert.AreEqual(newValue, ((Bar) o).Prop1);
                defaultUpdatedCount++;
            };

            bar.GetPropertyByName("Prop1").SetValue(newValue);
            Assert.AreEqual(1, defaultUpdatedCount, "expect default auditor to be called for updates");
            Assert.AreEqual(1, defaultPersistedCount, "expect default auditor to be called for persists");
        }
    }

    #region Classes used by tests

    public class AuditDbContext : DbContext
    {
        public const string DatabaseName = "TestAudit";
        public AuditDbContext() : base(DatabaseName) { }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<Bar> Bars { get; set; }
        public DbSet<Qux> Quxes { get; set; }
    }

    public  class Auditor  {
        public Action<IPrincipal, string, object, bool, object[]> actionInvokedCallback;
        public Action<IPrincipal, object> objectPersistedCallback;
        public Action<IPrincipal, object> objectUpdatedCallback;
        public Action<IPrincipal, string, string, bool, object[]> serviceActionInvokedCallback;

        public Auditor(string name) {
            serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback(name);
            actionInvokedCallback = TestAuditManager.UnexpectedActionCallback(name);
            objectPersistedCallback = TestAuditManager.UnexpectedCallback(name);
            objectUpdatedCallback = TestAuditManager.UnexpectedCallback(name);
        }

       
    }

    public class MyDefaultAuditor : IAuditor {

       
        public MyDefaultAuditor()  {
            Auditor.actionInvokedCallback = (p, a, o, b, pp) => { };
            Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        }

        public static void SetActionCallbacksExpected() {
            Auditor.actionInvokedCallback = (p, a, o, b, pp) => { };
            Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        }

        public static void SetActionCallbacksUnexpected() {
            Auditor.actionInvokedCallback = TestAuditManager.UnexpectedActionCallback("default");
            Auditor.serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback("default");
        }

        public static readonly Auditor Auditor = new Auditor("default");


        public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {

            Auditor.actionInvokedCallback(byPrincipal, actionName, onObject, queryOnly, withParameters);
        }

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {

            Auditor.serviceActionInvokedCallback(byPrincipal, actionName, serviceName, queryOnly, withParameters);
        }

        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectUpdatedCallback(byPrincipal, updatedObject);
        }

        public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectPersistedCallback(byPrincipal, updatedObject);
        }
    }

    public class FooAuditor : INamespaceAuditor {
        public FooAuditor(){
          
            NamespaceToAudit = typeof (Foo).FullName;
        }

        public string NamespaceToAudit { get; private set; }


        public static readonly Auditor Auditor = new Auditor("foo");


        public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {

            Auditor.actionInvokedCallback(byPrincipal, actionName, onObject, queryOnly, withParameters);
        }

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {

            Auditor.serviceActionInvokedCallback(byPrincipal, actionName, serviceName, queryOnly, withParameters);
        }

        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectUpdatedCallback(byPrincipal, updatedObject);
        }

        public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectPersistedCallback(byPrincipal, updatedObject);
        }

    }

    public class QuxAuditor :  INamespaceAuditor {
        public QuxAuditor()
            {
            NamespaceToAudit = typeof (Qux).FullName;
        }

        public string NamespaceToAudit { get; private set; }


        public static readonly Auditor Auditor = new Auditor("qux");


        public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {

            Auditor.actionInvokedCallback(byPrincipal, actionName, onObject, queryOnly, withParameters);
        }

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {

            Auditor.serviceActionInvokedCallback(byPrincipal, actionName, serviceName, queryOnly, withParameters);
        }

        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectUpdatedCallback(byPrincipal, updatedObject);
        }

        public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {

            Auditor.objectPersistedCallback(byPrincipal, updatedObject);
        }
    }

    public class Foo {
        
        public virtual int Id { get; set; }
      
        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual void AnAction() {}
        public virtual void AnActionWithParm(int aParm) {}
        public virtual void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
        public virtual IQueryable<Foo> AnotherQueryOnlyAction() { return new QueryableList<Foo>(); }
    }

    public class Bar
    {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void AnAction() { }
        public void AnActionWithParm(int aParm) { }
        public void AnActionWithParms(int parm1, Foo parm2) { }

        [QueryOnly]
        public virtual void AQueryOnlyAction() { }
    }

    public class Qux {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public void AnAction() {}
        public void AnActionWithParm(int aParm) {}
        public void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
    }

    public class FooService {
        public virtual void AnAction() {}
        public virtual void AnActionWithParm(int aParm) {}
        public virtual void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
    }

    public class BarService {
        public virtual void AnAction() {}
        public virtual void AnActionWithParm(int aParm) {}
        public virtual void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
    }

    public class QuxService {
        public virtual void AnAction() {}
        public virtual void AnActionWithParm(int aParm) {}
        public virtual void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
    }

 #endregion
}