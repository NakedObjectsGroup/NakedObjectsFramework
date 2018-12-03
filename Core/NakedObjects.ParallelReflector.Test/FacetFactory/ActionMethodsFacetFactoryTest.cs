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
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.Test.FacetFactory {
    [TestClass]
    public class ActionMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        private ActionMethodsFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get {
                return new[] {
                    typeof (INamedFacet),
                    typeof (IExecutedFacet),
                    typeof (IActionValidationFacet),
                    typeof (IActionParameterValidationFacet),
                    typeof (IActionInvocationFacet),
                    typeof (IActionDefaultsFacet),
                    typeof (IActionChoicesFacet),
                    typeof (IDescribedAsFacet),
                    typeof (IMandatoryFacet)
                };
            }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private static IActionSpecImmutable CreateHolderWithParms() {
            var tps1 = new Mock<IObjectSpecImmutable>(); //"System.Int32"
            var tps2 = new Mock<IObjectSpecImmutable>(); //System.Int64"
            var tps3 = new Mock<IObjectSpecImmutable>(); //"System.Int64"

            var param1 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps1.Object, null);
            var param2 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps2.Object, null);
            var param3 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps3.Object, null);

            var parms = new[] {param1, param2, param3};

            var tpi = new Mock<IIdentifier>(); // ""action"

            IIdentifier id = tpi.Object;
            return ImmutableSpecFactory.CreateActionSpecImmutable(id, null, parms);
        }

        private void CheckDefaultFacet(MethodInfo defaultMethod, IActionParameterSpecImmutable parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionDefaultsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionDefaultsFacetViaMethod);
            Assert.AreEqual(defaultMethod, ((ActionDefaultsFacetViaMethod) facet).GetMethod());
            Assert.IsNotNull(((ActionDefaultsFacetViaMethod)facet).MethodDelegate);

            AssertMethodRemoved(defaultMethod);
        }

        private void CheckValidatePrameterFacet(MethodInfo method, IActionParameterSpecImmutable parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionParameterValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionParameterValidation);
            Assert.AreEqual(method, ((ActionParameterValidation) facet).GetMethod());

            AssertMethodRemoved(method);
        }

        private void CheckChoicesFacet(MethodInfo choicesMethod, IActionParameterSpecImmutable parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionChoicesFacetViaMethod);
            Assert.AreEqual(choicesMethod, ((ActionChoicesFacetViaMethod) facet).GetMethod());

            AssertMethodRemoved(choicesMethod);
        }

        private void CheckAutoCompleteFacet(MethodInfo autoCompleteMethod, IActionParameterSpecImmutable parameter, int pageSize, int minLength) {
            IFacet facet = parameter.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacet);
            var acf = (AutoCompleteFacet) facet;
            Assert.AreEqual(autoCompleteMethod, acf.GetMethod());

            AssertMethodRemoved(autoCompleteMethod);

            Assert.AreEqual(pageSize, acf.PageSize);
            Assert.AreEqual(minLength, acf.MinLength);
        }

        private void CheckAutoCompleteFacetIsNull(MethodInfo autoCompleteMethod, IActionParameterSpecImmutable parameter) {
            IFacet facet = parameter.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNull(facet);

            AssertMethodNotRemoved(autoCompleteMethod);
        }

        [TestMethod]
        public void TestActionInvocationFacetIsInstalledAndMethodRemoved() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
            Assert.IsFalse(actionInvocationFacetViaMethod.IsQueryOnly);

            AssertMethodRemoved(actionMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestActionInvocationFacetQueryableByType() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer33), "SomeQueryableAction1");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
            Assert.IsTrue(actionInvocationFacetViaMethod.IsQueryOnly);

            AssertMethodRemoved(actionMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestActionInvocationFacetQueryableByAnnotation() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer33), "SomeQueryableAction2");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
            Assert.IsTrue(actionInvocationFacetViaMethod.IsQueryOnly);

            AssertMethodRemoved(actionMethod);
            Assert.AreEqual(0, metamodel.Count);

        }

        [TestMethod]
        public void TestActionOnType() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            Type type = typeof (Customer16);
            MethodInfo actionMethod = FindMethod(type, "SomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(type), actionInvocationFacetViaMethod.OnType);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestActionReturnTypeWhenNotVoid() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer15), "SomeAction");
            //   reflector.LoadSpecification(typeof(string));
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(typeof (string)), actionInvocationFacetViaMethod.ReturnType);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestActionReturnTypeWhenVoid() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer14), "SomeAction");
            //     reflector.setLoadSpecificationClassReturn(voidNoSpec);
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(typeof (void)), actionInvocationFacetViaMethod.ReturnType);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestAddsNullableFacetToParm() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer1), "AnActionWithNullableParm");
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (INullableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NullableFacetAlways);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestAjaxFacetAddedIfNoValidate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer25), "SomeAction");
            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, facetHolderWithParms, metamodel);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedDisabled() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer24), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer24), "ValidateSomeAction", new[] {typeof (int)});
            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, facetHolderWithParms, metamodel);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacet);

            AssertMethodRemoved(propertyValidateMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestAjaxFacetFoundAndMethodRemovedEnabled() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer23), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer23), "ValidateSomeAction", new[] {typeof (int)});
            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, facetHolderWithParms, metamodel);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);

            AssertMethodRemoved(propertyValidateMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestAjaxFacetNotAddedByDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer20), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (int)});
            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, facetHolderWithParms, metamodel);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);

            AssertMethodRemoved(propertyValidateMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestDoesntAddNullableFacetToParm() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethodIgnoreParms(typeof (Customer1), "AnActionWithoutNullableParm");
            metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (INullableFacet));
            Assert.IsNull(facet);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestIgnoresParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer27), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete2SomeAction");

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckAutoCompleteFacetIsNull(autoComplete0Method, facetHolderWithParms.Parameters[0]);
            CheckAutoCompleteFacetIsNull(autoComplete1Method, facetHolderWithParms.Parameters[1]);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsDisabledForSessionFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (CustomerStatic), "SomeAction", new[] {typeof (int), typeof (long)});
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForSessionFacetNone);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (CustomerStatic), "SomeAction", new[] {typeof (int), typeof (long)});
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterAutoCompleteMethodAttrributes() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer28), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer28), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer28), "AutoComplete1SomeAction");

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 33, 2);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 66, 3);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer26), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete2SomeAction");

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethodInterface() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer32), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete2SomeAction");

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer13), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer13), "Choices0SomeAction", new Type[] {});
            MethodInfo choices1Method = FindMethod(typeof (Customer13), "Choices1SomeAction", new Type[] {});
            MethodInfo choices2Method = FindMethod(typeof (Customer13), "Choices2SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Where.Default);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodDuplicate() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer30), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method1 = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long), typeof (long)});
            MethodInfo choices0Method2 = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long)});
            MethodInfo choices0Method3 = FindMethod(typeof (Customer30), "Choices0SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckChoicesFacet(choices0Method1, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            AssertMethodNotRemoved(choices0Method2);
            AssertMethodNotRemoved(choices0Method3);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodWithParms() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer30), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long), typeof (long)});
            MethodInfo choices1Method = FindMethod(typeof (Customer30), "Choices1SomeAction", new[] {typeof (long)});
            MethodInfo choices2Method = FindMethod(typeof (Customer30), "Choices2SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Where.Default);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterChoicesMethodByNameNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer21), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer21), "ChoicesSomeAction", new[] {typeof (int)});
            MethodInfo choices1Method = FindMethod(typeof (Customer21), "ChoicesSomeAction", new[] {typeof (long)});
            MethodInfo choices2Method = FindMethod(typeof (Customer21), "Choices2SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Where.Default);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterDefaultsMethodByIndexNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer11), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo default0Method = FindMethod(typeof (Customer11), "Default0SomeAction", new Type[] {});
            MethodInfo default1Method = FindMethod(typeof (Customer11), "Default1SomeAction", new Type[] {});
            MethodInfo default2Method = FindMethod(typeof (Customer11), "Default2SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();

            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(default1Method), Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(default0Method), Where.Default);

            CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default2Method), Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default0Method), Where.Default);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterDefaultsMethodByNameNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer22), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo default0Method = FindMethod(typeof (Customer22), "DefaultSomeAction", new[] {typeof (int)});
            MethodInfo default1Method = FindMethod(typeof (Customer22), "DefaultSomeAction", new[] {typeof (long)});
            MethodInfo default2Method = FindMethod(typeof (Customer22), "Default2SomeAction", new Type[] {});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();

            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(default1Method), Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(default0Method), Where.Default);

            CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default2Method), Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default0Method), Where.Default);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterValidationMethodByIndexNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer17), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo validateParameter0Method = FindMethod(typeof (Customer17), "Validate0SomeAction", new[] {typeof (int)});
            MethodInfo validateParameter1Method = FindMethod(typeof (Customer17), "Validate1SomeAction", new[] {typeof (long)});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();

            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckValidatePrameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckValidatePrameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNull(facetExecuted1);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsParameterValidationMethodByNameNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer20), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo validateParameter0Method = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (int)});
            MethodInfo validateParameter1Method = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (long)});

            IActionSpecImmutable facetHolderWithParms = CreateHolderWithParms();

            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

            CheckValidatePrameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckValidatePrameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNull(facetExecuted1);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsValidateMethodNoArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer8), "SomeAction");
            MethodInfo validateMethod = FindMethod(typeof (Customer8), "ValidateSomeAction");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionValidationFacet);
            var actionValidationFacetViaMethod = (ActionValidationFacet) facet;
            Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
            AssertMethodRemoved(validateMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestInstallsValidateMethodSomeArgsFacetAndRemovesMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer9), "SomeAction", new[] {typeof (int), typeof (int)});
            MethodInfo validateMethod = FindMethod(typeof (Customer9), "ValidateSomeAction", new[] {typeof (int), typeof (int)});
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (IActionValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionValidationFacet);
            var actionValidationFacetViaMethod = (ActionValidationFacet) facet;
            Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
            AssertMethodRemoved(validateMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDefaultDisableMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer18), "SomeActionThree");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer18), "DisableActionDefault");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
            AssertMethodNotRemoved(disableMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDefaultHideMethod() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer19), "SomeActionThree");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer19), "HideActionDefault");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
            AssertMethodNotRemoved(disableMethod);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDisableMethodDifferentSignature() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer12), "SomeActionThree");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer12), "DisableSomeActionThree");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDisableMethodNoParms() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer12), "SomeActionOne");
            MethodInfo hideMethod = FindMethod(typeof (Customer12), "DisableSomeActionOne");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDisableMethodOverriddingDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer18), "SomeActionTwo");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer18), "DisableSomeActionTwo");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpDisableMethodSameSignature() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer12), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer12), "DisableSomeActionTwo");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpHideMethodDifferentSignature() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer10), "SomeActionThree");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer10), "HideSomeActionThree");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpHideMethodNoParms() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethod(typeof (Customer10), "SomeActionOne");
            MethodInfo hideMethod = FindMethod(typeof (Customer10), "HideSomeActionOne");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpHideMethodOverriddingDefault() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer19), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer19), "HideSomeActionTwo");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestPickUpHideMethodSameSignature() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer10), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer10), "HideSomeActionTwo");
            metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

            var facet = Specification.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacet);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestProvidesDefaultNameForActionButIgnoresAnyNamedAnnotation() {
            IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            MethodInfo method = FindMethod(typeof (Customer1), "AnActionWithNamedAnnotation");
            metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is INamedFacet);
            var namedFacet = (INamedFacet) facet;
            Assert.AreEqual("An Action With Named Annotation", namedFacet.NaturalName);
            Assert.AreEqual(0, metamodel.Count);
        }

        [TestMethod]
        public void TestFindActions() {
            ReflectorConfiguration.NoValidate = true;

            var config = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new[] {typeof (Customer34).Namespace});

            var classStrategy = new DefaultClassStrategy(config);

            var methods = typeof (Customer34).GetMethods().ToList();
            var actions = facetFactory.FindActions(methods, classStrategy);

            var expectedNames = new List<string> {
                "ActionWithNoParameters",
                "ActionWithOneGoodParameter",
                "ActionWithTwoGoodParameter",
                "ActionWithNullableParameter",
                "ToString",
                "Equals",
                "GetHashCode"
            };

            Assert.AreEqual(expectedNames.Count, actions.Count);

            expectedNames.ForEach(n => Assert.IsTrue(actions.Select(a => a.Name).Contains(n)));
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ActionMethodsFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local

        private class Customer {
            public void SomeAction() {}
        }

        private class Customer1 {
            [Named("Renamed an action with a named annotation")]
            public void AnActionWithNamedAnnotation() {}

            public void AnActionWithNullableParm(bool? parm) {}
            public void AnActionWithoutNullableParm(bool parm) {}
        }

        private class Customer11 {
            public void SomeAction(int x, long y, long z) {}

            public int Default0SomeAction() {
                return 0;
            }

            [Executed(Where.Remotely)]
            public long Default1SomeAction() {
                return 0;
            }

            [Executed(Where.Locally)]
            public long Default2SomeAction() {
                return 0;
            }
        }

        private class Customer22 {
            public void SomeAction(int x, long y, long z) {}

            public int DefaultSomeAction(int x) {
                return 0;
            }

            [Executed(Where.Remotely)]
            public long DefaultSomeAction(long y) {
                return 0;
            }

            [Executed(Where.Locally)]
            public long Default2SomeAction() {
                return 0;
            }
        }

        private class Customer13 {
            public void SomeAction(int x, long y, long z) {}

            public int[] Choices0SomeAction() {
                return new int[0];
            }

            [Executed(Where.Remotely)]
            public long[] Choices1SomeAction() {
                return new long[0];
            }

            [Executed(Where.Locally)]
            public long[] Choices2SomeAction() {
                return new long[0];
            }
        }

        private class Customer26 {
            public void SomeAction(string x, Customer26 y, long z) {}

            public IQueryable<string> AutoComplete0SomeAction(string name) {
                return new string[0].AsQueryable();
            }

            public IQueryable<Customer26> AutoComplete1SomeAction(string name) {
                return new Customer26[0].AsQueryable();
            }

            public IQueryable<long> AutoComplete2SomeAction(string name) {
                return new long[0].AsQueryable();
            }
        }

        private class Customer27 {
            public void SomeAction(string x, string y, string z) {}

            public IQueryable<int> AutoComplete0SomeAction(string name) {
                return new int[0].AsQueryable();
            }

            public IQueryable<string> AutoComplete1SomeAction() {
                return new string[0].AsQueryable();
            }

            public IQueryable<string> AutoComplete2SomeAction(int name) {
                return new string[0].AsQueryable();
            }
        }

        private class Customer28 {
            public void SomeAction(string x, Customer26 y, long z) {}

            [PageSize(33)]
            public IQueryable<string> AutoComplete0SomeAction([MinLength(2)] string name) {
                return new string[0].AsQueryable();
            }

            [PageSize(66)]
            public IQueryable<Customer26> AutoComplete1SomeAction([MinLength(3)] string name) {
                return new Customer26[0].AsQueryable();
            }
        }

        private class Customer30 {
            public void SomeAction(int x, long y, long z) {}

            public int[] Choices0SomeAction(long y, long z) {
                return new int[0];
            }

            [Executed(Where.Remotely)]
            public long[] Choices1SomeAction(long z) {
                return new long[0];
            }

            [Executed(Where.Locally)]
            public long[] Choices2SomeAction() {
                return new long[0];
            }
        }

        private class Customer31 {
            public void SomeAction(int x, long y, long z) {}

            public int[] Choices0SomeAction(long y, long z) {
                return new int[0];
            }

            [Executed(Where.Remotely)]
            public long[] Choices0SomeAction(long z) {
                return new long[0];
            }

            [Executed(Where.Locally)]
            public long[] Choices0SomeAction() {
                return new long[0];
            }
        }

        private class Customer21 {
            public void SomeAction(int x, long y, long z) {}

            public int[] ChoicesSomeAction(int x) {
                return new int[0];
            }

            [Executed(Where.Remotely)]
            public long[] ChoicesSomeAction(long y) {
                return new long[0];
            }

            [Executed(Where.Locally)]
            public long[] Choices2SomeAction() {
                return new long[0];
            }
        }

        private class Customer14 {
            public void SomeAction() {}
        }

        private class Customer15 {
            public string SomeAction() {
                return null;
            }
        }

        private class Customer16 {
            public string SomeAction() {
                return null;
            }
        }

        private class Customer8 {
            public void SomeAction() {}

            public string ValidateSomeAction() {
                return null;
            }
        }

        private class Customer9 {
            public void SomeAction(int x, int y) {}

            public string ValidateSomeAction(int x, int y) {
                return null;
            }
        }

        private class Customer10 {
            public void SomeActionOne() {}

            public bool HideSomeActionOne() {
                return false;
            }

            public void SomeActionTwo(int x) {}

            public bool HideSomeActionTwo() {
                return false;
            }

            public void SomeActionThree(int x) {}

            public bool HideSomeActionThree() {
                return false;
            }

            public void SomeActionFour(int x, int y) {}

        
            public bool HideSomeActionFour() {
                return false;
            }
        }

        private class Customer12 {
            public void SomeActionOne() {}

            public string DisableSomeActionOne() {
                return "";
            }

            public void SomeActionTwo(int x) {}

            public string DisableSomeActionTwo() {
                return "";
            }

            public void SomeActionThree(int x) {}

            public string DisableSomeActionThree() {
                return "";
            }

            public void SomeActionFour(int x, int y) {}

            public string DisableSomeActionFour() {
                return "";
            }
        }

        private class Customer18 {
            public string DisableActionDefault() {
                return "";
            }

            public void SomeActionTwo(int x) {}

            public string DisableSomeActionTwo() {
                return "";
            }

            public void SomeActionThree(int x) {}
        }

        private class Customer19 {
            public bool HideActionDefault() {
                return false;
            }

            public void SomeActionTwo(int x) {}

            public bool HideSomeActionTwo() {
                return false;
            }

            public void SomeActionThree(int x) {}
        }

        public class CustomerStatic {
            public void SomeAction(int x, long y) {}

            public static bool HideSomeAction(IPrincipal principal) {
                return true;
            }

            public static string DisableSomeAction(IPrincipal principal) {
                return "disabled for this user";
            }

            public static void OtherAction(int x, long y) {}
        }

        private class Customer17 {
            public void SomeAction(int x, long y, long z) {}

            public string Validate0SomeAction(int x) {
                return "failed";
            }

            public string Validate1SomeAction(long x) {
                return null;
            }
        }

        private class Customer20 {
            public void SomeAction(int x, long y, long z) {}

            public string ValidateSomeAction(int x) {
                return "failed";
            }

            public string ValidateSomeAction(long y) {
                return null;
            }
        }

        private class Customer23 {
            public void SomeAction(int x, long y, long z) {}

            [Executed(Ajax.Enabled)]
            public string ValidateSomeAction(int x) {
                return "failed";
            }

            public string ValidateSomeAction(long y) {
                return null;
            }
        }

        private class Customer24 {
            public void SomeAction(int x, long y, long z) {}

            [Executed(Ajax.Disabled)]
            public string ValidateSomeAction(int x) {
                return "failed";
            }

            public string ValidateSomeAction(long y) {
                return null;
            }
        }

        private class Customer25 {
            public void SomeAction(int x, long y, long z) {}
        }

        public interface ICustomer {}

        private class Customer32 {
            public void SomeAction(string x, ICustomer y, long z) {}

            public IQueryable<string> AutoComplete0SomeAction(string name) {
                return new string[0].AsQueryable();
            }

            public IQueryable<ICustomer> AutoComplete1SomeAction(string name) {
                return new ICustomer[0].AsQueryable();
            }

            public IQueryable<long> AutoComplete2SomeAction(string name) {
                return new long[0].AsQueryable();
            }
        }

        private class Customer33 {
            public IQueryable<Customer33> SomeQueryableAction1() {
                return null;
            }

            [QueryOnly]
            public IEnumerable<Customer33> SomeQueryableAction2() {
                return null;
            }
        }

        private class Customer34 {
            [NakedObjectsIgnore]
            public void ActionIgnored() {}

            public static void ActionStatic() {}

            public void ActionGeneric<T>(T parm) {}

            public void ActionWithNoParameters() {}
            public void ActionWithOneGoodParameter(int i) {}
            public void ActionWithTwoGoodParameter(int i, Customer34 c) {}

            public void ActionWithOneBadParameter(out int c) {
                c = 0;
            }

            public void ActionWithOneGoodOneBadParameter(int i, ref int j) {}
            public void ActionWithGenericParameter(Predicate<int> p) {}
            public void ActionWithNullableParameter(int? i) {}
            public void ActionWithDictionaryParameter(string path, Dictionary<string, object> answers) {}
        }

        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedParameter.Local
    }

    // Copyright (c) Naked Objects Group Ltd.
}