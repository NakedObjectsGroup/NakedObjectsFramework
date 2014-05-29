// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.Xat {
    public class TestServiceDecorator : ITestService {
        private readonly ITestService wrappedObject;

        protected TestServiceDecorator(ITestService wrappedObject) {
            this.wrappedObject = wrappedObject;
        }

        #region ITestService Members

        public INakedObject NakedObject {
            get { return wrappedObject.NakedObject; }
        }

        public string Title {
            get { return wrappedObject.Title; }
        }

        public ITestAction[] Actions {
            get { return wrappedObject.Actions; }
        }

        public ITestAction GetAction(string name) {
            return wrappedObject.GetAction(name);
        }

        public ITestAction GetAction(string name, params Type[] parameterTypes) {
            return wrappedObject.GetAction(name, parameterTypes);
        }

        public ITestAction GetAction(string name, string subMenu)
        {
            return wrappedObject.GetAction(name, subMenu);
        }

        public ITestAction GetAction(string name, string subMenu, params Type[] parameterTypes)
        {
            return wrappedObject.GetAction(name, subMenu, parameterTypes);
        }

        [Obsolete("Use GetAction, specifying contributor as the 'subMenu' parameter")]
        public ITestAction GetContributedAction(string name, string contributor) {
            return wrappedObject.GetAction(name, contributor);
        }

        [Obsolete("Use GetAction, specifying contributor as the 'subMenu' parameter")]
        public ITestAction GetContributedAction(string name, string contributor, params Type[] parameterTypes) {
            return wrappedObject.GetAction(name, contributor, parameterTypes);
        }

        public string GetObjectActionOrder() {
            return wrappedObject.GetObjectActionOrder();
        }

        public ITestHasActions AssertActionOrderIs(string order)
        {
            Assert.AreEqual(order, GetObjectActionOrder());
            return this;
        }


        #endregion

        // Copyright (c) INakedObject Objects Group Ltd.
    }
}