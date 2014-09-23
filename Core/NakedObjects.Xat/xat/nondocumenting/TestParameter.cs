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
        private readonly INakedObjectActionParameter parameter;
        private readonly ILifecycleManager persistor;
        private INakedObjectAction action;

        public TestParameter(ILifecycleManager persistor, INakedObjectAction action, INakedObjectActionParameter parameter, ITestHasActions owningObject, ITestObjectFactory factory) {
            this.persistor = persistor;
            this.action = action;
            this.parameter = parameter;
            this.owningObject = owningObject;
            this.factory = factory;
        }

        #region ITestParameter Members

        public string Name {
            get { return parameter.GetName(persistor); }
        }

        public INakedObject NakedObject {
            get { return owningObject.NakedObject; }
        }

        public string Title {
            get { throw new NotImplementedException(); }
        }

        public ITestNaked[] GetChoices() {
            return parameter.GetChoices(NakedObject, null, persistor).Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked[] GetCompletions(string autoCompleteParm) {
            return parameter.GetCompletions(NakedObject, autoCompleteParm, persistor).Select(x => factory.CreateTestNaked(x)).ToArray();
        }

        public ITestNaked GetDefault() {
            INakedObject defaultValue = parameter.GetDefault(NakedObject, persistor);
            TypeOfDefaultValue defaultType = parameter.GetDefaultType(NakedObject, persistor);

            if (defaultType == TypeOfDefaultValue.Implicit && defaultValue.Object is Enum) {
                defaultValue = null;
            }

            return factory.CreateTestNaked(defaultValue);
        }

        public ITestParameter AssertIsOptional() {
            Assert.IsTrue(!parameter.IsMandatory, string.Format("Parameter: {0} is mandatory", Name));
            return this;
        }

        public ITestParameter AssertIsMandatory() {
            Assert.IsTrue(parameter.IsMandatory, string.Format("Parameter: {0} is optional", Name));
            return this;
        }

        public ITestParameter AssertIsDescribedAs(string description) {
            Assert.IsTrue(parameter.Description == description, string.Format("Parameter: {0} description: {1} expected: {2}", Name, parameter.Description, description));
            return this;
        }

        public ITestParameter AssertIsNamed(string name) {
            Assert.IsTrue(Name == name, string.Format("Parameter name : {0} expected {1}", Name, name));
            return this;
        }

        #endregion
    }
}