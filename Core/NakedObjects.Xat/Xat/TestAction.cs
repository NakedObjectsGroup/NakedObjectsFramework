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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Xat {
    internal class TestAction : ITestAction {
        private readonly IActionSpec actionSpec;
        private readonly ITestObjectFactory factory;
        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodelManager;
        private readonly ITestHasActions owningObject;
        private readonly ISession session;
        private readonly ITransactionManager transactionManager;

        public TestAction(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IActionSpec actionSpec, ITestHasActions owningObject, ITestObjectFactory factory, INakedObjectManager manager, ITransactionManager transactionManager)
            : this(metamodelManager, session, lifecycleManager, string.Empty, actionSpec, owningObject, factory, manager, transactionManager) {}

        public TestAction(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, string contributor, IActionSpec actionSpec, ITestHasActions owningObject, ITestObjectFactory factory, INakedObjectManager manager, ITransactionManager transactionManager) {
            SubMenu = contributor;
            this.metamodelManager = metamodelManager;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
            this.owningObject = owningObject;
            this.factory = factory;
            this.manager = manager;
            this.transactionManager = transactionManager;
            this.actionSpec = actionSpec;
        }

        #region ITestAction Members

        public string Name {
            get { return actionSpec.Name; }
        }

        public string SubMenu { get; private set; }

        public string LastMessage { get; private set; }

        public ITestParameter[] Parameters {
            get { return actionSpec.Parameters.Select(x => factory.CreateTestParameter(actionSpec, x, owningObject)).ToArray(); }
        }

        public bool MatchParameters(Type[] typestoMatch) {
            if (actionSpec.Parameters.Count() == typestoMatch.Length) {
                int i = 0;
                return actionSpec.Parameters.All(x => x.Spec.IsOfType(metamodelManager.GetSpecification(typestoMatch[i++])));
            }
            return false;
        }

        public ITestObject InvokeReturnObject(params object[] parameters) {
            return (ITestObject) DoInvoke(ParsedParameters(parameters));
        }

        public ITestCollection InvokeReturnCollection(params object[] parameters) {
            return (ITestCollection) DoInvoke(ParsedParameters(parameters));
        }

        public void Invoke(params object[] parameters) {
            DoInvoke(ParsedParameters(parameters));
        }

        public ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters) {
            return (ITestCollection) DoInvoke(page, ParsedParameters(parameters));
        }

        #endregion

        private ITestNaked DoInvoke(int page, params object[] parameters) {
            ResetLastMessage();
            AssertIsValidWithParms(parameters);
            INakedObject[] parameterObjects = parameters.AsTestNakedArray(manager).Select(x => x.NakedObject).ToArray();

            INakedObject[] parms = actionSpec.RealParameters(owningObject.NakedObject, parameterObjects);
            INakedObject target = actionSpec.RealTarget(owningObject.NakedObject);
            INakedObject result = actionSpec.GetFacet<IActionInvocationFacet>().Invoke(target, parms, page, lifecycleManager, metamodelManager, session, manager);

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
            INakedObject[] parameterObjects = parameters.AsTestNakedArray(manager).Select(x => x.NakedObject).ToArray();
            INakedObject result = null;
            try {
                result = actionSpec.Execute(owningObject.NakedObject, parameterObjects);
            }
            catch (ArgumentException e) {
                Assert.IsInstanceOfType(e, typeof (ArgumentException));
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

        #region Asserts

        public ITestAction AssertIsDisabled() {
            ResetLastMessage();
            if (actionSpec.IsVisible(owningObject.NakedObject)) {
                IConsent canUse = actionSpec.IsUsable(owningObject.NakedObject);
                LastMessage = canUse.Reason;
                Assert.IsFalse(canUse.IsAllowed, "Action '" + Name + "' is usable: " + canUse.Reason);
            }
            return this;
        }

        public ITestAction AssertIsEnabled() {
            ResetLastMessage();
            AssertIsVisible();
            IConsent canUse = actionSpec.IsUsable(owningObject.NakedObject);
            LastMessage = canUse.Reason;
            Assert.IsTrue(canUse.IsAllowed, "Action '" + Name + "' is disabled: " + canUse.Reason);
            return this;
        }


        public ITestAction AssertIsInvalidWithParms(params object[] parameters) {
            ResetLastMessage();

            object[] parsedParameters = ParsedParameters(parameters);

            if (actionSpec.IsVisible(owningObject.NakedObject)) {
                IConsent canUse = actionSpec.IsUsable(owningObject.NakedObject);
                LastMessage = canUse.Reason;
                if (canUse.IsAllowed) {
                    INakedObject[] parameterObjects = parsedParameters.AsTestNakedArray(manager).Select(x => x == null ? null : x.NakedObject).ToArray();
                    IConsent canExecute = actionSpec.IsParameterSetValid(owningObject.NakedObject, parameterObjects);
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

            object[] parsedParameters = ParsedParameters(parameters);


            INakedObject[] parameterObjects = parsedParameters.AsTestNakedArray(manager).Select(x => x == null ? null : x.NakedObject).ToArray();
            IConsent canExecute = actionSpec.IsParameterSetValid(owningObject.NakedObject, parameterObjects);
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

            Assert.IsTrue(parameters.Count() == actionSpec.Parameters.Count(), String.Format("Action '{0}' is unusable: wrong number of parameters, got {1}, expect {2}", Name, parameters.Count(), actionSpec.Parameters.Count()));

            int i = 0;
            foreach (IActionParameterSpec parm in actionSpec.Parameters) {
                object value = parameters[i++];

                if (value is string && parm.Spec.IsParseable) {
                    parsedParameters.Add(parm.Spec.GetFacet<IParseableFacet>().ParseTextEntry((string) value, manager).Object);
                }
                else {
                    parsedParameters.Add(value);
                }
            }

            return parsedParameters.ToArray();
        }

        #endregion
    }
}