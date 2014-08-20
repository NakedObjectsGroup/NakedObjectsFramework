// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Security;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Authorization.Installer5 {
    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles1 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Anon");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }

        [TestMethod]
        public void AccessByAnonForProperty() {
            prop1.AssertIsInvisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAnonForAction() {
            act1.AssertIsInvisible();
        }
    }

    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles2 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Fred");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }


        [TestMethod]
        public void AccessByAuthorizedViewUser() {
            prop1.AssertIsVisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAuthorizedActionUser() {
            act1.AssertIsVisible();
            act1.AssertIsEnabled();
        }
    }

    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles3 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Joe");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }


        [TestMethod]
        public void AuthorizedForEditOnlyDoesNotMakePropertyVisible() {
            prop1.AssertIsInvisible();
        }
    }

    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles4 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Bob");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }


        [TestMethod]
        public void AuthorizedForEditAndViewView() {
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }
    }

    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles5 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Anon", "sysAdmin");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }


        [TestMethod]
        public void AccessByAnonUserWithViewRole() {
            prop1.AssertIsVisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAnonUserWitActionRole() {
            act1.AssertIsVisible();
            act1.AssertIsEnabled();
        }
    }

    [TestClass]
    public class TestAttributeAuthorizationUsersAndRoles6 : AbstractSystemTest {
        private ITestAction act1;
        private ITestObject foo1;
        private ITestProperty prop1;

        #region "Services & Fixtures"

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            SetUser("Anon", "super");
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            prop1 = foo1.GetPropertyByName("Prop1");
            act1 = foo1.GetAction("Act1");
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
            prop1 = null;
            act1 = null;
        }


        [TestMethod]
        public void AccessByAnonUserWithViewAndEditRole() {
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }
    }


    public class Foo {
        [Optionally]
        [AuthorizeProperty(ViewUsers = "Fred, Bob", ViewRoles = "sysAdmin, super", EditUsers = "Joe, Bob", EditRoles = "super")]
        public virtual string Prop1 { get; set; }

        [AuthorizeAction(Users = "Fred", Roles = "sysAdmin")]
        public void Act1() {}
    }
}