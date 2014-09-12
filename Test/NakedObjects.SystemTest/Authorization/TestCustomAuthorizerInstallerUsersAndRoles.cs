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
    public class TestCustomAuthoriserInstallerUsersAndRoles1 : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        //protected override IAuthorizerInstaller Authorizer {
        //    get { return new CustomAuthorizerInstaller(new MyDefaultAuthorizer(), new MyDefaultAuthorizer()); }
        //}

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
            SetUser("Fred");
        }


        [TestMethod]
        public void AccessByAuthorizedUserName() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass]
    public class TestCustomAuthoriserInstallerUsersAndRoles2 : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        //protected override IAuthorizerInstaller Authorizer {
        //    get { return new CustomAuthorizerInstaller(new MyDefaultAuthorizer(), new MyDefaultAuthorizer()); }
        //}

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
            SetUser("Anon");
        }


        [TestMethod]
        public void AccessByAnonUserWithoutRole() {
            GetTestService("Foos").GetAction("New Instance").AssertIsInvisible();
        }
    }

    [TestClass]
    public class TestCustomAuthoriserInstallerUsersAndRoles3 : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        //protected override IAuthorizerInstaller Authorizer {
        //    get { return new CustomAuthorizerInstaller(new MyDefaultAuthorizer(), new MyDefaultAuthorizer()); }
        //}

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
            SetUser("Anon", "sysAdmin");
        }


        [TestMethod]
        public void AccessByAnonUserWithRole() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }

    [TestClass]
    public class TestCustomAuthoriserInstallerUsersAndRoles4 : AbstractSystemTest {
        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        //protected override IAuthorizerInstaller Authorizer {
        //    get { return new CustomAuthorizerInstaller(new MyDefaultAuthorizer(), new MyDefaultAuthorizer()); }
        //}

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework(this);
            SetUser("Anon", "service", "sysAdmin");
        }


        [TestMethod]
        public void AccessByAnonUserWithMultipleRoles() {
            GetTestService("Foos").GetAction("New Instance").AssertIsVisible();
        }
    }


    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
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

    public class Foo {
        [Optionally]
        public virtual string Prop1 { get; set; }
    }
}