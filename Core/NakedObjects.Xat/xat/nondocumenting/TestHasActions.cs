// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Xat {
    internal abstract class TestHasActions : ITestHasActions {
        protected readonly ITestObjectFactory factory;

        protected TestHasActions(ITestObjectFactory factory) {
            this.factory = factory;
        }

        #region ITestHasActions Members

        public INakedObject NakedObject { get; set; }

        public ITestAction[] Actions {
            get {
                List<ITestAction> actions = NakedObject.Specification.GetObjectActions().
                    OfType<NakedObjectActionImpl>().
                    Select(x => factory.CreateTestAction(x, this)).ToList();

                foreach (NakedObjectActionSet set in NakedObject.Specification.GetObjectActions().OfType<NakedObjectActionSet>()) {
                    actions.AddRange(set.Actions.Select(x => factory.CreateTestAction(set.Name, x, this)));
                }

                return actions.ToArray();
            }
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
            ITestAction[] actions = Actions.Where(x => x.Name == actionName && x.SubMenu == subMenu).ToArray();
            AssertErrors(actions, actionName, string.Format(" within sub-menu '{0}'", subMenu));
            return actions.Single();
        }

        public ITestAction GetAction(string actionName, string subMenu, params Type[] parameterTypes) {
            ITestAction[] actions = Actions.Where(x => x.Name == actionName && x.SubMenu == subMenu && x.MatchParameters(parameterTypes)).ToArray();
            AssertErrors(actions, actionName, string.Format(" (with specified parameters) within sub-menu '{0}'", subMenu));
            return actions.Single();
        }

        public virtual string GetObjectActionOrder() {
            INakedObjectSpecification specification = NakedObject.Specification;
            INakedObjectAction[] actions = specification.GetObjectActions();
            var order = new StringBuilder();
            order.Append(AppendActions(actions));
            return order.ToString();
        }

        public ITestHasActions AssertActionOrderIs(string order) {
            Assert.AreEqual(order, GetObjectActionOrder());
            return this;
        }

        public abstract string Title { get; }

        #endregion

        private static void AssertErrors(ITestAction[] actions, string actionName, string condition = "") {
            if (!actions.Any()) {
                Assert.Fail(string.Format("No Action named '{0}'{1}", actionName, condition));
            }
            if (actions.Count() > 1) {
                Assert.Fail(string.Format("{0} Actions named '{1}' found{2}", actions.Count(), actionName, condition));
            }
        }

        public ITestObject AssertIsDescribedAs(string expected) {
            string description = NakedObject.Specification.Description;
            Assert.IsTrue(expected.Equals(description), "Description expected: '" + expected + "' actual: '" + description + "'");
            return (ITestObject) this;
        }

        public ITestObject AssertIsImmutable() {
            INakedObjectSpecification specification = NakedObject.Specification;
            var facet = specification.GetFacet<IImmutableFacet>();

            bool immutable = facet.Value == WhenTo.Always || facet.Value == WhenTo.OncePersisted && NakedObject.ResolveState.IsPersistent();

            Assert.IsTrue(immutable, "Not immutable");
            return (ITestObject) this;
        }

        public override string ToString() {
            if (NakedObject == null) {
                return base.ToString() + " " + "null";
            }
            return base.ToString() + " " + NakedObject.Specification.ShortName + "/" + NakedObject;
        }

        private static string AppendActions(INakedObjectAction[] actions) {
            var order = new StringBuilder();
            for (int i = 0; i < actions.Length; i++) {
                INakedObjectAction action = actions[i];
                string name = action.Name;
                if (action is NakedObjectActionSet) {
                    order.Append("(").Append(name).Append(":");
                    order.Append(AppendActions(action.Actions));
                    order.Append(")");
                }
                else {
                    order.Append(name);
                }

                order.Append(i < actions.Length - 1 ? ", " : "");
            }
            return order.ToString();
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}