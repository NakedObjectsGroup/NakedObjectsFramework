//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Data.Entity;
//using System.Security.Principal;
//
//using NakedObjects.Architecture.Component;
//using NakedObjects.Architecture.Configuration;
//using NakedObjects.Core.Configuration;
//using NakedObjects.Core.Util;
//using NakedObjects.Meta.Authorization;
//using NakedObjects.Security;
//using NakedObjects.Services;
//using NakedObjects.SystemTest.Audit;
//using NakedObjects.Xat;
//using Microsoft.Practices.Unity;
//using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace NakedObjects.SystemTest.Authorization.CustomAuthorizer {
//    [TestFixture]
//    public class TestCustomAuthorizationManager : AbstractSystemTest<CustomAuthorizationManagerDbContext> {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();
//            config.AddTypeAuthorizer<Foo, FooAuthorizer>();
//            config.AddTypeAuthorizer<Qux, QuxAuthorizer>();

//            container.RegisterInstance<IAuthorizationConfiguration>(config, (new ContainerControlledLifetimeManager()));
//            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager", new ContainerControlledLifetimeManager());

//            var reflectorConfig = new ReflectorConfiguration(
//                new[] {
//                    typeof (QueryableList<Foo>)
//                },
//                new[] {
//                    typeof (SimpleRepository<Foo>),
//                    typeof (SimpleRepository<FooSub>),
//                    typeof (SimpleRepository<SubTypeOfFoo>),
//                    typeof (SimpleRepository<Bar>),
//                    typeof (SimpleRepository<Qux>),
//                    typeof (FooService),
//                    typeof (BarService),
//                    typeof (QuxService)
//                },
//                new[] {typeof (Foo).Namespace});

//            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
//        }

//        #region Setup/Teardown

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext tc) {
//            Database.Delete(CustomAuthorizationManagerDbContext.DatabaseName);
//            var context = Activator.CreateInstance<CustomAuthorizationManagerDbContext>();

//            context.Database.Create();
//        }

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthorizationManager());
//        }

//        [SetUp()]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//            SetUser("sven");
//        }

//        [TearDown()]
//        public void TearDown() {}

//        #endregion

//        #region "Services & Fixtures"

//        protected override object[] Fixtures {
//            get { return (new object[] {}); }
//        }

//        protected override object[] MenuServices {
//            get {
//                return (new object[] {
//                    new SimpleRepository<Foo>(),
//                    new SimpleRepository<Bar>(),
//                    new SimpleRepository<FooSub>(),
//                    new SimpleRepository<Qux>()
//                });
//            }
//        }

//        #endregion

//        #region Tests

//        [Test]
//        public void VisibilityUsingSpecificTypeAuthorizer() {
//            ITestObject foo = GetTestService(typeof (SimpleRepository<Foo>)).GetAction("New Instance").InvokeReturnObject();
//            try {
//                foo.GetPropertyByName("Prop1").AssertIsVisible();
//                Assert.Fail("Should not get to here");
//            }
//            catch (Exception e) {
//                Assert.AreEqual("FooAuthorizer#IsVisible, user: sven, target: foo1, memberName: Prop1", e.Message);
//            }
//        }

//        [Test]
//        public void EditabilityUsingSpecificTypeAuthorizer() {
//            ITestObject qux = GetTestService(typeof (SimpleRepository<Qux>)).GetAction("New Instance").InvokeReturnObject();
//            try {
//                qux.GetPropertyByName("Prop1").AssertIsModifiable();
//                Assert.Fail("Should not get to here");
//            }
//            catch (Exception e) {
//                Assert.AreEqual("QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1", e.Message);
//            }
//        }

//        [Test]
//        public void DefaultAuthorizerCalledForNonSpecificType() {
//            ITestObject bar1 = GetTestService(typeof (SimpleRepository<Bar>)).GetAction("New Instance").InvokeReturnObject();
//            ITestProperty prop1 = bar1.GetPropertyByName("Prop1");
//            prop1.AssertIsVisible();
//            prop1.AssertIsModifiable();
//        }

//        [Test]
//        public void SubClassIsNotPickedUpByTypeAuthorizer() {
//            ITestObject fooSub = GetTestService(typeof (SimpleRepository<FooSub>)).GetAction("New Instance").InvokeReturnObject();
//            ITestProperty prop1 = fooSub.GetPropertyByName("Prop1");
//            prop1.AssertIsVisible();
//            prop1.AssertIsModifiable();
//        }

//        [Test]
//        public void SubClassIsNotPickedUpByTypeAuthorizerWhereSubTypeNameExtendsSupertypeName() {
//            ITestObject fooSub = GetTestService(typeof (SimpleRepository<SubTypeOfFoo>)).GetAction("New Instance").InvokeReturnObject();
//            ITestProperty prop1 = fooSub.GetPropertyByName("Prop1");
//            prop1.AssertIsVisible();
//            prop1.AssertIsModifiable();
//        }

//        #endregion
//    }

//    #region Classes used by tests

//    public class CustomAuthorizationManagerDbContext : DbContext {
//        public const string DatabaseName = "TestCustomAuthorizationManager";
//        public CustomAuthorizationManagerDbContext() : base(DatabaseName) {}

//        public DbSet<Foo> Foos { get; set; }
//        public DbSet<Bar> Bars { get; set; }
//        public DbSet<Qux> Quxes { get; set; }
//    }

//    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
//        public IDomainObjectContainer Container { protected get; set; }
//        public SimpleRepository<Foo> Service { protected get; set; }

//        #region ITypeAuthorizer<object> Members

//        public bool IsEditable(IPrincipal principal, object target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            return true;
//        }

//        public bool IsVisible(IPrincipal principal, object target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            return true;
//        }

//        #endregion

//        public void Init() {
//            throw new NotImplementedException();
//        }

//        public void Shutdown() {
//            //Does nothing
//        }
//    }

//    public class FooAuthorizer : ITypeAuthorizer<Foo> {
//        public IDomainObjectContainer Container { protected get; set; }
//        public SimpleRepository<Foo> Service { protected get; set; }

//        #region ITypeAuthorizer<Foo> Members

//        public bool IsEditable(IPrincipal principal, Foo target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            throw new NotImplementedException();
//        }

//        public bool IsVisible(IPrincipal principal, Foo target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            throw new Exception(String.Format("FooAuthorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target, memberName));
//        }

//        #endregion

//        public void Init() {
//            //Does nothing
//        }

//        public void Shutdown() {
//            //Does nothing
//        }
//    }

//    public class QuxAuthorizer : ITypeAuthorizer<Qux> {
//        public IDomainObjectContainer Container { protected get; set; }
//        public SimpleRepository<Foo> Service { protected get; set; }

//        #region ITypeAuthorizer<Qux> Members

//        //"QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1"
//        public bool IsEditable(IPrincipal principal, Qux target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            throw new Exception(String.Format("QuxAuthorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target, memberName));
//        }

//        public bool IsVisible(IPrincipal principal, Qux target, string memberName) {
//            Assert.IsNotNull(Container);
//            Assert.IsNotNull(Service);
//            return true;
//        }

//        #endregion

//        public void Init() {
//            //Does nothing
//        }

//        public void Shutdown() {
//            //Does nothing
//        }
//    }

//    public class Foo {
//        public virtual int Id { get; set; }

//        [Optionally]
//        public virtual string Prop1 { get; set; }

//        public override string ToString() {
//            return "foo1";
//        }
//    }

//    public class Bar {
//        public virtual int Id { get; set; }

//        [Optionally]
//        public virtual string Prop1 { get; set; }

//        public void Act1() {}
//    }

//    public class Qux {
//        public virtual int Id { get; set; }

//        [Optionally]
//        public virtual string Prop1 { get; set; }

//        public override string ToString() {
//            return "qux1";
//        }
//    }

//    public class FooSub : Foo {
//        [Optionally]
//        public virtual string Prop2 { get; set; }
//    }

//    public class SubTypeOfFoo : Foo {
//        [Optionally]
//        public virtual string Prop2 { get; set; }
//    }

//    #endregion
//}