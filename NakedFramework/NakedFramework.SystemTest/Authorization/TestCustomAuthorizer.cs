// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Security.Principal;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Security;
using NakedObjects.Reflector.Authorization;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.Authorization.Installer;

public abstract class TestCustomAuthorizer<TDefault> : AbstractSystemTest<CustomAuthorizerInstallerDbContext> where TDefault : ITypeAuthorizer<object> {
    protected override Type[] ObjectTypes => new[] { typeof(TDefault), typeof(Foo) };

    protected override Type[] Services => new[] { typeof(SimpleRepository<Foo>) };

    protected override IAuthorizationConfiguration AuthorizationConfiguration => new AuthorizationConfiguration<TDefault>();
}

[TestFixture] //Use DefaultAuthorizer1
public class TestCustomAuthorizer1 : TestCustomAuthorizer<DefaultAuthorizer1> {
    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeSetUp]
    public void FixtureSetUp() {
        CustomAuthorizerInstallerDbContext.Delete();
        var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

        context.Database.Create();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    [Test]
    public void AttemptToUseAuthorizerForAbstractType() {
        try {
            InitializeNakedObjectsFramework(this);
        }
        catch (InitialisationException e) {
            Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
        }
    }
}

[TestFixture] //Use DefaultAuthorizer2
public class TestCustomAuthorizer2 : TestCustomAuthorizer<DefaultAuthorizer2> {
    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AttemptToUseNonImplementationOfITestAuthorizer() {
        try {
            InitializeNakedObjectsFramework(this);
        }
        catch (InitialisationException e) {
            Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
        }
    }
}

[TestFixture] //Use DefaultAuthorizer1
public class TestCustomAuthorizer3 : TestCustomAuthorizer<DefaultAuthorizer1> {
    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AttemptToUseITestAuthorizerOfObject() {
        try {
            InitializeNakedObjectsFramework(this);
        }
        catch (InitialisationException e) {
            Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
        }
    }
}

[TestFixture] //Use DefaultAuthorizer3
public class TestCustomAuthoriser4 : TestCustomAuthorizer<DefaultAuthorizer3> {
    [SetUp]
    public void SetUp() {
        StartTest();
        SetUser("Fred");
    }

    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeSetUp]
    public void FixtureSetUp() {
        CustomAuthorizerInstallerDbContext.Delete();

        var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AccessByAuthorizedUserName() {
        GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
    }
}

[TestFixture] //Use DefaultAuthorizer3
public class TestCustomAuthoriser5 : TestCustomAuthorizer<DefaultAuthorizer3> {
    [SetUp]
    public void SetUp() {
        StartTest();
        SetUser("Anon");
    }

    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeSetUp]
    public void ClassSetUp() {
        CustomAuthorizerInstallerDbContext.Delete();

        var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AccessByAnonUserWithoutRole() {
        GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
    }
}

[TestFixture] //Use DefaultAuthorizer3
public class TestCustomAuthoriser6 : TestCustomAuthorizer<DefaultAuthorizer3> {
    [SetUp]
    public void SetUp() {
        StartTest();
        SetUser("Anon", "sysAdmin");
    }

    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeSetUp]
    public void ClassSetUp() {
        CustomAuthorizerInstallerDbContext.Delete();
        var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AccessByAnonUserWithRole() {
        GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
    }
}

[TestFixture] //Use DefaultAuthorizer3
public class TestCustomAuthoriser7 : TestCustomAuthorizer<DefaultAuthorizer3> {
    [SetUp]
    public void SetUp() {
        StartTest();
        SetUser("Anon", "service", "sysAdmin");
    }

    [TearDown]
    public void TearDown() {
        EndTest();
    }

    [OneTimeSetUp]
    public void ClassSetUp() {
        CustomAuthorizerInstallerDbContext.Delete();
        var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

        context.Database.Create();
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
        CustomAuthorizerInstallerDbContext.Delete();
    }

    [Test]
    public void AccessByAnonUserWithMultipleRoles() {
        GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
    }
}

#region Classes used by tests

public class CustomAuthorizerInstallerDbContext : DbContext {
    public const string DatabaseName = "TestCustomAuthorizerInstaller";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public CustomAuthorizerInstallerDbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }

    public static void Delete() => Database.Delete(Cs);
}

public class DefaultAuthorizer1 : ITypeAuthorizer<object> {
    public void Init() {
        //Does nothing
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<object> Members

    public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

    public bool IsVisible(IPrincipal principal, object target, string memberName) => true;

    #endregion
}

public class DefaultAuthorizer2 : ITypeAuthorizer<object> {
    public void Init() {
        throw new NotImplementedException();
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<object> Members

    public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

    public bool IsVisible(IPrincipal principal, object target, string memberName) => true;

    #endregion
}

public class DefaultAuthorizer3 : ITypeAuthorizer<object> {
    public void Init() {
        //Does nothing
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<object> Members

    public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

    public bool IsVisible(IPrincipal principal, object target, string memberName) => principal.Identity.Name == "Fred" || principal.IsInRole("sysAdmin");

    #endregion
}

public class FooAbstractAuthorizer : ITypeAuthorizer<BarAbstract> {
    public void Init() {
        //Does nothing
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<BarAbstract> Members

    public bool IsEditable(IPrincipal principal, BarAbstract target, string memberName) => throw new NotImplementedException();

    public bool IsVisible(IPrincipal principal, BarAbstract target, string memberName) => throw new NotImplementedException();

    #endregion
}

public abstract class BarAbstract {
    public void Act1() { }
}

public class Foo {
    public virtual int Id { get; set; }
    public virtual string Prop1 { get; set; }
}

#endregion