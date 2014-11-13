// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Authorization;
using NakedObjects.Security;
using NakedObjects.Services;
using NakedObjects.SystemTest.Audit;
using NakedObjects.SystemTest.Authorization.CustomAuthorizer;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Bar = NakedObjects.SystemTest.Authorization.CustomAuthorizer.Bar;
using Qux = NakedObjects.SystemTest.Authorization.CustomAuthorizer.Qux;

namespace NakedObjects.SystemTest.Authorization.Installer {
    public abstract class TestCustomAuthorizer : AbstractSystemTest<CustomAuthorizerInstallerDbContext> {
        protected override object[] MenuServices {
            get { return (new object[] {new SimpleRepository<Foo>()}); }
        }
    }

    [TestClass] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer1 : TestCustomAuthorizer {
        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new AuthorizationByTypeConfiguration {DefaultAuthorizer = typeof (DefaultAuthorizer1)};

        

            container.RegisterInstance<IAuthorizationByTypeConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IFacetDecorator, AuthorizationByTypeManager>("AuthorizationManager", new ContainerControlledLifetimeManager());

            var reflectorConfig = new ReflectorConfiguration(
                new[] {
                    typeof (DefaultAuthorizer1)
                },
                new Type[] {},
                new Type[] {},
                new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, new ContainerControlledLifetimeManager());
        }

        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer1());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        #endregion

        [TestMethod]
        public void AttemptToUseAuthorizerForAbstractType() {
            try {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e) {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass] //Use DefaultAuthorizer2
    public class TestCustomAuthorizer2 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer2());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AttemptToUseNonImplementationOfITestAuthorizer() {
            try {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e) {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass] //Use DefaultAuthorizer1
    public class TestCustomAuthorizer3 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthorizer3());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AttemptToUseITestAuthorizerOfObject() {
            try {
                InitializeNakedObjectsFramework(this);
            }
            catch (InitialisationException e) {
                Assert.AreEqual("Attempting to specify a typeAuthorizer that does not implement ITypeAuthorizer<T>, where T is concrete", e.Message);
            }
        }
    }

    [TestClass] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser4 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser4());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("Fred");
        }

        [TestCleanup]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AccessByAuthorizedUserName() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser5 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("Anon");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithoutRole() {
            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
        }
    }

    [TestClass] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser6 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser6());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("Anon", "sysAdmin");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithRole() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass] //Use DefaultAuthorizer3
    public class TestCustomAuthoriser7 : TestCustomAuthorizer {
        #region Setup/Teardown

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestCustomAuthoriser5());
            Database.Delete(CustomAuthorizerInstallerDbContext.DatabaseName);
        }

        [TestInitialize]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
            SetUser("Anon", "service", "sysAdmin");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        [TestMethod]
        public void AccessByAnonUserWithMultipleRoles() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    #region Classes used by tests

    public class CustomAuthorizerInstallerDbContext : DbContext {
        public const string DatabaseName = "TestCustomAuthorizerInstaller";
        public CustomAuthorizerInstallerDbContext() : base(DatabaseName) {}

        public DbSet<Foo> Foos { get; set; }
    }

    public class DefaultAuthorizer1 : ITypeAuthorizer<object> {
        #region ITypeAuthorizer<object> Members

        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public void Shutdown() {
            //Does nothing
        }

        #endregion
    }

    public class DefaultAuthorizer2 : ITypeAuthorizer<object> {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }

        #endregion
    }

    public class DefaultAuthorizer3 : ITypeAuthorizer<object> {
        #region ITypeAuthorizer<object> Members

        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return principal.Identity.Name == "Fred" || principal.IsInRole("sysAdmin");
        }

        public void Shutdown() {
            //Does nothing
        }

        #endregion
    }

    public class FooAbstractAuthorizer : ITypeAuthorizer<BarAbstract> {
        #region ITypeAuthorizer<BarAbstract> Members

        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, BarAbstract target, string memberName) {
            throw new NotImplementedException();
        }

        public bool IsVisible(IPrincipal principal, BarAbstract target, string memberName) {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }

        #endregion
    }

    public abstract class BarAbstract {
        public void Act1() {}
    }

    public class Foo {
        public virtual string Prop1 { get; set; }
    }

    #endregion
}