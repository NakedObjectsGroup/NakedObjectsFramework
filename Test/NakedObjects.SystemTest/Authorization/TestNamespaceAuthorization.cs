// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.MyCluster1;
using MyApp.MyCluster2;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;
using NakedObjects.Services;
using NotMyApp.MyCluster2;
using Unity;
using Unity.Lifetime;

namespace NakedObjects.SystemTest.Authorization.NamespaceAuthorization {
    [TestClass]
    public class TestNamespaceAuthorization : AbstractSystemTest<NamespaceAuthorizationDbContext> {
        #region Services & Fixtures

        protected override object[] MenuServices {
            get {
                return new object[] {
                    new SimpleRepository<Foo1>(),
                    new SimpleRepository<Bar1>(),
                    new SimpleRepository<Foo2>(),
                    new SimpleRepository<Bar2>()
                };
            }
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
            config.AddNamespaceAuthorizer<MyAppAuthorizer>("MyApp");
            config.AddNamespaceAuthorizer<MyCluster1Authorizer>("MyApp.MyCluster1");
            config.AddNamespaceAuthorizer<MyBar1Authorizer>("MyApp.MyCluster1.Bar1");

            container.RegisterInstance<IAuthorizationConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager", new ContainerControlledLifetimeManager());

            var reflectorConfig = new ReflectorConfiguration(
                new[] {
                    typeof (Bar1),
                    typeof (Bar2),
                    typeof (Foo1),
                    typeof (Foo2)
                },
                new[] {
                    typeof (SimpleRepository<Bar1>),
                    typeof (SimpleRepository<Bar2>),
                    typeof (SimpleRepository<Foo1>),
                    typeof (SimpleRepository<Foo2>),
                },
                new[] {typeof (Bar1).Namespace, typeof (Bar2).Namespace, typeof (Foo2).Namespace});

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
        }

        [TestMethod]
        public void AuthorizerWithMostSpecificNamespaceIsInvokedForVisibility() {
            //Bar1
            var bar1 = GetTestService(typeof (SimpleRepository<Bar1>)).GetAction("New Instance").InvokeReturnObject();
            try {
                bar1.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyBar1Authorizer#IsVisible, user: sven, target: Bar1, memberName: Prop1", e.Message);
            }

            //Foo1
            var foo1 = GetTestService(typeof (SimpleRepository<Foo1>)).GetAction("New Instance").InvokeReturnObject();
            try {
                foo1.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyCluster1Authorizer#IsVisible, user: sven, target: Foo1, memberName: Prop1", e.Message);
            }

            //Foo2
            var foo2 = GetTestService(typeof (SimpleRepository<Foo2>)).GetAction("New Instance").InvokeReturnObject();
            try {
                foo2.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyAppAuthorizer#IsVisible, user: sven, target: Foo2, memberName: Prop1", e.Message);
            }

            //Bar2
            var bar2 = GetTestService(typeof (SimpleRepository<Bar2>)).GetAction("New Instance").InvokeReturnObject();
            bar2.GetPropertyByName("Prop1").AssertIsVisible();
        }

        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc) {
            Database.Delete(NamespaceAuthorizationDbContext.DatabaseName);
            var context = Activator.CreateInstance<NamespaceAuthorizationDbContext>();

            context.Database.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestNamespaceAuthorization());
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("sven");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion
    }

    #region Classes used by tests 

    public class NamespaceAuthorizationDbContext : DbContext {
        public const string DatabaseName = "TestAttributes";
        public NamespaceAuthorizationDbContext() : base(DatabaseName) {}

        public DbSet<Foo1> Foo1s { get; set; }
        public DbSet<Bar1> Bar1s { get; set; }
        public DbSet<Foo2> Foo2s { get; set; }
        public DbSet<Bar2> Bar2s { get; set; }
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        #endregion

        public void Init() {
            //initialized = false;
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        //bool initialized = false;
    }

    public class MyAppAuthorizer : INamespaceAuthorizer {
        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyAppAuthorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyAppAuthorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {}

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }

    public class MyCluster1Authorizer : INamespaceAuthorizer {
        public string NamespaceToAuthorize {
            get { return "MyApp.MyCluster1"; }
        }

        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyCluster1Authorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyCluster1Authorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }

    public class MyBar1Authorizer : INamespaceAuthorizer {
        public string NamespaceToAuthorize {
            get { return "MyApp.MyCluster1.Bar1"; }
        }

        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyBar1Authorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyBar1Authorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }
}

namespace MyApp.MyCluster1 {
    public class Foo1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Foo1";
        }

        public void Act1() {}
    }

    public class Bar1 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Bar1";
        }

        public void Act1() {}
    }
}

namespace MyApp.MyCluster2 {
    public class Foo2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Foo2";
        }

        public void Act1() {}
    }
}

namespace NotMyApp.MyCluster2 {
    public class Bar2 {
        public virtual int Id { get; set; }

        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Bar2";
        }

        public void Act1() {}
    }

    #endregion
}