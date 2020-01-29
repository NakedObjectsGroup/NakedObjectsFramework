// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class PropertyFieldMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private PropertyMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof(IMandatoryFacet),
                    typeof(IPropertyAccessorFacet),
                    typeof(IPropertyValidateFacet),
                    typeof(IPropertyDefaultFacet),
                    typeof(IPropertyChoicesFacet),
                    typeof(IPropertySetterFacet),
                    typeof(INotPersistedFacet),
                    typeof(IDisabledFacet)
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

            public void ModifyFirstName(string firstName) { }
        }

        private class Customer6 {
            public string FirstName {
                get { return null; }
            }

            public void ModifyFirstName(string firstName) { }
        }

        private class Customer7 {
            public string FirstName {
                get { return null; }
                set { }
            }

            public void ModifyFirstName(string firstName) { }
        }

        private class Customer8 {
            public string FirstName {
                get { return null; }
            }

            public void ClearFirstName() { }
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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer2), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedDisabled() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer19), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof(Customer19), "ValidateFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);
            AssertMethodRemoved(propertyValidateMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedEnabled() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer20), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof(Customer20), "ValidateFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAjaxFacet));
            Assert.IsNull(facet);
            AssertMethodRemoved(propertyValidateMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAjaxFacetNotAddedByDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer12), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof(Customer12), "ValidateFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAjaxFacet));
            Assert.IsNull(facet);
            AssertMethodRemoved(propertyValidateMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAutoCompleteFacetAttributes() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer26), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof(Customer26), "AutoCompleteFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(33, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(3, propertyAutoCompleteFacet.MinLength);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer21), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof(Customer21), "AutoCompleteFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemovedForIEnumerableOfString() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer22), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof(Customer22), "AutoCompleteFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAutoCompleteFacetFoundAndMethodRemovedForInterface() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer27), "FirstName");
            MethodInfo propertyAutoCompleteMethod = FindMethodIgnoreParms(typeof(Customer27), "AutoCompleteFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var propertyAutoCompleteFacet = (AutoCompleteFacet) facet;
            Assert.AreEqual(propertyAutoCompleteMethod, propertyAutoCompleteFacet.GetMethod());
            AssertMethodRemoved(propertyAutoCompleteMethod);
            Assert.AreEqual(50, propertyAutoCompleteFacet.PageSize);
            Assert.AreEqual(0, propertyAutoCompleteFacet.MinLength);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestAutoCompleteFacetIgnored() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            var property = FindProperty(typeof(Customer23), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(IAutoCompleteFacet)));

            property = FindProperty(typeof(Customer24), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(IAutoCompleteFacet)));

            property = FindProperty(typeof(Customer25), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(IAutoCompleteFacet)));
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer10), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof(Customer10), "ChoicesFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof(IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedDuplicate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer18), "FirstName");
            MethodInfo propertyChoicesMethod1 = FindMethod(typeof(Customer18), "ChoicesFirstName", new Type[] { });
            MethodInfo propertyChoicesMethod2 = FindMethod(typeof(Customer18), "ChoicesFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod1, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod1);
            AssertMethodNotRemoved(propertyChoicesMethod2);
            IFacet facetExecuted = Specification.GetFacet(typeof(IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedLocal() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer10l), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof(Customer10l), "ChoicesFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyChoicesMethod), Where.Locally);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedRemote() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer10r), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof(Customer10r), "ChoicesFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            var propertyChoicesFacet = (PropertyChoicesFacet) facet;
            Assert.AreEqual(propertyChoicesMethod, propertyChoicesFacet.GetMethod());
            AssertMethodRemoved(propertyChoicesMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyChoicesMethod), Where.Remotely);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestChoicesFacetFoundAndMethodRemovedWithParms() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer17), "FirstName");
            MethodInfo propertyChoicesMethod = FindMethod(typeof(Customer17), "ChoicesFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyChoicesFacet);
            AssertMethodRemoved(propertyChoicesMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof(IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer11), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof(Customer11), "DefaultFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            IFacet facetExecuted = Specification.GetFacet(typeof(IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemovedLocal() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer11l), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof(Customer11l), "DefaultFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyDefaultMethod), Where.Locally);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDefaultFacetFoundAndMethodRemovedRemote() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer11r), "FirstName");
            MethodInfo propertyDefaultMethod = FindMethod(typeof(Customer11r), "DefaultFirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyDefaultFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyDefaultFacetViaMethod);
            var propertyDefaultFacet = (PropertyDefaultFacetViaMethod) facet;
            Assert.AreEqual(propertyDefaultMethod, propertyDefaultFacet.GetMethod());
            AssertMethodRemoved(propertyDefaultMethod);
            var facetExecuted = Specification.GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted);
            Assert.AreEqual(facetExecuted.ExecutedWhere(propertyDefaultMethod), Where.Remotely);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisableDefaultMethodFacet() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer16), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof(Customer16), "DisablePropertyDefault", new Type[0]);
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var disableFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, disableFacet.GetMethod());
            AssertMethodNotRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisableMethodOverridsDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer16), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof(Customer16), "DisableSecondName", new Type[0]);
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var disableFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, disableFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisableMethodWithParameterFacet() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer15), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IDisableForContextFacet));
            Assert.IsNull(facet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestDisableMethodWithoutParameterFacet() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer15), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof(Customer15), "DisableSecondName", new Type[0]);
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IDisableForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            var propertyValidateFacet = (DisableForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
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
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer14), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof(Customer14), "HidePropertyDefault", new Type[0]);
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodNotRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHideMethodOverridesDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer14), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof(Customer14), "HideSecondName", new Type[] { });
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHideMethodWithParameterFacet() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer13), "SecondName");
            MethodInfo hideMethod = FindMethod(typeof(Customer13), "HideSecondName", new Type[] { });
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestHideMethodWithoutParameterFacet() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer13), "FirstName");
            MethodInfo hideMethod = FindMethod(typeof(Customer13), "HideFirstName", new Type[0]);
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IHideForContextFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            var propertyValidateFacet = (HideForContextFacet) facet;
            Assert.AreEqual(hideMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(hideMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestIfHaveSetterAndModifyFacetThenTheModifyFacetWinsOut() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer7), "FirstName");
            MethodInfo propertyModifyMethod = FindMethod(typeof(Customer7), "ModifyFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaModifyMethod);
            var propertySetterFacet = (PropertySetterFacetViaModifyMethod) facet;
            Assert.AreEqual(propertyModifyMethod, propertySetterFacet.GetMethod());
            AssertMethodRemoved(propertyModifyMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestInitializationFacetIsInstalledForSetterMethodAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer2), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyInitializationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is IPropertyInitializationFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestInstallsDisabledForSessionFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(CustomerStatic), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForSessionFacetNone);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(CustomerStatic), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestModifyMethodWithNoSetterStillInstallsDisabledAndDerivedFacets() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer6), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(INotPersistedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NotPersistedFacet);
            facet = Specification.GetFacet(typeof(IDisabledFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisabledFacetAlways);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestPropertyAccessorFacetIsInstalledAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyAccessorFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyAccessorFacet);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledForModifyMethodAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer4), "FirstName");
            MethodInfo propertyModifyMethod = FindMethod(typeof(Customer4), "ModifyFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaModifyMethod);
            var propertySetterFacet = (PropertySetterFacetViaModifyMethod) facet;
            Assert.AreEqual(propertyModifyMethod, propertySetterFacet.GetMethod());
            AssertMethodRemoved(propertyModifyMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledForSetterMethodAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer1), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertySetterFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertySetterFacetViaSetterMethod);
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestSetterFacetIsInstalledMeansNoDisabledOrDerivedFacetsInstalled() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer3), "FirstName");
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            Assert.IsNull(Specification.GetFacet(typeof(INotPersistedFacet)));
            Assert.IsNull(Specification.GetFacet(typeof(IDisabledFacet)));
            Assert.IsNotNull(metamodel);
        }

        [TestMethod]
        public void TestValidateFacetFoundAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            PropertyInfo property = FindProperty(typeof(Customer12), "FirstName");
            MethodInfo propertyValidateMethod = FindMethod(typeof(Customer12), "ValidateFirstName", new[] {typeof(string)});
            metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof(IPropertyValidateFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is PropertyValidateFacetViaMethod);
            var propertyValidateFacet = (PropertyValidateFacetViaMethod) facet;
            Assert.AreEqual(propertyValidateMethod, propertyValidateFacet.GetMethod());
            AssertMethodRemoved(propertyValidateMethod);
            Assert.IsNotNull(metamodel);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
    // ReSharper restore UnusedMember.Local
    // ReSharper restore ValueParameterNotUsed
    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedParameter.Local
}