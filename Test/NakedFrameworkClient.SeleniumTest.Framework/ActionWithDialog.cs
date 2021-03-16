using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ActionWithDialog : SubView
    {
        public ActionWithDialog(IWebElement element, Helper helper, View enclosingView) : base(element, helper, enclosingView) { }

        public ActionWithDialog AssertIsEnabled()
        {
            element.AssertIsEnabled();
            return this;
        }

        public Dialog Open()
        {
            var actionName = element.GetAttribute("value");
            helper.Click(element);
            //TODO: Need to find the dialog from the enclosing view - it is not within the menu action
            helper.wait.Until(dr => enclosingView.element.FindElement(By.CssSelector(".dialog .title")).Text == actionName);
            var dialogEl = enclosingView.element.FindElement(By.CssSelector(".dialog"));
            return new Dialog(dialogEl, helper, enclosingView);
        }
    }
}
