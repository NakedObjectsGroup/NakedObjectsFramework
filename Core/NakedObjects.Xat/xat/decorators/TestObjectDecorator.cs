// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Xat {
    public abstract class TestObjectDecorator : ITestObject {
        private readonly ITestObject wrappedObject;

        protected TestObjectDecorator(ITestObject wrappedObject) {
            this.wrappedObject = wrappedObject;
        }

        #region ITestObject Members

        public INakedObject NakedObject {
            get { throw new NotImplementedException(); }
        }

        public string Title {
            get { throw new NotImplementedException(); }
        }

        public ITestAction[] Actions {
            get { throw new NotImplementedException(); }
        }

        public ITestAction GetAction(string name) {
            throw new NotImplementedException();
        }

        public ITestAction GetAction(string name, params Type[] parameterTypes) {
            throw new NotImplementedException();
        }

        public ITestAction GetAction(string name, string subMenu) {
            throw new NotImplementedException();
        }

        public ITestAction GetAction(string name, string subMenu, params Type[] parameterTypes) {
            throw new NotImplementedException();
        }

        [Obsolete("Use GetAction, specifying contributor as the 'subMenu' parameter")]
        public ITestAction GetContributedAction(string name, string contributor) {
            throw new NotImplementedException();
        }

        [Obsolete("Use GetAction, specifying contributor as the 'subMenu' parameter")]
        public ITestAction GetContributedAction(string name, string contributor, params Type[] parameterTypes) {
            throw new NotImplementedException();
        }

        public string GetObjectActionOrder() {
            throw new NotImplementedException();
        }

        public ITestHasActions AssertActionOrderIs(string order) {
            throw new NotImplementedException();
        }

        public ITestProperty[] Properties {
            get { throw new NotImplementedException(); }
        }

        public string GetPropertyOrder() {
            throw new NotImplementedException();
        }

        public ITestHasProperties AssertPropertyOrderIs(string order) {
            throw new NotImplementedException();
        }

        public ITestObject Save() {
            throw new NotImplementedException();
        }

        public ITestObject Refresh() {
            throw new NotImplementedException();
        }

        public ITestProperty GetPropertyByName(string name) {
            throw new NotImplementedException();
        }

        public ITestProperty GetPropertyById(string id) {
            throw new NotImplementedException();
        }

        public ITestObject AssertCanBeSaved() {
            throw new NotImplementedException();
        }

        public ITestObject AssertCannotBeSaved() {
            throw new NotImplementedException();
        }

        public ITestObject AssertIsTransient() {
            throw new NotImplementedException();
        }

        public ITestObject AssertIsPersistent() {
            throw new NotImplementedException();
        }

        public ITestObject AssertIsImmutable() {
            throw new NotImplementedException();
        }

        public ITestObject AssertIsDescribedAs(string expectedDescription) {
            throw new NotImplementedException();
        }

        public ITestObject AssertTitleEquals(string expectedTitle) {
            throw new NotImplementedException();
        }

        public ITestObject AssertIsType(Type expectedType) {
            throw new NotImplementedException();
        }

        public object GetDomainObject() {
            throw new NotImplementedException();
        }

        public string IconName {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}