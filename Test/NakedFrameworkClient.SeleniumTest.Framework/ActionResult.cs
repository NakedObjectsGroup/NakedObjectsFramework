using OpenQA.Selenium;
using System;
using System.Linq;

namespace NakedFrameworkClient.TestFramework
{
    public abstract class ActionResult : View
    {
        protected ActionResult(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) {}

        public virtual ActionResult AssertTitleIs(string title) => throw new NotImplementedException();

        public Menu OpenActions()
        {
            IWebElement menu = null;
            //test that actions are not already open
            if (element.FindElements(By.CssSelector("nof-action-list")).Any())
            {
                menu = element.FindElement(By.CssSelector("nof-action-list"));
            }
            else
            {
                helper.OpenObjectActions(pane);
                menu = helper.WaitForChildElement(element, "nof-action-list");
            }
            return new Menu(menu, helper, this);
        }

        public virtual ActionResult ClickReload() => throw new NotImplementedException();

        public string GetTitle() => helper.WaitForChildElement(element, ".title").Text;
    }
}