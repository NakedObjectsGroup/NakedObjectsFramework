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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;

namespace NakedObjects.Xat {
    internal abstract class TestHasActions : ITestHasActions {
        private readonly ITestObjectFactory factory;

        protected TestHasActions(ITestObjectFactory factory) {
            this.factory = factory;
        }

        protected ITestObjectFactory Factory {
            get { return factory; }
        }

        #region ITestHasActions Members

        public INakedObject NakedObject { get; set; }

        public ITestAction[] Actions {
            get { return NakedObject.Spec.GetObjectActions().Select(x => Factory.CreateTestAction(x, this)).ToArray(); }
        }

        public ITestAction GetAction(string actionName) {
            ITestAction[] actions = Actions.Where(x => x.Name == actionName && string.IsNullOrEmpty(x.SubMenu)).ToArray();
            AssertErrors(actions, actionName);
            return actions.Single();
        }

        public ITestAction GetAction(string actionName, params Type[] parameterTypes) {
            ITestAction[] actions = Actions.Where(x => x.Name == actionName && string.IsNullOrEmpty(x.SubMenu) && x.MatchParameters(parameterTypes)).ToArray();
            AssertErrors(actions, actionName, " (with specified parameters)");
            return actions.Single();
        }

        public ITestAction GetAction(string actionName, string subMenu) {
            return GetMenu().GetSubMenu(subMenu).GetAction(actionName);
        }

        public ITestAction GetAction(string actionName, string subMenu, params Type[] parameterTypes) {
            var action = GetAction(actionName, subMenu);
            Assert.IsTrue(action.MatchParameters(parameterTypes), "Parameter Types do not match for action: " + actionName);
            return action;
        }

        public virtual string GetObjectActionOrder() {
            ITypeSpec spec = NakedObject.Spec;
            IActionSpec[] actionsSpec = spec.GetObjectActions();
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
            IMenuImmutable menu = NakedObject.Spec.ObjectMenu;
            return new TestMenu(menu, Factory, this);
        }

        #endregion

        // ReSharper disable UnusedParameter.Local
        private static void AssertErrors(ITestAction[] actions, string actionName, string condition = "") {
            // ReSharper restore UnusedParameter.Local
            if (!actions.Any()) {
                Assert.Fail("No Action named '{0}'{1}", actionName, condition);
            }
            if (actions.Count() > 1) {
                Assert.Fail("{0} Actions named '{1}' found{2}", actions.Count(), actionName, condition);
            }
        }

        public ITestObject AssertIsDescribedAs(string expected) {
            string description = NakedObject.Spec.Description;
            Assert.IsTrue(expected.Equals(description), "Description expected: '" + expected + "' actual: '" + description + "'");
            return (ITestObject) this;
        }

        public ITestObject AssertIsImmutable() {
            ITypeSpec spec = NakedObject.Spec;
            var facet = spec.GetFacet<IImmutableFacet>();

            bool immutable = facet.Value == WhenTo.Always || facet.Value == WhenTo.OncePersisted && NakedObject.ResolveState.IsPersistent();

            Assert.IsTrue(immutable, "Not immutable");
            return (ITestObject) this;
        }

        public override string ToString() {
            if (NakedObject == null) {
                return base.ToString() + " " + "null";
            }
            return base.ToString() + " " + NakedObject.Spec.ShortName + "/" + NakedObject;
        }

        private string AppendActions(IActionSpec[] actionsSpec) {
            var order = new StringBuilder();
            for (int i = 0; i < actionsSpec.Length; i++) {
                IActionSpec actionSpec = actionsSpec[i];
                string name = actionSpec.Name;
                order.Append(name);
                order.Append(i < actionsSpec.Length - 1 ? ", " : "");
            }
            return order.ToString();
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}