// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;
using NakedFramework.Test.Interface;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NakedObjects.SystemTest.Component; 

[TestFixture]
public class ComponentUnusedApiTest : AbstractSystemTest<FooContext> {
    private ITestObject foo1;
    protected override Type[] ObjectTypes => new[] {typeof(Foo)};

    protected override Type[] Services => new[] {typeof(SimpleRepository<Foo>)};

    [SetUp]
    public void Initialize() {
        StartTest();
        foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
        foo1.GetPropertyByName("Id").SetValue("12345");
        foo1.GetPropertyByName("Name").SetValue("Foo1");
        foo1.Save();
    }

    [TearDown]
    public void Cleanup() {
        EndTest();
    }

    [OneTimeSetUp]
    public void SetupTestFixture() {
        FooContext.Delete();
        var context = Activator.CreateInstance<FooContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void TearDownTest() {
        CleanupNakedObjectsFramework(this);
        FooContext.Delete();
    }

    [Test]
    public virtual void MetamodelManagerAllSpecs() {
        var allSpecs = NakedFramework.MetamodelManager.AllSpecs;
        Assert.AreEqual(59, allSpecs.Length);
    }

    [Test]
    public virtual void NakedObjectManagerNewAdapterForKnownObjectTest() {
        var poco = foo1.GetDomainObject();
        var adapter = NakedFramework.NakedObjectManager.NewAdapterForKnownObject(poco, null);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorReloadTest() {
        var poco = foo1.GetDomainObject();
        var adapter = NakedFramework.NakedObjectManager.GetAdapterFor(poco);
        NakedFramework.Persistor.Reload(adapter);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorResolveFieldTest() {
        var poco = foo1.GetDomainObject();
        var adapter = NakedFramework.NakedObjectManager.GetAdapterFor(poco);
        var field = (adapter.Spec as IObjectSpec)?.Properties.First();
        NakedFramework.Persistor.ResolveField(adapter, field);
        Assert.AreEqual(poco, adapter.Object);
    }

    [Test]
    public virtual void ObjectPersistorLoadFieldTest() {
        var poco = foo1.GetDomainObject();
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

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public FooContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public static void Delete() => Database.Delete(Cs);
}

public class Foo : IHasIntegerId {
    public IDomainObjectContainer Container { set; protected get; }

    [Title]
    public virtual string Name { get; set; }

    #region IHasIntegerId Members

    public virtual int Id { get; set; }

    #endregion
}