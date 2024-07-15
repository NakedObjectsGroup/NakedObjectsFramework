// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Framework;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedFramework.SystemTest.Injection;

[TestFixture]
public class TestInjection : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
        NakedFramework = ServiceScope.ServiceProvider.GetService<INakedFramework>();
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
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = ObjectTypes;
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected INakedFramework NakedFramework { get; set; }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new InjectionDbContext() };

    protected Type[] ObjectTypes => new[] {
        typeof(Object1),
        typeof(Object2),
        typeof(Object3),
        typeof(Object4),
        typeof(Object5),
        typeof(Object6),
        typeof(Service1),
        typeof(IService2),
        typeof(IService3)
    };

    protected Type[] Services =>
        new[] {
            typeof(SimpleRepository<Object1>),
            typeof(SimpleRepository<Object2>),
            typeof(SimpleRepository<Object3>),
            typeof(SimpleRepository<Object4>),
            typeof(SimpleRepository<Object5>),
            typeof(SimpleRepository<Object6>),
            typeof(Service1),
            typeof(ServiceImplementation),
            typeof(Service4ImplA),
            typeof(Service4ImplB),
            typeof(Service4ImplC)
        };

    [Test]
    public void InjectArrayOfServicesDefinedByInterface() {
        var testObject = NakedFramework.Persistor.Instances<Object4>().Single();
        var arr = testObject.GetService4s();
        Assert.IsNotNull(arr);
        Assert.AreEqual(3, arr.Length);
        Assert.IsTrue(arr[0].GetType() == typeof(Service4ImplA));
        Assert.IsTrue(arr[1].GetType() == typeof(Service4ImplB));
        Assert.IsTrue(arr[2].GetType() == typeof(Service4ImplC));

        arr = testObject.GetService4ImplBs();
        Assert.IsNotNull(arr);
        Assert.AreEqual(2, arr.Length);
        Assert.IsTrue(arr[0].GetType() == typeof(Service4ImplB));
        Assert.IsTrue(arr[1].GetType() == typeof(Service4ImplC));

        arr = testObject.GetService4ImplAs();
        Assert.IsNotNull(arr);
        Assert.AreEqual(1, arr.Length);
        Assert.IsTrue(arr[0].GetType() == typeof(Service4ImplA));

        var value = testObject.GetService4ImplC();
        Assert.IsNotNull(value);
        Assert.IsTrue(value.GetType() == typeof(Service4ImplC));

        var emptyArr = testObject.GetObjects();
        Assert.IsNull(emptyArr);
    }

    [Test]
    public void InjectContainer() {
        var testObject = NakedFramework.Persistor.Instances<Object1>().Single();
        Assert.IsNotNull(testObject.Container);
        Assert.IsInstanceOf<IDomainObjectContainer>(testObject.Container);
    }

    [Test]
    public void InjectedPropertiesAreHidden() {
        var obj = NewTestObject<Object2>();
        try {
            obj.GetPropertyByName("My Service1");
            Assert.Fail();
        }
        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No Property named 'My Service1'", e.Message);
        }

        var prop = obj.GetPropertyByName("Id");
        prop.AssertIsVisible();
    }

    [Test]
    public void InjectService() {
        var testObject = NakedFramework.Persistor.Instances<Object2>().Single();
        Assert.IsNotNull(testObject.GetService1());
        Assert.IsInstanceOf<Service1>(testObject.GetService1());
    }

    [Test]
    public void InjectServiceDefinedByInterface() {
        var testObject = NakedFramework.Persistor.Instances<Object2>().Single();
        Assert.IsNotNull(testObject.GetService2());
        Assert.IsInstanceOf<ServiceImplementation>(testObject.GetService2());
        Assert.IsNotNull(testObject.GetService3());
        Assert.IsInstanceOf<ServiceImplementation>(testObject.GetService3());
        Assert.IsNull(testObject.GetObject());
    }

    [Test]
    public void RuntimeExceptionForAmbigiousInjecton() {
        try {
            var testObject = NakedFramework.Persistor.Instances<Object5>().Single();
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.AreEqual("Cannot inject service into property Service4 on target NakedFramework.SystemTest.Injection.Object5 because multiple services implement type NakedFramework.SystemTest.Injection.IService4: NakedFramework.SystemTest.Injection.Service4ImplA; NakedFramework.SystemTest.Injection.Service4ImplB; NakedFramework.SystemTest.Injection.Service4ImplC; ", e.Message);
        }
    }

    [Test]
    public void InjectLogger() {
        var testObject = NakedFramework.Persistor.Instances<Object6>().Single();
        Assert.IsNotNull(testObject.LoggerFactory);
        Assert.IsNotNull(testObject.Logger);
        Assert.IsInstanceOf<ILoggerFactory>(testObject.LoggerFactory);
        Assert.IsInstanceOf<ILogger<Object6>>(testObject.Logger);
    }
}

#region Domain classes used by tests

public class InjectionDbContext : DbContext {
    public const string DatabaseName = "TestInjection";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public InjectionDbContext() : base(Cs) { }

    public DbSet<Object1> Object1 { get; set; }
    public DbSet<Object2> Object2 { get; set; }
    public DbSet<Object3> Object3 { get; set; }
    public DbSet<Object4> Object4 { get; set; }
    public DbSet<Object5> Object5 { get; set; }
    public DbSet<Object6> Object6 { get; set; }

    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new InjectionDatabaseInitializer());

    public class InjectionDatabaseInitializer : DropCreateDatabaseAlways<InjectionDbContext> {
        protected override void Seed(InjectionDbContext context) {
            context.Object1.Add(new Object1 { Id = 1 });
            context.Object2.Add(new Object2 { Id = 1 });
            context.Object3.Add(new Object3 { Id = 1 });
            context.Object4.Add(new Object4 { Id = 1 });
            context.Object5.Add(new Object5 { Id = 1 });
            context.Object6.Add(new Object6 { Id = 1 });
        }
    }
}

public class Object1 {
    [NakedObjectsIgnore] // because public getter for test
    public IDomainObjectContainer Container { get; set; }

    public virtual int Id { get; set; }
}

public class Object2 {
    public Service1 MyService1 { protected get; set; }
    public IService2 MyService2 { protected get; set; }
    public IService3 MyService3 { protected get; set; }

    public virtual int Id { get; set; }

    //Should be ignored by injector mechanism
    public object Object { protected get; set; }

    public object GetService1() => MyService1;

    public object GetService2() => MyService2;

    public object GetService3() => MyService3;

    public object GetObject() => Object;
}

public class Object3 {
    public Service1 MyService1 { protected get; set; }
    public NotRegisteredService MyService2 { protected get; set; }

    public virtual int Id { get; set; }
}

public class Object4 {
    public IService4[] Service4s { protected get; set; }

    public Service4ImplB[] Service4ImplBs { protected get; set; }

    public Service4ImplA[] Service4ImplAs { protected get; set; }

    public Service4ImplC Service4ImplC { protected get; set; }

    //Should be ignored by injector mechanism
    public object[] Objects { protected get; set; }

    public virtual int Id { get; set; }

    public object[] GetService4s() => Service4s;

    public object[] GetService4ImplBs() => Service4ImplBs;

    public object[] GetService4ImplAs() => Service4ImplAs;

    public object GetService4ImplC() => Service4ImplC;

    public object[] GetObjects() => Objects;
}

public class Object5 {
    //Should cause runtime exception as there is more than one implementation
    public IService4 Service4 { protected get; set; }

    public virtual int Id { get; set; }
}

public class Object6 {
    [NakedObjectsIgnore] // because public getter for test
    public ILoggerFactory LoggerFactory { get; set; }

    [NakedObjectsIgnore] // because public getter for test
    public ILogger<Object6> Logger { get; set; }

    public virtual int Id { get; set; }
}

public class Service1 { }

public interface IService2 { }

public interface IService3 { }

public interface IService4 { }

public class NotRegisteredService { }

public class ServiceImplementation : IService2, IService3 { }

public class Service4ImplA : IService4 { }

public class Service4ImplB : IService4 { }

public class Service4ImplC : Service4ImplB { }

public class Service4ImplNotRegistered : IService4 { }

#endregion