using NakedObjects.Architecture.Menu;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Xat {
    public class TestMenuItem : ITestMenuItem {
        private IMenuItemImmutable item;
        private ITestObjectFactory factory;
        private ITestObject owningObject; //Non-null if this is on an objectMenu

        public TestMenuItem(IMenuItemImmutable item, ITestObjectFactory factory, ITestObject owningObject) {
            this.item = item;
            this.factory = factory;
            this.owningObject = owningObject;
        }
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
            IActionSpecImmutable actionSpecIm = (item as IMenuActionImmutable).Action;
            if (this.owningObject == null) { 
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
            var menu = (item as IMenuImmutable);
            return factory.CreateTestMenuMain(menu);
        }
    }
}
