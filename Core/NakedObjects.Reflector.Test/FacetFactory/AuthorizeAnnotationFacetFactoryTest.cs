// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Security;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class AuthorizeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private AuthorizeAnnotationFacetFactory facetFactory;
        private ILifecycleManager lifecycleManager;
        private Mock<ILifecycleManager> mockPersistor;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IHiddenFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public void TestAuthorizeAnnotationActionRoleAuthorized() {
            var actionMethod = FindMethod(typeof(Customer), "Action5");
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
            var actionMethod = FindMethod(typeof(Customer13), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer14), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action5");
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
            var actionMethod = FindMethod(typeof(Customer13), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action3");
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
            var actionMethod = FindMethod(typeof(Customer11), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action3");
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
            var actionMethod = FindMethod(typeof(Customer11), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action5");
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
            var actionMethod = FindMethod(typeof(Customer13), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action5");
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
            var actionMethod = FindMethod(typeof(Customer13), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action4");
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
            var actionMethod = FindMethod(typeof(Customer12), "Action1");
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
            var actionMethod = FindMethod(typeof(Customer), "Action4");
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
            var actionMethod = FindMethod(typeof(Customer12), "Action1");
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
            var property = FindProperty(typeof(Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleAuthorizedClass() {
            var property = FindProperty(typeof(Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleNotAuthorized() {
            var property = FindProperty(typeof(Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditRoleNotAuthorizedClass() {
            var property = FindProperty(typeof(Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserAuthorized() {
            var property = FindProperty(typeof(Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserAuthorizedClass() {
            var property = FindProperty(typeof(Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserNotAuthorized() {
            var property = FindProperty(typeof(Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationEditUserNotAuthorizedClass() {
            var property = FindProperty(typeof(Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IDisableForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to edit", facet.DisabledReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnAction() {
            var actionMethod = FindMethod(typeof(Customer), "Action3");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassAll() {
            var property = FindProperty(typeof(Customer9), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRole() {
            var property = FindProperty(typeof(Customer5), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditRoleUser() {
            var property = FindProperty(typeof(Customer8), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassEditUser() {
            var property = FindProperty(typeof(Customer6), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassForAction() {
            var actionMethod = FindMethod(typeof(Customer11), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRole() {
            var property = FindProperty(typeof(Customer3), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewRoleUser() {
            var property = FindProperty(typeof(Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnClassViewUser() {
            var property = FindProperty(typeof(Customer4), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyAll() {
            var property = FindProperty(typeof(Customer), "Property9");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRole() {
            var property = FindProperty(typeof(Customer), "Property5");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditRoleUser() {
            var property = FindProperty(typeof(Customer), "Property8");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyEditUser() {
            var property = FindProperty(typeof(Customer), "Property6");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRole() {
            var property = FindProperty(typeof(Customer), "Property3");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewRoleUser() {
            var property = FindProperty(typeof(Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationPickedUpOnPropertyViewUser() {
            var property = FindProperty(typeof(Customer), "Property4");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleAuthorized() {
            var property = FindProperty(typeof(Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleAuthorizedClass() {
            var property = FindProperty(typeof(Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("aRole", "");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleNotAuthorized() {
            var property = FindProperty(typeof(Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewRoleNotAuthorizedClass() {
            var property = FindProperty(typeof(Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("anotherRole", "");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorized() {
            var property = FindProperty(typeof(Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorizedClass() {
            var property = FindProperty(typeof(Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "aUser");
            Assert.IsNull(facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserAuthorizedClassOverProperty() {
            var property = FindProperty(typeof(Customer14), "Property1");
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
            var property = FindProperty(typeof(Customer), "Property7");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeAnnotationViewUserNotAuthorizedClass() {
            var property = FindProperty(typeof(Customer7), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet<IHideForSessionFacet>();
            Assert.IsNotNull(facet);

            var testSession = new TestSession("", "anotherUser");
            Assert.AreEqual("Not authorized to view", facet.HiddenReason(testSession, null, lifecycleManager, Metamodel));
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnAction() {
            var actionMethod = FindMethod(typeof(Customer), "Action2");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnClassForAction() {
            var actionMethod = FindMethod(typeof(Customer10), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnClassForProperty() {
            var property = FindProperty(typeof(Customer2), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeEmptyAnnotationOnProperty() {
            var property = FindProperty(typeof(Customer), "Property2");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnAction() {
            var actionMethod = FindMethod(typeof(Customer), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnClassForAction() {
            var actionMethod = FindMethod(typeof(Customer), "Action1");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnClassForProperty() {
            var property = FindProperty(typeof(Customer1), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestAuthorizeNoAnnotationOnProperty() {
            var property = FindProperty(typeof(Customer1), "Property1");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNull(facet);
            facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
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

            public string UserName => Principal.Identity.Name;

            public bool IsAuthenticated => Principal.Identity.IsAuthenticated;

            public IPrincipal Principal => new TestPrincipal(testRole, testUser);

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

        
        private class Customer {
            public int Property1 => 0;

            [AuthorizeProperty]
            public int Property2 => 0;

            [AuthorizeProperty(ViewRoles = "")]
            public int Property3 => 0;

            [AuthorizeProperty(ViewUsers = "")]
            public int Property4 => 0;

            [AuthorizeProperty(EditRoles = "")]
            public int Property5 => 0;

            [AuthorizeProperty(EditUsers = "")]
            public int Property6 => 0;

            [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
            public int Property7 => 0;

            [AuthorizeProperty(EditRoles = "aRole", EditUsers = "aUser")]
            public int Property8 => 0;

            [AuthorizeProperty(ViewRoles = "", ViewUsers = "", EditRoles = "", EditUsers = "")]
            public int Property9 => 0;

            public int Action1() => 0;

            [AuthorizeAction]
            public int Action2() => 0;

            [AuthorizeAction(Roles = "aRole")]
            public int Action3() => 0;

            [AuthorizeAction(Users = "aUser")]
            public int Action4() => 0;

            [AuthorizeAction(Roles = "aRole", Users = "aUser")]
            public int Action5() => 0;
        }

        private class Customer1 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty]
        private class Customer2 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(ViewRoles = "")]
        private class Customer3 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(ViewUsers = "")]
        private class Customer4 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(EditRoles = "")]
        private class Customer5 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(EditUsers = "")]
        private class Customer6 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
        private class Customer7 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(EditRoles = "aRole", EditUsers = "aUser")]
        private class Customer8 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeProperty(ViewRoles = "", ViewUsers = "", EditRoles = "", EditUsers = "")]
        private class Customer9 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeAction]
        private class Customer10 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeAction(Roles = "aRole")]
        private class Customer11 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeAction(Users = "aUser")]
        private class Customer12 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeAction(Roles = "aRole", Users = "aUser")]
        private class Customer13 {
            public int Property1 => 0;

            public int Action1() => 0;
        }

        [AuthorizeAction(Roles = "aRole", Users = "aUser")]
        [AuthorizeProperty(ViewRoles = "aRole", ViewUsers = "aUser")]
        private class Customer14 {
            [AuthorizeProperty(ViewRoles = "anotherRole", ViewUsers = "anotherUser")]
            public int Property1 => 0;

            [AuthorizeAction(Roles = "anotherRole", Users = "anotherUser")]
            public int Action1() => 0;
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

        public bool IsInRole(string role) => role == testRole;

        public IIdentity Identity => new TestIdentity(testUser);

        #endregion
    }

    internal class TestIdentity : IIdentity {
        public TestIdentity(string testUser) => Name = testUser;

        #region IIdentity Members

        public string Name { get; }

        public string AuthenticationType => "";

        public bool IsAuthenticated => true;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}