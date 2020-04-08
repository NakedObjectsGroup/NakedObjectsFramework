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
//using NakedObjects.Core;
//using NakedObjects.Core.Configuration;
//using NakedObjects.Meta.Authorization;
//using NakedObjects.Security;
//using NakedObjects.Services;
//using Microsoft.Practices.Unity;

//namespace NakedObjects.SystemTest.Authorization.Installer {
//    public abstract class TestCustomAuthorizer : AbstractSystemTest<CustomAuthorizerInstallerDbContext> {
//        protected override object[] MenuServices {
//            get { return (new object[] {new SimpleRepository<Foo>()}); }
//        }

//        protected void RegisterAuthorizerTypes<TDefault>(IUnityContainer container)
//            where TDefault : ITypeAuthorizer<object> {
//            base.RegisterTypes(container);
//            var config = new AuthorizationConfiguration<TDefault>();

//            container.RegisterInstance<IAuthorizationConfiguration>(config, (new ContainerControlledLifetimeManager()));
//            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager", new ContainerControlledLifetimeManager());

//            var reflectorConfig = new ReflectorConfiguration(
//                new[] {typeof (TDefault)},
//                new[] {typeof (SimpleRepository<Foo>)},
//                new[] {typeof (Foo).Namespace});

//            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
//        }
//    }

//    [TestFixture] //Use DefaultAuthorizer1
//    public class TestCustomAuthorizer1 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer1>(container);
//        }

//        [Test]
//        public void AttemptToUseAuthorizerForAbstractType() {
//            try {
//                InitializeNakedObjectsFramework(this);
//            }
//            catch (InitialisationException e) {
//                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
//            }
//        }

//        #region Setup/Teardown

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext tc) {
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//            var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

//            context.Database.Create();
//        }

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthorizer1());
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer2
//    public class TestCustomAuthorizer2 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer2>(container);
//        }

//        [Test]
//        public void AttemptToUseNonImplementationOfITestAuthorizer() {
//            try {
//                InitializeNakedObjectsFramework(this);
//            }
//            catch (InitialisationException e) {
//                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
//            }
//        }

//        #region Setup/Teardown

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthorizer2());
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer1
//    public class TestCustomAuthorizer3 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer1>(container);
//        }

//        [Test]
//        public void AttemptToUseITestAuthorizerOfObject() {
//            try {
//                InitializeNakedObjectsFramework(this);
//            }
//            catch (InitialisationException e) {
//                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
//            }
//        }

//        #region Setup/Teardown

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthorizer3());
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//        }

//        [SetUp]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer3
//    public class TestCustomAuthoriser4 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer3>(container);
//        }

//        [Test]
//        public void AccessByAuthorizedUserName() {
//            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
//        }

//        #region Setup/Teardown

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext tc) {
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//            var context = Activator.CreateInstance<CustomAuthorizerInstallerDbContext>();

//            context.Database.Create();
//        }

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthoriser4());
//        }

//        [SetUp]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//            SetUser("Fred");
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer3
//    public class TestCustomAuthoriser5 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer3>(container);
//        }

//        [Test]
//        public void AccessByAnonUserWithoutRole() {
//            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
//        }

//        #region Setup/Teardown

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//        }

//        [SetUp]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//            SetUser("Anon");
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer3
//    public class TestCustomAuthoriser6 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer3>(container);
//        }

//        [Test]
//        public void AccessByAnonUserWithRole() {
//            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
//        }

//        #region Setup/Teardown

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthoriser6());
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//        }

//        [SetUp]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//            SetUser("Anon", "sysAdmin");
//        }

//        #endregion
//    }

//    [TestFixture] //Use DefaultAuthorizer3
//    public class TestCustomAuthoriser7 : TestCustomAuthorizer {
//        protected override void RegisterTypes(IUnityContainer container) {
//            base.RegisterTypes(container);
//            RegisterAuthorizerTypes<DefaultAuthorizer3>(container);
//        }

//        [Test]
//        public void AccessByAnonUserWithMultipleRoles() {
//            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
//        }

//        #region Setup/Teardown

//        [ClassCleanup]
//        public static void ClassCleanup() {
//            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
//            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
//        }

//        [SetUp]
//        public void SetUp() {
//            InitializeNakedObjectsFrameworkOnce();
//            StartTest();
//            SetUser("Anon", "service", "sysAdmin");
//        }

//        #endregion
//    }

//    #region Classes used by tests

//    public class CustomAuthorizerInstallerDbContext : DbContext {
//        public const string DatabaseName = "TestCustomAuthorizerInstaller";
//        public CustomAuthorizerInstallerDbContext() : base(DatabaseName) {}

//        public DbSet<Foo> Foos { get; set; }
//    }

//    public class DefaultAuthorizer1 : ITypeAuthorizer<object> {
//        #region ITypeAuthorizer<object> Members

//        public bool IsEditable(IPrincipal principal, object target, string memberName) {
//            return true;
//        }

//        public bool IsVisible(IPrincipal principal, object target, string memberName) {
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

//    public class DefaultAuthorizer2 : ITypeAuthorizer<object> {
//        #region ITypeAuthorizer<object> Members

//        public bool IsEditable(IPrincipal principal, object target, string memberName) {
//            return true;
//        }

//        public bool IsVisible(IPrincipal principal, object target, string memberName) {
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

//    public class DefaultAuthorizer3 : ITypeAuthorizer<object> {
//        #region ITypeAuthorizer<object> Members

//        public bool IsEditable(IPrincipal principal, object target, string memberName) {
//            return true;
//        }

//        public bool IsVisible(IPrincipal principal, object target, string memberName) {
//            return principal.Identity.Name == "Fred" || principal.IsInRole("sysAdmin");
//        }

//        #endregion

//        public void Init() {
//            //Does nothing
//        }

//        public void Shutdown() {
//            //Does nothing
//        }
//    }

//    public class FooAbstractAuthorizer : ITypeAuthorizer<BarAbstract> {
//        #region ITypeAuthorizer<BarAbstract> Members

//        public bool IsEditable(IPrincipal principal, BarAbstract target, string memberName) {
//            throw new NotImplementedException();
//        }

//        public bool IsVisible(IPrincipal principal, BarAbstract target, string memberName) {
//            throw new NotImplementedException();
//        }

//        #endregion

//        public void Init() {
//            //Does nothing
//        }

//        public void Shutdown() {
//            //Does nothing
//        }
//    }

//    public abstract class BarAbstract {
//        public void Act1() {}
//    }

//    public class Foo {
//        public virtual int Id { get; set; }
//        public virtual string Prop1 { get; set; }
//    }

//    #endregion
//}