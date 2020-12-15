// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Menu;

namespace NakedObjects.Xat {
    public class TestMenuItem : ITestMenuItem {
        private readonly ITestObjectFactory factory;
        private readonly IMenuItemImmutable item;
        private readonly ITestHasActions owningObject; //Non-null if this is on an objectMenu

        public TestMenuItem(IMenuItemImmutable item, ITestObjectFactory factory, ITestHasActions owningObject) {
            this.item = item;
            this.factory = factory;
            this.owningObject = owningObject;
        }

        #region ITestMenuItem Members

        public ITestMenuItem AssertNameEquals(string name) {
            Assert.AreEqual(name, item.Name);
            return this;
        }

        public ITestMenuItem AssertIsAction() {
            Assert.IsInstanceOfType(item, typeof(IMenuActionImmutable));
            return this;
        }

        public ITestAction AsAction() {
            AssertIsAction();
            var actionSpecIm = ((IMenuActionImmutable) item).Action;
            if (owningObject == null) {
                return factory.CreateTestActionOnService(actionSpecIm);
            }

            return factory.CreateTestAction(actionSpecIm, owningObject);
        }

        public ITestMenuItem AssertIsSubMenu() {
            Assert.IsInstanceOfType(item, typeof(IMenuImmutable));
            return this;
        }

        public ITestMenu AsSubMenu() {
            AssertIsSubMenu();
            var menu = item as IMenuImmutable;
            return factory.CreateTestMenuForObject(menu, owningObject);
        }

        #endregion
    }
}