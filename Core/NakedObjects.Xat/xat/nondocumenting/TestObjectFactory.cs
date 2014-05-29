// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Xat {
    public class TestObjectFactory : ITestObjectFactory {
        #region ITestObjectFactory Members

        public ITestService CreateTestService(Object service) {
            return new TestService(service, this);
        }

        public ITestCollection CreateTestCollection(INakedObject instances) {
            return new TestCollection(instances, this);
        }

        public ITestObject CreateTestObject(INakedObject nakedObject) {
            return new TestObject(nakedObject, this);
        }

        public ITestNaked CreateTestNaked(INakedObject nakedObject) {
            if (nakedObject == null) {
                return null;
            }
            if (nakedObject.Specification.IsParseable) {
                return CreateTestValue(nakedObject);
            }
            if (nakedObject.Specification.IsObject) {
                return CreateTestObject(nakedObject);
            }
            if (nakedObject.Specification.IsCollection) {
                return CreateTestCollection(nakedObject);
            }

            return null;
        }

        public ITestAction CreateTestAction(INakedObjectAction action, ITestHasActions owningObject) {
            return new TestAction(action, owningObject, this);
        }

        public ITestAction CreateTestAction(string contributor, INakedObjectAction action, ITestHasActions owningObject) {
            return new TestAction(contributor, action, owningObject, this);
        }

        public ITestProperty CreateTestProperty(INakedObjectAssociation field, ITestHasActions owningObject) {
            return new TestProperty(field, owningObject, this);
        }

        public ITestParameter CreateTestParameter(INakedObjectAction action, INakedObjectActionParameter parameter, ITestHasActions owningObject) {
            return new TestParameter(action, parameter, owningObject, this);
        }

        #endregion

        private static ITestValue CreateTestValue(INakedObject nakedObject) {
            return new TestValue(nakedObject);
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}