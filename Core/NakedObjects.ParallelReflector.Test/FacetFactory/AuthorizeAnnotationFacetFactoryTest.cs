// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.ParallelReflect.FacetFactory;
using NakedObjects.Security;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class AuthorizeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private AuthorizeAnnotationFacetFactory facetFactory;
        private ILifecycleManager lifecycleManager;
        private Mock<ILifecycleManager> mockPersistor;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IHiddenFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleAuthorizedClassPriorityOverMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer14), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));

            testSession = new TestSession("anotherRole", "");
            Assert.IsNotNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNotNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleOnlyAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleOnlyAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleOnlyNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleOnlyNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action5");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserOnlyAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action4");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserOnlyAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer12), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.IsNull(facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserOnlyNotAuthorized() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action4");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationActionUserOnlyNotAuthorizedClass() {
            MethodInfo actionMethod = FindMethod(typeof (Customer12), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));

            var facet1 = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            Assert.AreEqual("Not authorized to view", facet1.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action3");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassAll() {
            PropertyInfo property = FindProperty(typeof (Customer9), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRole() {
            PropertyInfo property = FindProperty(typeof (Customer5), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditUser() {
            PropertyInfo property = FindProperty(typeof (Customer6), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRole() {
            PropertyInfo property = FindProperty(typeof (Customer3), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewUser() {
            PropertyInfo property = FindProperty(typeof (Customer4), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyAll() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property9");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRole() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property5");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property6");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRole() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property3");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRoleUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewUser() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property4");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorizedClassOverProperty() {
            PropertyInfo property = FindProperty(typeof (Customer14), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));

            testSession = new TestSession("", "anotherUser");
            Assert.IsNotNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserNotAuthorized() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserNotAuthorizedClass() {
            PropertyInfo property = FindProperty(typeof (Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action2");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer10), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnClassForProperty() {
            PropertyInfo property = FindProperty(typeof (Customer2), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer), "Property2");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnClassForAction() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnClassForProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        #region Nested type: TestSession

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

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new AuthorizeAnnotationFacetFactory(0);

            mockPersistor = new Mock<ILifecycleManager>();
            lifecycleManager = mockPersistor.Object;
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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