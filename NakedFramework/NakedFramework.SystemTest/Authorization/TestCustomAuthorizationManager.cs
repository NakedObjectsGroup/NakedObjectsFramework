﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Authorization;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NUnit.Framework;
using ROSI.Exceptions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Authorization.CustomAuthorizer;

[TestFixture]
public class TestCustomAuthorizationManager : AcceptanceTestCase {
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
        services.AddScoped(p => CreatePrincipal("sven", Array.Empty<string>()));
    }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new CustomAuthorizationManagerDbContext() };

    protected Type[] ObjectTypes => new[] {
        typeof(Foo),
        typeof(Audit.Foo),
        typeof(FooSub),
        typeof(SubTypeOfFoo),
        typeof(Bar),
        typeof(Qux),
        typeof(QueryableList<Foo>)
    };

    protected Type[] Services => new[] {
        typeof(SimpleRepository<Foo>),
        typeof(SimpleRepository<FooSub>),
        typeof(SimpleRepository<SubTypeOfFoo>),
        typeof(SimpleRepository<Bar>),
        typeof(SimpleRepository<Qux>),
        typeof(FooService),
        typeof(BarService),
        typeof(QuxService)
    };

    protected IAuthorizationConfiguration AuthorizationConfiguration {
        get {
            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
            config.AddTypeAuthorizer<Foo, FooAuthorizer>();
            config.AddTypeAuthorizer<Qux, QuxAuthorizer>();
            return config;
        }
    }

    [Test]
    public void DefaultAuthorizerCalledForNonSpecificType() {
        var bar1 = GetTestService(typeof(SimpleRepository<Bar>)).GetAction("New Instance").InvokeReturnObject();
        var prop1 = bar1.GetPropertyByName("Prop1");
        prop1.AssertIsVisible();
        prop1.AssertIsModifiable();
    }

    [Test]
    public void EditabilityUsingSpecificTypeAuthorizer() {
        
        try {
            var qux = GetTestService(typeof(SimpleRepository<Qux>)).GetAction("New Instance").InvokeReturnObject();
            Assert.Fail("Should not get to here");
        }
        catch (AggregateException ae) {
            Assert.IsInstanceOf<HttpErrorRosiException>(ae.InnerException);

            Assert.AreEqual(@"199 RestfulObjects ""QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Id""", ae.InnerException.Message);
        }
    }

    [Test]
    public void SubClassIsNotPickedUpByTypeAuthorizer() {
        var fooSub = GetTestService(typeof(SimpleRepository<FooSub>)).GetAction("New Instance").InvokeReturnObject();
        var prop1 = fooSub.GetPropertyByName("Prop1");
        prop1.AssertIsVisible();
        prop1.AssertIsModifiable();
    }

    [Test]
    public void SubClassIsNotPickedUpByTypeAuthorizerWhereSubTypeNameExtendsSupertypeName() {
        var fooSub = GetTestService(typeof(SimpleRepository<SubTypeOfFoo>)).GetAction("New Instance").InvokeReturnObject();
        var prop1 = fooSub.GetPropertyByName("Prop1");
        prop1.AssertIsVisible();
        prop1.AssertIsModifiable();
    }

    [Test]
    public void VisibilityUsingSpecificTypeAuthorizer() {
        try {
            var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
            Assert.Fail("Should not get to here");
        }
        catch (AggregateException ae) {
            Assert.IsInstanceOf<HttpErrorRosiException>(ae.InnerException);

            Assert.AreEqual(@"199 RestfulObjects ""FooAuthorizer#IsVisible, user: sven, target: foo1, memberName: Id""", ae.InnerException.Message);
        }
       
    }
}

#region Classes used by tests

public class CustomAuthorizationManagerDbContext : DbContext {
    public const string DatabaseName = "TestCustomAuthorizationManager";

    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
    public CustomAuthorizationManagerDbContext() : base(Cs) { }

    public DbSet<Foo> Foos { get; set; }
    public DbSet<Bar> Bars { get; set; }
    public DbSet<Qux> Quxes { get; set; }
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

    public bool IsEditable(IPrincipal principal, object target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        return true;
    }

    public bool IsVisible(IPrincipal principal, object target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        return true;
    }

    #endregion
}

public class FooAuthorizer : ITypeAuthorizer<Foo> {
    public IDomainObjectContainer Container { protected get; set; }
    public SimpleRepository<Foo> Service { protected get; set; }

    public void Init() {
        //Does nothing
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<Foo> Members

    public bool IsEditable(IPrincipal principal, Foo target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        throw new NotImplementedException();
    }

    public bool IsVisible(IPrincipal principal, Foo target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        throw new Exception($"FooAuthorizer#IsVisible, user: {principal.Identity?.Name}, target: {target}, memberName: {memberName}");
    }

    #endregion
}

public class QuxAuthorizer : ITypeAuthorizer<Qux> {
    public IDomainObjectContainer Container { protected get; set; }
    public SimpleRepository<Foo> Service { protected get; set; }

    public void Init() {
        //Does nothing
    }

    public void Shutdown() {
        //Does nothing
    }

    #region ITypeAuthorizer<Qux> Members

    //"QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1"
    public bool IsEditable(IPrincipal principal, Qux target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        throw new Exception($"QuxAuthorizer#IsEditable, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");
    }

    public bool IsVisible(IPrincipal principal, Qux target, string memberName) {
        Assert.IsNotNull(Container);
        Assert.IsNotNull(Service);
        return true;
    }

    #endregion
}

public class Foo {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public override string ToString() => "foo1";
}

public class Bar {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public void Act1() { }
}

public class Qux {
    public virtual int Id { get; set; }

    [Optionally]
    public virtual string Prop1 { get; set; }

    public override string ToString() => "qux1";
}

public class FooSub : Foo {
    [Optionally]
    public virtual string Prop2 { get; set; }
}

public class SubTypeOfFoo : Foo {
    [Optionally]
    public virtual string Prop2 { get; set; }
}

#endregion