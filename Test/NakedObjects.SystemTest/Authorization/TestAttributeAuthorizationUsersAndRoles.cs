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
    public class TestAttributeAuthorizationUsersAndRoles : AbstractSystemTest {
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
            SetUser("Anon");
            prop1.AssertIsInvisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAnonForAction() {
            SetUser("Anon");
            act1.AssertIsInvisible();
        }

        [TestMethod]
        public void AccessByAuthorizedViewUser() {
            SetUser("Fred");
            prop1.AssertIsVisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAuthorizedActionUser() {
            SetUser("Fred");
            act1.AssertIsVisible();
            act1.AssertIsEnabled();
        }

        [TestMethod]
        public void AuthorizedForEditOnlyDoesNotMakePropertyVisible() {
            SetUser("Joe");
            prop1.AssertIsInvisible();
        }

        [TestMethod]
        public void AuthorizedForEditAndViewView() {
            SetUser("Bob");
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }


        [TestMethod]
        public void AccessByAnonUserWithViewRole() {
            SetUser("Anon", "sysAdmin");
            prop1.AssertIsVisible();
            prop1.AssertIsUnmodifiable();
        }

        [TestMethod]
        public void AccessByAnonUserWitActionRole() {
            SetUser("Anon", "sysAdmin");
            act1.AssertIsVisible();
            act1.AssertIsEnabled();
        }

        [TestMethod]
        public void AccessByAnonUserWithViewAndEditRole() {
            SetUser("Anon", "super");
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