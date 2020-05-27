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
using NakedObjects.Core.Util;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;
using NakedObjects.Services;
using NakedObjects.SystemTest.Audit;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace NakedObjects.SystemTest.Authorization.CustomAuthorizer {
    [TestFixture]
    public class TestCustomAuthorizationManager : AbstractSystemTest<CustomAuthorizationManagerDbContext> {
        protected override Type[] Types => new[] {
            typeof(QueryableList<Foo>)
        };

        protected override Type[] Services => new[] {
            typeof(SimpleRepository<Foo>),
            typeof(SimpleRepository<FooSub>),
            typeof(SimpleRepository<SubTypeOfFoo>),
            typeof(SimpleRepository<Bar>),
            typeof(SimpleRepository<Qux>),
            typeof(FooService),
            typeof(BarService),
            typeof(QuxService)
        };

        protected override string[] Namespaces => new[] {typeof(Foo).Namespace};

        [SetUp]
        public void SetUp() {
            StartTest();
            SetUser("sven");
        }

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            CustomAuthorizationManagerDbContext.Delete();
            var context = Activator.CreateInstance<CustomAuthorizationManagerDbContext>();

            context.Database.Create();
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            CustomAuthorizationManagerDbContext.Delete();
        }

        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
            config.AddTypeAuthorizer<Foo, FooAuthorizer>();
            config.AddTypeAuthorizer<Qux, QuxAuthorizer>();

            services.AddSingleton<IAuthorizationConfiguration>(config);
            services.AddSingleton<IFacetDecorator, AuthorizationManager>();
        }

        //protected override object[] Fixtures {
        //    get { return new object[] { }; }
        //}

        //protected override object[] MenuServices {
        //    get {
        //        return new object[] {
        //            new SimpleRepository<Foo>(),
        //            new SimpleRepository<Bar>(),
        //            new SimpleRepository<FooSub>(),
        //            new SimpleRepository<Qux>()
        //        };
        //    }
        //}

        [Test]
        public void DefaultAuthorizerCalledForNonSpecificType() {
            var bar1 = GetTestService(typeof(SimpleRepository<Bar>)).GetAction("New Instance").InvokeReturnObject();
            var prop1 = bar1.GetPropertyByName("Prop1");
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }

        [Test]
        public void EditabilityUsingSpecificTypeAuthorizer() {
            var qux = GetTestService(typeof(SimpleRepository<Qux>)).GetAction("New Instance").InvokeReturnObject();
            try {
                qux.GetPropertyByName("Prop1").AssertIsModifiable();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1", e.Message);
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
            var foo = GetTestService(typeof(SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
            try {
                foo.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("FooAuthorizer#IsVisible, user: sven, target: foo1, memberName: Prop1", e.Message);
            }
        }
    }

    #region Classes used by tests

    public class CustomAuthorizationManagerDbContext : DbContext {
        public const string DatabaseName = "TestCustomAuthorizationManager";

        private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
        public CustomAuthorizationManagerDbContext() : base(Cs) { }

        public DbSet<Foo> Foos { get; set; }
        public DbSet<Bar> Bars { get; set; }
        public DbSet<Qux> Quxes { get; set; }
        public static void Delete() => Database.Delete(Cs);
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
        public IDomainObjectContainer Container { protected get; set; }
        public SimpleRepository<Foo> Service { protected get; set; }

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

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class FooAuthorizer : ITypeAuthorizer<Foo> {
        public IDomainObjectContainer Container { protected get; set; }
        public SimpleRepository<Foo> Service { protected get; set; }

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

        public void Init() {
            //Does nothing
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class QuxAuthorizer : ITypeAuthorizer<Qux> {
        public IDomainObjectContainer Container { protected get; set; }
        public SimpleRepository<Foo> Service { protected get; set; }

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

        public void Init() {
            //Does nothing
        }

        public void Shutdown() {
            //Does nothing
        }
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
}