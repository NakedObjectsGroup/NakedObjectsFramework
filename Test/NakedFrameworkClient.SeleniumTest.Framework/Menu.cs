using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Menu : SubView
    {
        public Menu(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        /// <summary>
        /// Members means action names and submenu names, and should be specified in presented order
        /// </summary>
        public Menu AssertHasMembers(params string[] members) => throw new NotImplementedException();

        public ActionWithDialog GetActionWithDialog(string actionName)
        {
            string actionSelector = $"nof-action input[value=\"{actionName}\"]";
            var act = helper.wait.Until(d => element.FindElement(By.CssSelector(actionSelector)));
            //TODO: test that it generates a dialog - information not currently available in HTML see #292
            return new ActionWithDialog(act, helper, enclosingView);
        }

        public ActionWithoutDialog GetActionWithoutDialog(string actionName) => throw new NotImplementedException();

        public Menu GetSubMenu(string subMenuName) => throw new NotImplementedException();
  
        public void Close() => throw new NotImplementedException();
    }
}