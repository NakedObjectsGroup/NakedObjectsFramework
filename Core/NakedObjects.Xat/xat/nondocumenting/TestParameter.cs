// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Xat {
    internal class TestParameter : ITestParameter {
        private readonly ITestObjectFactory factory;
        private readonly ITestHasActions owningObject;
        private readonly IActionParameterSpec parameterSpec;
        private readonly ILifecycleManager persistor;
        private IActionSpec actionSpec;

        public TestParameter(ILifecycleManager persistor, IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject, ITestObjectFactory factory) {
            this.persistor = persistor;
            this.actionSpec = actionSpec;
            this.parameterSpec = parameterSpec;
            this.owningObject = owningObject;
            this.factory = factory;
        }

        #region ITestParameter Members

        public string Name {
            get { return parameterSpec.GetName(); }
        }

        public INakedObject NakedObject {
            get { return owningObject.NakedObject; }
        }

        public string Title {
            get { throw new NotImplementedException(); }
        }

        public ITestNaked[] GetChoices() {
            return parameterSpec.GetChoices(NakedObject, null).Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked[] GetCompletions(string autoCompleteParm) {
            return parameterSpec.GetCompletions(NakedObject, autoCompleteParm).Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked GetDefault() {
            INakedObject defaultValue = parameterSpec.GetDefault(NakedObject);
            TypeOfDefaultValue defaultType = parameterSpec.GetDefaultType(NakedObject);

            if (defaultType == TypeOfDefaultValue.Implicit && defaultValue.Object is Enum) {
                defaultValue = null;
            }

            return factory.CreateTestNaked(defaultValue);
        }

        public ITestParameter AssertIsOptional() {
            Assert.IsTrue(!parameterSpec.IsMandatory, string.Format("Parameter: {0} is mandatory", Name));
            return this;
        }

        public ITestParameter AssertIsMandatory() {
            Assert.IsTrue(parameterSpec.IsMandatory, string.Format("Parameter: {0} is optional", Name));
            return this;
        }

        public ITestParameter AssertIsDescribedAs(string description) {
            Assert.IsTrue(parameterSpec.Description == description, string.Format("Parameter: {0} description: {1} expected: {2}", Name, parameterSpec.Description, description));
            return this;
        }

        public ITestParameter AssertIsNamed(string name) {
            Assert.IsTrue(Name == name, string.Format("Parameter name : {0} expected {1}", Name, name));
            return this;
        }

        #endregion
    }
}