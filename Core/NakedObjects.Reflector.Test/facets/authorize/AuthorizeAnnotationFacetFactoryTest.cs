// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using System.Security.Principal;
using Moq;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Reflector.DotNet.Facets.Authorize;
using NakedObjects.Security;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Hide {
    [TestFixture]
    public class AuthorizeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new AuthorizeAnnotationFacetFactory(Reflector);

            mockPersistor = new Mock<ILifecycleManager>();
            persistor = mockPersistor.Object;
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class TestSession : ISession {
            private readonly string testRole;
            private readonly string testUser;

            public TestSession(string testRole, string testUser) {
                this.testRole = testRole;
                this.testUser = testUser;
            }

            #region ISession Members

            public string UserName {
                get { return Principal.Identity.Name; }
            }

            public bool IsAuthenticated {
                get { return Principal.Identity.IsAuthenticated; }
            }

            public IPrincipal Principal {
                get { return new TestPrincipal(testRole, testUser); }
            }

            #endregion
        }

        private AuthorizeAnnotationFacetFactory facetFactory;
        private Mock<ILifecycleManager> mockPersistor;
        private ILifecycleManager persistor;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IHiddenFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        // ReSharper disable UnusedMember.Local
        private class Customer {
            public int Property1 {
                get { return 0; }
            }

            [AuthorizeProperty]
            public int Property2 {
                get { return 0; }
            }

            [AuthorizeProperty(ViewRoles = "")]
            public int Property3 {
                get { return 0; }
            }

            [AuthorizeProperty(ViewUsers = "")]
            public int Property4 {
                get { return 0; }
            }

            [AuthorizeProperty(EditRoles = "")]
            public int Property5 {
                get { return 0; }
            }

            [AuthorizeProperty(EditUsers = "")]
            public int Property6 {
                get { return 0; }
            }

            [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
            public int Property7 {
                get { return 0; }
            }


            [AuthorizeProperty(EditRoles = "aRole", EditUsers = "aUser")]
            public int Property8 {
                get { return 0; }
            }

            [AuthorizeProperty(ViewRoles = "", ViewUsers = "", EditRoles = "", EditUsers = "")]
            public int Property9 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }

            [AuthorizeAction]
            public int Action2() {
                return 0;
            }

            [AuthorizeAction(Roles = "aRole")]
            public int Action3() {
                return 0;
            }

            [AuthorizeAction(Users = "aUser")]
            public int Action4() {
                return 0;
            }


            [AuthorizeAction(Roles = "aRole", Users = "aUser")]
            public int Action5() {
                return 0;
            }
        }

        private class Customer1 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty]
        private class Customer2 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(ViewRoles = "")]
        private class Customer3 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(ViewUsers = "")]
        private class Customer4 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(EditRoles = "")]
        private class Customer5 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(EditUsers = "")]
        private class Customer6 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
        private class Customer7 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(EditRoles = "aRole", EditUsers = "aUser")]
        private class Customer8 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeProperty(ViewRoles = "", ViewUsers = "", EditRoles = "", EditUsers = "")]
        private class Customer9 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeAction]
        private class Customer10 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeAction(Roles = "aRole")]
        private class Customer11 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeAction(Users = "aUser")]
        private class Customer12 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeAction(Roles = "aRole", Users = "aUser")]
        private class Customer13 {
            public int Property1 {
                get { return 0; }
            }

            public int Action1() {
                return 0;
            }
        }

        [AuthorizeAction(Roles = "aRole", Users = "aUser")]
        [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
        private class Customer14 {
            [AuthorizeProperty(ViewRoles = "anotherRole", ViewUsers = "anotherUser")]
            public int Property1 {
                get { return 0; }
            }

            [AuthorizeAction(Roles = "anotherRole", Users = "anotherUser")]
            public int Action1() {
                return 0;
            }
        }

        // ReSharper restore UnusedMember.Local

        [Test]
        public void TestAuthorizeAnnotationActionRoleAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleAuthorizedClassPriorityOverMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer14), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));

            testSession = new TestSession("anotherRole", "");
            Assert.IsNotNull(facet.DisabledReason(testSession, null, persistor));

            facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNotNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleOnlyAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleOnlyAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationActionRoleOnlyNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionRoleOnlyNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserOnlyAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action4");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserOnlyAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer12), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserOnlyNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action4");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationActionUserOnlyNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer12), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationEditRoleAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditRoleAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditRoleNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditRoleNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditUserAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditUserAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditUserNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationEditUserNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassAll() {
            PropertyInfo property = FindProperty(typeof (Customer9), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRole() {
            PropertyInfo property = FindProperty(typeof (Customer5), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassEditUser() {
            PropertyInfo property = FindProperty(typeof (Customer6), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRole() {
            PropertyInfo property = FindProperty(typeof (Customer3), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnClassViewUser() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyAll() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property9");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRole() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property5");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property6");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRole() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property3");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }


        [Test]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property4");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeAnnotationViewRoleAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationViewRoleAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationViewRoleNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationViewRoleNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationViewUserAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationViewUserAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationViewUserAuthorizedClassOverProperty() {
            PropertyInfo property = FindProperty(typeof (Customer14), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, persistor));

            testSession = new TestSession("", "anotherUser");
            Assert.IsNotNull(facet.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeAnnotationViewUserNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, persistor));
        }

        [Test]
        public void TestAuthorizeAnnotationViewUserNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, persistor));
        }


        [Test]
        public void TestAuthorizeEmptyAnnotationOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action2");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeEmptyAnnotationOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer10), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }


        [Test]
        public void TestAuthorizeEmptyAnnotationOnClassForProperty() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeEmptyAnnotationOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property2");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeNoAnnotationOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeNoAnnotationOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action1");
            facetFactory.Process(actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeNoAnnotationOnClassForProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestAuthorizeNoAnnotationOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Property1");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }


        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Action));
            Assert.IsFalse(Contains(featureTypes, FeatureType.ActionParameter));
        }
    }

    internal class TestPrincipal : IPrincipal {
        private readonly string testRole;
        private readonly string testUser;

        public TestPrincipal(string testRole, string testUser) {
            this.testRole = testRole;
            this.testUser = testUser;
        }

        #region IPrincipal Members

        public bool IsInRole(string role) {
            return role == testRole;
        }

        public IIdentity Identity {
            get { return new TestIdentity(testUser); }
        }

        #endregion
    }

    internal class TestIdentity : IIdentity {
        private readonly string testUser;

        public TestIdentity(string testUser) {
            this.testUser = testUser;
        }

        #region IIdentity Members

        public string Name {
            get { return testUser; }
        }

        public string AuthenticationType {
            get { return ""; }
        }

        public bool IsAuthenticated {
            get { return true; }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}