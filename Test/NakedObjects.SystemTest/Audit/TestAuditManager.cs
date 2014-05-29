// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Util;
using NakedObjects.Audit;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Reflector.Audit;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Audit {
    [TestClass]
    public class TestAuditManager : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetupTest() {
            InitializeNakedObjectsFramework();
            SetUser("sven");
        }

        [TestCleanup]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
            MemoryObjectStore.DiscardObjects();
        }

        #endregion

        #region "Services & Fixtures"

        protected FooAuditor fooAuditor = new FooAuditor();
        protected MyDefaultAuditor myDefaultAuditor = new MyDefaultAuditor();
        protected QuxAuditor quxAuditor = new QuxAuditor();

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>(), new SimpleRepository<Bar>(), new SimpleRepository<Qux>(), new FooService(), new BarService(), new QuxService()}); }
        }


        protected override IAuditorInstaller Auditor {
            get { return new AuditInstaller(myDefaultAuditor, fooAuditor, quxAuditor); }
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
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
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

            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AQueryOnlyAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsTrue(b);
                Assert.AreEqual(0, pp.Count());
                fooCalledCount++;
            };

            foo.GetAction("A Query Only Action").InvokeReturnObject();
            Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
        }

        [TestMethod]
        public void AuditUsingSpecificTypeAuditorImplicitQueryOnlyAction() {
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();

            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnotherQueryOnlyAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
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

            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParm", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
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
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnActionWithParms", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
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
            ITestService foo = GetTestService("Foo Service");
            ITestObject fooObj = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooCalledCount = 0;

            fooAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int fooUpdatedCount = 0;
            int fooPersistedCount = 0;

            string newValue = Guid.NewGuid().ToString();

            fooAuditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Foo", o.GetType().FullName);
                Assert.IsNull(((Foo) o).Prop1);
                fooPersistedCount++;
            };


            foo.Save();

            fooAuditor.objectUpdatedCallback = (p, o) => {
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
            ITestObject qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int quxCalledCound = 0;

            quxAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.AreEqual("AnAction", a);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Qux", o.GetType().FullName);
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
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int quxCalledCound = 0;

            quxAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int quxUpdatedCount = 0;
            int quxPersistedCount = 0;

            string newValue = Guid.NewGuid().ToString();

            quxAuditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Qux", o.GetType().FullName);
                Assert.IsNull(((Qux) o).Prop1);
                quxPersistedCount++;
            };

            qux.Save();

            quxAuditor.objectUpdatedCallback = (p, o) => {
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
            ITestObject bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultCalledCount = 0;

            myDefaultAuditor.actionInvokedCallback = (p, a, o, b, pp) => {
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
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultCalledCount = 0;

            myDefaultAuditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
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
            ITestObject bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
            myDefaultAuditor.SetActionCallbacksUnexpected();

            int defaultUpdatedCount = 0;
            int defaultPersistedCount = 0;


            string newValue = Guid.NewGuid().ToString();

            myDefaultAuditor.objectPersistedCallback = (p, o) => {
                Assert.AreEqual("sven", p.Identity.Name);
                Assert.IsNotNull(o);
                Assert.AreEqual("NakedObjects.Proxy.NakedObjects.SystemTest.Audit.Bar", o.GetType().FullName);
                Assert.IsNull(((Bar) o).Prop1);
                defaultPersistedCount++;
            };

            bar.Save();

            myDefaultAuditor.objectUpdatedCallback = (p, o) => {
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

    public abstract class Auditor : IAuditor {
        public Action<IPrincipal, string, object, bool, object[]> actionInvokedCallback;
        public Action<IPrincipal, object> objectPersistedCallback;
        public Action<IPrincipal, object> objectUpdatedCallback;
        public Action<IPrincipal, string, string, bool, object[]> serviceActionInvokedCallback;

        protected Auditor(string name) {
            serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback(name);
            actionInvokedCallback = TestAuditManager.UnexpectedActionCallback(name);
            objectPersistedCallback = TestAuditManager.UnexpectedCallback(name);
            objectUpdatedCallback = TestAuditManager.UnexpectedCallback(name);
        }

        public IDomainObjectContainer Container { get; set; }
        public SimpleRepository<Foo> Service { get; set; }


        public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            actionInvokedCallback(byPrincipal, actionName, onObject, queryOnly, withParameters);
        }

        public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            serviceActionInvokedCallback(byPrincipal, actionName, serviceName, queryOnly, withParameters);
        }

        public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            objectUpdatedCallback(byPrincipal, updatedObject);
        }

        public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            objectPersistedCallback(byPrincipal, updatedObject);
        }
    }


    public class MyDefaultAuditor : Auditor {
        public MyDefaultAuditor() : base("default") {
            actionInvokedCallback = (p, a, o, b, pp) => { };
            serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        }

        public void SetActionCallbacksUnexpected() {
            actionInvokedCallback = TestAuditManager.UnexpectedActionCallback("default");
            serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback("default");
        }
    }


    public class FooAuditor : Auditor, INamespaceAuditor {
        public FooAuditor()
            : base("foo") {
            NamespaceToAudit = typeof (Foo).FullName;
        }

        public string NamespaceToAudit { get; private set; }
    }

    public class QuxAuditor : Auditor, INamespaceAuditor {
        public QuxAuditor()
            : base("qux") {
            NamespaceToAudit = typeof (Qux).FullName;
        }

        public string NamespaceToAudit { get; private set; }
    }

    public class Foo {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public virtual void AnAction() {}
        public virtual void AnActionWithParm(int aParm) {}
        public virtual void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
        public virtual IQueryable<Foo> AnotherQueryOnlyAction() { return new QueryableList<Foo>(); }
    }

    public class Qux {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public void AnAction() {}
        public void AnActionWithParm(int aParm) {}
        public void AnActionWithParms(int parm1, Foo parm2) {}

        [QueryOnly]
        public virtual void AQueryOnlyAction() {}
    }

    public class Bar {
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
}