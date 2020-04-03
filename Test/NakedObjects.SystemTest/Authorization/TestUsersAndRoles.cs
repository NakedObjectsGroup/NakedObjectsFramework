//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Data.Entity;
//using System.Security.Principal;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NakedObjects.Architecture.Component;
//using NakedObjects.Architecture.Configuration;
//using NakedObjects.Core.Configuration;
//using NakedObjects.Meta.Authorization;
//using NakedObjects.Security;
//using NakedObjects.Services;
//using NakedObjects.SystemTest.Audit;
//using Microsoft.Practices.Unity;


//namespace NakedObjects.SystemTest.Authorization.UsersAndRoles {
//    [TestClass]
//    public class TestUsersAndRoles : AbstractSystemTest<CustomAuthorizationManagerDbContext> {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            var config = new AuthorizationConfiguration<MyDefaultAuthorizer>();

//            container.RegisterInstance<IAuthorizationConfiguration>(config, (new ContainerControlledLifetimeManager()));
//            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager", new ContainerControlledLifetimeManager());

//            var reflectorConfig = new ReflectorConfiguration(
//                new[] {
//                    typeof (MyDefaultAuthorizer),
//                },
//                new[] {
//                    typeof (SimpleRepository<Foo>),
//                    typeof (FooService),
//                },
//                new[] {typeof (Foo).Namespace});

//            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
//        }

//        #region Tests

//        [TestMethod] //Pending #9227
//        public void SetUserOnTestIsPassedThroughToAuthorizer() {
//            SetUser("svenFoo", "Bar");
//            try {
//                GetTestService(typeof (SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
//                Assert.Fail("Should not get to here");
//            }
//            catch (Exception e) {
//                Assert.AreEqual("User name: svenFoo, IsInRole Bar = True", e.Message);
//            }

//            SetUser("svenBar", "Bar");
//            try {
//                GetTestService(typeof (SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
//                Assert.Fail("Should not get to here");
//            }
//            catch (Exception e) {
//                Assert.AreEqual("User name: svenBar, IsInRole Bar = True", e.Message);
//            }

//            SetUser("svenFoo");
//            try {
//                GetTestService(typeof (SimpleRepository<Foo>)).GetAction("New Instance").AssertIsVisible();
//                Assert.Fail("Should not get to here");
//            }
//            catch (Exception e) {
//                Assert.AreEqual("User name: svenFoo, IsInRole Bar = False", e.Message);
//            }
//        }

//        #endregion

//        #region Setup/Teardown

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext tc) {
//            Database.Delete(CustomAuthorizationManagerDbContext.DatabaseName);
//            var context = Activator.CreateInstance<CustomAuthorizationManagerDbContext>();

//            context.Database.Create();
//        }

//        [ClassCleanup]
//        public static void ClassCleanup() {}

//        [TestInitialize()]
//        public void TestInitialize() {
//            InitializeNakedObjectsFramework(this);
//            StartTest();
//            SetUser("default");
//        }

//        [TestCleanup()]
//        public void TestCleanup() {}

//        #endregion

//        #region "Services & Fixtures"

//        protected override object[] Fixtures {
//            get { return (new object[] {}); }
//        }

//        protected override object[] MenuServices {
//            get {
//                return (new object[] {
//                    new SimpleRepository<Foo>()
//                });
//            }
//        }

//        #endregion
//    }

//    #region Classes used by tests

//    public class CustomAuthorizationManagerDbContext : DbContext {
//        public const string DatabaseName = "TestCustomAuthorizationManager";
//        public CustomAuthorizationManagerDbContext() : base(DatabaseName) {}

//        public DbSet<Foo> Foos { get; set; }
//    }

//    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
//        public IDomainObjectContainer Container { protected get; set; }
//        public SimpleRepository<Foo> Service { protected get; set; }

//        #region ITypeAuthorizer<object> Members

//        public bool IsEditable(IPrincipal principal, object target, string memberName) {
//            throw new NotImplementedException();
//        }

//        public bool IsVisible(IPrincipal principal, object target, string memberName) {
//            throw new Exception("User name: " + principal.Identity.Name + ", IsInRole Bar = " + principal.IsInRole("Bar"));
//        }

//        #endregion

//        public void Init() {
//            throw new NotImplementedException();
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

//    #endregion
//}