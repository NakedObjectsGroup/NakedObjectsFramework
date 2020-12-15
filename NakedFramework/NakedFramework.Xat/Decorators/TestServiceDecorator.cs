// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Xat {
    public class TestServiceDecorator : ITestService {
        private readonly ITestService wrappedObject;

        protected TestServiceDecorator(ITestService wrappedObject) => this.wrappedObject = wrappedObject;

        #region ITestService Members

        public INakedObjectAdapter NakedObject => wrappedObject.NakedObject;

        public string Title => wrappedObject.Title;

        public ITestAction[] Actions => wrappedObject.Actions;

        public ITestAction GetAction(string friendlyName) => wrappedObject.GetAction(friendlyName);

        public ITestAction GetActionById(string methodName) => wrappedObject.GetActionById(methodName);

        public ITestAction GetAction(string friendlyName, params Type[] parameterTypes) => wrappedObject.GetAction(friendlyName, parameterTypes);

        public ITestAction GetActionById(string methodName, params Type[] parameterTypes) => wrappedObject.GetActionById(methodName, parameterTypes);

        public ITestAction GetAction(string friendlyName, string subMenu) => wrappedObject.GetAction(friendlyName, subMenu);

        public ITestAction GetActionById(string methodName, string subMenu) => wrappedObject.GetActionById(methodName, subMenu);

        public ITestAction GetAction(string friendlyName, string subMenu, params Type[] parameterTypes) => wrappedObject.GetAction(friendlyName, subMenu, parameterTypes);

        public ITestAction GetActionById(string methodName, string subMenu, params Type[] parameterTypes) => wrappedObject.GetActionById(methodName, subMenu, parameterTypes);

        public string GetObjectActionOrder() => wrappedObject.GetObjectActionOrder();

        public ITestHasActions AssertActionOrderIs(string order) {
            Assert.AreEqual(order, GetObjectActionOrder());
            return this;
        }

        public ITestMenu GetMenu() => wrappedObject.GetMenu();

        #endregion
    }
}