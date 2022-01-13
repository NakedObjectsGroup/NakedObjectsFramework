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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Facet;
using NakedObjects.Reflector.FacetFactory;

#pragma warning disable 612

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory;

[TestClass]
public class ActionMethodsFacetFactoryTest : AbstractFacetFactoryTest {
    private ActionMethodsFacetFactory facetFactory;

    protected override Type[] SupportedTypes {
        get {
            return new[] {
                typeof(INamedFacet),
                typeof(IActionValidationFacet),
                typeof(IActionParameterValidationFacet),
                typeof(IActionInvocationFacet),
                typeof(IActionDefaultsFacet),
                typeof(IActionChoicesFacet),
                typeof(IDescribedAsFacet),
                typeof(IMandatoryFacet)
            };
        }
    }

    protected override IFacetFactory FacetFactory => facetFactory;

    private static IActionSpecImmutable CreateHolderWithParms() {
        var tps1 = new Mock<IObjectSpecImmutable>(); //"System.Int32"
        var tps2 = new Mock<IObjectSpecImmutable>(); //System.Int64"
        var tps3 = new Mock<IObjectSpecImmutable>(); //"System.Int64"

        var param1 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps1.Object, null);
        var param2 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps2.Object, null);
        var param3 = ImmutableSpecFactory.CreateActionParameterSpecImmutable(tps3.Object, null);

        var parms = new[] { param1, param2, param3 };

        var tpi = new Mock<IIdentifier>(); // ""action"

        var id = tpi.Object;
        return ImmutableSpecFactory.CreateActionSpecImmutable(id, null, parms);
    }

    private void CheckDefaultFacet(MethodInfo defaultMethod, IActionParameterSpecImmutable parameter) {
        var facet = parameter.GetFacet(typeof(IActionDefaultsFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionDefaultsFacetViaMethod);
        Assert.AreEqual(defaultMethod, ((ActionDefaultsFacetViaMethod)facet).GetMethod());
        Assert.IsNotNull(((ActionDefaultsFacetViaMethod)facet).MethodDelegate);

        AssertMethodRemoved(defaultMethod);
    }

    private void CheckValidateParameterFacet(MethodInfo method, IActionParameterSpecImmutable parameter) {
        var facet = parameter.GetFacet(typeof(IActionParameterValidationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionParameterValidation);
        Assert.AreEqual(method, ((ActionParameterValidation)facet).GetMethod());

        AssertMethodRemoved(method);
    }

    private void CheckChoicesFacet(MethodInfo choicesMethod, IActionParameterSpecImmutable parameter) {
        var facet = parameter.GetFacet(typeof(IActionChoicesFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionChoicesFacetViaMethod);
        Assert.AreEqual(choicesMethod, ((ActionChoicesFacetViaMethod)facet).GetMethod());

        AssertMethodRemoved(choicesMethod);
    }

    private void CheckAutoCompleteFacet(MethodInfo autoCompleteMethod, IActionParameterSpecImmutable parameter, int pageSize, int minLength) {
        var facet = parameter.GetFacet(typeof(IAutoCompleteFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is AutoCompleteFacet);
        var acf = (AutoCompleteFacet)facet;
        Assert.AreEqual(autoCompleteMethod, acf.GetMethod());

        AssertMethodRemoved(autoCompleteMethod);

        Assert.AreEqual(pageSize, acf.PageSize);
        Assert.AreEqual(minLength, acf.MinLength);
    }

    private void CheckAutoCompleteFacetIsNull(MethodInfo autoCompleteMethod, IActionParameterSpecImmutable parameter) {
        var facet = parameter.GetFacet(typeof(IAutoCompleteFacet));
        Assert.IsNull(facet);

        AssertMethodNotRemoved(autoCompleteMethod);
    }

    private void CheckChoicesFacetIsNull(MethodInfo choicesMethod, IActionParameterSpecImmutable parameter) {
        var facet = parameter.GetFacet(typeof(IActionChoicesFacet));
        Assert.IsNull(facet);

        AssertMethodNotRemoved(choicesMethod);
    }

    [TestMethod]
    public void TestActionInvocationFacetIsInstalledAndMethodRemoved() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer), "SomeAction");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
        Assert.IsFalse(actionInvocationFacetViaMethod.IsQueryOnly);

        AssertMethodRemoved(actionMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestActionInvocationFacetQueryableByType() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer33), "SomeQueryableAction1");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
        Assert.IsTrue(actionInvocationFacetViaMethod.IsQueryOnly);

        AssertMethodRemoved(actionMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestActionInvocationFacetQueryableByAnnotation() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer33), "SomeQueryableAction2");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());
        Assert.IsTrue(actionInvocationFacetViaMethod.IsQueryOnly);

        AssertMethodRemoved(actionMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestActionOnType() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var type = typeof(Customer16);
        var actionMethod = FindMethod(type, "SomeAction");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        var (_, expectedSpec) = Reflector.LoadSpecification(type, null);
        Assert.AreEqual(expectedSpec, actionInvocationFacetViaMethod.OnType);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestActionReturnTypeWhenNotVoid() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer15), "SomeAction");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        var (_, expectedSpec) = Reflector.LoadSpecification(typeof(string), null);
        Assert.AreEqual(expectedSpec, actionInvocationFacetViaMethod.ReturnType);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestActionReturnTypeWhenVoid() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer14), "SomeAction");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionInvocationFacet));
        var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod)facet;
        var (_, expectedSpec) = Reflector.LoadSpecification(typeof(void), null);
        Assert.AreEqual(expectedSpec, actionInvocationFacetViaMethod.ReturnType);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestAddsNullableFacetToParm() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethodIgnoreParms(typeof(Customer1), "AnActionWithNullableParm");
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(INullableFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is NullableFacetAlways);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestDoesntAddNullableFacetToParm() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethodIgnoreParms(typeof(Customer1), "AnActionWithoutNullableParm");
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(INullableFacet));
        Assert.IsNull(facet);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestIgnoresParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer27), "SomeAction");
        var autoComplete0Method = FindMethodIgnoreParms(typeof(Customer27), "AutoComplete0SomeAction");
        var autoComplete1Method = FindMethodIgnoreParms(typeof(Customer27), "AutoComplete1SomeAction");
        var autoComplete2Method = FindMethodIgnoreParms(typeof(Customer27), "AutoComplete2SomeAction");

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckAutoCompleteFacetIsNull(autoComplete0Method, facetHolderWithParms.Parameters[0]);
        CheckAutoCompleteFacetIsNull(autoComplete1Method, facetHolderWithParms.Parameters[1]);
        CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsDisabledForSessionFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(CustomerStatic), "SomeAction", new[] { typeof(int), typeof(long) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IDisableForSessionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForSessionFacetNone);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(CustomerStatic), "SomeAction", new[] { typeof(int), typeof(long) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IHideForSessionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForSessionFacetNone);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterAutoCompleteMethodAttrributes() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer28), "SomeAction");
        var autoComplete0Method = FindMethodIgnoreParms(typeof(Customer28), "AutoComplete0SomeAction");
        var autoComplete1Method = FindMethodIgnoreParms(typeof(Customer28), "AutoComplete1SomeAction");

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 33, 2);
        CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 66, 3);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer26), "SomeAction");
        var autoComplete0Method = FindMethodIgnoreParms(typeof(Customer26), "AutoComplete0SomeAction");
        var autoComplete1Method = FindMethodIgnoreParms(typeof(Customer26), "AutoComplete1SomeAction");
        var autoComplete2Method = FindMethodIgnoreParms(typeof(Customer26), "AutoComplete2SomeAction");

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
        CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
        CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethodInterface() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer32), "SomeAction");
        var autoComplete0Method = FindMethodIgnoreParms(typeof(Customer32), "AutoComplete0SomeAction");
        var autoComplete1Method = FindMethodIgnoreParms(typeof(Customer32), "AutoComplete1SomeAction");
        var autoComplete2Method = FindMethodIgnoreParms(typeof(Customer32), "AutoComplete2SomeAction");

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
        CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
        CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer13), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method = FindMethod(typeof(Customer13), "Choices0SomeAction", Array.Empty<Type>());
        var choices1Method = FindMethod(typeof(Customer13), "Choices1SomeAction", Array.Empty<Type>());
        var choices2Method = FindMethod(typeof(Customer13), "Choices2SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);
        CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);
        CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodDuplicate() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer30), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method1 = FindMethod(typeof(Customer30), "Choices0SomeAction", new[] { typeof(long), typeof(long) });
        var choices0Method2 = FindMethod(typeof(Customer30), "Choices0SomeAction", new[] { typeof(long) });
        var choices0Method3 = FindMethod(typeof(Customer30), "Choices0SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method1, facetHolderWithParms.Parameters[0]);
        AssertMethodNotRemoved(choices0Method2);
        AssertMethodNotRemoved(choices0Method3);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodWithParms() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer30), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method = FindMethod(typeof(Customer30), "Choices0SomeAction", new[] { typeof(long), typeof(long) });
        var choices1Method = FindMethod(typeof(Customer30), "Choices1SomeAction", new[] { typeof(long) });
        var choices2Method = FindMethod(typeof(Customer30), "Choices2SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);
        CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);
        CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterChoicesMethodByNameNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer21), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method = FindMethod(typeof(Customer21), "ChoicesSomeAction", new[] { typeof(int) });
        var choices1Method = FindMethod(typeof(Customer21), "ChoicesSomeAction", new[] { typeof(long) });
        var choices2Method = FindMethod(typeof(Customer21), "Choices2SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);
        CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);
        CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterDefaultsMethodByIndexNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer11), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var default0Method = FindMethod(typeof(Customer11), "Default0SomeAction", Array.Empty<Type>());
        var default1Method = FindMethod(typeof(Customer11), "Default1SomeAction", Array.Empty<Type>());
        var default2Method = FindMethod(typeof(Customer11), "Default2SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();

        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);
        CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);
        CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterDefaultsMethodByNameNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer22), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var default0Method = FindMethod(typeof(Customer22), "DefaultSomeAction", new[] { typeof(int) });
        var default1Method = FindMethod(typeof(Customer22), "DefaultSomeAction", new[] { typeof(long) });
        var default2Method = FindMethod(typeof(Customer22), "Default2SomeAction", Array.Empty<Type>());

        var facetHolderWithParms = CreateHolderWithParms();

        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);
        CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);
        CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterValidationMethodByIndexNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer17), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var validateParameter0Method = FindMethod(typeof(Customer17), "Validate0SomeAction", new[] { typeof(int) });
        var validateParameter1Method = FindMethod(typeof(Customer17), "Validate1SomeAction", new[] { typeof(long) });

        var facetHolderWithParms = CreateHolderWithParms();

        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckValidateParameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);
        CheckValidateParameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsParameterValidationMethodByNameNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer20), "SomeAction", new[] { typeof(int), typeof(long), typeof(long) });
        var validateParameter0Method = FindMethod(typeof(Customer20), "ValidateSomeAction", new[] { typeof(int) });
        var validateParameter1Method = FindMethod(typeof(Customer20), "ValidateSomeAction", new[] { typeof(long) });

        var facetHolderWithParms = CreateHolderWithParms();

        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckValidateParameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);
        CheckValidateParameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsValidateMethodNoArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer8), "SomeAction");
        var validateMethod = FindMethod(typeof(Customer8), "ValidateSomeAction");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionValidationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionValidationFacet);
        var actionValidationFacetViaMethod = (ActionValidationFacet)facet;
        Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
        AssertMethodRemoved(validateMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestInstallsValidateMethodSomeArgsFacetAndRemovesMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer9), "SomeAction", new[] { typeof(int), typeof(int) });
        var validateMethod = FindMethod(typeof(Customer9), "ValidateSomeAction", new[] { typeof(int), typeof(int) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IActionValidationFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is ActionValidationFacet);
        var actionValidationFacetViaMethod = (ActionValidationFacet)facet;
        Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
        AssertMethodRemoved(validateMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDefaultDisableMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer18), "SomeActionThree");
        var disableMethod = FindMethodIgnoreParms(typeof(Customer18), "DisableActionDefault");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IDisableForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForContextFacet);
        Assert.AreEqual(disableMethod, ((IImperativeFacet)facet).GetMethod());
        AssertMethodNotRemoved(disableMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDefaultHideMethod() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer19), "SomeActionThree");
        var disableMethod = FindMethodIgnoreParms(typeof(Customer19), "HideActionDefault");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IHideForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForContextFacet);
        Assert.AreEqual(disableMethod, ((IImperativeFacet)facet).GetMethod());
        AssertMethodNotRemoved(disableMethod);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDisableMethodDifferentSignature() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer12), "SomeActionThree");
        var hideMethod = FindMethodIgnoreParms(typeof(Customer12), "DisableSomeActionThree");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IDisableForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDisableMethodNoParms() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer12), "SomeActionOne");
        var hideMethod = FindMethod(typeof(Customer12), "DisableSomeActionOne");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IDisableForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDisableMethodOverriddingDefault() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer18), "SomeActionTwo");
        var disableMethod = FindMethodIgnoreParms(typeof(Customer18), "DisableSomeActionTwo");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IDisableForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForContextFacet);
        Assert.AreEqual(disableMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpDisableMethodSameSignature() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer12), "SomeActionTwo");
        var hideMethod = FindMethodIgnoreParms(typeof(Customer12), "DisableSomeActionTwo");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IDisableForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is DisableForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpHideMethodDifferentSignature() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer10), "SomeActionThree");
        var hideMethod = FindMethodIgnoreParms(typeof(Customer10), "HideSomeActionThree");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IHideForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpHideMethodNoParms() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer10), "SomeActionOne");
        var hideMethod = FindMethod(typeof(Customer10), "HideSomeActionOne");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IHideForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpHideMethodOverriddingDefault() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer19), "SomeActionTwo");
        var hideMethod = FindMethodIgnoreParms(typeof(Customer19), "HideSomeActionTwo");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IHideForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestPickUpHideMethodSameSignature() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethodIgnoreParms(typeof(Customer10), "SomeActionTwo");
        var hideMethod = FindMethodIgnoreParms(typeof(Customer10), "HideSomeActionTwo");
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);

        var facet = Specification.GetFacet<IHideForContextFacet>();
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is HideForContextFacet);
        Assert.AreEqual(hideMethod, ((IImperativeFacet)facet).GetMethod());
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestProvidesDefaultNameForActionButIgnoresAnyNamedAnnotation() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer1), "AnActionWithNamedAnnotation");
        metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet<IMemberNamedFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual("An Action With Named Annotation", facet.FriendlyName(null));
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestChoicesParametersDoNotMatchNames() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        var actionMethod = FindMethod(typeof(Customer35), nameof(Customer35.SomeAction), new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method = FindMethod(typeof(Customer35), nameof(Customer35.Choices0SomeAction), new[] { typeof(int) });
        var choices1Method = FindMethod(typeof(Customer35), nameof(Customer35.Choices1SomeAction), new[] { typeof(long) });
        var choices2Method = FindMethod(typeof(Customer35), nameof(Customer35.Choices2SomeAction), new[] { typeof(long) });

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);
        CheckChoicesFacetIsNull(choices1Method, facetHolderWithParms.Parameters[1]);
        CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    [TestMethod]
    public void TestChoicesParametersDoNotMatchTypes() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        var actionMethod = FindMethod(typeof(Customer36), nameof(Customer36.SomeAction), new[] { typeof(int), typeof(long), typeof(long) });
        var choices0Method = FindMethod(typeof(Customer36), nameof(Customer36.Choices0SomeAction), new[] { typeof(int) });
        var choices1Method = FindMethod(typeof(Customer36), nameof(Customer36.Choices1SomeAction), new[] { typeof(long) });
        var choices2Method = FindMethod(typeof(Customer36), nameof(Customer36.Choices2SomeAction), new[] { typeof(string) });

        var facetHolderWithParms = CreateHolderWithParms();
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, facetHolderWithParms, metamodel);

        CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);
        CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);
        CheckChoicesFacetIsNull(choices2Method, facetHolderWithParms.Parameters[2]);
        Assert.AreEqual(0, metamodel.Count);
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new ActionMethodsFacetFactory(GetOrder<ActionMethodsFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion

    // ReSharper disable UnusedParameter.Local

    private class Customer {
        public void SomeAction() { }
    }

    private class Customer1 {
        [Named("Renamed an action with a named annotation")]
        public void AnActionWithNamedAnnotation() { }

        public void AnActionWithNullableParm(bool? parm) { }
        public void AnActionWithoutNullableParm(bool parm) { }
    }

    private class Customer11 {
        public void SomeAction(int x, long y, long z) { }

        public int Default0SomeAction() => 0;

        public long Default1SomeAction() => 0;

        public long Default2SomeAction() => 0;
    }

    private class Customer22 {
        public void SomeAction(int x, long y, long z) { }

        public int DefaultSomeAction(int x) => 0;

        public long DefaultSomeAction(long y) => 0;

        public long Default2SomeAction() => 0;
    }

    private class Customer13 {
        public void SomeAction(int x, long y, long z) { }

        public int[] Choices0SomeAction() => Array.Empty<int>();

        public long[] Choices1SomeAction() => Array.Empty<long>();

        public long[] Choices2SomeAction() => Array.Empty<long>();
    }

    private class Customer26 {
        public void SomeAction(string x, Customer26 y, long z) { }

        public IQueryable<string> AutoComplete0SomeAction(string name) => Array.Empty<string>().AsQueryable();

        public IQueryable<Customer26> AutoComplete1SomeAction(string name) => Array.Empty<Customer26>().AsQueryable();

        public IQueryable<long> AutoComplete2SomeAction(string name) => Array.Empty<long>().AsQueryable();
    }

    private class Customer27 {
        public void SomeAction(string x, string y, string z) { }

        public IQueryable<int> AutoComplete0SomeAction(string name) => Array.Empty<int>().AsQueryable();

        public IQueryable<string> AutoComplete1SomeAction() => Array.Empty<string>().AsQueryable();

        public IQueryable<string> AutoComplete2SomeAction(int name) => Array.Empty<string>().AsQueryable();
    }

    private class Customer28 {
        public void SomeAction(string x, Customer26 y, long z) { }

        [PageSize(33)]
        public IQueryable<string> AutoComplete0SomeAction([MinLength(2)] string name) => Array.Empty<string>().AsQueryable();

        [PageSize(66)]
        public IQueryable<Customer26> AutoComplete1SomeAction([MinLength(3)] string name) => Array.Empty<Customer26>().AsQueryable();
    }

    private class Customer30 {
        public void SomeAction(int x, long y, long z) { }

        public int[] Choices0SomeAction(long y, long z) => Array.Empty<int>();

        public long[] Choices1SomeAction(long z) => Array.Empty<long>();

        public long[] Choices2SomeAction() => Array.Empty<long>();
    }

    private class Customer21 {
        public void SomeAction(int x, long y, long z) { }

        public int[] ChoicesSomeAction(int x) => Array.Empty<int>();

        public long[] ChoicesSomeAction(long y) => Array.Empty<long>();

        public long[] Choices2SomeAction() => Array.Empty<long>();
    }

    private class Customer14 {
        public void SomeAction() { }
    }

    private class Customer15 {
        public string SomeAction() => null;
    }

    private class Customer16 {
        public string SomeAction() => null;
    }

    private class Customer8 {
        public void SomeAction() { }

        public string ValidateSomeAction() => null;
    }

    private class Customer9 {
        public void SomeAction(int x, int y) { }

        public string ValidateSomeAction(int x, int y) => null;
    }

    private class Customer10 {
        public void SomeActionOne() { }

        public bool HideSomeActionOne() => false;

        public void SomeActionTwo(int x) { }

        public bool HideSomeActionTwo() => false;

        public void SomeActionThree(int x) { }

        public bool HideSomeActionThree() => false;

        public void SomeActionFour(int x, int y) { }

        public bool HideSomeActionFour() => false;
    }

    private class Customer12 {
        public void SomeActionOne() { }

        public string DisableSomeActionOne() => "";

        public void SomeActionTwo(int x) { }

        public string DisableSomeActionTwo() => "";

        public void SomeActionThree(int x) { }

        public string DisableSomeActionThree() => "";

        public void SomeActionFour(int x, int y) { }

        public string DisableSomeActionFour() => "";
    }

    private class Customer18 {
        public string DisableActionDefault() => "";

        public void SomeActionTwo(int x) { }

        public string DisableSomeActionTwo() => "";

        public void SomeActionThree(int x) { }
    }

    private class Customer19 {
        public bool HideActionDefault() => false;

        public void SomeActionTwo(int x) { }

        public bool HideSomeActionTwo() => false;

        public void SomeActionThree(int x) { }
    }

    public class CustomerStatic {
        public void SomeAction(int x, long y) { }

        public static bool HideSomeAction(IPrincipal principal) => true;

        public static string DisableSomeAction(IPrincipal principal) => "disabled for this user";

        public static void OtherAction(int x, long y) { }
    }

    private class Customer17 {
        public void SomeAction(int x, long y, long z) { }

        public string Validate0SomeAction(int x) => "failed";

        public string Validate1SomeAction(long x) => null;
    }

    private class Customer20 {
        public void SomeAction(int x, long y, long z) { }

        public string ValidateSomeAction(int x) => "failed";

        public string ValidateSomeAction(long y) => null;
    }

    private class Customer23 {
        public void SomeAction(int x, long y, long z) { }

        public string ValidateSomeAction(int x) => "failed";

        public string ValidateSomeAction(long y) => null;
    }

    private class Customer24 {
        public void SomeAction(int x, long y, long z) { }

        public string ValidateSomeAction(int x) => "failed";

        public string ValidateSomeAction(long y) => null;
    }

    private class Customer25 {
        public void SomeAction(int x, long y, long z) { }
    }

    public interface ICustomer { }

    private class Customer32 {
        public void SomeAction(string x, ICustomer y, long z) { }

        public IQueryable<string> AutoComplete0SomeAction(string name) => Array.Empty<string>().AsQueryable();

        public IQueryable<ICustomer> AutoComplete1SomeAction(string name) => Array.Empty<ICustomer>().AsQueryable();

        public IQueryable<long> AutoComplete2SomeAction(string name) => Array.Empty<long>().AsQueryable();
    }

    private class Customer33 {
        public IQueryable<Customer33> SomeQueryableAction1() => null;

        [QueryOnly]
        public IEnumerable<Customer33> SomeQueryableAction2() => null;
    }

    private class Customer34 {
        [NakedObjectsIgnore]
        public void ActionIgnored() { }

        public static void ActionStatic() { }

        public void ActionGeneric<T>(T parm) { }

        public void ActionWithNoParameters() { }
        public void ActionWithOneGoodParameter(int i) { }
        public void ActionWithTwoGoodParameter(int i, Customer34 c) { }

        public void ActionWithOneBadParameter(out int c) {
            c = 0;
        }

        public void ActionWithOneGoodOneBadParameter(int i, ref int j) { }
        public void ActionWithGenericParameter(Predicate<int> p) { }
        public void ActionWithNullableParameter(int? i) { }
        public void ActionWithDictionaryParameter(string path, Dictionary<string, object> answers) { }
    }

    private class Customer35 {
        public void SomeAction(int x, long y, long z) { }

        public int[] Choices0SomeAction(int x) => Array.Empty<int>();

        public long[] Choices1SomeAction(long p) => Array.Empty<long>();

        public long[] Choices2SomeAction(long z) => Array.Empty<long>();
    }

    private class Customer36 {
        public void SomeAction(int x, long y, long z) { }

        public int[] Choices0SomeAction(int x) => Array.Empty<int>();

        public long[] Choices1SomeAction(long y) => Array.Empty<long>();

        public long[] Choices2SomeAction(string z) => Array.Empty<long>();
    }

    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedParameter.Local
}

// Copyright (c) Naked Objects Group Ltd.