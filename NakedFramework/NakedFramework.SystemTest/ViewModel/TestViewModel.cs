// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Data.Entity;
using NakedFramework.Error;
using NakedFramework.Test.Interface;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedVariable

namespace NakedObjects.Helpers.Test.ViewModel;

[TestFixture]
public class TestViewModel : AbstractSystemTest<FooContext> {
    [SetUp]
    public void Initialize() {
        StartTest();
        views = GetTestService(typeof(ViewModelService));
        foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
        foo1.GetPropertyByName("Id").SetValue("12345");
        foo1.GetPropertyByName("Name").SetValue("Foo1");
        foo1.Save();
    }

    [TearDown]
    public void Cleanup() {
        EndTest();
        foo1 = null;
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

    private ITestObject foo1;
    private ITestService views;

    protected override Type[] ObjectTypes => new[] { typeof(Foo), typeof(ViewFoo) };

    protected override Type[] Services => new[] { typeof(ViewModelService), typeof(SimpleRepository<Foo>) };

    [Test]
    public virtual void AttemptRestoreWithInvalidKey() {
        try {
            var view1 = views.GetAction("New View Foo").InvokeReturnObject("54321");
            Assert.Fail("Test should not get to this line");
        }
        catch (Exception e) {
            IsInstanceOfType(e.InnerException, typeof(DomainException));
            Assert.AreEqual("No instance with Id: 54321", e.Message);
        }
    }

    [Test]
    public virtual void ViewModelDerivesKeyFromRoot() {
        var foo1key = foo1.GetPropertyByName("Id").Title;
        var vm = foo1.GetAction("View Model").InvokeReturnObject();
        vm.AssertIsType(typeof(ViewFoo));
        vm.AssertCannotBeSaved();
        var vmkey = vm.GetPropertyByName("Key").Title;
        Assert.AreEqual(foo1key, vmkey);
    }

    [Test]
    public virtual void ViewModelRestoresRootGivenKey() {
        var foo1key = foo1.GetPropertyByName("Id").Title;
        var view1 = views.GetAction("New View Foo").InvokeReturnObject(foo1key);
        var root = view1.GetPropertyByName("Root");
        root.AssertTitleIsEqual("Foo1");
    }
}

public class FooContext : DbContext {
    public const string DatabaseName = "HelpersTest";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public FooContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public static void Delete() => Database.Delete(Cs);
}

//Persistent object on which ViewModel is based
public class Foo : IHasIntegerId {
    public IDomainObjectContainer Container { set; protected get; }

    [Title]
    public virtual string Name { get; set; }

    #region IHasIntegerId Members

    public virtual int Id { get; set; }

    #endregion

    public ViewFoo ViewModel() {
        var vm = Container.NewViewModel<ViewFoo>();
        vm.Root = this;
        return vm;
    }
}

public class ViewFoo : ViewModel<Foo> {
    //For testing only
    public string Key => DeriveKeys()[0];

    public override bool HideRoot() => false;
}

[DisplayName("Views")]
public class ViewModelService {
    public IDomainObjectContainer Container { set; protected get; }

    public ViewFoo NewViewFoo(string key) {
        var vm = Container.NewViewModel<ViewFoo>();
        vm.PopulateUsingKeys(new[] { key });
        return vm;
    }
}