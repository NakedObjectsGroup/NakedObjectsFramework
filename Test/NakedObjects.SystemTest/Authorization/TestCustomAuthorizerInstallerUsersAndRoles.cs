// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using NakedObjects.Services;

namespace NakedObjects.SystemTest.Authorization.Installer4 {
    [TestClass]
    public class TestCustomAuthoriserInstallerUsersAndRoles : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        protected override IAuthorizerInstaller Authorizer {
            get { return new CustomAuthorizerInstaller(new MyDefaultAuthorizer(), new MyDefaultAuthorizer()); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
        }


        [TestMethod]
        public void AccessByAuthorizedUserName() {
            SetUser("Fred");
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }


        [TestMethod]
        public void AccessByAnonUserWithoutRole() {
            SetUser("Anon");
            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
        }

        [TestMethod]
        public void AccessByAnonUserWithRole() {
            SetUser("Anon", "sysAdmin");
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }

        [TestMethod]
        public void AccessByAnonUserWithMultipleRoles() {
            SetUser("Anon", "service", "sysAdmin");
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
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
    }

    public class Foo {
        [Optionally]
        public virtual string Prop1 { get; set; }
    }
}