// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Moq;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Choices;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Facets.Actions.Validate;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Facets.Actions.Choices;
using NakedObjects.Reflector.DotNet.Facets.Actions.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Actions.Executed;
using NakedObjects.Reflector.DotNet.Facets.Actions.Invoke;
using NakedObjects.Reflector.DotNet.Facets.Actions.Validate;
using NakedObjects.Reflector.DotNet.Facets.AutoComplete;
using NakedObjects.Reflector.DotNet.Facets.Disable;
using NakedObjects.Reflector.DotNet.Facets.Hide;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.Peer;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    [TestFixture]
    public class ActionMethodsFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ActionMethodsFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

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

        private static DotNetNakedObjectActionPeer CreateHolderWithParms() {
            var tps1 = new Mock<INakedObjectSpecification>(); //"System.Int32"
            var tps2 = new Mock<INakedObjectSpecification>(); //System.Int64"
            var tps3 = new Mock<INakedObjectSpecification>(); //"System.Int64"

            var param1 = new DotNetNakedObjectActionParamPeer(tps1.Object);
            var param2 = new DotNetNakedObjectActionParamPeer(tps2.Object);
            var param3 = new DotNetNakedObjectActionParamPeer(tps3.Object);

            var parms = new INakedObjectActionParamPeer[] {param1, param2, param3};

            var tpi = new Mock<IIdentifier>(); // ""action"

            IIdentifier id = tpi.Object;
            return new DotNetNakedObjectActionPeer(id, parms);
        }

        private void CheckDefaultFacet(MethodInfo defaultMethod, INakedObjectActionParamPeer parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionDefaultsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionDefaultsFacetViaMethod);
            Assert.AreEqual(defaultMethod, ((ActionDefaultsFacetViaMethod) facet).GetMethod());

            AssertMethodRemoved(defaultMethod);
        }

        private void CheckValidatePrameterFacet(MethodInfo method, INakedObjectActionParamPeer parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionParameterValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionParameterValidationFacetViaMethod);
            Assert.AreEqual(method, ((ActionParameterValidationFacetViaMethod) facet).GetMethod());

            AssertMethodRemoved(method);
        }

        private void CheckChoicesFacet(MethodInfo choicesMethod, INakedObjectActionParamPeer parameter) {
            IFacet facet = parameter.GetFacet(typeof (IActionChoicesFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionChoicesFacetViaMethod);
            Assert.AreEqual(choicesMethod, ((ActionChoicesFacetViaMethod) facet).GetMethod());

            AssertMethodRemoved(choicesMethod);
        }

        private void CheckAutoCompleteFacet(MethodInfo autoCompleteMethod, INakedObjectActionParamPeer parameter, int pageSize, int minLength) {
            IFacet facet = parameter.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AutoCompleteFacetViaMethod);
            var acf = (AutoCompleteFacetViaMethod) facet;
            Assert.AreEqual(autoCompleteMethod, acf.GetMethod());

            AssertMethodRemoved(autoCompleteMethod);

            Assert.AreEqual(pageSize, acf.PageSize);
            Assert.AreEqual(minLength, acf.MinLength);
        }

        private void CheckAutoCompleteFacetIsNull(MethodInfo autoCompleteMethod, INakedObjectActionParamPeer parameter) {
            IFacet facet = parameter.GetFacet(typeof (IAutoCompleteFacet));
            Assert.IsNull(facet);

            AssertMethodNotRemoved(autoCompleteMethod);
        }

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

            public IEnumerable<string> AutoComplete0SomeAction(string name) {
                return new string[0].AsQueryable();
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

            public bool HideSomeActionTwo(int x) {
                return false;
            }

            public void SomeActionThree(int x) {}

            public bool HideSomeActionThree() {
                return false;
            }

            public void SomeActionFour(int x, int y) {}

            public bool HideSomeActionFour(int x, int y) {
                return false;
            }

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

            public string DisableSomeActionTwo(int x) {
                return "";
            }

            public void SomeActionThree(int x) {}

            public string DisableSomeActionThree() {
                return "";
            }

            public void SomeActionFour(int x, int y) {}

            public string DisableSomeActionFour(int x, int y) {
                return "";
            }

            public string DisableSomeActionFour() {
                return "";
            }
        }

        private class Customer18 {
            public string DisableActionDefault() {
                return "";
            }

            public void SomeActionTwo(int x) {}

            public string DisableSomeActionTwo(int x) {
                return "";
            }

            public void SomeActionThree(int x) {}
        }

        private class Customer19 {
            public bool HideActionDefault() {
                return false;
            }

            public void SomeActionTwo(int x) {}

            public bool HideSomeActionTwo(int x) {
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

        // ReSharper restore UnusedMember.Local
        // ReSharper restore UnusedParameter.Local

        [Test]
        public void TestActionInvocationFacetIsInstalledAndMethodRemoved() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionInvocationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionInvocationFacetViaMethod);
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(actionMethod, actionInvocationFacetViaMethod.GetMethod());

            AssertMethodRemoved(actionMethod);
        }

        [Test]
        public void TestActionOnType() {
            Type type = typeof (Customer16);
            MethodInfo actionMethod = FindMethod(type, "SomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(type), actionInvocationFacetViaMethod.OnType);
        }

        [Test]
        public void TestActionReturnTypeWhenNotVoid() {
            MethodInfo actionMethod = FindMethod(typeof (Customer15), "SomeAction");
            //   reflector.LoadSpecification(typeof(string));
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(typeof (string)), actionInvocationFacetViaMethod.ReturnType);
        }

        [Test]
        public void TestActionReturnTypeWhenVoid() {
            MethodInfo actionMethod = FindMethod(typeof (Customer14), "SomeAction");
            //     reflector.setLoadSpecificationClassReturn(voidNoSpec);
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionInvocationFacet));
            var actionInvocationFacetViaMethod = (ActionInvocationFacetViaMethod) facet;
            Assert.AreEqual(Reflector.LoadSpecification(typeof (void)), actionInvocationFacetViaMethod.ReturnType);
        }

        [Test]
        public void TestAddsNullableFacetToParm() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer1), "AnActionWithNullableParm");
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (INullableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is NullableFacetAlways);
        }

        [Test]
        public void TestAjaxFacetAddedIfNoValidate() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer25), "SomeAction");
            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(method, MethodRemover, facetHolderWithParms);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacetAnnotation);
        }

        [Test]
        public void TestAjaxFacetFoundAndMethodRemovedDisabled() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer24), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer24), "ValidateSomeAction", new[] {typeof (int)});
            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(method, MethodRemover, facetHolderWithParms);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is AjaxFacetAnnotation);


            AssertMethodRemoved(propertyValidateMethod);
        }

        [Test]
        public void TestAjaxFacetFoundAndMethodRemovedEnabled() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer23), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer23), "ValidateSomeAction", new[] {typeof (int)});
            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(method, MethodRemover, facetHolderWithParms);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);

            AssertMethodRemoved(propertyValidateMethod);
        }

        [Test]
        public void TestAjaxFacetNotAddedByDefault() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer20), "SomeAction");
            MethodInfo propertyValidateMethod = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (int)});
            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(method, MethodRemover, facetHolderWithParms);
            IFacet facet = facetHolderWithParms.Parameters[0].GetFacet(typeof (IAjaxFacet));
            Assert.IsNull(facet);

            AssertMethodRemoved(propertyValidateMethod);
        }

        [Test]
        public void TestDoesntAddNullableFacetToParm() {
            MethodInfo method = FindMethodIgnoreParms(typeof (Customer1), "AnActionWithoutNullableParm");
            facetFactory.ProcessParams(method, 0, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (INullableFacet));
            Assert.IsNull(facet);
        }


        [Test]
        public override void TestFeatureTypes() {
            NakedObjectFeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Objects));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, NakedObjectFeatureType.Collection));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, NakedObjectFeatureType.ActionParameter));
        }

        [Test]
        public void TestIgnoresParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer27), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer27), "AutoComplete2SomeAction");

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckAutoCompleteFacetIsNull(autoComplete0Method, facetHolderWithParms.Parameters[0]);
            CheckAutoCompleteFacetIsNull(autoComplete1Method, facetHolderWithParms.Parameters[1]);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        }


        [Test]
        public void TestInstallsDisabledForSessionFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (CustomerStatic), "SomeAction", new[] {typeof (int), typeof (long)});
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IDisableForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForSessionFacetNone);
        }


        [Test]
        public void TestInstallsHiddenForSessionFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (CustomerStatic), "SomeAction", new[] {typeof (int), typeof (long)});
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IHideForSessionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForSessionFacetNone);
        }

        [Test]
        public void TestInstallsParameterAutoCompleteMethodAttrributes() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer28), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer28), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer28), "AutoComplete1SomeAction");

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 33, 2);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 66, 3);
        }


        [Test]
        public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer26), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer26), "AutoComplete2SomeAction");

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        }

        [Test]
        public void TestInstallsParameterAutoCompleteMethodByIndexNoArgsFacetAndRemovesMethodInterface() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer32), "SomeAction");
            MethodInfo autoComplete0Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete0SomeAction");
            MethodInfo autoComplete1Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete1SomeAction");
            MethodInfo autoComplete2Method = FindMethodIgnoreParms(typeof (Customer32), "AutoComplete2SomeAction");

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckAutoCompleteFacet(autoComplete0Method, facetHolderWithParms.Parameters[0], 50, 0);
            CheckAutoCompleteFacet(autoComplete1Method, facetHolderWithParms.Parameters[1], 50, 0);
            CheckAutoCompleteFacetIsNull(autoComplete2Method, facetHolderWithParms.Parameters[2]);
        }

        [Test]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer13), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer13), "Choices0SomeAction", new Type[] {});
            MethodInfo choices1Method = FindMethod(typeof (Customer13), "Choices1SomeAction", new Type[] {});
            MethodInfo choices2Method = FindMethod(typeof (Customer13), "Choices2SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Architecture.Facets.Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Architecture.Facets.Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);
        }


        [Test]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodDuplicate() {
            MethodInfo actionMethod = FindMethod(typeof (Customer30), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method1 = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long), typeof (long)});
            MethodInfo choices0Method2 = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long)});
            MethodInfo choices0Method3 = FindMethod(typeof (Customer30), "Choices0SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckChoicesFacet(choices0Method1, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            AssertMethodNotRemoved(choices0Method2);
            AssertMethodNotRemoved(choices0Method3);
        }

        [Test]
        public void TestInstallsParameterChoicesMethodByIndexNoArgsFacetAndRemovesMethodWithParms() {
            MethodInfo actionMethod = FindMethod(typeof (Customer30), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer30), "Choices0SomeAction", new[] {typeof (long), typeof (long)});
            MethodInfo choices1Method = FindMethod(typeof (Customer30), "Choices1SomeAction", new[] {typeof (long)});
            MethodInfo choices2Method = FindMethod(typeof (Customer30), "Choices2SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Architecture.Facets.Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Architecture.Facets.Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);
        }


        [Test]
        public void TestInstallsParameterChoicesMethodByNameNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer21), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo choices0Method = FindMethod(typeof (Customer21), "ChoicesSomeAction", new[] {typeof (int)});
            MethodInfo choices1Method = FindMethod(typeof (Customer21), "ChoicesSomeAction", new[] {typeof (long)});
            MethodInfo choices2Method = FindMethod(typeof (Customer21), "Choices2SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();
            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckChoicesFacet(choices0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckChoicesFacet(choices1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices1Method), Architecture.Facets.Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);

            CheckChoicesFacet(choices2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices2Method), Architecture.Facets.Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(choices0Method), Architecture.Facets.Where.Default);
        }


        [Test]
        public void TestInstallsParameterDefaultsMethodByIndexNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer11), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo default0Method = FindMethod(typeof (Customer11), "Default0SomeAction", new Type[] {});
            MethodInfo default1Method = FindMethod(typeof (Customer11), "Default1SomeAction", new Type[] {});
            MethodInfo default2Method = FindMethod(typeof (Customer11), "Default2SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();

            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(default1Method), Architecture.Facets.Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(default0Method), Architecture.Facets.Where.Default);


            CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default2Method), Architecture.Facets.Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default0Method), Architecture.Facets.Where.Default);
        }


        [Test]
        public void TestInstallsParameterDefaultsMethodByNameNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer22), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo default0Method = FindMethod(typeof (Customer22), "DefaultSomeAction", new[] {typeof (int)});
            MethodInfo default1Method = FindMethod(typeof (Customer22), "DefaultSomeAction", new[] {typeof (long)});
            MethodInfo default2Method = FindMethod(typeof (Customer22), "Default2SomeAction", new Type[] {});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();

            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckDefaultFacet(default0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckDefaultFacet(default1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted1);

            Assert.AreEqual(facetExecuted1.ExecutedWhere(default1Method), Architecture.Facets.Where.Remotely);
            Assert.AreEqual(facetExecuted1.ExecutedWhere(default0Method), Architecture.Facets.Where.Default);


            CheckDefaultFacet(default2Method, facetHolderWithParms.Parameters[2]);

            var facetExecuted2 = facetHolderWithParms.Parameters[2].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNotNull(facetExecuted2);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default2Method), Architecture.Facets.Where.Locally);
            Assert.AreEqual(facetExecuted2.ExecutedWhere(default0Method), Architecture.Facets.Where.Default);
        }


        [Test]
        public void TestInstallsParameterValidationMethodByIndexNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer17), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo validateParameter0Method = FindMethod(typeof (Customer17), "Validate0SomeAction", new[] {typeof (int)});
            MethodInfo validateParameter1Method = FindMethod(typeof (Customer17), "Validate1SomeAction", new[] {typeof (long)});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();

            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckValidatePrameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckValidatePrameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNull(facetExecuted1);
        }

        [Test]
        public void TestInstallsParameterValidationMethodByNameNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer20), "SomeAction", new[] {typeof (int), typeof (long), typeof (long)});
            MethodInfo validateParameter0Method = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (int)});
            MethodInfo validateParameter1Method = FindMethod(typeof (Customer20), "ValidateSomeAction", new[] {typeof (long)});

            DotNetNakedObjectActionPeer facetHolderWithParms = CreateHolderWithParms();

            facetFactory.Process(actionMethod, MethodRemover, facetHolderWithParms);

            CheckValidatePrameterFacet(validateParameter0Method, facetHolderWithParms.Parameters[0]);

            IFacet facetExecuted0 = facetHolderWithParms.Parameters[0].GetFacet(typeof (IExecutedControlMethodFacet));
            Assert.IsNull(facetExecuted0);

            CheckValidatePrameterFacet(validateParameter1Method, facetHolderWithParms.Parameters[1]);

            var facetExecuted1 = facetHolderWithParms.Parameters[1].GetFacet<IExecutedControlMethodFacet>();
            Assert.IsNull(facetExecuted1);
        }


        [Test]
        public void TestInstallsValidateMethodNoArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer8), "SomeAction");
            MethodInfo validateMethod = FindMethod(typeof (Customer8), "ValidateSomeAction");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionValidationFacetViaMethod);
            var actionValidationFacetViaMethod = (ActionValidationFacetViaMethod) facet;
            Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
            AssertMethodRemoved(validateMethod);
        }

        [Test]
        public void TestInstallsValidateMethodSomeArgsFacetAndRemovesMethod() {
            MethodInfo actionMethod = FindMethod(typeof (Customer9), "SomeAction", new[] {typeof (int), typeof (int)});
            MethodInfo validateMethod = FindMethod(typeof (Customer9), "ValidateSomeAction", new[] {typeof (int), typeof (int)});
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (IActionValidationFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionValidationFacetViaMethod);
            var actionValidationFacetViaMethod = (ActionValidationFacetViaMethod) facet;
            Assert.AreEqual(validateMethod, actionValidationFacetViaMethod.GetMethod());
            AssertMethodRemoved(validateMethod);
        }

        [Test]
        public void TestPickUpDefaultDisableMethod() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer18), "SomeActionThree");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer18), "DisableActionDefault");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
            AssertMethodNotRemoved(disableMethod);
        }

        [Test]
        public void TestPickUpDefaultHideMethod() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer19), "SomeActionThree");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer19), "HideActionDefault");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
            AssertMethodNotRemoved(disableMethod);
        }


        [Test]
        public void TestPickUpDisableMethodDifferentSignature() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer12), "SomeActionThree");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer12), "DisableSomeActionThree");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpDisableMethodNoParms() {
            MethodInfo actionMethod = FindMethod(typeof (Customer12), "SomeActionOne");
            MethodInfo hideMethod = FindMethod(typeof (Customer12), "DisableSomeActionOne");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpDisableMethodOverriddingDefault() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer18), "SomeActionTwo");
            MethodInfo disableMethod = FindMethodIgnoreParms(typeof (Customer18), "DisableSomeActionTwo");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(disableMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpDisableMethodSameSignature() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer12), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer12), "DisableSomeActionTwo");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpDisableMethodSignatureChoice() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer12), "SomeActionFour");
            MethodInfo hideMethodGood = FindMethod(typeof (Customer12), "DisableSomeActionFour", new[] {typeof (int), typeof (int)});
            MethodInfo hideMethodBad = FindMethod(typeof (Customer12), "DisableSomeActionFour");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IDisableForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is DisableForContextFacetViaMethod);
            Assert.AreEqual(hideMethodGood, ((IImperativeFacet) facet).GetMethod());
            Assert.AreNotEqual(hideMethodBad, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpHideMethodDifferentSignature() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer10), "SomeActionThree");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer10), "HideSomeActionThree");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpHideMethodNoParms() {
            MethodInfo actionMethod = FindMethod(typeof (Customer10), "SomeActionOne");
            MethodInfo hideMethod = FindMethod(typeof (Customer10), "HideSomeActionOne");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpHideMethodOverriddingDefault() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer19), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer19), "HideSomeActionTwo");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpHideMethodSameSignature() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer10), "SomeActionTwo");
            MethodInfo hideMethod = FindMethodIgnoreParms(typeof (Customer10), "HideSomeActionTwo");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(hideMethod, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestPickUpHideMethodSignatureChoice() {
            MethodInfo actionMethod = FindMethodIgnoreParms(typeof (Customer10), "SomeActionFour");
            MethodInfo hideMethodGood = FindMethod(typeof (Customer10), "HideSomeActionFour", new[] {typeof (int), typeof (int)});
            MethodInfo hideMethodBad = FindMethod(typeof (Customer10), "HideSomeActionFour");
            facetFactory.Process(actionMethod, MethodRemover, FacetHolder);

            var facet = FacetHolder.GetFacet<IHideForContextFacet>();
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is HideForContextFacetViaMethod);
            Assert.AreEqual(hideMethodGood, ((IImperativeFacet) facet).GetMethod());
            Assert.AreNotEqual(hideMethodBad, ((IImperativeFacet) facet).GetMethod());
        }

        [Test]
        public void TestProvidesDefaultNameForActionButIgnoresAnyNamedAnnotation() {
            MethodInfo method = FindMethod(typeof (Customer1), "AnActionWithNamedAnnotation");
            facetFactory.Process(method, MethodRemover, FacetHolder);
            IFacet facet = FacetHolder.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is INamedFacet);
            var namedFacet = (INamedFacet) facet;
            Assert.AreEqual("An Action With Named Annotation", namedFacet.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}