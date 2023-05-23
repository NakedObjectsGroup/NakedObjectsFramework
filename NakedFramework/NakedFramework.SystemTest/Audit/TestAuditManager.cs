// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework;
using NakedFramework.Audit;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.Persistor.EF6.Util;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects;
using NakedFramework.SystemTest;
using NakedObjects.Reflector.Audit;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace NakedFramework.SystemTest.Audit;

[TestFixture]
public class TestAuditManager : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.AddEF6Persistor(options => { options.ContextCreators = ContextCreators; });
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.AuditConfiguration = AuditConfiguration;
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = ObjectTypes;
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
    }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new AuditDbContext() };

    protected Type[] ObjectTypes => new[] {
        typeof(Foo),
        typeof(Bar),
        typeof(Qux),
        typeof(MyDefaultAuditor),
        typeof(FooAuditor),
        typeof(QuxAuditor)
    };

    protected Type[] Services => new[] {
        typeof(SimpleRepository<Foo>),
        typeof(SimpleRepository<Bar>),
        typeof(SimpleRepository<Qux>),
        typeof(FooService),
        typeof(BarService),
        typeof(QuxService)
    };

    protected IAuditConfiguration AuditConfiguration {
        get {
            var config = new AuditConfiguration<MyDefaultAuditor>();
            config.AddNamespaceAuditor<FooAuditor>(typeof(Foo).FullName);
            config.AddNamespaceAuditor<QuxAuditor>(typeof(Qux).FullName);
            return config;
        }
    }

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

    [Test]
    public void AuditUsingSpecificTypeAuditorAction() {
        MyDefaultAuditor.SetCallbacksExpected();

        var foo = NewTestObject<Foo>();

        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnAction", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetEF6ProxiedType().FullName);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            fooCalledCount++;
        };

        foo.GetAction("An Action").InvokeReturnObject();
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorActionQux() {
        MyDefaultAuditor.SetCallbacksExpected();
        var qux = NewTestObject<Qux>();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var quxCalledCound = 0;

        QuxAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnAction", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Qux", o.GetEF6ProxiedType().FullName);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            quxCalledCound++;
        };

        qux.GetAction("An Action").InvokeReturnObject();
        Assert.AreEqual(1, quxCalledCound, "expect qux auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorActionWithParm() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = NewTestObject<Foo>();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnActionWithParm", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetEF6ProxiedType().FullName);
            Assert.IsFalse(b);
            Assert.AreEqual(1, pp.Length);
            Assert.AreEqual(1, pp[0]);
            fooCalledCount++;
        };

        foo.GetAction("An Action With Parm").InvokeReturnObject(1);
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorActionWithParms() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = NewTestObject<Foo>();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnActionWithParms", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetEF6ProxiedType().FullName);
            Assert.IsFalse(b);
            Assert.AreEqual(2, pp.Length);
            Assert.AreEqual(1, pp[0]);
            //Assert.AreSame(foo.NakedObject.Object, pp[1]);
            fooCalledCount++;
        };

        foo.GetAction("An Action With Parms").InvokeReturnObject(1, foo);
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorImplicitQueryOnlyAction() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = NewTestObject<Foo>();

        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnotherQueryOnlyAction", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetEF6ProxiedType().FullName);
            Assert.IsTrue(b);
            Assert.AreEqual(0, pp.Length);
            fooCalledCount++;
        };

        foo.GetAction("Another Query Only Action").InvokeReturnCollection();
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorPersist() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooPersistedCount = 0;

        FooAuditor.Auditor.objectPersistedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity?.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetType().GetProxiedType().FullName);
            Assert.IsNull(((Foo)o).Prop1);
            fooPersistedCount++;
        };

        foo.Save();

        Assert.AreEqual(1, fooPersistedCount, "expect foo auditor to be called for persists");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorPersistQux() {
        MyDefaultAuditor.SetCallbacksExpected();
        var qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var quxPersistedCount = 0;

        QuxAuditor.Auditor.objectPersistedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity?.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Qux", o.GetType().GetProxiedType().FullName);
            Assert.IsNull(((Qux)o).Prop1);
            quxPersistedCount++;
        };

        qux.Save();

        Assert.AreEqual(1, quxPersistedCount, "expect qux auditor to be called for persists");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorQueryOnlyAction() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = NewTestObject<Foo>();

        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AQueryOnlyAction", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetEF6ProxiedType().FullName);
            Assert.IsTrue(b);
            Assert.AreEqual(0, pp.Length);
            fooCalledCount++;
        };

        foo.GetAction("A Query Only Action").InvokeReturnObject();
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorQueryOnlyServiceAction() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = GetTestService("Foo Service");

        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AQueryOnlyAction", a);
            Assert.AreEqual("Foo Service", s);
            Assert.IsTrue(b);
            Assert.AreEqual(0, pp.Length);
            fooCalledCount++;
        };

        foo.GetAction("A Query Only Action").InvokeReturnObject();
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorServiceAction() {
        var foo = GetTestService("Foo Service");

        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnAction", a);
            Assert.AreEqual("Foo Service", s);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            fooCalledCount++;
        };

        foo.GetAction("An Action").InvokeReturnObject();
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorServiceActionQux() {
        var qux = GetTestService("Qux Service");
        MyDefaultAuditor.SetCallbacksUnexpected();

        var quxCalledCound = 0;

        QuxAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnAction", a);
            Assert.AreEqual("Qux Service", s);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            quxCalledCound++;
        };

        qux.GetAction("An Action").InvokeReturnObject();
        Assert.AreEqual(1, quxCalledCound, "expect qux auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorServiceActionWithParm() {
        var foo = GetTestService("Foo Service");
        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnActionWithParm", a);
            Assert.AreEqual("Foo Service", s);
            Assert.IsFalse(b);
            Assert.AreEqual(1, pp.Length);
            Assert.AreEqual(1, pp[0]);
            fooCalledCount++;
        };

        foo.GetAction("An Action With Parm").InvokeReturnObject(1);
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorServiceActionWithParms() {
        MyDefaultAuditor.SetCallbacksExpected();
        var foo = GetTestService("Foo Service");
        var fooObj = NewTestObject<Foo>();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var fooCalledCount = 0;

        FooAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnActionWithParms", a);
            Assert.AreEqual("Foo Service", s);
            Assert.IsFalse(b);
            Assert.AreEqual(2, pp.Length);
            Assert.AreEqual(1, pp[0]);
            //Assert.AreSame(fooObj.NakedObject.Object, pp[1]);
            fooCalledCount++;
        };

        foo.GetAction("An Action With Parms").InvokeReturnObject(1, fooObj);
        Assert.AreEqual(1, fooCalledCount, "expect foo auditor to be called");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorUpdate() {
        MyDefaultAuditor.SetCallbacksExpected();
        FooAuditor.SetCallbacksExpected();
        var foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
        foo = foo.Save();
        MyDefaultAuditor.SetCallbacksUnexpected();
        FooAuditor.SetCallbacksUnexpected();

        var fooUpdatedCount = 0;

        var newValue = Guid.NewGuid().ToString();

        FooAuditor.Auditor.objectUpdatedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Foo", o.GetType().GetProxiedType().FullName);
            Assert.AreEqual(newValue, ((Foo)o).Prop1);
            fooUpdatedCount++;
        };

        //NakedFramework.TransactionManager.StartTransaction();

        foo.GetPropertyByName("Prop1").SetValue(newValue);

        //NakedFramework.TransactionManager.EndTransaction();

        Assert.AreEqual(1, fooUpdatedCount, "expect foo auditor to be called for updates");
    }

    [Test]
    public void AuditUsingSpecificTypeAuditorUpdateQux() {
        MyDefaultAuditor.SetCallbacksExpected();
        QuxAuditor.SetCallbacksExpected();
        var qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
        qux = qux.Save();
        MyDefaultAuditor.SetCallbacksUnexpected();
        QuxAuditor.SetCallbacksUnexpected();

        var quxUpdatedCount = 0;

        var newValue = Guid.NewGuid().ToString();

        QuxAuditor.Auditor.objectUpdatedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Qux", o.GetType().GetProxiedType().FullName);
            Assert.AreEqual(newValue, ((Qux)o).Prop1);
            quxUpdatedCount++;
        };

        //NakedFramework.TransactionManager.StartTransaction();

        qux.GetPropertyByName("Prop1").SetValue(newValue);

        //NakedFramework.TransactionManager.EndTransaction();
        Assert.AreEqual(1, quxUpdatedCount, "expect qux auditor to be called for updates");
    }

    [Test]
    public void DefaultAuditorCalledForNonSpecificTypeAction() {
        MyDefaultAuditor.SetCallbacksExpected();
        var bar = GetTestService(typeof(SimpleRepository<Bar>)).GetAction("New Instance").InvokeReturnObject();
        bar = bar.Save();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var defaultCalledCount = 0;

        MyDefaultAuditor.Auditor.actionInvokedCallback = (p, a, o, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("AnAction", a);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Bar", o.GetType().GetProxiedType().FullName);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            defaultCalledCount++;
        };

        bar.GetAction("An Action").InvokeReturnObject();
        Assert.AreEqual(1, defaultCalledCount, "expect default auditor to be called");
    }

    [Test]
    public void DefaultAuditorCalledForNonSpecificTypePersist() {
        MyDefaultAuditor.SetCallbacksExpected();
        var bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var defaultPersistedCount = 0;

        MyDefaultAuditor.Auditor.objectPersistedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity?.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Bar", o.GetType().GetProxiedType().FullName);
            Assert.IsNull(((Bar)o).Prop1);
            defaultPersistedCount++;
        };

        bar.Save();

        Assert.AreEqual(1, defaultPersistedCount, "expect default auditor to be called for persists");
    }

    [Test]
    public void DefaultAuditorCalledForNonSpecificTypeServiceAction() {
        var bar = GetTestService(typeof(SimpleRepository<Bar>));
        MyDefaultAuditor.SetCallbacksUnexpected();

        var defaultCalledCount = 0;

        MyDefaultAuditor.Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.AreEqual("NewInstance", a);
            Assert.AreEqual("Bars", s);
            Assert.IsFalse(b);
            Assert.AreEqual(0, pp.Length);
            defaultCalledCount++;
        };

        bar.GetAction("New Instance").InvokeReturnObject();
        Assert.AreEqual(1, defaultCalledCount, "expect default auditor to be called");
    }

    [Test]
    public void DefaultAuditorCalledForNonSpecificTypeUpdate() {
        MyDefaultAuditor.SetCallbacksExpected();
        var bar = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
        bar = bar.Save();
        MyDefaultAuditor.SetCallbacksUnexpected();

        var defaultUpdatedCount = 0;

        var newValue = Guid.NewGuid().ToString();

        MyDefaultAuditor.Auditor.objectUpdatedCallback = (p, o) => {
            Assert.AreEqual("Test", p.Identity.Name);
            Assert.IsNotNull(o);
            Assert.AreEqual("NakedFramework.SystemTest.Audit.Bar", o.GetType().GetProxiedType().FullName);
            Assert.AreEqual(newValue, ((Bar)o).Prop1);
            defaultUpdatedCount++;
        };
        //NakedFramework.TransactionManager.StartTransaction();
        bar.GetPropertyByName("Prop1").SetValue(newValue);
        //NakedFramework.TransactionManager.EndTransaction();
        Assert.AreEqual(1, defaultUpdatedCount, "expect default auditor to be called for updates");
    }
}

#region Classes used by tests

public class AuditDbContext : DbContext {
    public const string DatabaseName = "TestAudit";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public AuditDbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public DbSet<Bar> Bars { get; set; }
    public DbSet<Qux> Quxes { get; set; }

    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new AuditDatabaseInitializer());

    public class AuditDatabaseInitializer : DropCreateDatabaseAlways<AuditDbContext> {
        protected override void Seed(AuditDbContext context) {
            context.Foos.Add(new Foo { Id = 1 });
            context.Bars.Add(new Bar { Id = 1 });
            context.Quxes.Add(new Qux { Id = 1 });
        }
    }
}

public class Auditor {
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
    public static readonly Auditor Auditor = new("default");

    public string NamespaceToAudit { get; }

    public static void SetCallbacksExpected() {
        Auditor.actionInvokedCallback = (p, a, o, b, pp) => { };
        Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        Auditor.objectUpdatedCallback = (p, o) => { };
        Auditor.objectPersistedCallback = (p, o) => { };
    }

    public static void SetCallbacksUnexpected() {
        Auditor.actionInvokedCallback = TestAuditManager.UnexpectedActionCallback("default");
        Auditor.serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback("default");
        Auditor.objectUpdatedCallback = TestAuditManager.UnexpectedCallback("default");
        Auditor.objectPersistedCallback = TestAuditManager.UnexpectedCallback("default");
    }

    #region IAuditor Members

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

    #endregion
}

public class FooAuditor : IAuditor {
    public static readonly Auditor Auditor = new("foo");

    public FooAuditor() => NamespaceToAudit = typeof(Foo).FullName;

    public string NamespaceToAudit { get; }

    public static void SetCallbacksExpected() {
        Auditor.actionInvokedCallback = (p, a, o, b, pp) => { };
        Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        Auditor.objectUpdatedCallback = (p, o) => { };
        Auditor.objectPersistedCallback = (p, o) => { };
    }

    public static void SetCallbacksUnexpected() {
        Auditor.actionInvokedCallback = TestAuditManager.UnexpectedActionCallback("foo");
        Auditor.serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback("foo");
        Auditor.objectUpdatedCallback = TestAuditManager.UnexpectedCallback("foo");
        Auditor.objectPersistedCallback = TestAuditManager.UnexpectedCallback("foo");
    }

    #region IAuditor Members

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

    #endregion
}

public class QuxAuditor : IAuditor {
    public static readonly Auditor Auditor = new("qux");

    public QuxAuditor() => NamespaceToAudit = typeof(Qux).FullName;

    public string NamespaceToAudit { get; }

    public static void SetCallbacksExpected() {
        Auditor.actionInvokedCallback = (p, a, o, b, pp) => { };
        Auditor.serviceActionInvokedCallback = (p, a, s, b, pp) => { };
        Auditor.objectUpdatedCallback = (p, o) => { };
        Auditor.objectPersistedCallback = (p, o) => { };
    }

    public static void SetCallbacksUnexpected() {
        Auditor.actionInvokedCallback = TestAuditManager.UnexpectedActionCallback("qux");
        Auditor.serviceActionInvokedCallback = TestAuditManager.UnexpectedServiceActionCallback("qux");
        Auditor.objectUpdatedCallback = TestAuditManager.UnexpectedCallback("qux");
        Auditor.objectPersistedCallback = TestAuditManager.UnexpectedCallback("qux");
    }

    #region IAuditor Members

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

    #endregion
}

public class Foo {
    public IDomainObjectContainer Container { private get; set; }

    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public int DefaultId() => 1;

    public virtual void AnAction() { }
    public virtual void AnActionWithParm(int aParm) { }
    public virtual void AnActionWithParms(int parm1, Foo parm2) { }

    [QueryOnly]
    public virtual void AQueryOnlyAction() { }

    public virtual IQueryable<Foo> AnotherQueryOnlyAction() => Container.Instances<Foo>();
}

public class Bar {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public int DefaultId() => 1;

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

    public int DefaultId() => 1;

    public void AnAction() { }
    public void AnActionWithParm(int aParm) { }
    public void AnActionWithParms(int parm1, Foo parm2) { }

    [QueryOnly]
    public virtual void AQueryOnlyAction() { }
}

public class FooService {
    public virtual void AnAction() { }
    public virtual void AnActionWithParm(int aParm) { }
    public virtual void AnActionWithParms(int parm1, Foo parm2) { }

    [QueryOnly]
    public virtual void AQueryOnlyAction() { }
}

public class BarService {
    public virtual void AnAction() { }
    public virtual void AnActionWithParm(int aParm) { }
    public virtual void AnActionWithParms(int parm1, Foo parm2) { }

    [QueryOnly]
    public virtual void AQueryOnlyAction() { }
}

public class QuxService {
    public virtual void AnAction() { }
    public virtual void AnActionWithParm(int aParm) { }
    public virtual void AnActionWithParms(int parm1, Foo parm2) { }

    [QueryOnly]
    public virtual void AQueryOnlyAction() { }
}

#endregion