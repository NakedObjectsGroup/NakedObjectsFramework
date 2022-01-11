using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public class Menu : SubView
    {
        public Menu(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        /// <summary>
        /// Members means action names and submenu names, and should be specified in presented order
        /// </summary>
        public Menu AssertHasActions(params string[] expectedNames)
        {
            var actualNames = ActionNames();
            CollectionAssert.AreEqual(expectedNames, actualNames);
            return this;
        }

        private string[] ActionNames() =>
            element.FindElements(By.CssSelector("nof-action input")).Select(m => m.GetAttribute("value")).ToArray();

        public Menu AssertHasSubMenus(params string[] subMenuNames)
        {
            CollectionAssert.AreEqual(subMenuNames, SubMenuNames());
            return this;
        }

        private string[] SubMenuNames() =>
      element.FindElements(By.CssSelector(".submenu")).Select(m => m.Text).ToArray();

        public Menu AssertHasSubMenu(string subMenuName)
        {
            Assert.IsTrue(SubMenuNames().Contains(subMenuName));
            return this;
        }

        public Menu AssertHasAction(string memberName)
        {
            Assert.IsTrue(ActionNames().Contains(memberName));
            return this;
        }

        public Menu AssertDoesNotHaveAction(string memberName)
        {
            Assert.IsFalse(ActionNames().Contains(memberName));
            return this;
        }


        public ActionWithDialog GetActionWithDialog(string actionName)
        {
            string actionSelector = $"nof-action input[value=\"{actionName}\"]";
            var act = helper.wait.Until(d => element.FindElement(By.CssSelector(actionSelector)));
            //TODO: test that it generates a dialog - information not currently available in HTML see #292
            return new ActionWithDialog(act, helper, enclosingView);
        }

        public ActionWithoutDialog GetActionWithoutDialog(string actionName)
        {
            string actionSelector = $"nof-action input[value=\"{actionName}\"]";
            var act = helper.wait.Until(d => element.FindElement(By.CssSelector(actionSelector)));
            //TODO: test that it does not generate a dialog - information not currently available in HTML see #292
            return new ActionWithoutDialog(act, helper, enclosingView);
        }

        public Menu OpenSubMenu(string subMenuName)
        {
            var sub = element.FindElements(By.CssSelector(".submenu")).Single(element => element.Text == subMenuName);
            helper.Click(sub);
            var el = helper.WaitForChildElement(element, ".menuitem.open");
            return this;
        }
  
        public void Close() => throw new NotImplementedException();
    }
}