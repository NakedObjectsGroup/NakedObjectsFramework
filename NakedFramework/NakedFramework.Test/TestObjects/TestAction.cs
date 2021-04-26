// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Xat.Interface;
using NakedFramework.Xat.TestCase;

namespace NakedFramework.Xat.TestObjects {
    internal class TestAction : ITestAction {
        private readonly IActionSpec actionSpec;
        private readonly ITestObjectFactory factory;
        private readonly INakedObjectsFramework framework;
        private readonly ITestHasActions owningObject;

        public TestAction(INakedObjectsFramework framework, IActionSpec actionSpec, ITestHasActions owningObject, ITestObjectFactory factory)
            : this(framework, string.Empty, actionSpec, owningObject, factory) { }

        public TestAction(INakedObjectsFramework framework, string contributor, IActionSpec actionSpec, ITestHasActions owningObject, ITestObjectFactory factory) {
            SubMenu = contributor;
            this.framework = framework;
            this.owningObject = owningObject;
            this.factory = factory;
            this.actionSpec = actionSpec;
        }

        private ITestNaked DoInvoke(int page, params object[] parameters) {
            ResetLastMessage();
            AssertIsValidWithParms(parameters);
            var parameterObjectsAdapter = parameters.AsTestNakedArray(framework.NakedObjectManager).Select(x => x.NakedObject).ToArray();

            var parms = actionSpec.RealParameters(owningObject.NakedObject, parameterObjectsAdapter);
            var target = actionSpec.RealTarget(owningObject.NakedObject);
            var result = actionSpec.GetFacet<IActionInvocationFacet>().Invoke(target, parms, page, framework);

            if (result == null) {
                return null;
            }

            if (result.Spec.IsCollection) {
                return factory.CreateTestCollection(result);
            }

            return factory.CreateTestObject(result);
        }

        private ITestNaked DoInvoke(params object[] parameters) {
            ResetLastMessage();
            AssertIsValidWithParms(parameters);
            var parameterObjectsAdapter = parameters.AsTestNakedArray(framework.NakedObjectManager).Select(x => x.NakedObject).ToArray();
            INakedObjectAdapter result = null;
            try {
                result = actionSpec.Execute(owningObject.NakedObject, parameterObjectsAdapter);
            }
            catch (ArgumentException) {
                Assert.Fail("Invalid Argument(s)");
            }
            catch (InvalidCastException) {
                Assert.Fail("Invalid Argument(s)");
            }

            if (result == null) {
                return null;
            }

            if (result.Spec.IsCollection && !result.Spec.IsParseable) {
                return factory.CreateTestCollection(result);
            }

            return factory.CreateTestObject(result);
        }

        private void ResetLastMessage() {
            LastMessage = string.Empty;
        }

        #region ITestAction Members

        public string Name => actionSpec.Name;

        public string SubMenu { get; }
        public string LastMessage { get; private set; }

        public ITestParameter[] Parameters {
            get { return actionSpec.Parameters.Select(x => factory.CreateTestParameter(actionSpec, x, owningObject)).ToArray(); }
        }

        public bool MatchParameters(Type[] typestoMatch) {
            if (actionSpec.Parameters.Length == typestoMatch.Length) {
                var i = 0;
                return actionSpec.Parameters.All(x => x.Spec.IsOfType(framework.MetamodelManager.GetSpecification(typestoMatch[i++])));
            }

            return false;
        }

        public ITestObject InvokeReturnObject(params object[] parameters) {
            try {
                framework.TransactionManager.StartTransaction();
                return (ITestObject) DoInvoke(ParsedParameters(parameters));
            }
            finally {
                framework.TransactionManager.EndTransaction();
            }
        }

        public ITestCollection InvokeReturnCollection(params object[] parameters) {
            try {
                framework.TransactionManager.StartTransaction();
                return (ITestCollection) DoInvoke(ParsedParameters(parameters));
            }
            finally {
                framework.TransactionManager.EndTransaction();
            }
        }

        public void Invoke(params object[] parameters) {
            try {
                framework.TransactionManager.StartTransaction();
                DoInvoke(ParsedParameters(parameters));
            }
            finally {
                framework.TransactionManager.EndTransaction();
            }
        }

        public ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters) {
            try {
                framework.TransactionManager.StartTransaction();
                return (ITestCollection) DoInvoke(page, ParsedParameters(parameters));
            }
            finally {
                framework.TransactionManager.EndTransaction();
            }
        }

        #endregion

        #region Asserts

        public ITestAction AssertIsDisabled() {
            ResetLastMessage();
            if (actionSpec.IsVisible(owningObject.NakedObject)) {
                var canUse = actionSpec.IsUsable(owningObject.NakedObject);
                LastMessage = canUse.Reason;
                Assert.IsFalse(canUse.IsAllowed, "Action '" + Name + "' is usable: " + canUse.Reason);
            }

            return this;
        }

        public ITestAction AssertIsEnabled() {
            ResetLastMessage();
            AssertIsVisible();
            var canUse = actionSpec.IsUsable(owningObject.NakedObject);
            LastMessage = canUse.Reason;
            Assert.IsTrue(canUse.IsAllowed, "Action '" + Name + "' is disabled: " + canUse.Reason);
            return this;
        }

        public ITestAction AssertIsInvalidWithParms(params object[] parameters) {
            ResetLastMessage();

            var parsedParameters = ParsedParameters(parameters);

            if (actionSpec.IsVisible(owningObject.NakedObject)) {
                var canUse = actionSpec.IsUsable(owningObject.NakedObject);
                LastMessage = canUse.Reason;
                if (canUse.IsAllowed) {
                    var parameterObjectsAdapter = parsedParameters.AsTestNakedArray(framework.NakedObjectManager).Select(x => x?.NakedObject).ToArray();
                    var canExecute = actionSpec.IsParameterSetValid(owningObject.NakedObject, parameterObjectsAdapter);
                    LastMessage = canExecute.Reason;
                    Assert.IsFalse(canExecute.IsAllowed, "Action '" + Name + "' is usable and executable");
                }
            }

            return this;
        }

        public ITestAction AssertIsValidWithParms(params object[] parameters) {
            ResetLastMessage();
            AssertIsVisible();
            AssertIsEnabled();

            var parsedParameters = ParsedParameters(parameters);

            var parameterObjectsAdapter = parsedParameters.AsTestNakedArray(framework.NakedObjectManager).Select(x => x?.NakedObject).ToArray();
            var canExecute = actionSpec.IsParameterSetValid(owningObject.NakedObject, parameterObjectsAdapter);
            Assert.IsTrue(canExecute.IsAllowed, "Action '" + Name + "' is unusable: " + canExecute.Reason);
            return this;
        }

        public ITestAction AssertIsVisible() {
            ResetLastMessage();
            Assert.IsTrue(actionSpec.IsVisible(owningObject.NakedObject), "Action '" + Name + "' is hidden");
            return this;
        }

        public ITestAction AssertIsInvisible() {
            ResetLastMessage();
            Assert.IsFalse(actionSpec.IsVisible(owningObject.NakedObject), "Action '" + Name + "' is visible");
            return this;
        }

        public ITestAction AssertIsDescribedAs(string expected) {
            Assert.IsTrue(expected.Equals(actionSpec.Description), "Description expected: '" + expected + "' actual: '" + actionSpec.Description + "'");
            return this;
        }

        public ITestAction AssertLastMessageIs(string message) {
            Assert.IsTrue(message.Equals(LastMessage), "Last message expected: '" + message + "' actual: '" + LastMessage + "'");
            return this;
        }

        public ITestAction AssertLastMessageContains(string message) {
            Assert.IsTrue(LastMessage.Contains(message), "Last message expected to contain: '" + message + "' actual: '" + LastMessage + "'");
            return this;
        }

        private object[] ParsedParameters(params object[] parameters) {
            var parsedParameters = new List<object>();

            Assert.IsTrue(parameters.Length == actionSpec.Parameters.Length, $"Action '{Name}' is unusable: wrong number of parameters, got {parameters.Length}, expect {actionSpec.Parameters.Length}");

            var i = 0;
            foreach (var parm in actionSpec.Parameters) {
                var value = parameters[i++];

                var valueAsString = value as string;
                if (valueAsString != null && parm.Spec.IsParseable) {
                    parsedParameters.Add(parm.Spec.GetFacet<IParseableFacet>().ParseTextEntry(valueAsString, framework.NakedObjectManager).Object);
                }
                else {
                    parsedParameters.Add(value);
                }
            }

            return parsedParameters.ToArray();
        }

        public ITestAction AssertHasFriendlyName(string friendlyName) {
            Assert.AreEqual(friendlyName, Name);
            return this;
        }

        #endregion
    }
}