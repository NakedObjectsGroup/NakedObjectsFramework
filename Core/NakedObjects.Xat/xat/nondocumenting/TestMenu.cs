// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NakedObjects.Xat {
    internal class TestMenu : ITestMenu {

        private IMenuImmutable menu;
        private ITestObjectFactory factory;
        private ITestObject owningObject; //May be null if it is a MainMenu

        public TestMenu(IMenuImmutable menu, ITestObjectFactory factory, ITestObject owningObject) {
            this.menu = menu;
            this.factory = factory;
            this.owningObject = owningObject;
        }

        public ITestMenu AssertNameEquals(string name) {
            Assert.AreEqual(name, menu.Name);
            return this;
        }

        public ITestMenu AssertItemCountIs(int count) {
            Assert.AreEqual(count, menu.MenuItems.Count);
            return this;
        }

        public ITestAction GetAction(string name) {
            return GetItem(name).AsAction();
        }

        public ITestMenu GetSubMenu(string name) {
            return GetItem(name).AsSubMenu();
        }

        public ITestMenuItem GetItem(string name) {
            var item = menu.MenuItems.FirstOrDefault(i => i.Name == name);
            Assert.IsNotNull(item, "No menu item with name: "+name);
            return factory.CreateTestMenuItem(item, owningObject);
        }

        public ITestMenuItem[] AllItems() {
            return menu.MenuItems.Select(mi => factory.CreateTestMenuItem(mi, owningObject)).ToArray();
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}