// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Security;
using NakedFramework.SystemTest.Audit;
using NakedObjects;
using NakedObjects.Reflector.Authorization;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Authorization.UsersAndRoles;

public class TestUsersAndRoles : AcceptanceTestCase {
    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new CustomAuthorizationManagerDbContext() };

    protected Type[] ObjectTypes => new[] {
        typeof(Foo),
        typeof(Audit.Foo),
        typeof(MyDefaultAuthorizer)
    };

    protected Type[] Services {
        get {
            return new[] {
                typeof(SimpleRepository<Foo>),
                typeof(FooService)
            };
        }
    }

    protected IAuthorizationConfiguration AuthorizationConfiguration => new AuthorizationConfiguration<MyDefaultAuthorizer>();

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
            frameworkOptions.AuthorizationConfiguration = AuthorizationConfiguration;
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = ObjectTypes;
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => PrincipalNamed("sven"));
    }

    protected virtual IPrincipal PrincipalNamed(string name, string[] roles = null) => CreatePrincipal(name, roles ?? new string[] { });
}

[TestFixture]
public class TestUsersAndRoles1 : TestUsersAndRoles {
    protected override IPrincipal PrincipalNamed(string name, string[] roles = null) => base.PrincipalNamed("svenFoo", new[] { "Bar" });

    [Test] //Pending #9227
    public void SetUserOnTestIsPassedThroughToAuthorizer() {
        //SetUser("svenFoo", "Bar");
        try {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No such service: Foos", e.Message);
        }
    }
}

[TestFixture]
public class TestUsersAndRoles2 : TestUsersAndRoles {
    protected override IPrincipal PrincipalNamed(string name, string[] roles = null) => base.PrincipalNamed("svenBar", new[] { "Bar" });

    [Test] //Pending #9227
    public void SetUserOnTestIsPassedThroughToAuthorizer() {
        //SetUser("svenBar", "Bar");
        try {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No such service: Foos", e.Message);
        }
    }
}

[TestFixture]
public class TestUsersAndRoles3 : TestUsersAndRoles {
    protected override IPrincipal PrincipalNamed(string name, string[] roles = null) => base.PrincipalNamed("svenFoo");

    [Test] //Pending #9227
    public void SetUserOnTestIsPassedThroughToAuthorizer() {
        //SetUser("svenFoo");
        try {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
            Assert.Fail("Should not get to here");
        }
        catch (Exception e) {
            Assert.AreEqual("Assert.Fail failed. No such service: Foos", e.Message);
        }
    }
}

[TestFixture]
public class TestUsersAndRoles4 : TestUsersAndRoles {
    //protected override IPrincipal PrincipalNamed(string name, string[] roles = null) => base.PrincipalNamed("svenFoo");

    [Test] //Pending #9227
    public void SetUserOnTestIsPassedThroughToAuthorizer() {
        GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
    }
}

#region Classes used by tests

public class CustomAuthorizationManagerDbContext : DbContext {
    public const string DatabaseName = "TestCustomAuthorizationManager";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public CustomAuthorizationManagerDbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }

    public static void Delete() => Database.Delete(Cs);
}

public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
    public IDomainObjectContainer Container { protected get; set; }
    public SimpleRepository<Foo> Service { protected get; set; }

    public void Init() {
        throw new NotImplementedException();
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<object> Members

    public bool IsEditable(IPrincipal principal, object target, string memberName) => throw new NotImplementedException();

    public bool IsVisible(IPrincipal principal, object target, string memberName) {
        if (principal.Identity.Name == "sven") {
            return true;
        }

        throw new Exception($"User name: {principal.Identity.Name}, IsInRole Bar = {principal.IsInRole("Bar")}");
    }

    #endregion
}

public class Foo {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public override string ToString() => "foo1";
}

#endregion