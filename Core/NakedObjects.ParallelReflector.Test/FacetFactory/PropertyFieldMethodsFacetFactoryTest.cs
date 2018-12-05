// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class PropertyFieldMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private PropertyMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof (IMandatoryFacet),
                    typeof (IPropertyAccessorFacet),
                    typeof (IPropertyValidateFacet),
                    typeof (IPropertyDefaultFacet),
                    typeof (IPropertyChoicesFacet),
                    typeof (IPropertySetterFacet),
                    typeof (INotPersistedFacet),
                    typeof (IDisabledFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new PropertyMethodsFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer {
// ReSharper disable UnusedMember.Local
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer1 {
            public string FirstName {
                get { return null; }
// ReSharper disable ValueParameterNotUsed
                set { }
            }
        }

        private class Customer10 {
            public string FirstName {
                get { return null; }
            }

            public string[] ChoicesFirstName() {
                return null;
            }
        }

// ReSharper disable InconsistentNaming
        private class Customer10r {
            public string FirstName {
                get { return null; }
            }

            [Executed(Where.Remotely)]
            public string[] ChoicesFirstName() {
                return null;
            }
        }

        private class Customer10l {
            public string FirstName {
                get { return null; }
            }

            [Executed(Where.Locally)]
            public string[] ChoicesFirstName() {
                return null;
            }
        }

        private class Customer11 {
            public string FirstName {
                get { return null; }
            }

            public string DefaultFirstName() {
                return null;
            }
        }

        private class Customer11r {
            public string FirstName {
                get { return null; }
            }

            [Executed(Where.Remotely)]
            public string DefaultFirstName() {
                return null;
            }
        }

        private class Customer11l {
            public string FirstName {
                get { return null; }
            }

            [Executed(Where.Locally)]
            public string DefaultFirstName() {
                return null;
            }
        }

        private class Customer12 {
            public string FirstName {
                get { return null; }
            }

// ReSharper disable UnusedParameter.Local
            public string ValidateFirstName(string firstName) {
                return null;
            }
        }

        private class Customer13 {
            public string FirstName {
                get { return null; }
            }

            public string SecondName {
                get { return null; }
            }

            public bool HideFirstName() {
                return false;
            }

            public bool HideSecondName() {
                return false;
            }
        }

        private class Customer14 {
            public string FirstName {
                get { return null; }
            }

            public string SecondName {
                get { return null; }
            }

            public bool HidePropertyDefault() {
                return false;
            }

            public bool HideSecondName() {
                return false;
            }
        }

        private class Customer15 {
            public string FirstName {
                get { return null; }
            }

            public string SecondName {
                get { return null; }
            }

            public string DisableFirstName(string firstName) {
                return null;
            }

            public string DisableSecondName() {
                return null;
            }
        }

        private class Customer16 {
            public string FirstName {
                get { return null; }
            }

            public string SecondName {
                get { return null; }
            }

            public string DisablePropertyDefault() {
                return null;
            }

            public string DisableSecondName() {
                return null;
            }
        }

        private class Customer17 {
            public string FirstName {
                get { return null; }
            }

            public string LastName {
                get { return null; }
            }

            public string[] ChoicesFirstName(string lastName) {
                return null;
            }
        }

        private class Customer18 {
            public string FirstName {
                get { return null; }
            }

            public string LastName {
                get { return null; }
            }

            public string[] ChoicesFirstName() {
                return null;
            }

            public string[] ChoicesFirstName(string lastName) {
                return null;
            }
        }

        private class Customer19 {
            public string FirstName {
                get { return null; }
            }

            [Executed(Ajax.Disabled)]
            public string ValidateFirstName(string firstName) {
                return null;
            }
        }

        private class Customer20 {
            public string FirstName {
                get { return null; }
            }

            [Executed(Ajax.Enabled)]
            public string ValidateFirstName(string firstName) {
                return null;
            }
        }

        private class Customer2 {
            public string FirstName {
                get { return null; }
                set { }
            }
        }

        private class Customer3 {
            public string FirstName {
                get { return null; }
                set { }
            }
        }

        private class Customer4 {
            public string FirstName {
                get { return null; }
            }

            public void ModifyFirstName(string firstName) {}
        }

        private class Customer6 {
            public string FirstName {
                get { return null; }
            }

            public void ModifyFirstName(string firstName) {}
        }

        private class Customer7 {
            public string FirstName {
                get { return null; }
                set { }
            }

            public void ModifyFirstName(string firstName) {}
        }

        private class Customer8 {
            public string FirstName {
                get { return null; }
            }

            public void ClearFirstName() {}
        }

        private class Customer9 {
            public string FirstName {
                get { return null; }
                set { }
            }
        }

        public class CustomerStatic {
            public string FirstName {
                get { return null; }
                set { }
            }

            public string LastName {
                get { return null; }
                set { }
            }

            // set required otherwise marked as DisabledFacetAlways          
            public static string NameFirstName() {
                return "Given name";
            }

            public static string DescriptionFirstName() {
                return "Some old description";
            }

            public static bool AlwaysHideFirstName() {
                return true;
            }

            public static bool ProtectFirstName() {
                return true;
            }

            public static bool HideFirstName(IPrincipal principal) {
                return true;
            }

            public static string DisableFirstName(IPrincipal principal) {
                return "disabled for this user";
            }

            // set required otherwise marked as DisabledFacetAlways    
            public static bool AlwaysHideLastName() {
                return false;
            }

            public static bool ProtectLastName() {
                return false;
            }
        }

        private class Customer21 {
            public string FirstName {
                get { return null; }
            }

            public IQueryable<string> AutoCompleteFirstName(string name) {
                return null;
            }
        }

        private class Customer22 {
            public string FirstName {
                get { return null; }
            }

            public IEnumerable<string> AutoCompleteFirstName(string name) {
                return null;
            }
        }

        private class Customer23 {
            public string FirstName {
                get { return null; }
            }

            public IQueryable<string> AutoCompleteFirstName() {
                return null;
            }
        }

        private class Customer24 {
            public string FirstName {
                get { return null; }
            }

            public IQueryable<string> AutoCompleteFirstName(int name) {
                return null;
            }
        }

        private class Customer25 {
            public string FirstName {
                get { return null; }
            }

            public IQueryable<string> AutoCompletFirstName(string name) {
                return null;
            }
        }

        private class Customer26 {
            public string FirstName {
                get { return null; }
            }

            [PageSize(33)]
            public IQueryable<string> AutoCompleteFirstName([MinLength(3)] string name) {
                return null;
            }
        }

        public interface NameInterface {
            string Name { get; set; }
        }

        private class Customer27 {
            public NameInterface FirstName {
                get { return null; }
            }

            public IQueryable<NameInterface> AutoCompleteFirstName(string name) {
                return null;
            }
        }

        [TestMethod]
        public void TestAjaxFacetAddedIfNoValidate() {
            PropertyInfo property = FindProperty(typeof (Customer2), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedDisabled() {
            PropertyInfo property = FindProperty(typeof (Customer19), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer19), "ValidateFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);
            AssertMethodRemoved(propertyValidateMethod);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedEnabled() {
            PropertyInfo property = FindProperty(typeof (Customer20), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer20), "ValidateFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);
            AssertMethodRemoved(propertyValidateMethod);
        }

        [TestMethod]
        public void TestAjaxFacetNotAddedByDefault() {
            PropertyInfo property = FindProperty(typeof (Customer12), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer12), "ValidateFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);
            AssertMethodRemoved(propertyValidateMethod);
        }

        [TestMethod]
        public void TestAutoCompleteFacetAttributes() {
            PropertyInfo property = FindProperty(typeof (Customer26), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof (Customer26), "AutoCompleteFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(33, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(3, propertyAutoCompleteFacet.MinLength);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer21), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof (Customer21), "AutoCompleteFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemovedForIEnumerableOfString() {
            PropertyInfo property = FindProperty(typeof(Customer22), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof(Customer22), "AutoCompleteFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof(IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet)facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemovedForInterface() {
            PropertyInfo property = FindProperty(typeof (Customer27), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof (Customer27), "AutoCompleteFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
        }

        [TestMethod]
        public void TestAutoCompleteFacetIgnored() {
            var property = FindProperty(typeof (Customer23), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IAutoCompleteFacet)));

            property = FindProperty(typeof (Customer24), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IAutoCompleteFacet)));

            property = FindProperty(typeof (Customer25), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (IAutoCompleteFacet)));
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer10), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof (Customer10), "ChoicesFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedDuplicate() {
            PropertyInfo property = FindProperty(typeof (Customer18), "FirstName");
            MethodInfo propertyChoicesMethod1 = FindMethod(typeof (Customer18), "ChoicesFirstName", new Type[] {});
            MethodInfo propertyChoicesMethod2 = FindMethod(typeof (Customer18), "ChoicesFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod1, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod1);
            AssertMethodNotRemoved(propertyChoicesMethod2);
            IFacet facetExecuted = Specification.GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedLocal() {
            PropertyInfo property = FindProperty(typeof (Customer10l), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof (Customer10l), "ChoicesFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyChoicesMethod), Where.Locally);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedRemote() {
            PropertyInfo property = FindProperty(typeof (Customer10r), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof (Customer10r), "ChoicesFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyChoicesMethod), Where.Remotely);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedWithParms() {
            PropertyInfo property = FindProperty(typeof (Customer17), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof (Customer17), "ChoicesFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            AssertMethodRemoved(propertyChoicesMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer11), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof (Customer11), "DefaultFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemovedLocal() {
            PropertyInfo property = FindProperty(typeof (Customer11l), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof (Customer11l), "DefaultFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyDefaultMethod), Where.Locally);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemovedRemote() {
            PropertyInfo property = FindProperty(typeof (Customer11r), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof (Customer11r), "DefaultFirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyDefaultMethod), Where.Remotely);
        }

        [TestMethod]
        public void TestDisableDefaultMethodFacet() {
            PropertyInfo property = FindProperty(typeof (Customer16), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof (Customer16), "DisablePropertyDefault", new Type[0]);
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var disableFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, disableFacet.GetMethod());
            AssertMethodNotRemoved(hideMethod);
        }

        [TestMethod]
        public void TestDisableMethodOverridsDefault() {
            PropertyInfo property = FindProperty(typeof (Customer16), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof (Customer16), "DisableSecondName", new Type[0]);
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var disableFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, disableFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
        }

        [TestMethod]
        public void TestDisableMethodWithParameterFacet() {
            PropertyInfo property = FindProperty(typeof (Customer15), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisableForContextFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestDisableMethodWithoutParameterFacet() {
            PropertyInfo property = FindProperty(typeof (Customer15), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof (Customer15), "DisableSecondName", new Type[0]);
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var propertyValidateFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestHideDefaultMethodFacet() {
            PropertyInfo property = FindProperty(typeof (Customer14), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof (Customer14), "HidePropertyDefault", new Type[0]);
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodNotRemoved(hideMethod);
        }

        [TestMethod]
        public void TestHideMethodOverridesDefault() {
            PropertyInfo property = FindProperty(typeof (Customer14), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof (Customer14), "HideSecondName", new Type [] {});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
        }

        [TestMethod]
        public void TestHideMethodWithParameterFacet() {
            PropertyInfo property = FindProperty(typeof (Customer13), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof (Customer13), "HideSecondName", new Type[]{});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
        }

        [TestMethod]
        public void TestHideMethodWithoutParameterFacet() {
            PropertyInfo property = FindProperty(typeof (Customer13), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof (Customer13), "HideFirstName", new Type[0]);
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
        }

        [TestMethod]
        public void TestIfHaveSetterAndModifyFacetThenTheModifyFacetWinsOut() {
            PropertyInfo property = FindProperty(typeof (Customer7), "FirstName");
            MethodInfo propertyModifyMethod = FindMethod(typeof (Customer7), "ModifyFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaModifyMethod);
            var propertySetterFacet = (PropertySetterFacetViaModifyMethod) facet;
            Assert.AreEqual(propertyModifyMethod, propertySetterFacet.GetMethod());
            AssertMethodRemoved(propertyModifyMethod);
        }

        [TestMethod]
        public void TestInitializationFacetIsInstalledForSetterMethodAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer2), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyInitializationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IPropertyInitializationFacet);
        }

        [TestMethod]
        public void TestInstallsDisabledForSessionFacetAndRemovesMethod() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForSessionFacetNone);
        }

        [TestMethod]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            PropertyInfo property = FindProperty(typeof (CustomerStatic), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
        }

        [TestMethod]
        public void TestModifyMethodWithNoSetterStillInstallsDisabledAndDerivedFacets() {
            PropertyInfo property = FindProperty(typeof (Customer6), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacet);
            facet = Specification.GetFacet(typeof (IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAlways);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledForModifyMethodAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer4), "FirstName");
            MethodInfo propertyModifyMethod = FindMethod(typeof (Customer4), "ModifyFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaModifyMethod);
            var propertySetterFacet = (PropertySetterFacetViaModifyMethod) facet;
            Assert.AreEqual(propertyModifyMethod, propertySetterFacet.GetMethod());
            AssertMethodRemoved(propertyModifyMethod);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledForSetterMethodAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaSetterMethod);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledMeansNoDisabledOrDerivedFacetsInstalled() {
            PropertyInfo property = FindProperty(typeof (Customer3), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            Assert.IsNull(Specification.GetFacet(typeof (INotPersistedFacet)));
            Assert.IsNull(Specification.GetFacet(typeof (IDisabledFacet)));
        }

        [TestMethod]
        public void TestValidateFacetFoundAndMethodRemoved() {
            PropertyInfo property = FindProperty(typeof (Customer12), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer12), "ValidateFirstName", new[] {typeof (string)});
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IPropertyValidateFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyValidateFacetViaMethod);
            var propertyValidateFacet = (PropertyValidateFacetViaMethod) facet;
            Assert.AreEqual(propertyValidateMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(propertyValidateMethod);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore ValueParameterNotUsed
    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedParameter.Local
}