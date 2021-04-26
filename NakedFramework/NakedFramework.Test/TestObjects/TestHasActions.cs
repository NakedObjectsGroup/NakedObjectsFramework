// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Resolve;
using NakedFramework.Xat.Interface;

namespace NakedFramework.Xat.TestObjects {
    internal abstract class TestHasActions : ITestHasActions {
        protected TestHasActions(ITestObjectFactory factory) => Factory = factory;

        protected ITestObjectFactory Factory { get; }

        private ITestAction[] ActionsWithFriendlyName(string friendlyName) {
            return Actions.Where(x => x.Name == friendlyName).ToArray();
        }

        private ITestAction[] ActionsForMethodName(string methodName) {
            return NakedObject.Spec.GetActions().Where(
                                  a => a.GetFacet<IActionInvocationFacet>().ActionMethod.Name.Split('.').Last() == methodName)
                              .Select(x => Factory.CreateTestAction(x, this)).ToArray();
        }

        // ReSharper disable UnusedParameter.Local
        private static void AssertErrors(ITestAction[] actions, string actionName, string condition = "") {
            // ReSharper restore UnusedParameter.Local
            if (!actions.Any()) {
                Assert.Fail("No Action named '{0}'{1}", actionName, condition);
            }

            if (actions.Length > 1) {
                Assert.Fail("{0} Actions named '{1}' found{2}", actions.Length, actionName, condition);
            }
        }

        public ITestObject AssertIsDescribedAs(string expected) {
            var description = NakedObject.Spec.Description;
            Assert.IsTrue(expected.Equals(description), $"Description expected: '{expected}' actual: '{description}'");
            return (ITestObject) this;
        }

        public ITestObject AssertIsImmutable() {
            var spec = NakedObject.Spec;
            var facet = spec.GetFacet<IImmutableFacet>();

            var immutable = facet.Value == WhenTo.Always || facet.Value == WhenTo.OncePersisted && NakedObject.ResolveState.IsPersistent();

            Assert.IsTrue(immutable, "Not immutable");
            return (ITestObject) this;
        }

        public override string ToString() {
            if (NakedObject == null) {
                return $"{base.ToString()} null";
            }

            return $"{base.ToString()} {NakedObject.Spec.ShortName}/{NakedObject}";
        }

        private static string AppendActions(IActionSpec[] actionsSpec) {
            var order = new StringBuilder();
            for (var i = 0; i < actionsSpec.Length; i++) {
                var actionSpec = actionsSpec[i];
                var name = actionSpec.Name;
                order.Append(name);
                order.Append(i < actionsSpec.Length - 1 ? ", " : "");
            }

            return order.ToString();
        }

        #region ITestHasActions Members

        public INakedObjectAdapter NakedObject { get; set; }

        public ITestAction[] Actions {
            get { return NakedObject.Spec.GetActions().Select(x => Factory.CreateTestAction(x, this)).ToArray(); }
        }

        public ITestAction GetAction(string friendlyName) {
            var actions = ActionsWithFriendlyName(friendlyName).Where(x => string.IsNullOrEmpty(x.SubMenu)).ToArray();
            AssertErrors(actions, friendlyName);
            return actions.Single();
        }

        public ITestAction GetActionById(string methodName) {
            var actions = ActionsForMethodName(methodName).Where(x => string.IsNullOrEmpty(x.SubMenu)).ToArray();
            AssertErrors(actions, methodName, " (as method name)");
            return actions.Single();
        }

        public ITestAction GetAction(string friendlyName, params Type[] parameterTypes) {
            var actions = ActionsWithFriendlyName(friendlyName).Where(x => string.IsNullOrEmpty(x.SubMenu) && x.MatchParameters(parameterTypes)).ToArray();
            AssertErrors(actions, friendlyName, " (with specified parameters)");
            return actions.Single();
        }

        public ITestAction GetActionById(string methodName, params Type[] parameterTypes) {
            var actions = ActionsForMethodName(methodName).Where(x => string.IsNullOrEmpty(x.SubMenu) && x.MatchParameters(parameterTypes)).ToArray();
            AssertErrors(actions, methodName, " (as method name & with specified parameters)");
            return actions.Single();
        }

        public ITestAction GetAction(string actionName, string subMenu) => GetMenu().GetSubMenu(subMenu).GetAction(actionName);

        public ITestAction GetActionById(string methodName, string subMenu) {
            var action = GetActionById(methodName);
            var friendlyName = action.Name;
            return GetAction(friendlyName, subMenu);
        }

        public ITestAction GetAction(string friendlyName, string subMenu, params Type[] parameterTypes) {
            var action = GetAction(friendlyName, subMenu);
            Assert.IsTrue(action.MatchParameters(parameterTypes), $"Parameter Types do not match for action: {friendlyName}");
            return action;
        }

        public ITestAction GetActionById(string methodName, string subMenu, params Type[] parameterTypes) {
            var action = GetActionById(methodName, subMenu);
            Assert.IsTrue(action.MatchParameters(parameterTypes), $"Parameter Types do not match for action with method name: {methodName}");
            return action;
        }

        public virtual string GetObjectActionOrder() {
            var spec = NakedObject.Spec;
            var actionsSpec = spec.GetActions();
            var order = new StringBuilder();
            order.Append(AppendActions(actionsSpec));
            return order.ToString();
        }

        public ITestHasActions AssertActionOrderIs(string order) {
            Assert.AreEqual(order, GetObjectActionOrder());
            return this;
        }

        public abstract string Title { get; }

        public ITestMenu GetMenu() {
            var menu = NakedObject.Spec.Menu;
            return new TestMenu(menu, Factory, this);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}