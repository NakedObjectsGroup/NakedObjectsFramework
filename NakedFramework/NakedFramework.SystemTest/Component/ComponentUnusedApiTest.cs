// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
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

namespace NakedFramework.SystemTest.Component;

[TestFixture]
public class ComponentUnusedApiTest : AcceptanceTestCase {
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
    }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new FooContext() };

    protected Type[] ObjectTypes => new[] { typeof(Foo) };

    protected Type[] Services => new[] { typeof(SimpleRepository<Foo>) };

    private INakedFramework NakedFramework { get; set; }

    [Test]
    public virtual void MetamodelManagerAllSpecs() {
        var allSpecs = NakedFramework.MetamodelManager.AllSpecs;
        Assert.AreEqual(60, allSpecs.Length);
    }

    [Test]
    public virtual void NakedObjectManagerNewAdapterForKnownObjectTest() {
        var poco = NakedFramework.Persistor.Instances<Foo>().Single();
        var adapter = NakedFramework.NakedObjectManager.NewAdapterForKnownObject(poco, null);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorReloadTest() {
        var poco = NakedFramework.Persistor.Instances<Foo>().Single();
        var adapter = NakedFramework.NakedObjectManager.GetAdapterFor(poco);
        NakedFramework.Persistor.Reload(adapter);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorResolveFieldTest() {
        var poco = NakedFramework.Persistor.Instances<Foo>().Single();
        var adapter = NakedFramework.NakedObjectManager.GetAdapterFor(poco);
        var field = (adapter.Spec as IObjectSpec)?.Properties.First();
        NakedFramework.Persistor.ResolveField(adapter, field);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorLoadFieldTest() {
        var poco = NakedFramework.Persistor.Instances<Foo>().Single();
        var adapter = NakedFramework.NakedObjectManager.GetAdapterFor(poco);
        NakedFramework.Persistor.LoadField(adapter, nameof(Foo.Name));
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectStoreIsInitialisedTest() {
        var objectStore = ServiceScope.ServiceProvider.GetService<IObjectStore>();
        Assert.IsTrue(objectStore.IsInitialized);
    }

    [Test]
    public virtual void ServicesManagerGetServiceTest() {
        var service = NakedFramework.ServicesManager.GetService("SimpleRepository-Foo");
        Assert.AreEqual(typeof(SimpleRepository<Foo>), service.Object.GetType());
    }

    [Test]
    public virtual void SessionIsAuthenticatedTest() {
        var session = NakedFramework.Session;
        Assert.IsTrue(session.IsAuthenticated);
    }

    [Test]
    public virtual void TransactionManagerTransactionLevelTest() {
        var transactionManager = NakedFramework.TransactionManager;
        transactionManager.StartTransaction();
        Assert.AreEqual(1, transactionManager.TransactionLevel);
        transactionManager.EndTransaction();
        Assert.AreEqual(0, transactionManager.TransactionLevel);
    }
}

public class FooContext : DbContext {
    public const string DatabaseName = "ComponentUnusedApiTest";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public FooContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) => Database.SetInitializer(new FooContextDatabaseInitializer());

    public class FooContextDatabaseInitializer : DropCreateDatabaseAlways<FooContext> {
        protected override void Seed(FooContext context) {
            context.Foos.Add(new Foo { Id = 12345, Name = "Foo1" });
        }
    }
}

public class Foo : IHasIntegerId {
    public IDomainObjectContainer Container { set; protected get; }

    [Title]
    public virtual string Name { get; set; }

    #region IHasIntegerId Members

    public virtual int Id { get; set; }

    #endregion
}