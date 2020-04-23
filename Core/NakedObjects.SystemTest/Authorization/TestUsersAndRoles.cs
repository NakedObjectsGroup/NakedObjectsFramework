// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;
using NakedObjects.Services;
using NakedObjects.SystemTest.Audit;
using NUnit.Framework;

namespace NakedObjects.SystemTest.Authorization.UsersAndRoles {
    [TestFixture]
    public class TestUsersAndRoles : AbstractSystemTest<CustomAuthorizationManagerDbContext> {
        [SetUp]
        public void SetUp() {
            StartTest();
            SetUser("default");
        }

        [TearDown]
        public void TearDown() { }

        protected override Type[] Types => new[] {typeof(MyDefaultAuthorizer)};

        protected override Type[] Services {
            get {
                return new[] {
                    typeof(SimpleRepository<Foo>),
                    typeof(FooService)
                };
            }
        }

        protected override string[] Namespaces => new[] {typeof(Foo).Namespace};

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();

            services.AddSingleton<IAuthorizationConfiguration>(config);
            services.AddSingleton<IFacetDecorator, AuthorizationManager>();
        }

        [OneTimeSetUp]
        public void FixtureSetUp() {
            CustomAuthorizationManagerDbContext.Delete();
            var context = Activator.CreateInstance<CustomAuthorizationManagerDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() { }

        protected override object[] Fixtures {
            get { return new object[] { }; }
        }

        protected override object[] MenuServices {
            get {
                return new object[] {
                    new SimpleRepository<Foo>()
                };
            }
        }

        [Test] //Pending #9227
        public void SetUserOnTestIsPassedThroughToAuthorizer() {
            SetUser("svenFoo", "Bar");
            try {
                GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("User name: svenFoo, IsInRole Bar = True", e.Message);
            }

            SetUser("svenBar", "Bar");
            try {
                GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("User name: svenBar, IsInRole Bar = True", e.Message);
            }

            SetUser("svenFoo");
            try {
                GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("User name: svenFoo, IsInRole Bar = False", e.Message);
            }
        }
    }

    #region Classes used by tests

    public class CustomAuthorizationManagerDbContext : DbContext {
        public const string DatabaseName = "TestCustomAuthorizationManager";
        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
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

        public bool IsVisible(IPrincipal principal, object target, string memberName) => throw new Exception("User name: " + principal.Identity.Name + ", IsInRole Bar = " + principal.IsInRole("Bar"));

        #endregion
    }

    public class Foo {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() => "foo1";
    }

    #endregion
}