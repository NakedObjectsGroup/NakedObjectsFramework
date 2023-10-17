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
using MyApp.MyCluster1;
using MyApp.MyCluster2;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedObjects.Security;
using NakedObjects;
using NakedObjects.Reflector.Authorization;
using NakedObjects.Reflector.Extensions;
using NakedObjects.Services;
using NakedObjects.SystemTest;
using Newtonsoft.Json;
using NotMyApp.MyCluster2;
using NUnit.Framework;
using ROSI.Exceptions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Authorization.NamespaceAuthorization {
    [TestFixture]
    public class TestNamespaceAuthorization : AcceptanceTestCase {
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

        protected Func<IConfiguration, DbContext>[] ContextCreators =>
            new Func<IConfiguration, DbContext>[] { config => new NamespaceAuthorizationDbContext() };

        protected Type[] ObjectTypes => new[] {
            typeof(Bar1),
            typeof(Bar2),
            typeof(Foo1),
            typeof(Foo2)
        };

        protected Type[] Services => new[] {
            typeof(SimpleRepository<Bar1>),
            typeof(SimpleRepository<Bar2>),
            typeof(SimpleRepository<Foo1>),
            typeof(SimpleRepository<Foo2>)
        };

        protected IAuthorizationConfiguration AuthorizationConfiguration {
            get {
                var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
                config.AddNamespaceAuthorizer<MyAppAuthorizer>("MyApp");
                config.AddNamespaceAuthorizer<MyCluster1Authorizer>("MyApp.MyCluster1");
                config.AddNamespaceAuthorizer<MyBar1Authorizer>("MyApp.MyCluster1.Bar1");
                return config;
            }
        }

        [Test]
        public void AuthorizerWithMostSpecificNamespaceIsInvokedForVisibility() {
            //Bar1
            
            try {
                var bar1 = GetTestService(typeof(SimpleRepository<Bar1>)).GetAction("New Instance").InvokeReturnObject();
             
                Assert.Fail("Should not get to here");
            }
            catch (AggregateException ae) {
                Assert.IsInstanceOf<HttpErrorRosiException>(ae.InnerException);
                Assert.AreEqual(@"199 RestfulObjects ""MyBar1Authorizer#IsVisible, user: sven, target: Bar1, memberName: Act1""", ae.InnerException.Message);
            }

            //Foo1
            
            try {
                var foo1 = GetTestService(typeof(SimpleRepository<Foo1>)).GetAction("New Instance").InvokeReturnObject();
               
                Assert.Fail("Should not get to here");
            }
            catch (AggregateException ae) {
                Assert.IsInstanceOf<HttpErrorRosiException>(ae.InnerException);
                Assert.AreEqual(@"199 RestfulObjects ""MyCluster1Authorizer#IsVisible, user: sven, target: Foo1, memberName: Act1""", ae.InnerException.Message);
            }

            //Foo2
            
            try {
                var foo2 = GetTestService(typeof(SimpleRepository<Foo2>)).GetAction("New Instance").InvokeReturnObject();
               
                Assert.Fail("Should not get to here");
            }
            catch (AggregateException ae) {
                Assert.IsInstanceOf<HttpErrorRosiException>(ae.InnerException);
                Assert.AreEqual(@"199 RestfulObjects ""MyAppAuthorizer#IsVisible, user: sven, target: Foo2, memberName: Act1""", ae.InnerException.Message);
            }

            //Bar2
            var bar2 = GetTestService(typeof(SimpleRepository<Bar2>)).GetAction("New Instance").InvokeReturnObject();
            bar2.GetPropertyByName("Prop1").AssertIsVisible();
        }
    }

    #region Classes used by tests

    public class NamespaceAuthorizationDbContext : DbContext {
        public const string DatabaseName = "TestAttributes";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;Encrypt=False;";
        public NamespaceAuthorizationDbContext() : base(Cs) { }

        public DbSet<Foo1> Foo1s { get; set; }
        public DbSet<Bar1> Bar1s { get; set; }
        public DbSet<Foo2> Foo2s { get; set; }
        public DbSet<Bar2> Bar2s { get; set; }
        public static void Delete() => Database.Delete(Cs);
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

        public bool IsVisible(IPrincipal principal, object target, string memberName) => true;

        #endregion
    }

    public class MyAppAuthorizer : INamespaceAuthorizer {
        public void Init() { }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => throw new Exception($"MyAppAuthorizer#IsEditable, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        public bool IsVisible(IPrincipal principal, object target, string memberName) => throw new Exception($"MyAppAuthorizer#IsVisible, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        #endregion
    }

    public class MyCluster1Authorizer : INamespaceAuthorizer {
        public string NamespaceToAuthorize => "MyApp.MyCluster1";

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => throw new Exception($"MyCluster1Authorizer#IsEditable, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        public bool IsVisible(IPrincipal principal, object target, string memberName) => throw new Exception($"MyCluster1Authorizer#IsVisible, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        #endregion
    }

    public class MyBar1Authorizer : INamespaceAuthorizer {
        public string NamespaceToAuthorize => "MyApp.MyCluster1.Bar1";

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => throw new Exception($"MyBar1Authorizer#IsEditable, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        public bool IsVisible(IPrincipal principal, object target, string memberName) => throw new Exception($"MyBar1Authorizer#IsVisible, user: {principal.Identity.Name}, target: {target}, memberName: {memberName}");

        #endregion
    }
}

namespace MyApp.MyCluster1 {
    public class Foo1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Foo1";

        public void Act1() { }
    }

    public class Bar1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Bar1";

        public void Act1() { }
    }
}

namespace MyApp.MyCluster2 {
    public class Foo2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Foo2";

        public void Act1() { }
    }
}

namespace NotMyApp.MyCluster2 {
    public class Bar2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "Bar2";

        public void Act1() { }
    }

    #endregion
}